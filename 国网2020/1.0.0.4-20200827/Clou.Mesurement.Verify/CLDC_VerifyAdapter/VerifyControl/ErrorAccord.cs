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
    /// ���һ���Լ춨������
    /// </summary>
    class ErrorAccord : VerifyBase
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

        #endregion

        #region ----------���캯��----------
        public ErrorAccord(object plan)
            : base(plan)
        {
            //���춨�����Ƿ���ȷ
            // m_VerifyDemo.BWCount = BwCount;
            ResultNames = new string[] { "���1","���2","ƽ��ֵ","����ֵ","��Ʒ��ֵ","��ֵ","����" };
        }
        #endregion

        #region ���෽����д

        /// <summary>
        /// ��д����
        /// </summary>
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

        /// <summary>
        /// �ܽ���ID
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                if (CurPlan.Pc == 1)
                    return string.Format("{0}", (int)Cus_MeterResultPrjID.��׼ƫ��);
                else
                    return string.Format("{0}", (int)Cus_MeterResultPrjID.�����������);
            }
        }
        /// <summary>
        /// �������Keyֵ:PrjID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return VerifyProcess.Instance.CurrentKey;
            }
        }

        #endregion

        /// <summary>
        /// �������ͱ�׼ƫ�����춨
        /// </summary>
        public override void Verify()
        {

            #region ----------��������----------
            GlobalUnit.g_CurTestType = 0;
            float meterLevel = 2;                                                           //��ȼ����� 
            MeterBasicInfo meterInfo;                                                       //�������
            StPlan_WcPoint curPoint = CurPlan;                                      //����ת��
            base.Verify();
            PowerOn();
            string[] verPlan = VerifyPara.Split('|');
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("��ʼԤ�ȣ���ȴ�" + TIME + "��");
                if (Stop) return;
              

                ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * TIME);

            }//���û���ļ춨������ִ��Ĭ�ϲ���
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
            #endregion

            Helper.LogHelper.Instance.WriteInfo("��ʼ���������춨����...");
            //��ʼ������,��200MS��ʱ
            InitVerifyPara(tableHeader, ref arrPlanList, ref arrPulseLap, errorTable);
            #region �ϴ�������
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
            //    {
            //        //"����Ԫ��","���ʷ���","��������","��������","�������","�������","���Ȧ��"
            //        ResultDictionary["����Ԫ��"][i] = arrPlanList[i].PowerYuanJian.ToString();
            //        ResultDictionary["���ʷ���"][i] = arrPlanList[i].PowerFangXiang.ToString();
            //        ResultDictionary["��������"][i] = arrPlanList[i].PowerDianLiu;
            //        ResultDictionary["��������"][i] = arrPlanList[i].PowerYinSu;
            //        ResultDictionary["�������"][i] = arrPlanList[i].ErrorXiaXian.ToString();
            //        ResultDictionary["�������"][i] = arrPlanList[i].ErrorShangXian.ToString();
            //        ResultDictionary["���Ȧ��"][i] = arrPlanList[i].LapCount.ToString();
            //    }
            //}
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����Ԫ��", ResultDictionary["����Ԫ��"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���ʷ���", ResultDictionary["���ʷ���"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "��������", ResultDictionary["��������"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "��������", ResultDictionary["��������"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "�������", ResultDictionary["�������"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "�������", ResultDictionary["�������"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���Ȧ��", ResultDictionary["���Ȧ��"]);
            #endregion
            /*��ǰ�Ƿ��Ѿ�ֹͣУ��*/
            if (Stop) return;
            
            int maxWCnum = tableHeader;                         //���������
            meterInfo = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);
            Helper.LogHelper.Instance.WriteInfo("��ʼ���豸����...");
            //��ʼ���豸
            bool resultInitEquip = InitEquipment(curPoint, float.Parse(meterInfo.Mb_chrHz), arrPulseLap);
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
            int[] PulseClone = (int[])arrPulseLap.Clone();
            Number.PopDesc(ref PulseClone, true);                      //����
            int sleepTime = PulseClone[0] * GetOneErrorTime();   //����һ����������Ҫ��ʱ�� ����
            m_WCMaxSeconds = (int)(sleepTime * 13 * maxWCnum / 1000F + 10);
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
                if (base.VerifyPassTime > m_WCMaxSeconds &&
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

                //if (listNotPass.Count > 0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("��{0}���û��ͨ��", string.Join(",", listNotPass.ToArray())));
                //}
                //if (listNotEnough.Count > 0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("��{0}��λ��û�дﵽ�춨����", string.Join(",", listNotEnough.ToArray())));
                //}
                //if (listOver.Count>0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("��{0}��λ�������춨����", string.Join(",", listOver.ToArray())));
                //}

                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo meterTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i];
                    if (!meterTemp.YaoJianYn) continue;
                    if ((!meterTemp.MeterErrors.ContainsKey(ItemKey)) || string.IsNullOrEmpty(meterTemp.MeterErrors[ItemKey].Me_chrWcMore))
                    {
                        if (meterTemp.YaoJianYn == true)
                        {
                            ResultDictionary["����"][i] = Variable.CTG_BuHeGe;
                        }
                        continue;
                    }
                    string[] arrayTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore.Split('|');
                    if (arrayTemp.Length > 3)
                    {
                        ResultDictionary["���1"][i] = arrayTemp[0];
                        ResultDictionary["���2"][i] = arrayTemp[1];
                        ResultDictionary["ƽ��ֵ"][i] = arrayTemp[2];
                        ResultDictionary["����ֵ"][i] = arrayTemp[3];
                    }
                    //ResultDictionary["����"][i] = tmpErrorConc[i]; //curError.Me_chrWcJl;
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���1", ResultDictionary["���1"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "���2", ResultDictionary["���2"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "ƽ��ֵ", ResultDictionary["ƽ��ֵ"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����ֵ", ResultDictionary["����ֵ"]);


                //����ǵ���״̬�򲻼���Ƿ�춨���
                //if ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.����) == Cus_CheckStaute.����)
                //{
                //    m_CheckOver = false;
                //}
                #endregion while����
            }

            for (int i = 0; i < BwCount; i++)
            {
                #region
                if (Stop) break;
                meterInfo = Helper.MeterDataHelper.Instance.Meter(i);      //�������Ϣ
                if (!meterInfo.YaoJianYn)
                {
                    arrCheckOver[i] = true;//�������
                }

                MeterError _EndError = GetMeterErrorData(meterInfo, curPoint);      //��ȡ��ǰ�ڵ�����
                _EndError.Me_chrWcJl = tmpErrorConc[i];

                #endregion
            }

            float[] arrAvgErr = new float[BwCount]; 

            //�������
            MessageController.Instance.AddMessage("���ڴ������,���Ժ�....");

            float AvgErr_YP =GetArryAvgErr(ResultDictionary["ƽ��ֵ"]);

            float Wcx = GetWcx(curPoint);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    string AvgErr1 = ResultDictionary["ƽ��ֵ"][i];
                    float VarietyErr = 100;

                    if (!string.IsNullOrEmpty(AvgErr1))
                    {
                        VarietyErr = Math.Abs(float.Parse(AvgErr1) - AvgErr_YP);
                        ResultDictionary["��ֵ"][i] = VarietyErr.ToString();
                    }


                    if (VarietyErr <= Wcx)
                    {
                        ResultDictionary["����"][i] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["����"][i] = Variable.CTG_BuHeGe;
                    }
                    ResultDictionary["��Ʒ��ֵ"][i] = AvgErr_YP.ToString();
                }
            }
            //֪ͨ����
            UploadTestResult("��Ʒ��ֵ");
            UploadTestResult("��ֵ");
            UploadTestResult("����");


            //����ǰ�������
            //ControlResult();
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "ƽ��ֵ", ResultDictionary["ƽ��ֵ"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����ֵ", ResultDictionary["����ֵ"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����", ResultDictionary["����"]);
            errorTable = null;
            MessageController.Instance.AddMessage("ֹͣ����!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 0);

        }

        #region ------------����---------------

        #region ���㣬���
        /// <summary>
        /// ��ȡ��ƽ����ƽ�����ġ���ֵ
        /// </summary>
        /// <returns></returns>
        private float GetErrLimit()
        {
            float value = 0f;
            string _Dj = GlobalUnit.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrBdj;
            float[] DJ = GetDJ(_Dj);//�й�������0���޹�������1
            if (CurPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹� || CurPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹�)
            {
                if (DJ[1] == 2)
                    value = 3f;
                else if (DJ[1] == 3)
                    value = 4f;

            }
            else if (CurPlan.PowerFangXiang == Cus_PowerFangXiang.�����й� || CurPlan.PowerFangXiang == Cus_PowerFangXiang.�����й�)
            {
                if (DJ[0] == 0.2f)
                    value = 0.3f;
                else if (DJ[0] == 0.5f)
                    value = 0.6f;
                else if (DJ[0] == 1f)
                    value = 2;
                else if (DJ[0] == 2f)
                    value = 3f;
            }
            return value;
        }
        /// <summary>
        /// ת�����ܱ�ȼ����ͣ����ַ���ת���ɸ����ͣ�
        /// </summary>
        /// <param name="DJ"></param>
        /// <returns></returns>
        private float[] GetDJ(string DJ)
        {
            float[] value = new float[] { 0f, 0f };
            DJ = DJ.Replace("s", "");
            DJ = DJ.Replace("S", "");
            if (!DJ.Contains("("))
            {
                float.TryParse(DJ, out value[0]);
            }
            else
            {
                string temp = DJ;

                temp = temp.Substring(0, DJ.IndexOf("("));

                float.TryParse(temp, out value[0]);
                temp = DJ;
                temp = temp.Substring(DJ.IndexOf("(") + 1);
                temp = temp.Replace(")", "");
                float.TryParse(temp, out value[1]);
            }
            return value;
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
            float current = Number.GetCurrentByIb(CurPlan.PowerDianLiu, firstMeter.Mb_chrIb,firstMeter.Mb_BlnHgq);
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
        /// ��ȡһ�����ݵ�ƽ��ֵ
        /// </summary>
        /// <param name="arryErr"></param>
        /// <returns></returns>
        private float GetArryAvgErr(string[] arryErr)
        {
            float RevArryAvgErr = 0;
            int k = 0;

            for (int i = 0; i < arryErr.Length; i++)
            {
                if (string.IsNullOrEmpty(arryErr[i])) continue;
                RevArryAvgErr += float.Parse(arryErr[i]);
                k++;
            }
            RevArryAvgErr = RevArryAvgErr / k;
            return RevArryAvgErr;
        }

        /// <summary>
        /// ������������������������
        /// </summary>
        /// <param name="curPoint"></param>
        /// <returns></returns>
        private float GetWcx(StPlan_WcPoint curPoint)
        {
            float Wcx = 100;
            if (curPoint.PowerDianLiu == "Ib" && curPoint.PowerYinSu == "1.0")
            {
                Wcx = 0.3F;
            }
            else if (curPoint.PowerDianLiu == "Ib" && curPoint.PowerYinSu == "0.5L")
            {
                Wcx = 0.3F;
            }
            else if (curPoint.PowerDianLiu == "Itr" && curPoint.PowerYinSu == "1.0")
            {
                Wcx = 0.4F;
            }
            return Wcx;
        }
        

        #endregion

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

        #region ��ʼ���豸��������ʽ�汾��Ч[�������]:InitEquipment
        /// <summary>
        /// ��ʼ���豸����,����ÿһ�����Ҫ�춨��Ȧ��
        /// </summary>
        /// <param name="_curPoint"></param>
        /// <param name="PL"></param>
        /// <param name="_Pulselap"></param>
        /// <returns></returns>
        private bool InitEquipment(StPlan_WcPoint _curPoint, float PL, int[] _Pulselap)
        {
            if (GlobalUnit.IsDemo) return true;
            //MessageController.Instance.AddMessage("���ñ���!", false);
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());
            

            MessageController.Instance.AddMessage("��ʼ����Դ���!");
            float _xIb = Number.GetCurrentByIb(_curPoint.PowerDianLiu, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            bool result = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb, (int)_curPoint.PowerYuanJian, (int)_curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
            if (!result)
            {
                bool isSuccess = false;
                for (int i = 1; i <= 3; i++)
                {
                    System.Threading.Thread.Sleep(i * 1000);
                    string msg = "������Դ���ʧ��,�ظ�" + i.ToString() + "��";
                    MessageController.Instance.AddMessage(msg);
                    msg = DateTime.Now.ToString() + "==========>" + msg;
                    ErrorLog.Write(new Exception(msg));
                    isSuccess = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb, (int)_curPoint.PowerYuanJian, (int)_curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
                    if (isSuccess)
                    {
                        break;
                    }
                }
                if (!isSuccess)
                {
                    MessageController.Instance.AddMessage("����Դ���ʧ��", 6, 2);
                    //return false;
                }
            }
            if (Stop) return true;
            MessageController.Instance.AddMessage("����Դ����ɹ�");
            MessageController.Instance.AddMessage("��ʼ��ʼ���������춨����!");
            int[] meterconst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
            //System.Threading.Thread.Sleep(base.m_WaitTime_PowerOn);
            result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang,meterconst, _Pulselap);
            MessageController.Instance.AddMessage("��ʼ�����춨����" + GetResultString(result));
            if (!result)
            {
                result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang,meterconst, _Pulselap);
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

        #region ----------����/���춨�����Ƿ���ȷ:CheckPara
        /// <summary>
        /// ����/���춨�����Ƿ���ȷ
        /// </summary>
        protected override bool CheckPara()
        {
            //�����������|���ʷ���|����Ԫ��|��������|��������|���г��|������|�������Ȧ��|����|����
            //�������|�����й�|H|1.0|Imax|��|��
            string[] arrayErrorPara = VerifyPara.Split('|');
         
            StPlan_WcPoint st_Wc = new StPlan_WcPoint();
            st_Wc.PrjID = "111010700";
            st_Wc.Dif_Err_Flag = 0;
            GlobalUnit.g_CUS.DnbData.SetWcxPercent(1, 1);
            //st_Wc.ErrorShangXian = 1;//�����ֵ���ٸ��ݹ�̼���
            //st_Wc.ErrorXiaXian = -1;S
            st_Wc.IsCheck = true;
            st_Wc.LapCount = 2;
            GlobalUnit.g_CUS.DnbData.SetCzQsIb(2, "Ib");
            st_Wc.nCheckOrder = 1;
            st_Wc.Pc = 0;
            st_Wc.PointId = 1;
            st_Wc.PowerDianLiu = arrayErrorPara[1];
            st_Wc.PowerFangXiang = Cus_PowerFangXiang.�����й�;
            st_Wc.PowerYinSu = arrayErrorPara[2];
            st_Wc.PowerYuanJian = Cus_PowerYuanJian.H;
            
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

        #region ��ǰ�춨�������
        /// <summary>
        /// ����ǰ�������
        /// </summary>
        protected void ControlResult()
        {
            if (Stop) return;
            Cus_MeterResultPrjID curItemID;

            StPlan_WcPoint curPoint = (StPlan_WcPoint)CurPlan;
            if (curPoint.Pc == 1)
            {
                curItemID = Cus_MeterResultPrjID.��׼ƫ��;
            }
            else
            {
                curItemID = Cus_MeterResultPrjID.�����������;
            }
            //��ǰ�춨�����ܽ��۽ڵ�����
            string curItemKey = ((int)curItemID).ToString("D3") + ((int)PowerFangXiang).ToString();
            for (int bw = 0; bw < BwCount; bw++)
            {

                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(bw);
                //��ʼ����ַ������
                if (curMeter.MeterResults.ContainsKey(curItemKey))
                    curMeter.MeterResults.Remove(curItemKey);
                if (!curMeter.YaoJianYn) continue;
                //��׼ƫ����۴���
                if (curItemID == Cus_MeterResultPrjID.��׼ƫ��)
                {
                    ControlPCResult(curMeter);
                }
                else
                {
                    //���������۴���
                    MeterResult curResult = new MeterResult();
                    curResult.Mr_chrRstId = curItemKey;
                    curResult.Mr_chrRstName = curItemID.ToString() + PowerFangXiang.ToString() + "����";
                    curResult.Mr_chrRstValue = Variable.CTG_HeGe;
                    curMeter.MeterResults.Add(curItemKey, curResult);
                    //�ַ�����۴������
                    if (curMeter.MeterErrors.ContainsKey(ItemKey))
                    {

                        if (curMeter.MeterErrors[ItemKey].Me_chrWcJl == Variable.CTG_BuHeGe)
                            curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                        else
                        {
                            //��⵱ǰ�����µ��������Ƿ�ϸ�
                            if (!isTheSamePowerPDHeGe(curMeter))
                                curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                        }
                    }
                    else
                    {
                        //GlobalUnit.ForceVerifyStop = true;
                        //Stop = true;
                        //Check.Require(false, "��ǰ����û�ж�ȡ������ͨ�����¼��ַ����������\r\n1��������������Ƿ���ȷ\r\n2���������ͨ�����������Ƿ���ȷ\r\n3�������С�����㣬ϵͳ�����������ʱ��ֵ���ô�һЩ");
                        //ThreadManage.Sleep(1000);
                        return;
                    }
                }
            }
        }

        //�������ƫ��
        private void ControlPCResult(MeterBasicInfo curMeter)
        {

            //��׼ƫ���
            #region -----�������ƫ��:һ���һ�����ƫ��-----
            string strKey = String.Format("{0}", ((int)CLDC_Comm.Enum.Cus_MeterResultPrjID.���ƫ��).ToString());
            //strKey += ((int)_curPoint.PowerFangXiang).ToString();       //�ӹ��ʷ���
            MeterResult _MaxWindage = null;
            if (!curMeter.MeterErrors.ContainsKey(ItemKey))
            {
                Stop = true;
                //GlobalUnit.ForceVerifyStop = true;
                //MessageController.Instance.AddMessage("��ǰ����û�ж�ȡ������ͨ�����¼��ַ����������\r\n1��������������Ƿ���ȷ\r\n2���������ͨ�����������Ƿ���ȷ\r\n3�������С�����㣬ϵͳ�����������ʱ��ֵ���ô�һЩ", 6, 2);
                //ThreadManage.Sleep(1000);
                return;
                //throw new Exception("��ǰ�춨����û�з����������");
            }

            string[] _w = curMeter.MeterErrors[ItemKey].Me_chrWcMore.Split('|');
            if (!curMeter.MeterResults.ContainsKey(strKey))
            {
                _MaxWindage = new MeterResult();
                _MaxWindage.Mr_chrRstId = strKey;
                _MaxWindage.Mr_chrRstName = CLDC_Comm.Enum.Cus_MeterResultPrjID.���ƫ��.ToString();
                _MaxWindage.Mr_chrRstValue = _w[_w.Length - 1];       //���ƫ��ʱ����������ƫ��ֵ
                curMeter.MeterResults.Add(strKey, _MaxWindage);
            }
            else
            {
                _MaxWindage = curMeter.MeterResults[strKey];
                //������α��ϴδ����滻
                float _LastWindage = float.Parse(_MaxWindage.Mr_chrRstValue);
                float _thisWindage = float.Parse(_w[_w.Length - 1]);
                if (_thisWindage > _LastWindage)
                {
                    /* 
                     ���ΪʲôΪ����_thisWindage�أ�
                     ��Ϊ_thisWindage�Ǳ�parse���ģ���Ӱ������С��λ
                     */
                    _MaxWindage.Mr_chrRstValue = _w[_w.Length - 1];
                }
            }

            #endregion

        }

        /// <summary>
        /// �Ƿ���ͬ�����µ����е�ǰ�춨��Ŀ���ϸ�
        /// </summary>
        /// <param name="curMeter">��ǰ������</param>
        /// <returns></returns>
        private bool isTheSamePowerPDHeGe(MeterBasicInfo curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.MeterErrors.Keys)
            {
                //��ǰ���ʷ���
                MeterError _Item = curMeter.MeterErrors[strKey];

                if (_Item.Me_chrProjectNo.Substring(0, 1).Trim() == "1")            //��������
                {
                    Cus_PowerFangXiang thisPointFX = (Cus_PowerFangXiang)(int.Parse(_Item.Me_chrProjectNo.Substring(1, 1)));
                    //CheckPoint curResultItem = (CheckPoint)CurPlan;
                    if (PowerFangXiang == thisPointFX)
                    {
                        if (_Item.Me_chrWcJl == Variable.CTG_BuHeGe)
                        {
                            isAllItemOk = false;
                            break;
                        }
                    }
                }
            }
            return isAllItemOk;
        }
        #endregion
        #endregion
    }
}
