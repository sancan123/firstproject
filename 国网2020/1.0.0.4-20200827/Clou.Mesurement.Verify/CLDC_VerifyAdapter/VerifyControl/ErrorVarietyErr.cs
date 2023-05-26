using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.SystemModel.Item; //�ֵ�
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using System.Data;
using CLDC_VerifyAdapter.VerifyService;


namespace CLDC_VerifyAdapter
{ 
    /// <summary>
    /// ���������
    /// </summary>
    class ErrorVarietyErr : VerifyBase
    {
        #region ----------��������----------

        /// <summary>
        /// ÿһ������ȡ�������������
        /// </summary>
        private int m_WCTimesPerPoint;
        /// <summary>
        /// ��׼ƫ���ȡ�������������
        /// </summary>
        private int m_WindageTimesPerPoint;
        /// <summary>
        /// ÿһ����������ȡ���ٴ����
        /// </summary>
        private int m_WCMaxTimes;
        /// <summary>
        /// ÿһ����������������
        /// </summary>
        private int m_WCMaxSeconds;
        /// <summary>
        /// �����ж���׼
        /// </summary>
        private float m_WCJump;

        /// <summary>
        ///��׼�����Ƶϵ��
        /// </summary>
        private float m_DriverF;
        string strPlan = "";
    
        string BC = "";
        string WCX = "";
        float xUa = 0;
        float xUb = 0;
        float xUc = 0;
        float xIa = 0;
        float xIb = 0;
        float xIc =  0;
        int Cs =  0;
        

        private bool InfluenceBefore = true;

        #endregion
        public ErrorVarietyErr(object plan)
            : base(plan) 
        {
            ResultNames = new string[] { "����ʱ��", "���(5minǰ1.0)", "���(5minǰ0.5L)", "���(5min��1.0)", "���(5min��0.5L)", "(1.0)���ֵ", "(0.5L)���ֵ", "����", "���ϸ�ԭ��" };
        }
        protected override string ItemKey
        {
            get
            {
                return CurPlan.PrjID;
            }
        }

        protected override string ResultKey
        {
            get { return null; }
        }
        private new StPlan_WcPoint CurPlan
        {
            get
            {
                return (StPlan_WcPoint)base.CurPlan;
            }
            set
            {
                base.CurPlan = value;
            }
        }
      

