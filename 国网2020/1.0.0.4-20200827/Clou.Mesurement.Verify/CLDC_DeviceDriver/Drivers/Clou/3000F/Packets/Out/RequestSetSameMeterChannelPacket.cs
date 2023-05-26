using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置统一被检表通道号
    /// </summary>
    class RequestSetSameMeterChannelPacket:ClouSendPacket_CLT11
    {
        private int meterChannel;
        /// <summary>
        /// 表通道号
        /// </summary>
        public int MeterChannel
        {
            get { return meterChannel; }
            set { meterChannel = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x20);
            buf.Put(0x40);
            buf.PutInt_S(meterChannel);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetSameMeterChannelPacket";
        }
    }
}
