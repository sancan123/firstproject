using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 源联机指令
    /// 0x52是字母"R"的ASC码
    /// ox4F是O的ASC码
    /// </summary>
    internal class CL303_RequestLinkPacket : CL303SendPacket
    {

        /// <summary>
        /// 是否是联机
        /// </summary>
        public bool IsLink = true;


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (IsLink)
                buf.Put(0x52);
            else
                buf.Put(0x4F);
            return buf.ToByteArray();
        }
    }
}
