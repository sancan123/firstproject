using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 停止计算 误差 
    /// </summary>
    class Geny_RequestWCBStopErrCountPacket : GenySendPacket
    {
        public Geny_RequestWCBStopErrCountPacket(byte deviceID)
            : base(deviceID, 0x2A)
        {
        }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
