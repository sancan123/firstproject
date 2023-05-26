
using System;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// ����/������춨��
    /// �������������ֱ�Ӽ�¼��_MeterQdQid��
    /// </summary>
    class Starts : VerifyBase
    {

        bool[] CheckOver = new bool[0];
        /// <summary>
        /// ÿһ�����Ҫ����ʱ��
        /// </summary>
        float[] arrStartTimes = new float[0];
        /// <summary>
        /// ÿһ�����Ҫ���𶯵���
        /// </summary>
        float[] arrStartCurrents = new float[0];
        /// <summary>
        /// �𶯶�ȡ�ĵ�һ��ʱ��
        /// </summary>
        float[] StartTimeBefore = new float[0];
        /// <summary>
        /// �𶯶�ȡ�ĵڶ���ʱ��
        /// </summary>
        float[] StartTimeAffter = new float[0];
        /// <summary>
        /// �����𶯵���
        /// </summary>
        float startCurrent = 0F;

        public Starts(object plan)
            : base(plan)
        {
        }
        /// <summary>
        /// ����в���Ҫ��дCheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //����Ҫ�����춨����
            //���ʷ���|�Ƿ�Ĭ�Ϻϸ�|�Զ������������|�������|�Զ���������ʱ��|����ʱ��(����)
            string[] arrayTemp = VerifyPara.Split('|');
            if (arrayTemp.Length != 7)
            {
                return false;
            }
            Cus_PowerFangXiang fangxiangTemp = Cus_PowerFangXiang.�����й�;
            fangxiangTemp = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), arrayTemp[1]);
            curPlan.PowerFangXiang = fangxiangTemp;
            if (arrayTemp[3] == "��")
            {
                curPlan.FloatxIb = 0;
            }
            else
            {
                curPlan.FloatxIb = float.Parse(arrayTemp[4]);
            }
            if (arrayTemp[5] == "��")
            {
                curPlan.xTime = 0;
            }
            else
            {
                curPlan.xTime = float.Parse(arrayTemp[6]);
            }
            curPlan.DefaultValue = arrayTemp[2] == "��" ? 1 : 0;
            //ȷ���춨�������Щ��ϸ����,���������
            ResultNames = new string[] { "����ʱ��", "���ʷ���", "�����ѹ", "��׼����ʱ��(һ������)", "�������", "��ʼʱ��", "����ʱ��", "ʵ������ʱ��", "������", "���1", "���2", "����", "���ϸ�ԭ��" };

            return true;
        }
        /// <summary>
        /// ��Ŀ�ܽ�������
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)CLDC_Comm.Enum.Cus_MeterResultPrjID.������);
            }
        }

        private StPlan_QiDong curPlan = new StPlan_QiDong();

        /// <summary>
        /// ��Ŀ����ֵ,������ֻ��¼��ǰ���ʷ�����ۡ�����¼�ܽ���
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}"
                , ResultKey
                , ((int)PowerFangXiang).ToString());
            }
        }

        /// <summary>
        /// ����춨����,����춨ʱ����𶯵���
        /// </summary>
        protected override void DefaultItemData()
        {
            //�������ݸ���
            string[] arrStrResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);

                /*
                 Rem:
                 * ���춨�ı�ҲҪ�ѽڵ���ϣ���Ȼ������ѡ��Ҫ������
                 */
                /*��ӽ��۽ڵ�*/
                MeterQdQid _MeterQdQid = null;
                if (_Meter.MeterQdQids.ContainsKey(ItemKey))
                {
                    _Meter.MeterQdQids.Remove(ItemKey);                             //�Ӹ�����
                }
                _MeterQdQid = new MeterQdQid();
                _Meter.MeterQdQids.Add(ItemKey, _MeterQdQid);
                _MeterQdQid.Mqd_chrDL = curPlan.FloatxIb.ToString();                 //�𶯵���
                _MeterQdQid._intMyId = _Meter._intMyId;                        //��Ψһ��ʶ��
                _MeterQdQid.Mqd_chrProjectNo = ItemKey;                                     //��ĿID
                _MeterQdQid.Mqd_chrProjectName = curPlan.ToString();                       //��Ŀ����
                _MeterQdQid.Mqd_chrJdfx = (Convert.ToInt32(curPlan.PowerFangXiang)).ToString();
                _MeterQdQid.Mqd_chrJL = Variable.CTG_DEFAULTRESULT;            //Ĭ��
                _MeterQdQid.AVR_STANDARD_TIME = arrStartTimes[i].ToString();
                _MeterQdQid.AVR_ACTUAL_TIME = (arrStartTimes[i] - 4).ToString();
                _MeterQdQid.Mqd_chrTime = (arrStartTimes[i] - 4).ToString();                 //Ĭ��ʱ��Ϊ��׼����ʱ��  
                arrStrResultKey[i] = ItemKey;
                objResultValue[i] = _MeterQdQid;
                //if (Stop) return;
            }

            //ˢ��UI
            base.DefaultItemData();
        }

        /// <summary>
        /// ��ʼ���춨����
        /// </summary>
        /// <returns>��ʱ��</returns>
        private float InitVerifyPara()
        {
            string[] arrayErrorPara = VerifyPara.Split('|');
            if (arrayErrorPara[3] == "��")
            {
                //  curPlan.FloatxIb = 1F;
                curPlan.FloatxIb = float.Parse(arrayErrorPara[4]);
            }
            if (arrayErrorPara[5] == "��")
            {
                curPlan.xTime = float.Parse(arrayErrorPara[6]);
                curPlan.CheckTime = float.Parse(arrayErrorPara[6]);

            }


            //���ϵͳ���չ��
            //����ÿһ������ʱ��
            int[] _MeterConst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
            arrStartTimes = new float[BwCount];
            arrStartCurrents = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                //�����𶯵���
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (_Meter == null || !_Meter.YaoJianYn)
                {
                    continue;
                }

                bool bFind = false;

                for (int j = i - 1; j >= 0; j--)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                    if (_Meter.Mb_chrBcs == Helper.MeterDataHelper.Instance.Meter(j).Mb_chrBcs && _Meter.Mb_chrBdj == Helper.MeterDataHelper.Instance.Meter(j).Mb_chrBdj)
                    {
                        arrStartTimes[i] = arrStartTimes[j];
                        arrStartCurrents[i] = arrStartCurrents[j];
                        bFind = true;
                        break;
                    }
                    if (Stop) return 0F;
                }

                if (!bFind)
                {
                    StPlan_QiDong _tagQiDong = (StPlan_QiDong)curPlan;
                
                    _tagQiDong.CheckTimeAndIb(_Meter.GuiChengName,
                                              GlobalUnit.Clfs,
                                              GlobalUnit.U,
                                              _Meter.Mb_chrIb,
                                              _Meter.Mb_chrBdj,
                                              _Meter.Mb_chrBcs,
                                              _Meter.Mb_BlnZnq,
                                              _Meter.Mb_BlnHgq);
                    arrStartTimes[i] = (float)Math.Round(_tagQiDong.CheckTime, 2);
                    arrStartCurrents[i] = _tagQiDong.FloatIb;
                    /*
                    ���ͬһ��ƽ���ڲ�ͬ�𶯵��������𶯵���ȡ���ֵ
                    */
                    if (_tagQiDong.FloatIb > startCurrent)
                    {
                        startCurrent = _tagQiDong.FloatIb;
                    }
                }
            }

            float[] arrStartTimeClone = (float[])arrStartTimes.Clone();
            CLDC_DataCore.Function.Number.PopDesc(ref arrStartTimeClone, false);                        //ѡ��һ�������ʱ��
            MessageController.Instance.AddMessage("����춨�������");
            if (GlobalUnit.IsDemo)
                return 1F;
            else
                return arrStartTimeClone[0];
        }

        #region ----------��ʼ�춨---------
        /// <summary>
        /// ��ʼ�춨
        /// </summary>
        public override void Verify()
        {
           
            string[] PulseTime = new string[BwCount];                   //��¼��ʼ��ʱ��
            int[] PulseCount = new int[BwCount];                        //�������
            CheckOver = new bool[BwCount];
            float TotalTime = InitVerifyPara();                         //��ʼ����
            float _MaxStartTime = TotalTime * 60F;                      //���������ʱ��
            StartTimeAffter = new float[BwCount];
            StartTimeBefore = new float[BwCount];
            base.Verify();
            string[] verPlan = VerifyPara.Split('|');
            PowerOn();
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("��ʼԤ�ȣ���ȴ�" + TIME + "��");
                if (Stop) return;
               

                ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * TIME);

            }//��¼��ǰ�춨ID
            GlobalUnit.g_CurTestType = 3;
            m_StartTime = DateTime.Now;

            float[] TestTime = new float[BwCount];
            float[] TimesD = new float[BwCount];
            //Ĭ�Ϻϸ���������춨

            //�ϱ��춨����
            //string dataValueKey = ItemKey;                                                  //�춨���ݽڵ�����:P_�춨ID
            //string[] arrStrResultKey = new string[BwCount];
            //object[] arrObjResultValue = new object[BwCount];
            if (curPlan.DefaultValue == 1)
            {
                int totalTime = 3000;
                while (totalTime > 0)
                {
                    MessageController.Instance.AddMessage("��������Ĭ�Ϻϸ�,�ȴ�" + (totalTime / 1000) + "��");
                    if (Stop) break;
                    Thread.Sleep(1000);
                    totalTime -= 1000;
                }
                for (int Num = 0; Num < BwCount; Num++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn) continue;
                    ResultDictionary["����"][Num] = Variable.CTG_HeGe;
                    ResultDictionary["�������"][Num] = startCurrent.ToString("F2");
                    ResultDictionary["��ʼʱ��"][Num] = m_StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["���ʷ���"][Num] = curPlan.PowerFangXiang.ToString();
                    ResultDictionary["�����ѹ"][Num] = GlobalUnit.U.ToString("F2");
                    ResultDictionary["ʵ������ʱ��"][Num] = (VerifyPassTime / 60.0).ToString("F4") + "��";
                    ResultDictionary["����ʱ��"][Num] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    
                }
                ConvertTestResult("��׼����ʱ��(һ������)", arrStartTimes, 2);

                UploadTestResult("����ʱ��");
                Thread.Sleep(1000);
                UploadTestResult("�����ѹ");
                Thread.Sleep(1000);
                UploadTestResult("�������");
                Thread.Sleep(1000);
                UploadTestResult("��ʼʱ��");
                Thread.Sleep(1000);
                UploadTestResult("���ʷ���");
                Thread.Sleep(1000);
                UploadTestResult("��׼����ʱ��(һ������)");
                Thread.Sleep(1000);
                UploadTestResult("ʵ������ʱ��");
                Thread.Sleep(1000);
                UploadTestResult("����");
            }
            else
            {
                #region -----------��ʼ����-----------
                //��һ������ɫ��
                if (!GlobalUnit.IsDemo)
                {
                    //��ɫ��
                    //PulseChannelDetection pulseDetect = new PulseChannelDetection(null);
                    //pulseDetect.ParentControl = this;
                    //if (!pulseDetect.DuiSheBiao(curPlan.PowerFangXiang, CLDC_DataCore.Const.GlobalUnit.Ib * 0.1F, 1))
                    //{
                    //    return;
                    //}
                    if (Stop)
                    {
                        return;
                    }
                    //���ù��ܲ���
                    int[] startTimes = new int[BwCount];
                    for (int bw = 0; bw < BwCount; bw++)
                    {
                        startTimes[bw] = (int)(arrStartTimes[bw] * 60F);
                    }
                    //���������ѹ����
                    if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, startCurrent, (int)Cus_PowerYuanJian.H, (int)curPlan.PowerFangXiang, FangXiangStr + "1.0", IsYouGong, false))
                        return;
                  int[] meterconst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
                  if (GlobalUnit.clfs == Cus_Clfs.����)
                  {
                      if (curPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹� || curPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹�)
                      {
                          MeterProtocolAdapter.Instance.SetPulseCom(3);
                      }
                  }


                    if (!Helper.EquipHelper.Instance.InitPara_Start(curPlan.PowerFangXiang, startCurrent, IsYouGong, startTimes,meterconst))
                    {
                        Helper.EquipHelper.Instance.InitPara_Start(curPlan.PowerFangXiang, startCurrent, IsYouGong, startTimes,meterconst);
                        //MessageController.Instance.AddMessage("��ʼ�������豸����ʧ��!",false , Cus_MessageType.��ʾ��Ϣ); 
                        //return;
                    }
                    if (Stop) return;
                }

                m_CheckOver = false;
                MessageController.Instance.AddMessage("�춨��...");
                m_StartTime = DateTime.Now;

                #region �ϱ��������
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["�������"][i] = startCurrent.ToString("F2");
                        ResultDictionary["��ʼʱ��"][i] = m_StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["���ʷ���"][i] = curPlan.PowerFangXiang.ToString();
                        ResultDictionary["�����ѹ"][i] = GlobalUnit.U.ToString("F2");
                        TestTime[i] = (float)(arrStartTimes[i] * 4.5 / 100);
                        TimesD[i] = arrStartTimes[i] / 60F;
                    }
                }

                ConvertTestResult("��׼����ʱ��(һ������)", TimesD, 2);

                UploadTestResult("�����ѹ");
                Thread.Sleep(1000);
                UploadTestResult("�������");
                Thread.Sleep(1000);
                UploadTestResult("��ʼʱ��");
                Thread.Sleep(1000);
                UploadTestResult("���ʷ���");
                Thread.Sleep(1000);
                UploadTestResult("��׼����ʱ��(һ������)");
                #endregion

                while (true)
                {

                    /*����Ӳ��������ǰ���ʱ���ȡƵ��Ϊ1�Σ���30%����5��/�ε�Ƶ�ʶ�ȡ����20%��һ��һ�ε�Ƶ��*/
                    //ÿһ��ˢ��һ������
                    long _PastTime = base.VerifyPassTime;
                    Thread.Sleep(1000);
                    m_CheckOver = true;
                    if (!GlobalUnit.IsDemo)
                    {
                        ReadAndDealData(_PastTime);
                    }
                    else
                    {
                        m_CheckOver = false;
                    }

                    if (Stop)
                    {
                        return;
                    }

                    float pastMinute = _PastTime / 60F;
                    GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = "��(��)��ʱ��" + (TotalTime * 4.5 / 60).ToString("F2") + "�֣��Ѿ�����" + pastMinute.ToString("F2") + "��";
                    if (Helper.MeterDataHelper.Instance.TypeCount > 1)
                    {
                        strDes += ",�����Ƕ��ֱ��죬�����������ǰ������";
                    }
                    MessageController.Instance.AddMessage(strDes);

                    if (Stop || m_CheckOver)
                    {
                        GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }
                }

                #endregion

            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["����"][i] = ResultDictionary["����"][i] == "�ϸ�" ? "�ϸ�" : "���ϸ�";
                    if (ResultDictionary["����"][i] == "���ϸ�")
                    {
                        ResultDictionary["����ʱ��"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            UploadTestResult("����ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("����");

            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 3);
            MessageController.Instance.AddMessage("��ǰ������Ŀ���");
        }

        /// <summary>
        /// ��ȡ������춨����
        /// </summary>
        private void ReadAndDealData(long verifyTime)
        {

              //  CLDC_DeviceDriver.stError[] arrTagData = Helper.EquipHelper.Instance.ReadWcb(true);
            m_CheckOver = true;
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    CheckOver[k] = true;
                    continue;
                }
                int num = 0;
                string data = string.Empty;
                string time = string.Empty;

                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(k + 1, 3, out num, out data, out time);
                CLDC_DeviceDriver.stError arrTagData = Helper.EquipHelper.Instance.ReadWc(k + 1);
                if (verifyTime <= arrStartTimes[k] * 4.5 && CheckOver[k] == false)
                {
                    ResultDictionary["������"][k] = data;
                    if (data != null && time != null && data != "" && time != "")
                    {
                        if (data == "2")
                        {
                            StartTimeBefore[k] = float.Parse(time);
                            ResultDictionary["���1"][k] = arrTagData.szError;
                        }
                        else if (data == "3")
                        {
                            StartTimeAffter[k] = float.Parse(time);
                            ResultDictionary["���2"][k] = arrTagData.szError;
                            CheckOver[k] = true;
                        }


                        if (StartTimeBefore[k] != 0 && StartTimeAffter[k] != 0 && arrStartTimes[k] * 1.5 > StartTimeBefore[k] && arrStartTimes[k] * 1.5 > StartTimeAffter[k])
                        {
                            ResultDictionary["����"][k] = Variable.CTG_HeGe;
                            ResultDictionary["����ʱ��"][k] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            ResultDictionary["ʵ������ʱ��"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "��";
                        }
                        else
                        {
                            ResultDictionary["����"][k] = "";
                            ResultDictionary["ʵ������ʱ��"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "��";
                            CheckOver[k] = false;
                        }
                    }

                    if (!CheckOver[k])
                    {
                        m_CheckOver = false;
                    }
                }
                if (Stop) break;
            }

            UploadTestResult("����ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("ʵ������ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("������");
            Thread.Sleep(1000);
            UploadTestResult("���1");
            Thread.Sleep(1000);
            UploadTestResult("���2");
            Thread.Sleep(1000);
        }

        #endregion




    }
}
