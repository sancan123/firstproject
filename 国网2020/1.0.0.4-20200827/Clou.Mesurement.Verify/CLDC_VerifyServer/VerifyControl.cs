using System;
using System.Collections.Generic;
using CLDC_Interfaces;
using CLDC_VerifyAdapter.Helper;
using System.Reflection;
using CLDC_DataCore.Const;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_VerifyAdapter;
using System.Xml;

namespace CLDC_VerifyServer
{
    class VerifyControl : IVerifyControl, IVerifyForMis
    {
        private static int _connectionID = 0;
        private static int _Random = 5367;

        private static int _meterCount = 0;

        internal static ClientMain cm = new ClientMain();

        #region IVerifyControl 成员

        /// <summary>
        /// 请求登录ID
        /// </summary>
        public int RequestID()
        {
            Console.WriteLine("RequestID:" + _Random);
            return _Random;
        }

        /// <summary>
        /// 登录服务
        /// </summary>
        public bool Login(int connectionID)
        {
            _connectionID = connectionID;
            Console.WriteLine("Login:" + _connectionID);
            return true;
        }

        /// <summary>
        /// 初始化装置参数
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="equipentID"></param>
        /// <param name="meterCount"></param>
        /// <param name="isDan"></param>
        /// <param name="isDemo"></param>
        /// <param name="listEquipmentInfo">intdex0=南网设备厂家</param>
        /// <returns></returns>
        public bool InitialEquipment(int connectionID, string equipentID, int meterCount, bool isDan, bool isDemo, string[] EquipmentInfos)
        {
            if (_connectionID != connectionID)
            {
                return false;
            }
            GlobalUnit.CheckControlType = CLDC_DataCore.Function.File.ReadInIString(CLDC_DataCore.Function.File.GetPhyPath("CheckControlType.ini"), "CheckControlType", "CheckControlType", "0");
            GlobalUnit.IsDan = isDan;
            _meterCount = meterCount;
            cm.Logger = GlobalUnit.Logger;
            cm.Initialize(int.Parse(equipentID), meterCount, isDemo);
            if (EquipmentInfos.Length > 0)
            {
                switch (EquipmentInfos[0])
                {
                    case "科陆":
                        GlobalUnit.DeviceManufacturers = CLDC_Comm.Enum.Cus_DeviceManufacturers.科陆;
                        break;
                    case "涵普":
                        GlobalUnit.DeviceManufacturers = CLDC_Comm.Enum.Cus_DeviceManufacturers.涵普;
                        break;
                    case "格宁":
                        GlobalUnit.DeviceManufacturers = CLDC_Comm.Enum.Cus_DeviceManufacturers.格宁;
                        break;
                    default:
                        GlobalUnit.DeviceManufacturers = CLDC_Comm.Enum.Cus_DeviceManufacturers.科陆;
                        break;
                }
            }
            if (GlobalUnit.g_CUS.DnbData.MeterGroup == null)
            {
                GlobalUnit.g_CUS.DnbData.MeterGroup = new List<CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo>();
            }
            for (int _I = GlobalUnit.g_CUS.DnbData.MeterGroup.Count; _I < meterCount; _I++)
            {
                GlobalUnit.g_CUS.DnbData.MeterGroup.Add(new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo(_I + 1));
            }
            Console.WriteLine("InitialEquipment:" + _meterCount);
            return true;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="deviceParams">设备名|数量|序号|IP或“COM”|起始端口|远程端口|端口号|驱动文件全名|完整类名</param>
        /// <returns></returns>
        public int InitDevice(int connectionID, string[] deviceParams)
        {
            if (_connectionID != connectionID) return 1;

            //if (VerifyClient.Instance.Status)
            //{
            //    VerifyClient.Instance.OutMessage(6, 0, "检定消息服务开启成功!");
            //}

            if (null != deviceParams && deviceParams.Length > 0)
            {
                for (int i = 0; i < deviceParams.Length; i++)
                {
                    Console.WriteLine("deviceParams[" + i + "]:" + deviceParams[i]);
                }
                EquipHelper.Instance.Initialize(_meterCount, deviceParams);
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 设置表信息
        /// </summary>
        public bool SetMerter(int connectionID, int meterCount, MeterInfo[] meterInfos)
        {
            if (_connectionID != connectionID) return false;
            _meterCount = meterCount;
            if (GlobalUnit.g_CUS == null)
            {
                GlobalUnit.g_CUS = new CLDC_DataCore.CusModel(_meterCount, 1);
            }
            if (GlobalUnit.g_CUS.DnbData.MeterGroup == null)
            {
                GlobalUnit.g_CUS.DnbData.MeterGroup = new List<CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo>();
            }
            if (GlobalUnit.g_CUS.DnbData.MeterGroup.Count != _meterCount)
            {
                for (int _I = GlobalUnit.g_CUS.DnbData.MeterGroup.Count; _I < _meterCount; _I++)
                {
                    GlobalUnit.g_CUS.DnbData.MeterGroup.Add(new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo(_I + 1));
                }
                Console.WriteLine("SetMeterCount:" + _meterCount);
            }
            if (meterInfos != null && meterInfos.Length != 0 && meterInfos.Length == _meterCount)
            {
                bool rst = false;

                for (int i = 0; i < meterInfos.Length; i++)
                {
                    if (meterInfos[i].YaoJianYn)
                    {
                        GlobalUnit.IsNZLoadRelayControl = meterInfos[i].Mb_intFKType == 2 ? true : false;
                        if (meterInfos[i].Mb_intClfs == 5)
                        {
                            GlobalUnit.clfs = CLDC_Comm.Enum.Cus_Clfs.单相;
                        }
                        else if (meterInfos[i].Mb_intClfs == 0)
                        {
                            GlobalUnit.clfs = CLDC_Comm.Enum.Cus_Clfs.三相四线;
                        }
                        else if (meterInfos[i].Mb_intClfs == 1)
                        {
                            GlobalUnit.clfs = CLDC_Comm.Enum.Cus_Clfs.三相三线;
                        }
                        break;
                    }
                }

                for (int i = 0; i < meterInfos.Length; i++)
                {
                    try
                    {
                        CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo mb = GlobalUnit.g_CUS.DnbData.MeterGroup[meterInfos[i].Mb_intBno - 1];
                        mb._intBno = meterInfos[i].Mb_intBno;
                        mb.AVR_CARR_PROTC_NAME = meterInfos[i].AVR_CARR_PROTC_NAME;
                        mb.AVR_PROTOCOL_NAME = meterInfos[i].AVR_PROTOCOL_NAME;
                        mb.GuiChengName = meterInfos[i].GuiChengName;
                        mb.Mb_BlnHgq = meterInfos[i].Mb_BlnHgq;
                        mb.Mb_BlnZnq = meterInfos[i].Mb_BlnZnq;
                        mb.Mb_Bxh = meterInfos[i].Mb_Bxh;
                        mb.Mb_chrAddr = meterInfos[i].Mb_chrAddr;
                        mb._Mb_MeterNo = meterInfos[i]._Mb_MeterNo;
                        mb.Mb_chrBcs = meterInfos[i].Mb_chrBcs;
                        mb.Mb_chrBdj = meterInfos[i].Mb_chrBdj;
                        mb.Mb_chrBlx = meterInfos[i].Mb_chrBlx;
                        mb.Mb_chrHardVer = meterInfos[i].Mb_chrHardVer;
                        mb.Mb_chrHz = meterInfos[i].Mb_chrHz;
                        mb.Mb_chrIb = meterInfos[i].Mb_chrIb;
                        mb.Mb_chrSoftVer = meterInfos[i].Mb_chrSoftVer;
                        mb.Mb_ChrTxm = meterInfos[i].Mb_ChrTxm;
                        mb.Mb_chrUb = meterInfos[i].Mb_chrUb;
                        mb.Mb_gygy = (CLDC_Comm.Enum.Cus_GyGyType)meterInfos[i].Mb_gygy;
                        mb.Mb_intClfs = meterInfos[i].Mb_intClfs;
                        mb.Mb_intFKType = meterInfos[i].Mb_intFKType;
                        mb.YaoJianYn = meterInfos[i].YaoJianYn;
                        mb.Mb_chrAddr_MAC = meterInfos[i].AVR_OTHER_2;
                        mb.Mb_chrOther4 = meterInfos[i].AVR_OTHER_4;
                        mb.Mb_ID = meterInfos[i].MB_ID;

                        rst = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        rst = false;
                    }
                }

                //初始化表数据
                MeterDataHelper.Instance.Init();
                //更新表协议
                Adapter.Instance.UpdateMeterProtocol();
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 0);
                Console.WriteLine("SetMeter:" + meterInfos.ToString());
                return rst;
            }
            Console.WriteLine("SetMeter:" + meterInfos.ToString());
            return false;
        }
     
        /// <summary>
        /// 更新表位要检标志
        /// </summary>
        public bool UpdateCheckFlag(int connectionID, bool[] checkFlag)
        {
            if (_connectionID != connectionID) return false;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn = checkFlag[i];
            }
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 0);
            return true;
        }
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <param name="connectionID">连接密码</param>
        /// <param name="itemName">检定项名称</param>
        /// <param name="itemKey">检定项编号</param>
        /// <param name="className">方法名称</param>
        /// <param name="formatPara">参数数据格式</param>
        /// <param name="formatParaValue">参数数据值</param>
        /// <param name="option">检定结束后的电源控制</param>
        /// <returns>操作结果</returns>
        public bool Start(int connectionID,string itemName, string itemKey,string className, string formatPara, string formatParaValue, string option)
        {
            if (_connectionID != connectionID) return false;
            if (VerifyProcess.Instance.IsBusy)
            {
                MessageController.Instance.AddMessage("检定器正在检定,请等待完毕以后再开始", 6, 2);
                return false;
            }
            #region 检定器开始检定
            if (VerifyProcess.Instance.StartVerify(itemName,itemKey,className, formatParaValue, option))
            {
                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }

        /// <summary>
        /// 停止检定
        /// </summary>
        public bool Stop(int connectionID, string itemKey)
        {
            if (_connectionID != connectionID) return false;
            VerifyProcess.Instance.StopVerify(itemKey);
            return true;
        }

        #endregion

        #region IVerifyForMis 成员

        /// <summary>
        /// MIS请求刷新数据
        /// </summary>
        public void RefreshData()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVerifyControl 成员

        /// <summary>
        /// 设备控制
        /// </summary>
        public int DeviceControl(int connectionID, string MethodName, object[] paramArry)
        {
            try
            {
                #region 找到设备控制方法
                //设备控制由EquipHelper提供统一接口
                Type equipType = EquipHelper.Instance.GetType();
                Type[] paramTypes = Type.EmptyTypes;
                if (paramArry != null)
                {
                    paramTypes = new Type[paramArry.Length];
                    
                    for (int i = 0; i < paramArry.Length; i++)
                    {
                        Type typeTemp = paramArry[i].GetType();
                        if (MethodName == "SetLoadRelayControl" && i == 0)
                        {
                            typeTemp = GlobalUnit.blnYaoJianMeter.GetType();
                            paramArry[i] = GlobalUnit.blnYaoJianMeter;
                        }
                        paramTypes[i] = typeTemp;
                    }
                }
                MethodInfo method = equipType.GetMethod(MethodName, paramTypes);
                if (method == null)
                {
                    method = equipType.GetMethod(MethodName);
                }
                if (method == null)
                {
                    MessageController.Instance.AddMessage(string.Format("未找到设备控制方法:{0}", MethodName), 7, 2);
                    return -1;
                }
                else
                {   
                    object resultTemp = method.Invoke(EquipHelper.Instance, paramArry);
                    if (resultTemp is bool)
                    {
                        if ((bool)resultTemp)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else if (resultTemp == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                MessageController.Instance.AddMessage(string.Format("调用设备控制方法{0}异常:{1}!!", MethodName, e.Message), 7, 2);
                return -1;
            }
        }

        #endregion

        /// <summary>
        /// 下发表协议
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="nodeProtocols"></param>
        /// <returns></returns>
        public int LoadMeterProtocols(int connectionID, XmlElement nodeProtocols)
         {
            if (_connectionID != connectionID) return 1;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo.NodeProtocols = nodeProtocols;
            }
            
            return 0;
        }

        /// <summary>
        /// 初始化检定参数
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="configId"></param>
        /// <param name="strConfig"></param>
        /// <returns></returns>
        public int InitialCheckParam(int connectionID, string configId, string strConfig)
        {
            if (_connectionID != connectionID) return 1;
            if (string.IsNullOrEmpty(strConfig)) return 2;
            //TODO:用集合解析
            if (configId == "07001")
            {
                string[] strcon = strConfig.Split('|');
                if (strcon.Length >= 5)
                {
                    GlobalUnit.ENCRYPTION_MACHINE_TYPE = strcon[0];
                    GlobalUnit.ENCRYPTION_MACHINE_IP = strcon[1];
                    GlobalUnit.ENCRYPTION_MACHINE_PORT = strcon[2];
                    GlobalUnit.ENCRYPTION_MACHINE_PASSWORD = strcon[3];
                    GlobalUnit.ENCRYPTION_MACHINE_OUTTIME = strcon[4];
                    return 0;
                }
                return 4;
            }
            else if (configId == "08001")
            {//误差板|0|是|是|是|是|4|否|15|90|是
                //GlobalUnit.SpcSetting = new CLDC_DataCore.ConfigModel.SpecialSetting(strConfig);
                return 0;
            }
            else if (configId == "03005")
            {
                GlobalUnit.g_CommunType = strConfig == "蓝牙" ? CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙 : CLDC_Comm.Enum.Cus_CommunType.通讯485;
                return 0;
            }
            else if (configId == "03008")
            {
                GlobalUnit.IsCL3112 = strConfig == "CL3112" ? true : false;
                return 0;
            }
            return 3;
        }
    }
}