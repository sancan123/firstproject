using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取电能累计脉冲数
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalPulseNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalPulseNumPacket()
            : base()
        {}

        /*
         * 81 30 PCID 09 a0 02 40 80 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x40);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
}
