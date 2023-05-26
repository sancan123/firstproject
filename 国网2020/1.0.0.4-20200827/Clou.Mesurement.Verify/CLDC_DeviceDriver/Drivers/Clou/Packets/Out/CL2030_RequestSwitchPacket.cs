using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2030CT档位控制器联机/脱机请求包
    /// </summary>
    internal class CL2030_RequestSwitchPacket : CL2030SendPacket
    {
        public bool IsLink = true;

        /// <summary>
        /// CT档位类型  0x00,100A档位；0x01，2A档位
        /// </summary>
        private byte byt_CurrentType = 0;

        public CL2030_RequestSwitchPacket()
            : base()
        {}

        public void SetPara(int intI)
        {
            if (intI > 2)
                byt_CurrentType = 0x00;
            else
                byt_CurrentType = 0x01;
        }
        /*
         * 81 30 PCID 08 C0 01 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xC3);          //命令 
            buf.Put(0x01);
            buf.Put(byt_CurrentType);
            return buf.ToByteArray();
        }
    }
}
