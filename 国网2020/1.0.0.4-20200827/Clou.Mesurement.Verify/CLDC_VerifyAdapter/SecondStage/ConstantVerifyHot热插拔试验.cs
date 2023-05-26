
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.Helper;
using CLDC_DeviceDriver;

namespace CLDC_VerifyAdapter.SecondStage
{
    #region 
    enum TestType1
    {
        Ĭ�� = 0,
        /// <summary>
        /// �����ͬʱ����ָ�����ܷ��ʼ������ַ�����һ��ʱ���ȶ�ȡ�ܷ��ʵ����룬Ȼ���߷ַ��ʡ�����ٶ�ȡ��
        /// ���ʵ�ֹ�롣
        /// </summary>
        ����ַ���ͬʱ�� = 1,
        /// <summary>
        /// �������ַ�ʽ��ָֻ����ʱ��ȡ���зַ������룬��������ٶ�ȡ���зַ���ֹ�롣
        /// ���ַ�ʽӦ����������ʱ��ϳ���ͬʱ�缸���ַ��ʵ����
        /// </summary>
        �Զ��춨��ʱ���ڵ����зַ��� = 2
    }

    /// <summary>
    /// ��ȡ������ʽ
    /// </summary>
    public enum ReadEnergyType
    {
        ʹ��485�Զ���ȡ,
        //��ȡ������������,
        �ֶ�����
    }
    #endregion

    /// <summary>
    /// �Ȳ������
    /// </summary>
    class ConstantVerifyHot : VerifyBase
    {
        #region ----------��������----------
        /// <summary>
        /// ��ǰ�����Ƿ�ȫ�����
        /// </summary>
        private bool CurActionOver = false;

        //��׼���ۻ�����
        private float _StarandMeterDl = 0F;

        #endregion

        public CLDC_Encryption.CLEncryption.Interface.IAmMeterEncryption EncryptionTool;


        public CLDC_Encryption.CLEncryption.EncryptionFactory encryptionFactory;
        /// <summary>
        /// ����ڵ�����[��д]
        /// </summary>
        protected override void ClearItemData()
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo meter = null;
            for (int i = 0; i < BwCount; i++)
            {
                meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (meter != null)
                {
                    if (meter.MeterZZErrors.ContainsKey(ItemKey))
                    {
                        meter.MeterZZErrors.Remove(ItemKey);
                    }
                }
            }
        }

