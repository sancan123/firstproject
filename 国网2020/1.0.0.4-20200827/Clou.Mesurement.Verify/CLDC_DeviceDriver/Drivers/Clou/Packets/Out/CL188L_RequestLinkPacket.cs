using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 188联机操作请求包
    /// </summary>
    internal class CL188L_RequestLinkPacket:Cl188LSendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xC0;

        public CL188L_RequestLinkPacket()
            : base(true)
        {
            this.Pos = 0;
        }

        public CL188L_RequestLinkPacket(bool[] bwstatus)
            : base(true)
        {
            this.Pos = 0;
            this.BwStatus = bwstatus;
        }

        public CL188L_RequestLinkPacket(bool[] bwstatus, int iChannelNo)
            : base(true)
        {
            this.Pos = 0;
            this.iChannelNo = iChannelNo;
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
            buf.Put(0x00);
            return buf.ToByteArray();
        }

    }
}
