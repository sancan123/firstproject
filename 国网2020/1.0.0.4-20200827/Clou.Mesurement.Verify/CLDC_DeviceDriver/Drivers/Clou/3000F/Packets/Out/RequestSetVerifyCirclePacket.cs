using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置校验圈数
    /// </summary>
    class RequestSetVerifyCirclePacket:ClouSendPacket_CLT11
    {
        private byte[] cricleNum;
        /// <summary>
        /// 圈数
        /// </summary>
        public byte[] CricleNum
        {
            get { return cricleNum; }
            set { cricleNum = value; }
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
            buf.Put(0x1a);
            buf.PutInt_S(StartPos);
            buf.PutInt_S(cricleNum.Length*4);

            for (int i = 0; i<cricleNum.Length; i++)
            {
                buf.PutInt_S(cricleNum[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetVerifyCirclePacket";
        }
    }
}
