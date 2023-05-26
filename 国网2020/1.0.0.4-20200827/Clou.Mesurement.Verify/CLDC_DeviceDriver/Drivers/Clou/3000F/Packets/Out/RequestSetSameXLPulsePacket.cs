using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    
    /// <summary>
    /// 设置统一需量周期脉冲个数和脉冲间隔时间
    /// </summary>
    class RequestSetSameXLPulsePacket : ClouSendPacket_CLT11
    {
        private byte pulse_num;
        private byte pulse_Time;
        /// <summary>
        /// 需量周期脉冲个数
        /// </summary>
        public byte Pulse_num
        {
            get { return pulse_num; }
            set { pulse_num = value; }
        }
        /// <summary>
        /// 需量周期脉冲间隔时间
        /// </summary>
        public byte Pulse_Time
        {
            get { return pulse_Time; }
            set { pulse_Time = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x28);
            buf.Put(0x20);
            buf.Put(pulse_num);
            buf.Put(pulse_Time);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetSameXLPulsePacket";
        }
    }
    
}
