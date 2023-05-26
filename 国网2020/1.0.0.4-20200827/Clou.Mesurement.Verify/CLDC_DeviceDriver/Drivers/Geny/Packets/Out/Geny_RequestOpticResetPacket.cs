using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 表光电头复位命令
    /// </summary>
    class Geny_RequestOpticResetPacket : GenySendPacket
    {
        public Geny_RequestOpticResetPacket(byte meterIndex)
            : base(meterIndex, 0x36)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
