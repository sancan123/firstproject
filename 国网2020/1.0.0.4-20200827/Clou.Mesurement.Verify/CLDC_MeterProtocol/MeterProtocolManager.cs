using System;
using System.Collections.Generic;
using CLDC_MeterProtocol.Settings;
using CLDC_DataCore.SocketModule;
using CLDC_DataCore.SocketModule.Packet;
using CLDC_DataCore.Struct;
using CLDC_DeviceDriver.PortFactory;
using CLDC_DeviceDriver;
using CLDC_DataCore.Function;

namespace CLDC_MeterProtocol
{
    /// <summary>
    /// 多功能协议管理
    /// </summary>
    public class MeterProtocolManager : CLDC_Comm.BaseClass.SingletonBase<MeterProtocolManager>
    {
        private Dictionary<int, StPortInfo> ChannelPortInfo = new Dictionary<int, StPortInfo>();

        

        

        public MeterProtocolManager()
        {
        }

        /// <summary>
        /// 获取485通道数
        /// </summary>
        /// <returns></returns>
        public int GetChannelCount()
        {
            return DgnConfigManager.Instance.GetChannelCount();
        }

        /// <summary>
        /// 获取指定通道的的端口号
        /// </summary>
        /// <param name="channelId">通道号</param>
        /// <returns></returns>
        public string GetChannelPortName(int channelId)
        {
            if (ChannelPortInfo.ContainsKey(channelId))
            {
                StPortInfo portInfo = ChannelPortInfo[channelId];
                string strSetting = portInfo.m_Port_Setting;
                string strIp = portInfo.m_IP;//strSetting.Split('|')[0];
                return GetPortName(strIp,portInfo.m_Port);
            }
            return string.Empty;
        }
        /// <summary>
        /// 查询指定通道端口名称
        /// </summary>
        /// <param name="port">通道号</param>
        /// <returns>端口名称</returns>
        private string GetPortNameCOM(int port)
        {
            return string.Format("COM_{0}", port);
        }
        /// <summary>
        /// 查询指定通道端口名称
        /// </summary>
        /// <param name="Ip">IP</param>
        /// <param name="port">通道号</param>
        /// <returns>端口名称</returns>
        private string GetPortName(string Ip, int port)
        {
            string strName = "Port_" + Ip;
            return string.Format("{0}_{1}", strName, port);
        }

        /// <summary>
        /// 查询指定通道端口名称
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private string GetPortName(StPortInfo port)
        {
            string strName = "Port_" + port.m_IP;//.Substring(Ip.Length - 1, 1);
            return string.Format("{0}_{1}", strName, port.m_Port);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            DgnConfigManager.Instance.Load();
            DeviceParaUnit[] _Dpu= Rs485Setting.GetRs485Ports();
            for (int i = 0; i < DgnConfigManager.Instance.GetChannelCount(); i++)
            {
                StPortInfo port = DgnConfigManager.Instance.GetConfig(i);
                if (ChannelPortInfo.ContainsKey(i + 1))
                {
                    ChannelPortInfo.Remove(i + 1);
                }
                ChannelPortInfo.Add(i+1,port);
                
                RegisterPort(_Dpu[i]);
            }
        }

        private void RegisterPort(DeviceParaUnit port)
        {
            if (port == null) return;
            if (port.Address == "COM")
            {
                RegisterPort(port.PortNo, port.Baudrate);
            }
            else
            {
                RegisterPort(port.PortNo, port.Baudrate, port.Address, port.RemotePort.ToString(), port.StartPort,port.MaxTimePerFrame,port.MaxTimePerByte);
            }
        }


        /// <summary>
        /// 注册端口[UDP端口]
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="strSetting">端口参数</param>
        /// <param name="IP">串口服务器IP</param>
        /// <param name="strRemotPort">10003,10004</param>
        /// <param name="intBasePort">起始端口</param>
        private void RegisterPort(int port, string strSetting, string IP, string strRemotPort, int intBasePort,int MaxWaiteTime,int MaxWaitePerbyte)
        {
            System.Net.IPAddress ipa = System.Net.IPAddress.Parse(IP);
            string portNmae = GetPortName(IP, port);
            //int intBasePort = 20000;
            //if (IP != "193.168.18.1")
            //intBasePort = 30000;
            //注册数据端口
            SockPool.Instance.AddUdpSock(portNmae, ipa, Convert.ToInt32(strRemotPort), port, intBasePort, MaxWaiteTime, MaxWaitePerbyte);
        }

        /// <summary>
        /// 注册端口[串口]
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="strSetting">串口设置</param>
        private void RegisterPort(int port, string strSetting)
        {
            if (strSetting == string.Empty || strSetting == null)
                strSetting = "2400,e,8,1";
            string portName = GetPortNameCOM(port);
            //注册设置端口2400
            SockPool.Instance.AddComSock(portName, port, strSetting, 1000, 100);
        }

