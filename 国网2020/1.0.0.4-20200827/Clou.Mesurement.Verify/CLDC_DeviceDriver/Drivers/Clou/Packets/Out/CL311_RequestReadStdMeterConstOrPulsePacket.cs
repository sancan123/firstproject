using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 35指令,0x12读取标准表常数 0x13读取标准表脉冲数
    /// </summary>
    class CL311_RequestReadStdMeterConstOrPulsePacket : Cl311SendPacket
    {
        private byte m_data = 0x12;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStdConst"></param>
        public CL311_RequestReadStdMeterConstOrPulsePacket(bool readStdConst)
        {
            ToID = 0x16;
            if (readStdConst)
                m_data = 0x12;
            else
                m_data = 0x13;
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x35);
            buf.Put(m_data);
            return buf.ToByteArray();
        }
    }
}
