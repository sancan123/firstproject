using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取表位各类型误差及各种状态
    /// </summary>
    internal class CL188L_RequestReadBwWcAndStatusPacket : Cl188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC0;

        /// <summary>
        /// 误差类型
        /// </summary>
        private int wcBoardQueryType ;

        /// <summary>
        /// 查询误差板当前误差及当前状态指令,C0指令默认查询表位状态。注：此指令只返回当前误差值。
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="wcType">检定误差类型</param>
        public CL188L_RequestReadBwWcAndStatusPacket(bool[] bwstatus, Cus_WuchaType wcType)
            : base(true)
        {
            this.BwStatus = bwstatus;
            this.wcBoardQueryType = (int)wcType;
        }
        /// <summary>
        /// 查询误差板当前误差及当前状态指令,C0指令默认查询表位状态。注：此指令只返回当前误差值。
        /// </summary>
        /// <param name="wcType">检定误差类型</param>
        public CL188L_RequestReadBwWcAndStatusPacket(Cus_WuchaType wcType)
            : base(true)
        {
            this.wcBoardQueryType = (int)wcType;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadBwWcAndStatusPacket";
        }

        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）。
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(Data1);
            buf.Put(0x0C);
            if (ChannelByte == null)
                return ChannelByte;
            else
                buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(wcBoardQueryType));
            return buf.ToByteArray();
        }

    }
}
