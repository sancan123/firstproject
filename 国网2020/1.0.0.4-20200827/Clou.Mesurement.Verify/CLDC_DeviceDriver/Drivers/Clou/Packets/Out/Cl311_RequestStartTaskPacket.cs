using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 请求启动标准表指令包
    /// 返回0x4b成功
    /// </summary>
    internal class Cl311_RequestStartTaskPacket : Cl311SendPacket
    {
        public Cl311_RequestStartTaskPacket()
            : base()
        {
            ToID = 0x16;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
}
