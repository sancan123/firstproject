using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 读取色标的 数据包
    /// </summary>
    internal class Geny_RequestReadSeBiaoPacket : Geny_RequestReadPacket
    {

        public Geny_RequestReadSeBiaoPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        public Geny_RequestReadSeBiaoPacket(byte meterIndex)
            :base(meterIndex,0x0b)
        {
        }
    }
}
