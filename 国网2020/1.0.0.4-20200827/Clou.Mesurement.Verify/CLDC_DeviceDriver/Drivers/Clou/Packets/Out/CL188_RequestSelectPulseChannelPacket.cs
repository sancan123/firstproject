using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 选择脉冲通道请求包[无回复]0x46
    /// </summary>
    internal class CL188_RequestSelectPulseChannelPacket :Cl188SendPacket
    {
        private byte m_Gygy = 0;
        private byte m_PulseType = 0;
        private byte m_PulseChannelID = 0;
        
        public CL188_RequestSelectPulseChannelPacket()
            :base(false)
        {
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="Pos">第N块误差板</param>
        /// <param name="GygyType">共阴共阳设置0:共阳,1:共阳</param>
        /// <param name="PulseType">脉冲通道口，0:1#;1:2#[光电头,脉冲盒]</param>
        /// <param name="PulseChannelID">脉冲通道号</param>
        public void SetPara(byte pos, byte GygyType, byte PulseType, byte PulseChannelID)
        {
          
            this.Pos = pos;
            m_Gygy = GygyType;
            m_PulseType = PulseType;
            m_PulseChannelID = PulseChannelID;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSelectPulseChannelPacket";
        }
        /// <summary>
        /// 说明：选择被检脉冲通道，Bit0、Bit1、Bit2表示通道号，如bi2bit1bit0=0自动设置为光电头，Bit4为0
        ///表示公共端输出高电平（共阳），Bit4为1输出低电平（共阴）Bit7=0:选择1号被检脉冲口；
        ///bit7=8:选择2号口。
        /// </summary>
        /// <param name="buf"></param>
        /// 
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x46);
            buf.Put(Pos);
            m_PulseChannelID = (byte)(m_PulseChannelID | (m_PulseType << 7) | (m_Gygy << 3));
            buf.Put(m_PulseChannelID);
            return buf.ToByteArray();
        }
    }
}
