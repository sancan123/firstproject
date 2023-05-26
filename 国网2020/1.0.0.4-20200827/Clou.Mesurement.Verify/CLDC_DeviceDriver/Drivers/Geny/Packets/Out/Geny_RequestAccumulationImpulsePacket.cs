using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 脉冲累积 指令
    /// </summary>
    class Geny_RequestAccumulationImpulsePacket : GenySendPacket
    {
        public Geny_RequestAccumulationImpulsePacket(byte meterIndex)
            : base(meterIndex, 0x23)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
