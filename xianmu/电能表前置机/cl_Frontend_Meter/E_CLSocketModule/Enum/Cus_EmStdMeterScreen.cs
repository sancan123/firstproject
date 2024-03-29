﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace E_CLSocketModule.Enum
{
    /// <summary>
    /// 标准表界面指示
    /// </summary>
    public enum Cus_EmStdMeterScreen
    {
        /// <summary>
        /// 谐波柱图界面
        /// </summary>
        谐波柱图界面 = 0x09,
        /// <summary>
        /// 谐波列表界面
        /// </summary>
        谐波列表界面 = 0x0A,
        /// <summary>
        /// 波形界面
        /// </summary>
        波形界面 = 0x0B,
        /// <summary>
        /// 清除设置界面
        /// </summary>
        清除设置界面 = 0xFE
    }
}
