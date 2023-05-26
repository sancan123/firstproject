using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL311_RequestLinkPacket : Cl311SendPacket
    {
        public bool IsLink = true;

        public CL311_RequestLinkPacket()
        {
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (IsLink)
                buf.Put(0x60);
            else
                buf.Put(0x63);
            return buf.ToByteArray();
        }
    }
}
