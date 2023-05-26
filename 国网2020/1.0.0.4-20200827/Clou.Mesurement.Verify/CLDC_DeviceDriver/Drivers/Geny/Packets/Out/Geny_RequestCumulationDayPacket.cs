using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 日计时误差 包
    /// </summary>
    internal class Geny_RequestCumulationDayPacket : GenySendPacket
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
        /// 被检表频率
        /// </summary>
        public float MeterFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// 脉冲通道
        /// </summary>
        public byte ImpluesChannel
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


        public Geny_RequestCumulationDayPacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="meterFrequency">被检表频率</param>
        /// <param name="signalChannel"></param>
        /// <param name="impluseMode"></param>
        public Geny_RequestCumulationDayPacket(byte meterIndex, float meterFrequency, byte impluseChannel, CLDC_Comm.Enum.Cus_GyGyType impluseMode)
            : base(meterIndex, 0x1F)
        {
            this.MeterIndex = meterIndex;
            this.MeterFrequency = meterFrequency;
            this.ImpluseMode = impluseMode;
            this.ImpluesChannel = impluseChannel;
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            byte[] buf = new byte[16];

            SetArrayValue(buf);
            byte[] freBuf = DataFormart.FormartToASCIIByte((int)this.MeterFrequency);
            Array.Copy(freBuf, 0, buf, 0, freBuf.Length);

            buf[14] = this.ImpluesChannel;
            buf[15] = (byte)(this.ImpluseMode);

            return buf;
        }
    }
}
