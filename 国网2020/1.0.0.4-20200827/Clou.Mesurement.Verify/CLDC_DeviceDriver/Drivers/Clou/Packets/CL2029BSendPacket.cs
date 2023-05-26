using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 2029B发送包基类
    /// </summary>
    internal class CL2029BSendPacket : ClouSendPacket_CLT11
    {
        public CL2029BSendPacket()
            : base()
        {
            ToID = 0x42;
            MyID = 0x01;
        }

        public CL2029BSendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x42;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
