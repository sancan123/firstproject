using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 输出电流，当电流值为0时表示电流复位
    /// </summary>
    internal class Geny_RequestCurrentPacket : GenySendPacket
    {
        /// <summary>
        /// 相位
        /// </summary>
        public PhaseType Phase
        {
            get;
            set;
        }

        /// <summary>
        /// 电流
        /// </summary>
        public float Current
        {
            get;
            set;
        }

        public Geny_RequestCurrentPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="current">电流</param>
        public Geny_RequestCurrentPacket(PhaseType phase, float current)
            : base(0, 0xA)
        {
            this.SendID = GetDriverId(phase);
            Phase = phase;
            Current = current;
        }

        /// <summary>
        /// 已重写，返回设置的电流数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return DataFormart.Formart(this.Current * 1000, 0, false);
        }
    }
}
