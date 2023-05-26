using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Const
{
    /// <summary>
    /// 故障编码
    /// </summary>
    public class FaultCode
    {
        /// <summary>
        /// 缺少数据（数据字典）
        /// </summary>
        public const int FC_LACKDATA = -1;
        /// <summary>
        /// 未提供加密接口
        /// </summary>
        public const int FC_NOHAVENCRYPT = -2;
        /// <summary>
        /// 数据加密失败
        /// </summary>
        public const int FC_ENCRYPTFAULT = -3;
        /// <summary>
        /// 缺乏请求数据（电能表）
        /// </summary>
        public const int FC_LACKREQUESTDATA = -11;
        /// <summary>
        /// 未授权或密码错误
        /// </summary>
        public const int FC_UNNAUTHORIZED = -12;


    }
}
