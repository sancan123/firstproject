using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2030CT档位控制器清除过载信号请求包
    /// </summary>
    internal class CL2030_RequestClearOverPacket : CL2030SendPacket
    {
        public bool IsLink = true;

        public CL2030_RequestClearOverPacket()
            : base()
        {}

        /*
         * 81 30 PCID 08 C1 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC1);          //命令 
            buf.Put(0x01);
            buf.Put(0x00);
            return buf.ToByteArray();
        }
    }
}
