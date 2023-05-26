using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.Out
{
    class RequestReadTimePacket:DL645SendPacket
    {
        public RequestReadTimePacket()
            : base()
        {
            byte[] data = new byte[2] { 0xC0, 0x11 };
            Data.Add(data);
        }
    }
}
