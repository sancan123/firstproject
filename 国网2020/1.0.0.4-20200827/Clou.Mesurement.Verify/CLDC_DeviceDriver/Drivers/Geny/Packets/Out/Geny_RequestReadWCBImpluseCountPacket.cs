using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 读误差板 累计 脉冲数
    /// </summary>
    class Geny_RequestReadWCBImpluseCountPacket : Geny_RequestReadWCBDataPacket
    {
        public Geny_RequestReadWCBImpluseCountPacket(byte meterIndex)
            : base(meterIndex)
        {
            this.CmdCode = 0x24;
        }
    }
}
