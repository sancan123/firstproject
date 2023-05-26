using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// CL188L接收数据包基类
    /// </summary>
    internal class Cl188LRecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
}
