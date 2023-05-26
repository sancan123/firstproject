using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Struct
{
    /// <summary>
    /// ESAM模块运行信息文件
    /// </summary>
    public struct StEsamRunFile
    {
        /// <summary>
        /// 表号
        /// </summary>
        public string MeterNum;
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerNum;
        /// <summary>
        /// 用户类型
        /// </summary>
        public byte UserType;
        /// <summary>
        /// 剩余金额
        /// </summary>
        public float SpareMoney;
        /// <summary>
        /// 透支金额
        /// </summary>
        public float OverdrawMoney;
        /// <summary>
        /// 购电次数
        /// </summary>
        public int RechargeCount;
        /// <summary>
        /// 密钥状态
        /// </summary>
        public byte KeyState;
        /// <summary>
        /// 密钥更新方式
        /// </summary>
        public byte UpdateKeyWay;
        /// <summary>
        /// 密钥版本号
        /// </summary>
        public byte KeyVersion;

    }
}
