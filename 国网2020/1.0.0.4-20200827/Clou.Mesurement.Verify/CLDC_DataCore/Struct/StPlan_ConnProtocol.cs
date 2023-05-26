using System;

namespace CLDC_DataCore.Struct
{
    /// <summary>
    /// ���ݲ�������
    /// </summary>
    [Serializable()]
    public enum StMeterOperType
    {
        /// <summary>
        /// 
        /// </summary>
        ��=0,
        /// <summary>
        /// 
        /// </summary>
        д=1
    }

    /// <summary>
    /// ͨѶЭ��������
    /// </summary>
    [Serializable()]
    public struct StPlan_ConnProtocol
    {

        private string _PrjID;
        /// <summary>
        /// ��Ŀ���
        /// </summary>
        public string PrjID
        {
            get
            {
                return _PrjID;
            }
            set
            {
                _PrjID = value;
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        public string ConnProtocolItem;

        /// <summary>
        /// ��ʶ����
        /// </summary>
        public string ItemCode;

        /// <summary>
        /// ���ݳ���
        /// </summary>
        public int DataLen;

        /// <summary>
        /// С��λ����
        /// </summary>
        public int PointIndex;

        /// <summary>
        /// ���ݸ�ʽ
        /// </summary>
        public string StrDataType;

        /// <summary>
        /// ��������,��/д
        /// </summary>
        public StMeterOperType OperType;

        /// <summary>
        /// д������
        /// </summary>
        public string WriteContent;

        //add by wzs on  20191231
        /// <summary>
        /// ��׼ֵ
        /// </summary>
        public float BzValue;

        /// <summary>
        /// �Ƿ��г��
        /// </summary>
        public bool IsXieBo;

        /// <summary>
        /// г������
        /// </summary>
        public int XBcount;
        
        /// <summary>
        /// г������
        /// </summary>
        public float XBContent;

        /// <summary>
        /// г�����
        /// </summary>
        public float XBPhase;


        /// <summary>
        /// ���ݱ�ʶ����
        /// </summary>
        public string DiDesc;

        //end add
        /// <summary>
        /// ͨѶЭ����������Ŀ����
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ConnProtocolItem == null) ConnProtocolItem = "";
            return string.Format("ͨѶЭ�������飺({0}){1}", OperType == StMeterOperType.�� ? "��" : "д", ConnProtocolItem.ToString());
        }
    }
}
