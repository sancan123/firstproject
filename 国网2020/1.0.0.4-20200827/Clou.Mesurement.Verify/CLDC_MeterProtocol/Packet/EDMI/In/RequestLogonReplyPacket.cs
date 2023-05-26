using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.EDMI.In
{
    /// <summary>
    /// EDMI登录回复包
    /// </summary>
    public class RequestLogonReplyPacket:EDMIRecvPacket
    {
        protected override bool ParseBody(byte[] buf)
        {
            return true;
        }
    }
}
