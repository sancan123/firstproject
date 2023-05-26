using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 开始走字 数据包
    /// 返回指令
    /// </summary>
    internal class Geny_RequestZouZiPacket : GenySendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 走字脉冲数
        /// </summary>
        public double ImpluseCount
        {
            get;
            set;
        }

        /// <summary>
        /// 信号通道
        /// </summary>
        public byte SignalChannel
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

        public Geny_RequestZouZiPacket()
        { }

        public Geny_RequestZouZiPacket(byte meterIndex, double impluseCount, byte signalChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 0x1d)
        {
            this.MeterIndex = meterIndex;
            this.ImpluseCount = impluseCount;
            this.SignalChannel = signalChannel;
            this.ImpluseMode = impluseMode;
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>(6);

            buf.AddRange(DataFormart.Formart(ImpluseCount, 0, false));
            buf.Add((byte)(this.SignalChannel));
            buf.Add((byte)(this.ImpluseMode));

            return buf.ToArray();
        }
    }
}
