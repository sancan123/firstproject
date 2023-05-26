using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取测量数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterTestDataPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTestDataPacket()
            : base()
        {}

        public override string GetPacketResolving()
        {
            string resolve = string.Format("读取测量数据");
            return resolve;
        }
         /*
          * 81 30 PCID 0e        a0 02 3f ff 80 3f ff ff 0f         CS
          */
         protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0x80);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x0F);
            return buf.ToByteArray();
        }
    }
}
