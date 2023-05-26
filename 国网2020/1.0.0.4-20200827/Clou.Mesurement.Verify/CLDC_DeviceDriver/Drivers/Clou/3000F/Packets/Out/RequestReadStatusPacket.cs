using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取工作状态请求包
    /// </summary>
    class RequestReadStatusPacket:ClouSendPacket_CLT11
    {

        protected override byte[] GetBody()
        {
            return new byte[]{
                0x50};
        }

        public override string GetPacketName()
        {
            return "RequestReadStatusPacket";
        }
    }
}
