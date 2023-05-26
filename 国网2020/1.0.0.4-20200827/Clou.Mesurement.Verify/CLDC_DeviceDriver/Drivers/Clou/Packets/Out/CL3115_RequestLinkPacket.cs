using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL3115_RequestLinkPacket : CL3115SendPacket
    {
        public bool IsLink = true;

        public CL3115_RequestLinkPacket()
            : base()
        {}

        public override string GetPacketResolving()
        {
            return "标准表联机/脱机请求包，读取本机真实常数";
        }
        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }
    }
}
