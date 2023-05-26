using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取检定数据请求包
    /// </summary>
    internal class CL311_RequestReadVerifyDataPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVerifyDataPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x02);
            return buf.ToByteArray();
        }
    }
}
