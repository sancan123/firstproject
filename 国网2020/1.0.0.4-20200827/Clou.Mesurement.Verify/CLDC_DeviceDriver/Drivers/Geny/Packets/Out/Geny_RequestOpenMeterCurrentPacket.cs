using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 打开某表位 对应的 电流
    /// </summary>
    class Geny_RequestOpenMeterCurrentPacket : GenySendPacket
    {
        /// <summary>
        /// 要打开的表位地址
        /// </summary>
        /// <param name="meterIndex"></param>
        public Geny_RequestOpenMeterCurrentPacket(byte meterIndex)
            : base(meterIndex, 0x29)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
