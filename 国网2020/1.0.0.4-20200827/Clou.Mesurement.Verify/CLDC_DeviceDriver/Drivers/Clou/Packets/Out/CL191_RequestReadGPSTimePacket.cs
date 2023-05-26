using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
 
     /// <summary>
    /// 191读取GPS时间请求包
    /// 回复:CL191_RequestReadGPSTimePacket
    /// </summary>
    internal class CL191_RequestReadGPSTimePacket : Cl191SendPacket
    {

        public CL191_RequestReadGPSTimePacket():base()
        {
            ToID = 0xBF;
            MyID = 0x20;
        }

        public override string GetPacketName()
        {
            return "CL191_RequestLinkPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[4] { 0xA0 ,00 ,00 ,00 };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
}
