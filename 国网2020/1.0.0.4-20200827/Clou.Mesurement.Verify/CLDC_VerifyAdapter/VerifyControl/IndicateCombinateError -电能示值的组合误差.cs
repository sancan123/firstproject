using System;
using CLDC_DataCore;
using System.Data;
using CLDC_Comm;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using System.Collections.Generic;
using System.Threading;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// ����ʾֵ��������
    /// </summary>
    class IndicateCombinateError: VerifyBase
    {
        public IndicateCombinateError(object plan)
            : base(plan)
               
        {
            ResultNames = new string[] { "����ǰ���ʵ���", "����ǰ�ܵ���", "�������ʵ���", "������ܵ���","���", "����" };
          
        
        
        }
        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        public override void Verify()
        {
            float[] flt_DLFlq =new  float[BwCount];
            float[] flt_DLY = new float[BwCount];
            float[] flt_DLFlh = new float[BwCount];
            float[] flt_DLYh = new float[BwCount];
            Stop = false;
            int _MaxTestTime = 60;
            string stroutmessage = "";
            bool bResult = PowerOn();
            Check.Require(bResult, "����Դ���ʧ��");
            base.Verify();

            string curSD = string.Empty;
            string[] arrPara = VerifyPara.Split('|');
            string[] arrSD = arrPara[0].Split(',');
            _MaxTestTime = int.Parse(arrPara[1]) * _MaxTestTime;
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
            string[] arrStrResultKey = new string[BwCount];
            string[] arrSumKey = new string[BwCount];//��ǰ�����ϱ�key
            string[] arrCurKey = new string[BwCount];//��ǰ������ϱ�key
            string totalKey = ResultKey;// CurPlan.DgnPrjID;//((int)Cus_DgnItem.����ʱ��ʾֵ���).ToString("D3");//�ܽ��۽ڵ�����
            PowerFangXiang = Cus_PowerFangXiang.�����й�; //getCurPowerFangXiang();//��ǰ�춨���ʷ���
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Thread.Sleep(1000);
            //��ÿһ��ʱ�ν�������
            for (int k = 0; k < arrSD.Length; k++)
            {
                curSD = arrSD[k];
                string[] para = curSD.Split('(');
                if (para.Length != 2)
                    continue;
                MessageController.Instance.AddMessage("������" + curSD );

                string sdTime = DateTime.Parse(para[0]).ToString("yyMMddHHmmss");
                //Comm.MessageController.Instance.AddMessage("��ʼ" + sdTime, false);
                //System.Windows.Forms.MessageBox.Show("��ȷ�ϴ򿪵���̼�", "��ʾ", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                //����ʱ��
                Thread.Sleep(1000);
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = sdTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += sdTime.Substring(6, 6);
                    strShowData[i] = sdTime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                //result = MeterProtocolAdapter.Instance.WriteDateTime(sdTime);
                bResult = true;
                for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
                {
                    if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                    {
                        if (!result[i])
                            bResult = false;
                    }
                }
                Thread.Sleep(3000);
                //����
                Cus_FeiLv curTri = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), para[1].Replace(")", ""));
                /*
                    09/25/2009 12-12-45  By Niaoked
                    ����˵����
                    ע�������ȡ�����λ�ã������¼���ͻ
                */


              
                MeterBasicInfo curMeter;
                stRatePeriodData[] tagRatePeriodData = new stRatePeriodData[BwCount];


                MessageController.Instance.AddMessage("������...");

                string strFX = "00";
                string StrDNZ = "00000000";
                switch (PowerFangXiang)
                {
                    case Cus_PowerFangXiang.�����й�:
                        strFX = "01";
                        StrDNZ = "00010000";
                        break;
                    case Cus_PowerFangXiang.�����й�:
                        strFX = "02";
                        StrDNZ = "00020000";
                            break;
                    case Cus_PowerFangXiang.�����޹�:
                        strFX = "03";
                        StrDNZ = "00030000";
                        break;
                    case Cus_PowerFangXiang.�����޹�:
                        strFX = "04";
                        StrDNZ = "00040000";
                        break;
                }
                string StrFL = "";

                switch (curTri)
                {
                    case Cus_FeiLv.��:
                        StrFL = "01";
                        break;
                    case Cus_FeiLv.��:
                        StrFL = "02";
                        break;
                    case Cus_FeiLv.ƽ:
                        StrFL = "03";
                        break;
                    case Cus_FeiLv.��:
                        StrFL = "04";
                        break;
                }

                string StrFLDN = "00" + strFX + StrFL + "00";
                if (Stop)
                    break;
                MessageController.Instance.AddMessage("��ȡ����ǰ����");
                flt_DLFlq = MeterProtocolAdapter.Instance.ReadData(StrFLDN, 4, 2);
                flt_DLY = MeterProtocolAdapter.Instance.ReadData(StrDNZ, 4, 2);
               
                Thread.Sleep(1000);
                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //�ѷ������ʱ��ת������������˳���
                  
                    if (!curMeter.YaoJianYn) continue;
                 
                 
                    #region----------�ܼ춨���----------
                  
                 
                    #endregion



                    ResultDictionary["����ǰ���ʵ���"][iNum] = flt_DLFlq[iNum].ToString() + "(" + curTri.ToString() + ")";
                    ResultDictionary["����ǰ�ܵ���"][iNum] = flt_DLY[iNum].ToString() + "(��)";
                         //��ȡ������ϣ�ˢ��һ������


                }
                UploadTestResult("����ǰ���ʵ���");
                UploadTestResult("����ǰ�ܵ���");
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, "1", true, false) == false)
                {
                    MessageController.Instance.AddMessage("����Դ���ʧ��");
                    return;
                }
                Thread.Sleep(300);
                DateTime startTime = DateTime.Now;

                m_CheckOver = false;
                while (true)
                {
                    Thread.Sleep(5000);
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


                    if (_PastTime >= _MaxTestTime)
                    {
                        m_CheckOver = true;
                    }
                    GlobalUnit.g_CUS.DnbData.NowMinute = _PastTime / 60F;
                    stroutmessage = string.Format("���õ�ǰ�������У�{0}�֣��Ѿ����֣�{1}��", _MaxTestTime / 60, Math.Round(GlobalUnit.g_CUS.DnbData.NowMinute, 2));

                    MessageController.Instance.AddMessage(stroutmessage);
                    if (m_CheckOver)
                    {
                        break;
                    }
                }
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.�����й�, "1", true, false) == false)
                {
                    MessageController.Instance.AddMessage("����Դ���ʧ��");
                    return;
                }
                Thread.Sleep(5000);
                MessageController.Instance.AddMessage("��ȡ��������");
                flt_DLFlh = MeterProtocolAdapter.Instance.ReadData(StrFLDN, 4, 2);
                flt_DLYh = MeterProtocolAdapter.Instance.ReadData(StrDNZ, 4, 2);

                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //�ѷ������ʱ��ת������������˳���
                   
                    if (!curMeter.YaoJianYn) continue;



                    ResultDictionary["�������ʵ���"][iNum] = flt_DLFlh[iNum].ToString() + "(" + curTri.ToString() + ")";
                    ResultDictionary["������ܵ���"][iNum] = flt_DLYh[iNum].ToString() + "(��)";

                    float fErr = -999;
                    if (flt_DLFlh[iNum] == -1 || flt_DLYh[iNum] == -1 || flt_DLY[iNum] == -1 || flt_DLYh[iNum] == -1)
                    {
                        fErr = -999;
                    }
                    else if ((flt_DLFlh[iNum] - flt_DLFlq[iNum]) == 0)
                    {
                        fErr = 1;
                    }
                    else
                    {
                        fErr = (flt_DLFlh[iNum] - flt_DLFlq[iNum]) - (flt_DLYh[iNum] - flt_DLY[iNum]);
                    }

                 
                    ResultDictionary["���"][iNum] =  Math.Abs(fErr).ToString("0.0000") ;
                    ResultDictionary["����"][iNum] = "�ϸ�";
                    if (Math.Abs(fErr) > 0.01)
                        ResultDictionary["����"][iNum] = "���ϸ�";
                    //��ȡ������ϣ�ˢ��һ������


                }
                //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrCurKey);
            }
            UploadTestResult("�������ʵ���");
            UploadTestResult("������ܵ���");
            UploadTestResult("���");
            UploadTestResult("����");
            //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
            MessageController.Instance.AddMessage("����д�뵱ǰʱ��");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
        }
      
        /// <summary>
        /// �춨����У��
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {

            //
            // Check.Require(PrjPara.Length > 0, "�춨������û�����ñ����ʱ�Σ��뵽�춨����������");
            //��ȡһ��ʱ��
#if OLD
            //�Ա�ʱ���Ƿ�ͷ���һ��
            string[] strP = new string[0];
            string[] arrScheme = new string[0];
            string strTime = string.Empty;     //����ʱ��
            string strDesc = string.Empty;     //���ʺ�
            string strTmp = string.Empty;      //
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    continue;
                if (!Control485.curReturnStringA.ContainsKey(i.ToString()))
                {
                    MessageController.Instance.AddMessage(string.Format("��[{0}]��λ��ȡʱ��ʧ��!", i + 1), false);
                    continue;
                }
                strP = Control485.curReturnStringA[i.ToString()];
                //�Ա�
                for (int j = 0; j < strP.Length; j++)
                {
                    if (j > PrjPara.Length - 1) break;
                    arrScheme = PrjPara[j].Split('(');
                    Check.Require(arrScheme.Length == 2, "������ʱ�θ�ʽ����");
                    //ʱ����
                    strTime = strP[j].Substring(0, 4); //ȡʱ��
                    strDesc = strP[j].Substring(4, 2); //ȡ���
                    strTmp = arrScheme[0].Replace(":", "");    //�滻�������е�":"
                    Check.Require(strTmp.Equals(strTime), string.Format("������{0}������ʱ���뱻���{1}��һ��", j + 1, i + 1));
                    //��ż��
                    strTmp = arrScheme[1].Replace(")", "");
                    strTmp = SwitchFeiLvNameToOrder(strTmp, i).ToString("00");
                    Check.Require(Comm.Function.Number.IsIntNumber(strTmp), "���ʺű���Ϊ����");
                    Check.Require(strTmp.Equals(strDesc), string.Format("������{0}�����ʺ��뱻���{1}��һ��,�����ʱ��Ϊ��{2}", j + 1, i + 1, strP));
                }


            }
