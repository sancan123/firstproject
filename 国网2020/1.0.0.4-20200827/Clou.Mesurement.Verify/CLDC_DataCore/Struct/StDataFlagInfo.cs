using System;

namespace CLDC_DataCore.Struct
{

    [Serializable()]
    public struct StDataFlagInfo
    {
        /// <summary>
        /// 数据标识名称
        /// </summary>
        public string DataFlagName;
        /// <summary>
        /// 数据标识
        /// </summary>
        public string DataFlag;
        /// <summary>
        /// 数据长度
        /// </summary>
        public string DataLength;
        /// <summary>
        /// 小数位
        /// </summary>
        public string DataSmallNumber;        
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat;        
    }
}
