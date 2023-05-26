using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取误差板数据
    /// </summary>
    class Geny_RequestReadWCBDataPacket : Geny_RequestReadPacket
    {

        /// <summary>
        /// 表位号
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        public Geny_RequestReadWCBDataPacket()
        { }

        public Geny_RequestReadWCBDataPacket(byte meterIndex)
            : base(meterIndex, 0x0b)
        {
            this.MeterIndex = meterIndex;
        }
    }
}
