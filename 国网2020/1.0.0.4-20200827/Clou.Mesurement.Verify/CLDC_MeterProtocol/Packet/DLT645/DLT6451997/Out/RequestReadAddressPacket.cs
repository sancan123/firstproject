using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.Out
{
    class RequestReadAddressPacket : DL645SendPacket
    {
        public RequestReadAddressPacket()
            : base()
        {
            Address = "AAAAAAAAAAAA";
            CmdCode = 0x01;
        }
    }
}
