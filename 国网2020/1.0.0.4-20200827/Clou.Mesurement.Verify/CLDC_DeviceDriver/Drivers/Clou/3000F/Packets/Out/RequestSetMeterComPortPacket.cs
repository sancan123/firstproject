using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置电能表通讯端口
    /// </summary>
    class RequestSetMeterComPortPacket : ClouSendPacket_CLT11
    {
        private byte funRoute;
        private bool isOpen;
        /// <summary>
        /// 误差路数
        /// </summary>
        public byte FunRoute
        {
            get { return funRoute; }
            set { funRoute = value; }
        }
        /// <summary>
        /// 是否打开端口
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x03);
            buf.Put(funRoute);
            if (isOpen)
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
            return "RequestSetMeterComPortPacket";
        }
    }
}
