using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 故障表位电压电流隔离控制、次级开路试验
    /// </summary>
    internal class CL188L_RequestSeperateBwControlPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xB4;

        /// <summary>
        /// 隔离/恢复
        /// </summary>
        private int Seperate=0;

        /// <summary>
        /// 隔离/恢复,需要发两次指令，先隔离需要隔离的表位，再恢复掉需要恢复的表位
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="seperate">True为隔离，False为恢复</param>
        public CL188L_RequestSeperateBwControlPacket()
            : base(false)
        {
            
        }
        public void SetPara(bool[] bwstatus, bool seperate)
        {
            this.Seperate = seperate ? 1 : 0;
            ChannelByte = SeperateBwToChannelByte(bwstatus, seperate);
        }
        public override string GetPacketName()
        {
            return "CL188L_RequestSeperateBwControlPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+隔离控制状态（1Byte）。
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
            buf.Put(Convert.ToByte(Seperate));
            return buf.ToByteArray();
        }

        /// <summary>
        /// 隔离故障表位,恢复正常表位
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <returns></returns>
        private byte[] SeperateBwToChannelByte(bool[] bwstatus, bool seperate)
        {
            bool bSendByte = false;
            BwNum = bwstatus.Length;
            byte[] ChannelByte = new byte[12];
            for (int i = 0; i < ChannelByte.Length; i++)
            {
                string hex2 = "";
                int tmp = iChannelNo * (BwNum / 5);
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (seperate)
                    {
                        if (j < tmp + BwNum / 5 && j >= tmp)
                            hex2 += bwstatus[j] ? "1" : "0";
                        else
                            hex2 += "0";
                    }
                    else
                    {
                        if (j < tmp + BwNum / 5 && j >= tmp)
                            hex2 += bwstatus[j] ? "0" : "1";
                        else
                            hex2 += "0";
                    }
                }
                ChannelByte[ChannelByte.Length - 1 - i] = Str2ToByte(hex2);
                if (ChannelByte[ChannelByte.Length - 1 - i] != 0)
                    bSendByte = true;
            }
            if (bSendByte)
                return ChannelByte;
            else
                return null;
        }
    }
}
