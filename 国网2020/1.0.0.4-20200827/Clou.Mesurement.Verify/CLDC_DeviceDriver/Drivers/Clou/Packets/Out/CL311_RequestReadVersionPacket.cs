using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 请求读取标准表版本号请求包
    /// </summary>
    internal class CL311_RequestReadVersionPacket : Cl311SendPacket
    {
       public override string GetPacketName()
       {
           return "CL311_RequestReadVersionPacket";
       }
       protected override byte[] GetBody()
       {
           ByteBuffer buf = new ByteBuffer();
           buf.Initialize();
           buf.Put(0x20);
           return buf.ToByteArray();
       }
    }
}
