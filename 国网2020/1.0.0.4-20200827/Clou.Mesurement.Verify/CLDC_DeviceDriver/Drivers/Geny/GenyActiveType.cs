using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    /// <summary>
    /// 有功无功类型
    /// </summary>
    public enum GenyActiveType : byte
    {

        /// <summary>
        /// 有功
        /// </summary>
        Active = 0,

        /// <summary>
        /// 无功
        /// </summary>
        Reactive = 1
    }
}
