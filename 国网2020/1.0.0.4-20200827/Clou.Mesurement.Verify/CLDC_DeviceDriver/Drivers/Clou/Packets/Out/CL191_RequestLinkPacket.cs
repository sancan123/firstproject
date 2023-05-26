using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{

    /// <summary>
    /// 191联机操作请求包
    /// 回复:RequestResultReplayPacket
    /// </summary>
    internal class CL191_RequestLinkPacket : Cl191SendPacket
    {

        public CL191_RequestLinkPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;
        }

        public override string GetPacketName()
        {
            return "CL191_RequestLinkPacket";
        }

        public override string GetPacketResolving()
        {
            return "191联机操作请求包";
        }

        public override int GetPacketType()
        {
            return 1;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[5] { 0xA3, 0x00, 0x00, 0x00,0xFF };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
}
