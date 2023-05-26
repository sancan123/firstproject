using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 单次 电流 谐波设置
    /// </summary>
    class Geny_RequestCurrentHarmonicPacket : Geny_RequestVoltageHarmonicPacket
    {
        public Geny_RequestCurrentHarmonicPacket(float value, int times, PhaseType phase, GenyHarmonicType harmonicType, double phaseAngle)
            : base(value, times, phase, harmonicType, phaseAngle)
        {
            this.CmdCode = 0x15;
        }
    }
}
