using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取 ptct 数据
    /// </summary>
    class Geny_RequestReadPTCTPacket : GenySendPacket
    {

        public Geny_RequestReadPTCTPacket(byte deviceID)
            : base(deviceID, 0x10)
        {

        }

        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
