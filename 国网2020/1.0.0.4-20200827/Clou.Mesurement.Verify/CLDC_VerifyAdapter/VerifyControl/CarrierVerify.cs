using System;
using System.Collections.Generic;
using System.Text;

using CLDC_DataCore.Struct;
using CLDC_Comm;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;

using System.Windows.Forms;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// �����������ز�ͨѶ�춨
    /// ��    �ߣ�vs
    /// ��д���ڣ�2010-09-03
    /// �޸ļ�¼��
    ///         �޸�����		     �޸���	            �޸�����
    ///
    /// </summary>
    public class CarrierVerify : VerifyBase
    {
        #region--------------˽�б���-----------------
        private const Cus_MeterResultPrjID CST_CMP_CURITEMID = Cus_MeterResultPrjID.�ز�����;               //��ǰ�춨����
        private int[] m_int_CheckNums;                                                                      //�ٲ��������
        private int[] m_int_SuccessNums;                                                                    //�ɹ���������
        private int m_int_VerifyNum = 1;                                                                    //���ʹ���
        private double m_dbl_SuccessRate = 1.0;                                                             //�ϸ�ɹ���
        /// <summary>
        /// ����ֵ��10��16�Ƚ��ƣ����ֵ���ֵͨ
        /// </summary>
        private string[] str_RevD;
        private string[] str_RevDnumber;
        /// <summary>
        /// ����ֵ��10��16�Ƚ��ƣ����ֵ���ֵͨ  �ز�����
        /// </summary>
        private string[] str_RevD1;
        /// <summary>
        /// ����ֵ��10��16�Ƚ��ƣ����ֵ���ֵͨ  ���߳���
        /// </summary>
        private string[] str_RevD2;
        /// <summary>
        /// ת��������ַ���ASCII��
        /// </summary>
        private string[] str_RevDConvert;
        private static bool Is_SM = GlobalUnit.CarrierInfo.IsCheck_SM;
        #endregion------------------------------------

        #region--------------��������-----------------
        /// <summary>
        /// ��Ŀ��
        /// </summary>
        public string CodeName
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).str_Name;
            }
        }
        /// <summary>
        /// ��ʶ��
        /// </summary>
        public string Code
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).str_Code;
            }
        }

        /// <summary>
        /// �ɹ���
        /// </summary>
        public double SuccessRate
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).flt_Success;
            }
        }

        /// <summary>
        /// ���ʹ���
        /// </summary>
        public int VerifyNum
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).int_Times;
            }

            set
            {

            }
        }
        /// <summary>
        /// ģ�黥��
        /// </summary>
        public bool ModuleSwaps
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).b_ModuleSwaps;
            }
        }
        #endregion------------------------------------

        #region--------------���캯��-----------------
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public CarrierVerify(object plan)
            : base(plan)
        {
            ResultNames = new string[] { "�ز���Ŀ����", "��������",  "����" };
        }


        #endregion------------------------------------

        #region-------���෽����д --------
        /// <summary>
        /// �ܽ�������
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.�ز�����);
            }
        }

        /// <summary>
        /// ��д��������
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}{2}"                                          //Key:�μ����ݽṹ��Ƹ�2
                        , ResultKey
                        , Code
                        , ((StPlan_Carrier)CurPlan).str_Type);
            }
        }
        #endregion

        #region--------------��������-----------------
        /// <summary>
        /// �ز�����춨
        /// </summary>
        public override void Verify()
        {
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = true;

            if (GlobalUnit.CarrierInfo.CarrierName == "�е绪��2016")
            {
                GlobalUnit.Flag_IsZD2016 = true;
            }
            else
            {
                GlobalUnit.Flag_IsZD2016 = false;
            }
            string[] arrStrResultKey = new string[BwCount];
            //������춨��
            //����ʼ��������
            //����ʼ���豸��
            try
            {
                GlobalUnit.g_MsgControl.OutMessage("���ڽ����ز�����춨...");
                base.Verify();

                str_RevD = new string[BwCount];
                str_RevD2 = new string[BwCount];
                str_RevD1 = new string[BwCount];

                str_RevDnumber = new string[BwCount];
                str_RevDConvert = new string[BwCount];
                string[] strModel = new string[BwCount];

                //����Ϣ����
                //GlobalUnit.g_MsgControl.OutMessage(Cus_MessageType.�����Ϣ����);
                GlobalUnit.g_MsgControl.OutMessage("���ڽ����ز�����춨...", false);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.�����й�);//ֻ�����ѹ
                GlobalUnit.g_MsgControl.OutMessage("�ȴ���Դ�ȶ�...", false);
                Thread.Sleep(5000);
                if (Stop) return;

    
                //���ز��춨��
                CarrierCheck();


                Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.�����й�);//ֻ�����ѹ
                GlobalUnit.g_MsgControl.OutMessage("�ȴ���Դ�ȶ�...", false);
                Thread.Sleep(5000);

                if (GlobalUnit.CarrierInfo.CarrierName == "�е绪��2016")
                {
                    //�ָ������ϱ�ģʽ��
                    GlobalUnit.g_MsgControl.OutMessage("���ڻָ������ϱ�ģʽ��...", false);
                    MeterProtocolAdapter.Instance.WriteArrData("04001104", 8, strModel);
                }

                if (Stop) return;
                //����Ϣ����
                GlobalUnit.g_MsgControl.OutMessage("�ز�����춨��ϡ�", false);

                //����λ�������
           
            //    CLDC_DataCore.Const.GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_CARRIER_WAVE, (string[])arrStrResultKey);

            }
            catch (Exception e)
            {
                GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
          //      CLDC_DataCore.Function.LogInfoHelper.Write(e);
         //       CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = false;

            }
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = false;

        }

        /// <summary>
        /// ת��״̬�֣���0װ����1��1ת����0
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string[] ConvertStatus(string[] status)
        {
            string[] statusTmp = new string[status.Length];
            for (int i = 0; i < status.Length; i++)
            {
                if (!string.IsNullOrEmpty(status[i]))
                {
                    string strTmp = "";
                    for (int j = 0; j < status[i].Length / 2; j++)
                    {
                        strTmp += (255 - Convert.ToInt32(status[i].Substring(j * 2, 2), 16)).ToString("X2");
                    }
                    statusTmp[i] = strTmp;
                }
                else
                {
                    statusTmp[i] = status[i];
                }
            }
            return statusTmp;
        }


        #endregion------------------------------------

        #region--------------˽�к���-----------------
 /// <summary>
 /// �ز��춨
 /// </summary>
 /// <returns></returns>
        private bool CarrierCheck()
        {




            SwitchCarrierOr485(Cus_CommunType.ͨѶ�ز�);
            MeterBasicInfo curMeter;

          //  if (ModuleSwaps) m_int_VerifyNum = 2;
           // for (int intCheckTime = 0; intCheckTime < m_int_VerifyNum; intCheckTime++)
           // {
                //if (intCheckTime > 0)
                //{
                //    if (MessageBox.Show("�뻥���ز�ģ������"
                //                , "ȷ����ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                //    {
                //        return false;
                //    }
                //}
                for (int iBw = 0; iBw < BwCount; iBw++)
                {
                    GlobalUnit.Carrier_Cur_BwIndex = iBw;
                    if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                    {
                        break;
                    }
                    //����ȡָ����λ�����Ϣ��
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);
                    //���ж��Ƿ�Ҫ�졿
                    if (!curMeter.YaoJianYn)
                    {
                        continue;
                    }
                    //GlobalUnit.g_MsgControl.OutMessage("������ڵ��ַ", false);
                    //Helper.EquipHelper.Instance.AddMainNode(iBw);
                    //Thread.Sleep(500);


                    GlobalUnit.g_MsgControl.OutMessage("�����ز������" + (iBw + 1) + "��λ...", false);
                    float flt_RevD = -1;
               //     StDataFlagInfo dt = new StDataFlagInfo();
                    float flt_RevD2 = -1;
                    try
                    {
                        #region


                   //     dt = GlobalUnit.g_SystemConfig.DataFlagInfo.GetDataFlagInfo(CodeName);
                        if (Stop) return false;
                        for (int i = 0; i < 3; i++)
                        {
                            if (Stop) return false;
                            //�����ٲ����ݡ�
                            Thread.Sleep(1000);
                        //    if (CodeName.IndexOf("��") != -1 || int.Parse(dt.DataSmallNumber) > 0)
                          //  {
                                flt_RevD = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);

                              //  if (ModuleSwaps)//�ж��Ƿ�ģ�黥��
                              //  {
                              //      str_RevDnumber[iBw] = str_RevDnumber[iBw] + flt_RevD.ToString() + ",";
                             //       str_RevD[iBw] = str_RevD[iBw] + "��" + (intCheckTime + 1).ToString() + "�������ݣ�" + flt_RevD.ToString() + "  ";

                           //     }
                            //    else
                            //    {
                                    str_RevD[iBw] = flt_RevD.ToString();

                              //  }
                                if (GlobalUnit.CarrierInfo.IsCheck_SM)
                                {
                                    CLDC_DataCore.Const.GlobalUnit.boolIsWxOrZB = true;
                                    flt_RevD2 = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);
                                    str_RevD2[iBw] = flt_RevD2.ToString();
                                    str_RevD1[iBw] = flt_RevD.ToString();

                                    str_RevD[iBw] = "�ز�����:" + str_RevD1[iBw] + "|" + "���߳���:" + str_RevD2[iBw];
                                }
                           // }
                            //else
                            //{

                            //    flt_RevD = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);
                            //    if (GlobalUnit.CarrierInfo.IsCheck_SM)
                            //    {
                            //        flt_RevD = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);

                            //        str_RevD[iBw] = flt_RevD.ToString();
                            //    }
                            //    int temp_int = -1;
                            //    try
                            //    {
                            //        temp_int = (int)flt_RevD;

                            //    }
                            //    catch (Exception exint)
                            //    {

                            //    }
                            //    //for(int i=Convert.ToInt16( flt_RevD);)
                            //    if (ModuleSwaps)
                            //    {
                            //        str_RevDnumber[iBw] = str_RevDnumber[iBw] + Convert.ToString(temp_int) + ",";
                            //        str_RevD[iBw] = str_RevD[iBw] + "��" + (intCheckTime + 1).ToString() + "�������ݣ�" + Convert.ToString(temp_int) + "  ";

                            //    }
                            //    else
                            //    {
                            //        str_RevD[iBw] = flt_RevD.ToString();

                            //    }

                            //    // str_RevD[iBw] = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadDataByPos(Code, int.Parse(dt.DataLength), iBw);
                            //}

                            if (str_RevD[iBw] != null && str_RevD[iBw] != "" && str_RevD[iBw] != "-1")
                            {
                                break;
                            }

                            GlobalUnit.g_MsgControl.OutMessage("�������Ե�" + (i + 1) + "��,��" + (iBw + 1) + "��λ�ز�����...", false);
                            if (Stop) return false;
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                     //   CLDC_DataCore.Function.LogInfoHelper.Write(e);
                    }

                   
                 
                    GlobalUnit.g_MsgControl.OutMessage();
                }
            
            SwitchCarrierOr485(Cus_CommunType.ͨѶ485);
            for (int Num = 0; Num < BwCount; Num++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn)
                {
                    continue;
                }
                 ResultDictionary["�ز���Ŀ����"][Num] = "��ǰ�����й��ܵ���";
                 ResultDictionary["��������"][Num] = str_RevD[Num];
                 if (string.IsNullOrEmpty(str_RevD[Num]) || str_RevD[Num] == "-1")
                 {
                     ResultDictionary["����"][Num] = Variable.CTG_BuHeGe; //��Ŀ���

                 }
                 else
                 {
                     ResultDictionary["����"][Num] = Variable.CTG_HeGe; 
                 }



            }

            UploadTestResult("�ز���Ŀ����");
            UploadTestResult("��������");
            UploadTestResult("����");
            return true;
        }
        #endregion------------------------------------
    }
}
