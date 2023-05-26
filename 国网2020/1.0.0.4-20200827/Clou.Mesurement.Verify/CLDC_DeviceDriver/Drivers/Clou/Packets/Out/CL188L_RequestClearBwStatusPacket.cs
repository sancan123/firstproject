using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 清除表位状态
    /// </summary>
    internal class CL188L_RequestClearBwStatusPacket : Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC2;

        /// <summary>
        /// 清除/不改变状态
        /// </summary>
        private int Clear=0;

        /// <summary>
        /// 清除表位状态
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">True为清除，False为不改变状态</param>
        public CL188L_RequestClearBwStatusPacket(bool[] bwstatus, bool isclear)
            : base(false)
        {
            this.BwStatus = bwstatus;
            this.Clear = isclear ? 1 : 0;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestClearBwStatusPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 状态类型（1Byte）+ 状态参数（12Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(Data1);
            buf.Put(0x0c);
            if (ChannelByte == null)
                return ChannelByte;
            else
                buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(Clear));
            return buf.ToByteArray();
        }

    }
}
