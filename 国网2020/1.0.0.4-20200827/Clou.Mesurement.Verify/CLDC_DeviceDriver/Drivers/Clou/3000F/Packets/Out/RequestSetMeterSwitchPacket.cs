using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置表位开关请求包
    /// </summary>
    class RequestSetMeterSwitchPacket:ClouSendPacket_CLT11
    {
        private byte[] meterSwitch;
        /// <summary>
        /// 表位开关
        /// </summary>
        public byte[] MeterSwitch
        {
            get { return meterSwitch; }
            set { meterSwitch = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x80);
            buf.Put(0x40);
            buf.Put(meterSwitch);

            return  buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetMeterSwitchPacket";
        }
    }
}
