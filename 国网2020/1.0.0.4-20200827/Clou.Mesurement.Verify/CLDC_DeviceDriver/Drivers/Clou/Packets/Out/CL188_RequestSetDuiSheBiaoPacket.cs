using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置/取消对标指令
    /// </summary>
    internal class CL188_RequestSetDuiSheBiaoPacket : Cl188SendPacket
    {
        private byte m_isDsb = 0x37;
        /// <summary>
        /// 表位
        /// </summary>
        public byte Pos = 0;


        public CL188_RequestSetDuiSheBiaoPacket()
            : base(false)
        {

        }
        /// <summary>
        /// 设置参数：是否是对标T:是;F:取消对标
        /// </summary>
        /// <param name="isDuiSheBiao">操作类型</param>
        public void SetPara(bool isDuiSheBiao)
        {
            m_isDsb = (byte)(isDuiSheBiao ? 0x37 : 0x38);
           // Pos = pos;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetDuiSheBiaoPacket";
        }

        protected override byte[] GetBody()
        {
            return new byte[] { this.m_isDsb };
        }
    }
}
