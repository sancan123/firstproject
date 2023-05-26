using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 累积脉冲读数
    /// </summary>
    class Geny_RequestReadMaiChongPacket : Geny_RequestReadPacket
    {

        public Geny_RequestReadMaiChongPacket()
        { }

        public Geny_RequestReadMaiChongPacket(byte meterIndex)
            : base(meterIndex, 0x24)
        {
        }
    }
}
