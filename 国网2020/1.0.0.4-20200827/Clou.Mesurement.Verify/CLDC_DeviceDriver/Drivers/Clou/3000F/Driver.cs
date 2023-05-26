using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F
{
    class Driver:DriverBase,IDriver 
    {
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 3;

        /// <summary>
        /// 重试间隔等待时间 ms
        /// </summary>
        public static int RETRYTTIME = 500;

        #region ---------设备端口---------
        /// <summary>
        /// 2036服务器IP
        /// </summary>
        private string Cl2036ServerIp = "193.168.18.1";

        /// <summary>
        /// 2036通道
        /// </summary>
        private int Cl2036ChannelID = 31;
        /// <summary>
        /// 标准表端口
        /// </summary>
        //private int Cl2036ChannelID = 0;
        /// <summary>
        /// 标准表通讯是否为串口
        /// </summary>
       // private bool m_MeterPort_IsCom = false;
        /// <summary>
        /// 误差板端口
        /// </summary>
        private int[] m_arrErrorPort = new int[0];
        /// <summary>
        /// 源控制端口
        /// </summary>
        //private int m_PowerPort = 0;
        #endregion


        public Driver(int bws)
            :base(bws)
        {
            //加载配置
            LoadSetting();

            //注册端口
            //string strIP = Setting.GetValue("ServerIP");
            //string strsetting = Setting.GetValue("MeterSetting");
            //if (m_MeterPort_IsCom)
            //    RegisterPort(Cl2036ChannelID, strsetting, 1000, 100);       //标准表
            //else
            //    RegisterPort(Cl2036ChannelID, strsetting, strIP, 1000, 100);
            RegisterPort(Cl2036ChannelID, "19200,n,8,1",Cl2036ServerIp,1000, 100);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadSetting()
        {
            Cl2036ServerIp = Setting.GetValue("ServerIP");
            int.TryParse(Setting.GetValue("ServerPort"), out Cl2036ChannelID);
        }


        /// <summary>
        /// 初始化设置
        /// </summary>
        protected override void InitSetting()
        {
            base.Setting = new CLDC_DeviceDriver.Drivers.Clou._3000F.Setting.Setting();
        }

        /// <summary>
        /// 开始发送数据前的准备工作
        /// </summary>
        /// <param name="tagPortInfo">等待发送的数据端口</param>
        protected override void BeginSendData(stEquipInfo tagPortInfo)
        {
            if (!tagPortInfo.IsCom && !tagPortInfo.IsInitialized())
            {
            }
        }



        #region IDriver 成员

        public MsgCallBack CallBack
        {
            get
            {
                return msgCallBack;
            }
            set
            {
                msgCallBack=value;
            }
        }

        private bool isShowLog=false;

        /// <summary>
        /// 显示日志
        /// </summary>
        public bool ShowLog
        {
            get
            {
                return isShowLog;
            }
            set
            {
                isShowLog =true;
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            Packets.Out.RequestCtrlVerifyPacket sendPacket = new Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvPacket = new Packets.In.ReplyOkPacket();

            sendPacket.IsStop = true;

            bool bet = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (bet)
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 联机
        /// </summary>
        /// <returns></returns>
        public bool Link()
        {
            Packets.Out.RequestLinkPacket sendPacket=new Packets.Out.RequestLinkPacket();
            ClouRecvPacket_CLT11 recvPacket = new Packets.In.ReplyOkPacket();

            sendPacket.IsLink = true;
            bool ret = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (ret)
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 脱机
        /// </summary>
        /// <returns></returns>
        public bool UnLink()
        {
            Packets.Out.RequestLinkPacket sendPacket = new Packets.Out.RequestLinkPacket();
            Packets.In.ReplyOkPacket recvPacket= new Packets.In.ReplyOkPacket();

            sendPacket.IsLink = false;
            bool ret = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (ret)
            {
                if (recvPacket.ReciveResult==CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                    return true;
            }
            return false;
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
        /// 设置谐波参数
        /// </summary>
        /// <param name="int_XSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相各次开关,数组元素值：0=不加谐波，1=加谐波,数组各元素：各相（0-5），各次（0-64）</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <returns>设置谐波参数是否成功</returns>
        public bool SetHarmonic(int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
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
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="isOn">true为停止，false为启动</param>
        /// <returns></returns>
        public bool SetCurFunctionOnOrOff(bool isOn)
        {
            return true;
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
        /// 升源
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="sng_Ub">额定电压</param>
        /// <param name="sng_Ib">额定电流</param>
        /// <param name="sng_IMax">最大电流</param>
        /// <param name="sng_xUb_A">输出额定电压倍数A</param>
        /// <param name="sng_xUb_B">输出额定电压倍数B</param>
        /// <param name="sng_xUb_C">输出额定电压倍数C</param>
        /// <param name="sng_xIb_A">输出额定电流倍数A</param>
        /// <param name="sng_xIb_B">输出额定电流倍数B</param>
        /// <param name="sng_xIb_C">输出额定电流倍数C</param>
        /// <param name="element">元件</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_IsNxx">是否逆向序</param>
        /// <returns></returns>
        public bool PowerOn(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, string strGlys, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, CLDC_Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            //Check status
            Packets.Out.RequestReadStatusPacket sendStatusPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestReadStatusPacket();
            Packets.In.RequestReadStatusReplyPacket recvStatusPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.RequestReadStatusReplyPacket();

            
            while (true)
            {
                if (SendData(Cl2036ChannelID, sendStatusPacket, recvStatusPacket))
                {
                    Packets.Out.RequestSetWorkingPointPacket sendWP=new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetWorkingPointPacket();
                    Packets.Out.RequestAcSourceUpdatePacket sendAcPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestAcSourceUpdatePacket();
                    Packets.In.ReplyOkPacket recvAcPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

                    sendWP.LineType = (byte)clfs;
                    sendWP.Un = sng_Ub;
                    sendWP.Ua = sng_xUb_A / sng_Ub;
                    sendWP.Ub = sng_xUb_B / sng_Ub;
                    sendWP.Uc = sng_xUb_C / sng_Ub;
                    sendWP.Inb = sng_Ib;
                    sendWP.Ia = sng_xIb_A / sng_Ib;
                    sendWP.Ib = sng_xIb_B / sng_Ib;
                    sendWP.Ic = sng_xIb_C / sng_Ib;
                    sendWP.Inmax = sng_IMax;
                    sendWP.PowerFactor = strGlys;
                    sendWP.Freq = sng_Freq;
                    sendWP.Element = (byte)element;
                    sendAcPacket.IsPowerOn = true;

                    bool ret = SendData(Cl2036ChannelID, sendAcPacket, recvAcPacket);
                    if (ret)
                    {
                        if (recvAcPacket.ReciveResult==CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        public bool PowerOff()
        {
            Packets.Out.RequestAcSourceUpdatePacket sendAcPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestAcSourceUpdatePacket();
            Packets.In.ReplyOkPacket recvAcPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            sendAcPacket.IsPowerOn=false;
            sendAcPacket.Ua = 0;
            sendAcPacket.Ub = 0;
            sendAcPacket.Uc = 0;
            sendAcPacket.Ia = 0;
            sendAcPacket.Ib = 0;
            sendAcPacket.Ic = 0;

            bool ret = SendData(Cl2036ChannelID, sendAcPacket, recvAcPacket);
            if (ret)
            {
                if (recvAcPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 初始化预热
        /// </summary>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位</param>
        /// <returns></returns>
        public bool InitWarmUp(CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            return true;
        }

        /// <summary>
        /// 对色标
        /// </summary>
        /// <param name="glfx">功率方向</param>
        /// <param name="sng_U">对标电压</param>
        /// <param name="sng_I">对标电流</param>
        /// <param name="PulseCount">对标脉冲个数</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <returns>初始化对色标</returns>
        public bool InitDuiSeBiao(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, float sng_U, float sng_I, int PulseCount, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte=new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length-1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
                SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 3;
            SetVer.Pluse191 = 0xff;
            SetVer.ErrNum = PulseCount;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化启动项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位启动时间【单位秒】</param>
        /// <returns>初始化是否成功</returns>
        public bool InitStartUp(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes)
        {
            Packets.Out.RequestSetMeterSwitchPacket sendSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            if (SendData(Cl2036ChannelID, sendSwitchPacket, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket sendVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();
            
            if (SendData(Cl2036ChannelID, sendVerifyPacket, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket sendCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            sendCtrlPacket.IsStop = false;
            if (SendData(Cl2036ChannelID, sendCtrlPacket, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 初始化潜动项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位启动时间【单位秒】</param>
        /// <returns></returns>
        public bool InitCreeping(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes)
        {
            Packets.Out.RequestSetMeterSwitchPacket sendSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            if (SendData(Cl2036ChannelID, sendSwitchPacket, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket sendVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            if (SendData(Cl2036ChannelID, sendVerifyPacket, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket sendCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            sendCtrlPacket.IsStop = false;
            if (SendData(Cl2036ChannelID, sendCtrlPacket, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 初始化基本误差项目、不包含升源操作
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="bcs">表常数</param>
        /// <param name="quans">检定圈数</param>
        /// <param name="wccs">计算误差脉冲个数</param>
        /// <param name="im">脉冲通道</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <returns>初始化基本误差是否成功</returns>
        public bool InitError(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, float[] bcs, int[] quans, int wccs, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 0;
            SetVer.Pluse191 = 0xff;
            SetVer.ErrNum = wccs;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
            //设置表位开关
            if (!SetMeterOnOff(isOnOff))
            {
                return false;
            }

            //联机
            if (!Link())
            {
                return false;
            }

            //停止当前设置的功能
            if (!SetCurFunctionOnOrOff(true))
            {
                return false;
            }

            //交流源更新(关源)
            if (!PowerOff())
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 初始化通讯测试、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <returns></returns>
        public bool InitCommTest(bool[] IsOnOff)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetMeterComPortPacket SetMeterCom = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterComPortPacket();
            Packets.In.ReplyOkPacket recvPortPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetMeterCom.FunRoute = 0xff;
            SetMeterCom.IsOpen = true;
            if (SendData(Cl2036ChannelID, SetMeterCom, recvPortPacket))
            {
                if (recvPortPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 初始化日计时误差、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="MeterFre">各表位时钟周期</param>
        /// <returns></returns>
        public bool InitTimeAccuracy(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, float[] MeterFre, float[] bcs, int[] quans)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 2;
            SetVer.Pluse191 = 0x00;
            SetVer.ErrNum = 10;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化时段投切、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="pulseCount">接收脉冲</param>
        /// <returns></returns>
        public bool InitTimePeriod(bool[] IsOnOff, int[] pulseCount)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;

            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 3;
            SetVer.Pluse191 = 0xff;
            SetVer.ErrNum = 1;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetMeterComPortPacket SetMeterCom = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterComPortPacket();
            Packets.In.ReplyOkPacket recvPortPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetMeterCom.FunRoute = 0xff;
            SetMeterCom.IsOpen = true;
            if (SendData(Cl2036ChannelID, SetMeterCom, recvPortPacket))
            {
                if (recvPortPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 初始化最大需量
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="xlzqSeconds"></param>
        /// <param name="hccs"></param>
        /// <returns></returns>
        public bool InitMaxDemand(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            Packets.Out.RequestSetMeterComPortPacket SetMeterCom = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterComPortPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetMeterCom.FunRoute = 0xff;
            SetMeterCom.IsOpen = true;
            if (SendData(Cl2036ChannelID, SetMeterCom, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 初始化需量周期误差
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="im"></param>
        /// <param name="xlzqSeconds">需量周期</param>
        /// <param name="hccs">滑差次数</param>
        /// <returns></returns>
        public bool InitDemandPeriod(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] xlzqSeconds, int[] hccs)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetSameXLPulsePacket SetSameXLPulse = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetSameXLPulsePacket();
            Packets.In.ReplyOkPacket recvXLPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetSameXLPulse.Pulse_num = 0x01;
            SetSameXLPulse.Pulse_Time = (byte)(xlzqSeconds[0]/hccs[0]);

            if (SendData(Cl2036ChannelID, SetSameXLPulse, recvXLPacket))
            {
                if (recvXLPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 1;
            SetVer.Pluse191 = 0xff;
            SetVer.ErrNum = 1;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
            return true;
        }

        /// <summary>
        /// 初始化走字项目，不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="glfx">功率方向</param>
        /// <returns></returns>        
        public bool InitZZ(bool[] IsOnOff, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] impluseCount)
        {
            Packets.Out.RequestSetMeterSwitchPacket SetMeterSwitch = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterSwitchPacket();
            Packets.In.ReplyOkPacket recvSwitchPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            byte[] TmpByte = new byte[IsOnOff.Length];
            for (int n = 0; n <= IsOnOff.Length - 1; n++)
            {
                if (IsOnOff[n])
                {
                    TmpByte[n] = 1;
                }
                else
                {
                    TmpByte[n] = 0;
                }
            }
            SetMeterSwitch.MeterSwitch = TmpByte;
            if (SendData(Cl2036ChannelID, SetMeterSwitch, recvSwitchPacket))
            {
                if (recvSwitchPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetMeterComPortPacket SetMeterCom = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetMeterComPortPacket();
            Packets.In.ReplyOkPacket recvPortPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetMeterCom.FunRoute = 0xff;
            SetMeterCom.IsOpen = true;
            if (SendData(Cl2036ChannelID, SetMeterCom, recvPortPacket))
            {
                if (recvPortPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestSetVerifyPacket SetVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestSetVerifyPacket();
            Packets.In.ReplyOkPacket recvVerifyPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            SetVer.TstType = 5;
            SetVer.Pluse191 = 0xff;
            SetVer.ErrNum = 1;
            SetVer.DivideFreq = 0;
            SetVer.EdayFreq = 500000;
            SetVer.Filter = 0;
            SetVer.PluseTime = 0;
            if (SendData(Cl2036ChannelID, SetVer, recvVerifyPacket))
            {
                if (recvVerifyPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            Packets.Out.RequestCtrlVerifyPacket CtrlVer = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestCtrlVerifyPacket();
            Packets.In.ReplyOkPacket recvCtrlPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.ReplyOkPacket();

            CtrlVer.IsStop = false;
            if (SendData(Cl2036ChannelID, CtrlVer, recvCtrlPacket))
            {
                if (recvCtrlPacket.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取所有误差板的数据，表位=返回数组下标+1
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError[] ReadWcb(bool[] IsOnOff)
        {
            stError[] errData=new stError[g_Bws];
            int oneline = getOneLineLoads();

            for (int i = 0; i < oneline; i++)
            {
                for (int j = 0; j < m_arrErrorPort.Length; j++)
                {
                    int pos = (byte)((j * oneline + i) + 1);
                    if (IsOnOff[pos - 1])
                    {
                        errData[pos - 1] = ReadWcb(pos, 0);
                    }
                    else
                    {
                        errData[pos - 1] = new stError();
                        errData[pos - 1].szError = "";
                        errData[pos - 1].MeterIndex = i + 1;
                    }
                    return errData;
                }
            }
            return errData;
        }

        /// <summary>
        /// 读取所有误差板的数据
        /// </summary>
        /// <param name="intBwh"></param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError ReadWcb(int intBwh, int errTimes)
        {
            Packets.Out.RequestReadVerifyDataPacket sendPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out.RequestReadVerifyDataPacket();
            Packets.In.RequestReadVerifyDataReplyPacket recvPacket = new CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In.RequestReadVerifyDataReplyPacket();

            sendPacket.StartPos = intBwh;
            sendPacket.ReadCount = (byte)errTimes;
            int line = (intBwh - 1) / getOneLineLoads();

            stError tagError = new stError();
            tagError.szError = "";
            
            if (SendData(m_arrErrorPort[line], sendPacket, recvPacket))
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    tagError.szError = recvPacket.BasicError.ToString();
                    tagError.Index = errTimes;
                    tagError.MeterIndex = intBwh;
                }
            }
            return tagError;
        }

        /// <summary>
        /// 读取标准表信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo ReadStdInfo()
        {
            Packets.Out.RequestReadStdInfoPacket sendPacket = new Packets.Out.RequestReadStdInfoPacket();
            Packets.In.RequestReadStdInfoReplyPacket recvPacket = new Packets.In.RequestReadStdInfoReplyPacket();

            stStdInfo stdInfoTemp = new stStdInfo();
            bool ret = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (ret)
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    stdInfoTemp.Ia=recvPacket.Ia;
                    stdInfoTemp.Ib=recvPacket.Ib;
                    stdInfoTemp.Ic=recvPacket.Ic;

                    stdInfoTemp.Phi_Ia=recvPacket.PhiA;
                    stdInfoTemp.Phi_Ib=recvPacket.PhiB;
                    stdInfoTemp.Phi_Ic=recvPacket.PhiC;

                    stdInfoTemp.Phi_Ua=recvPacket.Ua;
                    stdInfoTemp.Phi_Ub=recvPacket.Ub;
                    stdInfoTemp.Phi_Uc=recvPacket.Uc;

                    //有功功率
                    stdInfoTemp.P=recvPacket.PTotal;
                    stdInfoTemp.Pa=recvPacket.PATotal;
                    stdInfoTemp.Pb=recvPacket.PBTotal;
                    stdInfoTemp.Pc=recvPacket.PCTotal;

                    //无功功率
                    stdInfoTemp.Q=recvPacket.QTotal;
                    stdInfoTemp.Qa=recvPacket.QATotal;
                    stdInfoTemp.Qb=recvPacket.QBTotal;
                    stdInfoTemp.Qc=recvPacket.QCTotal;

                    //视在功率
                    stdInfoTemp.S=recvPacket.STotal;
                    stdInfoTemp.Sa=recvPacket.SATotal;
                    stdInfoTemp.Sb=recvPacket.SBTotal;
                    stdInfoTemp.Sc=recvPacket.SCTotal;

                    stdInfoTemp.Ua=recvPacket.Ua;
                    stdInfoTemp.Ub=recvPacket.Ub;
                    stdInfoTemp.Uc=recvPacket.Uc;

                    return stdInfoTemp;
                }
            }
            return stdInfoTemp;
        }

        /// <summary>
        /// 读取温度湿度
        /// </summary>
        /// <param name="sng_temp"></param>
        /// <param name="sng_huim"></param>
        /// <returns></returns>
        public bool ReadTempHuim(ref float sng_temp, ref float sng_huim)
        {
            Packets.Out.RequestReadTmpAndHuiPacket sendPacket= new Packets.Out.RequestReadTmpAndHuiPacket();
            Packets.In.RequestReadTmpAndHuiReplyPacket recvPacket= new Packets.In.RequestReadTmpAndHuiReplyPacket();

            bool ret = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (ret)
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    sng_temp = recvPacket.ITemperature;
                    sng_huim = recvPacket.IHumidity;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取标准时间
        /// </summary>
        /// <returns></returns>
        public DateTime ReadGPSTime()
        {
            Packets.Out.RequestReadGpsTimePacket sendPacket = new Packets.Out.RequestReadGpsTimePacket();
            Packets.In.RequestReadGpsTimeReplyPacket recvPacket = new Packets.In.RequestReadGpsTimeReplyPacket();

            bool ret = SendData(Cl2036ChannelID, sendPacket, recvPacket);
            if (ret)
            {
                if (recvPacket.ReciveResult == CLDC_Comm.SocketModule.Packet.RecvResult.OK)
                {
                    DateTime dt = new DateTime(recvPacket.IYear, recvPacket.IMonth, recvPacket.IDay, recvPacket.IHour, recvPacket.IMinute, recvPacket.ISecond);
                    return dt;
                }
            }
            return DateTime.Now;
        }

        #endregion

        /// <summary>
        /// 获取误差每路负载
        /// </summary>
        /// <returns></returns>
        private int getOneLineLoads()
        {
            int nNum = g_Bws / m_arrErrorPort.Length;
            return nNum;
        }
    }
}
