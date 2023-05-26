using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// CT电流档位选择控制
    /// </summary>
    internal class CL188L_RequestIChannelControlPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB5;

        /// <summary>
        /// 电流档位,01表示100A档位、02表示2A档位。
        /// </summary>
        private Cus_IChannelType iType;

        public CL188L_RequestIChannelControlPacket(bool[] bwstatus, Cus_IChannelType itype)
            : base(false)
        {
            this.BwStatus = bwstatus;
            this.iType = itype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestIChannelControlPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 档位选择（1Byte）
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
            buf.Put(Convert.ToByte((int)iType));
            return buf.ToByteArray();
        }
    }
}
