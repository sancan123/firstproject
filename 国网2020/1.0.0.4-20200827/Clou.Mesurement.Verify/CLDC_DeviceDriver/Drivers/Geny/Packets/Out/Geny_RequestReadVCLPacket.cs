using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取 电压，电流档位
    /// </summary>
    class Geny_RequestReadVCLPacket : GenySendPacket
    {
        public Geny_RequestReadVCLPacket(byte deviceID)
            : base(deviceID, 0x0F)
        {
        }


        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