        public override void Verify()
        {

            #region ----------��������----------
            float meterLevel = 2;                                                           //��ȼ����� 
            MeterBasicInfo meterInfo;                                                       //�������
            StPlan_WcPoint curPoint = CurPlan;
            DateTime _thisPointStartTime = DateTime.Now;    //��¼�춨��ʼʱ��
            base.Verify();                                                                  //���û���ļ춨������ִ��Ĭ�ϲ���
            string[] arrCurWCValue = new string[BwCount];                                   //��ǰ���
            int[] arrCurWCTimes = new int[BwCount];                                         //������
            string dataValueKey = ItemKey;                                                  //�춨���ݽڵ�����:P_�춨ID
            int tableHeader = 2;// curPoint.Pc == 0 ? m_WCTimesPerPoint : m_WindageTimesPerPoint;  //ÿ����ϸ�������
            DataTable errorTable = new DataTable();                                         //���ֵ���
            StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[BwCount];                             //�������
            int[] arrPulseLap = new int[BwCount];                                           //�춨Ȧ��
            //DateTime thisPointStartTime = DateTime.Now;                                     //��¼�춨��ʼʱ��
            int[] arrVerifyTimes = new int[BwCount];                                        //��Ч������,��ǰ��λ�Ѿ����˶��ٴ���Ч���
            int[] arrCurrentWcNum = new int[BwCount];                                       //��ǰ�ۼƼ춨����,��ָ�����������ʼ��Ŀǰ�������˶��ٴ����
            int[] arrMeterWcNum = new int[BwCount];                                         //��λȡ������
            bool[] arrCheckOver = new bool[0];                                              //��λ��ɼ�¼
            //�ϱ��춨����
            string[] arrStrResultKey = new string[BwCount];
            object[] arrObjResultValue = new object[BwCount];
            MeterError tmpError = null;
            MeterError curError = null;
            string[] tmpErrorConc = new string[BwCount];
            string[] BeforeWc = new string[BwCount];
            string[] AfterWc = new string[BwCount];
            string[] FirWc1 = new string[BwCount];
            string[] FirWc5 = new string[BwCount];
            string[] SecWc1 = new string[BwCount];
            string[] SecWc5 = new string[BwCount];

            #endregion
            InitVerifyPara(tableHeader, ref arrPlanList, ref arrPulseLap, errorTable);
         
            /*��ǰ�Ƿ��Ѿ�ֹͣУ��*/
            if (Stop) return;

          
            for (int gg = 0; gg < 4; gg++)
            {

       
                #region Ӱ������
                errorTable = new DataTable();
            arrVerifyTimes = new int[BwCount];
            arrCurrentWcNum = new int[BwCount];
            arrMeterWcNum = new int[BwCount];
            tableHeader = 2;
            tmpError = null;
            curError = null;
            tmpErrorConc = new string[BwCount];
            arrPlanList = new StPlan_WcPoint[BwCount];
            arrCheckOver = new bool[0];   
            arrPulseLap = new int[BwCount];
        
            m_CheckOver = false;
            InitVerifyPara(tableHeader, ref arrPlanList, ref arrPulseLap, errorTable);
            InfluenceBefore = false;

            int     maxWCnum = tableHeader+1;                         //���������
            meterInfo = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);

            if (gg == 2)
            {
                PowerOn();
                ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 300);
            }
            _thisPointStartTime = DateTime.Now;     //��¼�¼춨��ʼʱ��
            Helper.LogHelper.Instance.WriteInfo("��ʼ���豸����...");
            //��ʼ���豸
          bool   resultInitEquip = InitEquipment(curPoint, float.Parse(meterInfo.Mb_chrHz), arrPulseLap,InfluenceBefore, gg);
            if (!resultInitEquip)
            {
                MessageController.Instance.AddMessage("��ʼ����������豸����ʧ��", 6, 2);
                //CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState = Cus_CheckStaute.����;
                //Stop = true;
                //return;
            }
            if (Stop)
            {
                return;
            }
            //      Helper.VerifyDemoHelper.Instance.Reset();
            m_StartTime = DateTime.Now;                                             //��¼�¼춨��ʼʱ��
            arrCheckOver = new bool[BwCount];                                       //�Ƿ��Ѿ���ɱ��μ춨
       int[]     PulseClone = (int[])arrPulseLap.Clone();
            Number.PopDesc(ref PulseClone, true);                      //����
         int    sleepTime = PulseClone[0] * GetOneErrorTime();   //����һ����������Ҫ��ʱ�� ����
            m_WCMaxSeconds = (int)(sleepTime * 20 * maxWCnum / 1000F + 10);
            if (sleepTime > 10000)
            {
                sleepTime = 5000;
            }       //����ȴ�ʱ�������5���Ӷ�ȡһ�����
            Helper.LogHelper.Instance.WriteInfo("��ʼ�춨");
            //��ʼ�춨
            Common.Memset(ref arrCurrentWcNum, -1);
            Common.Memset(ref tmpErrorConc, Variable.CTG_DEFAULTRESULT);
            while (!m_CheckOver)
            {
                #region
                //MessageController.Instance.AddMessage("���ڼ춨...");
                if (Stop) break;

                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                {
                    break;
                }
                if (CLDC_DataCore.Function.DateTimes.DateDiff(_thisPointStartTime) > m_WCMaxSeconds &&
                   (GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.����) != Cus_CheckStaute.����)
                {
                    Helper.LogHelper.Instance.WriteWarm("��ǰ��춨�Ѿ��������춨ʱ��" + m_WCMaxSeconds + "�룡", null);
                    MessageController.Instance.AddMessage("��ǰ��춨�Ѿ��������춨ʱ��" + m_WCMaxSeconds + "�룡");
                    m_CheckOver = true;
                    break;
                }
                if ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.����) == Cus_CheckStaute.����)
                {
                    arrCheckOver = new bool[BwCount];
                }
                string[] arrLastWcValue = arrCurWCValue;
                int[] arrLastWcTimes = arrCurrentWcNum;

                arrCurWCValue = new string[BwCount];               //���³�ʼ���������
                arrCurrentWcNum = new int[BwCount];           //
                if (!GlobalUnit.IsDemo)
                    ShowWaitMessage("Ԥ����һ������{0}��������ϣ���ȴ�", sleepTime);
                else
                    ShowWaitMessage("Ԥ����һ������{0}��������ϣ���ȴ�", 3000);
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                {
                    break;
                }
                Helper.LogHelper.Instance.WriteInfo("��ʼ��ȡ��ǰ�춨���������");
                if (!ReadData(ref arrCurWCValue, ref arrCurrentWcNum, arrPlanList[GlobalUnit.FirstYaoJianMeter].ErrorShangXian))
                {
                    continue;
                }

