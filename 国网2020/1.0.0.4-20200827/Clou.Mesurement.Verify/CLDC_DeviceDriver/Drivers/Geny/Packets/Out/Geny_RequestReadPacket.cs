using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 所有读取数据的包的基类
    /// </summary>
    class Geny_RequestReadPacket : GenySendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        public Geny_RequestReadPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="cmdCode">控制码</param>
        public Geny_RequestReadPacket(byte meterIndex, byte cmdCode)
            : base(meterIndex, cmdCode)
        {
            this.MeterIndex = meterIndex;
        }


        protected override byte[] GetBody()
        {
            return new byte[0];
        }
    }
}
