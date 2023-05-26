using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 脉幅 
    /// </summary>
    class Geny_RequestMaiFuPacket : GenySendPacket
    {

        /// <summary>
        /// 误差板通道
        /// </summary>
        public byte ImpluseChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 脉冲类型
        /// </summary>
        public CLDC_Comm.Enum.Cus_GyGyType ImpluseMode
        {
            get;
            set;
        }

        public Geny_RequestMaiFuPacket(byte deviceID, byte impluseChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(deviceID, 0x21)
        {
            this.ImpluseChannel = impluseChannel;
            this.ImpluseMode = impluseMode;
        }

        protected override byte[] GetBody()
        {
            return new byte[] { this.ImpluseChannel, (byte)this.ImpluseMode };
        }
    }
}
