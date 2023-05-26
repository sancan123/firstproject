
using System;
using CLDC_DeviceDriver.Drivers.Clou.Packets.Out;
using CLDC_DataCore.SocketModule;
using System.Threading;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Const;
using CLDC_DataCore.SocketModule.Packet;
using CLDC_Comm.Enum;
using CLDC_DeviceDriver.PortFactory;
using CLDC_DataCore;
using CLDC_DeviceDriver.Drivers.Clou.DllPackage;
using CLDC_DataCore.Function;

namespace CLDC_DeviceDriver.Drivers.Clou
{
    #region 枚举
    /// <summary>
    /// 试验类型
    /// </summary>
    public enum enmTaskType
    {
        AutoThreadMethod = -1,
        电能误差 = 0,
        需量周期 = 1,
        时钟日误差 = 2,
        脉冲计数 = 3,
        对标 = 4,
        走字 = 5,
        设置预付费试验 = 6,
        多功能脉冲计数 = 7,
        误差板功耗数据 = 8,
        读取误差板温度 = 9,
        只返回成功失败的命令14 = 10,
        只返回成功失败的命令16 = 11,
        只返回成功失败的命令18 = 12,
        耐压 = 13,
        压接电机延时时间 = 14

    }
    /// <summary>
    /// 标准脉冲类型
    /// </summary>
    enum enmStdPulseType
    {
        标准时钟脉冲 = 0,
        标准电能脉冲 = 1
    }
    #endregion

    class Driver : DriverBase, IDriver
    {
        public bool[] bSelectBw = new bool[0];
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 2;

        /// <summary>
        /// 重试间隔等待时间 ms
        /// </summary>
        public static int RETRYTTIME = 500;

        #region ---------设备端口---------
        /// <summary>
        /// 标准表端口
        /// </summary>
        private StPortInfo[] m_MeterStd;

        /// <summary>
        /// 红外通讯设备端口
        /// </summary>
        private StPortInfo[] m_InfraredPort;

        /// <summary>
        /// 误差板端口通讯
        /// </summary>
        private StPortInfo[] m_arrErrorPort;


        /// <summary>
        /// 485端口
        /// </summary>
        private StPortInfo[] m_arrRs485Port;




        #endregion



        #region ----------私有变量----------
        /// <summary>
        /// 本次升源电压
        /// </summary>
        private float curU = 0;
        /// <summary>
        /// 本次升源电流
        /// </summary>
        private float curI = 0;
        /// <summary>
        /// 当前标准表常数，查表得到
        /// </summary>
        private int intBzMeterConst = 4000000;
        /// <summary>
        /// 是否加谐波
        /// </summary>
        private bool m_XieBo = false;
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool m_IsRuning;
        ///// <summary>
        ///// 标准表端口工作标志，在获取标准表常数时不允许读标准表信息
        ///// </summary>
        //private bool isstdMeterPortWorking = false;
        /// <summary>
        /// 当前试验类型
        /// </summary>
        private enmTaskType m_curTaskType = enmTaskType.电能误差;
        /// <summary>
        /// 标准表端口使用完毕时的事件
        /// 使用标准表端口时，锁住整个类，防止其它人使用，
        /// 增加该属性，用来通知使用完成
        /// </summary>
        private AutoResetEvent stdMeterPortCompletedEvent = new AutoResetEvent(false);
        /// <summary>
        /// 误差板控制命令是否返回
        /// </summary>
        private bool m_CL188LReturnType = true;
        #endregion

        public Driver(int bws, string[] arrayDevice)
            : base(bws, arrayDevice)
        {
            InitSetting(arrayDevice);
        }

        protected void InitSetting(string[] arrayDevice)
        {
            CLDC_DataCore.MessageController.Instance.AddMessage("开始初始化台体仪表设备...", 7);
            if (!DeviceFactory.Instance.InitialDeviceSetting(arrayDevice))
            {
                return;
            }

            DeviceControl.Instance.LoadDeviceControl();

            CardReaderControl.Instance.LoadCardControl();

            CLDC_DataCore.MessageController.Instance.AddMessage("台体仪表设备初始化完成!", 7);
        }



        #region ---------消息事件---------
        /// <summary>
        /// 外发进度消息
        /// </summary>
        /// <param name="msg">消息内容</param>
        private void outMessage(string msg)
        {
            if (CallBack != null)
            {
                CallBack(msg);
            }
        }
        #endregion

        #region --------私有函数--------

        /// <summary>
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool SetCurFunctionOnOrOff(bool IsOnOff, byte state)
        {
            bool result = false;
            if (IsOnOff)
            {
                result = StartTestControl(state);
            }
            else
            {
                result = StopTestControl(state);
            }
            return result;
        }
        /// <summary>
        /// 控制表位和误差板
        /// </summary>
        /// <param name="TypeNo">功能号：1进入跳闸检测、2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸</param>
        /// <returns></returns>
        public bool SetRelayControl(int TypeNo)
        {
            int[] ret = new int[g_Bws];
            for (int j = 0; j < g_Bws; j++)
            {

                if (GlobalUnit.blnYaoJianMeter[j] == false)
                {
                    continue;
                }

                if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                {
                    int Flag = 1;
                    int meterControlType = GlobalUnit.g_CUS.DnbData.MeterGroup[j].Mb_intFKType;
                    ret[j] = DeviceControl.Instance.Dev_DeviceControl[0].RelayControl(j + 1, TypeNo, meterControlType, ref Flag);
                }
            }
            if (Array.IndexOf(ret, 1) > -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 设置误差板设置脉冲通道和类型 
        /// </summary>
        /// <param name="p_bol_Positions"></param>
        /// <param name="wcchannelno">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟,0x06：耐压实验 0x07：多功能脉冲计数试验</param>
        /// <param name="pulsetype">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="gygy">脉冲类型,0=共阳,1=共阴</param>
        /// <param name="EnergyChangeType">电能走字选择位，1为显示脉冲间隔时间，0为显示脉冲计数</param>
        /// <param name="dgnwcchannelno">多功能误差通道号,1=日计时，2=需量脉冲</param>
        /// <param name="checktype">检定类型0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验</param>
        public bool SetPulseChannelAndTypeControl(int wcchannelno, int pulsetype, int gygy, int EnergyChangeType, int dgnwcchannelno, int checktype)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetPulseChannelAndType(GlobalUnit.blnYaoJianMeter, wcchannelno, pulsetype, gygy, EnergyChangeType, dgnwcchannelno, checktype);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置时钟频率
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterTimeFreq">标准表时钟频率</param>
        /// <param name="meterTimeFreq">被检表时钟频率</param>
        /// <param name="meterPulseNum">电表脉冲常数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public bool SetClockFrequencyControl(int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetClockFrequency(GlobalUnit.blnYaoJianMeter, stdMeterTimeFreq, meterTimeFreq, meterPulseNum);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 启动检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型 0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标,5=预付费功能检定,6=耐压实验,7=多功能脉冲计数试验</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public bool StartTestControl(byte verifyType)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].StartTest(GlobalUnit.blnYaoJianMeter, verifyType);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 停止检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型 0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标,5=预付费功能检定,6=耐压实验,7=多功能脉冲计数试验</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public bool StopTestControl(byte verifyType)
        {
            int ret = -1;
            if (GlobalUnit.IsCL3112)
            {
                FuncMstate( 0xFE);
            }

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].StopTest(GlobalUnit.blnYaoJianMeter, verifyType);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置电能误差检定时脉冲参数
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterConst">标准表脉冲常数</param>
        /// <param name="stdPulseFreq">标准脉冲频率</param>
        /// <param name="stdMeterConstShorttime">标准脉冲缩放倍数</param>
        /// <param name="meterConst">被检表脉冲常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="meterConstZooms">被检脉冲缩放倍数</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public bool SetEnergePulseParamsControl(int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int[] meterConst, int[] circles, int meterConstZooms)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetEnergePulseParams(GlobalUnit.blnYaoJianMeter, stdMeterConst, stdPulseFreq, stdMeterConstShorttime, meterConst, circles, meterConstZooms);
            }

            return ret == 0 ? true : false;
        }



