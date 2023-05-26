using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置191通道请求包
    /// </summary>
    internal class CL191_RequestSetChannelPacket : Cl191SendPacket
    {
        public CL191_RequestSetChannelPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;

        }
        /// <summary>
        /// 通道类型[0xFF为标准电能脉冲，00为时间脉冲]
        /// </summary>
        public byte channelType = 0;

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xa3);
            buf.Put(0);
            buf.Put(0);
            buf.Put(0);
            buf.Put(channelType);
            return buf.ToByteArray();
        }

      
    }
}
