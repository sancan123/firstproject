using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 调整相位
    /// </summary>
    class Geny_RequestPhaseOffsetPacket : GenySendPacket
    {

        public double PhaseValue
        {
            get;
            set;
        }

        public Geny_RequestPhaseOffsetPacket(byte deviceID, double phaesValue)
            : base(deviceID, 0x16)
        {
            this.PhaseValue = phaesValue;
        }

        protected override byte[] GetBody()
        {
           return DataFormart.Formart((ushort)(this.PhaseValue*100),false);
        }
    }
}
