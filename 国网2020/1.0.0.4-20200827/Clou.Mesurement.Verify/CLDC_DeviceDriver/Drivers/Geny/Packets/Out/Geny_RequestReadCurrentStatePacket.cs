using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 读取电流通断状态
    /// </summary>
    class Geny_RequestReadCurrentStatePacket : Geny_RequestReadPacket
    {

        public Geny_RequestReadCurrentStatePacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        public Geny_RequestReadCurrentStatePacket(byte meterIndex)
            : base(meterIndex, 0x26)
        {
        }
    }
}
