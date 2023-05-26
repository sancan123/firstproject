using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取真实本机常数
    /// </summary>
    internal class CL3115_RequestReadStdMeterConstPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterConstPacket()
            : base()
        {}

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }
    }
}
