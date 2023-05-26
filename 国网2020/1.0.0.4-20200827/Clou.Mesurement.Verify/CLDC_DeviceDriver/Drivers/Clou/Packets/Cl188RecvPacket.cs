using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// CL188接收数据包基类
    /// </summary>
    internal class Cl188RecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }

    }
}
