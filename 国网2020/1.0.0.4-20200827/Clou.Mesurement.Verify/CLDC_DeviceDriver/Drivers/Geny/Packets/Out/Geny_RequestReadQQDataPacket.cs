using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 启动，潜动 数据读取包
    /// </summary>
    internal class Geny_RequestReadQQDataPacket : Geny_RequestReadPacket
    {
        public Geny_RequestReadQQDataPacket()
        { }

        public Geny_RequestReadQQDataPacket(byte meterIndex)
            : base(meterIndex, 0x0b)
        {
        }
    }
}
