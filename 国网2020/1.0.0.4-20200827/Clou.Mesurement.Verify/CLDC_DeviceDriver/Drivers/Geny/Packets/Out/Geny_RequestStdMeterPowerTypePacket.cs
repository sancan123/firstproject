using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 设置电流表有功与无功
    /// </summary>
    class Geny_RequestStdMeterPowerTypePacket : Geny_RequestStdMeterPacket
    {

        /// <summary>
        /// 有功无功类型
        /// </summary>
        public GenyPowerType PowerType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="powerType"></param>
        public Geny_RequestStdMeterPowerTypePacket(string stdmeterType, GenyPowerType powerType)
            : base(stdmeterType)
        {
            this.PowerType = powerType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return Encoding.ASCII.GetBytes(this.PowerType.ToString().PadRight(7, ' '));
        }
    }
}
