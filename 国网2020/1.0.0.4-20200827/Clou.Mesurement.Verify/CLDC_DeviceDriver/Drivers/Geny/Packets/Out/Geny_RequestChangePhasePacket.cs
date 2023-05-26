using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 相位更改指令
    /// </summary>
    class Geny_RequestChangePhasePacket : GenySendPacket
    {
        private double phaseDataValue = 0D;
        public Geny_RequestChangePhasePacket(PhaseType phase, double phaseData)
            : base(GetDriverId(phase), 0x0C)
        {
            phaseDataValue = phaseData;
        }

        protected override byte[] GetBody()
        {
            return DataFormart.Formart(phaseDataValue, 3, false);
        }
    }
}
