using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 误差板数据包基类
    /// </summary>
    internal class Cl188SendPacket : ClouSendPacket_CLT11 
    {
        public Cl188SendPacket()
            : base()
        {
            ToID = 0x10;
            MyID = 0x20;
        }
        public Cl188SendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x10;
            MyID = 0x20;
        }

        public byte Pos { get; set; }

        protected override byte[] GetBody()
        {
            return new byte[0]; 
        }
    }
}
