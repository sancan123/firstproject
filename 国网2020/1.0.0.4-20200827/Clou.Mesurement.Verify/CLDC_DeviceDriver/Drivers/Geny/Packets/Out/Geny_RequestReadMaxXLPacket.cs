using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取最大需量 包
    /// </summary>
    class Geny_RequestReadMaxXLPacket : Geny_RequestReadPacket
    {

        public Geny_RequestReadMaxXLPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        public Geny_RequestReadMaxXLPacket(byte meterIndex)
            : base(meterIndex, 0x1a)
        {
        }
    }
}
