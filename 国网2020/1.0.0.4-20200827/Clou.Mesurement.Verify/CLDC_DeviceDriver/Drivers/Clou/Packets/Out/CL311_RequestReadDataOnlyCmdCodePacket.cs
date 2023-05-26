using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 一个参数请求包
    /// </summary>
    internal class CL311_RequestReadDataOnlyCmdCodePacket : Cl311SendPacket
    {
        private byte m_CmdCode = 0;
        public CL311_RequestReadDataOnlyCmdCodePacket(byte cmd)
        {
            m_CmdCode = cmd;
            ToID = 0x16;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_CmdCode);
            return buf.ToByteArray();
        }
    }
}
