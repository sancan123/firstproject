using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置某一相，正反 类型
    /// </summary>
    class Geny_RequestPhaseModeCtr : GenySendPacket
    {

        public GenyActiveType ActiveType
        {
            get;
            set;
        }

        public Geny_RequestPhaseModeCtr(PhaseType phase, GenyActiveType activeType)
            : base(GetDriverId(phase), 0x11)
        {
            this.ActiveType = activeType;
        }

        protected override byte[] GetBody()
        {
            return new byte[] { (byte)(this.ActiveType),0 };
        }
    }
}
