using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 309功率源发送包基类
    /// </summary>
    internal class Cl309SendPacket : ClouSendPacket_CLT11
    {
        public Cl309SendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x07;
        }

        public Cl309SendPacket(bool needReplay)
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
