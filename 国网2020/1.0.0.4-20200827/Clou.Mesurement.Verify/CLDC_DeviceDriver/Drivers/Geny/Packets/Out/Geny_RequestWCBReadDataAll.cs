using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取误差板6次误差数据
    /// </summary>
    class Geny_RequestWCBReadDataAll : GenySendPacket
    {
        public Geny_RequestWCBReadDataAll(byte deviceID)
            : base(deviceID, 0x30)
        {

        }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
