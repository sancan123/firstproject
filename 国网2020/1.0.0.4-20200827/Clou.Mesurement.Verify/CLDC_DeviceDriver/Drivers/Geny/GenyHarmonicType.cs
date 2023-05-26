using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    /// <summary>
    /// 谐波类型
    /// </summary>
    public enum GenyHarmonicType
    {
        /// <summary>
        /// 普通谐波
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 次谐波
        /// </summary>
        Second,

        /// <summary>
        /// 偶谐波
        /// </summary>
        Even,

        /// <summary>
        /// 奇次谐波
        /// </summary>
        Odd
    }

}
