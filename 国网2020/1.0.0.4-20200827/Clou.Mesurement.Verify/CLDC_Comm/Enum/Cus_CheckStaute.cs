using System;

namespace CLDC_Comm.Enum
{

    [Flags]
    public enum Cus_CheckStaute
    {
        /// <summary>
        /// 
        /// </summary>
        未赋值的=0,
        /// <summary>
        /// 
        /// </summary>
        检定=1,
        /// <summary>
        /// 
        /// </summary>
        停止检定=2,
        /// <summary>
        /// 
        /// </summary>
        调表=4,
        /// <summary>
        /// 
        /// </summary>
        单步检定=8,
        /// <summary>
        /// 
        /// </summary>
        录入完成=16,
        /// <summary>
        /// 
        /// </summary>
        错误=32
    }
}
