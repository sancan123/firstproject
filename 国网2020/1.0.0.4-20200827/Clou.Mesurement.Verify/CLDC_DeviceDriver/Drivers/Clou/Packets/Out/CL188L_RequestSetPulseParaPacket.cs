using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置电能误差检定时脉冲参数
    /// </summary>
    internal class CL188L_RequestSetPulseParaPacket :Cl188LSendPacket
    {
        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xF1;

        /// <summary>
        /// 标准脉冲常数
        /// </summary>
        private int stdMeterConst = 0;

        /// <summary>
        /// 标准脉冲频率
        /// </summary>
        private int stdPulseFreq = 0;

        /// <summary>
        /// 标准脉冲常数缩放倍数
        /// </summary>
        private int stdMeterConstShortTime = 0;

        /// <summary>
        /// 被检脉冲常数
        /// </summary>
        private int meterConst = 0;

        /// <summary>
        /// 校验圈数
        /// </summary>
        private int meterQuans = 0;

        /// <summary>
        /// 被检脉冲常数缩放倍数
        /// </summary>
        private int meterConstShortTime = 0;

        /// <summary>
        /// 发送标志
        /// </summary>
        private byte sendFlag = 0xAA;

        public CL188L_RequestSetPulseParaPacket()
            :base(false)
        {}

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="stdmeterconst">标准脉冲常数</param>
        /// <param name="stdpulsefreq">标准脉冲频率</param>
        /// <param name="stdmeterconstshorttime">标准脉冲常数缩放倍数</param>
        /// <param name="meterconst">被检脉冲常数</param>
        /// <param name="meterquans">校验圈数</param>
        /// <param name="meterconstshorttime">被检脉冲常数缩放倍数</param>
        public void SetPara(bool[] bwstatus, int stdmeterconst, int stdpulsefreq, int stdmeterconstshorttime, int meterconst, int meterquans, int meterconstshorttime)
        {            
            stdMeterConst = stdmeterconst;
            stdPulseFreq = stdpulsefreq;
            stdMeterConstShortTime = stdmeterconstshorttime;
            meterConst = meterconst;
            meterQuans = meterquans;
            meterConstShortTime = meterconstshorttime;
            this.Pos = 0;
            this.BwStatus = bwstatus;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSetPulseParaPacket";
        }
        public override string GetPacketResolving()
        {
            string resolve = string.Format("设置电能误差检定时脉冲参数->标准脉冲常数:{0}+ 标准脉冲频率:{1}+ 标准脉冲常数缩放倍数:{2}+ 被检脉冲常数:{3} + 校验圈数:{4}+ 被检脉冲常数缩放倍数:{5}+发送标志:{6}",
                stdMeterConst,
                stdPulseFreq,
                stdMeterConstShortTime,
                meterConst,
                meterQuans,
                meterConstShortTime,
                sendFlag
                );
            return resolve;
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 标准脉冲常数（4Bytes）+ 
         * 标准脉冲频率（4Bytes）+ 标准脉冲常数缩放倍数（1Bytes）+ 被检脉冲常数（4Bytes） + 校验圈数（4Bytes）+ 被检脉冲常数缩放倍数(1Byte)+发送标志1（1Byte） 。
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
            buf.PutInt_S(stdMeterConst);
            buf.PutInt_S(stdPulseFreq);
            buf.Put(Convert.ToByte(stdMeterConstShortTime));
            buf.PutInt_S(meterConst);
            buf.PutInt_S(meterQuans);
            buf.Put(Convert.ToByte(meterConstShortTime));
            buf.Put(Convert.ToByte(sendFlag));
            return buf.ToByteArray();
        }
    }
}
