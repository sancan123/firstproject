using System;

namespace CLDC_DataCore.Struct
{
    /// <summary>
    /// 电能表耐压测试相关参数的数据模型
    /// </summary>
    [Serializable()]
    public class StInsulationParam
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public StInsulationParam()
        {
            Voltage = 2000;
            Time = 60;
            Current = 1;
            CurrentDevice = 1;
            PowerOnTime = 8;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="voltage">耐压值</param>
        /// <param name="time">耐压时间</param>
        /// <param name="current">漏电流</param>
        public StInsulationParam(int voltage,int time,int current,int powerontime)
        {
            Voltage = voltage;
            Time = time;
            Current = current;
            PowerOnTime = powerontime;
        }

        /// <summary>
        /// 功耗项目ID
        /// </summary>
        public string InsulationPrjID
    {
        get
        {
            return String.Format("{0}{1}"                                          //Key:参见数据结构设计附2
                    , ((int)CLDC_Comm.Enum.Cus_MeterResultPrjID.工频耐压试验 ).ToString()
                    , (int)InsulationType);
        }
    }
        /// <summary>
        /// 耐压试验类型：
        /// </summary>
        public EnumInsulationType InsulationType { get; set; }

        /// <summary>
        /// 耐压测试时间
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 耐压测试电压
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// 表位漏电流(单位：毫安)
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// 耐压机总漏电流(单位：毫安)
        /// </summary>
        public int CurrentDevice { get; set; }
        /// <summary>
        ///耐压仪源升起时间
        /// </summary>
        public int PowerOnTime { get; set; }
        /// <summary>
        /// 耐压仪最高限制电压
        /// </summary>
        public int MaxVoltage { get; set; }

        /// <summary>
        /// 耐压类型枚举
        /// </summary>
        public enum EnumInsulationType
        {
            /// <summary>
            ///  电流对电流 IA-IB
            /// </summary>
             PostAB=3,
             /// <summary>
             /// 电流对电流 IB-IC
             /// </summary>
            PostBC=5,
            /// <summary>
            /// 电流对电流 IA-IC
            /// </summary>
            PostAC=4,
            /// <summary>
            /// 电压对电流
            /// </summary>
            VoltageCurrent=2,
            /// <summary>
            ///电压电流对地
            /// </summary>
            VCandGND=1,
            /// <summary>
            /// 电压电流对辅助端子
            /// </summary>
            VCandP=6,
        }

        /// <summary>
        /// 重写方法，获取耐压试验名
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            if (InsulationType == EnumInsulationType.VoltageCurrent)
                return "电压对电流";
            else if (InsulationType == EnumInsulationType.PostAB)
                return "电流对电流IA-IB";
            else if (InsulationType == EnumInsulationType.PostAC)
                return "电流对电流IA-IC";
            else if (InsulationType == EnumInsulationType.PostBC)
                return "电流对电流IB-IC";
            else if (InsulationType == EnumInsulationType.VCandP)
                return "电压电流对辅助端子";
            else
                return "电压电流对地";

        }
    }
}
