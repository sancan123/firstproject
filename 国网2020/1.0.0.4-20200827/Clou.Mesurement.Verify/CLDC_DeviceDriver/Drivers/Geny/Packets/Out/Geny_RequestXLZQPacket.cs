using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 启动 需量周期 指令包 
    /// </summary>
    class Geny_RequestXLZQPacket : GenySendPacket
    {
        public int MeterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 一个需量周期的时间
        /// </summary>
        public int XLZQTime
        {
            get;
            set;
        }

        /// <summary>
        /// 脉冲通道
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

        public Geny_RequestXLZQPacket()
        { }

        public Geny_RequestXLZQPacket(byte meterIndex, int xlzqTime, byte impluseChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 27)
        {
            this.MeterIndex = meterIndex;
            this.XLZQTime = xlzqTime;
            this.ImpluseMode = impluseMode;
            this.ImpluseChannel = impluseChannel;
        }


        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>();

            buf.AddRange(DataFormart.Formart(this.XLZQTime * 60.0, 0, false));
            buf.Add(this.ImpluseChannel);
            buf.Add((byte)this.ImpluseMode);

            return buf.ToArray();
        }
    }
}
