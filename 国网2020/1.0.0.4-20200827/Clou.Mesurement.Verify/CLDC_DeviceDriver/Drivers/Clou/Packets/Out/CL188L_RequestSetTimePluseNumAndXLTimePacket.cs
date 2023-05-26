using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置日计时误差检定时钟频率及需量周期误差检定时间
    /// </summary>
    internal class CL188L_RequestSetTimePluseNumAndXLTimePacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xF3;

        /// <summary>
        /// 标准时钟频率100倍（4Bytes）
        /// </summary>
        private int stdMeterTimeFreq = 0;

        /// <summary>
        /// 被检时钟频率100倍
        /// </summary>
        private int meterTimeFreq = 0;

        /// <summary>
        /// 被检脉冲个数（4Bytes）
        /// </summary>
        private int meterPulseNum = 0;

        /// <summary>
        /// 发送标志
        /// </summary>
        private byte sendFlag = 0x55;

        public CL188L_RequestSetTimePluseNumAndXLTimePacket()
            :base(false)
        {}

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="stdmetertimefreq">标准时钟频率100倍（4Bytes）</param>
        /// <param name="metertimefreq">被检时钟频率100倍</param>
        /// <param name="meterpulsenum">被检脉冲个数(4Bytes)</param>
        public void SetPara(bool[] bwstatus, int stdmetertimefreq, int metertimefreq, int meterpulsenum)
        {
            IsNeedReturn = false;
            BwStatus = bwstatus;
            stdMeterTimeFreq = stdmetertimefreq * 100;
            meterTimeFreq = metertimefreq * 100;
            meterPulseNum = meterpulsenum;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSetPulseParaPacket";
        }

        /*
         *Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte） 
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

            buf.PutInt_S(stdMeterTimeFreq);
            buf.PutInt_S(meterTimeFreq);
            buf.PutInt_S(meterPulseNum);            
            buf.Put(Convert.ToByte(sendFlag));
            return buf.ToByteArray();
        }
    }
}
