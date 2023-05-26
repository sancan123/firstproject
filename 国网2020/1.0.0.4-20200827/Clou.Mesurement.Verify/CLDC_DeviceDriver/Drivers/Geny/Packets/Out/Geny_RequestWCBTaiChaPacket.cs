using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 设置 台差
    /// </summary>
    class Geny_RequestWCBTaiChaPacket : GenySendPacket
    {
        /// <summary>
        /// 误差
        /// </summary>
        public double Error
        {
            get;
            set;
        }

        public Geny_RequestWCBTaiChaPacket(byte deviceID, double error)
            : base(deviceID, 0x0F)
        {
            this.Error = error;
        }

        protected override byte[] GetBody()
        {
            return DataFormart.Formart(this.Error, 5, false);
        }
    }
}
