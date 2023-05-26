using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 
    /// </summary>
    class Geny_RequestWaveResetPacket : GenySendPacket
    {
        public Geny_RequestWaveResetPacket(byte deviceID)
            : base(deviceID, 0x17)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
