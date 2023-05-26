using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Const
{
    /// <summary>
    /// ���ϱ���
    /// </summary>
    public class FaultCode
    {
        /// <summary>
        /// ȱ�����ݣ������ֵ䣩
        /// </summary>
        public const int FC_LACKDATA = -1;
        /// <summary>
        /// δ�ṩ���ܽӿ�
        /// </summary>
        public const int FC_NOHAVENCRYPT = -2;
        /// <summary>
        /// ���ݼ���ʧ��
        /// </summary>
        public const int FC_ENCRYPTFAULT = -3;
        /// <summary>
        /// ȱ���������ݣ����ܱ�
        /// </summary>
        public const int FC_LACKREQUESTDATA = -11;
        /// <summary>
        /// δ��Ȩ���������
        /// </summary>
        public const int FC_UNNAUTHORIZED = -12;


    }
}