        /// <summary>
        /// 使用指定的端口发送数据包
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="sendPacket">发送包</param>
        /// <param name="recvPacket">回复包,如果不需要回复可以为null</param>
        /// <param name="setting">RS485波特率</param>
        /// <returns>发送是否成功</returns>
        internal bool SendData(string portName, SendPacket sendPacket, RecvPacket recvPacket, string setting)
        {

            //string[] testname = portName.Split('_');  //暂时调试用
            //if (testname[2] == "1")
            //{
            //    CLDC_DataCore.Const.GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯485;
            //    setting = "2400,e,8,1";
            //}
            //else
            //{
            //    CLDC_DataCore.Const.GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯485;
            //    setting = "38400,e,8,1";
            //}

            bool bln_Return = false;
            if (CLDC_DataCore.Const.GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯红外)
            {
                //if (CLDC_DataCore.Const.GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                {
                    if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                    {
                        setting = "1200,e,8,1";
                        string[] strPortName = portName.Split('_');
                        if (strPortName.Length > 1)
                        {
                            if (sendPacket.GetPacketData() != null && sendPacket.GetPacketData().Length > 0)
                            {
                                int meterIndex = int.Parse(strPortName[2]);
                                string strSendFrame = BitConverter.ToString(sendPacket.GetPacketData()).Replace("-", "");
                                string strRecvFrame = "";
                                int result = DeviceControl.Instance.Dev_DeviceControl[0].InitInfrared(meterIndex, setting);
                                if (result == 0)
                                {
                                    result = DeviceControl.Instance.Dev_DeviceControl[0].SendInfraredToMeter(meterIndex, setting, strSendFrame, 5000, ref strRecvFrame);
                                    strRecvFrame = strRecvFrame.Replace(" ", "");
                                    recvPacket.ParsePacket(ConvertArray.GetBytesArry(strRecvFrame.Length / 2, strRecvFrame, false));
                                    bln_Return = result == 0 ? true : false;
                                }
                            }
                        }

                    }
                }
            }
            else if (CLDC_DataCore.Const.GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯载波 || CLDC_DataCore.Const.GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯无线)
            {
              
                string CarrPort = GetPortName(DgnConfigManager.Instance.getCarrierPort(CLDC_DataCore.Const.GlobalUnit.Carrier_Cur_BwIndex));
                SockPool.Instance.UpdatePortSetting(CarrPort, CLDC_DataCore.Const.GlobalUnit.CarrierInfo.BaudRate);
                if (sendPacket is CLDC_MeterProtocol.Packet.MeterProtocolSendPacket)
                {
                    byte[] out_645F=null;
                    ((CLDC_MeterProtocol.Packet.MeterProtocolSendPacket)sendPacket).PacketTo3762(ref out_645F, CLDC_DataCore.Const.GlobalUnit.Carrier_Cur_BwIndex);//376包打完发送了，并直接返回645帧
                    ((CLDC_MeterProtocol.Packet.MeterProtocolRecvPacket)recvPacket).RecvData = out_645F;
                    bln_Return = true;
                }
                else
                {
                    //发送前先更新一下端口
                    bln_Return = SockPool.Instance.Send(CarrPort, sendPacket, recvPacket);//这里发送的是初始化载波节点的。
                }
            }
            else if (CLDC_DataCore.Const.GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯485)
            {
                //if (CLDC_DataCore.Const.GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                {
                    if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                    {
                        string[] strPortName = portName.Split('_');
                        //setting = "2400,e,8,1";
                        if (strPortName.Length > 1)
                        {
                            if (sendPacket.GetPacketData() != null && sendPacket.GetPacketData().Length > 0)
                            {
                                int meterIndex = int.Parse(strPortName[2]);
                                string strSendFrame = BitConverter.ToString(sendPacket.GetPacketData()).Replace("-", "");
                                string strRecvFrame = "";
                                int result = DeviceControl.Instance.Dev_DeviceControl[0].Init485(meterIndex, setting);
                                //多芯表，要工装模块，测试中间需要变波特率， add by wzs on 20200616
                                if(CLDC_DataCore.Const.GlobalUnit.IsGZMK)
                                {
                                    result = DeviceControl.Instance.Dev_DeviceControl[0].Init485(meterIndex, "115200,e,8,1");//38400
                                }
                               
                                if (result == 0)
                                {
                                    result = DeviceControl.Instance.Dev_DeviceControl[0].SendToMeter(meterIndex, setting, strSendFrame, 5000, ref strRecvFrame);
                                    strRecvFrame = strRecvFrame.Replace(" ", "");
                                    if (!string.IsNullOrEmpty(strRecvFrame))
                                    {
                                        recvPacket.ParsePacket(ConvertArray.GetBytesArry(strRecvFrame.Length / 2, strRecvFrame, false));
                                    }
                                    bln_Return = result == 0 ? true : false;
                                }
                            }
                        }
                    }
                }
            }
            else if (CLDC_DataCore.Const.GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙)
            {
                //if (CLDC_DataCore.Const.GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
                {
                    if (DeviceControl.Instance.Dev_DeviceControl != null && DeviceControl.Instance.Dev_DeviceControl.Length > 0)
                    {
                        string[] strPortName = portName.Split('_');
                        setting = "115200,n,8,1";
                        if (strPortName.Length > 1)
                        {
                            if (sendPacket.GetPacketData() != null && sendPacket.GetPacketData().Length > 0)
                            {
                                int meterIndex = int.Parse(strPortName[2]);
                                string strSendFrame = BitConverter.ToString(sendPacket.GetPacketData()).Replace("-", "");
                                string strRecvFrame = "";
                                int result = DeviceControl.Instance.Dev_DeviceControl[0].Init485(meterIndex, setting);
                                if (result == 0)
                                {
                                    result = DeviceControl.Instance.Dev_DeviceControl[0].SendToMeterByBlue(meterIndex, setting, strSendFrame, 5000, ref strRecvFrame);
                                    strRecvFrame = strRecvFrame.Replace(" ", "");
                                    if (!string.IsNullOrEmpty(strRecvFrame))
                                    {
                                        recvPacket.ParsePacket(ConvertArray.GetBytesArry(strRecvFrame.Length / 2, strRecvFrame, false));
                                    }
                                    bln_Return = result == 0 ? true : false;
                                }
                            }
                        }
                    }
                }
            }
            return bln_Return;
        }
    }
}
