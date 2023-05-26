using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 对应表位 清标志位
    /// </summary>
    internal class Geny_RequestClearMarkPacket : GenySendPacket
    {
        public Geny_RequestClearMarkPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex">对应的表位号</param>
        public Geny_RequestClearMarkPacket(byte meterIndex)
            : base(meterIndex, 0x0A)
        {
        }

        /// <summary>
        /// 已重写
        /// 返回长度为0的数组
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return new byte[] { };
        }
    }
}
