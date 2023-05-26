using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 电流谐波设置 
    /// </summary>
    class Geny_RequestCurrentHarmonicMultiPacket : Geny_RequestVoltageHarmonicMultiPacket
    {
        public Geny_RequestCurrentHarmonicMultiPacket(/*float[] voltages,*/ Single[] value, int[] times, float[] phaseAngles, PhaseType phaseType)
            : base(value, times, phaseAngles, phaseType)
        {
            this.CmdCode = 0x26;
        }
    }
}
