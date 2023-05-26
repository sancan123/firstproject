using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny
{

    /// <summary>
    /// 读取 格林 标准表的数据类型
    /// </summary>
    public enum Geny_StandMeterDataType
    {
        /// <summary>
        /// 标准表常数
        /// </summary>
        Const,

        /// <summary>
        /// 标准表型号
        /// </summary>
        Type,

        /// <summary>
        /// 标准表测量数据
        /// </summary>
        Data,

        /// <summary>
        /// 设置标准表自动返回 测量数据
        /// </summary>
        data,

        /// <summary>
        /// 用来结束 Data 指令 的命令
        /// </summary>
        Non,

        /// <summary>
        /// A相谐波
        /// </summary>
        HA,

        /// <summary>
        /// B相谐波
        /// </summary>
        HB,

        /// <summary>
        /// C相谐波
        /// </summary>
        HC,

        /// <summary>
        /// 有功 
        /// </summary>
        Active,

        /// <summary>
        /// 无功
        /// </summary>
        Reactive,

        /// <summary>
        /// 谐波开始命令
        /// </summary>
        EStart,

        /// <summary>
        /// 谐波结束命令
        /// </summary>
        EEnd
    }
}
