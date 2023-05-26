using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    /// <summary>
    /// 通道类型
    /// </summary>
    public enum GenyDoorType
    {
        /// <summary>
        /// 误差计通道
        /// </summary>
        Door_Error = 1,

        /// <summary>
        /// 源通道
        /// </summary>
        Door_Power = 2,

        /// <summary>
        /// 485通道
        /// </summary>
        RS485 = 3,

        /// <summary>
        /// GPS通道
        /// </summary>
        Door_GPS = 4,

        /// <summary>
        /// 标准表
        /// </summary>
        StdMeter = 7

    }
}
