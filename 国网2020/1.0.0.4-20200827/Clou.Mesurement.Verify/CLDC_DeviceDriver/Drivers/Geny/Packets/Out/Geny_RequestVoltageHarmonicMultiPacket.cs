using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置谐波 多重
    /// </summary>
    class Geny_RequestVoltageHarmonicMultiPacket : GenySendPacket
    {
        /// <summary>
        /// 谐波 含量
        /// </summary>
        Single[] Value
        {
            get;
            set;
        }

        /// <summary>
        /// 第几次 谐波次数
        /// </summary>
        public int[] Times
        {
            get;
            set;
        }

        /// <summary>
        /// 相位角
        /// </summary>
        public float[] PhaseAngle
        {
            get;
            set;
        }

        /// <summary>
        /// a,b,c相
        /// </summary>
        public PhaseType Phase
        {
            get;
            set;
        }

        public Geny_RequestVoltageHarmonicMultiPacket()
        { }

        public Geny_RequestVoltageHarmonicMultiPacket(/*float[] voltages,*/ Single[] value, int[] times, float[] phaseAngles, PhaseType phaseType)
            : base(GetDriverId(phaseType), 37)
        {
            this.Value = value;
            this.Times = times;
            this.PhaseAngle = phaseAngles;
            this.Phase = phaseType;
        }


        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            byte[] buf = new byte[84];

            //请注意，格林的VB代码似乎有错，因为其数据下标不对
            //此处下标从 0 开始，所以是正确的
            for (int i = 0; i < this.PhaseAngle.Length && i < 21; i++)
            {
                buf[i] = (byte)this.Value[i];
                buf[i + 21] = (byte)this.Times[i];
                buf[i * 2 + 41] = (byte)((this.PhaseAngle[i] * 10) / 256);
                buf[i * 2 + 42] = (byte)((this.PhaseAngle[i] * 10) % 256);
            }
            return buf;
        }
    }
}
