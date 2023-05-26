using System;

namespace CLDC_DataCore.Struct
{

    [Serializable()]
    public struct StDataFlagInfo
    {
        /// <summary>
        /// ���ݱ�ʶ����
        /// </summary>
        public string DataFlagName;
        /// <summary>
        /// ���ݱ�ʶ
        /// </summary>
        public string DataFlag;
        /// <summary>
        /// ���ݳ���
        /// </summary>
        public string DataLength;
        /// <summary>
        /// С��λ
        /// </summary>
        public string DataSmallNumber;        
        /// <summary>
        /// ���ݸ�ʽ
        /// </summary>
        public string DataFormat;        
    }
}
