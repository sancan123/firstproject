using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 初始化 台体 GPS
    /// 不需要返回
    /// 选通道 4
    /// </summary>
    class Geny_RequestGPSIntPacket : GenySendPacket
    {
        public Geny_RequestGPSIntPacket()
        {
            this.IsNeedReturn = false;
        }

        public override byte[] GetPacketData()
        {
            return new byte[] { 0x50, 0x47, 0x52, 0x4D, 0x49, 0x45 };
        }

        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
