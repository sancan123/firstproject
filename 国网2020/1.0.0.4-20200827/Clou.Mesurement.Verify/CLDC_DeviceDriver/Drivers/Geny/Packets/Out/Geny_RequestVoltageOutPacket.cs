using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;
using CLDC_DeviceDriver.Drivers.Geny.Packets;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 输出电压，电压为0时表示电压复位
    /// </summary>
    internal class Geny_RequestVoltageOut : GenySendPacket
    {

        /// <summary>
        /// 电压
        /// </summary>
        public double Voltage
        {
            get;
            set;
        }

        public Geny_RequestVoltageOut()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="voltage"></param>
        public Geny_RequestVoltageOut(PhaseType phase, double voltage)
            : base(GetDriverId(phase), 0x0b)
        {
            this.Voltage = voltage;
        }

        /// <summary>
        /// 已经重写，返回电压值数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return DataFormart.Formart(this.Voltage * 1000, 0, false);
        }
    }
}
