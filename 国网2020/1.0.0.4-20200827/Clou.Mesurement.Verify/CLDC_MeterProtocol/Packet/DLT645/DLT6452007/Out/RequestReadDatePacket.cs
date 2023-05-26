﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6452007.Out
{
    class RequestReadDatePacket:DL645SendPacket
    {
        public RequestReadDatePacket()
            : base()
        {
            CmdCode = 0x11;
            Data.Add(new byte[] { 0x04, 0x00, 0x01, 0x01 });
        }
    }
}
