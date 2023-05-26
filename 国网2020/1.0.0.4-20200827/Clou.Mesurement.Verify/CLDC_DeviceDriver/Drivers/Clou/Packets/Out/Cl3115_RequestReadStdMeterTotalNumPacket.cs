using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 取电能
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalNumPacket()
            : base()
        {}

        /*
         * 81 30 PCID 09 a0 02 20 10 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x20);
            buf.Put(0x10);
            return buf.ToByteArray();
        }
    }
}
