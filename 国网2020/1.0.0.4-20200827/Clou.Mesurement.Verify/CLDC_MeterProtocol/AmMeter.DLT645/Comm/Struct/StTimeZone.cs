/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: StTimeZone.cs
 * 文件功能描述: 时区数据结构体
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
    /// 时区数据
    /// </summary>
    public struct StTimeZone
    {
        /// <summary>
        /// 时区编号（范围：1-14）
        /// </summary>
        public byte TZoneID;
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime StartDate;
        /// <summary>
        /// 日时段表编号（范围：1-8）
        /// </summary>
        public byte DayPeriodListID;
    }
}
