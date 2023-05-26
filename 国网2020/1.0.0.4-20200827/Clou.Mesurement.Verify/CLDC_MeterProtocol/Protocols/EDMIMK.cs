using System;
using System.Collections.Generic;
using System.Text;
using MeterProtocol.Packet.EDMI.Out;
using MeterProtocol.Packet.EDMI.In;
using SocketModule.Packet;
using MeterProtocol.Packet.EDMI;

namespace MeterProtocol.Protocols
{
    /// <summary>
    /// ElSTER(ABB)的ALPHA协议基类
    /// </summary>
  public  class EDMIMK : ProtocolBase
    {
        /// <summary>
        /// 通讯测试
        /// </summary>
        /// <returns></returns>
        public override bool ComTest()
        {
            return Logon(protocolInfo.UserID,protocolInfo.WritePassword);
        }



        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="str_UserID"></param>
        /// <param name="str_Password"></param>
        /// <returns></returns>
        public bool Logon( string str_UserID, string str_Password)
        {
            RequestLogonPacket sendPacket = new RequestLogonPacket(str_UserID, str_Password);
            RequestLogonReplyPacket recvPacket = new RequestLogonReplyPacket();
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.SubCmd == 0x06;
            }
            return false;
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns></returns>
        private bool LogonOut()
        {
            EDMISendPacket sendPacket = new EDMISendPacket() { 
            CmdCode=EDMICmdCode.CST_BYT_EXE,
            SubCmd = (byte)EDMICmdCode.CST_BYT_EXIT
            };
            EDMIRecvPacket recvPacket = new EDMIRecvPacket();
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.SubCmd == 0x06;
            } return false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        private new bool SendData(EDMISendPacket sendPacket, EDMIRecvPacket recvPacket)
        {
            if (sendPacket != null && recvPacket != null)
                recvPacket.RandData = sendPacket.RandData;
            if (sendPacket != null)
                sendPacket.Address = MeterAddress;
            return base.SendData(sendPacket, recvPacket);
        }
    }
}
