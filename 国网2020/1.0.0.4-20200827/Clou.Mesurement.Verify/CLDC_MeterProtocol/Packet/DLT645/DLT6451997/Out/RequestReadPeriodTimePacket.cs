using System;
using System.Collections.Generic;
using System.Text;
using MeterProtocol.Packet.DLT645;

namespace MeterProtocol.Packet.DLT645.DLT6451997.Out
{
    class RequestReadPeriodTimePacket:DL645SendPacket
    {
        public RequestReadPeriodTimePacket()
            : base()
        {
            CmdCode = 0x01;   
        }
        public byte ReadType
        {
            set
            {

            }
        }
    }
}
