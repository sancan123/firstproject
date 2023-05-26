using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class CL3115SendPacket : ClouSendPacket_CLT11
    {
        public CL3115SendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        public CL3115SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
