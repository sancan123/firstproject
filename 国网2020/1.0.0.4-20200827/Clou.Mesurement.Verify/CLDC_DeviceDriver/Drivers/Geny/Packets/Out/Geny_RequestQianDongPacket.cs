using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 潜动请求包
    /// </summary>
    internal class Geny_RequestQianDongPacket : GenySendPacket
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
        /// 启动时间
        /// </summary>
        public int QianDongTime
        {
            get;
            set;
        }

        /// <summary>
        /// 启动脉冲
        /// </summary>
        public byte QianDongImpluse
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

        public Geny_RequestQianDongPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="qiDongTime"></param>
        /// <param name="qiDongImpluse"></param>
        /// <param name="signalChannel"></param>
        /// <param name="impluseMode"></param>
        public Geny_RequestQianDongPacket(byte meterIndex, int qianDongTime, byte qianDongImpluse, byte signalChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 0x18)
        {
            this.MeterIndex = meterIndex;
            this.QianDongTime = qianDongTime;
            this.QianDongImpluse = qianDongImpluse;
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

            buf[0] = (byte)(this.QianDongTime >> 8);
            buf[1] = (byte)(this.QianDongTime & 0xFF);

            buf[2] = this.QianDongImpluse;

            buf[3] = this.SignalChannel;

            buf[4] = (byte)(this.ImpluseMode);

            return buf;
        }
    }
}
