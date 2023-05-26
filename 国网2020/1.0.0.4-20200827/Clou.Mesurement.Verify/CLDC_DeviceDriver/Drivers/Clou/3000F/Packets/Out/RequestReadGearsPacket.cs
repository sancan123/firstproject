using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取档位请求包
    /// </summary>
    class RequestReadGearsPacket:ClouSendPacket_CLT11
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0xa0,0x02,0x02,0x3f};
        }

        public override string GetPacketName()
        {
            return "RequestReadGearsPacket";
        }
    }
}
