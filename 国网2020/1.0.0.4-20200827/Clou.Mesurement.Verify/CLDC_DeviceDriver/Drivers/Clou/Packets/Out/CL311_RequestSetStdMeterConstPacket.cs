using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置标准表常数
    /// </summary>
    internal class CL311_RequestSetStdMeterConstPacket : Cl311SendPacket
    {
        private byte m_auto = 0;
        private int m_meterconst = 0;

        public void SetPara(int meterconst, bool auto)
        {
            m_meterconst = meterconst;
            if (auto)
                m_auto = 0;
            else
                m_auto = 1;
        }
        public override string GetPacketName()
        {
            return "CL311_RequestSetStdMeterConstPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x44);
            buf.Put(m_auto);
            buf.PutInt(m_meterconst);
            return buf.ToByteArray();
        }
    }
}
