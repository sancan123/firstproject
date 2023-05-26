using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置统一日误差被检表脉冲个数和被检表时钟频率
    /// </summary>
    class RequestSetPulseAndTimeFreqPacket:ClouSendPacket_CLT11
    {
        private int pulseNum;
        /// <summary>
        /// 表脉冲个数
        /// </summary>
        public int PulseNum
        {
            get { return pulseNum; }
            set { pulseNum = value; }
        }

        private int timeFreq;
        /// <summary>
        /// 时钟频率
        /// </summary>
        public int TimeFreq
        {
            get { return timeFreq; }
            set { timeFreq = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x20);
            buf.Put(0x18);
            buf.PutInt_S(timeFreq*100);
            buf.PutInt_S(pulseNum);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetPulseAndTimeFreqPacket";
        }
    }
}
