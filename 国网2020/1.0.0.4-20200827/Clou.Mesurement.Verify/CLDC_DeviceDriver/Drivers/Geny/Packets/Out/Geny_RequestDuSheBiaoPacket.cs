using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 对色标 包
    /// 该指令发给误差板
    /// </summary>
    internal class Geny_RequestDuSheBiaoPacket : GenySendPacket
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
        /// 信号通道
        /// </summary>
        public byte AccessNum
        {
            get;
            set;
        }

        /// <summary>
        /// 脉冲方式
        /// </summary>
        public CLDC_Comm.Enum.Cus_GyGyType ImpluseMode
        {
            get;
            set;
        }

        public Geny_RequestDuSheBiaoPacket()
        { }

        public Geny_RequestDuSheBiaoPacket(byte meterIndex, byte accessNum, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 0x1c)
        {
            this.MeterIndex = meterIndex;
            this.AccessNum = accessNum;
            this.ImpluseMode = impluseMode;
        }

        /// <summary>
        /// 已重写
        /// 返回，通道与脉冲方式
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            return new byte[] { this.AccessNum, (byte)this.ImpluseMode };
        }
    }
}
