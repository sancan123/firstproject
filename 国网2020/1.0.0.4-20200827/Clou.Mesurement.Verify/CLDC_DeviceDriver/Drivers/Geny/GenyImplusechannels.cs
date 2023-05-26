using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    /// <summary>
    /// 误差版（脉冲盒）通道类型
    /// </summary>
    public enum GenyImplusechannels
    {
        正向有功 = 1,
        反向有功 = 2,
        正向无功 = 3,
        反向无功 = 4,
        最大需量 = 6,
        日计时误差 = 7,
        需量周期误差 = 8
    }
}
