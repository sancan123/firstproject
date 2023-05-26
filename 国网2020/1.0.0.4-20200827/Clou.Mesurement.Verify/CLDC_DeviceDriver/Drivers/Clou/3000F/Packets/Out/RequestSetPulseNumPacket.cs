using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置日误差被检脉冲个数
    /// </summary>
    class RequestSetPulseNumPacket : ClouSendPacket_CLT11
    {
        private byte[] pulseNum;
        /// <summary>
        /// 脉冲个数
        /// </summary>
        public byte[] PulseNum
        {
            get { return pulseNum; }
            set { pulseNum = value; }
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
            buf.Put(0x2c);
            buf.PutInt_S(StartPos);
            buf.PutInt_S(pulseNum.Length * 4);

            for (int i = 0; i < pulseNum.Length; i++)
            {
                buf.PutInt_S(pulseNum[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetPulseNumPacket";
        }
    }
}
