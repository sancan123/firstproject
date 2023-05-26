using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读GPS时间请求包
    /// </summary>
    class RequestReadGpsTimePacket:ClouSendPacket_CLT11
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0x09,0xa0,0x00,0x02,0x8e};
        }

        public override string GetPacketName()
        {
            return "RequestReadGpsTimePacket";
        }
    }
}
