using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 序列号显示指令
    /// </summary>
    internal class CL188L_RequestShowSerialNoPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xA0;

        /// <summary>
        /// 序列号
        /// </summary>
        private int SerialNo = 0;

        /// <summary>
        /// 序列号显示指令
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">True为隔离，False为恢复</param>
        public CL188L_RequestShowSerialNoPacket()
            : base(false)
        {
            
        }
        public void SetPara(bool[] bwstatus, int intSerialNo)
        {
            this.SerialNo = intSerialNo;
            this.BwStatus = bwstatus;
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestShowSerialNoPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 要显示的系列号（4Bytes）。
        */
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
            buf.PutInt_S(SerialNo);
            return buf.ToByteArray();
        }

    }
}
