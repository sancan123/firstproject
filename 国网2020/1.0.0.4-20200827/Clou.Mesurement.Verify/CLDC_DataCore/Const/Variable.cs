using System;

namespace CLDC_DataCore.Const
{

    public class Variable
    {
        /// <summary>
        /// Grid���̬��ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Normal = System.Drawing.Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Grid���������ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Alter = System.Drawing.Color.FromArgb(235, 250, 235);

        /// <summary>
        /// �̶��У��У���ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_Frone = System.Drawing.Color.FromArgb(225, 225, 225);
        /// <summary>
        ///���ϸ���ɫ
        /// </summary>
        public static System.Drawing.Color Color_Grid_BuHeGe = System.Drawing.Color.Red;



        /// <summary>
        /// �ϸ��ı�����
        /// </summary>
        public const string CTG_HeGe = "�ϸ�";
        /// <summary>
        /// δ������ǰ��Ĭ����ʾ
        /// </summary>
        public const string CTG_DEFAULTRESULT = "--";
        /// <summary>
        /// ���ϸ��ı�����
        /// </summary>
        public const string CTG_BuHeGe = "���ϸ�";

        /// <summary>
        /// �ϸ��ǡ�
        /// </summary>
        public const string CMG_HeGe = "��";

        /// <summary>
        /// ���ϸ��־
        /// </summary>
        public const string CMG_BuHeGe = "��";
        /// <summary>
        /// �������Ͽ�������Ϣ��ʾ�ı�
        /// </summary>
        public const string CTG_SERVERUNCONNECT = "�������Ͽ�����";
        /// <summary>
        /// ��Ŀ�춨�����ʾ�ı�
        /// </summary>
        public const string CTG_VERIFYOVER = "������Ŀ�춨���";
        /// <summary>
        /// �����������ɹ��ı�
        /// </summary>
        public const string CTG_CONNECTSERVERSUCCESS = "�����������ɹ�";
        /// <summary>
        /// �����ı���ʶ
        /// </summary>
        public const string CTG_CONTROLMODEL_CONTROL = "����";
        /// <summary>
        /// �����ı���ʶ
        /// </summary>
        public const string CTG_CONTROLMODEL_BECONTROL = "����";
        /// <summary>
        /// û�г����Ĭ��ֵ
        /// </summary>
        public const float WUCHA_INVIADE = -999F;

        /// <summary>
        /// ̨������,1-����̨,0-����̨
        /// </summary>
        public const string CTC_DESKTYPE = "DESKTYPE";
        #region ----------------���춨�������-------------
        /// <summary>
        /// ÿ������ȡ�������������
        /// </summary>
        public const string CTC_WC_TIMES_BASICERROR = "TIMES_BASICERROR";
        /// <summary>
        /// ��׼ƫ��ȡ�������������
        /// </summary>
        public const string CTC_WC_TIMES_WINDAGE = "TIMES_WINDAGE";
        /// <summary>
        /// ÿ���������������
        /// </summary>
        public const string CTC_WC_MAXTIMES = "WC_MAXTIMES";
        /// <summary>
        /// ÿ�������춨ʱ��
        /// </summary>
        public const string CTC_WC_MAXSECONDS = "WC_MAXSECONDS";
        /// <summary>
        /// ������ж�
        /// </summary>
        public const string CTC_WC_JUMP = "WC_JUMP";
        /// <summary>
        /// IN����
        /// </summary>
        public const string CTC_WC_IN = "WC_IN";
        /// <summary>
        /// ƽ��ֵ����С��λ
        /// </summary>
        public const string CTC_WC_AVGPRECISION = "AVGPRECISION";
        /// <summary>
        /// ������ֵ��ļ�
        /// </summary>
        public const string CONST_WCLIMIT = "\\Const\\WcLimit.Mdb";

        /// <summary>
        /// ��׼�����Ƶϵ��������ǿ�½��׼����Ϊ1
        /// </summary>
        public const string CTC_DRIVERF = "DRIVERF";
        #endregion

        #region �๦�ܼ춨����
        /// <summary>
        /// �๦��Ӧ�ò㷢�����ݺ����ȴ�ʱ��
        /// </summary>
        public const string CTC_DGN_MAXWAITDATABACKTIME = "MAXWAITDATABACKTIME";
        /// <summary>
        /// �๦�ܼ춨Դ�ȶ�����ʱ��
        /// </summary>
        public const String CTC_DGN_POWERON_ATTERTIME = "POWERON_ATTERTIME";
        /// <summary>
        /// ��Ҫ�Ա����д����ʱ������ʾ
        /// </summary>
        public const string CTC_DGN_WRITEMETERALARM = "WRITEMETERALARM";
        /// <summary>
        /// �ռ�ʱ���춨����:����ģʽ|��׼ģʽ
        /// </summary>
        public const string CTC_DGN_RJSVERIFYTYPE = "RJSVERIFYTYPE";

        /// <summary>
        /// 
        /// </summary>
        public const string CTC_DGN_READDATAFROMRS485 = "READDATAFROMRS485";
        #endregion

        #region ��������ļ�

        /// <summary>
        /// �ز����������ļ�
        /// </summary>
        public const string CONST_CARRIER = "\\Const\\CarrierProtocol.xml";

        #endregion 

    }
}
