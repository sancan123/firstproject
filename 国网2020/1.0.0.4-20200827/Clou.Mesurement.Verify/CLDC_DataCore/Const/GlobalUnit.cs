using System;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Function;

namespace CLDC_DataCore.Const
{
    public class GlobalUnit
    {
        #region 定义

        /// <summary>
        /// 是否在做载波
        /// </summary>
        public static bool Flag_IsCarrier = false;
        /// <summary>
        /// 电压是否已升起
        /// </summary>
        public static bool g_IsOnPowerU = false;
        private static bool enableReadStd = true;

        public static int intBzMeterConst = 4000000;


        public static bool IsCL3112 = false;

        /// <summary>
        /// 是否是中电华瑞2016
        /// </summary>
        public static bool Flag_IsZD2016 = false;

        /// <summary>
        /// 允许读取
        /// 信息
        /// 在设置标准表参数的时候不允许读取标准表参数
        /// </summary>
        public static bool EnableReadStd
        {
            get { return enableReadStd; }
            set { enableReadStd = value; }
        }

        private static int? _dispatcherType = null;
        /// <summary>
        /// 调度类型：0 不带调度；1 调度控制
        /// </summary>
        public static int DispatcherType
        {
            get
            {
                return _dispatcherType == null ? 0 : _dispatcherType.Value;
            }
            set
            {
                if (_dispatcherType == null)
                {
                    _dispatcherType = value;
                }
                else
                {
                    throw new Exception("只能在Main函数写一次");
                }
            }
        }
        /// <summary>
        /// 调度控制：是否可以自动开始检定
        /// </summary>
        public static bool DispatcherCanAutoStart
        {
            get;
            set;
        }
        /// <summary>
        /// 总检定开始时间
        /// </summary>
        private static DateTime _CheckTimeStartSum;
        /// <summary>
        /// 总检定开始时间
        /// </summary>
        public static DateTime g_CheckTimeStartSum
        {
            get { return _CheckTimeStartSum; }
            set
            {
                _CheckTimeStartSum = value;
            }
        }

        /// <summary>
        /// 更新检定ActiveID
        /// </summary>
        public static void UpdateActiveID(int NewID)
        {
            //lock (objUpdateActiveIDLock)
            {
                if (g_CUS != null)
                {
                    g_CUS.DnbData.ActiveItemID = NewID;
                }
            }
        }

        /// <summary>
        /// 总检定结束时间
        /// </summary>
        private static DateTime _CheckTimeSumEnd;
        /// <summary>
        /// 总检定结束时间
        /// </summary>
        public static DateTime g_CheckTimeEndSum
        {
            get { return _CheckTimeSumEnd; }
            set
            {
                _CheckTimeSumEnd = value;
            }
        }

        /// <summary>
        /// 当前检定项目开始时间
        /// </summary>
        private static DateTime _CheckTimeCurrentStart;
        /// <summary>
        /// 当前检定项目开始时间
        /// </summary>
        public static DateTime g_CheckTimeCurrentStart
        {
            get { return _CheckTimeCurrentStart; }
            set
            {
                _CheckTimeCurrentStart = value;
            }
        }

        /// <summary>
        /// 当前检定项目结束时间
        /// </summary>
        private static DateTime _CheckTimeCurrentEnd;
        /// <summary>
        /// 当前检定项目结束时间
        /// </summary>
        public static DateTime g_CheckTimeCurrentEnd
        {
            get { return _CheckTimeCurrentEnd; }
            set
            {
                _CheckTimeCurrentEnd = value;
            }
        }



        /// <summary>
        /// 当前检定任务类型
        /// </summary>
        private static int _CurTestType = 00;

