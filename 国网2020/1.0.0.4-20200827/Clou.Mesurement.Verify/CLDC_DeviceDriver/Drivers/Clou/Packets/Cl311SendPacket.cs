using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class Cl311SendPacket : ClouSendPacket_NotCltOne
    {
        public Cl311SendPacket()
            : base(true, 0x16)
        {
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
