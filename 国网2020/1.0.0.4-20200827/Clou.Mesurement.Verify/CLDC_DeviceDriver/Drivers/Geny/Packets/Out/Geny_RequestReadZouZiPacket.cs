using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 走字读数包
    /// </summary>
    class Geny_RequestReadZouZiPacket : Geny_RequestReadPacket
    {

        public Geny_RequestReadZouZiPacket()
        { }

        public Geny_RequestReadZouZiPacket(byte meterIndex)
            : base(meterIndex, 0x1e)
        {
        }
    }
}
