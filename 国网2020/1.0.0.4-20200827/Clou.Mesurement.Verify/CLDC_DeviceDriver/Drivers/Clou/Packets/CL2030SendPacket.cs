using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 2030 CT档位控制器发送包基类
    /// </summary>
    internal class CL2030SendPacket : ClouSendPacket_CLT11
    {
        public CL2030SendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        public CL2030SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x01;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
