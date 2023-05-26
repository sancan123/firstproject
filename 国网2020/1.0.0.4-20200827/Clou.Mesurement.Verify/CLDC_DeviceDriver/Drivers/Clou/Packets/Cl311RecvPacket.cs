using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 311 标准表接收基类
    /// </summary>
    internal class Cl311RecvPacket : ClouRecvPacket_NotCltOne
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
