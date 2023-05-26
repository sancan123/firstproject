using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取表常数请求包
    /// </summary>
    class RequestReadMeterConstPacket:ClouSendPacket_CLT11
    {
        protected override byte[] GetBody()
        {
            return new byte[]{
                0xa0,0x00,0x08,0x40};
        }

        public override string GetPacketName()
        {
            return "RequestReadMeterConstPacket";
        }
    }
}
