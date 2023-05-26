using System;
using System.Collections.Generic;
using System.Text;
using DeviceDriver.Sock;

namespace DeviceDriver.Drivers.Clou.V90
{
    internal class Driver : DriverBase, DeviceDriver.Drivers.IDriver
    {

        public Driver(int bw)
            : base(bw)
        {
            RegisterPort(32, "19200,n,8,1");
        }

        #region ---------端口操作&数据包发送----------
        /// <summary>
        /// 注册端口
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="strSetting">端口参数</param>
        private void RegisterPort(int port, string strSetting)
        { 
            int datePort = localPortTo2036Port(port);
            int setingPort = datePort++;
            System.Net.IPAddress ipa = System.Net.IPAddress.Parse("193.168.18.1");
            //注册数据端口
            SockPool.AddUdpSock(getPortNameByPort(datePort), ipa, 10003, datePort, 5);
            //注册设置端口
            SockPool.AddUdpSock(getPortNameByPort(setingPort), ipa, 10003, setingPort, 5);
            //先发一包初始化端口
            initPortSetting(setingPort, strSetting);
        }
        /// <summary>
        /// 端口命名
        /// </summary>
        /// <param name="port">2018端口号</param>
        /// <returns></returns>
        private string getPortNameByPort(int port)
        {
            if (port < 20000) port = localPortTo2036Port(port);
            return "Port_" + port.ToString();
        }
        /// <summary>
        /// 本地通道转换成2036端口
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private int localPortTo2036Port(int port)
        {
            return 20000 + 2 * (port - 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        private void initPortSetting(int port, string setting)
        {
            Packets.Out.RequestInit2018PortPacket rc = new DeviceDriver.Drivers.Clou.V90.Packets.Out.RequestInit2018PortPacket(setting);
            DeviceDriver.PackBase.Packet pa = null;
            SendData(port, rc, ref pa);
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="port">数据端口号</param>
        /// <param name="sendPack">发送包</param>
        /// <param name="recvPack">接收包</param>
        /// <returns></returns>
        public bool SendData(int port, DeviceDriver.PackBase.Packet sendPack, ref DeviceDriver.PackBase.Packet recvPack)
        {
            string portName = getPortNameByPort(port);

            //Console.WriteLine("Send Packet:" + BitConverter.ToString(sendPack.l) ); 

            return SockPool.Send(portName, sendPack, ref recvPack);
        }
        #endregion

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

        #region ---------Drivers成员----------

        protected override void InitSetting()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void BeginSendData(stEquipInfo tagPortInfo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IDriver 成员
        private MsgCallBack m_CallBack = null;
        public MsgCallBack CallBack
        {
            get
            {
                return m_CallBack;
            }
            set
            {
                m_CallBack = value;
            }
        }
        private bool m_Stop;
        /// <summary>
        /// 停止当前操作
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            m_Stop = true;
            return true;
        }
        /// <summary>
        /// 联机请求
        /// </summary>
        /// <returns></returns>
        public bool Link()
        {
            Packets.Out.RequestLinkPacket rcLink = new DeviceDriver.Drivers.Clou.V90.Packets.Out.RequestLinkPacket();
            Packets.In.RequestResultReplayPacket rcLinkReplay = new Packets.In.RequestResultReplayPacket();
            DeviceDriver.PackBase.Packet rc = rcLinkReplay as DeviceDriver.PackBase.Packet;
            rcLink.IsLink = true;
            outMessage("开始请求联机");
            if (SendData(32, rcLink, ref rc))
            {
                outMessage("数据成功返回");
                //Packets.In.RequestResultReplayPacket rc = (Packets.In.RequestResultReplayPacket)rcLinkReplay;
                if (rc == null) return false;
                return rcLinkReplay.Result == DeviceDriver.Drivers.Clou.V90.Packets.In.RequestResultReplayPacket.ReplayCode.Ok;
            }
            outMessage("发送数据包失败,请检定相关配置");
            return false;
        }
        /// <summary>
        /// 脱机操作
        /// </summary>
        /// <returns></returns>
        public bool UnLink()
        {
            Packets.Out.RequestLinkPacket rcLink = new DeviceDriver.Drivers.Clou.V90.Packets.Out.RequestLinkPacket();
            Packets.In.RequestResultReplayPacket rcLinkReplay = new Packets.In.RequestResultReplayPacket();
            DeviceDriver.PackBase.Packet rc = rcLinkReplay as DeviceDriver.PackBase.Packet;
            rcLink.IsLink = false;
            outMessage("开始请求脱机");
            if (SendData(32, rcLink, ref rc))
            {
                outMessage("数据成功返回");
                //Packets.In.RequestResultReplayPacket rc = (Packets.In.RequestResultReplayPacket)rcLinkReplay;
                if (rc == null) return false;
                return rcLinkReplay.Result == DeviceDriver.Drivers.Clou.V90.Packets.In.RequestResultReplayPacket.ReplayCode.Ok;
            }
            outMessage("发送数据包失败,请检定相关配置");
            return false;
        }

        public bool SetHarmonic(int[] int_XSwitch, int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
            throw new Exception("The method or operation is not implemented.");
        }

       

        public bool PowerOff()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitWarmUp(Comm.Enum.Cus_PowerFangXiang glfx, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDuiSeBiao(Comm.Enum.Cus_PowerFangXiang glfx, float sng_U, float sng_I, int PulseCount, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitStartUp(Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCreeping(Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitError(Comm.Enum.Cus_PowerFangXiang glfx, float[] bcs, int[] quans, int wccs, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCommTest(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitTimeAccuracy(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitTimePeriod(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitMaxDemand(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDemandPeriod(bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitZZ(bool[] IsOnOff, Comm.Enum.Cus_PowerFangXiang glfx)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError[] ReadWcb()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError ReadWcb(int intBwh)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        //public stPowerInfo ReadPowerInfo()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        public stStdInfo ReadStdInfo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool ReadTempHuim(ref float sng_temp, ref float sng_huim)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DateTime ReadGPSTime()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitRs485(string[] arBtl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte[] Rs485(byte[] bySend, ref byte[] byRecv, int intBwh)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion



        #region IDriver 成员


        public bool SetHarmonic(int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDuiSeBiao(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_U, float sng_I, int PulseCount, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitStartUp(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCreeping(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitError(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float[] bcs, int[] quans, int wccs, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        public bool InitWarmUp(Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitZZ(bool[] IsOnOff, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        public bool PowerOn(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitStartUp(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff, int[] startTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitCreeping(Comm.Enum.Cus_Clfs clfs, Comm.Enum.Cus_PowerFangXiang glfx, ImpluseMode[] im, bool[] IsOnOff, int[] startTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitMaxDemand(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool InitDemandPeriod(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError[] ReadWcb(bool[] IsOnOff, int errTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public stError ReadWcb(int intBwh, int errTimes)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte[] Rs485(byte[] bySend, ref byte[] byRecv, int intBwh, bool isNeedReturn)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        public bool InitTimePeriod(bool[] IsOnOff, int xlzqSeconds, int hccs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDriver 成员


        private bool isShowLog = false;
        public bool ShowLog
        {
            get
            {
                return isShowLog;
            }
            set
            {
                isShowLog = true;
            }
        }

        #endregion
    }
}
