using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置被检表通道号
    /// </summary>
    class RequestSetMeterChannelPacket:ClouSendPacket_CLT11
    {
        private byte[] meterChannel;
        /// <summary>
        /// 表通道号
        /// </summary>
        public byte[] MeterChannel
        {
            get { return meterChannel; }
            set { meterChannel = value; }
        }

        /// <summary>
        /// 起始位置
        /// </summary>
        private int StartPos=0;

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa6);
            buf.Put(0x00);
            buf.Put(0x26);
            buf.PutInt_S(StartPos);
            buf.PutInt_S(meterChannel.Length * 4);

            for (int i = 0; i < meterChannel.Length; i++)
            {
                buf.PutInt_S(meterChannel[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetMeterChannelPacket";
        }
    }
}
