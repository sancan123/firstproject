using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 升源命令，包含一相电源的，电流，电压，相位和频率设置
    /// </summary>
    internal class Geny_RequestPowerOnPacket : GenySendPacket
    {

        /// <summary>
        /// 相位
        /// </summary>
        public PhaseType Phase
        {
            get;
            set;
        }

        /// <summary>
        /// 电流实际值
        /// </summary>
        public double Current
        {
            get;
            set;
        }

        /// <summary>
        /// 电压实际值
        /// </summary>
        public double Voltage
        {
            get;
            set;
        }

        /// <summary>
        /// 象位角有余弦值
        /// </summary>
        public double COS
        {
            get;
            set;
        }
        /// <summary>
        /// 频率值
        /// </summary>
        public double Frequency
        {
            get;
            set;
        }

        /// <summary>
        /// 对于，电压，电流，相伴，频率
        /// 等数据是否使用 3位小数，
        /// 否则将使用 5 位小数
        /// </summary>
        public bool UseFiveDigit
        {
            get;
            set;
        }

        public Geny_RequestPowerOnPacket()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phase">相位</param>
        /// <param name="current">电流</param>
        /// <param name="voltage">电压</param>
        /// <param name="frequency">频率</param>
        /// <param name="useThreeDigit">是否使用三位小数</param>
        public Geny_RequestPowerOnPacket(PhaseType phase, double current, double voltage, double cos, double frequency, bool useFiveDigit)
            : base(GetDriverId(phase), 0x0e)
        {
            this.Phase = phase;
            this.Current = current;
            this.Voltage = voltage;
            this.COS = cos;
            this.Frequency = frequency;
            this.UseFiveDigit = useFiveDigit;
        }

        //public void Weave(PhaseType xiang, float sng_xI, float sng_xU, float Phi, float sng_Freq)
        //{

        //    // 设备地址
        //    base.Frame_DriverId = GetDriverId(xiang);

        //    //功能码
        //    base.Frame_FCode = 0x0E;

        //    //电流值
        //    byte[] byCurrent = FormatValue(sng_xI);
        //    base.Frame_Data.Add(byCurrent[0]);
        //    base.Frame_Data.Add(byCurrent[1]);
        //    base.Frame_Data.Add(byCurrent[2]);
        //    base.Frame_Data.Add(byCurrent[3]);

        //    //电压值
        //    byte[] byVol = FormatValue(sng_xU);
        //    base.Frame_Data.Add(byVol[0]);
        //    base.Frame_Data.Add(byVol[1]);
        //    base.Frame_Data.Add(byVol[2]);
        //    base.Frame_Data.Add(byVol[3]);

        //    //相位角值
        //    byte[] byPhi = FormatValue(Phi);
        //    base.Frame_Data.Add(byPhi[0]);
        //    base.Frame_Data.Add(byPhi[1]);
        //    base.Frame_Data.Add(byPhi[2]);
        //    base.Frame_Data.Add(byPhi[3]);

        //    //频率值
        //    byte[] byFreq = FormatValue(sng_Freq);
        //    base.Frame_Data.Add(byFreq[0]);
        //    base.Frame_Data.Add(byFreq[1]);
        //    base.Frame_Data.Add(byFreq[2]);
        //    base.Frame_Data.Add(byFreq[3]);
        //}

        /// <summary>
        /// 已重写
        /// 返回，相应的值
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>(16);

            int digit = UseFiveDigit ? 5 : 3;

            buf.AddRange(DataFormart.Formart(this.Current, digit, false));
            buf.AddRange(DataFormart.Formart(this.Voltage, digit, false));
            buf.AddRange(DataFormart.Formart(this.COS, digit, false));
            buf.AddRange(DataFormart.Formart(this.Frequency, digit, false));

            return buf.ToArray();
        }
    }
}
