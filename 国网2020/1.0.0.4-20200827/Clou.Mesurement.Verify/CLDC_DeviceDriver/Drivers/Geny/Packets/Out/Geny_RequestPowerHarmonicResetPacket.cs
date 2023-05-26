using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 格林基波复位，
    /// 在进行谐波测试后，需要使用复位指令
    /// </summary>
    class Geny_RequestPowerHarmonicResetPacket : GenySendPacket
    {
        /// <summary>
        /// 设置通道类型
        /// </summary>
        public PhaseType Phase
        {
            set
            {
                this.SendID = GetDriverId(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phase">要设置的相</param>
        public Geny_RequestPowerHarmonicResetPacket(PhaseType phase)
            : base(GetDriverId(phase), 0x17)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
