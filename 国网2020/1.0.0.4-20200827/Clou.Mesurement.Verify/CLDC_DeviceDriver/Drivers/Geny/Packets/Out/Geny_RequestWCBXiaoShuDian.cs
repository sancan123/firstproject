using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置误差板 小数位数
    /// </summary>
    class Geny_RequestWCBXiaoShuDian : GenySendPacket
    {

        public byte DigitNumber
        {
            get;
            set;
        }

        public Geny_RequestWCBXiaoShuDian(byte deviceID, byte digitNumber)
            : base(deviceID, 0x0d)
        {
            this.DigitNumber = digitNumber;
        }

        protected override byte[] GetBody()
        {
            return new byte[] { this.DigitNumber };
        }
    }
}
