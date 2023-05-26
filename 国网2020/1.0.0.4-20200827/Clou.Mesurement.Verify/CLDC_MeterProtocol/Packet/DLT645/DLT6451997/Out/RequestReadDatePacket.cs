using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.Out
{
    class RequestReadDatePacket : DL645SendPacket
    {
        public RequestReadDatePacket()
            : base()
        {
            byte[] data = new byte[2]{0xC0,0x10};
            Data.Add(data);
        }
    }
}
