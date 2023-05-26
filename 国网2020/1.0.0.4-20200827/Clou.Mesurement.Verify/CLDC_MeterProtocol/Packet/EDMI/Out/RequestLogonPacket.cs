using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.EDMI.Out
{
    /// <summary>
    /// 红相登录请求包
    /// </summary>
    public class RequestLogonPacket : EDMISendPacket
    {
        public RequestLogonPacket(string user,string pass)
            : base()
        {
            byte[] byt_Value = ASCIIEncoding.ASCII.GetBytes(user + "," + pass);
            Array.Resize(ref byt_Value, byt_Value.Length + 1);
            Data.Add(byt_Value);
            CmdCode = EDMICmdCode.CST_BYT_EXE;
            SubCmd = (byte)EDMICmdCode.CST_BYT_LOGON;
        }
    }
}
