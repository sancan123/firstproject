using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 某相设置步骤请求包      
    /// </summary>
    internal class Geny_RequestFrequencyPacket : GenySendPacket
    {
        /// <summary>
        /// 相
        /// </summary>
        public PhaseType Phase
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或者设置频率
        /// </summary>
        public double Frequency
        {
            get;
            set;
        }

        public Geny_RequestFrequencyPacket()
            : this(PhaseType.A, 0x0D)
        {
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <param name="phase">相</param>
        /// <param name="frequency">频率</param>
        public Geny_RequestFrequencyPacket(PhaseType phase, double frequency)
            : base(GetDriverId(phase), 0x0d)
        {
            this.Phase = phase;
            this.Frequency = frequency;
        }

        /// <summary>
        /// 已重写，返回频率数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return DataFormart.Formart(this.Frequency * 1000, 0, false);
        }
    }
}
