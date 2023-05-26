using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取检定数据请求包
    /// 0x34
    /// </summary>
    internal class CL188_RequestReadVerifyDataPacket : Cl188SendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        public byte Pos = 0;
        public override string GetPacketName()
        {
            return "CL188_RequestReadVerifyDataPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x34);
            buf.Put(Pos);
            return buf.ToByteArray();
        }
    }
}
