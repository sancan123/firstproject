using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.In;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 谐波设置 参数
    /// </summary>
    internal class Geny_RequestVoltageHarmonicPacket : GenySendPacket
    {
        /// <summary>
        /// 电压值
        /// </summary>
        public float Value
        {
            get;
            set;
        }

        /// <summary>
        /// 谐波次数
        /// </summary>
        public int Times
        {
            get;
            set;
        }

        /// <summary>
        /// 相位角
        /// </summary>
        public double PhaseAngle
        {
            get;
            set;
        }

        /// <summary>
        /// a,b,c相
        /// </summary>
        public PhaseType Phase
        {
            get;
            set;
        }

        /// <summary>
        /// 谐波类型
        /// </summary>
        public GenyHarmonicType HarmonicType
        {
            get;
            set;
        }

        public Geny_RequestVoltageHarmonicPacket()
        { }

        /// <summary>
        /// 设置某相的谐波参数
        /// </summary>
        /// <param name="voltage">电压值</param>
        /// <param name="times">谐波次数</param>
        /// <param name="phase">a,b,c相的其中一个值</param>
        /// <param name="harmonicType">谐波类型</param>
        public Geny_RequestVoltageHarmonicPacket(float value, int times, PhaseType phase, GenyHarmonicType harmonicType, double phaseAngle)
        {
            this.SendID = GetDriverId(phase);
            this.CmdCode = 0x14;
            this.Value = value;
            this.Times = times;
            this.Phase = phase;
            this.HarmonicType = harmonicType;
            this.PhaseAngle = phaseAngle;
        }

        /// <summary>
        /// 已重写，返回谐波设计的数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            byte[] buf = new byte[7];
            //电压
            buf[0] = ((byte)(this.Value * 100 / 256));
            buf[1] = ((byte)((this.Value * 100) % 256));

            //谐波次数
            buf[2] = ((byte)(this.Times / 256));
            buf[3] = ((byte)(this.Times % 256));

            //相位
            buf[4] = ((byte)((int)this.PhaseAngle * 100 / 256));
            buf[5] = ((byte)((int)this.PhaseAngle * 100 % 256));

            //谐波类型
            buf[6] = ((byte)this.HarmonicType);
            return buf;
        }
    }
}
