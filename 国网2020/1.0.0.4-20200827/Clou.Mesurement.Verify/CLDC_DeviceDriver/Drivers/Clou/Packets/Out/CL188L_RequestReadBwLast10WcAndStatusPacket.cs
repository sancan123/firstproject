using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取表位前10次各类型误差及当前各种状态
    /// </summary>
    internal class CL188L_RequestReadBwLast10WcAndStatusPacket : Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC3;

        /// <summary>
        /// 误差类型
        /// </summary>
        private int wcBoardQueryType ;

        /// <summary>
        /// 此指令与C0H指令的帧格式相同，区别为此指令要求误差板上报前10次误差及当前状态。
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="wcType">检定误差类型</param>
        public CL188L_RequestReadBwLast10WcAndStatusPacket(bool[] bwstatus, Cus_WuchaType wcType)
            : base(false)
        {
            this.BwStatus = bwstatus;
            this.wcBoardQueryType = (int)wcType;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadBwLast10WcAndStatusPacket";
        }
        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定误差类型（1Byte）。
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
