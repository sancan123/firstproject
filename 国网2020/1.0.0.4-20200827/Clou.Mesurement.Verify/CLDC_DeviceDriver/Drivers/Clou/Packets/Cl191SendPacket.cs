using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 时基源发送包基类
    /// </summary>
    internal class Cl191SendPacket : ClouSendPacket_CLT11
    {
        public Cl191SendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        public Cl191SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
