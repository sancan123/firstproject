using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取相应误差板软件版本号
    /// </summary>
    internal class CL188L_RequestReadWcBoardVerPacket :Cl188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC4;

        public CL188L_RequestReadWcBoardVerPacket(bool[] bwstatus)
            : base(false)
        {
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestReadWcBoardVerPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
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
            return buf.ToByteArray();
        }
    }
}
