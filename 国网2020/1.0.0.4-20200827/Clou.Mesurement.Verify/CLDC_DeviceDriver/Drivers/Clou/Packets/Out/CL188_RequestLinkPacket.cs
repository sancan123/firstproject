using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 188联机操作请求包
    /// </summary>
    internal class CL188_RequestLinkPacket:Cl188SendPacket
    {

        public CL188_RequestLinkPacket()
            : base()
        {
        }
        /// <summary>
        /// 误差路数
        /// </summary>
        //public byte Pos = 0;

        public override string GetPacketName()
        {
            return "CL188_RequestLinkPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x36);
            buf.Put(Pos);
            return buf.ToByteArray();
        }

    }
}
