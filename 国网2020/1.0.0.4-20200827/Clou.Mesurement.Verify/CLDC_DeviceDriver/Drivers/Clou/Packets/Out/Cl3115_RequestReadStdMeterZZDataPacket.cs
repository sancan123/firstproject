using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取电能走字数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterZZDataPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterZZDataPacket()
            : base()
        {}

        /*
         * 81 30 PCID 0a a0 02 60 10 80 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x60);
            buf.Put(0x10);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
}
