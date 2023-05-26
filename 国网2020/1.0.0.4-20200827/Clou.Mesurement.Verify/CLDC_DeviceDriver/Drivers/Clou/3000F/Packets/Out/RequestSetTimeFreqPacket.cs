using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置日计时误差时钟频率
    /// </summary>
    class RequestSetTimeFreqPacket : ClouSendPacket_CLT11
    {
        private byte[] timeFreq;
        /// <summary>
        /// 时钟频率
        /// </summary>
        public byte[] TimeFreq
        {
            get { return timeFreq; }
            set { timeFreq = value; }
        }

        /// <summary>
        /// 起始位置
        /// </summary>
        private int StartPos = 0;

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa6);
            buf.Put(0x00);
            buf.Put(0x2b);
            buf.PutInt_S(StartPos);
            buf.PutInt_S(timeFreq.Length * 4);

            for (int i = 0; i < timeFreq.Length; i++)
            {
                buf.PutInt_S(timeFreq[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetTimeFreqPacket";
        }
    }
}
