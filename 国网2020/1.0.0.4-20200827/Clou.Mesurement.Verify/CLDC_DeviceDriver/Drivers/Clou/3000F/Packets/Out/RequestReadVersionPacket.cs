using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取版本号请求包
    /// </summary>
    class RequestReadVersionPacket:ClouSendPacket_CLT11 
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0xc9};
        }

        public override string GetPacketName()
        {
            return "RequestReadVersionPacket";
        }
    }
}
