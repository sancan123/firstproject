using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    internal class CL188_RequestSetTimeErrorOfDayStdFreqPacket : Cl188SendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte m_Pos = 0;

        private int m_StdFreq = 0;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位:0XFF广播</param>
        /// <param name="freq">标准时钟频率</param>
        public void SetPara(byte pos, int freq)
        {
            m_Pos = pos;
            m_StdFreq = freq * 100;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetTimeErrorOfDayStdFreqPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x45);
            buf.Put(m_Pos);
            buf.PutInt(m_StdFreq);
            return buf.ToByteArray();
        }
    }
}