                if (Stop) break;
                m_CheckOver = true;
                //��¼ÿ��������
                //arrStrResultKey = new string[BwCount];
                arrObjResultValue = new object[BwCount];
                //����ÿһ�μ춨����
                Helper.LogHelper.Instance.WriteInfo("��ʼ����춨����");
                List<string> listNotEnough = new List<string>();
                List<string> listOver = new List<string>();
                List<string> listNotPass = new List<string>();
                for (int i = 0; i < BwCount; i++)
                {
                    #region
                    if (Stop) break;
                    meterInfo = Helper.MeterDataHelper.Instance.Meter(i);      //�������Ϣ
                    if (!meterInfo.YaoJianYn)
                        arrCheckOver[i] = true;//�������

                    if (!arrCheckOver[i] && arrCurrentWcNum[i] > 0)//
                    {
                        #region ----------���ݺϷ��Լ��----------
                        /*
                        ������255�ε����
                        */
                        if (arrMeterWcNum[i] > 0 && arrCurrentWcNum[i] < arrMeterWcNum[i])
                        {
                            while (arrMeterWcNum[i] > arrCurrentWcNum[i])
                            {
                                arrCurrentWcNum[i] += 255;
                            }
                        }

                        //����������
                        if (arrMeterWcNum[i] < arrCurrentWcNum[i])
                        {
                            arrMeterWcNum[i] = arrCurrentWcNum[i];
                            arrVerifyTimes[i]++;  //��������������������
                        }
                        else
                        {
                            //���������λ��û�г���������Ӧ����ʾ
                            int[] arr_Copy = (int[])arrVerifyTimes.Clone();
                            float[] arr_OtherWcnum = ConvertArray.ConvertInt2Float(arr_Copy);
                            Number.PopDesc(ref arr_OtherWcnum, false);
                            if (arr_OtherWcnum[0] > maxWCnum * 2 && arrVerifyTimes[i] == 0)
                            {
                                MessageController.Instance.AddMessage(String.Format("��λ{0}û�м�⵽���,�������", i + 1), 6, 2);
                                ////ThreadManage.Sleep(3000); //ֹͣ3�룬���û�����ʾ��Ϣ
                            }
                            //������û�����ӣ���˴���������û�и���
                            if (arrVerifyTimes[i] < maxWCnum)
                                m_CheckOver = false;
                            continue;
                        }
                        if (arrCurrentWcNum[i] == 0 || arrCurrentWcNum[i] == 255)
                        {
                            m_CheckOver = false;
                            continue;            //�������λû�г�������һ��
                        }
                        #endregion

                        curPoint = arrPlanList[i];                              //��ǰ�춨����
                        tmpError = GetMeterErrorData(meterInfo, curPoint);      //��ȡ��ǰ�ڵ�����
                        meterLevel = MeterLevel(meterInfo);                   //��ǰ��ĵȼ�
                        if (arrVerifyTimes[i] > tableHeader)
                        {
                            //������,���һ�������������ǰ��
                            for (int dtPos = tableHeader - 1; dtPos > 0; dtPos--)
                            {
                                errorTable.Rows[i][dtPos] = errorTable.Rows[i][dtPos - 1];
                            }
                            errorTable.Rows[i][0] = arrCurWCValue[i];     //���һ�����ʼ�շ��ڵ�һλ
                        }
                        else
                        {
                            errorTable.Rows[i][arrVerifyTimes[i] - 1] = arrCurWCValue[i];
                        }
                        /*�������*/
                        float[] tpmWc = ConvertArray.ConvertObj2Float(errorTable.Rows[i].ItemArray);  //Datable�е������ת��
                        curError = CalculateMeterError(curPoint, meterLevel, tpmWc);
                        //tmpError.Me_chrWcJl = curError.Me_chrWcJl;
                        tmpErrorConc[i] = curError.Me_chrWcJl;
                        tmpError.Me_chrWcMore = curError.Me_chrWc;
                        //������
                        if (arrVerifyTimes[i] > 1)
                        {
                            string _PreWc = errorTable.Rows[i][1].ToString();
                            if (CheckJumpError(_PreWc, arrCurWCValue[i], meterLevel, m_WCJump))
                            {
                                arrCheckOver[i] = false;
                                //tmpError.Me_chrWcJl = Variable.CTG_BuHeGe;
                                tmpErrorConc[i] = Variable.CTG_BuHeGe;
                                if (arrVerifyTimes[i] > m_WCMaxTimes)
                                    arrCheckOver[i] = true;
                                else
                                {
                                    Helper.LogHelper.Instance.WriteInfo("��⵽" + string.Format("{0}", i + 1) + "�������ȡ�����м���");
                                    MessageController.Instance.AddMessage("��⵽" + string.Format("{0}", i + 1) + "�������ȡ�����м���");
                                    arrVerifyTimes[i] = 1;     //��λ�����������
                                    m_CheckOver = false;
                                }
                            }
                        }
                        arrStrResultKey[i] = dataValueKey;
                        arrObjResultValue[i] = tmpError;
                        /*
                         * ����Ƿ����б��ϸ�
                         * ����춨�����Ѿ�����ÿ����춨��������������˵��Ƿ��Ѿ��ϸ�������ϸ���
                         * �ȼ�⵱ǰ�������Ƿ��Ѿ��ﵽÿ����������������ǰ��춨ʱ���Ƿ��Ѿ�����
                         * Ԥ�����ʱ�䡣�ǣ�����Ϊ�˱��ϸ񡣷����������ǰ�������ؼ졣
                         * _VerifyTimes[i] > _MaxWCnum ��ʱ��ѵ�һ����������������
                         */
                        if (arrVerifyTimes[i] >= maxWCnum)
                        {
                            //if (tmpError.Me_chrWcJl != Variable.CTG_HeGe && !arrCheckOver[i])
                            if (tmpErrorConc[i] != Variable.CTG_HeGe && !arrCheckOver[i])
                            {
                                if (arrVerifyTimes[i] > m_WCMaxTimes)
                                {
                                    arrCheckOver[i] = true;
                                    listOver.Add((i + 1).ToString());
                                }
                            }
                            else
                                arrCheckOver[i] = true;
                        }
                        else
                        {
                            arrCheckOver[i] = false;
                            listNotEnough.Add((i + 1).ToString());
                            //m_CheckOver = false;
                        }
                    }

                    if (i == BwCount - 1)
                    {
                        for (int j = 0; j < BwCount; j++)
                        {
                            if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                            if (!arrCheckOver[j])
                            {
                                listNotPass.Add((j + 1).ToString());
                                m_CheckOver = false;
                                break;
                            }
                            else
                            {
                                //int i6 = 123;
                            }
                        }
                    }
                    #endregion
                }

