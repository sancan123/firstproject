using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 控制与电表的 485开关
    /// </summary>
    internal class Geny_RequestRS485SwitchPacket : GenySendPacket
    {
        /// <summary>
        /// 表位
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 状态
        /// 1 开，0关
        /// </summary>
        public byte State
        {
            get;
            set;
        }

        public Geny_RequestRS485SwitchPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="state"> 1 开，0关</param>
        public Geny_RequestRS485SwitchPacket(byte meterIndex, byte state)
            : base(meterIndex, 0x0E)
        {
            this.MeterIndex = meterIndex;
            this.State = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return new byte[] { this.State };
        }
    }
}
