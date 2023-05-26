using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 选择被检脉冲通道及检定类型
    /// </summary>
    internal class CL188L_RequestSelectPulseChannelAndCheckTypePacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xA7;

        /// <summary>
        /// 电能误差通道号,0P+ 、1P-、 2Q+、 3Q-
        /// </summary>
        private Cus_MeterWcChannelNo wcChannelNo;

        /// <summary>
        /// 光电头选择位,1为感应式脉冲输入，0为电子式脉冲输入
        /// </summary>
        private Cus_PulseType pulseType;

        /// <summary>
        /// 脉冲极性选择(共阳/共阴),0表示公共端输出低电平（共阴），1表示公共端输出高电平（共阳）
        /// </summary>
        private Cus_GyGyType GyGy;

        /// <summary>
        /// 多功能误差通道号,1为日计时脉冲、2为需量脉冲。
        /// </summary>
        private Cus_DgnWcChannelNo dgnWcChannelNo;

        /// <summary>
        /// 检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。
        /// </summary>
        private Cus_CheckType checkType;

        public CL188L_RequestSelectPulseChannelAndCheckTypePacket()
            :base(false)
        {}


        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">电表状态</param>
        /// <param name="wcchannelno">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟</param>
        /// <param name="pulsetype">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="gygy">脉冲类型,0=共阳,1=共阴</param>
        /// <param name="dgnwcchannelno">多功能误差通道号,1=日计时，2=需量脉冲</param>
        /// <param name="checktype">检定类型</param>
        public void SetPara(bool[] bwstatus, Cus_MeterWcChannelNo wcchannelno, Cus_PulseType pulsetype,Cus_GyGyType gygy, Cus_DgnWcChannelNo dgnwcchannelno, Cus_CheckType checktype)
        {
            BwStatus = bwstatus;
            this.wcChannelNo = wcchannelno;
            this.pulseType = pulsetype;
            this.GyGy = gygy;
            this.dgnWcChannelNo = dgnwcchannelno;
            this.checkType = checktype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectPulseChannelAndCheckTypePacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 被检脉冲通道号（2Byte）+ 检定类型（1Byte）。
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
            //计算第一个字节
            string byte1 = Convert.ToString((int)wcChannelNo, 2).PadLeft(3,'0');
            byte1 = ((int)GyGy).ToString() + ((int)pulseType).ToString() + byte1;
            buf.Put(Str2ToByte(byte1));
            //计算第二个字节
            buf.Put(Convert.ToByte((int)dgnWcChannelNo));
            buf.Put(Convert.ToByte((int)checkType));
            return buf.ToByteArray();
        }
    }
}
