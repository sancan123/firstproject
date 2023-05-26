using System;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services.Description;
using Microsoft.CSharp;
using System.Collections;
using System.Web.Services.Protocols;

namespace Common
{
    public class WebserviceHelper
    {
        #region InvokeWebService//动态调用web服务
        public static object InvokeWebService(string url, string methodname, object[] args)
        {
            return InvokeWebService(url, null, methodname, args);
        }
        public static object InvokeWebService(string url, string classname, string methodname, object[] args)
        {
            string @namespace = "EnterpriseServerBase.WebService.DynamicWebCalling";
            if ((classname == null) || (classname == ""))
            {
                //classname = GetWsClassName(url);
            }

            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = provider.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    //报错
                }
                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }
        #endregion



        // 用Dictionary保存已经反射生成过的WebService和Method，省去每次调用都重新生成实例
        #region 动态调用WebService

        /// <summary>
        /// WebService服务列表
        /// </summary>
        private static Dictionary<string, object> ServiceDictionary = new Dictionary<string, object>();

        /// <summary>
        /// WebService Method方法列表
        /// </summary>
        private static Dictionary<string, System.Reflection.MethodInfo> MethodDictionary = new Dictionary<string, System.Reflection.MethodInfo>();


        /// <summary>
        /// 动态调用WebService（开发环境ping不通需要调用的WebService地址时使用）
        /// </summary>
        /// <param name="url">webservice地址</param>
        /// <param name="methodName">需要调用的方法名称</param>
        /// <param name="param">调用上方法时传入的参数</param>
        /// <returns>执行方法后返回的数据</returns>
        public static object InvokeWebService2(string url, string methodName, string[] param)
        {
            // 检查是否已经存在WebService服务 和 方法
            if (!ServiceDictionary.ContainsKey(url + methodName) || !MethodDictionary.ContainsKey(url + methodName))
            {
                //客户端代理服务命名空间，可以设置成需要的值。
                string space = string.Format("ProxyServiceReference");

                //获取WSDL
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(url + "?WSDL");
                ServiceDescription description = ServiceDescription.Read(stream);//服务的描述信息都可以通过ServiceDescription获取
                string classname = description.Services[0].Name;

                ServiceDescriptionImporter descriptionImporter = new ServiceDescriptionImporter();
                descriptionImporter.AddServiceDescription(description, "", "");
                CodeNamespace codeNamespace = new CodeNamespace(space);

                //生成客户端代理类代码
                CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
                codeCompileUnit.Namespaces.Add(codeNamespace);
                descriptionImporter.Import(codeNamespace, codeCompileUnit);
                CSharpCodeProvider provider = new CSharpCodeProvider();

                //设定编译参数
                CompilerParameters comilerParameters = new CompilerParameters();
                comilerParameters.GenerateExecutable = false;
                comilerParameters.GenerateInMemory = true;
                comilerParameters.ReferencedAssemblies.Add("System.dll");
                comilerParameters.ReferencedAssemblies.Add("System.XML.dll");
                comilerParameters.ReferencedAssemblies.Add("System.Web.Services.dll");
                comilerParameters.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults compilerResult = provider.CompileAssemblyFromDom(comilerParameters, codeCompileUnit);
                if (compilerResult.Errors.HasErrors == true)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in compilerResult.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = compilerResult.CompiledAssembly;
                Type service = assembly.GetType(space + "." + classname, true, true);

                if (!ServiceDictionary.ContainsKey(url+ methodName))
                {
                    object SSOService = Activator.CreateInstance(service);
                    ServiceDictionary.Add(url+ methodName, SSOService);
                }

                System.Reflection.MethodInfo SSOServiceMethod = service.GetMethod(methodName);
                MethodDictionary.Add(url + methodName, SSOServiceMethod);
            }
            //object ob = ServiceDictionary[url];
            return MethodDictionary[url + methodName].Invoke(ServiceDictionary[url+ methodName],param);

        }

        #endregion


        private static Hashtable WebServiceInstance = new Hashtable();



        /// <summary>
        /// 动态生成代理类调用WebService,以支持java与.net生成的wsdl
        /// </summary>
        /// <param name="url">调用的服务地址</param>
        /// <param name="classname"></param>
        /// <param name="methodname"></param>
        /// <param name="args">传参</param>
        /// <param name="wsInstance">代理类对象缓存</param>
        /// <returns>
        /// 服务调用返回的结果
        /// </returns>
        public static object InvokeWebService3(string url, string classname, string methodname, object[] args, Hashtable wsInstance)
        {
            string @namespace = "ProxyServiceReference";
            //wsInstance = new Hashtable();
            object obj = new object();
            if (!wsInstance.ContainsKey(url))
            {
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(url + "?WSDL");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;

                System.Type[] types = assembly.GetTypes();
                foreach (System.Type t in types)
                {
                    if (t.BaseType == typeof(System.Web.Services.Protocols.SoapHttpClientProtocol))
                    {

                        obj = Activator.CreateInstance(t);
                        ((SoapHttpClientProtocol)obj).Timeout = int.MaxValue;
                        if (!wsInstance.Contains(url))
                            wsInstance.Add(url, obj);
                        break;
                    }
                }
            }
            else
                obj = wsInstance[url] as object;
            System.Reflection.MethodInfo mi = obj.GetType().GetMethod(methodname);
            return mi.Invoke(obj, args);
        }


    }
}
