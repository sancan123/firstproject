using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取标准数据请求包
    /// </summary>
    class RequestReadStdInfoPacket:ClouSendPacket_CLT11 
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0xa0,0x02,0x3f,0x7f,0x08,0x3f,0xff,0xff,0x0f
            };
        }

        public override string GetPacketName()
        {
            return "RequestReadStdInfoPacket";
        }
    }
}