#endif
            return true;
        }



        #region ���ʵ���ʾֵ���ṹ��
        struct stRatePeriodData
        {
            public int BW;              //��λ��            
            public string FL;           //����
            public float fStartDLPeriod;       //�ַ�����ʼ����
            public float fEndDLPeriod;         //�ַ��ʽ�������
            public float fStartDLSum;          //����ʼ����
            public float fEndDLSum;            //�ܽ���
            public override string ToString()
            {
                string str = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", fStartDLPeriod, fEndDLPeriod, fStartDLSum, fEndDLSum, Error(), FL);

                return str;
            }
            /// <summary>
            /// ���ʵ���ʾֵ���
            /// </summary>
            /// <returns></returns>
            public string Error()
            {
                float fError;
                float fErrorPeriod;
                float fErrorSum;

                try
                {
                    fErrorPeriod = fEndDLPeriod - fStartDLPeriod;
                    fErrorSum = fEndDLSum - fStartDLSum;
                    //add by zxr in then nanchang 2014-08-26 �����ֵΪ0�������
                    if (fErrorSum == 0.0f || fErrorPeriod == 0.0f)
                    {
                        fError = 0.000f;
                    }
                    else
                    {
                        fError = (fErrorPeriod - fErrorSum) / fErrorSum;
                    }
                }
                catch
                {
                    return "";
                }
                return fError.ToString("0.000");
            }
        }
        /// <summary>
        /// ��������ת�������ʱ��
        /// </summary>
        /// <param name="FeiLvName"></param>
        /// <param name="bwIndex"></param>
        /// <returns>�����ĵڼ�������</returns>
        protected int SwitchFeiLvNameToOrder(string FeiLvName, int bwIndex)
        {
            if (FeiLvName == "��") return 0;
            if (FeiLvName.Length > 1) return 0;
            string[] arrSourceFL = new string[] { "��", "��", "��", "ƽ", "��" };
            int sourceOrder = 0;
            for (int j = 0; j < arrSourceFL.Length; j++)
            {
                sourceOrder = j;
                if (arrSourceFL[j].Equals(FeiLvName))
                {
                    break;
                }
            }
            //  sourceOrder++;
            return sourceOrder;

            /*
           int returnOrder = 0;
            string meterFeiLvOrder = Helper.MeterDataHelper.Instance.Meter(bwIndex).DgnProtocol.TariffOrderType;
            for (int k = 0; k < meterFeiLvOrder.Length; k++)
            {
                returnOrder = k;
                if (meterFeiLvOrder.Substring(k, 1) == sourceOrder.ToString())
                {
                    break;
                }
            }
            return returnOrder;
        */

        }
        #endregion
    }
}
