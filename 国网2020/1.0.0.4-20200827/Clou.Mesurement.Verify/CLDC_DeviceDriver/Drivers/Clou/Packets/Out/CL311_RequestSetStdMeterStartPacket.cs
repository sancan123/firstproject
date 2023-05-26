using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 启动标准表请求包
    /// </summary>
    internal class CL311_RequestSetStdMeterStartPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestSetStdMeterStartPacket";
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