        /// <summary>
        /// 控制表位继电器是否旁路
        /// </summary>
        /// <param name="ControlType">旁路继电器状态:1：旁路 0：正常</param>
        /// <returns></returns>
        public bool SetLoadRelayControl(bool[] MeterPosition, int ControlType)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetLoadRelayControl(MeterPosition, ControlType);
            }

            return ret == 0 ? true : false;
        }


        /// <summary>
        /// 191B设置脉冲通道
        /// </summary>
        /// <returns></returns>
        public bool SetTimePulse(bool isTime)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetTimePulse(isTime);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        ///获取标准表常数
        /// </summary>
        /// <returns></returns>
        public bool GetBzMeterConst(float MaxU, float MaxI, ref string Const)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].GetBzMeterConst(MaxU, MaxI, ref Const);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst">标准表常数</param>
        /// <returns></returns>

        public bool SetStdPulseConst(int pulseConst)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetStdPulseConst(pulseConst);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置标准表参数
        /// </summary>
        /// <param name="wiringMode"></param>
        /// <param name="powerMode"></param>
        /// <param name="calcType"></param>
        /// <returns></returns>
        public bool SetStdParams(int meterConst, int circle, int currentType, int wiringMode)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetStdParams(meterConst, circle, currentType, wiringMode);
            }

            return ret == 0 ? true : false;
        }


        /// <summary>
        /// 设置标准表接线方式
        /// </summary>
        /// <param name="wiringMode"></param>
     
        /// <returns></returns>
        public bool SetWiringMode(int wiringMode)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetWiringMode(1, wiringMode);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置标准表挡位
        /// </summary>
        /// <param name="Ua"></param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <returns></returns>
        public bool SetRangeControl(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetRange(Ua, Ub, Uc, Ia, Ib, Ic);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置标准表界面 1：谐波柱图界面2：谐波列表界面3：波形界面4：清除设置界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public bool SetDisplayFormControl(int formType)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetDisplayForm(formType);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 读取各相电压电流谐波幅值（分两帧读取数据）
        /// </summary>
        /// <param name="phase">相别，0是C相电压，1是B相电压，2是A相电压，3是C相电流，4是B相电流，5是A相电流</param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public bool ReadHarmonicArryControl(int phase, out float[] harmonicArry)
        {
            int ret = -1;
            float[] harmonic = new float[65];
            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].ReadHarmonicArry(phase, out harmonic);
            }
            harmonicArry = harmonic;
            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 获取误差每路负载
        /// </summary>
        /// <returns></returns>
        private int getOneLineLoads()
        {
            if (m_arrErrorPort.Length == 0) return 0;
            int nNum = g_Bws / m_arrErrorPort.Length;
            return nNum;
        }
        /// <summary>
        /// 表位到线路转换
        /// </summary>
        /// <param name="bw">表位号</param>
        /// <returns>属于第几路</returns>
        private int bwToLine(int bw)
        {
            if (getOneLineLoads() > 0)
            {
                return bw / getOneLineLoads();
            }
            return 0;
        }

        /// <summary>
        /// 转换当前要升源的测量方式
        /// 中的测试方式定义与检定器定义不一致。
        /// </summary>
        /// <param name="Clfs">测量方式0-7</param>
        /// <param name="pd">功率方向</param>
        /// <returns>测量方式</returns>
        private byte getClfs(CLDC_Comm.Enum.Cus_Clfs Clfs, Cus_PowerFangXiang pd)
        {
            /*   三相四线有功 = 0,
         三相四线无功 = 1,
         三相三线有功 = 2,
         三相三线无功 = 3,
         二元件跨相90 = 4,
         二元件跨相60 = 5,
         三元件跨相90 = 6,
             
        三相四线=0,
        三相三线=1,
        二元件跨相90=2,
        二元件跨相60=3,
        三元件跨相90=4,
        单相=5
             
             */
            bool IsYouGong = ((pd == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功) || (pd == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功));
            byte clfs = (byte)Clfs;
            if (clfs == 5)                            //单相台统一划分为三相四线
            {
                clfs = 0;
            }
            clfs += 2;                              //先保证后面对齐
            if (clfs < 4)                             //处理前面没有对齐部分
            {
                if (clfs == 3)
                {
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
                else
                {
                    clfs--;
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
            }
            return clfs;
        }

        #endregion

        protected override void InitSetting()
        {
            //CLDC_DeviceDriver.Setting.ConfigHelper.LoadConfig(0);
            //base.Setting = new CLDC_DeviceDriver.Drivers.Clou.Setting.Setting();
        }

        /// <summary>
        /// 获取端口号
        /// </summary>
        /// <param name="ErrorNo"></param>
        /// <returns></returns>
        public int getDuankouNumber(int ErrorNo)
        {
            int iNum = 0;
            int oneErrorNum = 15;//一排所挂表数目 单项四排每排15个表位，三相两排每排10个表位

            oneErrorNum = getOneLineLoads();
            iNum = ErrorNo / oneErrorNum;

            return iNum;
        }



        #region IDriver 成员

        #region --------消息事件---------
        public MsgCallBack CallBack
        {
            get
            {
                return msgCallBack;
            }
            set
            {
                msgCallBack = value;
            }
        }
        #endregion

        #region---------停止当前操作--------
        /// <summary>
        /// 停止当前操作
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            //停止当前操作功能
            m_IsRuning = true;
            m_IsRuning = false;
            return true;
        }
        #endregion

        #region ---------联机操作---------
        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        public bool Link()
        {
            //bool bLink = false;
            string strRunMsg = "";
            //bSelectBw = new bool[g_Bws];
            //for (int i = 0; i < g_Bws; i++)
            //{
            //    bSelectBw[i] = GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn;
            //}

            int EnResult = CLDC_Encryption.CLEncryption.API.SouthGridEncryptionAPI.OpenDevice(GlobalUnit.ENCRYPTION_MACHINE_TYPE, GlobalUnit.ENCRYPTION_MACHINE_IP, Convert.ToInt32(GlobalUnit.ENCRYPTION_MACHINE_PORT), Convert.ToInt32(GlobalUnit.ENCRYPTION_MACHINE_OUTTIME));
            string strEnMessage = string.Format("连接加密机{0}。", EnResult == 0 ? "成功" : "失败,请检查电脑IP是否正确或软件配置的加密机IP是否正确。");
            MessageController.Instance.AddMessage(strEnMessage, 7, (EnResult == 0 ? 0 : 2));

            //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
            {
                bool linkMeterOk = false;
                bool linkPowerOk = false;

                if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                {
                    int link1 = DeviceControl.Instance.Dev_DeviceControl[0].ConnectRefMeter();
                    if (link1 == 0)
                    {
                        linkMeterOk = true;
                        MessageController.Instance.AddMessage("标准表联机成功");
                    }
                    else if (link1 == 1)
                    {
                        MessageController.Instance.AddMessage("标准表联机失败");
                    }

                    int link2 = DeviceControl.Instance.Dev_DeviceControl[0].ConnectPower();
                    if (link2 == 0)
                    {
                        linkPowerOk = true;
                        MessageController.Instance.AddMessage("功率源联机成功");
                    }
                    else if (link2 == 1)
                    {
                        MessageController.Instance.AddMessage("功率源联机失败");
                    }
                    int link3 = DeviceControl.Instance.Dev_DeviceControl[0].ConnectWcB();
                    if (link3 == 0)
                    {

                        MessageController.Instance.AddMessage("误差板联机成功");
                    }
                    else if (link3 == 1)
                    {
                        MessageController.Instance.AddMessage("误差板联机失败");
                    }
                    int link4 = DeviceControl.Instance.Dev_DeviceControl[0].Connect191B();
                    if (link4 == 0)
                    {

                        MessageController.Instance.AddMessage("时基源联机成功");
                    }
                    else if (link4 == 1)
                    {
                        MessageController.Instance.AddMessage("时基源联机失败");
                    }
                    int link5 = DeviceControl.Instance.Dev_DeviceControl[0].Connect2029D();
                    if (link5 == 0)
                    {

                        MessageController.Instance.AddMessage("2029D联机成功");
                    }
                    else if (link5 == 1)
                    {
                        MessageController.Instance.AddMessage("2029D联机失败");
                    }

                    int link6 = DeviceControl.Instance.Dev_DeviceControl[0].Connect2029B();
                    if (link6 == 0)
                    {

                        MessageController.Instance.AddMessage("2029B联机成功");
                    }
                    else if (link6 == 1)
                    {
                        MessageController.Instance.AddMessage("2029B联机失败");
                    }

                    int link7 = DeviceControl.Instance.Dev_DeviceControl[0].ConnectCarrier();
                    if (link7 == 0)
                    {

                        MessageController.Instance.AddMessage("载波板联机成功");
                    }
                    else if (link7 == 1)
                    {
                        MessageController.Instance.AddMessage("载波板联机失败");
                    }

                    int link8 = DeviceControl.Instance.Dev_DeviceControl[0].ConnectTemperatureBoard();
                    if (link8 == 0)
                    {

                        MessageController.Instance.AddMessage("温控板联机成功");
                    }
                    else if (link8 == 1)
                    {
                        MessageController.Instance.AddMessage("温控板联机失败");
                    }



                    if (linkPowerOk)
                    {
                        return true;
                    }
                }
                else
                {
                    string message = string.Format("标准表联机{0}。", linkMeterOk ? "成功" : "失败");
                    strRunMsg += message;
                    MessageController.Instance.AddMessage(strRunMsg, 7, (linkMeterOk ? 0 : 2));

                    string Powermessage = string.Format("功率源联机{0}。", linkPowerOk ? "成功" : "失败");
                    strRunMsg += Powermessage;
                    MessageController.Instance.AddMessage(strRunMsg, 7, (linkPowerOk ? 0 : 2));

                    return false;
                }
            }
            return false;

        }
        #endregion

        #region ---------控制源输出---------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="strGlys"></param>
        /// <param name="sng_Ub">额定值</param>
        /// <param name="sng_Ib">额定值</param>
        /// <param name="sng_IMax">最大值</param>
        /// <param name="sng_xUb_A">电压值</param>
        /// <param name="sng_xUb_B">电压值</param>
        /// <param name="sng_xUb_C">电压值</param>
        /// <param name="sng_xIb_A">电流值</param>
        /// <param name="sng_xIb_B">电流值</param>
        /// <param name="sng_xIb_C">电流值</param>
        /// <param name="element">元件</param>
        /// <param name="sng_UaPhi"></param>
        /// <param name="sng_UbPhi"></param>
        /// <param name="sng_UcPhi"></param>
        /// <param name="sng_IaPhi"></param>
        /// <param name="sng_IbPhi"></param>
        /// <param name="sng_IcPhi"></param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="IsDuiBiao"></param>
        /// <param name="isQianDong"></param>
        /// <param name="bln_IsNxx">true逆相序 false正相序</param>
        /// <returns></returns>
        public bool PowerOn(Cus_Clfs clfs, Cus_PowerFangXiang glfx, string strGlys
            , float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A
            , float sng_xUb_B, float sng_xUb_C, float sng_xIb_A
            , float sng_xIb_B, float sng_xIb_C, Cus_PowerYuanJian element
            , float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi
            , float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao
            , bool isQianDong, bool bln_IsNxx)
        {
            try
            {
                //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
                {
                    GlobalUnit.MaxU = Math.Max(sng_xUb_A, Math.Max(sng_xUb_B, sng_xUb_C));
                    GlobalUnit.MaxI = Math.Max(sng_xIb_A, Math.Max(sng_xIb_B, sng_xIb_C));

                    bool blnRst = false;
                    if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                    {
                        int TestMode = 0;
                        int PowerDirection = 0;
                        int Element = 0;
                        int wiringMode = 0;
                        int powerMode = 0;
                        int calcType = 0;
                        if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
                        {
                            powerMode = 1;
                        }
                        else
                        {
                            powerMode = 0;
                        }

                        if (clfs == Cus_Clfs.单相)
                        {
                            wiringMode = 5;
                        }
                        else if (clfs == Cus_Clfs.三相四线)
                        {
                            wiringMode = 0;
                        }
                        else
                        {
                            wiringMode = 1;
                        }
                        if (clfs == Cus_Clfs.三相四线 && glfx.ToString().Contains("有功"))
                        {
                            TestMode = 0;
                        }
                        else if (clfs == Cus_Clfs.三相四线 && glfx.ToString().Contains("无功"))
                        {
                            TestMode = 1;
                        }
                        else if (clfs == Cus_Clfs.三相三线 && glfx.ToString().Contains("有功"))
                        {
                            TestMode = 2;
                        }
                        else if (clfs == Cus_Clfs.三相三线 && glfx.ToString().Contains("无功"))
                        {
                            TestMode = 3;
                        }
                        else if (clfs == Cus_Clfs.二元件跨相90)
                        {
                            TestMode = 4;
                        }
                        else if (clfs == Cus_Clfs.二元件跨相60)
                        {
                            TestMode = 5;
                        }
                        else if (clfs == Cus_Clfs.三元件跨相90)
                        {
                            TestMode = 6;
                        }
                        else if (clfs == Cus_Clfs.单相 && glfx.ToString().Contains("有功"))
                        {
                            TestMode = 7;
                        }
                        else if (clfs == Cus_Clfs.单相 && glfx.ToString().Contains("无功"))
                        {
                            TestMode = 8;
                        }

                        if (glfx.ToString().Contains("正向"))
                        {
                            PowerDirection = 0;
                        }
                        else if (glfx.ToString().Contains("反向"))
                        {
                            PowerDirection = 1;
                        }
                        if (element == Cus_PowerYuanJian.H)
                        {
                            Element = 0;
                        }
                        else if (element == Cus_PowerYuanJian.A)
                        {
                            Element = 1;
                        }
                        else if (element == Cus_PowerYuanJian.B)
                        {
                            Element = 2;
                        }
                        else if (element == Cus_PowerYuanJian.C)
                        {
                            Element = 3;
                        }

                       

                       
                        outMessage("设置标准表参数");
                     
                       
                        float[] instValue = new float[40];
                        if (0 == DeviceControl.Instance.Dev_DeviceControl[0].ReadInstMetricAll(out instValue))
                        {
                            float Ua = instValue[0];
                            float Ub = instValue[1];
                            float Uc = instValue[2];
                            float Ia = instValue[3];
                            float Ib = instValue[4];
                            float Ic = instValue[5];
                            int RangeUa = 0;
                            int RangeUb = 0;
                            int RangeUc = 0;
                            int RangeIa = 0;
                            int RangeIb = 0;
                            int RangeIc = 0;

                            if (GlobalUnit.IsCL3112)
                            {
                                RangeUa = (int)instValue[34];
                                RangeUb = (int)instValue[35];
                                RangeUc = (int)instValue[36];
                                RangeIa = (int)instValue[37];
                                RangeIb = (int)instValue[38];
                                RangeIc = (int)instValue[39];
                                if (GlobalUnit.IsXieBo != true)
                                {

                              
                                if (Math.Min(RangeUa, Math.Min(RangeUb, RangeUc)) < GetCL3112RangeU(Math.Min(sng_xUb_A, Math.Min(sng_xUb_B, sng_xUb_C))))
                                {
                                    if (true == PowerOff())
                                    {
                                        Thread.Sleep(3000);
                                    }
                                    else
                                    {
                                        outMessage("关源失败");
                                        return false;
                                    }


                                }
                                else if (Math.Min(RangeIa, Math.Min(RangeIb, RangeIc)) < GetCL3112RangeI(Math.Min(sng_xIb_A, Math.Min(sng_xIb_B, sng_xIb_C)))
                                    || RangeIa > GetCL3112RangeI(0.2) || RangeIb > GetCL3112RangeI(0.2) || RangeIc > GetCL3112RangeI(0.2)
                                    || Math.Min(sng_xIb_A, Math.Min(sng_xIb_B, sng_xIb_C)) < 0.2)
                                {
                                    if (0 == DeviceControl.Instance.Dev_DeviceControl[0].PowerOffOnlyCurrent())
                                    {
                                        Thread.Sleep(3000);
                                    }
                                    else
                                    {
                                        outMessage("只关电流失败");
                                        return false;
                                    }


                                }

                                DeviceControl.Instance.Dev_DeviceControl[0].PowerOffOnlyCurrent();
                                }
                                int r = DeviceControl.Instance.Dev_DeviceControl[0].SetWiringMode(1, wiringMode);
                                if (r != 0)
                                {
                                    outMessage("设置标准表参数失败");
                                }
                                else
                                {
                                    outMessage("设置标准表参数成功");
                                }
                            }
                            else
                            {
                                if (GetCL3112RangeU(Math.Min(Ua, Math.Min(Ub, Uc))) < GetCL3112RangeU(Math.Min(sng_xUb_A, Math.Min(sng_xUb_B, sng_xUb_C))) && GlobalUnit.IsXieBo != true)
                                {
                                  
                                        if (true == PowerOff())
                                        {
                                            Thread.Sleep(5000);
                                        }
                                        else
                                        {
                                            outMessage("关源失败");
                                            return false;
                                        }
                                    
                                }

                                bool ret1 = SetStdParams(wiringMode, powerMode, 0, 0);
                                if (!ret1)
                                {
                                    outMessage("设置标准表参数失败");
                                }
                                else
                                {
                                    outMessage("设置标准表参数成功");
                                }

                            }




                            if (GlobalUnit.IsJxb == true)
                            {
                                DeviceControl.Instance.Dev_DeviceControl[0].SetLoopFeed(false);
                            }
                            else
                            {
                                DeviceControl.Instance.Dev_DeviceControl[0].SetLoopFeed(true);
                            }
                           
                            GlobalUnit.IsMonDanWei = true;
                            bln_IsNxx = !bln_IsNxx;
                            outMessage("设置标准表挡位");
                            bool ret = false;//

                            int ryt = 1;
                            GlobalUnit.IsMonDanWei = true;
                            if (GlobalUnit.IsJdb)
                            {
                                ret = SetRangeControl(sng_xUb_A, sng_xUb_B, sng_xUb_C, sng_xIb_A * 2, sng_xIb_B * 2, sng_xIb_C * 2);
                                ryt = DeviceControl.Instance.Dev_DeviceControl[0].SetCL309UIRange(sng_xUb_A, sng_xUb_B, sng_xUb_C, sng_xIb_A * 2, sng_xIb_B * 2, sng_xIb_C * 2);

                            }
                            else 
                            {
                                float MaxU = Math.Max(sng_xUb_A, Math.Max(sng_xUb_B, sng_xUb_C));
                                float MaxI = Math.Max(sng_xIb_A, Math.Max(sng_xIb_B, sng_xIb_C));


                                ret = SetRangeControl(MaxU, MaxU, MaxU, MaxI, MaxI, MaxI);
                            }
                            if (ret == false)
                            {
                                outMessage("设置标准表挡位失败");
                                return false;
                            }
                            else
                            {
                                outMessage("设置标准表挡位成功");
                            }





                            GlobalUnit.IsMonDanWei = false;
                            int intRst = DeviceControl.Instance.Dev_DeviceControl[0].PowerOn(GlobalUnit.blnYaoJianMeter, TestMode, PowerDirection, strGlys, sng_xUb_A, sng_xUb_B, sng_xUb_C, sng_xIb_A,
                          sng_xIb_B, sng_xIb_C, sng_UaPhi, sng_UbPhi, sng_UcPhi, sng_IaPhi, sng_IbPhi, sng_IcPhi, Element, sng_Freq, bln_IsNxx);
                            int int_break = 0;
                         
                            int int_Time = 10;
                            while (int_break < 10000)
                            {
                                outMessage("新升源，等待源稳定" + int_Time-- + "秒");
                                //System.Windows.Forms.Application.DoEvents();
                                System.Threading.Thread.Sleep(1000);
                                int_break += 1000;
                            }
                            if (intRst == 0)
                            {
                                blnRst = true;
                            }
                          //  GlobalUnit.IsMonDanWei = true;
                        }
                        else
                        {
                            outMessage("读取标准表信息失败，升源失败");
                            return false;
                        }
                       
                       
                      
                        
                   


                    }
                    return blnRst;
                }
            }
            finally
            {
                GlobalUnit.EnableReadStd = true;
            }
        }



        /// <summary>
        /// 保存当前源输出值
        /// </summary>
        /// <param name="ua"></param>
        /// <param name="ub"></param>
        /// <param name="uc"></param>
        /// <param name="ia"></param>
        /// <param name="ib"></param>
        /// <param name="ic"></param>
        private void remberUandI(float ua, float ub, float uc, float ia, float ib, float ic)
        {
            curU = ua;
            if (ub > curU) curU = ub;
            if (uc > curU) curU = uc;
            curI = ia;
            if (ib > curI) curI = ib;
            if (ic > curI) curI = ic;
        }



        public int getiClfs(CLDC_Comm.Enum.Cus_Clfs wir, Cus_PowerFangXiang pulse)
        {
            //    单相 = 1,
            //三相三线 = 2,
            //三相四线 = 3,
            int pulseType = 0;
            if (wir == CLDC_Comm.Enum.Cus_Clfs.单相)
            { pulseType = 7; }
            else if (wir == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                if (Convert.ToInt32(pulse) <= 2)
                { pulseType = 2; }
                else
                { pulseType = 3; }
            }
            else if (wir == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                if (Convert.ToInt32(pulse) <= 2)
                { pulseType = 0; }
                else
                { pulseType = 1; }
            }
            return pulseType;
        }
        #endregion

        #region ---------关源操作--------
        /// <summary>
        /// 关源操作
        /// </summary>
        /// <returns></returns>
        public bool PowerOff()
        {
            //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
            {
                bool blnRst = false;
                int intRst = 1;
                if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                {
                    intRst = DeviceControl.Instance.Dev_DeviceControl[0].PowerOff();

                }
                blnRst = intRst == 0 ? true : false;
                return blnRst;
            }
        }


        #endregion


        #region ----------读取检定数据----------
        //zhengrubin
        /// <summary>
        /// 初始化日计时误差
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="im"></param>
        /// <param name="MeterFre"></param>
        /// <returns></returns>
        public bool InitTimeAccuracy(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, float[] MeterFre, float[] bcs, int[] quans)
        {
            bSelectBw = IsOnOff;
            bool bln_Rst = true;
            bool ret = false;
            m_IsRuning = true;
            //停止当前检定
            //     stopTask();
            if (!StopTestControl(2))
            {

            }
            if (!m_IsRuning) return true;

            Thread.Sleep(300);
            if (!m_IsRuning) return true;
            //第二步：设置校验参数
            ret = SetTimePulse(true);
            if (ret == false)
            {
                outMessage("设置191B设置脉冲通道失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置191B设置脉冲通道成功");
            }
            Thread.Sleep(300);
            if (!m_IsRuning) return true;

            //第三步：设置日计时误差参数
            ret = SetPulseChannelAndTypeControl(5, 1, 0, 0, 1, 2);
            if (ret == false)
            {
                outMessage("设置误差板设置脉冲通道和类型失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置误差板设置脉冲通道和类型成功");
            }
            Thread.Sleep(300);
            //第四步：设置时钟频率
            ret = SetClockFrequencyControl(500000, 1, 10);
            if (ret == false)
            {
                //   outMessage("设置时钟频率失败");
                // bln_Rst &= false;
            }
            else
            {
                //    outMessage("设置时钟频率成功");
            }
            Thread.Sleep(300);

            if (!m_IsRuning) return true;
            //启动误差板
            if (!StartTestControl(2))
            {

            }
            return bln_Rst;

        }

        public bool InitError(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, int[] bcs, int[] quans, int wccs, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            bool ret = false;
            if (im == null || im.Length == 0) return false;
            bool bln_Rst = true;
            m_IsRuning = true;
            int intPowerMode = 1;
            int intClfs = 0;
            int intGlfx = 1;
            string Const = "";
          
            // if (Stop) return ;
            if (GlobalUnit.IsJdb)
            {
                GetBzMeterConst(GlobalUnit.MaxU , GlobalUnit.MaxI * 2, ref Const);

            }
            else
            {
                 GetBzMeterConst(GlobalUnit.MaxU , GlobalUnit.MaxI, ref Const);
            }



            if (!StopTestControl(0))
            {

            }
            if (glfx == Cus_PowerFangXiang.正向有功)
            {
                intGlfx = 1;
            }
            else if (glfx == Cus_PowerFangXiang.反向有功)
            {
                intGlfx = 2;
            }
            else if (glfx == Cus_PowerFangXiang.正向无功)
            {
                intGlfx = 3;
              //  MeterProtocolAdapter.Instance.SetPulseCom(1);
            }
            else if (glfx == Cus_PowerFangXiang.反向无功)
            {
                intGlfx = 4;
            }





            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
            {
                intPowerMode = 1;
            }
            else
            {
                intPowerMode = 0;
            }

            if (clfs == Cus_Clfs.单相)
            {
                intClfs = 5;
            }
            else if (clfs == Cus_Clfs.三相四线)
            {
                intClfs = 0;
            }
            else
            {
                intClfs = 1;
            }

            //停止当前检定
            //    stopTask();
            if (!m_IsRuning) return true;

            bSelectBw = IsOnOff;

            if (!m_IsRuning) return true;
            Thread.Sleep(300);
            ret = SetStdPulseConst(Convert.ToInt32(Const));

            if (ret == false)
            {
                outMessage("设置标准表常数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置标准表常数成功");
            }
            Thread.Sleep(300);
            if (!m_IsRuning) return true;
            //设置功能参数
            ret = SetStdParams(intClfs, intPowerMode, 0, 0);

            if (ret == false)
            {
                outMessage("设置标准表参数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置标准表参数成功");
            }
            ret = SetTimePulse(false);
            if (ret == false)
            {
                outMessage("191B设置脉冲通道失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("191B设置脉冲通道成功");
            }

            ret = SetPulseChannelAndTypeControl(intGlfx - 1, 1, 0, 0, 0, 0);
            if (ret == false)
            {
                outMessage("设置误差板设置脉冲通道和类型失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置误差板设置脉冲通道和类型成功");
            }
            ret = SetEnergePulseParamsControl(Convert.ToInt32(Const), 1, 1,bcs, quans, 1);
            if (ret == false)
            {
                outMessage("设置电能误差检定时脉冲参数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置电能误差检定时脉冲参数成功");
            }



            if (!m_IsRuning) return true;

            // 选脉冲通道0x46
            //im[0] = CLDC_Comm.Enum.Cus_GyGyType.共阳;
            //int iglfx = (int)glfx - 1;
            //if (!SelectPulseChannel((byte)iglfx, im, IsOnOff))
            //{
            //    bln_Rst &= false;
            //}
            //if (!startTask())
            //{
            //    bln_Rst &= false;
            //}
            if (!StartTestControl(0))
            {
                bln_Rst &= false;
            }

            this.currentWorkFlow = WorkFlow.基本误差;
            return bln_Rst;
        }

        /// <summary>
        /// 读取检定数据
        /// </summary>
        /// <param name="IsOnOff">要检列表</param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError[] ReadWcb(bool[] IsOnOff, bool state)
        {
            stError[] tagError = new stError[IsOnOff.Length];
            ReadWcbManager readWcb = new ReadWcbManager();
            readWcb.BwStatus = IsOnOff;
            readWcb.m_curTaskType = m_curTaskType;
            readWcb.Start(state);
            //readWcb.WaitAllThreaWorkDone();
            tagError = readWcb.tagError;
            return tagError;
        }
        public stError ReadWc(int bw)
        {
            stError tagError = new stError();
            byte Type = (byte)0;
            int Index = 0;
            int Num = 0;
            string Data = "";

            int result = DeviceControl.Instance.Dev_DeviceControl[0].ReadCurrentData(bw, ref Type, ref  Index, ref Num, ref Data);
            if (result == 0)
            {
                tagError.szError = Data;
                tagError.Index = Num;
                tagError.MeterConst = Type;
                tagError.MeterIndex = Index;
            }

            return tagError;
        }


        #endregion

        #region ---------读取标准信息----------
        public stStdInfo ReadStdInfo()
        {
            stStdInfo tagInfo = new stStdInfo();

            //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
            {
                //return tagInfo;
                if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                {
                    float[] instValue = new float[40];
                    int result =  DeviceControl.Instance.Dev_DeviceControl[0].ReadInstMetricAll(out instValue);
                     if (result == 0)
                    {
                        tagInfo.Ua = instValue[0];
                        tagInfo.Ub = instValue[1];
                        tagInfo.Uc = instValue[2];
                        tagInfo.Ia = instValue[3];
                        tagInfo.Ib = instValue[4];
                        tagInfo.Ic = instValue[5];
                        tagInfo.Phi_Ua = instValue[6];
                        tagInfo.Phi_Ub = instValue[7];
                        tagInfo.Phi_Uc = instValue[8];
                        tagInfo.Phi_Ia = instValue[9];
                        tagInfo.Phi_Ib = instValue[10];
                        tagInfo.Phi_Ic = instValue[11];
                        tagInfo.Pa = instValue[16];
                        tagInfo.Pb = instValue[17];
                        tagInfo.Pc = instValue[18];
                        tagInfo.P = instValue[19];
                        tagInfo.Qa = instValue[20];
                        tagInfo.Qb = instValue[21];
                        tagInfo.Qc = instValue[22];
                        tagInfo.Q = instValue[23];
                        tagInfo.Sa = instValue[24];
                        tagInfo.Sb = instValue[25];
                        tagInfo.Sc = instValue[26];
                        tagInfo.S = instValue[27];
                        tagInfo.COS = instValue[31];
                        tagInfo.SIN = instValue[32];
                        tagInfo.Freq = instValue[33];
                    }
                }
            }

            return tagInfo;
        }

        #endregion

        #region --------RS485通讯---------
        private string[] m_arrBlt = new string[0];

        /// <summary>
        /// 初始化485参数
        /// </summary>
        /// <param name="arBtl"></param>
        /// <returns></returns>
        public bool InitRs485(string[] arBtl)
        {
            //只记录
            m_arrBlt = arBtl;
            return true;
        }

        #endregion


        #region 供电类型
        /// <summary>
        /// 供电类型，耐压供电=1、载波供电=2、普通供电=3、二回路=5、else一回路
        /// 耐压保护=6、
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="meterType">false直接式，true互感式</param>
        /// <returns></returns>
        public bool SetPowerSupplyType(int elementType, bool isMeterTypeHGQ)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].SetPowerSupplyType(elementType, isMeterTypeHGQ) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("设置2029D供电类型失败");

            }
            else
            {
                outMessage("设置2029D供电类型成功");
            }

            return ret;

        }

        #region 李鑫 20200618
        public bool SetRelay(int[] switchOpen, int[] switchClose)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].SetRelay(switchOpen, switchClose) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("设置2029继电器失败");

            }
            else
            {
                outMessage("设置2029D继电器成功");
            }
            return ret;
        }
        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="instValue">输出测量数据</param>
        /// <returns></returns>
        public bool ReadTemperature(int Index, out float[] instValue)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].ReadTemperature(Index, out instValue) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("温控板" + (Index) + "读取温度失败");

            }
            else
            {
                outMessage("温控板" + (Index) + "读取温度成功");
            }
            return ret;
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="Flags">需要温控标志位</param>
        /// <param name="Temperatures">控制温度</param>
        /// <returns></returns>
        public bool SetTemperature(int Index, bool[] Flags, float[] Temperatures)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].SetTemperature(Index, Flags, Temperatures) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("温控板" + (Index) + "设置温度失败");

            }
            else
            {
                outMessage("温控板" + (Index) + "设置温度成功");
            }
            return ret;
        }

        /// <summary>
        /// 开风扇
        /// </summary>
        /// <param name="Flag">控制标志位</param>
        /// <returns></returns>
        public bool OpenFan(bool Flag)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].OpenFan(Flag) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("开风扇失败");

            }
            else
            {
                outMessage("开风扇成功");
            }
            return ret;
        }
        /// <summary>
        /// 开锁
        /// </summary>
        /// <returns></returns>
        public bool OpenLock()
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].OpenLock() == 0 ? true : false;
            if (ret == false)
            {
                outMessage("开锁失败");

            }
            else
            {
                outMessage("开锁成功");
            }
            return ret;
        }

        #endregion
        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        public bool PacketToCarrierInit(int int_Fn, bool state, int num)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].PacketToCarrierInit(int_Fn, state, num) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("初始化2041失败");

            }
            else
            {
                outMessage("初始化2041成功");
            }

            return ret;
        }
        /// <summary>
        /// 不带数据域，返回确认/否认帧的命令
        /// </summary>
        /// <param name="byt_AFN">AFN</param>
        /// <param name="int_Fn">Fn</param>
        public bool PacketToCarrierInitA(byte byt_AFN, int int_Fn, int num)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].PacketToCarrierInitA(byt_AFN, int_Fn, num) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("暂停路由失败");

            }
            else
            {
                outMessage("暂停路由成功");
            }

            return ret;
        }
        /// <summary>
        /// 控制命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">数据域</param>
        public bool PacketToCarrierCtr(int int_Fn, byte[] Data, int num)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].PacketToCarrierCtr(int_Fn, Data, num) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("控制2041失败");

            }
            else
            {
                outMessage("控制2041成功");
            }

            return ret;
        }
        /// <summary>
        /// 添加载波从节点
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public bool PacketToCarrierAddAddr(int int_Fn, byte[] Data, bool state, int num)
        {
            bool ret;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].PacketToCarrierAddAddr(int_Fn, Data, state, num) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("添加载波从节点失败");

            }
            else
            {
                outMessage("添加载波从节点成功");
            }

            return ret;
        }
        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>

        public bool PacketTo3762Frame(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex, int num)
        {
            bool ret;
            RFrame645 = null;
            ret = DeviceControl.Instance.Dev_DeviceControl[0].Packet645To3762Frame(Frame645, byt_DLTType, ref RFrame645, state, int_BwIndex, num) == 0 ? true : false;
            if (ret == false)
            {
                outMessage("打包645成376.2失败");

            }
            else
            {
                outMessage("打包645成376.2成功");
            }

            return ret;
        }


        public bool SetTimeMaiCon(bool isTime)
        {
            bool ret;
            ret = SetTimePulse(false);
            if (ret == false)
            {
                outMessage("191B设置脉冲通道失败");

            }
            else
            {
                outMessage("191B设置脉冲通道成功");
            }

            return ret;
        }


        /// <summary>
        /// 初始化需量周期项目、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="xlzqSeconds">需量周期</param>
        /// <param name="hccs">滑差次数</param>
        /// <returns></returns>
        public bool InitDemandPeriod(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] xlzqSeconds, int[] hccs)
        {
            bool bln_Rst = true;
            bool ret = false;
            ret = SetPulseChannelAndTypeControl(5, 1, 0, 0, 0, 0);
            if (ret == false)
            {
                outMessage("设置误差板设置脉冲通道和类型失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置误差板设置脉冲通道和类型成功");
            }

            ret = SetTimePulse(false);
            if (ret == false)
            {
                outMessage("191B设置脉冲通道失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("191B设置脉冲通道成功");
            }

            if (!StartTestControl(1))
            {
                bln_Rst &= false;
            }



            return bln_Rst;
        }


        /// <summary>
        /// 读取GPS时间，如果失败会返回当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime ReadGPSTime()
        {
            int ret = -1;
            string strRevTime = "";

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].ReadGPSTime(ref strRevTime);
            }

            return DateTimes.FormatStringToDateTime(strRevTime);

        }



        /// <summary>
        /// 读取误差板的功耗数据
        /// </summary>
        /// <param name="blnBwIndex">要读的表位</param>
        /// <param name="flt_PD">出数据结构</param>
        public void ReadErrPltGHPram(bool[] blnBwIndex, out stGHPram[] flt_PD)
        {
            stGHPram[] flt = new stGHPram[1];
            flt_PD = flt;

        }

        /// <summary>
        /// 读取功耗，功耗板
        /// </summary>
        /// <param name="int_BwIndex">功耗板ID，一般等于表位号</param>
        /// <param name="byt_Chancel">通道号，1=A相电压,2=A相电流,3=B相电压,4=B相电流,5=C相电压,6=C相电流</param>
        /// <param name="flt_PD">传出，float[4]{电压有效值,电流有效值,基波有功功率,基波无功功率}</param>
        public bool ReadPowerDissipation(int int_BwIndex, byte byt_Chancel, out float[] flt_PD)
        {
            flt_PD = new float[4];



            return true;
        }
        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="Phase">数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <param name="frameAry">合成上行报文</param>
        /// <returns></returns>
        public bool SetHarmonic(int[][] int_XTSwitch, Single[][] sng_Value, Single[][] sng_Phase)
        {
            bool[] bResult = new bool[int_XTSwitch.Length];

            for (int i = 0; i < int_XTSwitch.Length; i++)
            {
                if (int_XTSwitch[i][0] != 1)
                {
                    bResult[i] = true;
                    continue;
                }
                bResult[i] = DeviceControl.Instance.Dev_DeviceControl[0].SetHarmonic(i, int_XTSwitch[i], sng_Value[i], sng_Phase[i]) == 0 ? true : false;
            }

            if (Array.IndexOf(bResult, false) > -1)
                return false;
            else
                return true;

        }
        /// <summary>
        /// 设置谐波总开关
        /// </summary>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public bool SetHarmonicSwitch(bool bSwitch)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetHarmonicSwitch(bSwitch);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 设置波形
        /// </summary>
        /// <param name="ua"></param>
        /// <param name="ub"></param>
        /// <param name="uc"></param>
        /// <param name="ia"></param>
        /// <param name="ib"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public bool SettingWaveformSelection(int ua, int ia, int ub, int ib, int uc, int ic)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SettingWaveformSelection(ua, ia, ub, ib, uc, ic);
            }

            return ret == 0 ? true : false;
        }
        /// <summary>
        /// 设置尖顶波-1 平顶波-2
        /// </summary>
        /// <returns></returns>
        public bool SetJd_Pd(int ua, int ia, int ub, int ib, int uc, int ic)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetJd_Pd(ua, ia, ub, ib, uc, ic);
            }

            return ret == 0 ? true : false;
        }


        #region   设置电压暂降，电流快速变化
        /// <summary>
        /// 设置暂降电压电流阀值
        /// </summary>
        /// <param name="Wave">float[6]类型 </param>
        /// Wave[0] ua  ;Wave[1] ub;Wave[2] uc;Wave[3] ia;Wave[4] ib;Wave[5] ic
        /// <returns></returns>
        public bool SetDropWave(float[] Wave)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetDropWave(Wave);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 设置暂降电压电流时间
        /// </summary>
        /// <param name="Time">int[2]</param>
        /// Ua,Ub,Uc,Ia,Ib,Ic
        /// /// <returns></returns>
        public bool SetDropTime(int[] Time)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetDropTime(Time);
            }

            return ret == 0 ? true : false;
        }

        /// <summary>
        /// 设置暂降电压电流开关
        /// </summary>
        /// <param name="Switch">bool[6]</param>
        ///  bool[0] ua  ;bool[1] ub;bool[2] uc;bool[3] ia;bool[4] ib;bool[5] ic
        /// <returns></returns>
        public bool SetDropSwitch(bool[] Switch)
        {
            int ret = -1;


            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetDropSwitch(Switch);
            }

            return ret == 0 ? true : false;
        }

        public bool SetErrCalcType(int calcType)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].SetErrCalcType(calcType);
            }

            return ret == 0 ? true : false;
        }

        public bool ReadTestEnergy(out float testEnergy, out long PulseNum)
        {
            int ret = -1;
            float Energy = 0.0f;
            long Num = 0;
            testEnergy = 0.0f;
            PulseNum = 0;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].ReadTestEnergy(out Energy, out Num);
                testEnergy = Energy;
                PulseNum = Num;
            }

            return ret == 0 ? true : false;
        }

        #endregion
        #endregion

        #region 远程上电
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OnOrOff">true 上电，false 断电</param>
        public void RemoteControlOnOrOff(bool OnOrOff)
        {
            //if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
            {
                if (CardReaderControl.Instance.Dev_CardControl != null && CardReaderControl.Instance.Dev_CardControl.Length > 0)
                {
                    int WorkFlag = OnOrOff ? 0 : 1;
                    int result = CardReaderControl.Instance.Dev_CardControl[0].SwitchCardState(WorkFlag);
                    MessageController.Instance.AddMessage("调用SwitchCardState返回值：" + result + ",参数=" + WorkFlag);
                }
            }
            System.Threading.Thread.Sleep(2000);
        }
        #endregion

        #region --------初始化启动参数--------
        /// <summary>
        /// 初始化启动参数，不带升源
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="sng_Ub"></param>
        /// <param name="sng_Ib"></param>
        /// <param name="sng_IMax"></param>
        /// <param name="sng_xUb_A"></param>
        /// <param name="sng_xUb_B"></param>
        /// <param name="sng_xUb_C"></param>
        /// <param name="sng_xIb_A"></param>
        /// <param name="sng_xIb_B"></param>
        /// <param name="sng_xIb_C"></param>
        /// <param name="im"></param>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool InitStartUp(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes, int[] meterconst)
        {
            m_IsRuning = true;

            if (!StopTestControl(0))
            {
                m_IsRuning = false;
            }
         
            //    string Const = "";
            //    GetBzMeterConst(ref Const);

            bool ret = false;
            if (im == null || im.Length == 0) return false;
            bool bln_Rst = true;
            m_IsRuning = true;
            int intPowerMode = 1;
            int intClfs = 0;
            int intGlfx = 1;
            string Const = "";
            GetBzMeterConst(GlobalUnit.MaxU, GlobalUnit.MaxI, ref Const);
            // if (Stop) return ;

         
            if (glfx == Cus_PowerFangXiang.正向有功)
            {
                intGlfx = 1;
            }
            else if (glfx == Cus_PowerFangXiang.反向有功)
            {
                intGlfx = 2;
            }
            else if (glfx == Cus_PowerFangXiang.正向无功)
            {
                intGlfx = 3;
            }
            else if (glfx == Cus_PowerFangXiang.反向无功)
            {
                intGlfx = 4;
            }





            if (glfx == Cus_PowerFangXiang.正向有功 || glfx == Cus_PowerFangXiang.反向有功)
            {
                intPowerMode = 1;
            }
            else
            {
                intPowerMode = 0;
            }

            if (clfs == Cus_Clfs.单相)
            {
                intClfs = 5;
            }
            else if (clfs == Cus_Clfs.三相四线)
            {
                intClfs = 0;
            }
            else
            {
                intClfs = 1;
            }

            //停止当前检定
            //    stopTask();
            if (!m_IsRuning) return true;

            bSelectBw = IsOnOff;

            if (!m_IsRuning) return true;
            Thread.Sleep(300);
            ret = SetStdPulseConst(Convert.ToInt32(Const));

            if (ret == false)
            {
                outMessage("设置标准表常数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置标准表常数成功");
            }
            Thread.Sleep(300);
            if (!m_IsRuning) return true;
            //设置功能参数
            ret = SetStdParams(intClfs, intPowerMode, 0, 0);

            if (ret == false)
            {
                outMessage("设置标准表参数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置标准表参数成功");
            }
            ret = SetTimePulse(false);
            if (ret == false)
            {
                outMessage("191B设置脉冲通道失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("191B设置脉冲通道成功");
            }

            SetPulseChannelAndTypeControl((int)glfx - 1, 1, 0, 1, 0, 3);
            if (ret == false)
            {
                outMessage("设置误差板设置脉冲通道和类型失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置误差板设置脉冲通道和类型成功");
            }
            int[] quans = new int[g_Bws];
            for (int i = 0; i < g_Bws; i++)
            {
                quans[i] = 1;
            }
            ret = SetEnergePulseParamsControl(Convert.ToInt32(Const), 1, 1, meterconst, quans, 1);
            if (ret == false)
            {
                outMessage("设置电能误差检定时脉冲参数失败");
                bln_Rst &= false;
            }
            else
            {
                outMessage("设置电能误差检定时脉冲参数成功");
            }

            // SetEnergePulseParamsControl(Const,1,1,)




            if (!StartTestControl(3))
            {
                m_IsRuning = false;
            }

            outMessage("开始启动当前功能");

            //   this.currentWorkFlow = WorkFlow.启动;
            return m_IsRuning;
        }
        #endregion

        #region ---------初始化潜动参数----------
        /// <summary>
        /// 初始化潜动参数
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="sng_Ub"></param>
        /// <param name="sng_Ib"></param>
        /// <param name="sng_IMax"></param>
        /// <param name="sng_xUb_A"></param>
        /// <param name="sng_xUb_B"></param>
        /// <param name="sng_xUb_C"></param>
        /// <param name="sng_xIb_A"></param>
        /// <param name="sng_xIb_B"></param>
        /// <param name="sng_xIb_C"></param>
        /// <param name="im"></param>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool InitCreeping(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes)
        {
            m_IsRuning = true;

            if (!StopTestControl(0))
            {

            }
            SetPulseChannelAndTypeControl((int)glfx - 1, 1, 0, 0, 0, 3);
            if (!StartTestControl(3))
            {
                m_IsRuning = false;
            }

            outMessage("开始启动当前功能");

            //   this.currentWorkFlow = WorkFlow.启动;
            return m_IsRuning;
        }
        #endregion


        #region 初始化：走字，时段
        /// <summary>
        /// 初始化走字参数
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="glfx"></param>
        /// <param name="im"></param>
        /// <param name="impluseCount"></param>
        /// <returns></returns>
        public bool InitZZ(bool[] IsOnOff, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] impluseCount)
        {
            m_IsRuning = true;
            if (!StopTestControl(0))
            {
                // m_IsRuning = false;
            }

            SetPulseChannelAndTypeControl((int)glfx - 1, 1, 0, 0, 0, 3);

           // if (GlobalUnit.IsCL3112)
           // {
                FuncMstate(0x05);
           // }

            if (!SetErrCalcType(2))
            {
                // m_IsRuning = false;
            }
            if (!StartTestControl(3))
            {
                //   m_IsRuning = false;
            }

            //  if (!))//channelID
            //  {
            //    m_IsRuning = false;
            //  }


            this.currentWorkFlow = WorkFlow.走字;
            return m_IsRuning;
        }

        /// <summary>
        /// 设置标准表走字界面
        /// </summary>
        /// <param name="funcType">
        /// 控制类型 0x00：默认界面 
        /// 0x01: 功率测量界面
        /// 0x02: 伏安测量界面
        /// 0x03: 电能误差与标准差界面
        /// 0x05: 电能量走字界面
        /// 0x09: 谐波测量界面
        /// 0x10: 稳定度测量界面
        /// 0xFE: 清除界面设置(返回默认界面) </param>
        /// <returns></returns>
        public bool FuncMstate(int funcType)
        {
            int ret = -1;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                ret = DeviceControl.Instance.Dev_DeviceControl[0].FuncMstate(funcType);
            }

            return ret == 0 ? true : false;
        }

        #endregion
        #endregion

        #region 其它
        bool[] getYJMeter()
        {
            bool[] meterYJ = new bool[g_Bws];
            for (int i = 0; i < g_Bws; i++)
            {
                if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    meterYJ[i] = true;
                }
                else
                {
                    meterYJ[i] = false;
                }
            }
            return meterYJ;
        }

        #endregion


        #region 起动读取脉冲数

        public bool ReadQueryCurrentErrorControl(int id, int CheckType, out int ErrNum, out string ErrData, out string TQtime)
        {

            int ret = 1;
            ErrNum = 0;
            ErrData = string.Empty;
            TQtime = string.Empty;
            int Num = 0;
            string Data = string.Empty;
            string time = string.Empty;

            if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {

                ret = DeviceControl.Instance.Dev_DeviceControl[0].ReadQueryCurrentError(id, CheckType, out  Num, out  Data, out  time);

                ErrNum = Num;
                ErrData = Data;
                TQtime = time;
            }

            return ret == 0 ? true : false;

        }





        #endregion





        /// <summary>
        /// 显示南网设备配置界面
        /// </summary>
        public void ShowDriverConfig()
        {
            if (DeviceControl.Instance.Dev_DeviceControl != null || DeviceControl.Instance.Dev_DeviceControl.Length > 0)
            {
                DeviceControl.Instance.Dev_DeviceControl[0].ShowDriverConfig();
            }
        }

        /// <summary>
        /// 显示南网读写卡器配置界面
        /// </summary>d
        public void ShowCardReaderConfig()
        {
            if (CardReaderControl.Instance.Dev_CardControl != null && CardReaderControl.Instance.Dev_CardControl.Length > 0)
            {
                CardReaderControl.Instance.Dev_CardControl[0].ShowCardReaderConfig();
            }
        }


        /// <summary>
        /// 电压档位ID
        /// </summary>
        /// <param name="voltage">V</param>
        /// <returns></returns>
        private int GetCL3112RangeU(double voltage)
        {
            if (voltage <= 66)
            {
                return 4;
            }
            else if (voltage <= 132)
            {
                return 3;
            }
            else if (voltage <= 264)
            {
                return 2;
            }
            else if (voltage <= 528)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 电流档位ID
        /// </summary>
        /// <param name="current">A</param>
        /// <returns></returns>
        private int GetCL3112RangeI(double current)
        {
            if (current <= 0.011)
            {
                return 12;
            }
            else if (current <= 0.022)
            {
                return 11;
            }
            else if (current <= 0.055)
            {
                return 10;
            }
            else if (current <= 0.11)
            {
                return 9;
            }
            else if (current <= 0.22)
            {
                return 8;
            }
            else if (current <= 0.55)
            {
                return 7;
            }
            else if (current <= 1.1)
            {
                return 6;
            }
            else if (current <= 2.2)
            {
                return 5;
            }
            else if (current <= 5.5)
            {
                return 4;
            }
            else if (current <= 11)
            {
                return 3;
            }
            else if (current <= 22)
            {
                return 2;
            }
            else if (current <= 55)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
