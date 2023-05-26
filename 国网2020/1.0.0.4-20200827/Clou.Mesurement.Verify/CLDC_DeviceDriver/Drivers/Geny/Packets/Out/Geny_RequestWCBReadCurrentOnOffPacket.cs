using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 读取 电流 状态
    /// </summary>
    class Geny_RequestWCBReadCurrentOnOffPacket  : GenySendPacket
    {
        public Geny_RequestWCBReadCurrentOnOffPacket(byte deviceID)
            : base(deviceID, 0x26)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
