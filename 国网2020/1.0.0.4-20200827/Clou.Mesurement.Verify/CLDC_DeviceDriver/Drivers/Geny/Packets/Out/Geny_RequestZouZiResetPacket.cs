using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 走字 复位
    /// </summary>
    class Geny_RequestZouZiResetPacket : GenySendPacket
    {
        public int ImpluseCount
        {
            get;
            set;
        }

        public Geny_RequestZouZiResetPacket(byte deviceID, int impluseCount)
            : base(deviceID, 0x28)
        {
            this.ImpluseCount = impluseCount;
        }

        protected override byte[] GetBody()
        {
            return DataFormart.Formart(this.ImpluseCount, 0, false);
        }
    }
}
