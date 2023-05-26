using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 启动/停止当前设置的功能
    /// </summary>
    class RequestCtrlVerifyPacket : ClouSendPacket_CLT11
    {
        private bool isStop;
        /// <summary>
        /// 是否停止
        /// </summary>
        public bool IsStop
        {
            get { return isStop; }
            set { isStop = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x10);
            buf.Put(0x01);
            if (isStop = true)
            {  
                buf.Put(0x00);
            }
            else
            {
                buf.Put(0x01);
            }
            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestCtrlVerifyPacket";
        }
    }
}
