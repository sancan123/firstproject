using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2029B联机/脱机请求包
    /// </summary>
    internal class CL2029B_RequestLinkPacket : CL2029BSendPacket
    {
        public bool IsLink = true;

        public CL2029B_RequestLinkPacket()
            : base(false)
        {}

        /*
         * 81 30 PCID 08 C1 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);            
            buf.Put(0x02);        
            
            return buf.ToByteArray();
        }
    }
}
