using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2030CT档位控制器联机/脱机请求包
    /// </summary>
    internal class CL2030_RequestSetLinkTypePacket : CL2030SendPacket
    {
        public bool IsLink = true;
        

        public CL2030_RequestSetLinkTypePacket()
            : base()
        {}

        
        /*
         * 81 30 PCID 08 C0 01 00 CS
         */
        protected override byte[] GetBody()
        {            
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC5);          //命令 
            buf.Put(0x01);
            if (CLDC_Comm.GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
                buf.Put(0x33);
            else
                buf.Put(0x34);
            return buf.ToByteArray();
        }
    }
}
