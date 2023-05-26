using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 基本误差 参数
    /// </summary>
    internal class Geny_RequestBasicErrorPacket : GenySendPacket
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
        /// 圈数 2
        /// </summary>
        public ushort CircleCount
        {
            get;
            set;
        }

        /// <summary>
        /// 标准预计数 4
        /// </summary>
        public int StandardPredcitValue
        {
            get;
            set;
        }

        /// <summary>
        /// 信号通道 1
        /// </summary>
        public byte SignalChannel
        {
            get;
            set;
        }

        /// <summary>
        /// 脉冲方式（1个byte：0是外部上拉；1是外部下拉；2是内部上拉；3是内部下拉；4是内部悬空）；
        /// </summary>
        public CLDC_Comm.Enum.Cus_GyGyType PulseType
        {
            get;
            set;
        }

        /// <summary>
        /// 误差上限 4 
        /// </summary>
        public double ErrorUpLimit
        {
            get;
            set;
        }

        /// <summary>
        /// 误差下限  4
        /// </summary>
        public double ErrorDownLimit
        {
            get;
            set;
        }

        /// <summary>
        /// 生成一个新实例
        /// </summary>
        public Geny_RequestBasicErrorPacket()
        {
            CmdCode = 0x16;
        }

        /// <summary>
        /// 是否是2为共阴，3为共阳
        /// </summary>
        public bool IsPulseType23 { get; set; }

        /// <summary>
        /// 生成一个新实例
        /// </summary>
        public Geny_RequestBasicErrorPacket(byte meterIndex, ushort circleCount, int standPredcitValue, byte signalChannel, CLDC_Comm.Enum.Cus_GyGyType pluseType, double errorUpLimit, double errorDownLimit)
            : base(meterIndex, 0x16)
        {
            this.MeterIndex = meterIndex;
            this.CircleCount = circleCount;
            this.StandardPredcitValue = standPredcitValue;
            this.SignalChannel = signalChannel;
            this.PulseType = pluseType;
            this.ErrorUpLimit = errorUpLimit;
            this.ErrorDownLimit = errorDownLimit;
        }

        /// <summary>
        /// 已重写，返回数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>(16);
            byte[] bTmp = null;

            //圈数 2 
            bTmp = DataFormart.Formart(this.CircleCount, false);
            buf.AddRange(bTmp);

            //标准预计数 4
            bTmp = DataFormart.Formart(this.StandardPredcitValue, 0, false);
            buf.AddRange(bTmp);

            //信号通道
            buf.Add((byte)this.SignalChannel);

            //脉冲方式
            byte ptype = (byte)PulseType;
            //if (IsPulseType23) ptype += 2;
            buf.Add(ptype);

            //误差上限
            bTmp = DataFormart.Formart(this.ErrorUpLimit * 10000, 0, false);
            buf.AddRange(bTmp);

            //误差下限
            bTmp = DataFormart.Formart(this.ErrorDownLimit * 10000, 0, false);
            buf.AddRange(bTmp);

            return buf.ToArray();
        }

    }
}
