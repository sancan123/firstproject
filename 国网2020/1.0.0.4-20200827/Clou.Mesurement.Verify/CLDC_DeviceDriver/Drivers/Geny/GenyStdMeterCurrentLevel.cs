using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    /// <summary>
    /// geny 标准表电流档位
    /// </summary>
    public enum GenyStdMeterCurrentLevel : byte
    {
        /// <summary>
        /// 1 a
        /// </summary>
        Level_1A = 0,

        /// <summary>
        /// 10 a
        /// </summary>
        Level_10A = 1,

        /// <summary>
        /// 1000 a
        /// </summary>
        Level_100A = 2
    }
}
