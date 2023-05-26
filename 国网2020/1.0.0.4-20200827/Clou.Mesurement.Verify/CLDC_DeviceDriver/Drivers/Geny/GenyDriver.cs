using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DeviceDriver.Drivers.Geny.Packets;
using CLDC_DeviceDriver.Drivers.Geny.Packets.In;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;
using CLDC_Comm.SocketModule;
using CLDC_Comm.SocketModule.Packet;
using CLDC_Comm.Utils;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    internal class GenyDriver : DriverBase, IDriver
    {
        public const string GENY_GPS_SOCKNAME = "Geny_GPSSockName";
        public const string GENY_TIMEBASE_SOCKNAME = "Geny_TimeBase_SockName";

        /// <summary>
        /// 控制源代码
        /// </summary>
        private short Com_Power_Port = 1; //源端口

        /// <summary>
        /// 485源端口设置参数
        /// </summary>
        private string Com_Power_Setting = "19200,n,8,2"; //源通信配置

        /// <summary>
        /// GPS串口速率设置
        /// </summary>
        private string Com_Gps_Setting = "";

        /// <summary>
        /// 时基源串口设置
        /// </summary>
        private string Com_TimeBase_Setting = "";

        /// <summary>
        /// 时基源口号
        /// </summary>
        private int Com_TimeBase_Port = 1;
        /// <summary>
        /// 
        /// </summary>
        private string szStdMeterName = string.Empty; //标准表名称


        /// <summary>
        /// 第一表位误差板设备编号
        /// </summary>
        private byte FirstMeterNo = 1;
        /// <summary>
        /// 重试次数
        /// </summary>
        private int RetryConunt = 6;

        /// <summary>
        /// 重试间隔
        /// </summary>
        private int RetryTime = 500;

        /// <summary>
        /// 当前的标准表常数值，升源时计算
        /// </summary>
        private double StdConst = 1F;

        /// <summary>
        /// 升源后，等待温定时间
        /// 秒计算
        /// </summary>
        private int WaitTimeAfterPowerOn = 9;

        /// <summary>
        /// 台体485通道数量
        /// </summary>
        private int RS485ChannelCount = 1;

        /// <summary>
        /// 共阴 方式脉冲起始值
        /// </summary>
        private byte ImpluseTypeStartValue = 0;

        /// <summary>
        /// 在读取时间时，是否使用时基源
        /// </summary>
        private bool isUseTimeBase = true;

        /// <summary>
        /// 485工作标志，由于标准表与485共用一个端口，所以485工作时不允许对其它通道进行操作
        /// </summary>
        private bool isRs485Working = false;

        #region PowerOn上次调用时的参数

        private CLDC_Comm.Enum.Cus_PowerYuanJiang Last_PowerOnParam_YuanJian = CLDC_Comm.Enum.Cus_PowerYuanJiang.Error;

        #endregion

        /// <summary>
        /// 是否设置过谐波
        /// </summary>
        private bool last_IsSetXiebo = false;

        /// <summary>
        /// 外部是否调用停止
        /// </summary>
        private bool isStop = false;

        /// <summary>
        /// 485 使用完毕时的事件
        /// 使用485时，锁住整个类，防止其它人使用，
        /// 增加该属性，用来通知使用完成
        /// </summary>
        private AutoResetEvent rs485CompletedEvent = new AutoResetEvent(false);

        #region IDriver 成员

        public MsgCallBack CallBack
        {
            get
            {
                return base.msgCallBack;
            }
            set
            {
                base.msgCallBack = value;
            }
        }

        /// <summary>
        /// 是否显示日志
        /// </summary>
        public bool ShowLog
        {
            get;
            set;
        }

        /// <summary>
        /// 已重写
        /// 
        /// </summary>
        /// <param name="bws">表位号</param>
        public GenyDriver(int bws)
            : base(bws)
        {
            //加载配置
            this.Setting.LoadFromFile();

            Com_Power_Port = short.Parse(this.Setting["台体通信_串口号"]);                      //源端口
            Com_Power_Setting = this.Setting["台体通信_串口参数"];                              //源波特率
            szStdMeterName = this.Setting["标准表名称"];                                        //标准表名
            FirstMeterNo = byte.Parse(this.Setting["1表位设备号"]);
            this.Com_Gps_Setting = this.Setting["GPS串口设置"];
            this.Com_TimeBase_Setting = this.Setting["时基源串口设置"];

            if (byte.TryParse(this.Setting["脉冲方式_共阴"], out this.ImpluseTypeStartValue) == false)
            {
                this.ImpluseTypeStartValue = 0;
            }
            if (int.TryParse(this.Setting["485通道数"], out this.RS485ChannelCount) == false)
            {
                this.RS485ChannelCount = 1;
            }
            if (int.TryParse(this.Setting["升源后等待稳定时间"], out this.WaitTimeAfterPowerOn) == false)
            {
                this.WaitTimeAfterPowerOn = 9;
            }
            if (int.TryParse(this.Setting["台体操作重试次数"], out this.RetryConunt) == false)
            {
                this.RetryConunt = 3;
            }
            if (int.TryParse(this.Setting["每次重试时间"], out this.RetryTime) == false)
            {
                this.RetryTime = 500;
            }
            int.TryParse(this.Setting["时基源串口号"], out this.Com_TimeBase_Port);
            if ("是".Equals(this.Setting["是否直接与时基源连接"]))
            {
                isUseTimeBase = true;
            }
            else
            {
                isUseTimeBase = false;
            }

            //使用封装数据包
            RegisterPort((int)Com_Power_Port, Com_Power_Setting, 1000, 100);       //注册源表控制端口
            SockPool.Instance.AddComSock(GENY_TIMEBASE_SOCKNAME, this.Com_TimeBase_Port, Com_TimeBase_Setting, 1000, 50);
            SockPool.Instance.AddComSock(GENY_GPS_SOCKNAME, Com_Power_Port, this.Com_Gps_Setting, 3000, 100);
        }

        /// <summary>
        /// 显示误差板指定信息
        /// </summary>
        /// <param name="intBwh"></param>
        /// <param name="intSerialno"></param>
        public void ShowSerialNoToErrPanel(int intBwh, int intSerialno)
        {
        }
        /// <summary>
        /// 控制CT切换
        /// </summary>
        /// <param name="intCurrentCT"></param>
        /// <returns></returns>
        public bool ControlCtSwitch(int intCurrentCT)
        {
            return true;
        }
        protected override void InitSetting()
        {
            this.Setting = new CLDC_DeviceDriver.Drivers.Geny.Setting.Settings();
        }

        public bool Stop()
        {
            this.isStop = true;
            return true;
        }

        public bool Link()
        {
            this.WriteStringToConsole("开始联机指令");
            return true;
        }

        /// <summary>
        /// 设置警示灯
        /// </summary>
        /// <param name="lightType"></param>
        /// <returns></returns>
        public bool SetLightType(CLDC_Comm.Enum.Cus_LightType lightType)
        {
            return true;
        }

        /// <summary>
        /// 切换载波供电
        /// </summary>
        /// <param name="bType">true,供电；false，停电</param>
        /// <returns></returns>
        public bool SetSwitchTypeForCarrier(bool bType)
        {
            return true;
        }

        public bool UnLink()
        {
            this.WriteStringToConsole("开始执行脱机指令");
            return true;
        }

        /// <summary>
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="isOn">true为停止，false为启动</param>
        /// <returns></returns>
        public bool SetCurFunctionOnOrOff(bool isOn)
        {
            return true;
        }

        /// <summary>
        /// 已重写，
        /// 设置谐波
        /// </summary>
        /// <param name="int_XTSwitch">谐波次数</param>
        /// <param name="sng_Value">谐波值</param>
        /// <param name="sng_Phase">谐波的相位角</param>
        /// <returns></returns>
        public bool SetHarmonic(int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
            isStop = false;
            try
            {
                this.last_IsSetXiebo = true;
                //设置标准表的谐波
                Geny_RequestStdMeterReadK6DDataPacket stdP = new Geny_RequestStdMeterReadK6DDataPacket("", Geny_StandMeterDataType.EStart);
                Geny_ReplyStdMeterPacket stdR = new Geny_ReplyStdMeterPacket();

                if (this.SendPacketToStdMeter(stdP, stdR) == false)
                {
                    this.OnMessage("设置标准表谐波采样失败");
                    return false;
                }

                for (int i = 0; i < int_XTSwitch.Length && i < 3; i++)
                {
                    Geny_RequestVoltageHarmonicMultiPacket packet = new Geny_RequestVoltageHarmonicMultiPacket(sng_Value[i], int_XTSwitch[i], sng_Phase[i], (PhaseType)(i));
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    if (isStop == true)
                        return true;
                    if (this.SendPacketToPower(packet, recvPacket) == false)
                    {
                        this.OnMessage("设置:" + (PhaseType)i + "电压谐波失败");
                        //return false;
                    }
                }

                for (int i = 3; i < 6 && i < int_XTSwitch.Length; i++)
                {
                    Geny_RequestCurrentHarmonicMultiPacket packet = new Geny_RequestCurrentHarmonicMultiPacket(sng_Value[i], int_XTSwitch[i], sng_Phase[i], (PhaseType)(i - 3));
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    if (this.SendPacketToPower(packet, recvPacket) == false)
                    {
                        this.OnMessage("设置:" + (PhaseType)(i - 3) + "谐波电流失败");
                    }
                }
            }
            catch (Exception ex)
            {
                this.WriteStringToConsole(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 升源
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
        /// <param name="element"></param>
        /// <param name="sng_UaPhi"></param>
        /// <param name="sng_UbPhi"></param>
        /// <param name="sng_UcPhi"></param>
        /// <param name="sng_IaPhi"></param>
        /// <param name="sng_IbPhi"></param>
        /// <param name="sng_IcPhi"></param>
        /// <param name="sng_Freq"></param>
        /// <param name="IsDuiBiao"></param>
        /// <param name="IsQianDong"></param>
        /// <param name="bln_IsNxx"></param>
        /// <returns></returns>
        public bool PowerOn(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, string strGlys, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, CLDC_Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            //升源的时候，不允许其它任何线程访问
            bool ret = false;
            lock (this)
            {
                ret = PowerOnImpl(clfs, glfx, sng_Ub, sng_Ib, sng_IMax, sng_xUb_A, sng_xUb_B, sng_xUb_C, sng_xIb_A, sng_xIb_B, sng_xIb_C, element, sng_UaPhi, sng_UbPhi, sng_UcPhi, sng_IaPhi, sng_IbPhi, sng_IcPhi, sng_Freq, IsDuiBiao, IsQianDong, bln_IsNxx);
            }
            return ret;
        }

        private bool PowerOnImpl(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, CLDC_Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            isStop = false;
            try
            {
                WriteStringToConsole("开始升源:");
                this.OnMessage("开始升源");
                //参数预处理
                if (element == CLDC_Comm.Enum.Cus_PowerYuanJiang.Error)
                {
                    element = CLDC_Comm.Enum.Cus_PowerYuanJiang.H;
                }
                //从功率源件从解析应该设置的 相
                PhaseType[] phaseTypes = Geny_RequestPowerModePacket.ParseYuanJiang(clfs, element);

                //第二步：控制接线方式
                foreach (PhaseType phaseType in phaseTypes)
                {
                    Geny_RequestPowerModePacket powerSendP = new Geny_RequestPowerModePacket(phaseType, bln_IsNxx ? GenyPowerType.逆 : GenyPowerType.正, clfs, LCType.C, element, glfx);
                    GenyRecvPacket recvPowerP = new GenyRecvPacket();
                    if (isStop) return true;
                    if (this.SendPacketToPower(powerSendP, recvPowerP) == false)
                    {
                        this.OnMessage("设置" + phaseType + "接线方式失败:" + recvPowerP.ReciveResult);
                        return false;
                    }
                    Thread.Sleep(2000);
                }
                //第三步:设置标准表 接线，有功无功，电流档位
                //要检测一下标准表类型
                WriteStringToConsole("开始设置标准表：测量方式，有功无功，电流档位");
                Geny_StdK6DTestType k6dTestType = GenyUtil.GetStdTestType(clfs, glfx);
                GenyActiveType activeType = glfx.ToString().IndexOf("有功") >= 0 ? GenyActiveType.Active : GenyActiveType.Reactive;
                GenyStdMeterCurrentLevel currentLevel = GenyUtil.GetCurrentLevel(sng_xIb_A, sng_xIb_B, sng_xIb_C);
                Geny_RequestStdMeterSetAllPacket stdSetPacket = new Geny_RequestStdMeterSetAllPacket("", k6dTestType, activeType, currentLevel);
                GenyRecvPacket stdRecv = new Geny_ReplyStdMeterPacket();
                if (this.SendPacketToStdMeter(stdSetPacket, stdRecv) == false)
                {
                    this.OnMessage("设置标准表：有功无功，接线方式，电流档位失败:" + stdRecv.ReciveResult);
                    return false;
                }
                WriteStringToConsole("开始设置 设备电压，电流");

                //台体电流，相位
                foreach (PhaseType phaseType in phaseTypes)
                {
                    float cosData = 0;
                    float voltageData = 0;
                    float currentData = 0;
                    if (phaseType == PhaseType.A)
                    {
                        voltageData = sng_xUb_A;
                        currentData = sng_xIb_A;
                        cosData = sng_IaPhi - sng_UaPhi;
                    }
                    else if (phaseType == PhaseType.B)
                    {
                        voltageData = sng_xUb_B;
                        currentData = sng_xIb_B;
                        cosData = sng_IbPhi - sng_UbPhi;
                    }
                    else if (phaseType == PhaseType.C)
                    {
                        voltageData = sng_xUb_C;
                        currentData = sng_xIb_C;
                        cosData = sng_IcPhi - sng_UcPhi;
                    }
                    else
                    { }

                    if (isStop) return true;
                    Geny_RequestCurrentPacket curp = new Geny_RequestCurrentPacket(phaseType, 0);
                    GenyRecvPacket recv2 = new GenyRecvPacket();
                    float tmpCurrent = currentData;
                    //如果是对色标，且台体支持 自身提供对色标电路，就不需要输出电流
                    if (IsDuiBiao && (CLDC_DeviceDriver.Drivers.Geny.Setting.Settings.SiBiaoType == GenySeBiaoType.Auto || CLDC_DeviceDriver.Drivers.Geny.Setting.Settings.SiBiaoType == GenySeBiaoType.NotSupport))
                    {
                        tmpCurrent = 0;
                    }
                    Geny_RequestPowerOnPacket powerOn = new Geny_RequestPowerOnPacket(phaseType, tmpCurrent, voltageData, cosData, sng_Freq, false);
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    if (isStop) return true;
                    if (this.SendPacketToPower(powerOn, recvPacket) == false)
                    {
                        this.OnMessage("设置" + phaseType + "相电压，电流，COS，频率失败");
                        return false;
                    }
                }

                //保存是一次控源数据
                Last_PowerOnParam_YuanJian = element;
                //标准表常数
                if (sng_xIb_A <= 1F)
                {
                    StdConst = 1000000000F;
                }
                else if (sng_xIb_A <= 10F)
                {
                    StdConst = 100000000F;
                }
                else
                {
                    StdConst = 10000000F;
                }
                this.OnMessage("等待电源稳定," + this.WaitTimeAfterPowerOn + "s");
                Thread.Sleep(this.WaitTimeAfterPowerOn * 1000);
                WriteStringToConsole("升源操作结束");
                return true;
            }
            catch (Exception ex)
            {
                WriteStringToConsole(ex.Message);
                return false;
            }
        }

        public bool PowerOff()
        {
            isStop = false;
            PhaseType[] phases = Geny_RequestPowerModePacket.ParseYuanJiang(Cus_Clfs.三相四线, this.Last_PowerOnParam_YuanJian);
            WriteStringToConsole("开始关源");

            try
            {
                foreach (PhaseType phaseType in phases)
                {
                    Geny_RequestPowerOnPacket powerOn = new Geny_RequestPowerOnPacket(phaseType, 0, 0, 0, 50, false);
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    if (this.SendPacketToPower(powerOn, recvPacket) == false)
                    {
                        this.OnMessage("关源" + phaseType + "失败");
                    }
                }
                if (last_IsSetXiebo == true)
                {
                    this.OnMessage("关于标准表谐波采样");
                    Geny_RequestStdMeterReadK6DDataPacket stdS = new Geny_RequestStdMeterReadK6DDataPacket("", Geny_StandMeterDataType.EEnd);
                    Geny_ReplyStdMeterPacket stdR = new Geny_ReplyStdMeterPacket();
                    if (this.SendPacketToStdMeter(stdS, stdR) == false)
                    {
                        this.OnMessage("关闭标准表谐波失败");
                    }
                    this.OnMessage("所有电源基波复位");
                    foreach (PhaseType phase in phases)
                    {
                        Geny_RequestPowerHarmonicResetPacket hrS = new Geny_RequestPowerHarmonicResetPacket(phase);
                        GenyRecvPacket hrR = new GenyRecvPacket();
                        if (this.SendPacketToPower(hrS, hrR) == false)
                        {
                            this.OnMessage("复位" + phase + "相基波失败");
                        }
                    }
                    this.last_IsSetXiebo = false;
                }
            }
            catch (Exception ex)
            {
                this.OnMessage(ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 初始化预热参数
        /// </summary>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲共阴共阳类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <returns>初始化是否成功</returns>
        public bool InitWarmUp(CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            //预热时误差板显示脉冲计数
            //Geny_RequestClearMarkPacket sendPacket = new Geny_RequestClearMarkPacket(0xC7);
            //GenyRecvPacket recvPacket = new GenyRecvPacket();
            //if(SendPacketToWcb(sendPacket,recvPacket))
            return true;
        }

        /// <summary>
        /// 初始化对色标
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="sng_U"></param>
        /// <param name="sng_I"></param>
        /// <param name="PulseCount"></param>
        /// <param name="im"></param>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool InitDuiSeBiao(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, float sng_U, float sng_I, int PulseCount, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            WriteStringToConsole("启动色标指令");
            ClearErrorBoardMark(0xC7);
            isStop = false;
            byte imc = this.GetImpluseChannel(glfx);
            Geny_RequestDuSheBiaoPacket duSheBiao = new Geny_RequestDuSheBiaoPacket((byte)199, imc, this.ComPulseType2GenyPulseType(im[0]));
            GenyRecvPacket recv = new GenyRecvPacket();
            if (this.SendPacketToWcb(duSheBiao, recv) == false)
            {
                return false;
            }
            this.currentWorkFlow = WorkFlow.对色标;
            return true;
        }

        /// <summary>
        /// 初始化启动参数 
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="im"></param>
        /// <param name="IsOnOff"></param>
        /// <param name="startTimes"></param>
        /// <returns></returns>
        public bool InitStartUp(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes)
        {
            isStop = false;
            //启动时间
            float sng_qidongTime = CLDC_Comm.Utils.ArrayHelper.Max(startTimes);
            //脉冲通道
            byte imc = this.GetImpluseChannel(glfx);
            //发送启动参数
            Geny_RequestQiDongPacket diDong = new Geny_RequestQiDongPacket(0xC7, (int)sng_qidongTime, 1, imc, this.ComPulseType2GenyPulseType(im[0]));
            GenyRecvPacket recvQiDong = new GenyRecvPacket();
            if (this.SendPacketToWcb(diDong, recvQiDong) == false)
            {
                this.OnMessage("误差板执行启动命令失败:" + recvQiDong.ReciveResult);
                return false;
            }
            this.currentWorkFlow = WorkFlow.启动;
            return true;
        }

        /// <summary>
        /// 初始化潜动数据
        /// </summary>
        /// <param name="clfs">测量方式【接线方式】</param>
        /// <param name="glfx">接线方式</param>
        /// <param name="im">共阴共阳类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="creepTimes">各表位潜动时间[单位：秒]</param>
        /// <returns>初始化潜动参数是否成功</returns>
        public bool InitCreeping(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] creepTimes)
        {
            isStop = false;
            int maxCreepTime = CLDC_Comm.Utils.ArrayHelper.Max(creepTimes);
            byte imc = this.GetImpluseChannel(glfx);
            //全部参数相同
            Cus_GyGyType useGygyType = ComPulseType2GenyPulseType(im[0]);
            Geny_RequestQianDongPacket qianDong = new Geny_RequestQianDongPacket(0xC7, maxCreepTime, 1, imc, useGygyType);
            GenyRecvPacket recvQianDong = new GenyRecvPacket();
            if (this.SendPacketToWcb(qianDong, recvQianDong) == false)
            {
                this.OnMessage("设置表位潜动指令失败");
                return false;
            }
            this.currentWorkFlow = WorkFlow.潜动;
            return true;
        }


        /// <summary>
        /// 初始化基本误差
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="glfx"></param>
        /// <param name="bcs"></param>
        /// <param name="quans"></param>
        /// <param name="wccs"></param>
        /// <param name="im"></param>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool InitError(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, float[] bcs, int[] quans, int wccs, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            isStop = false;
            float lastBcs = CLDC_Comm.Utils.ArrayHelper.Max(bcs);
            int lastQuanShu = CLDC_Comm.Utils.ArrayHelper.Max(quans);
            Geny_RequestBasicErrorPacket errorPacket = new Geny_RequestBasicErrorPacket()
            {
                MeterIndex = 0xC7,//广播地址
                CircleCount = (ushort)lastQuanShu,
                StandardPredcitValue = (int)(StdConst / lastBcs * lastQuanShu),
                SignalChannel = this.GetImpluseChannel(glfx),
                PulseType = ComPulseType2GenyPulseType(im[0]),
                ErrorUpLimit = 999,
                ErrorDownLimit = -999,
                SendID = 199,
            };
            GenyRecvPacket errorRecv = new GenyRecvPacket();
            for (int j = 0; j < 3; j++)
            {
                if (this.SendPacketToWcb(errorPacket, errorRecv) == false)
                {
                    this.OnMessage("启动表位误差板失败,将再次尝试");
                }
                else
                    break;
            }
            this.currentWorkFlow = WorkFlow.基本误差;
            return true;
        }

        /// <summary>
        /// 初始化日计时误差
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="im"></param>
        /// <param name="MeterFre"></param>
        /// <returns></returns>
        public bool InitTimeAccuracy(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, float[] MeterFre, float[] bcs, int[] quans)
        {
            isStop = false;
            bool retValue = true;
            bool isAllTheSame = true;
            float lastFreq = 0F;
            Cus_GyGyType lastGygyType = Cus_GyGyType.共阴;
            for (int i = 0; i < IsOnOff.Length; i++)
            {
                if (lastFreq == 0)
                {
                    lastFreq = MeterFre[i];
                    lastGygyType = im[i];
                }
                else
                {
                    if (lastFreq != MeterFre[i] || lastGygyType != im[i])
                    {
                        isAllTheSame = false;
                        break;
                    }
                }
            }

            if (isAllTheSame)
            {
                Cus_GyGyType useGygyType = ComPulseType2GenyPulseType(lastGygyType);
                Geny_RequestCumulationDayPacket cdPacket = new Geny_RequestCumulationDayPacket(199, lastFreq, this.GetImpluseChannel(GenyImplusechannels.日计时误差), useGygyType);
                GenyRecvPacket recvPacket = new GenyRecvPacket();
                if (isStop) return true;
                if (this.SendPacketToWcb(cdPacket, recvPacket) == false)
                {
                    this.OnMessage("启动误差板失败");
                    return false;
                }
            }
            else
            {
                for (int i = 0; i < IsOnOff.Length; i++)
                {
                    if (isStop) return true;
                    if (IsOnOff[i] == false) continue;
                    Cus_GyGyType useGygyType = ComPulseType2GenyPulseType(im[i]);
                    Geny_RequestCumulationDayPacket cdPacket = new Geny_RequestCumulationDayPacket((byte)(FirstMeterNo + i), MeterFre[i], this.GetImpluseChannel(GenyImplusechannels.日计时误差), useGygyType);
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    if (isStop) return true;
                    if (this.SendPacketToWcb(cdPacket, recvPacket) == false)
                    {
                        this.OnMessage("启动脉冲记数:" + (FirstMeterNo + i) + "失败");
                        return false;
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// 初始化时段投切，不需要操作
        /// </summary>
        /// <param name="IsOnOff"></param>
        ///<param name="pulseCount"></param>
        /// <returns></returns>
        public bool InitTimePeriod(bool[] IsOnOff, int[] pulseCount)
        {
            //把脉冲板切换到误差计数
            Geny_RequestAccumulationImpulsePacket sendPacket = new Geny_RequestAccumulationImpulsePacket(193);
            GenyRecvPacket recvPacket = new GenyRecvPacket();
            if (!SendPacketToWcb(sendPacket, recvPacket))
                return false;
            return recvPacket.ReciveResult == RecvResult.OK;
        }

        /// <summary>
        /// 初始化最大需量误差
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="xlzqSeconds"></param>
        /// <param name="hccs"></param>
        /// <returns></returns>
        public bool InitMaxDemand(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            isStop = false;
            if (this.OpenSignleChannle(GenyDoorType.Door_Error) == false)
                return false;
            if (isStop) return true;
            Geny_RequestMaxXLPacket maxXLPacket = new Geny_RequestMaxXLPacket(0xC7, StdConst.ToString(), (byte)xlzqSeconds, (byte)hccs);
            GenyRecvPacket recvPacket = new GenyRecvPacket();
            if (isStop) return true;
            if (this.SendPacketToWcb(maxXLPacket, recvPacket) == false)
            {
                this.OnMessage("初始化最大需量误差失败:" + recvPacket.ReciveResult);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化需量周期误差
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="im"></param>
        /// <param name="xlzqSeconds"></param>
        /// <param name="hccs"></param>
        /// <returns></returns>
        public bool InitDemandPeriod(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] xlzqSeconds, int[] hccs)
        {
            isStop = false;
            bool retValue = true;

            for (int i = 0; i < IsOnOff.Length; i++)
            {
                if (isStop) return true;
                if (IsOnOff[i] == false) continue;
                Geny_RequestXLZQPacket xlzqPacket = new Geny_RequestXLZQPacket((byte)(FirstMeterNo + i), xlzqSeconds[0], GetImpluseChannel(GenyImplusechannels.需量周期误差), CLDC_Comm.Enum.Cus_GyGyType.共阴);
                GenyRecvPacket recvPacket = new GenyRecvPacket();
                if (isStop) return true;
                if (this.SendPacketToWcb(xlzqPacket, recvPacket) == false)
                {
                    this.OnMessage("启动需量周期误差失败");
                    return false;
                }
            }
            return retValue;
        }

        /// <summary>
        /// 初始化走字
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="glfx"></param>
        /// <param name="im"></param>
        /// <param name="impluseCount">未使用</param>
        /// <returns></returns>
        public bool InitZZ(bool[] IsOnOff, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] impluseCount)
        {
            if (isStop) return true;
            byte imc = this.GetImpluseChannel(glfx);
            Geny_RequestZouZiPacket zouZiPacket = new Geny_RequestZouZiPacket(0xC7, 100000000, imc, this.ComPulseType2GenyPulseType(im[0]));
            GenyRecvPacket recv = new GenyRecvPacket();
            if (isStop) return true;
            if (this.SendPacketToWcb(zouZiPacket, recv) == false)
            {
                this.OnMessage(string.Format("启动第{0}表位走字指令失败", (0xC7)));
                return false;
            }
            this.currentWorkFlow = WorkFlow.走字;
            return true;
        }

        /// <summary>
        /// 表位隔离开关
        /// </summary>
        /// <param name="isOff"></param>
        /// <returns></returns>
        public bool SetMeterOnOff(bool[] isOff)
        {
            return false;
        }
        /// <summary>
        /// 读取误差板
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError[] ReadWcb(bool[] IsOnOff)
        {
            isStop = false;
            List<stError> lstErrors = new List<stError>();
            OpenSignleChannle(GenyDoorType.Door_Error);
            int channelCount = IsOnOff.Length / 6;

            for (int k = 0; k < channelCount; k++)
            {
                stError[] blockError = ReadWcb(k * 6 + 1);
                if (blockError == null)
                {
                    int i = 0;
                }
                if (blockError.Length == 6)
                {
                    lstErrors.AddRange(blockError);
                }
            }
            return lstErrors.ToArray();
        }

        /// <summary>
        /// 读取误差板
        /// </summary>
        /// <param name="intBwh"></param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError ReadWcb(int intBwh, int errTimes)
        {
            isStop = false;
            Geny_RequestReadWCBDataPacket wcbPacket = new Geny_RequestReadWCBDataPacket((byte)(intBwh));
            Geny_ReplyReadWCBDataPacket wcbRecvPacket = new Geny_ReplyReadWCBDataPacket(this.currentWorkFlow);
            if (isStop) return new stError();
            if (SendData(Com_Power_Port, wcbPacket, wcbRecvPacket) == false)
            {
                this.OnMessage("读取第" + (intBwh) + "表位失败");
            }
            else
            {
                return wcbRecvPacket.ToSTError();
            }
            return new stError();
        }
        /// <summary>
        /// 读取一组误差数据
        /// </summary>
        /// <param name="channelID"></param>
        /// <returns></returns>
        private stError[] ReadWcb(int channelID)
        {
            Geny_RequestReadPacket sendPacket = new Geny_RequestReadPacket((byte)channelID, 0x30);
            Geny_RequestReadWcbBlockReplyPacket recvPacket = new CLDC_DeviceDriver.Drivers.Geny.Packets.In.Geny_RequestReadWcbBlockReplyPacket(this.currentWorkFlow);
            if (!SendPacketToWcb(sendPacket, recvPacket))
            {
                return new stError[0];
            }
            if (recvPacket.Errors == null)
            {
                return new stError[6];
            }
            return recvPacket.Errors;
        }
        /// <summary>
        /// 读取 标准表信息
        /// 在读取标准表信息时，如果通道被485占用，
        /// 则调用线程将被挂起，等待最长时间为 3 分钟。
        /// 如果不挂起，返回为为零的值的，可以预计，
        /// 一个台体48表位，假定是读取电量块读，每块表使用200MS则共使用9600，
        /// 这里候，报表器模块，将读取四次电量，全部为0，将认为断流，或失压。从而报警
        /// </summary>
        /// <returns></returns>
        public stStdInfo ReadStdInfo()
        {
            if (isRs485Working)
            {
                CLDC_Comm.GlobalUnit.Logger.Info("RS485工作中,读取电源信息开始等待");
                this.rs485CompletedEvent.WaitOne(-1, false);
                CLDC_Comm.GlobalUnit.Logger.Info("RS485工作中,读取电源信息等待结束");
            }
            Geny_RequestStdMeterReadK6DDataPacket readpacket = new Geny_RequestStdMeterReadK6DDataPacket("", Geny_StandMeterDataType.Data);
            Geny_ReplyStdMeterPowerInfo recvPacket = new Geny_ReplyStdMeterPowerInfo();
            this.SendPacketToStdMeter(readpacket, recvPacket);
            stStdInfo powerInfo = recvPacket.StdInfo;
            return recvPacket.StdInfo;
            //}
        }

        /// <summary>
        /// 读取温度与温度
        /// 格林台体并没有实现，将两个值都设置成 0
        /// </summary>
        /// <param name="sng_temp"></param>
        /// <param name="sng_huim"></param>
        /// <returns></returns>
        public bool ReadTempHuim(ref float sng_temp, ref float sng_huim)
        {
            sng_huim = 0;
            sng_temp = 0;
            return true;
        }

        /// <summary>
        /// 调取台体GPS 时间
        /// </summary>
        /// <returns></returns>
        public DateTime ReadGPSTime()
        {
            Geny_RequestReadGPSTimepacket gpsPacket = new Geny_RequestReadGPSTimepacket(isUseTimeBase);
            Geny_ReplyReadGPSTimepacket gpsRecv = new Geny_ReplyReadGPSTimepacket(isUseTimeBase);
            bool isSc = false;
            string sockName = isUseTimeBase ? GENY_TIMEBASE_SOCKNAME : GENY_GPS_SOCKNAME;

            if (isUseTimeBase == false)
            {
                if (this.OpenSignleChannle(GenyDoorType.Door_GPS) == false)
                {
                    return DateTime.Now;
                }
            }

            isSc = false;
            for (int i = 0; i < RetryConunt; i++)
            {
                if (SockPool.Instance.Send(sockName, gpsPacket, gpsRecv) == true && gpsRecv.ReciveResult == RecvResult.OK)
                {
                    isSc = true;
                    break;
                }
            }
            if (isSc)
                return gpsRecv.GPSTime;
            else
                return DateTime.Now;
        }

        /// <summary>
        /// 该方法，已弃用
        /// 调用将导致异常
        /// </summary>
        /// <param name="arBtl"></param>
        /// <returns></returns>
        public bool InitRs485(string[] arBtl)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置电压跌落命令
        /// </summary>
        /// <param name="VolType"></param>
        /// <returns></returns>
        public bool SetVotFalloff(byte VolType)
        {
            return true;
        }
        /// <summary>
        /// 密钥下装初始化
        /// </summary>
        /// <param name="isOnOff"></param>
        /// <param name="im"></param>
        /// <returns></returns>
        public bool InitEncryption(bool[] isOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im)
        {
            
            return true;
        }
        /// <summary>
        /// 初始化COMM开关,用于打开
        /// </summary>
        /// <param name="IsOnOff">是否打开</param>
        /// <returns></returns>
        public bool InitCommTest(bool[] IsOnOff)
        {
            bool onlyOnePortNeedControl = true;
            bool isAllTheSame = ArrayHelper.IsAllValueMatch(IsOnOff);
            isRs485Working = true;//锁定485，不允许其它通道操作
            if (isAllTheSame)
            {
                Geny_RequestRS485SwitchPacket sendPacket = new Geny_RequestRS485SwitchPacket(199, (byte)(IsOnOff[0] ? 1 : 0));
                GenyRecvPacket recvPacket = new GenyRecvPacket();
                if (!SendPacketToWcb(sendPacket, recvPacket))
                {
                    Console.WriteLine("{0}所有485通道失败", IsOnOff[0] ? "打开" : "关闭");
                    CLDC_Comm.GlobalUnit.Logger.Debug(string.Format("{0}所有485通道失败", IsOnOff[0] ? "打开" : "关闭"));
                }
                if (IsOnOff[0] == false)
                {
                    isRs485Working = false;//如果接收到全部关闭的指令，则把RS485标志拉低
                    this.rs485CompletedEvent.Set();
                }
                CLDC_Comm.GlobalUnit.Logger.Debug(string.Format("{0}所有485通道", IsOnOff[0] ? "打开" : "关闭"));
                return true;
            }
            else
            {
                //锁住整个类，防止其它线程访问
                lock (this)
                {
                    this.rs485CompletedEvent.Reset();
                }
                int firstPos = Array.IndexOf(IsOnOff, true);
                int lastPos = Array.LastIndexOf(IsOnOff, true);
                onlyOnePortNeedControl = (firstPos == lastPos);

                if (onlyOnePortNeedControl == false)
                {
                    int oneChannelLoad = IsOnOff.Length / RS485ChannelCount;
                    int channelID = firstPos / oneChannelLoad;
                    int startPos = channelID * oneChannelLoad;
                    int endPos = startPos + oneChannelLoad;

                    for (int num = startPos; num < endPos; num++)
                    {
                        byte isOpen = (byte)(IsOnOff[num] ? 1 : 0);
                        //打开误差板485通道
                        Geny_RequestRS485SwitchPacket sendPacket = new Geny_RequestRS485SwitchPacket((byte)(FirstMeterNo + num), isOpen);
                        GenyRecvPacket recvPacket = new GenyRecvPacket();
                        CLDC_Comm.GlobalUnit.Logger.Debug(string.Format("{0}第{1}路485通道", IsOnOff[num] ? "打开" : "关闭", num));
                        if (!SendPacketToWcb(sendPacket, recvPacket))
                        {
                            Console.WriteLine("{0}第{1}路485通道失败", IsOnOff[num] ? "打开" : "关闭", num);
                            return false;
                        }
                    }
                }
                else
                {
                    Geny_RequestRS485SwitchPacket sendPacket = new Geny_RequestRS485SwitchPacket((byte)199, 0);
                    GenyRecvPacket recvPacket = new GenyRecvPacket();
                    //if (!SendPacketToWcb(sendPacket, recvPacket))
                    //{
                    //    Comm.GlobalUnit.Logger.Debug("关闭ALL485通道失败");
                    //    return false;
                    //}
                    //else
                    //{
                    //    Comm.GlobalUnit.Logger.Debug("关闭ALL485通道成功");
                    //}
                    sendPacket.SendID = (byte)(firstPos + FirstMeterNo);
                    sendPacket.State = 1;
                    recvPacket = new GenyRecvPacket();
                    if (!SendPacketToWcb(sendPacket, recvPacket))
                    {
                        Console.WriteLine("打开第{0}路485通道失败", firstPos + FirstMeterNo);
                        return false;
                    }
                    else
                    {
                        CLDC_Comm.GlobalUnit.Logger.Debug(string.Format("打开第{0}路485通道成功", firstPos + FirstMeterNo));
                    }
                }
            }
            if (isRs485Working)  //需要对485进行操作时对打开485通道
            {
                //切换台子通道到485
                if (!OpenSignleChannle(GenyDoorType.RS485))
                {
                    Console.WriteLine("切换台体通道到RS485失败");
                    return false;
                }
                else
                {
                    CLDC_Comm.GlobalUnit.Logger.Debug("切换台体通道到RS485成功");
                }
            }
            return true;

        }
        #endregion

        /// <summary>
        /// 框架共阴共阳类型转换为格宁共阴共阳类型
        /// </summary>
        /// <param name="sourceType">框架共阴共阳类型</param>
        /// <returns>转换后的格宁共阴共阳类型</returns>
        private Cus_GyGyType ComPulseType2GenyPulseType(Cus_GyGyType sourceType)
        {
            byte bytSosurceType = (byte)sourceType;
            bytSosurceType += ImpluseTypeStartValue;
            return (Cus_GyGyType)bytSosurceType;
        }

        /// <summary>
        /// 触发消息，
        /// </summary>
        /// <param name="message"></param>
        protected void OnMessage(string message)
        {
            if (this.ShowLog && this.msgCallBack != null)
            {
                this.msgCallBack(message);
            }
        }

        /// <summary>
        /// 向控制台输出日志
        /// 自动添加 换行符
        /// </summary>
        /// <param name="message"></param>
        protected void WriteStringToConsole(string message)
        {
            if (ShowLog)
            {
                Console.WriteLine(DateTime.Now + ": " + message);
            }
        }

        /// <summary>
        /// 打开 通道
        /// </summary>
        /// <param name="doorType">通道类型</param>
        /// <returns></returns>
        protected bool OpenSignleChannle(GenyDoorType doorType)
        {
            if (lastDoorType == doorType) return true;
            lastDoorType = doorType;
            bool isOpenDoorOk = false;
            Geny_RequestSelectDoorPacket sendPacket = new Geny_RequestSelectDoorPacket(doorType);
            GenyRecvPacket recvPackt = new GenyRecvPacket();
            string portName = base.GetPortNameByPortNumber(Com_Power_Port);
            SockPool.Instance.UpdatePortSetting(portName, Com_Power_Setting);
            for (int i = 0; i < RetryConunt; i++)
            {
                if (this.SendData(this.Com_Power_Port, sendPacket, recvPackt) == false)
                {
                    //this.OnMessage("选择格林信号通道出错:" + doorType);
                }
                else
                {
                    isOpenDoorOk = true;
                    //this.OnMessage("选择格林信号通道成功:" + doorType);
                    break;
                }
                if (isStop) break;
            }
            if (isOpenDoorOk == false)
                this.OnMessage("选择格林信号通道出错:" + doorType);
            return isOpenDoorOk;
        }

        /// <summary>
        /// 清除所有表位的误差板标志
        /// </summary>
        /// <returns></returns>
        private bool ClearErrorBoardMark(int index)
        {
            //清除标志位
            byte meterIndex = (byte)index;
            if (index != 0xc7) meterIndex += FirstMeterNo;
            Geny_RequestClearMarkPacket clearPacket = new Geny_RequestClearMarkPacket((byte)(FirstMeterNo + index));
            GenyRecvPacket recvClearPacket = new GenyRecvPacket();
            if (this.SendPacketToWcb(clearPacket, recvClearPacket) == false)
            {
                this.OnMessage(string.Format("清除第{0}表位失败", (FirstMeterNo + index)));
                return false;
            }
            this.OnMessage(string.Format("清除第{0}表位成功", (FirstMeterNo + index)));
            return true;
        }

        /// <summary>
        /// 上一次打开通道的ID
        /// </summary>
        private GenyDoorType lastDoorType;

        /// <summary>
        /// 向指定通道发送数据，
        /// 自动切换通道
        /// </summary>
        /// <param name="port">COM 端口</param>
        /// <param name="doorType">通道类型</param>
        /// <param name="sendPacket">要发送的数据包</param>
        /// <param name="recvPacket">用来解析回收的数据</param>
        /// <returns></returns>
        private bool SendData(int port, GenyDoorType doorType, SendPacket sendPacket, RecvPacket recvPacket)
        {
            //锁定当前操作，由于格宁台体只有一个数据端口，基本可以不考虑多线程
            lock (this)
            {
                if (!OpenSignleChannle(doorType))
                    return false;
                string portName = GetPortNameByPortNumber(port);
                //update portsetting
                SockPool.Instance.UpdatePortSetting(portName, Com_Power_Setting);
                //send
                return base.SendData(port, sendPacket, recvPacket);
            }
        }

        /// <summary>
        /// 向指定通道发送数据，
        /// 自动切换通道
        /// </summary>
        /// <param name="port">COM 端口</param>
        /// <param name="doorType">通道类型</param>
        /// <param name="sendPacket">要发送的数据包</param>
        /// <param name="recvPacket">用来解析回收的数据</param>
        /// <param name="times">重试次数</param>
        /// <returns></returns>
        private bool SendPacketWithTimes(int Port, GenyDoorType doorType, SendPacket sendPacket, RecvPacket recvPacket, int times)
        {
            bool isSC = false;

            for (int iTimes = 0; iTimes < times; iTimes++)
            {
                if (isStop == true)
                    return true;
                WriteStringToConsole("第" + iTimes + "次发送数据:");
                if (this.SendData(Port, doorType, sendPacket, recvPacket) == true)
                {
                    if (!sendPacket.IsNeedReturn || recvPacket.ReciveResult == RecvResult.OK)
                    {
                        isSC = true;
                        break;
                    }
                }
                if (isStop) return true;
                Thread.Sleep(RetryTime);
            }
            return isSC;
        }

        /// <summary>
        /// 向 控源口，发送数据包
        /// 重试 RetryCount
        /// </summary>
        /// <param name="sendPacket">要发送的包</param>
        /// <param name="recvPacket">用来解析收到数据的包</param>
        /// <returns></returns>
        private bool SendPacketToPower(SendPacket sendPacket, RecvPacket recvPacket)
        {
            return this.SendPacketWithTimes(Com_Power_Port, GenyDoorType.Door_Power, sendPacket, recvPacket, RetryConunt);
        }

        /// <summary>
        /// 发送数据到标准表
        /// </summary>
        /// <param name="sendPacket">要发送的包</param>
        /// <param name="recvPacket">用来解析收到数据的包</param>
        /// <returns></returns>
        private bool SendPacketToStdMeter(SendPacket sendPacket, RecvPacket recvPacket)
        {
            return this.SendPacketWithTimes(Com_Power_Port, GenyDoorType.StdMeter, sendPacket, recvPacket, RetryConunt);
        }

        /// <summary>
        /// 发送数据到误差板
        /// </summary>
        /// <param name="sendPacket">要发送的包</param>
        /// <param name="recvPacket">用来解析收到数据的包</param>
        /// <returns></returns>
        private bool SendPacketToWcb(SendPacket sendPacket, RecvPacket recvPacket)
        {
            return this.SendPacketWithTimes(Com_Power_Port, GenyDoorType.Door_Error, sendPacket, recvPacket, RetryConunt);
        }

        protected override void BeginSendData(stEquipInfo tagPortInfo)
        {
        }

        /// <summary>
        /// 将逻辑上通道，转换成物理上的通道
        /// </summary>
        /// <param name="impluseChannel"></param>
        /// <returns></returns>
        public byte GetImpluseChannel(GenyImplusechannels impluseChannel)
        {
            byte ret = byte.Parse(this.Setting[impluseChannel.ToString()]);
            return ret;
        }

        /// <summary>
        /// 将逻辑上通道，转换成物理上的通道
        /// </summary>
        /// <param name="fanXiang"></param>
        /// <returns></returns>
        public byte GetImpluseChannel(CLDC_Comm.Enum.Cus_PowerFangXiang fanXiang)
        {
            GenyImplusechannels imc = GenyImplusechannels.正向有功;
            switch (fanXiang)
            {
                case Cus_PowerFangXiang.正向有功: imc = (GenyImplusechannels.正向有功); break;
                case Cus_PowerFangXiang.正向无功: imc = GenyImplusechannels.正向无功; break;
                case Cus_PowerFangXiang.反向有功: imc = GenyImplusechannels.反向有功; break;
                case Cus_PowerFangXiang.反向无功: imc = GenyImplusechannels.反向无功; break;
                default: break;
            }
            return GetImpluseChannel(imc);
        }
    }
}
