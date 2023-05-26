using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 指定误差板 电流 通断 指令
    /// </summary>
    class Geny_RequestrWCBCurrentOnOff : GenySendPacket
    {

        public bool IsOff
        {
            get;
            set;
        }

        public Geny_RequestrWCBCurrentOnOff(byte meterIndex, bool isOff)
            : base(meterIndex, 0x25)
        {
            this.IsOff = IsOff;
        }

        protected override byte[] GetBody()
        {
            byte[] buf = new byte[1];

            buf[0] = (byte)(this.IsOff ? 0 : 1);

            return buf;
        }
    }
}