        /// <summary>
        /// 获取设置当前检定任务类型
        /// 
        /// </summary>
        public static int g_CurTestType
        {
            get { return _CurTestType; }
            set
            {
                _CurTestType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool _ReadingPara;
        /// <summary>
        /// 当前是否处于读取参数试验中
        /// </summary>
        public static bool ReadingPara
        {
            get
            {
                return _ReadingPara;
            }
            set
            {
                _ReadingPara = value;
            }
        }



        /// <summary>
        /// 电能表数据公共模型
        /// 客户端程序专用
        /// </summary>
        public static CLDC_DataCore.CusModel g_CUS = null;
        /// <summary>
        /// 只为升源，不判任何条件
        /// </summary>
        public static bool OnlyToPower = false;

        /// <summary>
        /// 设置是否发送错误校验帧
        /// </summary>
        public static bool IsErrChkSum = false;
        #endregion

        #region
        /// <summary>
        /// 获取一块表基本信息
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public static CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo Meter(int Index)
        {
            if (g_CUS == null) return null;
            if (Index < 0 || Index > g_CUS.DnbData._Bws)
                return null;
            return g_CUS.DnbData.MeterGroup[Index];
        }

        public static int intYaoJianMeterNum
        {
            get
            {
                int intNum = 0;
                if (g_CUS == null)
                    return intNum;
                for (int i = 0; i < g_CUS.DnbData._Bws; i++)
                {
                    if (g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                        intNum++;
                }
                return intNum;
            }
        }

        /// <summary>
        /// 获取当前检定状态
        /// </summary>
        public static Cus_CheckStaute CheckState
        {
            get { return g_CUS.DnbData.CheckState; }
        }

        /// <summary>
        /// 第一块表有效位
        /// </summary>
        public static int FirstYaoJianMeter
        {
            get
            {
                if (g_CUS == null)
                    return -1;
                for (int i = 0; i < g_CUS.DnbData._Bws; i++)
                {
                    if (g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                        return i;
                }
                return -1;
            }
        }
        public static string strYaoJianMeter
        {
            get
            {
                string defaultYaoJian = "000000000000" + "000000000000";
                string strTmp1 = "";
                string strTmp2 = "";
                if (g_CUS == null)
                    return defaultYaoJian;
                for (int i = 0; i < g_CUS.DnbData._Bws; i++)
                {
                    if (g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                        strTmp1 = "1" + strTmp1;
                    else
                        strTmp1 = "0" + strTmp1;
                }
                strTmp2 = defaultYaoJian + strTmp1;
                strTmp2 = strTmp2.Substring(strTmp1.Length, defaultYaoJian.Length);
                return strTmp2;
            }
        }

        public static bool[] blnYaoJianMeter
        {

            get
            {
                bool[] blnYaoJian = new bool[g_CUS.DnbData._Bws];

                for (int i = 0; i < g_CUS.DnbData._Bws; i++)
                {

                    blnYaoJian[i] = g_CUS.DnbData.MeterGroup[i].YaoJianYn;

                }
                return blnYaoJian;
            }
        }

        /// <summary>
        /// 获取整体表位结论
        /// </summary>
        public static bool MeterResult
        {
            get
            {
                bool bResult = true;
                if (g_CUS == null)
                    return bResult;
                for (int i = 0; i < g_CUS.DnbData._Bws; i++)
                {
                    if (g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                        if (g_CUS.DnbData.MeterGroup[i].Mb_chrResult == "不合格")
                        {
                            bResult = false;
                            break;
                        }
                }
                return bResult;
            }
        }


        /// <summary>
        /// 手动输入结论
        /// </summary>

        public static string[] ManualResult = new string[96];
     

        /// <summary>
        /// 手动输入检定数据
        /// </summary>

        public static string[] ManualShuju= new string[96];
       


        #endregion

        #region 电表基本信息

        /// <summary>
        /// Imax(A)
        /// </summary>
        public static float Imax
        {
            get
            {
                try
                {
                    if (FirstYaoJianMeter == -1)
                    {
                        return 4F;
                    }
                    else
                    {
                        return CLDC_DataCore.Function.Number.GetCurrentByIb("imax", Meter(FirstYaoJianMeter).Mb_chrIb,Meter(FirstYaoJianMeter).Mb_BlnHgq);

                        // return (Single)Comm.Function.Number.SplitKF(Meter(FirstYaoJianMeter).Mb_chrIb, false);
                    }
                }
                catch
                {
                    return 4F;
                }
            }
        }
        #region
        // add by wzs on 2019-11-20 多芯表
        /// <summary>
        /// Imin(A)
        /// </summary>
        public static float Imin
        {
            get
            {
                try
                {
                    if (FirstYaoJianMeter == -1)
                    {
                        return 4F;
                    }
                    else
                    {
                        return CLDC_DataCore.Function.Number.GetCurrentByIb("imin", Meter(FirstYaoJianMeter).Mb_chrIb,Meter(FirstYaoJianMeter).Mb_BlnHgq);

                        // return (Single)Comm.Function.Number.SplitKF(Meter(FirstYaoJianMeter).Mb_chrIb, false);
                    }
                }
                catch
                {
                    return 4F;
                }
            }
        }
        /// <summary>
        /// Itr(A)
        /// </summary>
        public static float Itr
        {
            get
            {
                try
                {
                    if (FirstYaoJianMeter == -1)
                    {
                        return 4F;
                    }
                    else
                    {
                        return CLDC_DataCore.Function.Number.GetCurrentByIb("itr", Meter(FirstYaoJianMeter).Mb_chrIb, Meter(FirstYaoJianMeter).Mb_BlnHgq);

                        // return (Single)Comm.Function.Number.SplitKF(Meter(FirstYaoJianMeter).Mb_chrIb, false);
                    }
                }
                catch
                {
                    return 4F;
                }
            }
        }

        //end  add by wzs on 2019-11-20

        #endregion
        /// <summary>
        /// Ib(A)
        /// </summary>
        public static float Ib
        {
            get
            {
                try
                {
                    if (FirstYaoJianMeter == -1)
                    {
                        return 1F;
                    }
                    else
                    {
                        return CLDC_DataCore.Function.Number.GetCurrentByIb("ib", Meter(FirstYaoJianMeter).Mb_chrIb,Meter(FirstYaoJianMeter).Mb_BlnHgq);
                        //return (Single)Comm.Function.Number.SplitKF(Meter(FirstYaoJianMeter).Mb_chrIb, true);
                    }
                }
                catch
                {
                    return 1F;
                }
            }
        }

        /// <summary>
        /// U(V)
        /// </summary>
        public static float U
        {
            get
            {
                try
                {
                    if (FirstYaoJianMeter == -1)
                    {
                        return 57.7F;
                    }
                    else
                    {
                        return float.Parse(Meter(FirstYaoJianMeter).Mb_chrUb);
                    }
                }
                catch
                {
                    return 57.7F;
                }
            }
        }
        /// <summary>
        /// 测量方式
        /// </summary>
        public static CLDC_Comm.Enum.Cus_Clfs Clfs
        {
            get
            {
                Model.DnbModel.DnbInfo.MeterBasicInfo meterinfo = Meter(FirstYaoJianMeter);
                if (meterinfo == null) return Cus_Clfs.三相四线;
                return (CLDC_Comm.Enum.Cus_Clfs)meterinfo.Mb_intClfs;
            }
        }

        /// <summary>
        /// 是否经互感器
        /// </summary>
        public static bool HGQ
        {
            get
            {
                if (FirstYaoJianMeter >= 0)
                {
                    return Meter(FirstYaoJianMeter).Mb_BlnHgq;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否经止逆器
        /// </summary>
        public static bool ZNQ
        {
            get
            {
                if (FirstYaoJianMeter >= 0)
                {
                    return Meter(FirstYaoJianMeter).Mb_BlnZnq;
                }
                return false;
            }
        }

        /// <summary>
        /// 频率
        /// </summary>
        public static float PL
        {
            get
            {
                if (FirstYaoJianMeter >= 0)
                {
                    return float.Parse(Meter(FirstYaoJianMeter).Mb_chrHz);
                }
                return 50F;
            }
        }
        #endregion

        #region 变量声明
        /// <summary>
        /// 系统配置模型，公用
        /// </summary>
        public static CLDC_DataCore.SystemModel.SystemInfo g_SystemConfig = null;

        /// <summary>
        /// 日志队列
        /// </summary>
        public static CLDC_DataCore.Function.RunLog Log = null;
        /// <summary>
        ///
        /// </summary>
        public static CLDC_Comm.Enum.Cus_NetState NetState = Cus_NetState.DisConnected;
        /// <summary>
        /// 本地设备联机状态
        /// </summary>
        public static CLDC_Comm.Enum.Cus_NetState EquipConnectedState = Cus_NetState.DisConnected;
        /// <summary>
        /// 帧日志队列
        /// </summary>
        public static RunLogFrame FrameLog = null;

        private static log4net.ILog logger = null;

        /// <summary>
        /// 日志记录组件，用来替代 日志队列 Log成员
        /// </summary>
        public static log4net.ILog Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = log4net.LogManager.GetLogger("AppLog");
                }
                return logger;
            }
        }

        /// <summary>
        ///事件队列 
        /// </summary>
        public static VerifyMsgControl g_MsgControl = null;

        /// <summary>
        /// 数据队列
        /// </summary>
        public static VerifyMsgControl g_DataControl = null;
        /// <summary>
        /// 通讯数据队列
        /// </summary>
        public static VerifyMsgControl g_485DataControl = null;
        /// <summary>
        /// 实时数据队列
        /// </summary>
        public static RealTimeMsgControl g_RealTimeDataControl = null;

        /// <summary>
        /// 应用程序是否已经退出.用于结束线程
        /// </summary>
        public static bool ApplicationIsOver = false;
        /// <summary>
        /// 检定线程是否停止，用于按钮状态
        /// </summary>
        public static bool TestThreadIsRunning = false;

        /// <summary>
        /// While循环时线程休眠时间，单位：MS
        /// </summary>
        public static int g_ThreadWaitTime = 1;

        /// <summary>
        /// 标准表功率[0:有功功率1:无功功率:2:视在功率]
        /// </summary>
        public static float[] g_StrandMeterP = new float[3];

        /// <summary>
        /// 标准表有功功率[0:A元1:B元:2:C元]
        /// </summary>
        public static float[] g_StrandMeterPFY = new float[3];

        /// <summary>
        /// 标准表无功功率[0:A元1:B元:2:C元]
        /// </summary>
        public static float[] g_StrandMeterQFY = new float[3];
        /// <summary>
        /// A元、B元、C元
        /// </summary>
        public static float[] g_StrandMeterU = new float[3];
        /// <summary>
        /// A元、B元、C元
        /// </summary>
        public static float[] g_StrandMeterI = new float[3];

        //是否所有要检表相同
        // public static bool g_AllMeterIsSame = true;
        /// <summary>
        /// 检验员
        /// </summary>
        public static StUserInfo User_Jyy;

        /// <summary>
        /// 核验员
        /// </summary>
        public static StUserInfo User_Hyy;

        /// <summary>
        /// 从数据库获取方案，flase，XML方案
        /// </summary>
        public static bool Plan_FromMDB = false;
        /// <summary>
        /// 通讯模式
        /// </summary>
        public static Cus_CommunType g_CommunType = Cus_CommunType.通讯485;

        public static StCarrierInfo CarrierInfo = new StCarrierInfo();

        public static string[] G_CurrentEventNum;
        public static string[] G_LastEventNum;
        public static float MaxI = 0;

        public static float MaxU = 0;
        #endregion

        #region 读取配置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="setValue"></param>
        public static void SetConfig(string strKey, string setValue)
        {
            //    if (g_SystemConfig.SystemMode.EditValue(strKey, setValue))
            //     {
            //          g_SystemConfig.SystemMode.Save();
            //       }
        }
        /// <summary>
        /// 取配置[文本类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static string GetConfig(string strKey, string DefaultValue)
        {

            if (g_SystemConfig == null)
                return DefaultValue;
            string strTmp = g_SystemConfig.SystemMode.getValue(strKey);
            if (Function.Common.IsEmpty(strTmp))
                return DefaultValue;
            return strTmp;
        }

        /// <summary>
        /// 取配置[Int类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static int GetConfig(string strKey, int DefaultValue)
        {
            string strValue = GetConfig(strKey, DefaultValue.ToString());
            if (!Function.Number.IsNumeric(strValue))
                return DefaultValue;
            return int.Parse(strValue);
        }

        /// <summary>
        /// 取配置[Float类型]
        /// </summary>
        /// <param name="strKey">主键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        public static float GetConfig(string strKey, float DefaultValue)
        {
            string strValue = GetConfig(strKey, DefaultValue.ToString());
            if (!Function.Number.IsNumeric(strValue))
                return DefaultValue;
            return float.Parse(strValue);
        }
        #endregion

        #region 系统信息
        /// <summary>
        /// 是否是演示版本
        /// </summary>
        public static bool IsDemo
        {
            get { return false; }
        }

        /// <summary>
        /// 是否是单相台
        /// </summary>
        public static bool IsDan { get; set; }

        /// <summary>
        /// 测量方式
        /// </summary>
        public static Cus_Clfs clfs = Cus_Clfs.单相;

        /// <summary>
        /// 南网设备厂家
        /// </summary>
        public static Cus_DeviceManufacturers DeviceManufacturers = Cus_DeviceManufacturers.科陆;

        /// <summary>
        /// 南网设备控制DLL的开发平台类型
        /// </summary>
        public static Cus_SouthDeviceDllType DeviceDllType = Cus_SouthDeviceDllType.DotNet平台开发;
        /// <summary>
        /// 南网读写卡器DLL的开发平台类型
        /// </summary>
        public static Cus_SouthCardDllType CardDllType = Cus_SouthCardDllType.DotNet平台开发;


        /// <summary>
        /// 南网费控通讯方式
        /// </summary>
        public static Cus_CommunType g_Dev_CommunType = Cus_CommunType.南网通讯DLL;

        public static string CheckControlType = "0";

        /// <summary>
        /// 是否内置继电器的表
        /// </summary>
        public static bool IsNZLoadRelayControl = false;

        #endregion

        #region 参变量分类
        /// <summary>
        /// 参变量分类
        /// </summary>
        /// <param name="str_ID">协议标识</param>
        /// <returns>1类,2类,3类</returns>
        public static int CheckStrIDType(string str_IDs)
        {
            int tp = 0;
            string str_ID = str_IDs.ToUpper();
            if (g_CUS.DnbData.MeterGroup[GlobalUnit.FirstYaoJianMeter].DgnProtocol.HaveProgrammingkey)
            {
                return 4;//有编程键
            }
            if (DicIDType.ContainsKey(str_ID))
            {
                tp = 1;
            }
            else if (str_ID.IndexOf("040501", 0) >= 0)
            {
                tp = 1;
            }
            else if (str_ID.IndexOf("040502", 0) >= 0)
            {
                tp = 1;
            }
            else if (str_ID == "04001301")
            {
                tp = 3;
            }
            else
            {
                tp = 2;
            }
            return tp;
        }
        /// <summary>
        /// 参变量分类字典。key 协议标识 value 1类2类3类
        /// </summary>
        private static Dictionary<string, int> DicIDType = new Dictionary<string, int> { 
        {"04000108",1},{"04000109",1},{"04000306",1},{"04000307",1},
        {"04000402",1},{"0400040E",1},{"04001001",1},{"04001002",1},
        {"040501XX",1},{"040502XX",1},{"040604FF",1},{"040605FF",1},
        };
        #endregion
        public static string ENCRYPTION_MACHINE_TYPE { get; set; }
        public static string ENCRYPTION_MACHINE_IP { get; set; }
        public static string ENCRYPTION_MACHINE_PORT { get; set; }
        public static string ENCRYPTION_MACHINE_PASSWORD { get; set; }
        public static string ENCRYPTION_MACHINE_OUTTIME { get; set; }

        /// <summary>
        /// 当前载波配置
        /// </summary>
        public static CLDC_DataCore.Model.CarrierProtocol.CarrierProtocolInfo[] CarrierInfos = null;
        /// <summary>
        /// 载波当前表位
        /// </summary>
        public static int Carrier_Cur_BwIndex = 0;


        /// <summary>
        /// false = 载波标示 true = 无线标示
        /// </summary>
        public static bool boolIsWxOrZB = false;

        /// <summary>
        /// 是否需要工装模块
        /// </summary>
        public static bool IsGZMK = false;

        /// <summary>
        /// 是否进行尖顶波试验
        /// </summary>

        public static bool IsJdb = false;

        /// <summary>
        /// 标准表是否切换挡位
        /// </summary>
        public static bool IsMonDanWei = false;

        /// <summary>
        /// 是否进行间谐波试验
        /// </summary>

        public static bool IsJxb = false;

        public static bool IsXieBo = false;
    }
}
