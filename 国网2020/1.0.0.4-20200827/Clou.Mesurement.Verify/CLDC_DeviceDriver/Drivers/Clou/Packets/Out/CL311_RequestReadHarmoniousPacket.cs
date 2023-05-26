using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取Harmonious请求包
    /// </summary>
    internal class CL311_RequestReadHarmoniousPacket : Cl311SendPacket
    {

        private byte[] m_readtype = new byte[0];
        public byte ReadType
        {
            set
            {
                if (value == 0)
                    m_readtype = new byte[] { 0x32, 0x80, 0x09 };
                else if (value == 1)
                    m_readtype = new byte[] { 0x32, 0xa0, 0x09 };
                else if (value == 2)
                    m_readtype = new byte[] { 0x32, 0xc0, 0x09 };
                else if (value == 3)
                    m_readtype = new byte[] { 0x32, 0xe0, 0x09 };
                else if (value == 4)
                    m_readtype = new byte[] { 0x32, 0x00, 0x0a };
                else
                    m_readtype = new byte[] { 0x32, 0x20, 0x0a };
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestReadHarmoniousPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x32);
            buf.Put(m_readtype);
            return buf.ToByteArray();
        }
    }
}