        #region------------����------------
        /// <summary>
        /// �Ƿ��ǻ�е��
        /// </summary>
        private bool IsJiXieBiao
        {
            get
            {
                MeterBasicInfo _m = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);
                return !CLDC_DataCore.Function.Common.IsEmpty(_m.Mb_chrAddr);
            }
        }


        protected override string ItemKey
        {
            get
            {
                return ((StPlan_ZouZi)CurPlan).itemKey;
            }
        }

        protected override string ResultKey
        {
            get { return string.Format("{0}", (int)Cus_MeterResultPrjID.��������); }
        }

        #endregion

        public ConstantVerifyHot(object plan)
            : base(plan)
        {
          
        }

        #region ��ȡ���롢ֹ�롢д��ʱ��
        /// <summary>
        /// ��ȡ�������Ϣ 
        /// </summary>
        /// <param name="isStartEnergy"></param>
        /// <returns></returns>
        private bool ReadMeterEnergys(bool isStartEnergy)
        {
            int YaoJianCount = 0;
            int noPassCount = 0;
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            MessageController.Instance.AddMessage(string.Format("���ڶ�ȡ{0}����...", isStartEnergy ? "��ʼ" : "����"));
          
            MeterBasicInfo curMeter;
            try
            {
                noPassCount = 0;
                YaoJianCount = 0;
                //�ȴ� 3�룬��Ϊ�л���Դ�󣬿��ܻ��·�ϵ磬�������ʱ����Ҫһ��ʱ��
                Thread.Sleep(3 * 1000);
                float[] energys = MeterProtocolAdapter.Instance.ReadEnergy((byte)(this.PowerFangXiang - 1), (byte)((StPlan_ZouZi)CurPlan).FeiLv);
                for (int i = 0; i < BwCount; i++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (curMeter.YaoJianYn == false)
                    {
                        continue;
                    }
                    if (Stop)
                    {
                        break;
                    }

                    YaoJianCount++;
                    if (energys[i] < 0)
                    {
                        energys[i] = MeterProtocolAdapter.Instance.ReadEnergy((byte)(this.PowerFangXiang - 1), (byte)((StPlan_ZouZi)CurPlan).FeiLv, i);
                    }
                    if (energys[i] < 0)
                    {
                        noPassCount++;
                        MessageController.Instance.AddMessage("��λ��" + (i + 1) + "û�ж�������ֵ(" + energys[i] + ")");
                        continue;
                    }
                }

                if (noPassCount >= YaoJianCount)
                {
                    //��ʾ�ֶ��������
                    //WaitInputDl(PowerFangXiang, _curPoint.FeiLv, isStartEnergy);
                }
                if (isStartEnergy)
                {
                    arrayQima = energys;
                }
                else
                {
                    arrayZima = energys;
                }
                string resultName = isStartEnergy ? "����" : "ֹ��";
                ConvertTestResult(resultName, energys);
                UploadTestResult(resultName);
                return true;
            }
            catch (System.Exception e)
            {
                MessageController.Instance.AddMessage(e.Message, 6, 2);
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// д��ʱ��
        /// </summary>
        /// <param name="dateTime">���ڸ�ʽ yymmddhhffss</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        private bool WriteDateTime(string dateTime)
        {
            //string showMsg = GlobalUnit.GetConfig(CLDC_DataCore.Const.Variable.CTC_DGN_WRITEMETERALARM, "��");
            //if (showMsg.Equals("��", StringComparison.OrdinalIgnoreCase))
            //{
            //    System.Windows.Forms.MessageBox.Show("��ȷ�ϴ򿪱����ı�̵Ŀ���", "��ʾ", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            //}
          
            bool bResult = true;
            string[] strRand1 = new string[BwCount];//�����
            string[] strRand2 = new string[BwCount];//���
            string[] strEsamNo = new string[BwCount];//Esam���к�
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] strMeterTime = new string[BwCount];
            string[] strShowData = new string[BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

         //   if (Stop) return;
            MessageController.Instance.AddMessage("��������ʱ��,���Ժ�....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += DateTime.Now.ToString("HHmmss");
                strShowData[i] = DateTime.Now.ToString("yyMMddHHmmss");
                strData[i] = strCode[i] + strSetData[i];
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
   //        MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����ֵ", strShowData);
            for (int i = 0; i < bln_Rst.Length; i++)
            {
                if (!GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn) continue;
                if (bln_Rst[i] == false)
                {
                    bResult = false;
                 //   strMessageText += (i + 1).ToString() + "��,";
                }
            }

            if (!bResult)
            {
            //    strMessageText = strMessageText.Trim(',');
             //   strMessageText += "��λ�޸�ʱ��ʧ�ܣ�����ֹͣ";
         //       MessageController.Instance.AddMessage(strMessageText, 6, 2);
                Stop = true;
            }
            return bResult;
        }

        /// <summary>
        /// �ȴ��ֶ�¼�����
        /// </summary>
        /// <param name="p">Ҫ����Ĺ��ʷ���</param>
        /// <param name="f">Ҫ����ķ���</param>
        //private void WaitInputDl(Cus_PowerFangXiang p, Cus_FeiLv f, bool isQiMa)
        //{
        //    String strDes = String.Format("������{0} {1}����", p.ToString(), f.ToString());

        //    CurActionOver = false;
        //    //�ֶ�¼���������������
        //    MessageController.Instance.AddMessage(strDes);
        //    while (!CurActionOver)
        //    {
        //        Thread.Sleep(GlobalUnit.g_ThreadWaitTime);
        //        if (Stop) return;
        //    }
        //}
        #endregion
        private float[] arrayQima = new float[GlobalUnit.g_CUS.DnbData._Bws];
        private float[] arrayZima = new float[GlobalUnit.g_CUS.DnbData._Bws];
        /// <summary>
        /// ��������[Ĭ��ģʽ]
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            GlobalUnit.g_CurTestType = 3;
            //����ȷ���춨ID
            base.Verify();
            #region ��ʼ������
            MeterBasicInfo _FirstMeter = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);       //��һ����������Ϣ
            Cus_ZouZiMethod _ZZMethod;                                              //�������鷽������׼������ͷ��
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            this.PowerFangXiang = _curPoint.PowerFangXiang;
            //�ѷ���ʱ���ת��Ϊ��
            int _MaxTestTime = (int)(_curPoint.UseMinutes * 60);
            _ZZMethod = _curPoint.ZouZiMethod;
            //����������������
            string[] arrData = new string[0];    //��������
            string strDesc = string.Empty;       //������Ϣ

            m_CheckOver = false;
            //��ȡ���ֵĵ���
            float testI = CLDC_DataCore.Function.Number.GetCurrentByIb(_curPoint.xIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            //��ʼ����صĵ��ܱ���������
            InitZZData(testI.ToString());
            if (Stop)
            {
                return;
            }
            //MessageController.Instance.AddMessage("���ñ���!");
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());
            #endregion

          

            #region //��һ����ѹ,��������,��Ϊ���ֻ���� ��Դ�󣬲��ܽ���ͨѶ
            bool isSc = EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            if (isSc == false)
            {
                Stop = true;
                return;
            }
            if (Stop)
            {
                return;
            }
            #endregion

            #region //�ڶ��� ������ǲ��� �ܷ��ʵĻ�����Ҫ �����ܱ�ĵ�ǰʱ�䣬�ĳ�Ҫ���Եķ��ʵĶ�Ӧʱ��
            if (_curPoint.FeiLv != Cus_FeiLv.��)
            {
                string time = _curPoint.StartTime;
                DateTime dt = DateTime.Now;
                int hh = int.Parse(_curPoint.StartTime.Substring(0, 2));
                int mm = int.Parse(_curPoint.StartTime.Substring(3, 2));
                dt = new DateTime(dt.Year, dt.Month, dt.Day, hh, mm, 0);
                if (this.WriteDateTime(dt.ToString("yyMMddHHmmss")) == false)
                {
                    return;
                }
            }
            #endregion

            #region // ��������������� �ƶ����巨 ��ȡ��ʼ����
            if (Stop)
            {
                return;
            }
            if (_ZZMethod != Cus_ZouZiMethod.�ƶ����巨 && ReadMeterEnergys(true) == false)
            {
                MessageController.Instance.AddMessage("��ȡ��ʼ����ʧ��,����춨����ֹ", 6, 2);
                //Stop = true;
                //return;
            }
            #endregion

            //EquipHelper.Instance.PowerOff();//��ǰ��309��ͻȻ����100AȻ��������ָ���ĵ������������ﲻ��Դ��
            if (Stop)
            {
                return;
            }
            Thread.Sleep(3000);

            #region //���Ĳ�����������ָ���ʼ����
            if (EquipHelper.Instance.InitPara_Constant(_curPoint.PowerFangXiang, null) == false)
            {
                //Stop = true;
                MessageController.Instance.AddMessage("������������ָ��ʧ��", 6, 2);
                //return;
            }
            #endregion

            #region //���岽�������ֵ���
            if (Stop)
            {
                return;
            }
            if (EquipHelper.Instance.PowerOn(GlobalUnit.U, testI, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false) == false)
            {
                //Stop = true;
                //return;
            }

            #endregion

            #region ��ʾ�γ�ͨѶģ��
            System.Windows.Forms.MessageBox.Show("��γ�ͨѶģ�����ȷ����");
            #endregion

            #region //���߲������� ִ�в���
            string stroutmessage = string.Empty;        //�ⷢ��Ϣ
            DateTime startTime = DateTime.Now.AddSeconds(-5);   //�춨��ʼʱ��,����Դ�ȴ�ʱ��
            _StarandMeterDl = 0;                        //��׼�����
            DateTime lastTime = DateTime.Now.AddSeconds(-5);
            //_ZZMethod = Cus_ZouZiMethod.�ƶ����巨;

            while (true)
            {
                Thread.Sleep(1000);
                if (Stop) return;
                int _PastTime = DateTimes.DateDiff(startTime);
                //�������
                if (_PastTime < 0) _PastTime += 43200;      //�����ǰPCΪ12Сʱ��
                if (_PastTime < 0) _PastTime += 43200;      //24Сʱ��
                if (_PastTime < 0)
                {
                    //������Ϊ�޸�ʱ��
                    //��ȳ���24Сʱ���϶���ϵͳ���ڱ��޸�
                    MessageController.Instance.AddMessage("ϵͳʱ�䱻��Ϊ�޸ĳ���24Сʱ���춨ֹͣ");
                    Stop = true;
                    return;
                }

                if (_ZZMethod == Cus_ZouZiMethod.�ƶ����巨)
                {
                    #region
                    if (!GlobalUnit.IsDemo)
                    {
                        if (arrData.Length < BwCount)
                        {
                            arrData = new string[BwCount];
                        }
                        stError[] errors = EquipHelper.Instance.ReadWcb(true);
                        for (int i = 0; i < errors.Length; i++)
                        {
                            arrData[i] = errors[i].szError;
                        }
                    }
                    else
                    {
                  //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
                    }
                    //�ȴ����б�����ָ��������
                    bool bOver = true;
                    for (int i = 0; i < BwCount; i++)
                    {
                        MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                        if (!_meterInfo.YaoJianYn)
                            continue;
                        if (arrData[i] == null || arrData[i].Length == 0 || int.Parse(arrData[i]) < (int)_curPoint.UseMinutes)
                        {
                            bOver = false;
                            break;
                        }

                    }
                    m_CheckOver = bOver;
                    GlobalUnit.g_CUS.DnbData.NowMinute = float.Parse(arrData[GlobalUnit.FirstYaoJianMeter]);
                    //�ⷢ�춨��Ϣ
                    stroutmessage = string.Format("�����������壺{0}������һҪ���λ�Ѿ��յ���{1}��", _curPoint.UseMinutes, arrData[GlobalUnit.FirstYaoJianMeter]);
                    #endregion
                }
                else if (_ZZMethod == Cus_ZouZiMethod.��׼�� || _ZZMethod == Cus_ZouZiMethod.У�˳���)
                {
                    #region
                    if (!GlobalUnit.IsDemo)
                    {
                        //��¼��׼�����
                        float pSum = 0;
                        if (IsYouGong)
                        {
                            //if (GlobalUnit.g_StrandMeterP[0] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
                        }
                        else
                        {
                            //if (GlobalUnit.g_StrandMeterP[1] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[1]);
                        }

                        float pastSecond = (float)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000/1000;
                        //ͬ����¼������������
                        if (arrData.Length < BwCount)
                        {
                            arrData = new string[BwCount];
                        }
                        stError[] errors = EquipHelper.Instance.ReadWcb(true);
                        for (int i = 0; i < errors.Length; i++)
                        {
                            arrData[i] = errors[i].szError;
                        }
                        //����һ�ε���
                        pastSecond = (int)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000/1000;
                    }
                    else
                    {
                        //ģ�����
                        _StarandMeterDl = _PastTime * GlobalUnit.U * testI / 3600000F;
                        //ͬ��ģ��������
                    }
                    //��������ﵽ�趨��ֹͣ
                    if (_StarandMeterDl >= _curPoint.UseMinutes - 0.002)
                    {
                        m_CheckOver = true;
                    }
                    //����������ﵽ�趨��Ҳֹͣ
                    float flt_C = 0;
                    if (arrData != null && arrData.Length > 0)
                    {
                        float.TryParse(arrData[GlobalUnit.FirstYaoJianMeter], out flt_C);
                    }
                    flt_C = flt_C / Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).GetBcs()[0];
                    if (flt_C >= _curPoint.UseMinutes - 0.002)
                    {
                        m_CheckOver = true;
                    }
                    //�ⷢ�춨��Ϣ
                    GlobalUnit.g_CUS.DnbData.NowMinute = _StarandMeterDl;
                    stroutmessage = string.Format("�����������ֵ�����{0}�ȣ��Ѿ����֣�{1}��", _curPoint.UseMinutes, _StarandMeterDl.ToString("F5"));
                    #endregion
                }
                else
                {
                    #region
                    //�������鷨
                    if (_PastTime >= _MaxTestTime)
                    {
                        m_CheckOver = true;
                    }
                    GlobalUnit.g_CUS.DnbData.NowMinute = _PastTime / 60F;
                    stroutmessage = string.Format("������������ʱ�䣺{0}�֣��Ѿ����֣�{1}��", _curPoint.UseMinutes, Math.Round(GlobalUnit.g_CUS.DnbData.NowMinute, 2));
                    #endregion
                }
                #region ��������
                //��������
                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_meterInfo.YaoJianYn)
                    {
                        continue;
                    }

                    //"������", "��׼������"
                    if (arrData != null && arrData.Length > i)
                    {
                        ResultDictionary["������"][i] = arrData[i];
                    }
                    ResultDictionary["��׼������"][i] = ((_StarandMeterDl * _meterInfo.GetBcs()[0])).ToString("F2");
                }
                UploadTestResult("������");
                UploadTestResult("��׼������");
                MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join(",0|", arrData));
                #endregion
                MessageController.Instance.AddMessage(stroutmessage);
                if (m_CheckOver)
                {
                    break;
                }
            }
            #endregion

            #region //�ڰ˲���ѹ,��������,��Ϊ���ֻ���� ��Դ�󣬲��ܽ���ͨѶ
            isSc = EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            if (isSc == false)
            {
                Stop = true;
                return;
            }
            #endregion


            #region //�ھŲ� ����ֹʱ�ĵ�����������
            //if (_ZZMethod == Cus_ZouZiMethod.У�˳���)
            {
                if (!GlobalUnit.IsDemo)
                {
                    if (arrData.Length < BwCount)
                    {
                        arrData = new string[BwCount];
                    }
                    stError[] errors = EquipHelper.Instance.ReadWcb(true);
                    for (int i = 0; i < errors.Length; i++)
                    {
                        arrData[i] = errors[i].szError;
                    }
                }
                else
                {
              //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
                }
            }
            if (Stop)
            {
                return;
            }
            if (_ZZMethod != Cus_ZouZiMethod.�ƶ����巨)
            {
                if (ReadMeterEnergys(false) == false)
                {
                    MessageController.Instance.AddMessage("��ȡ��ֹ�����ʧ��", 6, 2);
                    Stop = true;
                    return;
                }
            }

            //��������
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                if (!_meterInfo.YaoJianYn)
                {
                    continue;
                }
                //"������", "��׼������"
                if (arrData != null && arrData.Length > i)
                {
                    ResultDictionary["������"][i] = arrData[i];
                }
                float flt_QZC = arrayZima[i] - arrayQima[i];
                ResultDictionary["�����"][i] = ((flt_QZC * _meterInfo.GetBcs()[0])).ToString("F2");
            }
            UploadTestResult("������");
            UploadTestResult("�����");
            MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join(",0|", arrData));
            #endregion

            #region //��ʮ���� ���ʱ��ĳ� �������ǰʱ��
            if (_curPoint.FeiLv != Cus_FeiLv.��)
            {
                MessageController.Instance.AddMessage("�����޸ı�ʱ��Ϊ��ǰʱ��..");
                this.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            }
            #endregion

            #region ////��ʮһ����Դ,��һ�������춨���ദ��
            //isSc = EquipHelper.Instance.PowerOn(0, 0, _curPoint.PowerYj, _curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            //if (isSc == false)
            //{
            //    Stop = true;
            //    return;
            //}
            #endregion

            #region //��ʮ�������������
            try
            {
                ControlZZResult(_curPoint, _ZZMethod, arrData, ItemKey);
                ControlResult();
            }
            catch
            {
                Stop = true;
                MessageController.Instance.AddMessage("�����������ʱ���ִ���", 6, 2);
            }
            #endregion
        }

        #region �ڲ�����
        /// <summary>
        /// ���� ���ֽ��
        /// </summary>
        /// <param name="_curPoint">��ǰ�춨��</param>
        /// <param name="_ZZMethod">���ַ�ʽ</param>
        /// <param name="arrData"></param>
        /// <param name="strKey"></param>
        private void ControlZZResult(StPlan_ZouZi _curPoint, Cus_ZouZiMethod _ZZMethod,
            string[] arrData, string strKey)
        {
            MeterBasicInfo _Meter = null;                                           //��ǰ��춨������Ϣ
            bool isAllHeGe = true;                                                 //�Ƿ����б��ϸ�
            MessageController.Instance.AddMessage("���ڼ������ֽ��");
            CLDC_DataCore.Struct.StWuChaDeal zzWCPata = new CLDC_DataCore.Struct.StWuChaDeal();

            #region ---------- ����ͷ����� -----------
            if (_curPoint.ZouZiMethod == Cus_ZouZiMethod.�������ַ�)
            {
                CLDC_Comm.Win32Api.POINT _TouBiao = GetTouBiao(ItemKey);

                _StarandMeterDl = Helper.MeterDataHelper.Instance.Meter(_TouBiao.X).MeterZZErrors[ItemKey].Mz_chrZiMa -
                                Helper.MeterDataHelper.Instance.Meter(_TouBiao.X).MeterZZErrors[ItemKey].Mz_chrQiMa;
                _StarandMeterDl += Helper.MeterDataHelper.Instance.Meter(_TouBiao.Y).MeterZZErrors[ItemKey].Mz_chrZiMa -
                                Helper.MeterDataHelper.Instance.Meter(_TouBiao.Y).MeterZZErrors[ItemKey].Mz_chrQiMa;
                _StarandMeterDl = _StarandMeterDl / 2F;
            }
            #endregion

            for (int r = 0; r < BwCount; r++)
            {
                _Meter = Helper.MeterDataHelper.Instance.Meter(r);
                if (_Meter == null || !_Meter.YaoJianYn)
                {
                    continue;
                }
                //������һ�����������ڱ�����㷵�ؽ��
                MeterZZError curResult = null;
                float _MeterLevel = MeterLevel(_Meter);                   //��ǰ��ĵȼ�
                //���ü������
                zzWCPata.IsBiaoZunBiao = false;
                zzWCPata.MaxError = _MeterLevel;
                CLDC_DataCore.WuChaDeal.WuChaContext m_Context = null;

                if (_ZZMethod == Cus_ZouZiMethod.�ƶ����巨)
                {
                    zzWCPata.MinError = -1;
                    zzWCPata.MaxError = 1;
                    m_Context = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.�������֮�ƶ����巨, zzWCPata);
                    curResult = (MeterZZError)m_Context.GetResult(float.Parse(arrData[r]));
                }
                else
                {
                    if (_curPoint.FeiLv == Cus_FeiLv.��)
                    {
                        StPlan_WcPoint curWcPoint = new StPlan_WcPoint();
                        if (curWcPoint.LapCount == 0)
                        {
                            //ͨ�����ص�ļ춨Ȧ��ȷ���Ƿ��д˻�������
                            zzWCPata.MinError = -_MeterLevel * 1.0F;
                            zzWCPata.MaxError = _MeterLevel * 1.0F;
                        }
                        else
                        {
                            /*�ֶ����������*/
                            curWcPoint.SetWcx(GlobalUnit.g_CUS.DnbData.CzWcLimit,
                                                _Meter.GuiChengName, _Meter.Mb_chrBdj,
                                                _Meter.Mb_BlnHgq);
                            /*
                             2009-7-29
                             * �����ڿ������
                             */
                            zzWCPata.MaxError = curWcPoint.ErrorShangXian * GlobalUnit.g_CUS.DnbData.WcxUpPercent;
                            zzWCPata.MinError = -(Math.Abs(curWcPoint.ErrorXiaXian * GlobalUnit.g_CUS.DnbData.WcxDownPercent));
                        }
                    }
                    else
                    {
                        zzWCPata.MinError = -2;
                        zzWCPata.MeterLevel = 2;
                    }
                    m_Context = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.�������֮��׼��, zzWCPata);
                    //if (_ZZMethod == Cus_ZouZiMethod.��׼��)//&& ϵͳ�����˱�׼��
                    //{
                    //    curResult = (MeterZZError)m_Context.GetResult(_curPoint.FeiLv == Cus_FeiLv.�� ? 1L : 0L, _ZZError.Mz_chrQiMa, _ZZError.Mz_chrZiMa, _StarandMeterDl, 0.05F);
                    //}
                    //else
                    {
                        float flt_C = 0;
                        float.TryParse(arrData[r], out flt_C);
                        flt_C = flt_C / _Meter.GetBcs()[0];
                        curResult = (MeterZZError)m_Context.GetResult(_curPoint.FeiLv == Cus_FeiLv.�� ? 1L : 0L, arrayQima[r], arrayZima[r], flt_C, 0.0F);           //Ҫ���ϱ�׼��ĵȼ�������ط���0Ӧ���滻����������Ҫ��ϵͳ���õ���ʽ�����ñ�׼��ȼ�
                    }
                }

                if (arrayQima[r] == -1.0 || arrayZima[r] == -1.0)
                {
                    curResult.Mz_chrQiMa = arrayQima[r];
                    curResult.Mz_chrZiMa = arrayZima[r];
                    curResult.Mz_chrWc = "-999.00";
                    curResult.Mz_chrJL = "���ϸ�";
                }
                ResultDictionary["�����"][r] = curResult.Mz_chrQiZiMaC;
                ResultDictionary["���"][r] = curResult.Mz_chrWc;
                ResultDictionary["����"][r] = curResult.Mz_chrJL;
                if (ResultDictionary["����"][r] == Variable.CTG_BuHeGe && isAllHeGe)
                {
                    isAllHeGe = false;
                }
            }
            UploadTestResult("�����");
            UploadTestResult("���");
            UploadTestResult("����");
            /*
               01/22/2010 16-07-21  By Niaoked
               ����˵�������Ӳ��ϸ��ֶ���������
           */
            //if (!isAllHeGe)
            //{
            //MessageController.Instance.AddMessage(
            //    string.Format("�е��ܱ���{0}�в��ϸ���������������������¼���������[¼�����]��ť\r\n�������Ҫ��������ֱ�ӵ��[¼�����]��ť", _curPoint.ToString()), false, Cus_MessageType.¼���������);
            //this.WaitInputDl(_curPoint.PowerFangXiang, _curPoint.FeiLv, true);
            //ControlZZResult(_curPoint, _ZZMethod, arrData, strKey);
            //}
            /* Modify End */
            Stop = true;
        }

        /// <summary>
        /// �����ܵĽ���
        /// </summary>
        protected void ControlResult()
        {
            Cus_MeterResultPrjID curItemID = Cus_MeterResultPrjID.��������;

            if (!(CurPlan is StPlan_ZouZi)) return;
            StPlan_ZouZi curPoint = (StPlan_ZouZi)CurPlan;

            //��ǰ�춨�����ܽ��۽ڵ�����
            string curItemKey = ((int)curItemID).ToString("D3") + ((int)PowerFangXiang).ToString();
            for (int bw = 0; bw < BwCount; bw++)
            {
                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(bw);
                if (!curMeter.YaoJianYn) continue;
                MeterResult curResult = new MeterResult();
                if (curMeter.MeterResults.ContainsKey(curItemKey))
                    curMeter.MeterResults.Remove(curItemKey);
                curMeter.MeterResults.Add(curItemKey, curResult);
                curResult.Mr_chrRstId = curItemKey;
                curResult.Mr_chrRstName = curItemID.ToString() + PowerFangXiang.ToString();
                curResult.Mr_chrRstValue = Variable.CTG_HeGe;
                if (curMeter.MeterZZErrors[ItemKey].Mz_chrJL == Variable.CTG_BuHeGe)
                    curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                else
                {
                    //��⵱ǰ�����µ��������Ƿ�ϸ�
                    if (!isTheSamePowerPDHeGe(curMeter))
                        curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                }
            }
        }

        /// <summary>
        /// �Ƿ���ͬ�����µ����е�ǰ�춨��Ŀ���ϸ�
        /// </summary>
        /// <param name="curMeter"></param>
        /// <returns></returns>
        private bool isTheSamePowerPDHeGe(MeterBasicInfo curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.MeterZZErrors.Keys)
            {
                //��ǰ���ʷ���
                if (curMeter.MeterZZErrors[strKey].Mz_chrJL == Variable.CTG_BuHeGe)
                {
                    Cus_PowerFangXiang thisPointFX = (Cus_PowerFangXiang)(int.Parse(curMeter.MeterZZErrors[strKey].Me_chrProjectNo.Substring(0, 1)));
                    StPlan_ZouZi curResultItem = (StPlan_ZouZi)CurPlan;
                    if (curResultItem.PowerFangXiang == thisPointFX)
                    {
                        isAllItemOk = false;
                        break;
                    }
                }
            }
            return isAllItemOk;
        }

        /// <summary>
        /// ��ⷽ���Ƿ�Ϸ�
        /// ��ʽ:���ʷ���|����Ԫ��|��������|��������|�������鷽������|����|���ֵ���(��)|����ʱ��(����)
        /// Ĭ��ֵ:�����й�|H|1.0|3.0Ib|��׼��|��|1|0
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] _Prams = VerifyPara.Split('|');
            if (_Prams.Length < 8) return false;
            StPlan_ZouZi curPoint = new StPlan_ZouZi();
            curPoint.FeiLv = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]);
            curPoint.FeiLvString = _Prams[5];
            curPoint.Glys = _Prams[2];
            curPoint.PowerFangXiang = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), _Prams[0]);
            curPoint.PowerYj = (Cus_PowerYuanJian)Enum.Parse(typeof(Cus_PowerYuanJian), _Prams[1]);
            curPoint.xIb = _Prams[3];
            curPoint.ZouZiMethod = (Cus_ZouZiMethod)Enum.Parse(typeof(Cus_ZouZiMethod), _Prams[4]);
            string dufen = _Prams[6] + "��";
            curPoint.UseMinutes = float.Parse(_Prams[6]);
            if (_Prams[7].Trim() != "0")
            {
                curPoint.UseMinutes = float.Parse(_Prams[7]);
                dufen = _Prams[7] + "��";
            }
            curPoint.ZouZiPrj = new List<StPlan_ZouZi.StPrjFellv>() { 
                new StPlan_ZouZi.StPrjFellv()
                {
                    FeiLv= (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]),
                    StartTime="",
                    ZouZiTime=dufen
                }
            };
            curPoint.ZuHeWc = "0";
            
            CurPlan = curPoint;

            bool Result = true;
            TestType1 testMethod = TestType1.Ĭ��;
            string[] powerDirect = new string[1];
            //ȡ��ǰ�춨�����е����й��ʷ���
            powerDirect[0] = ((int)((Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), _Prams[0]))).ToString();
            //���ÿһ���춨�����µķ���
            for (int i = 0; i < powerDirect.Length; i++)
            {
                string[] feilv = new string[1];

                int zNum = 0;
                //ȡ��ǰ���ʷ����µķ���ʱ��
                feilv[0] = ((int)((Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]))).ToString();
                //�����ַ�ʽΪ�����ͬʱ��ʱ��Ҫ��ÿһ������ֻ��һ�����ҵ�һ�����ʱ���Ϊ�ܣ�
                if (testMethod == TestType1.����ַ���ͬʱ��)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.��).ToString())
                    {
                        MessageController.Instance.AddMessage("�����ַ�ʽΪ[" + testMethod.ToString() + "]ʱ����һ���������鷽������Ϊ[��]", 6, 2);
                        Result = false;
                        break;
                    }

                    for (int k = 0; k < feilv.Length; k++)
                    {

                        if (feilv[k] == ((int)Cus_FeiLv.��).ToString())
                            zNum++;
                        if (zNum > 1)
                        {
                            MessageController.Instance.AddMessage("�����ַ�ʽΪ[" + testMethod.ToString() + "]ʱ��ÿһ�����ʷ����������ҽ�������һ���ܷ��ʷ��򷽰�", 6, 2);
                            return false;
                        }
                    }
                }
                else if (testMethod == TestType1.�Զ��춨��ʱ���ڵ����зַ���)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.��).ToString()) //��һ����Ϊ��
                    {
                        Result = false;
                        break;
                    }
                }
                else
                {
                    Result = true;
                }
            }
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            if (_curPoint.ZouZiMethod == Cus_ZouZiMethod.�������ַ� && Helper.MeterDataHelper.Instance.YaoJianMeterCount < 3)
            {
                MessageController.Instance.AddMessage("�������ַ�����Ҫ�����������ϱ����!", 6, 2);
                return false;
            }

            ResultNames = new string[] { "����", "ֹ��", "�����", "������", "��׼������", "���", "����", "���ϸ�ԭ��" };

            return Result;
        }

        /// <summary>
        /// ͨ���Ƚϵ�ǰ�����µ���ȡǰ���������С�ı���Ϊͷ��
        /// </summary>
        /// <param name="ZZPrjID">���ַ���PrjID</param>
        /// <returns>��һ:Point.x;���:Point.y</returns>
        private CLDC_Comm.Win32Api.POINT GetTouBiao(string ZZPrjID)
        {
            /*
             ���ID:������+���ʷ���+Ԫ��+��������+��������+г��+����
             * ����ID�����ʷ���+Ԫ��+��������+��������+����
             */
            CLDC_Comm.Win32Api.POINT _TouBiao = new CLDC_Comm.Win32Api.POINT();
            string _Key = "1" + ZZPrjID.Substring(0, ZZPrjID.Length - 1);
            float[] wc = new float[BwCount];

            for (int k = 0; k < BwCount; k++)
            {
                //if (Helper.MeterDataHelper.Instance.Meter(k).MeterErrors.ContainsKey(_Key))
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    //���춨�ı�������
                    wc[k] = 999999 + k;
                    continue;
                }
                bool bFind = false;
                foreach (String strKey in Helper.MeterDataHelper.Instance.Meter(k).MeterErrors.Keys)
                {
                    MeterError mError = Helper.MeterDataHelper.Instance.Meter(k).MeterErrors[strKey];
                    if (mError.Me_chrProjectNo.Substring(0, _Key.Length) == _Key && mError.Me_chrWcJl == Variable.CTG_HeGe)
                    {
                        string[] _wc = mError.MeWc.Split('|');
                        //���ĺ�3λ����������λ,��������������ܹ�����������λ
                        wc[k] = float.Parse(_wc[_wc.Length - 1] + (k + 1).ToString("D3"));
                        bFind = true;
                        break;
                    }
                }
                //û���ҵ��˻�������
                if (!bFind)
                    wc[k] = float.Parse(999 + (k + 1).ToString("D3")); ;
            }
            //��˳��
            CLDC_DataCore.Function.Number.PopDesc(ref wc, true);
            string Meter1, Meter2;
            Meter1 = wc[0].ToString();
            Meter2 = wc[1].ToString();

            Meter1 = Meter1.Substring(Meter1.Length - 3, 3);
            Meter2 = Meter2.Substring(Meter2.Length - 3, 3);
            /*
            	10/09/2009 10-43-23  By Niaoked
            	����˵����
            	�������ͷ���������999,��Ϊ���ϱ����Ҫ�춨�ı���
             * 
            */
            //if (wc[0] > 999F || wc[1] > 999F)
            // {
            //     _TouBiao.X = -1;
            //     _TouBiao.Y = -1;
            // }
            // else
            // {
            _TouBiao.X = int.Parse(Meter1) - 1;
            _TouBiao.Y = int.Parse(Meter2) - 1;
            // }
            return _TouBiao;
        }

        /// <summary>
        /// �Ҽ춨���ݽڵ�
        /// </summary>
        /// <param name="curPoint">��ǰ�춨�㷽��</param>
        /// <param name="I">���ֵ���</param>
        /// <returns></returns>
        private void InitZZData(string I)
        {
            MeterBasicInfo curMeter = null;
            MeterZZError zzError = null;
            StPlan_ZouZi curPoint = (StPlan_ZouZi)CurPlan;
            for (int i = 0; i < BwCount; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMeter.MeterZZErrors.ContainsKey(ItemKey))
                    curMeter.MeterZZErrors.Remove(ItemKey);
                if (curMeter.YaoJianYn)
                {
                    zzError = new MeterZZError();
                    zzError.Mz_chrxIbString = curPoint.xIb;
                    zzError.Mz_chrJdfx = (Convert.ToInt32(curPoint.PowerFangXiang)).ToString();

                    switch (curPoint.FeiLv)
                    {
                        case Cus_FeiLv.��:
                            zzError.Mz_chrFl = "��";
                            break;
                        case Cus_FeiLv.��:
                            zzError.Mz_chrFl = "��";
                            break;
                        case Cus_FeiLv.ƽ:
                            zzError.Mz_chrFl = "ƽ";
                            break;
                        case Cus_FeiLv.��:
                            zzError.Mz_chrFl = "��";
                            break;
                        default:
                            zzError.Mz_chrFl = "��";
                            break;
                    }
                    zzError.Mz_chrGlys = curPoint.Glys;
                    zzError.Mz_chrxIb = I;
                    zzError.Mz_chrYj = (Convert.ToInt32(curPoint.PowerYj)).ToString();

                    zzError.Mz_chrWc = "--";
                    zzError.Mz_chrJL = Variable.CTG_DEFAULTRESULT;
                    zzError.AVR_STANDARD_METER_ENERGY = curPoint.UseMinutes.ToString();
                    zzError.Me_chrProjectNo = curPoint.PrjID;
                    zzError.Mz_chrStartTime = curPoint.StartTime;
                    curMeter.MeterZZErrors.Add(ItemKey, zzError);
                }
            }
            MessageController.Instance.AddMessage("������һ�μ춨�������...");
        }

        #endregion

      
    }
}
