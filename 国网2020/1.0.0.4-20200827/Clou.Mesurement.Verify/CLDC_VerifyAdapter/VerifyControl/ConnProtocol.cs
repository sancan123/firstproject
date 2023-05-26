
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// ͨѶЭ��������
    /// ֻ��¼��ǰ�������
    /// </summary>
    class ConnProtocol : VerifyBase
    {
        //���ϵͳ���չ��
        float[] arrCreepTimes = new float[0];

        public ConnProtocol(object plan)
            : base(plan)
        {
            ResultNames = new string[] { "��ʶ����", "��ʶ����", "��ʶ����", "���ݸ�ʽ", "������", "��������", "����" };
        }

        #region ������У��춨����
        /// <summary>
        /// ������У��춨����
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] arrayPara = VerifyPara.Split('|');
            if (arrayPara.Length < 7)
            {
                MessageController.Instance.AddMessage(string.Format("ͨѶЭ�����������,��������Ӧ��С��6,����:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.ConnProtocolItem = arrayPara[0];
            paraStruct.ItemCode = arrayPara[1];
            int lengthTemp = 0;
            if (int.TryParse(arrayPara[2], out lengthTemp))
            {
                paraStruct.DataLen = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("ͨѶЭ�����������,�������������ݱ�ʶ���Ƚ�������,����:{0}", VerifyPara), 6, 2);
                return false;
            }
            if (int.TryParse(arrayPara[3], out lengthTemp))
            {
                paraStruct.PointIndex = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("ͨѶЭ�����������,���ĸ��������ݱ�ʶС��λ����������,����:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.StrDataType = arrayPara[4];
            if (arrayPara[5] == "��")
            {
                paraStruct.OperType = StMeterOperType.��;
            }
            else if (arrayPara[5] == "д")
            {
                paraStruct.OperType = StMeterOperType.д;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("ͨѶЭ�����������,����������ӦΪ'��'��'д',����:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.WriteContent = arrayPara[6];
            return true;
        }
        #endregion

        #region ��ʼ���豸����:InitEquipment
        /// <summary>
        /// ��ʼ���豸����,����ÿһ�����Ҫ�춨��Ȧ��
        /// </summary>
        /// <param name="_curPoint"></param>
        /// <param name="PL"></param>
        /// <param name="_Pulselap"></param>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (GlobalUnit.IsDemo) return true;
            //MessageController.Instance.AddMessage("���ñ���!");
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());

            if (Stop) return false;                   //���統ǰֹͣ�춨�����˳�
            MessageController.Instance.AddMessage("ֹͣ��ǰ���õĹ���!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(true,0);
            if (Stop) return false;                   //���統ǰֹͣ�춨�����˳�
            MessageController.Instance.AddMessage("��ʼ����Դ���!");
            bool result = PowerOn();
            if (!result)
            {
                MessageController.Instance.AddMessage("����Դ���ʧ��", 6, 2);
                return false;
            }
            return true;
        }
        #endregion

        #region ����Դ���
        /// <summary>
        /// ����Դ���
        /// </summary>
        /// <returns>��Դ���</returns>
        protected override bool PowerOn()
        {
            int firstYaoJianMeterIndex = Helper.MeterDataHelper.Instance.FirstYaoJianMeter;
            MeterBasicInfo firstYaoJianMeter = Helper.MeterDataHelper.Instance.Meter(firstYaoJianMeterIndex);
            Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
            //����ǵ��ֻ࣬���AԪ
            if (GlobalUnit.IsDan) ele = Cus_PowerYuanJian.A;
            MessageController.Instance.AddMessage(string.Format("��ʼ���ƹ���Դ���:{0}V {1}A", GlobalUnit.U, 0));
            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U,
                                  0,
                                  (int)ele, (int)PowerFangXiang,
                                  FangXiangStr + "1.0",
                                  true, false);
            MessageController.Instance.AddMessage(string.Format("���ƹ���Դ���:{0}V {1}A {2}", GlobalUnit.U, 0, ret ? "�ɹ�" : "ʧ��"), 6, ret ? 0 : 2);
            return ret;
        }

        #endregion

        #region ----------��ʼ�춨----------
        /// <summary>
        /// ��ʼ�춨
        /// </summary>
        public override void Verify()
        {
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo meterTemp = Helper.MeterDataHelper.Instance.Meter(i);
                if (meterTemp.YaoJianYn)
                {
                    ResultDictionary["��ʶ����"][i] = paraStruct.ConnProtocolItem;
                    ResultDictionary["��ʶ����"][i] = paraStruct.ItemCode;
                    ResultDictionary["��ʶ����"][i] = paraStruct.DataLen.ToString();
                    ResultDictionary["���ݸ�ʽ"][i] = paraStruct.StrDataType;
                    ResultDictionary["��������"][i] = paraStruct.OperType.ToString();
                }
            }
            UploadTestResult("��ʶ����");
            UploadTestResult("��ʶ����");
            UploadTestResult("��ʶ����");
            UploadTestResult("���ݸ�ʽ");
            UploadTestResult("��������");

            base.Verify();                                                                  //���û���ļ춨������ִ��Ĭ�ϲ���

            Helper.LogHelper.Instance.WriteInfo("��ʼ���豸����...");
            Adapter.Instance.UpdateMeterProtocol();
            //��ʼ���豸
            bool resultInitEquip = InitEquipment();
            if (!resultInitEquip)
            {
                MessageController.Instance.AddMessage("��ʼ����������豸����ʧ��", 6, 2);
                return;
            }
            if (Stop) return;                   //���統ǰֹͣ�춨�����˳�

            if (GlobalUnit.IsDemo) return;
            if (Stop) return;                   //���統ǰֹͣ�춨�����˳�
            if (paraStruct.OperType == StMeterOperType.��)
            {
                string[] readdata = new string[BwCount];
                if (paraStruct.PointIndex > 0)
                {
                    float[] tmpdata = MeterProtocolAdapter.Instance.ReadData(paraStruct.ItemCode, paraStruct.DataLen, paraStruct.PointIndex);
                    readdata = CLDC_DataCore.Function.ConvertArray.ConvertFloat2Str(tmpdata);
                }
                else
                {
                    readdata = MeterProtocolAdapter.Instance.ReadData(paraStruct.ItemCode, paraStruct.DataLen);
                }
                //��������
                for (int i = 0; i < BwCount; i++)
                {
                    if (Stop) return;                   //���統ǰֹͣ�춨�����˳�
                    MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_Meter.YaoJianYn) continue;
                    ResultDictionary["������"][i] = readdata[i];
                    ResultDictionary["����"][i] = string.IsNullOrEmpty(readdata[i]) ? "���ϸ�" : "�ϸ�";
                }
                UploadTestResult("������");
                UploadTestResult("����");
            }
            else
            {
                string writedata = FormatWriteData(paraStruct.WriteContent, paraStruct.StrDataType, paraStruct.DataLen, paraStruct.PointIndex);
                bool[] bResult = MeterProtocolAdapter.Instance.WriteData(paraStruct.ItemCode, paraStruct.DataLen, writedata);
                ConvertTestResult("����", bResult);
                UploadTestResult("����");
            }
            
        }
        #endregion

        /// <summary>
        /// ��ʽ�����ַ���
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <returns></returns>
        private string FormatReadData(string data, string strformat)
        {
            if (data == "" || data == null) return "";
            string formatdata = data;
            bool blnIsNum = true;           //�ж϶�ȡ�������ǲ�������
            List<char> splitChar = new List<char>(new char[] { '.', '-', '#', '|', '@' });
            for (int i = 0; i < strformat.Length; i++)
            {
                if (Stop) return "";                   //���統ǰֹͣ�춨�����˳�
                if (splitChar.Contains(strformat[i]))
                {
                    formatdata = formatdata.Insert(i, strformat[i].ToString());
                    blnIsNum = false;
                }
                else if (strformat[i] == 'N')
                {
                    blnIsNum = false;
                }
            }
            if (blnIsNum) formatdata = float.Parse(formatdata).ToString();
            return formatdata;
        }

        /// <summary>
        /// ��ʽ��д�ַ���
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <param name="len"></param>
        /// <param name="pointindex"></param>
        /// <returns></returns>
        private string FormatWriteData(string data, string strformat, int len, int pointindex)
        {
            string formatdata = "";
            try
            {
                if (data == "" || data == null) return "";
                formatdata = data;
                bool blnIsNum = true;           //�ж϶�ȡ�������ǲ�������
                List<char> splitChar = new List<char>(new char[] { '.', 'N' });
                for (int i = 0; i < strformat.Length; i++)
                {
                    if (!splitChar.Contains(strformat[i]))
                    {
                        blnIsNum = false;
                        break;
                    }
                }
                if (pointindex != 0)
                {
                    if (blnIsNum)
                    {
                        int left = len * 2 - pointindex;
                        int right = pointindex;
                        formatdata = float.Parse(formatdata).ToString();
                        string[] newdata = formatdata.Split('.');
                        if (newdata.Length == 1)
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            formatdata = newdata[0] + "".PadRight(right, '0');
                        }
                        else
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            if (newdata[1].Length <= right)
                            {
                                newdata[1] = newdata[1].PadRight(right, '0');
                            }
                            else
                            {
                                newdata[1] = newdata[1].Substring(0, right);
                            }
                            formatdata = newdata[0] + newdata[1];
                        }
                    }
                    else
                    {
                        formatdata = formatdata.Replace(".", "");
                        formatdata = formatdata.Replace("-", "");
                        if (formatdata.Length <= len * 2)
                        {
                            formatdata = formatdata.PadRight(len * 2, '0');
                        }
                        else
                        {
                            formatdata = formatdata.Substring(0, len * 2);
                        }
                    }
                }
                else
                {
                    if (formatdata.Length <= len * 2)
                    {
                        formatdata = formatdata.PadLeft(len * 2, '0');
                    }
                    else
                    {
                        formatdata = formatdata.Substring(0, len * 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.Instance.WriteInfo(ex.StackTrace);
            }
            return formatdata;
        }

        private StPlan_ConnProtocol paraStruct = new StPlan_ConnProtocol();

        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }
    }
}
