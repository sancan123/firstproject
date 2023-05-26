using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置统一校验圈数
    /// </summary>
    class RequestSetSameVerifyCirclePacket:ClouSendPacket_CLT11
    {
        private int cricleNum;
        /// <summary>
        /// 圈数
        /// </summary>
        public int CricleNum
        {
            get { return cricleNum; }
            set { cricleNum = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x04);
            buf.PutInt_S(cricleNum);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetSameVerifyCirclePacket";
        }
    }
}
