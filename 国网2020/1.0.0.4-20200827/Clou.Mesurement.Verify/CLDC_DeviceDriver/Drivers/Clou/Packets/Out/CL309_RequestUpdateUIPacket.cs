using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 更新源 请求包
    /// </summary>
    internal class CL309_RequestUpdateUIPacket : Cl309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestUpdateUIPacket()
            : base()
        {}

        /*
         * 81 01 PCID 06 C9 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();                      
            buf.Put(0xA3);  //命令           
            buf.Put(0x05);
            buf.Put(0x44);
            buf.Put(0x80);
            buf.Put(0x07);
            buf.Put(0x0B);
            buf.Put(0x3F);
            buf.Put(0x3F);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
}
