using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6452007.In
{
    class RequestReadAddressReplayPacket:DL645RecvPacket
    {
        protected override bool ParseBody(byte[] buf)
        {
            Array.Reverse(buf);
            Address = BitConverter.ToString(buf).Replace("-", "").Substring(8);
            return true;
        }
    }
}