                if (listNotPass.Count > 0)
                {
                    MessageController.Instance.AddMessage(string.Format("��{0}���û��ͨ��", string.Join(",", listNotPass.ToArray())));
                }
                if (listNotEnough.Count > 0)
                {
                    MessageController.Instance.AddMessage(string.Format("��{0}��λ��û�дﵽ�춨����", string.Join(",", listNotEnough.ToArray())));
                }
                if (listOver.Count > 0)
                {
                    MessageController.Instance.AddMessage(string.Format("��{0}��λ�������춨����", string.Join(",", listOver.ToArray())));
                }



          
                //��

                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        MeterBasicInfo meterTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i];
                        if ((!meterTemp.MeterErrors.ContainsKey(ItemKey)) || string.IsNullOrEmpty(meterTemp.MeterErrors[ItemKey].Me_chrWcMore))
                        { 
                            if (meterTemp.YaoJianYn == true)
                            {
                              //  ResultDictionary["����"][i] = Variable.CTG_BuHeGe;
                            }
                            continue;
                        }
                         string[] arrayTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore.Split('|');
                       
                        //"5minǰ1.0���","5minǰ0.5L���","5min��1.0���","5min��0.5L���","1.0���ֵ","0.5L���ֵ"
                        if (gg == 0)
                        {
                            ResultDictionary["���(5minǰ1.0)"][i] = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore;
                            if (arrayTemp.Length > 3)
                            {
                                FirWc1[i] = arrayTemp[2];
                            }
                            else
                            {
                                FirWc1[i] = "";
                            }
                        }
                        else if (gg == 1)
                        {
                            ResultDictionary["���(5minǰ0.5L)"][i] = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore;
                            if (arrayTemp.Length > 3)
                            {
                                FirWc5[i] = arrayTemp[2];
                            }
                            else
                            {
                                FirWc5[i] = "";
                            }
                        }
                        else if (gg == 2)
                        {
                            ResultDictionary["���(5min��1.0)"][i] = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore;
                            if (arrayTemp.Length > 3)
                            {
                                SecWc1[i] = arrayTemp[2];
                            }
                            else
                            {
                                SecWc1[i] = "";
                            }
                        }
                        else if (gg == 3)
                        {
                            ResultDictionary["���(5min��0.5L)"][i] = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore;
                            if (arrayTemp.Length > 3)
                            {
                                SecWc5[i] = arrayTemp[2];
                            }
                            else
                            {
                                SecWc5[i] = "";
                            }
                        }

                       
                     
                       
                      //  = tmpErrorConc[i]; //curError.Me_chrWcJl;
                    }
                  
                 
                }

                //����ǵ���״̬�򲻼���Ƿ�춨���
                if ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.����) == Cus_CheckStaute.����)
                {
                    m_CheckOver = false;
                }
                #endregion while����
            }

            //����ǰ�������         
        //    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����", ResultDictionary["����"]);
            errorTable = null;
            MessageController.Instance.AddMessage("ֹͣ����!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 0);

            if (gg == 0)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���(5minǰ1.0)", ResultDictionary["���(5minǰ1.0)"]);
            }
            if (gg == 1)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���(5minǰ0.5L)", ResultDictionary["���(5minǰ0.5L)"]);
            }
            if (gg == 2)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���(5min��1.0)", ResultDictionary["���(5min��1.0)"]);
            }
            if (gg == 3)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���(5min��0.5L)", ResultDictionary["���(5min��0.5L)"]);
            }



            #endregion



            }
           
            float[] BC1 = new float[BwCount];
            float[] BC5 = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string[] dj = Number.getDj(Helper.MeterDataHelper.Instance.Meter(i).Mb_chrBdj);
                if (FirWc1[i] == "" || FirWc5[i] == "" || SecWc1[i] == "" || SecWc5[i] == "")
                {
                    ResultDictionary["����"][i] = "���ϸ�";
                    ResultDictionary["���ϸ�ԭ��"][i] = "���������";
                }
                else if (Math.Abs(float.Parse(FirWc1[i])) > float.Parse(dj[0]) || Math.Abs(float.Parse(FirWc5[i])) > float.Parse(dj[0]) || Math.Abs(float.Parse(SecWc1[i])) > float.Parse(dj[0]) || Math.Abs(float.Parse(SecWc5[i])) > float.Parse(dj[0]))
                {
                    ResultDictionary["����"][i] = "���ϸ�";
                    ResultDictionary["���ϸ�ԭ��"][i] = "����";
                }
                else
                {
                    BC1[i] = Math.Abs(float.Parse(SecWc1[i]) - float.Parse(FirWc1[i]));
                    BC5[i] = Math.Abs(float.Parse(SecWc5[i]) - float.Parse(FirWc5[i]));
                    ResultDictionary["(1.0)���ֵ"][i] = BC1[i].ToString("0.0000");
                    ResultDictionary["(0.5L)���ֵ"][i] = BC5[i].ToString("0.0000");

                    if (BC1[i] > 0.2 || BC5[i] > 0.2)
                    {
                        ResultDictionary["����"][i] = "���ϸ�";
                        ResultDictionary["���ϸ�ԭ��"][i] = "����";
                    }
                    else
                    {
                        ResultDictionary["����"][i] = "�ϸ�";

                    }
                }
              
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "(1.0)���ֵ", ResultDictionary["(1.0)���ֵ"]);
            ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "(0.5L)���ֵ", ResultDictionary["(0.5L)���ֵ"]);
            ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����", ResultDictionary["����"]);
            ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���ϸ�ԭ��", ResultDictionary["���ϸ�ԭ��"]);

















        }

        #region ----------������ʼ��InitVerifyPara----------
        /// <summary>
        /// ��ʼ���춨������������ʼ�����������ʼ��������������ʼ���������
        /// </summary>
        /// <param name="tableHeader">��ͷ����</param>
        /// <param name="planList">�����б�</param>
        /// <param name="Pulselap">�춨Ȧ��</param>
        /// <param name="DT">���</param>
        private void InitVerifyPara(int tableHeader, ref StPlan_WcPoint[] planList, ref int[] Pulselap, DataTable DT)
        {
            //�ϱ����ݲ���
            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            planList = new StPlan_WcPoint[BwCount];
            Pulselap = new int[BwCount];
            MessageController.Instance.AddMessage("��ʼ��ʼ���춨����...");
            //��ʼ�����ͷ
            for (int i = 0; i < tableHeader; i++)
            {
                DT.Columns.Add("WC" + i.ToString());
            }
            //��������
            MeterBasicInfo _MeterInfo = null;
            string[] arrCurTypeBw = new string[0];
            Helper.MeterDataHelper.Instance.Init();
            for (int iType = 0; iType < Helper.MeterDataHelper.Instance.TypeCount; iType++)
            {
                //�ӵ��ܱ����ݹ�������ȡÿһ�ֹ���ͺŵĵ��ܱ�
                arrCurTypeBw = Helper.MeterDataHelper.Instance.MeterType(iType);
                int curFirstiType = 0;//��ǰ���͵ĵ�һ������
                for (int i = 0; i < arrCurTypeBw.Length; i++)
                {
                    if (!Number.IsIntNumber(arrCurTypeBw[i]))
                        continue;
                    //ȡ��ǰҪ��ı��
                    int curMeterNumber = int.Parse(arrCurTypeBw[i]);
                    //�õ���ǰ��Ļ�����Ϣ
                    _MeterInfo = Helper.MeterDataHelper.Instance.Meter(curMeterNumber);
                    if (_MeterInfo.MeterErrors.ContainsKey(ItemKey))
                        _MeterInfo.MeterErrors.Remove(ItemKey);
                    strResultKey[curMeterNumber] = ItemKey;
                    if (_MeterInfo.YaoJianYn)
                    {
                        planList[curMeterNumber] = CurPlan;
                        planList[curMeterNumber].SetLapCount(Helper.MeterDataHelper.Instance.MeterConstMin(),
                                         _MeterInfo.Mb_chrBcs,
                                         _MeterInfo.Mb_chrIb,
                                         GlobalUnit.g_CUS.DnbData.CzIb,
                                         GlobalUnit.g_CUS.DnbData.CzQs
                                         );
                        planList[curMeterNumber].SetWcx(GlobalUnit.g_CUS.DnbData.CzWcLimit,
                            _MeterInfo.GuiChengName,
                             _MeterInfo.Mb_chrBdj,
                            _MeterInfo.Mb_BlnHgq);
                        planList[curMeterNumber].ErrorShangXian *= GlobalUnit.g_CUS.DnbData.WcxUpPercent;
                        planList[curMeterNumber].ErrorXiaXian *= GlobalUnit.g_CUS.DnbData.WcxDownPercent;

                        Pulselap[curMeterNumber] = planList[curMeterNumber].LapCount;
                        curFirstiType = curMeterNumber;
                    }
                    else
                    {
                        //���춨������Ϊ��һ��Ҫ�춨��Ȧ�������ڷ���ͳһ�춨��������߼춨Ч��
                        Pulselap[curMeterNumber] = planList[curFirstiType].LapCount;
                    }

                }
            }
            //������䲻��ı�λ
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)             //����ط���������У����ٱ�λ���������У���
            {
                DT.Rows.Add(new string[(tableHeader - 1)]);
                //����в���ı���ֱ�����Ϊ��һ��Ҫ����Ȧ��
                if (Pulselap[i] == 0)
                    Pulselap[i] = Pulselap[GlobalUnit.FirstYaoJianMeter];
            }

            MessageController.Instance.AddMessage("��ʼ���춨�������! ");
        }
        #endregion

        #region ----------����/���춨�����Ƿ���ȷ:CheckPara
        /// <summary>
        /// ����/���춨�����Ƿ���ȷ
        /// </summary>
        protected override bool CheckPara()
        {
            StPlan_WcPoint st_Wc = new StPlan_WcPoint();
            st_Wc.PrjID = "111010700";
            st_Wc.Dif_Err_Flag = 0;
            GlobalUnit.g_CUS.DnbData.SetWcxPercent(0.2F, -0.2F);
            //st_Wc.ErrorShangXian = 1;//�����ֵ���ٸ��ݹ�̼���
            //st_Wc.ErrorXiaXian = -1;
            st_Wc.IsCheck = true;
            st_Wc.LapCount = 2;
            GlobalUnit.g_CUS.DnbData.SetCzQsIb(2, "Ib");
            st_Wc.nCheckOrder = 1;
            st_Wc.Pc = 0;
            st_Wc.PointId = 1;
            st_Wc.PowerDianLiu = "10Itr";
            st_Wc.PowerFangXiang = Cus_PowerFangXiang.�����й�;
            st_Wc.PowerYinSu = "1.0";
            #region ����Ԫ��
            st_Wc.PowerYuanJian = Cus_PowerYuanJian.H;

            //switch (arrayErrorPara[2])
            //{
            //    case "H":
            //        st_Wc.PowerYuanJian = Cus_PowerYuanJian.H;
            //        break;
            //    case "A":
            //        st_Wc.PowerYuanJian = Cus_PowerYuanJian.A;
            //        break;
            //    case "B":
            //        st_Wc.PowerYuanJian = Cus_PowerYuanJian.B;
            //        break;
            //    case "C":
            //        st_Wc.PowerYuanJian = Cus_PowerYuanJian.C;
            //        break;
            //    default:
            //        st_Wc.PowerYuanJian = Cus_PowerYuanJian.H;
            //        break;
            //}
            #endregion

            st_Wc.PrjName = "";
            st_Wc.XiangXu = 0;
            st_Wc.XieBo = 0;
            CurPlan = st_Wc;

            //ÿһ������ȡ�������������
            m_WCTimesPerPoint = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_BASICERROR, 2);
            // ��׼ƫ���ȡ�������������
            m_WindageTimesPerPoint = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_WINDAGE, 5);
            if (m_WindageTimesPerPoint % 5 != 0 || m_WindageTimesPerPoint < 5)
            {
                string strinfo = string.Format("��⵽��ǰ����ֵΪ:{0},���ݹ��Ҫ�󣬱�׼ƫ��Ӧ��Ϊ5����������ϵͳ�Զ����Ƶ���Ϊ5\r\n����˶Ի����ٴγ����뵽ϵͳ�˵����޸ı�׼ƫ�����Ϊ5��������", m_WindageTimesPerPoint);
                m_WindageTimesPerPoint = 5;
                MessageController.Instance.AddMessage(strinfo, 6, 2);
            }
            /*ÿһ����������ȡ���ٴ����
         ���������С�ڼ������ʱ���޸�������
        */
            m_WCMaxTimes = GlobalUnit.GetConfig(Variable.CTC_WC_MAXTIMES, 2);
            if (m_WCMaxTimes < m_WCTimesPerPoint)
                m_WCMaxTimes = m_WCTimesPerPoint;
            m_WCMaxSeconds = GlobalUnit.GetConfig(Variable.CTC_WC_MAXSECONDS, 10);
            m_WCJump = GlobalUnit.GetConfig(Variable.CTC_WC_JUMP, 1F);
            //��׼�����Ƶϵ��
            m_DriverF = GlobalUnit.GetConfig(Variable.CTC_DRIVERF, 1F);
            return true;
        }
        #endregion
        #region ��ʼ���豸��������ʽ�汾��Ч[�������]:InitEquipment
        /// <summary>
        /// ��ʼ���豸����,����ÿһ�����Ҫ�춨��Ȧ��
        /// </summary>
        /// <param name="_curPoint"></param>
        /// <param name="PL"></param>
        /// <param name="_Pulselap"></param>
        /// <returns></returns>
        private bool InitEquipment(StPlan_WcPoint _curPoint, float PL, int[] _Pulselap ,bool before,int count)
        {
            float _xIb = Number.GetCurrentByIb("10Itr", Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb,Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);

            if (count == 0 || count == 2)
            {
                _curPoint.PowerYinSu = "1.0";
            }
            else
            {
                _curPoint.PowerYinSu = "0.5L";
            }

            if (GlobalUnit.IsDemo) return true;       
            MessageController.Instance.AddMessage("��ʼ����Դ���!");
         //   float _xIb = Number.GetCurrentByIb(_curPoint.PowerDianLiu, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb);
          
            bool result = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb, (int)_curPoint.PowerYuanJian, (int)_curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
            if (!result)
            {
                MessageController.Instance.AddMessage("����Դ���ʧ��", 6, 2);

            }
            if (Stop) return true;
            MessageController.Instance.AddMessage("����Դ����ɹ�");
            MessageController.Instance.AddMessage("��ʼ��ʼ���������춨����!");
            int[] meterconst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
            //System.Threading.Thread.Sleep(base.m_WaitTime_PowerOn);
            result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang, meterconst, _Pulselap);
            MessageController.Instance.AddMessage("��ʼ�����춨����" + GetResultString(result));
            if (!result)
            {
                result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang, meterconst, _Pulselap);
                if (!result)
                {
                    MessageController.Instance.AddMessage("��ʼ�����춨����ʧ��", 6, 2);
                    //return false;
                }
            }
            //System.Threading.Thread.Sleep(2000);
            return result;
        }
        #endregion
        /// <summary>
        /// �������ֵ
        /// </summary>
        /// <param name="curPoint">��ǰ�ڵ㷽��</param>
        /// <param name="meterLevel">��ȼ�</param>
        /// <param name="wcData">�������</param>
        /// <returns>���ṹ</returns>
        private MeterError CalculateMeterError(StPlan_WcPoint curPoint, float meterLevel, float[] wcData)
        {
            CLDC_DataCore.WuChaDeal.WuChaContext m_WuChaContext;
            CLDC_DataCore.Struct.StWuChaDeal wuChaPara = new CLDC_DataCore.Struct.StWuChaDeal();
            wuChaPara.MaxError = curPoint.ErrorShangXian;// * GlobalUnit.g_CUS.DnbData.WcxUpPercent;
            wuChaPara.MinError = curPoint.ErrorXiaXian;// * GlobalUnit.g_CUS.DnbData.WcxDownPercent;
            wuChaPara.MeterLevel = meterLevel;

            if (curPoint.Pc == 1)
                m_WuChaContext = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.��׼ƫ��, wuChaPara);
            else
                m_WuChaContext = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.�������, wuChaPara);

            //WuChaPara = _WuChaPara;
            return (MeterError)m_WuChaContext.GetResult(wcData);
        }
        /// <summary>
        /// �����һ�������Ҫ��ʱ��ms
        /// </summary>
        ///<remarks>
        ///������ڶ��ֳ����ĵ��ܱ��������ȳ�����ĵ��ܱ�Ϊ׼
        ///</remarks>
        /// <returns>��һ�������Ҫʱ�����ֵ,��λms</returns>
        private int GetOneErrorTime()
        {
            MeterBasicInfo firstMeter = Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter);
            if (firstMeter == null) return 1000;//Ĭ�ϰ�һ�봦��
            //���㵱ǰ���ع���
            float current = Number.GetCurrentByIb(CurPlan.PowerDianLiu, firstMeter.Mb_chrIb, firstMeter.Mb_BlnHgq);
            float currentPower = CalculatePower(GlobalUnit.U, current, GlobalUnit.Clfs, CurPlan.PowerYuanJian, CurPlan.PowerYinSu, IsYouGong);
            //����һ�ȴ���Ҫ��ʱ��,��λ����
            float needTime = 1000F / currentPower * 60F;
            return OnePulseNeedTime(IsYouGong, needTime);
        }
        /// <summary>
        /// ��ȡ��ǰ���ݽڵ㣬����������򴴽�һ���ڵ�
        /// </summary>
        /// <param name="meterInfo">�������Ϣ</param>
        /// <param name="curPoint">��ǰ����</param>
        /// <returns>���ݽڵ�</returns>
        private MeterError GetMeterErrorData(MeterBasicInfo meterInfo, StPlan_WcPoint curPoint)
        {
            MeterError tmpError;
            if (meterInfo.MeterErrors.ContainsKey(ItemKey))
            {
                tmpError = meterInfo.MeterErrors[ItemKey];
            }
            else
            {
                tmpError = new MeterError();
                tmpError.Me_chrGlys = curPoint.PowerYinSu;      //��������
                switch (curPoint.PowerFangXiang)
                {
                    case Cus_PowerFangXiang.�����й�:
                        tmpError.Me_Glfx = "�����й�";
                        break;
                    case Cus_PowerFangXiang.�����й�:
                        tmpError.Me_Glfx = "�����й�";
                        break;
                    case Cus_PowerFangXiang.�����޹�:
                        tmpError.Me_Glfx = "�����޹�";
                        break;
                    case Cus_PowerFangXiang.�����޹�:
                        tmpError.Me_Glfx = "�����޹�";
                        break;
                    case Cus_PowerFangXiang.��һ�����޹�:
                        tmpError.Me_Glfx = "��һ�����޹�";
                        break;
                    case Cus_PowerFangXiang.�ڶ������޹�:
                        tmpError.Me_Glfx = "�ڶ������޹�";
                        break;
                    case Cus_PowerFangXiang.���������޹�:
                        tmpError.Me_Glfx = "���������޹�";
                        break;
                    case Cus_PowerFangXiang.���������޹�:
                        tmpError.Me_Glfx = "���������޹�";
                        break;
                }
                switch (curPoint.PowerYuanJian)
                {
                    case Cus_PowerYuanJian.A:
                        tmpError.Me_intYj = 2;
                        break;
                    case Cus_PowerYuanJian.B:
                        tmpError.Me_intYj = 3;
                        break;
                    case Cus_PowerYuanJian.C:
                        tmpError.Me_intYj = 4;
                        break;
                    case Cus_PowerYuanJian.H:
                        tmpError.Me_intYj = 1;
                        break;
                    default:
                        tmpError.Me_intYj = 1;
                        break;
                }
                tmpError._intMyId = meterInfo._intMyId;  //Ψһ���
                //tmpError.Me_PL = meterInfo.Mb_chrHz;         //Ƶ��
                tmpError.Me_chrProjectNo = curPoint.PrjID;          //��Ŀ���
                //tmpError.Me_PrjName = curPoint.PrjName;      //��Ŀ����
                //tmpError.Me_Qs = curPoint.LapCount;          //Ȧ��
                tmpError.Me_WcLimit = String.Format("{0}|{1}" //���������
                    , Math.Round(curPoint.ErrorShangXian * GlobalUnit.g_CUS.DnbData.WcxUpPercent, 2),
                    curPoint.ErrorXiaXian * GlobalUnit.g_CUS.DnbData.WcxDownPercent);
                tmpError.Me_dblxIb = curPoint.PowerDianLiu;     //��������
                //tmpError.Me_xU = CLDC_DataCore.Const.GlobalUnit.U.ToString();           //��ѹ
                tmpError.Me_chrWcJl = Variable.CTG_DEFAULTRESULT;

                meterInfo.MeterErrors.Add(ItemKey, tmpError);
            }
            return tmpError;
        }


    }
}


