using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 启动计算功能指令
    /// </summary>
    internal class CL188L_RequestStartPCFunctionPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB1;

        /// <summary>
        /// 检定类型
        /// </summary>
        private Cus_CheckType checkType;


        /// <summary>
        /// 启动计算功能指令，若表位列表中某一位置1则启动对应表位检定，为0则不启动，若List = 0x30H，则只启动第5和第6表位的检定；检定类型设置同A7指令
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="checktype"></param>
        public CL188L_RequestStartPCFunctionPacket(bool[] bwstatus,Cus_CheckType checktype)
            : base(false)
        {
            this.Pos = 0;
            this.checkType = checktype;
            this.BwStatus = bwstatus;            
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /*
         * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
        */
        protected override byte[] GetBody()
        {
            int iType = (int)checkType;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(Data1);
            buf.Put(0x0C);
            if (ChannelByte == null)
                return ChannelByte;
            else
                buf.Put(ChannelByte);
            buf.Put(Convert.ToByte(iType));
            return buf.ToByteArray();
        }
    }
}
