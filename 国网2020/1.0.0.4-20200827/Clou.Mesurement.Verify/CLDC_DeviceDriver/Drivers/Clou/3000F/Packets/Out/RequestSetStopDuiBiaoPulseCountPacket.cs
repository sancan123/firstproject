using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置停止对标脉冲个数请求包
    /// </summary>
    class RequestSetStopDuiBiaoPulseCountPacket:ClouSendPacket_CLT11
    {
        /// <summary>
        /// 起始位置
        /// </summary>
        private int StartPos = 0;

        private byte[] pulseCount;
        /// <summary>
        /// 停止对标脉冲个数
        /// </summary>
        public byte[] PulseCount
        {
            get { return pulseCount; }
            set { pulseCount = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa6);
            buf.Put(0x00);
            buf.Put(0x1f);

            buf.PutInt_S(StartPos);
            buf.PutUShort_S((ushort)(pulseCount.Length * 2));

            for (int i = 0; i < pulseCount.Length; i++)
            {
                buf.PutUShort_S(pulseCount[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetStopDuiBiaoPulseCountPacket";
        }
    }
}
