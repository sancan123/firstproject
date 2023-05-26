using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读温度湿度请求包
    /// </summary>
    class RequestReadTmpAndHuiPacket:ClouSendPacket_CLT11
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0xa0,0x00,0x04,0x30};
        }

        public override string GetPacketName()
        {
            return "RequestReadTmpAndHuiPacket";
        }
    }
}
