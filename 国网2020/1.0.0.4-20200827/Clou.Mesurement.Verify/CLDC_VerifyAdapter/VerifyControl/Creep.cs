
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DeviceDriver;
////using ClInterface;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// Ǳ������
    /// ֻ��¼��ǰ�������
    /// </summary>
    class Creep : VerifyBase
    {
        float creepI = 9999999F;
        //���ϵͳ���չ��
        float[] arrCreepTimes = new float[0];
        bool[] CheckOver = new bool[0];
        public Creep(object plan) : base(plan) { }

        #region-------���෽����д --------
        /// <summary>
        /// �ܽ�������
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.Ǳ������);
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
                        , ((int)PowerFangXiang).ToString()
                       , (Convert.ToInt32(curPlan.FloatxU * 100)).ToString("D3"));//, curPlan.FloatxU.ToString()
            }
        }
        private StPlan_QianDong curPlan = new StPlan_QianDong();
        /// <summary>
        /// ����в���Ҫ��дCheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //����Ҫ�����춨����
            //���ʷ���|�Ƿ�Ĭ�Ϻϸ�|Ǳ����ѹ|Ǳ������|�Զ�����Ǳ��ʱ��|Ǳ��ʱ��
            string[] arrayTemp = VerifyPara.Split('|');
            if (arrayTemp.Length != 6)
            {
                return false;
            }
            Cus_PowerFangXiang fangxiangTemp = Cus_PowerFangXiang.�����й�;
            fangxiangTemp = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), arrayTemp[0]);
            curPlan.PowerFangXiang = fangxiangTemp;
            float floatTemp = 1.15F;
            if (float.TryParse(arrayTemp[2].Replace("%", ""), out floatTemp))
            {
                curPlan.FloatxU = floatTemp / 100F;
            }
            else
            {
                curPlan.FloatxU = 1.15F;
            }
            curPlan.FloatxIb = 0;
            switch (arrayTemp[3])
            {
                case "1/4��������":
                    curPlan.FloatxIb = 0.25F;
                    break;
                case "1/5��������":
                    curPlan.FloatxIb = 0.2F;
                    break;
            }
            if (arrayTemp[4] == "��")
            {
                curPlan.xTime = 0;
            }
            else
            {
                float fTemp = 0;
                if (float.TryParse(arrayTemp[5], out fTemp))
                {
                    curPlan.xTime = fTemp;
                }
                else
                {
                    curPlan.xTime = 0;
                }
            }
            curPlan.DefaultValue = arrayTemp[1] == "��" ? 1 : 0;
            //ȷ���춨�������Щ��ϸ����,���������
            ResultNames = new string[] { "���ʷ���", "�����ѹ", "��׼����ʱ��", "�������", "��ʼʱ��", "����ʱ��", "ʵ������ʱ��", "������", "����" };

            return true;
        }
        /// <summary>
        /// ��Ĭ������,��Ŀ������ResultKey+PowerFangXiand
        /// </summary>
        protected override void DefaultItemData()
        {
            //��������
            string[] arrStrResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
                /*�ҽӽ�������*/
                MeterQdQid _MeterQdQid = new MeterQdQid();
                if (_Meter.MeterQdQids.ContainsKey(ItemKey))
                {
                    _Meter.MeterQdQids.Remove(ItemKey);
                }
                _Meter.MeterQdQids.Add(ItemKey, _MeterQdQid);
                _MeterQdQid.Mqd_chrDL = _tagQianDong.FloatIb.ToString();// _StartI.ToString("F4");           //Ǳ������
                _MeterQdQid.AVR_VOLTAGE = curPlan.FloatxU.ToString();
                _MeterQdQid._intMyId = _Meter._intMyId;                //������ID
                _MeterQdQid.Mqd_chrProjectNo = ItemKey;                             //��ĿID
                _MeterQdQid.Mqd_chrProjectName = _tagQianDong.ToString();          //��Ŀ����
                _MeterQdQid.Mqd_chrJdfx = (Convert.ToInt32(curPlan.PowerFangXiang)).ToString();
                _MeterQdQid.Mqd_chrJL = Variable.CTG_HeGe;      //Ĭ�Ϻϸ�
                _MeterQdQid.AVR_STANDARD_TIME = arrCreepTimes[i].ToString();
                _MeterQdQid.AVR_ACTUAL_TIME = arrCreepTimes[i].ToString();
                _MeterQdQid.Mqd_chrTime = arrCreepTimes[i].ToString();   //��׼ʱ��
                arrStrResultKey[i] = ItemKey;
                objResultValue[i] = _MeterQdQid;
            }

            base.DefaultItemData();
        }
        #endregion

        #region ----------����Ǳ��ʱ�估����InitVerifyPara----------
        /// <summary>
        /// ��ʼ��Ǳ������
        /// </summary>
        /// <returns></returns>
        private float InitVerifyPara()
        {
            string[] arrayErrorPara = VerifyPara.Split('|');
          
            if (arrayErrorPara[4] == "��")
            {
                curPlan.xTime = float.Parse(arrayErrorPara[5]);
                curPlan.CheckTime = float.Parse(arrayErrorPara[5]);

            }
            float xU = curPlan.FloatxU;
            arrCreepTimes = new float[BwCount];
            bool bHasStartItem = false;             //�Ƿ�����ͬ���ʷ����������Ŀ
            float startXIB = 0F;                    //�Ƿ��Զ����𶯵���
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!_Meter.YaoJianYn) continue;
                StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
                if (_tagQianDong.CheckTime == 0 && _tagQianDong.FloatIb == 0)
                {
                    bHasStartItem = isThisPFHasStartItem(ref startXIB);
                    if (bHasStartItem && startXIB > 0F)
                    {
                        _tagQianDong.CheckTimeAndIb(_Meter.GuiChengName,
                                                   GlobalUnit.Clfs,
                                                   GlobalUnit.U,
                                                   _Meter.Mb_chrIb,
                                                   startXIB,
                                                   _Meter.Mb_chrBdj,
                                                   _Meter.Mb_chrBcs,
                                                   _Meter.Mb_BlnZnq,
                                                   _Meter.Mb_BlnHgq);

                    }
                    else
                    {
                        _tagQianDong.CheckTimeAndIb(_Meter.GuiChengName,
                                                   GlobalUnit.Clfs,
                                                   GlobalUnit.U,
                                                   _Meter.Mb_chrIb,
                                                   _Meter.Mb_chrBdj,
                                                   _Meter.Mb_chrBcs,
                                                   _Meter.Mb_BlnZnq,
                                                   _Meter.Mb_BlnHgq);
                    }
                }
                arrCreepTimes[i] = (float)Math.Round(_tagQianDong.CheckTime*60, 2);
                if (_tagQianDong.FloatIb < creepI)
                    creepI = _tagQianDong.FloatIb;

            }
            //ѡ��һ�����Ǳ��ʱ��
            float[] arrCreepTimeClone = (float[])arrCreepTimes.Clone();
            CLDC_DataCore.Function.Number.PopDesc(ref arrCreepTimeClone, false);
            MessageController.Instance.AddMessage("��ʼ���춨�������");
            if (GlobalUnit.IsDemo)
                return 1F;
            else
                return arrCreepTimeClone[0];
        }
        /// <summary>
        /// �Ƿ�ǰǱ�����ʷ�������������Ŀ
        /// </summary>
        /// <returns></returns>
        private bool isThisPFHasStartItem(ref float xIB)
        {
            StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
            //object o = null;
            StPlan_QiDong _tagQiDong;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.CheckPlan.Count; i++)
            {
                // o = getPlan(CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter, i);
                if (GlobalUnit.g_CUS.DnbData.CheckPlan[i] is StPlan_QiDong)
                {
                    _tagQiDong = (StPlan_QiDong)GlobalUnit.g_CUS.DnbData.CheckPlan[i];
                    //ֻ��Ѱ�ҵ���һ������Ҫ���������Ŀ
                    if (_tagQianDong.PowerFangXiang == _tagQiDong.PowerFangXiang)
                    {
                        xIB = _tagQiDong.FloatxIb;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region ----------��ʼ�춨----------
        /// <summary>
        /// ��ʼ�춨
        /// </summary>
        public override void Verify()
        {
            string[] NoUse = new string[0];
            int[] PulseNum = new int[0];
            long _PastTime = 0;
            GlobalUnit.g_CurTestType = 3;
            float TotalTime = InitVerifyPara();
            base.Verify();
            CheckOver = new bool[BwCount];
            m_StartTime = DateTime.Now;

            if (Stop) return;
            //�������Ǳ��ʱ��
            float _MaxCreepTime = TotalTime /60;
            //�ϱ��춨����
            object[] arrObjResultValue = new object[BwCount];
            //Ĭ�Ϻϸ���������춨
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
                    if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn)
                    {
                        continue;
                    }

                    ResultDictionary["�������"][Num] = creepI.ToString("F2");
                    ResultDictionary["��ʼʱ��"][Num] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["���ʷ���"][Num] = curPlan.PowerFangXiang.ToString();
                    ResultDictionary["�����ѹ"][Num] = (curPlan.FloatxU * GlobalUnit.U).ToString("F4");
                    ResultDictionary["ʵ������ʱ��"][Num] = (VerifyPassTime / 60.0).ToString("F4") + "��";
                    ResultDictionary["����"][Num] = Variable.CTG_HeGe;
                }
                ConvertTestResult("��׼����ʱ��", arrCreepTimes, 2);

                UploadTestResult("�����ѹ");
                UploadTestResult("�������");
                UploadTestResult("��ʼʱ��");
                UploadTestResult("���ʷ���");
                UploadTestResult("��׼����ʱ��");
                UploadTestResult("ʵ������ʱ��");
                UploadTestResult("����");
            }
            else
            {
                if (!GlobalUnit.IsDemo)
                {
                    #region
                    //PulseChannelDetection pulseDetec = new PulseChannelDetection(null);
                    //pulseDetec.ParentControl = this;
                    ////�Ա�
                    //if (!pulseDetec.DuiSheBiao(curPlan.PowerFangXiang, CLDC_DataCore.Const.GlobalUnit.Ib * 0.1F, 1))
                    //{
                    //    Helper.LogHelper.Instance.Loger.Debug("��ɫ��ʧ�ܣ��˳��춨");
                    //    return;
                    //}
                    if (Stop) return;
                    //��ʼ������
                    int[] creepTimes = new int[BwCount];
                    for (int bw = 0; bw < BwCount; bw++)
                    {
                        creepTimes[bw] = (int)(arrCreepTimes[bw] / 60F);
                    }

                  

                    if (Stop) return;
                    //����Դ���
                    if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * curPlan.FloatxU, creepI, (int)Cus_PowerYuanJian.H, (int)curPlan.PowerFangXiang, "1.0", IsYouGong, false))
                    {
                        return;
                    }
                    if (GlobalUnit.clfs == Cus_Clfs.����)
                    {
                        if (curPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹� || curPlan.PowerFangXiang == Cus_PowerFangXiang.�����޹�)
                        {
                            MeterProtocolAdapter.Instance.SetPulseCom(3);
                        }
                    }
                    if (!Helper.EquipHelper.Instance.InitPara_Creep(curPlan.PowerFangXiang, creepI, IsYouGong, creepTimes))
                    {

                        MessageController.Instance.AddMessage("��ʼ��Ǳ������ʧ�ܣ��˳��춨");
                        return;
                    }
                    #endregion
                    //
                    //Helper.EquipHelper.Instance.InitPara_InitTimeAccuracy();
                    //
                }
                m_StartTime = DateTime.Now;

                #region �ϱ��������
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["�������"][i] = creepI.ToString("F2");
                        ResultDictionary["��ʼʱ��"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["���ʷ���"][i] = curPlan.PowerFangXiang.ToString();
                        ResultDictionary["�����ѹ"][i] = (curPlan.FloatxU * GlobalUnit.U).ToString("F4");
                        arrCreepTimes[i] = arrCreepTimes[i] / 60F;
                    }
                }
                ConvertTestResult("��׼����ʱ��", arrCreepTimes, 2);
                UploadTestResult("�����ѹ");
                UploadTestResult("�������");
                UploadTestResult("��ʼʱ��");
                UploadTestResult("���ʷ���");
                UploadTestResult("��׼����ʱ��");
                #endregion

                Stop = false;
                //int rjs = 1;
                MessageController.Instance.AddMessage("�춨��...");
                while (true)
                {
                    _PastTime = VerifyPassTime;

                    //ÿһ��ˢ��һ������
                    Thread.Sleep(1000);
                    if (Stop)
                    {
                        Helper.LogHelper.Instance.Loger.Debug("�ⲿֹͣ���˳��춨");
                        Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false,7);
                        m_CheckOver = true;
                        return;
                    }
                    if (!GlobalUnit.IsDemo)
                    {
                        ReadAndDealData(_PastTime);
                    }
                    else
                    {
                        m_CheckOver = false;
                    }

                    float PastTime = _PastTime / 60F; //ת��Ϊ�ַ��͵�UI 
                    GlobalUnit.g_CUS.DnbData.NowMinute = PastTime;
                    MessageController.Instance.AddMessage("Ǳ��ʱ��" + (TotalTime/60).ToString("F2") + "�֣��Ѿ�����" + PastTime.ToString("F2") + "��");
                    if ((PastTime > _MaxCreepTime) || m_CheckOver)
                    {
                        m_CheckOver = true;
                        Helper.LogHelper.Instance.Loger.Debug("Ǳ��ʱ���ѵ����˳��춨");
                        MessageController.Instance.AddMessage("Ǳ��ʱ���ѵ����˳��춨");
                        break;
                    }
                    //if (PastTime > 1 && rjs<2)
                    //{
                    //    rjs++;
                    //    Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false);
                    //}
                }
            }

            if (!GlobalUnit.IsDemo && !Stop && !m_CheckOver)
            {
                //�����ٶ�һ�Σ��Է���һ
                ReadAndDealData((long)_MaxCreepTime);
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    string resultTemp = ResultDictionary["����"][i];
                    ResultDictionary["����"][i] = resultTemp == "���ϸ�" ? "���ϸ�" : "�ϸ�";
                    if (resultTemp == "�ϸ�")
                    {
                        ResultDictionary["����ʱ��"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            UploadTestResult("ʵ������ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("����ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("����");
            Thread.Sleep(1000);
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false,3);
            m_CheckOver = true;
        }

        /// <summary>
        /// ��ȡ����������[��ʾ����Ч]
        /// </summary>
        /// <param name="verifyTimes"></param>
        private void ReadAndDealData(long verifyTimes)
        {
           // stError[] arrTagError = Helper.EquipHelper.Instance.ReadWcb(true);
            if (Stop)
            {
                return;
            }
            //�����б�λ��Ϊ���ϸ�ʱ,�춨���
            m_CheckOver = true;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    CheckOver[i] = true;
                    continue;
                }
                int num = 0;
                string data = string.Empty;
                string time = string.Empty;

                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(i + 1, 3, out num, out data, out time);
                int intTemp = 0;
              //  int.TryParse(arrTagError[i].szError, out intTemp);
                //��������
                //�������������0,���ϸ�
                if (verifyTimes < (arrCreepTimes[i]*60F) && CheckOver[i] == false)
                {
                    ResultDictionary["������"][i] = data;
                    if (data == "" || data == null) data = "0";
                    if (data != null && data != "" )
                    {
                        if (float.Parse(data) > 0)
                        {
                            ResultDictionary["ʵ������ʱ��"][i] = (verifyTimes / 60.0).ToString("F4") + "��";
                            ResultDictionary["����ʱ��"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            ResultDictionary["����"][i] = Variable.CTG_BuHeGe;
                            CheckOver[i] = true;
                        }
                        else
                        {
                            ResultDictionary["ʵ������ʱ��"][i] = (((float)verifyTimes) / 60.0).ToString("F4") + "��";
                            CheckOver[i] = false;

                        }
                    }
                }
                if (!CheckOver[i])
                {
                    m_CheckOver = false;
                }
                if (Stop) break;

            }
            UploadTestResult("ʵ������ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("����ʱ��");
            Thread.Sleep(1000);
            UploadTestResult("������");
            Thread.Sleep(1000);
        }
        #endregion
    }
}
