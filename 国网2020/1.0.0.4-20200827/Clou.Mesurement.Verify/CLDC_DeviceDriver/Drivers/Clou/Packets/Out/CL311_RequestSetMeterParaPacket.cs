using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置标准表参数请求包
    /// 返回Result包
    /// </summary>
    internal class CL311_RequestSetMeterParaPacket : Cl311SendPacket
    {
        private int m_MeterConst;
        private int m_PulseCount;
        private byte m_Lx;
        private byte m_Clfs;

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="meterconst">被检表常数</param>
        /// <param name="pulsecount">脉冲个数</param>
        /// <param name="lx"></param>
        /// <param name="clfs"></param>
        public void SetPara(int meterconst, int pulsecount, byte lx, byte clfs)
        {
            m_MeterConst = meterconst;
            m_PulseCount = pulsecount;
            m_Lx = lx;
            m_Clfs = clfs;
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetMeterParaPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x00);////62指令的类型 ，0=设置参数，1=启动，2=读数据
            buf.PutInt_S(m_MeterConst);
            buf.PutInt_S(m_PulseCount);
            buf.Put(m_Lx);
            buf.Put(m_Clfs);
            return buf.ToByteArray();
        }
    }
}
