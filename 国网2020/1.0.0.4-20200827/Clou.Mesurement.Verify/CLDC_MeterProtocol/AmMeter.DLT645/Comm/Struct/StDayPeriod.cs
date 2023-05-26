/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: StDayPeriod.cs
 * 文件功能描述: 时段数据结构体
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Struct
{
    /// <summary>
    /// 时段数据
    /// </summary>
    public struct StDayPeriod
    {
        /// <summary>
        /// 时段编号（范围：1-14）
        /// </summary>
        public byte DPeriodID;
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime;
        /// <summary>
        /// 费率编号（范围：1-63）
        /// </summary>
        public byte TariffID;
    }
}