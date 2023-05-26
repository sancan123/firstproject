using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 电表启动 包
    /// </summary>
    internal class Geny_RequestQiDongPacket : GenySendPacket
    {
        /// <summary>
        /// 电表位
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 整个启动过程所用的时间
        /// 以秒为单位
        /// </summary>
        public int QiDongTime
        {
            get;
            set;
        }

        /// <summary>
        /// 启动脉冲
        /// </summary>
        public byte QiDongImpluse
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

        public Geny_RequestQiDongPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="qiDongTime"></param>
        /// <param name="qiDongImpluse"></param>
        /// <param name="signalChannel"></param>
        /// <param name="impluseMode"></param>
        public Geny_RequestQiDongPacket(byte meterIndex, int qiDongTime, byte qiDongImpluse, byte signalChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 0x17)
        {
            this.MeterIndex = meterIndex;
            this.QiDongTime = qiDongTime;
            this.QiDongImpluse = qiDongImpluse;
            this.SignalChannel = signalChannel;
            this.ImpluseMode = impluseMode;
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            byte[] buf = new byte[5];

            buf[0] = (byte)(this.QiDongTime >> 8);
            buf[1] = (byte)(this.QiDongTime & 0xFF);

            buf[2] = this.QiDongImpluse;

            buf[3] = this.SignalChannel;

            buf[4] = (byte)(this.ImpluseMode);

            return buf;
        }
    }
}
