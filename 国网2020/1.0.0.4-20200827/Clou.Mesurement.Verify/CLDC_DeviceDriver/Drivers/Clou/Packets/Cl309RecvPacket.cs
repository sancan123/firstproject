using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 309 功率源接收基类
    /// </summary>
    internal class Cl309RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
