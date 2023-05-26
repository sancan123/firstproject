using System;
using Mesurement.UiLayer.VerifyService;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.VerifyService.WcfDynamic;
using System.Reflection;
using Mesurement.UiLayer.DAL.Config;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.ViewModel.WcfService
{
    /// <summary>
    /// 
    /// </summary>
    public class WcfHelper
    {
        private static WcfHelper instance;
        /// 单例
        /// <summary>
        /// 单例
        /// </summary>
        public static WcfHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WcfHelper();
                }
                return instance;
            }
        }
        /// 创建检定消息服务
        /// <summary>
        /// 创建检定消息服务
        /// </summary>
        /// <returns>检定消息服务创建结果</returns>
        public bool InitialMessageService()
        {
            string url = ConfigHelper.Instance.ServiceAddressUI;
            try
            {
                WcfServiceHost.StartService(url, typeof(MeterVerifyService), typeof(IVerifyMessage));
                LogManager.AddMessage(string.Format("检定消息服务创建成功,地址为:{0}", url), EnumLogSource.检定业务日志);
                return true;
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("开启WCF服务失败:{0},异常信息:{1}", url, e.Message), EnumLogSource.检定业务日志, EnumLevel.Error, e);
                return false;
            }
        }
        /// 检定控制服务类
        /// <summary>
        /// 检定控制服务类
        /// </summary>
        private DynamicProxy verifyControlClient;
        /// 表信息数组
        /// <summary>
        /// 表信息数组
        /// </summary>
        private object MeterInfoArray;
        /// Wcf接口表信息类型
        /// <summary>
        /// Wcf接口表信息类型
        /// </summary>
        private Type meterInfoType;
        /// 检定服务客户端准备完毕
        /// <summary>
        /// 检定服务客户端准备完毕
        /// </summary>
        public bool ClientIsReady
        {
            get
            {
                if (verifyControlClient == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// 初始化检定控制客户端
        /// <summary>
        /// 初始化检定控制客户端
        /// </summary>
        /// <returns></returns>
        public bool InitialControlClient()
        {
            string url = ConfigHelper.Instance.ServiceAddressVerify;
            try
            {
                LogManager.AddMessage(string.Format("开始连接到检定控制服务,地址为:{0}", url), EnumLogSource.检定业务日志);
                DynamicProxyFactory factory = new DynamicProxyFactory(url);
                verifyControlClient = factory.CreateProxy("IVerifyControl");
                meterInfoType = factory.ProxyAssembly.GetType("CLDC_Interfaces.MeterInfo");
                Type meterInfoArrayType = factory.ProxyAssembly.GetType("CLDC_Interfaces.MeterInfo[]");
                MeterInfoArray = meterInfoArrayType.InvokeMember("Set", BindingFlags.CreateInstance, null, MeterInfoArray, new object[] { EquipmentData.Equipment.MeterCount });
                LogManager.AddMessage("连接检定控制服务完成,已创建检定控制客户端", EnumLogSource.检定业务日志);
                Login(RequestID()); 
                //下发检定配置信息
                LoadCheckConfig();
                InitialEquipment();
                //下发表通讯协议
                LoadMeterProtocols();
                //连接设备
                EquipmentData.DeviceManager.InitSetting();
                EquipmentData.DeviceManager.Link();
                //下发表信息
                SetMeters();
                return true;
            }
            catch
            {
                LogManager.AddMessage(string.Format("无法连接检定服务,请确定服务已开启,并且地址为:{0}", url), EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                EquipmentData.DeviceManager.IsConnected = false;
                return false;
            }
        }
        /// 开始检定
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <param name="paraKey">检定点编号</param>
        /// <param name="className">检定方法名称</param>
        /// <param name="paraFormat">参数描述</param>
        /// <param name="paraValue">参数值</param>
        /// <param name="option">检定的电源控制规则:0:检定结束后不处理  1:检定结束后关源</param>
        /// <returns></returns>
        public bool StartVerify(string paraKey, string className, string paraFormat, string paraValue, string optionPower)
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }

            try
            {
                var result = verifyControlClient.CallMethod("Start", connectionId, EquipmentData.Controller.CheckingName, paraKey, className, paraFormat, paraValue, optionPower);
                if (result is bool && (bool)result)
                {
                    LogManager.AddMessage("调用检定开始服务成功", EnumLogSource.检定业务日志);
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("检定开始服务调用返回值错误", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("调用检定开始服务异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// 停止检定
        /// <summary>
        /// 停止检定
        /// </summary>
        /// <returns></returns>
        public bool StopVerify(string checkKey)
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            try
            {
                var result = verifyControlClient.CallMethod("Stop", connectionId, checkKey);
                if (result is bool)
                {
                    if ((bool)result)
                    {
                        LogManager.AddMessage("调用停止检定服务成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("调用停止检定服务失败:返回值错误", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    }
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("停止检定服务调用返回值错误", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("调用停止检定服务异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// <summary>
        /// 更新要检标记
        /// </summary>
        /// <returns></returns>
        public bool UpdateCheckFlag()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            try
            {
                var result = verifyControlClient.CallMethod("UpdateCheckFlag", connectionId, EquipmentData.MeterGroupInfo.YaoJian);
                if (result is bool)
                {
                    if ((bool)result)
                    {
                        LogManager.AddMessage("调用表位要检标记服务成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("调用表位要检标记服务失败:返回值错误", EnumLogSource.检定业务日志, EnumLevel.Warning);
                    }
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("调用表位要检标记服务返回值错误", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("更新表位是否要检标记异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// 下发台体基本信息
        /// <summary>
        /// 下发台体基本信息
        /// </summary>
        /// <returns></returns>
        public bool InitialEquipment()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            try
            {
                bool isDan=(EquipmentData.Equipment.EquipmentType.Contains("单相")?true:false);
                string[] EquipmentInfos = new string[1];
                EquipmentInfos[0] = EquipmentData.Equipment.SouthManufacturers;

                var result = verifyControlClient.CallMethod(
                    "InitialEquipment", connectionId, EquipmentData.Equipment.ID, EquipmentData.Equipment.MeterCount, isDan, false, EquipmentInfos); 
                if (result is bool)
                {
                    if ((bool)result)
                    {
                        LogManager.AddMessage("初始化台体基本信息成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("初始化台体基本信息失败", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    }
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("下发表信息服务错误", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("初始化台体基本信息服务异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// 下发表信息
        /// <summary>
        /// 下发表信息
        /// </summary>
        /// <returns></returns>
        public bool SetMeters()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            LogManager.AddMessage("开始下发表信息服务", EnumLogSource.检定业务日志);
            ConvertMeterInfo();
            try
            {
                var result = verifyControlClient.CallMethod(
                    "SetMerter", connectionId, EquipmentData.Equipment.MeterCount, MeterInfoArray);
                if (result is bool)
                {
                    if ((bool)result)
                    {
                        LogManager.AddMessage("下发表信息服务成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("下发表信息服务失败", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    }
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("下发表信息服务错误", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("下发表信息服务异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        private int connectionId = 0;
        /// 获取编号
        /// <summary>
        /// 获取编号
        /// </summary>
        /// <returns></returns>
        public int RequestID()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return -1;
            }
            try
            {
                var result = verifyControlClient.CallMethod("RequestID");
                if (result is int)
                {
                    connectionId = (int)result;
                    return (int)result;
                }
                else
                {
                    LogManager.AddMessage("获取检定登录编号失败", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return -1;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("获取检定登录编号异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return -1;
            }
        }
        /// 获取检定控制权限
        /// <summary>
        /// 获取检定控制权限
        /// </summary>
        /// <param name="requestId">检定控制编号</param>
        /// <returns></returns>
        public bool Login(int requestId)
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            try
            {
                var result = verifyControlClient.CallMethod("Login", requestId);
                if (result is bool)
                {
                    if ((bool)result)
                    {
                        LogManager.AddMessage("获取检定控制权限成功!", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("获取检定控制权限失败,编号错误!", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    }
                    return (bool)result;
                }
                else
                {
                    LogManager.AddMessage("获取检定控制权限失败,返回码类型不正确!", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("获取检定控制权限异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// 将本地的表信息转换成WCF里面的表信息
        /// <summary>
        /// 将本地的表信息转换成WCF里面的表信息
        /// </summary>
        private void ConvertMeterInfo()
        {
            MeterInfo[] meterInfos = EquipmentData.MeterGroupInfo.GetVerifyMeterInfo();
            PropertyInfo[] properties = meterInfoType.GetProperties();
            for (int i = 0; i < meterInfos.Length; i++)
            {
                object meterInfo = meterInfoType.Assembly.CreateInstance("CLDC_Interfaces.MeterInfo");
                #region 给每一个属性赋值,MeterInfo和wcf中MeterInfo的属性名是一样的
                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        PropertyInfo property1 = meterInfos[i].GetType().GetProperty(property.Name);
                        if (property1 == null)
                        {
                            continue;
                        }
                        object obj1 = property1.GetValue(meterInfos[i], null);
                        property.SetValue(meterInfo, obj1, null);
                    }
                    catch (Exception e)
                    {
                        LogManager.AddMessage(string.Format("下发表信息失败,属性名:{0},{1}", property.Name, e.Message), EnumLogSource.检定业务日志, EnumLevel.Warning, e);
                    }
                }
                try
                {
                    //SetValue
                    MeterInfoArray.GetType().GetMethod("SetValue", new Type[] { meterInfo.GetType(), typeof(int) }).Invoke(MeterInfoArray, new Object[] { meterInfo, i });
                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("设置第{0}块表信息失败", i + 1), EnumLogSource.检定业务日志, EnumLevel.Warning, e);
                }
                #endregion
            }
        }

        #region 设备控制
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="deviceParams">设备名|数量|序号|IP或“COM”|起始端口|远程端口|端口号|驱动文件全名|完整类名</param>
        /// <returns></returns>
        public int InitDevice(string[] deviceParams)
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                EquipmentData.DeviceManager.IsConnected = false;
                return -1;
            }
            try
            {
                var result = verifyControlClient.CallMethod("InitDevice", connectionId, deviceParams);
                if (result is int)
                {
                    if ((int)result != 0)
                    {
                        EquipmentData.DeviceManager.IsConnected = false;
                    }
                    return (int)result;
                }
                else
                {
                    LogManager.AddMessage("初始化设备失败!", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    return -1;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("初始化设备信息异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return -1;
            }
        }

        public int DeviceControl(string methodName, object[] paramArry)
        {

            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                return -1;
            }
            try
            {
                var result = verifyControlClient.CallMethod("DeviceControl", connectionId, methodName, paramArry);
                if (result is int)
                {
                    return (int)result;
                }
                else
                {
                    LogManager.AddMessage(string.Format("设备控制服务调用失败,方法名:{0}!", methodName), EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                }
                return -1;
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("设备控制服务调用异常,方法名:{0},异常信息:{1}", methodName, e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return -1;
            }
        }
        #endregion

        #region 下发检定配置信息和通讯协议
        /// <summary>
        /// 下发表通讯协议
        /// </summary>
        /// <returns></returns>
        public bool LoadMeterProtocols()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            try
            {
                #region 加载xmlNode
                XmlDocument doc = new XmlDocument();
                doc.Load(string.Format(@"{0}\xml\DgnProtocol.xml", Directory.GetCurrentDirectory()));
                XmlElement nodeProtocols = (XmlElement)(doc.SelectSingleNode("DgnProtocol/Protocols"));
                XmlNodeList nodeList = nodeProtocols.SelectNodes("R");
                List<string> listNames = new List<string>();
                foreach (XmlNode nodeTemp in nodeList)
                {
                    listNames.Add(nodeTemp.Attributes["Name"].Value);
                }
                CodeTree.CodeTreeViewModel.Instance.SyncronizeMeterProtocols(listNames);
                #endregion
                var result = verifyControlClient.CallMethod(
                    "LoadMeterProtocols", new Type[] { typeof(int), typeof(XmlElement) }, new object[] { connectionId,  nodeProtocols });
                if (result is int)
                {
                    if ((int)result == 0)
                    {
                        LogManager.AddMessage("下发表通讯协议成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("下发表通讯协议失败", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    }
                    return (int)result == 0;
                }
                else
                {
                    LogManager.AddMessage("下发表通讯协议错误", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("下发表通讯协议异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        /// <summary>
        /// 下发检定配置信息
        /// </summary>
        /// <returns></returns>
        public bool LoadCheckConfig()
        {
            if (verifyControlClient == null)
            {
                LogManager.AddMessage("未能连接到检定服务,请判断检定服务已开启", EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
            //TODO:用集合发送
            bool blnRst = SetCheckConfigByEnumConfigId("加密机配置", EnumConfigId.加密机配置);
            blnRst = SetCheckConfigByEnumConfigId("软件设置", EnumConfigId.通讯方式);
          //  blnRst = SetCheckConfigByEnumConfigId("软件设置", EnumConfigId.标准表型号);
            //blnRst &= SetCheckConfigByEnumConfigId("设备特殊配置", EnumConfigId.设备特殊配置);
            return blnRst;
        }
        private bool SetCheckConfigByEnumConfigId(string configName, EnumConfigId Ecid)
        {
            try
            {
                LogManager.AddMessage("下发" + configName + "信息...", EnumLogSource.检定业务日志);
                var result = verifyControlClient.CallMethod(
                    "InitialCheckParam", connectionId, ((int)Ecid).ToString("D5"), ConfigHelper.Instance.GetConfigString(Ecid));
                if (result is int)
                {
                    if ((int)result == 0)
                    {
                        LogManager.AddMessage("下发" + configName + "信息成功", EnumLogSource.检定业务日志);
                    }
                    else
                    {
                        LogManager.AddMessage("下发" + configName + "信息失败", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    }
                    return (int)result == 0;
                }
                else
                {
                    LogManager.AddMessage("下发" + configName + "信息异常", EnumLogSource.检定业务日志, EnumLevel.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("下发" + configName + "信息异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                return false;
            }
        }
        #endregion
        /// <summary>
        /// 创建本机结论服务
        /// </summary>
        /// <returns>检定消息服务创建结果</returns>
        public bool InitialLocalMisDataService()
        {
            if (!ConfigHelper.Instance.IsOpenLocalMisService)
            {
                return true;
            }
            string url = ConfigHelper.Instance.LocalMisServiceAddress;
            try
            {
                WcfServiceHost.StartService(url, typeof(MisDataService), typeof(IMisData));
                LogManager.AddMessage(string.Format("本地检定结论服务创建成功,地址为:{0}", url), EnumLogSource.检定业务日志);
                return true;
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("开启本地检定结论服务失败:{0},异常信息:{1}", url, e.Message), EnumLogSource.检定业务日志, EnumLevel.Error, e);
                return false;
            }
        }
    }
}
