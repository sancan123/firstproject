using CLDC_Comm.BaseClass;
using CLDC_DataCore.Function;
using CLDC_DataCore.SocketModule;
using CLDC_DataCore.SocketModule.Packet;
using CLDC_DataCore.Struct;
using CLDC_DeviceDriver;
using CLDC_DeviceDriver.PortFactory;
using System;
using System.Collections.Generic;

namespace CLDC_SafeFileProtocol
{
    public class CardControlManager : SingletonBase<CardControlManager>
    {
        private Dictionary<int, StPortInfo> ChannelPortInfo = new Dictionary<int, StPortInfo>();
        StPortInfo[] m_arrRsCardPort = null;

        public int GetChannelCount()
        {
            return ChannelPortInfo.Count;
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
                
                return GetPortName(portInfo);
            }
            return string.Empty;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            DeviceParaUnit[] paraUnitsRsCard = Rs485Setting.GetRsCardPorts();
            m_arrRsCardPort = new StPortInfo[paraUnitsRsCard.Length];
            for (int i = 0; i < m_arrRsCardPort.Length; i++)
            {
                m_arrRsCardPort[i] = new StPortInfo
                {
                    m_Port = paraUnitsRsCard[i].PortNo,
                    m_Port_IsUDPorCom = paraUnitsRsCard[i].Address == "COM" ? false : true,
                    m_IP = paraUnitsRsCard[i].Address,
                    m_Port_Setting = paraUnitsRsCard[i].Baudrate,
                    m_Exist = 1
                };
            }
            for (int i = 0; i < m_arrRsCardPort.Length; i++)
            {
                StPortInfo port = m_arrRsCardPort[i];
                if (ChannelPortInfo.ContainsKey(i + 1))
                {
                    ChannelPortInfo.Remove(i + 1);
                }
                ChannelPortInfo.Add(i + 1, port);

                RegisterPort(paraUnitsRsCard[i]);
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
                RegisterPort(port.PortNo, port.Baudrate, port.Address, port.RemotePort.ToString(), port.StartPort);
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
        private void RegisterPort(int port, string strSetting, string IP, string strRemotPort, int intBasePort)
        {
            System.Net.IPAddress ipa = System.Net.IPAddress.Parse(IP);
            string portNmae = GetPortName(IP, port);
            //注册数据端口
            SockPool.Instance.AddUdpSock(portNmae, ipa, Convert.ToInt32(strRemotPort), port, intBasePort, 1000, 100);
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
            //注册设置端口
            SockPool.Instance.AddComSock(portName, port, strSetting, 1000, 100);
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
            if (port.m_Port_IsUDPorCom == false)
            {
                return string.Format("{0}_{1}", "COM", port.m_Port);
            }
            else
            {
                string strName = "Port_" + port.m_IP;
                return string.Format("{0}_{1}", strName, port.m_Port);
            }
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
            bool bln_Return = false;
            //if (CLDC_DataCore.Const.GlobalUnit.g_Dev_CommunType == CLDC_Comm.Enum.Cus_CommunType.南网通讯DLL)
            {
                if (CardReaderControl.Instance.Dev_CardControl != null && CardReaderControl.Instance.Dev_CardControl.Length > 0)
                {
                    string[] strPortName = portName.Split('_');
                    if (strPortName.Length > 1)
                    {
                        if (sendPacket.GetPacketData() != null && sendPacket.GetPacketData().Length > 0)
                        {
                            int meterIndex = int.Parse(strPortName[2]);
                            string strSendFrame = BitConverter.ToString(sendPacket.GetPacketData()).Replace("-", "");
                            string strRecvFrame = "";
                            int result = CardReaderControl.Instance.Dev_CardControl[0].SendToCard(meterIndex, strSendFrame, 5000, ref strRecvFrame);
                            recvPacket.ParsePacket(ConvertArray.GetBytesArry(strRecvFrame.Length / 2, strRecvFrame, false));
                            bln_Return = result == 0 ? true : false;
                        }
                    }
                }
            }
            return bln_Return;
        }
    }
}
