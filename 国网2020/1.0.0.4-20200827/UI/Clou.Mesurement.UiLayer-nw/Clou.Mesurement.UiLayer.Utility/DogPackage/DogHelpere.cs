////////////////////////////////////////////////////////////////////
// Demo program for SuperDog licensing functions
//
// Copyright (C) 2013 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc.
//
////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using SuperDog;
using System.IO;
using System.Xml;
using System.Threading;

namespace Mesurement.UiLayer.Utility.DogPackage
{
    /// <summary>
    /// 获取超级狗信息,解析密文获取明文,将明文与超级狗中的字符串进行对比,如果一致则人为超级狗存在
    /// </summary>
    public class DogHelper
    {
        private static DogHelper instance = null;
        public static DogHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DogHelper();
                }
                return instance;
            }
        }

        #region 用到的数组
        private string codeValue = "";
        /// <summary>
        /// 加密狗对应的密文
        /// </summary>
        private string CodeValue
        {
            get
            {
                if (string.IsNullOrEmpty(codeValue))
                {
                    string fileName = string.Format(@"{0}\DogFile\{1}", Directory.GetCurrentDirectory(), "License.dat");
                    if (File.Exists(fileName))
                    {
                        codeValue = File.ReadAllText(fileName);
                    }
                }
                return codeValue;
            }
        }
        string scope = "<dogscope />";
        //密文
        //CL3000S-H_V4
        private byte[] encryptArr = new byte[16]{0x1D, 0x46, 0x97, 0x6B, 0x01, 0xA6, 0xB6, 0x81, 0x06, 0xFE, 0x33, 0x49, 0xF6, 0x2A, 0xC0, 0xF3
 };
        #endregion

        #region 异常码解析
        /// <summary>
        /// 获取状态对应的信息
        /// </summary>
        /// <param name="statusTemp"></param>
        /// <returns></returns>
        private string GetErrorMessage(DogStatus statusTemp)
        {
            switch (statusTemp)
            {
                case DogStatus.StatusOk:
                    return "操作成功";
                case DogStatus.InvalidAddress:
                    return "请求超出数据文件的范围";
                case DogStatus.NotEnoughMemory:
                    return "系统内存不足";
                case DogStatus.TooManyOpenFeatures:
                    return "打开的登录会话数目过多";
                case DogStatus.AccessDenied:
                    return "访问被拒绝";
                case DogStatus.DogNotFound:
                    return "未找到所需的超级狗";
                case DogStatus.BufferTooShort:
                    return "加密/解密的数据长度太短";
                case DogStatus.InvalidHandle:
                    return "输入函数的句柄无效";
                case DogStatus.InvalidFile:
                    return "无法识别文件标识符";
                case DogStatus.InvalidFormat:
                    return "无效的XML格式";
                case DogStatus.KeyIdNotFound:
                    return "未找到待升级的超级狗";
                case DogStatus.InvalidUpdateData:
                    return "未找到所需的XML标记，或者二进制数据的内容已丢失或无效";
                case DogStatus.UpdateNotSupported:
                    return "该超级狗不支持升级请求";
                case DogStatus.InvalidUpdateCounter:
                    return "升级计数器设置不正确";
                case DogStatus.InvalidVendorCode:
                    return "输入的开发商代码无效";
                case DogStatus.InvalidTime:
                    return "输入的时间值超出被支持的数值范围";
                case DogStatus.UpdateNoAckSpace:
                    return "升级要求回执数据，但输入参数ack_data为NULL";
                case DogStatus.TerminalServiceDetected:
                    return "程序在终端服务器上运行";
                case DogStatus.UnknownAlgorithm:
                    return "V2C文件中使用了未知算法";
                case DogStatus.InvalidSignature:
                    return "签名验证失败";
                case DogStatus.FeatureNotFound:
                    return "特征不可用";
                case DogStatus.LocalCommErr:
                    return "API和超级狗运行环境（License Manager）通讯错误";
                case DogStatus.UnknownVcode:
                    return "API不识别开发商代码";
                case DogStatus.InvalidXmlSpec:
                    return "无效的XML格式";
                case DogStatus.InvalidXmlScope:
                    return "无效的XML范围";
                case DogStatus.TooManyKeys:
                    return "当前连接的超级狗数目过多";
                case DogStatus.BrokenSession:
                    return "会话被中断";
                case DogStatus.FeatureExpired:
                    return "特征已失效";
                case DogStatus.TooOldLM:
                    return "超级狗的运行环境版本太旧";
                case DogStatus.DeviceError:
                    return "与超级狗通讯中出现USB通信错误";
                case DogStatus.TimeError:
                    return "系统时钟已被篡改";
                case DogStatus.SecureChannelError:
                    return "安全通道中发生了通信错误";
                case DogStatus.EmptyScopeResults:
                    return "超级狗软许可数据被破坏";
                case DogStatus.UpdateTooOld:
                    return "文件中的升级计数器的数值小于超级狗中的升级计数器的数值，不允许安装V2C文件";
                case DogStatus.UpdateTooNew:
                    return "文件中的升级计数器的数值大于超级狗中的升级计数器的数值，不允许安装V2C文件";
                case DogStatus.NoApiDylib:
                    return "未找到API的动态库";
                case DogStatus.InvApiDylib:
                    return "API 的动态库无效";
                case DogStatus.InvalidObject:
                    return "对象的初始化不正确";
                case DogStatus.InvalidParameter:
                    return "无效的函数参数";
                case DogStatus.AlreadyLoggedIn:
                    return "两次登录到同一对象";
                case DogStatus.AlreadyLoggedOut:
                    return "从同一对象注销两次";
                case DogStatus.OperationFailed:
                    return "系统或平台的使用不正确";
                case DogStatus.NetDllBroken:
                    return "加密狗动态库文件损坏";
                case DogStatus.NotImplemented:
                    return "未实施所请求的功能";
                case DogStatus.InternalError:
                    return "API中内部错误";
                //case DogStatus.NextFreeValues:
                //    return "";
                default:
                    return "未知错误";
            }
        }
        #endregion

        #region 初始化和判断加密狗存在
        private XmlDocument doc = new XmlDocument();
        private string dogId = "";
        /// <summary>
        /// 文件编号
        /// </summary>
        private int fileId = 0xfff4;
        /// <summary>
        /// 初始化加密机信息:获取加密狗编号和文件编号
        /// </summary>
        /// <param name="errorString"></param>
        /// <returns></returns>
        private bool Initialize(out string errorString)
        {
            Dog curDog = new Dog();
            errorString = "";
            DogStatus status;
            try
            {
                status = curDog.Login(CodeValue, scope);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("登录到加密狗失败:{0}", GetErrorMessage(status));
                    return false;
                }
                string xmlString = "";
                status = curDog.GetSessionInfo(Dog.KeyInfo, ref xmlString);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("获取加密狗基本信息失败:{0}", GetErrorMessage(status));
                    return false;
                }
                doc.LoadXml(xmlString);
                XmlNode nodeTemp = doc.DocumentElement;
                XmlNode nodeDogId = nodeTemp.SelectSingleNode(@"//dogid");
                dogId = nodeDogId.InnerText;
                XmlNode nodeFileId = nodeTemp.SelectSingleNode(@"//fileid");
                int.TryParse(nodeFileId.InnerText, out fileId);
                return true;
            }
            catch (Exception e)
            {
                errorString = string.Format("初始化加密狗失败:{0}", e.Message);
                return false;
            }
            finally
            {
                curDog.Logout();
            }
        }

        /// <summary>
        /// 判断加密狗在不在
        /// </summary>
        /// <returns></returns>
        private bool CheckKey(out string errorString)
        {
            errorString = "";
            DogStatus status;
            Dog curDog = new Dog(DogFeature.Default);
            try
            {
                #region 登录
                status = curDog.Login(CodeValue, scope);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("登录到加密狗失败:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region  获取加密狗文件信息
                DogFile fileTemp1 = curDog.GetFile(fileId);
                int fileSize = 0x80;
                status = fileTemp1.FileSize(ref fileSize);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("登录到加密狗失败:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region 读取加密狗内存中的内容
                byte[] bufData = new byte[fileSize];
                status = fileTemp1.Read(bufData, 0, fileSize);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("登录到加密狗失败:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region 利用加密狗解析加密数组,获取明文
                byte[] bufTemp = new byte[encryptArr.Length];
                for (int i = 0; i < encryptArr.Length; i++)
                {
                    bufTemp[i] = encryptArr[i];
                }
                status = curDog.Decrypt(bufTemp);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("登录到加密狗失败:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region 将内存中的内容与明文数据对比,如果一致则验证成功
                for (int i = 0; i < 12; i++)
                {
                    if (bufTemp[i] != bufData[10 + i])
                    {
                        return false;
                    }
                }
                #endregion
                return true;
            }
            catch (Exception e)
            {
                errorString = string.Format("验证加密狗出现异常:{0}", e.Message);
                return false;
            }
            finally
            {
                curDog.Logout();
            }
        }
        #endregion

        private void ThreadProcess()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(10000);
                    string errorString = "";
                    //如果查询三次都不能查找到加密狗,程序退出
                    //每次查询加密狗时间间隔为10秒
                    for (int i = 0; i < 3; i++)
                    {
                        if (!CheckKey(out errorString))
                        {
                            if (EventLostDog != null)
                            {

                                EventLostDog(this, new DogEventArgs(i == 2, errorString));
                            }
                            //找不到加密狗退出线程
                            if (i == 2)
                            {
                                return;
                            }
                            Thread.Sleep(10000);
                        }
                        //找到加密狗则不执行循环
                        else
                        {
                            break;
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private Thread threadDog = null;
        /// <summary>
        /// 开启加密狗扫描
        /// </summary>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public bool Run(out string errorString)
        {
            errorString = "";
            if (threadDog == null || threadDog.ThreadState != ThreadState.Running)
            {
                if (!Initialize(out errorString))
                {
                    return false;
                }
                threadDog = new Thread(() => ThreadProcess());
                threadDog.Start();
            }
            return true;
        }
        public event EventHandler EventLostDog;
        /// <summary>
        /// 停止加密狗扫描
        /// </summary>
        public void Stop()
        {
            if (threadDog != null && threadDog.ThreadState == ThreadState.Running)
            {
                threadDog.Abort();
            }
        }
    }
}