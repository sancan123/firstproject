using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置标准表常数0x31
    /// </summary>
    internal class CL188_RequestSetStdMeterConstPacket :Cl188SendPacket
    {

        private int m_stdMeterConst = 0;
        private ushort m_fenpingxishu = 1;

        public CL188_RequestSetStdMeterConstPacket():base(false)
        {
        }

        /// <summary>
        /// 设置参数
        /// 0<分频系数<7fffH,最高位Bit15用于表示是否使用互感器：Bit15=0：未使用  Bit15=1：使用
        /// </summary>
        /// <param name="stdmeterconst">标准脉冲常数</param>
        /// <param name="fenpinxishu"> 分频系数</param>
        public void SetPara(int stdmeterconst, ushort fenpinxishu)
        {
            //m_stdMeterConst = stdmeterconst * 100;
            m_stdMeterConst = stdmeterconst;
            m_fenpingxishu = fenpinxishu;
        }
        /// <summary>
        /// 设置标准表常数 
        /// </summary>
        /// <returns></returns>
        public override string GetPacketName()
        {
            return "CL188_RequestSetStdMeterConstPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x31);
            buf.PutInt(m_stdMeterConst);
            buf.PutUShort(m_fenpingxishu);
            return buf.ToByteArray();
        }
    }
}
