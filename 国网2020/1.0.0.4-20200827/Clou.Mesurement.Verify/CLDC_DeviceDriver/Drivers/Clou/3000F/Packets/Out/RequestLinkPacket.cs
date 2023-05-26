using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 联机请求包
    /// </summary>
    class RequestLinkPacket:ClouSendPacket_CLT11 
    {
        private bool isLink=true;
        /// <summary>
        /// 是否联机
        /// </summary>
        public bool IsLink
        {
            get { return isLink; }
            set { isLink = value; }
        }

        protected override byte[] GetBody()
        {

            ByteBuffer buf=new ByteBuffer();
            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x04);
            buf.Put(0x01);
            if (isLink)
            {
                buf.Put(0x01);
            }
            else
            { 
                buf.Put(0x00);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestLinkPacket";
        }
    }
}
