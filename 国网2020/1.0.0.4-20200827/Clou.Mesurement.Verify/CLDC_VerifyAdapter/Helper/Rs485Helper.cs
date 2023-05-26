using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using Comm.Const;
using Comm.Enum;
//using ClInterface;
using Comm.Model.DnbModel.DnbInfo;
using Comm.Model.DgnProtocol;
using Comm.BaseClass;

namespace VerifyAdapter.Helper
{
    /// <summary>
    /// RS485���Ƶ�Ԫ
    /// </summary>
    public class Rs485Helper : SingletonBase<Rs485Helper>
    {
        #region ------------��������------------
        /// <summary>
        /// ���ܱ�����ӿ�
        /// </summary>
        private MeterProtocol.IMeterProtocol IMeterController = null;
        /// <summary>
        /// ���б�־
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// ֹͣ��־
        /// </summary>
        private bool isStop = false;

        /// <summary>
        /// �Ѿ��ɹ�������
        /// </summary>
        private int successCount = 0;

        /// <summary>
        /// ����ָ������ȵ�ʱ��
        /// </summary>
        private int maxWaitReturnTime = 45;
        /// <summary>
        /// ���ݷ��ر�־,���ڱ���ÿһ�������ݷ���״̬
        /// </summary>
        private bool[] arrReturnFlag = new bool[0];

        /// <summary>
        /// ��ɱ�־,���ڱ�־ÿһ���������Ƿ񷵻����
        /// </summary>
        private bool[] arrOverFlag = new bool[0];

        /// <summary>
        /// BOOL�ͷ���ֵ
        /// </summary>
        private bool[] arrReturnValue_Bool = new bool[0];

        /// <summary>
        /// �ַ����ͷ���ֵ
        /// </summary>
        private string[] arrReturnValue_String = new string[0];
        /// <summary>
        /// �������͵ķ���ֵ
        /// </summary>
        private Dictionary<int, string[]> dicReturnValue_StringArray = new Dictionary<int, string[]>();

        /// <summary>
        /// �������������ͷ���
        /// </summary>
        private Dictionary<int, float[]> dicReturnValue_FloatArray = new Dictionary<int, float[]>();
        #endregion

        public Rs485Helper()
        {
            maxWaitReturnTime = GlobalUnit.GetConfig(Variable.CTC_DGN_MAXWAITDATABACKTIME, 45);
        }

        #region -----------------����-----------------
        /// <summary>
        /// RS485�Ƿ��Ѿ��ɹ���ʼ��
        /// </summary>
        private bool isInitOk = false;

        private int bwCount = 1;
        /// <summary>
        /// ��λ����
        /// </summary>
        public int BwCount
        {
            get { return bwCount; }
            set { bwCount = value; }
        }

        private int retryTimes = 3;
        /// <summary>
        /// ��ȡ���Դ���
        /// </summary>
        public int RetryTimes
        {
            get { return retryTimes; }
            set { retryTimes = value; }
        }
        #endregion


        /// <summary>
        /// ֹͣ��ǰ����
        /// </summary>
        public void Stop()
        {
            isStop = true;                                              //���б�־����Ϊֹͣ
            while (isRunning)                                           //�ȴ���ǰ�����������
            {
                System.Threading.Thread.Sleep(30);
            }
        }

        /// <summary>
        /// ��ȡָ����λ�Ľ���
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool GetResult(int pos)
        {
            if (pos > -1 && pos < BwCount)
            { 
                return arrReturnValue_Bool[pos]; 
            }
            return false;
        }

        /// <summary>
        /// ��λ
        /// </summary>
        private void Reset()
        {
            arrOverFlag = new bool[BwCount];                                 //��ɱ�־
            arrReturnFlag = new bool[BwCount];                              //���ر�ʶ
            arrReturnValue_Bool = new bool[BwCount];                        //���ؽ���
            arrReturnValue_String = new string[BwCount];                     //�����ַ���
            dicReturnValue_StringArray = new Dictionary<int, string[]>();   //��������
            dicReturnValue_FloatArray = new Dictionary<int, float[]>();
            successCount = 0;
        }

        /// <summary>
        /// ��λָ����λ
        /// </summary>
        /// <param name="pos">ָ����λ��</param>
        private void Reset(int pos)
        {
            if (pos < 0 || pos > arrOverFlag.Length - 1 || pos > arrReturnFlag.Length - 1) return;
            arrOverFlag[pos] = false;
            arrReturnFlag[pos] = false;
            arrReturnValue_Bool[pos] = false;
            arrReturnValue_String[pos] = string.Empty;
            if (dicReturnValue_StringArray.ContainsKey(pos))
                dicReturnValue_StringArray.Remove(pos);
            if (dicReturnValue_FloatArray.ContainsKey(pos))
                dicReturnValue_FloatArray.Remove(pos);
            //successCount--;
        }

        /// <summary>
        /// �Ƿ����н���Ѿ�����
        /// </summary>
        /// <returns></returns>
        internal bool IsAllReturn()
        {
            return successCount == BwCount;
        }

        /// <summary>
        /// ��Ҫ���Եĵ���б�
        /// </summary>
        /// <returns></returns>
        private int[] GetNeedReturnList()
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < BwCount; i++)
            {
                if (arrReturnFlag[i])
                    ret.Add(i);
            }
            return ret.ToArray();
        }

        /// <summary>
        /// �������ݺ�ȴ�����
        /// </summary>
        internal void WaitReturn()
        {
            DateTime dtStartTime = DateTime.Now;
            while (true)
            {
                if (IsAllReturn())            //ȫ������
                    break;
                if (isStop)                 //ֹͣ��־
                    break;
                if (((TimeSpan)(DateTime.Now - dtStartTime)).TotalSeconds > maxWaitReturnTime)
                    break;                  //��ʱ�˳�
            }
        }

        #region ----------------------��ʼ��RS485���----------------------
        /// <summary>
        /// ��ʼ���๦��
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
           // Settings.DgnConfigManager.Instance.Load();
           // MeterProtocolAdapter.Instance.SetBwCount(); 
            return true;
        }

        /// <summary>
        /// �๦���¼�ע��
        /// </summary>
        private void RegisterEvent()
        {
            //�¼���Ӧ
            //IMeterController.OnChangePassword += new DelegateChangePassword(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventBroadcaseTime += new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventChangeSetting += new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearDemand += new DelegateClearDemand(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEnergy += new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEventLog += new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventCommTest += new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventFreezeCmd += new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventReadAddress += new DelegateReadAddress(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadData += new DelegateReadData(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDateTime += new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDemand += new DelegateReadDemand(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadEnergy += new DelegateReadEnergy(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadPeriodTime += new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
            //IMeterController.OnEventSetPulseCom += new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteAddress += new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteData += new DelegateWriteData(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteDateTime += new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWritePeriodTime += new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            ////�����¼�
            //IMeterController.OnEventRxFrame += new Dge_EventRxFrame(IMeterControler_OnEventRxFrame);
            //IMeterController.OnEventTxFrame += new Dge_EventTxFrame(IMeterControler_OnEventTxFrame);

        }

        /// <summary>
        /// ����¼���
        /// </summary>
        public void RealseDgnEvent()
        {

            //IMeterController.OnChangePassword -= new DelegateChangePassword(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventBroadcaseTime -= new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventChangeSetting -= new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearDemand -= new DelegateClearDemand(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEnergy -= new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEventLog -= new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventCommTest -= new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventFreezeCmd -= new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventReadAddress -= new DelegateReadAddress(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadData -= new DelegateReadData(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDateTime -= new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDemand -= new DelegateReadDemand(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadEnergy -= new DelegateReadEnergy(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadPeriodTime -= new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
            //IMeterController.OnEventSetPulseCom -= new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteAddress -= new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteData -= new DelegateWriteData(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteDateTime -= new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWritePeriodTime -= new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            ////�����¼�
            //IMeterController.OnEventRxFrame -= new Dge_EventRxFrame(IMeterControler_OnEventRxFrame);
            //IMeterController.OnEventTxFrame -= new Dge_EventTxFrame(IMeterControler_OnEventTxFrame);

        }
        #endregion

        #region ----------�๦���¼�����----------

        /// <summary>
        /// ֻ���ؽ�����͵��¼�
        /// </summary>
        /// <param name="int_Index">��λ��[0-BW]</param>
        /// <param name="bln_Result">����</param>
        private void IMeterControler_OnEventReturnResult(int int_Index, bool bln_Result)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //�Ѿ��ϸ��ٴ���
            arrReturnValue_Bool[int_Index] = bln_Result;    //��¼�·���ֵ
            arrReturnFlag[int_Index] = true;                //���·��ر�־
            arrOverFlag[int_Index] = true;                  //������ɱ�־
            successCount++;                                 //������ɼ�����
        }

        /// <summary>
        ///  �����ַ���������
        /// </summary>
        /// <param name="int_Index">��λ��[0-BW]</param>
        /// <param name="bln_Result">����</param>
        /// <param name="str_Data">��������</param>
        private void IMeterControler_OnEventReturnString(int int_Index, bool bln_Result, string str_Data)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //�Ѿ��ϸ��ٴ���
            arrReturnValue_String[int_Index] = str_Data;    //��¼�·���ֵ
            arrReturnFlag[int_Index] = true;                //���·��ر�־
            arrOverFlag[int_Index] = bln_Result;            //������ɱ�־
            if (bln_Result) successCount++;                  //������ɼ�����
        }
        /// <summary>
        /// �ַ����������ͷ���
        /// </summary>
        /// <param name="int_Index"></param>
        /// <param name="bln_Result"></param>
        /// <param name="str_Data"></param>
        private void IMeterControler_OnEventReturnStringA(int int_Index, bool bln_Result, string[] str_Data)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //�Ѿ��ϸ��ٴ���
            if (dicReturnValue_StringArray.ContainsKey(int_Index))
                dicReturnValue_StringArray.Remove(int_Index);
            dicReturnValue_StringArray.Add(int_Index, str_Data);
            arrReturnFlag[int_Index] = true;                //���·��ر�־
            arrOverFlag[int_Index] = bln_Result;            //������ɱ�־
            if (bln_Result)
                successCount++;                             //������ɼ�����
        }

        /// <summary>
        ///  //��ȡ����
        /// </summary>
        /// <param name="int_Index">��λ����,0��ʼ</param>
        /// <param name="bln_Result">��ȡ�������</param>
        /// <param name="sng_Energy">��������</param>

        public void IMeterControler_OnEventReturnFloat(int int_Index, bool bln_Result, float[] sng_Energy)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //�Ѿ��ϸ��ٴ���
            if (dicReturnValue_FloatArray.ContainsKey(int_Index))
                dicReturnValue_FloatArray.Remove(int_Index);
            dicReturnValue_FloatArray.Add(int_Index, sng_Energy);
            arrReturnFlag[int_Index] = true;                //���·��ر�־
            arrOverFlag[int_Index] = true;                  //������ɱ�־
            if (bln_Result) successCount++;                                 //������ɼ�����
        }

        #endregion

        #region ------------ͨѶ����------------
        /// <summary>
        /// ͨѶ���ԣ����б�λ��Ч
        /// </summary>
        /// <returns>�����Ƿ�ɹ�</returns>
        public bool CommTest()
        {
            Reset();
            Helper.Rs485CallBackHelper.CallBack callBack = IMeterController.ComTest;
            return Helper.Rs485CallBackHelper.Instance.CallBackWithNoPara(callBack);  
        }

        /// <summary>
        /// ��ָ����λ����ͨѶ����
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool CommTest(int pos)
        {
            Reset(pos);
            Helper.Rs485CallBackHelper.CallBackOnePara_Int callBack=null;//IMeterController.ComTest;
            return Helper.Rs485CallBackHelper.Instance.CallBackWithOnePara_Int(callBack, pos); 
        }
        #endregion

        #region ����ʱ�� ReadDateTime()
        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <returns></returns>
        public bool ReadDateTime(ref string[] arrReturnData)
        {
            Reset();

            for (int i = 0; i < RetryTimes; i++)
            {
               // if (IMeterController.ReadDateTime())
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;               //ȫ��������˳�
            }
            if (arrReturnData.Length != BwCount)
                Array.Resize(ref arrReturnData, BwCount);
            Array.Copy(arrReturnValue_String, arrReturnData, arrReturnValue_String.Length);
            return IsAllReturn();
        }

        /// <summary>
        /// ��ȡָ����λ������ʱ��
        /// </summary>
        /// <param name="pos">��λ��</param>
        /// <param name="returnData">��ȡ��������ʱ��</param>
        /// <returns>��ȡ�Ƿ�ɹ�</returns>
        public bool ReadDateTime(int pos, ref string returnData)
        {
            successCount = BwCount - 1;
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.ReadDateTime(pos))                                        //����ָ��
                {
                    LogHelper.Instance.WriteDebug(string.Format("��{0}�η��Ͷ�ȡָ��ʧ��", i));
                    continue;
                }
                //��⵱ǰ��λ�Ƿ��з���
                WaitReturn();
                if (IsAllReturn()) break;
            }
            returnData = arrReturnValue_String[pos];
            return arrOverFlag[pos];
        }

        #endregion

        #region �����-д��ʱ��:m_485Adpater.WriteDateTime(string str_DateTime)
        /// <summary>
        /// ���ñ����ʱ��
        /// </summary>
        /// <param name="str_DateTime">Ҫ���õ�ʱ��[yyMMddHHmmss]</param>
        /// <returns>���һ�鶼û��д������False,��֮����True</returns>
        public bool WriteDateTime(string str_DateTime)
        {
            Comm.GlobalUnit.g_MsgControl.OutMessage("��ʼд��ʱ�䵽" + str_DateTime, false);
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
               // if (!IMeterController.WriteDateTime(str_DateTime))
                {
                    continue;
                }
                WaitReturn();
                if (IsAllReturn()) break;
            }
            return GetNeedReturnList().Length == 0;
        }

        /// <summary>
        /// ����ָ����λ�ĵ��ʱ��
        /// </summary>
        /// <param name="str_DateTime">Ҫ���õĵ��ʱ��</param>
        /// <param name="pos">�Ǳ��λ</param>
        /// <returns>�����Ƿ�ɹ�</returns>
        public bool WriteDateTime(string str_DateTime, int pos)
        {
            Reset(pos);
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.WriteDateTime(str_DateTime))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            return IsAllReturn();
        }
        #endregion

        #region ��ȡ����� ReadEnergy(enmPDirectType pDirect, enmTariffType TariffType)

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="pDirect">���ʷ���:0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="TariffType">����:0=�ܣ�1=�壬2=ƽ��3=�ȣ�4=��,5=���� </param>
        /// <returns>�����Ƿ�ɹ�����</returns>
        public bool ReadEnergy(byte pDirect, byte TariffType, ref Dictionary<int, float[]> dicReturnValue)
        {
            Reset();
            Comm.GlobalUnit.g_MsgControl.OutMessage("��ʼ��ȡ" + pDirect.ToString() + TariffType.ToString() + "����");
            for (int i = 0; i < RetryTimes; i++)
            {
               // if (!IMeterController.ReadEnergy(pDirect, TariffType))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            dicReturnValue = dicReturnValue_FloatArray;
            return IsAllReturn();
        }

        #endregion

        #region ��ȡ����ReadDemand(enmPDirectType PDirect, enmTariffType TariffType)

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="PDirect">���ʷ���</param>
        /// <param name="TariffType">����</param>
        /// <returns>�����Ƿ�ɹ�</returns>

        public bool ReadDemand(Cus_PowerFangXiang PDirect, Cus_FeiLv TariffType, ref Dictionary<int, float[]> dicReturnValue)
        {
            // if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
                //��ʽ�����
               // if (!IMeterController.ReadDemand((enmPDirectType)PDirect, (enmTariffType)TariffType))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            dicReturnValue = dicReturnValue_FloatArray;
            return IsAllReturn();
        }

        #endregion

        #region----------�������----------
        /// <summary>
        /// �������
        /// </summary>
        /// <returns></returns>
        public bool ClearDemand()
        {
            // Check.Require(CommAdpter.m_IMeterControler, "���ܱ�ͨѶ������", Check.NotNull);
            //OpenPramgramLock();
            //int noPassCount = 0;
            //����״̬��λ
            Reset();
            // CallBack callback_All = new CallBack(CommAdpter.m_IMeterControler.ClearDemand);
            // CallBack_Retry callback_one = new CallBack_Retry(CommAdpter.m_IMeterControler.ClearDemand);
            //return DoTestCallBack_NoPara(callback_All, callback_one);

            //#region ----------ԭ�����õĴ���
            for (int i = 0; i < RetryTimes; i++)
            {
                //if (!IMeterController.ClearDemand())
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }

            return IsAllReturn();
            //if (!isAllReturn())
            //    return false;
            //noPassCount = NoPassCount();
            //if (noPassCount == 0)                           //��һ��Ⱥ���Ƿ�ͨ��
            //    return true;
            //else
            //{
            //    //ֻ��ȫ��ͨ����ֻ����û��ͨ���ı�
            //    ResetNoPassMeterStatus();                   //����û�гɹ���ķ���״̬
            //    int reTryMeter = 0;
            //    int[] reTryMeterArr = new int[lstNoPassMeter.Count];
            //    lstNoPassMeter.Values.CopyTo(reTryMeterArr, 0);
            //    for (int n = 0; n < reTryMeterArr.Length; n++)
            //    {
            //        reTryMeter = reTryMeterArr[n];
            //        for (int i = 0; i < ReTryTimes; i++)
            //        {
            //            if (CommAdpter.m_IMeterControler.ClearDemand(reTryMeter))
            //            {
            //                //��෢�����Σ���һ�γɹ����˳�
            //                break;
            //            }
            //        }
            //    }
            //    if (!isAllReturn())
            //        return false;
            //    noPassCount = NoPassCount();
            //}
            //return noPassCount == 0;
            //#endregion
        }
        #endregion

        #region----------��յ���----------
        /// <summary>
        /// ��յ���
        /// </summary>
        /// <returns></returns>
        public bool ClearEnergy()
        {
            int noPassCount = 0;
            //if (CommAdpter.m_IMeterControler == null) return false;
            //OpenPramgramLock();
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.ClearEnergy())
                    continue;
                WaitReturn();
                if (IsAllReturn())
                    break;
            }
            return IsAllReturn();
        }
        #endregion

        #region ----------��ȡ������,�Զ����ʶ----------
        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="int_Len">����</param>
        /// <param name="int_Dot">С��λ,С��λΪ0����û��С��λ</param>
        /// <returns></returns>
        public bool ReadData(string str_ID, int int_Len, int int_Dot, ref string[] returnValue)
        {
            // if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            //readDataID = str_ID;
            //readDataLen = int_Len;
            //readDataDot = int_Dot;           //-1Ϊ�����Ƿ���С�����ȡ��ʽ
            for (int i = 0; i < RetryTimes; i++)
            {
                if (int_Dot > 0)
                {
                   // if (!IMeterController.ReadData(str_ID, int_Len, int_Dot))
                    {
                        continue;
                    }
                }
                else
                {
                   // if (!IMeterController.ReadData(str_ID, int_Len))
                    {
                        continue;
                    }
                }
                WaitReturn();
                if (IsAllReturn()) break;
            }
            if (returnValue.Length != BwCount)
                Array.Resize(ref returnValue, BwCount);
            Array.Copy(arrReturnValue_String, returnValue, arrReturnValue_String.Length);
            return IsAllReturn();
        }
        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="str_ID">��ʶ��</param>
        /// <param name="int_Len">����</param>
        /// <returns></returns>
        //public bool ReadData(string str_ID, int int_Len)
        //{
        //    bool Result = false;
        //    for (int i = 0; i < ReTryTimes; i++)
        //    {
        // Result = CommAdpter.m_IMeterControler.ReadData(str_ID, int_Len);
        //       if (Result)
        //          break;
        // }
        //return Result;
        //}
        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="str_ID">��ʶ</param>
        /// <param name="int_Len">����</param>
        /// <param name="int_Dot">С��λ</param>
        /// <returns></returns>
        //public bool ReadDataBlock(string str_ID, int int_Len, int int_Dot,)
        //{
        //    //if (CommAdpter.m_IMeterControler == null) return false;
        //    Reset();
        //    //if (!CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len, int_Dot))
        //    {
        //        Comm.GlobalUnit.g_MsgControl.OutMessage("���Ͷ�ȡ����������ʧ��!", false);
        //        return false;
        //    }
        //    return isAllReturn();
        //}
        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="str_ID"> ��ʶ</param>
        /// <param name="int_Len">����</param>
        /// <returns></returns>
        //public bool ReadDataBlock(string str_ID, int int_Len)
        //{
        //    //  if (CommAdpter.m_IMeterControler == null) return false;
        //    Reset();
        //    for (int i = 0; i < ReTryTimes; i++)
        //    {
        //        //  if (CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len))
        //        {
        //            break;  //�������Ρ���һ�γɹ���BREAK    
        //        }
        //    }
        //    return isAllReturn();
        //}
        /// <summary>
        /// �๦������������ʾ
        /// </summary>
        /// <param name="str_Frame"></param>
        private void IMeterControler_OnEventRxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("==>" + str_Frame);
        }
        /// <summary>
        /// �๦������������ʾ
        /// </summary>
        /// <param name="str_Frame"></param>
        private void IMeterControler_OnEventTxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("<==" + str_Frame);
        }

        #endregion

        #region ���ñ�ͨѶЭ��m_485Adpater.setMeterPara

        /// <summary>
        /// ��������һ���๦�ܲ���������Э������
        /// </summary>
        /// <param name="protocol">���Э��</param>
        /// <param name="addr">���ַ</param>
        /// <param name="BwH">��λ��</param>
        /// <returns></returns>
        public bool SetMeterPara(Comm.Model.DgnProtocol.DgnProtocolInfo protocol, string addr, int BwH)
        {
            //ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
            //meterDgnInfo[BwH] = new CAmMeterInfo();
            //IMeterController.Selected[BwH] = true;                                  //Ҫ���־
            //if (!SetProtocol(protocol, meterDgnInfo[BwH], addr))
            //{
            //    return false;
            //}
            ////�����������Ϣ
            //IMeterController.AmMeterInfo = meterDgnInfo;
            return true;
        }

        /// <summary>
        /// ���ö๦�ܲ��������ڼ춨ʱ����
        /// </summary>
        //public bool SetMeterPara()
        //{
        //    ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
        //    string meterAddress = string.Empty;
        //    for (int k = 0; k < BwCount; k++)
        //    {
        //        MeterBasicInfo curMeter = GlobalUnit.Meter(k);
        //        meterDgnInfo[k] = new CAmMeterInfo();
        //        IMeterController.Selected[k] = curMeter.YaoJianYn;
        //        if (curMeter.YaoJianYn)
        //        {
        //            meterAddress = curMeter.Mb_chrAddr.Length > 0 ? curMeter.Mb_chrAddr : curMeter.Mb_ChrCcbh;
        //            if (!curMeter.DgnProtocol.Loading) curMeter.DgnProtocol.Load();
        //            if (!curMeter.DgnProtocol.Loading)
        //            {
        //                Helper.LogHelper.Instance.WriteError(string.Format("��{0}��λ���ܱ�๦��Э���ʼ��ʧ��", k + 1), null);
        //                return false;
        //            }
        //            if (!SetProtocol(curMeter.DgnProtocol, meterDgnInfo[k], meterAddress))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    IMeterController.AmMeterInfo = meterDgnInfo;
        //    return true;
        //}

        /// <summary>
        /// �豸Э������
        /// </summary>
        //private bool SetProtocol(DgnProtocolInfo protocol, ClInterface.CAmMeterInfo meterDgnInfo, string meterAddr)
        //{

        //    meterDgnInfo.DllFile = protocol.DllFile;     //��̬
        //    meterDgnInfo.ClassName = protocol.ClassName;
        //    meterDgnInfo.Address = meterAddr;
        //    meterDgnInfo.Setting = protocol.Setting;
        //    meterDgnInfo.UserID = protocol.UserID;
        //    meterDgnInfo.VerifyPasswordType = protocol.VerifyPasswordType;
        //    meterDgnInfo.WritePassword = protocol.WritePassword;
        //    meterDgnInfo.WritePswClass = protocol.WriteClass;
        //    meterDgnInfo.ClearDemandPassword = protocol.ClearDemandPassword;
        //    meterDgnInfo.ClearDemandPswClass = protocol.ClearDemandClass;
        //    meterDgnInfo.ClearEnergyPassword = protocol.ClearDLPassword;
        //    meterDgnInfo.ClearEnergyPswClass = protocol.ClearDLClass;
        //    meterDgnInfo.DataFieldPassword = protocol.DataFieldPassword;
        //    meterDgnInfo.BlockAddAA = protocol.BlockAddAA;
        //    meterDgnInfo.TariffOrderType = protocol.TariffOrderType;
        //    meterDgnInfo.DateTimeFormat = protocol.DateTimeFormat;
        //    meterDgnInfo.SundayIndex = protocol.SundayIndex;
        //    meterDgnInfo.ConfigFile = protocol.ConfigFile;
        //    meterDgnInfo.ComTestType = GetType(protocol.DgnPras, "001", 1);
        //    // meterDgnInfo[k].BroadcastTimeType = 1;
        //    meterDgnInfo.ReadEnergyType = GetType(protocol.DgnPras, "006", 1);
        //    meterDgnInfo.ReadDemandType = GetType(protocol.DgnPras, "005", 1);
        //    meterDgnInfo.ReadDateTimeType = GetType(protocol.DgnPras, "003", 1);
        //    //meterDgnInfo[k].ReadAddressType = 1;
        //    meterDgnInfo.ReadPeriodTimeType = GetType(protocol.DgnPras, "007", 1);
        //    // meterDgnInfo[k].ReadDataType = 1;
        //    // meterDgnInfo[k].WriteAddressType = 1;
        //    meterDgnInfo.WriteDateTimeType = GetType(protocol.DgnPras, "002", 1);
        //    // meterDgnInfo[k].WritePeriodTimeType = 1;
        //    // meterDgnInfo[k].WriteDataType = 1;
        //    meterDgnInfo.ClearDemandType = GetType(protocol.DgnPras, "004", 1);
        //    meterDgnInfo.ClearEnergyType = GetType(protocol.DgnPras, "008", 1);
        //    //meterDgnInfo[k].ClearEventLogType = 1;
        //    // meterDgnInfo[k].SetPulseComType = 1;
        //    // meterDgnInfo[k].FreezeCmdType = 1;
        //    //meterDgnInfo[k].ChangeSettingType = 1;
        //    //meterDgnInfo[k].ChangePasswordType = 1;
        //    meterDgnInfo.FECount = protocol.FECount;
        //    //�����������Ϣ
        //    return true;
        //}




        /// <summary>
        /// ��ȡ�๦��Э�����ò���
        /// </summary>
        /// <param name="DgnPram">�๦�ܲ����б�</param>
        /// <param name="PramKey">Ҫȡֵ��KEY</param>
        /// <param name="DefaultValue">Ĭ��ֵ</param>
        /// <returns></returns>
        private int GetType(Dictionary<string, string> DgnPram, string PramKey, int DefaultValue)
        {
            if (!DgnPram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = DgnPram[PramKey].Split('|');
            if (Arr_Pram.Length == 2)
            {
                return int.Parse(Arr_Pram[0]);
            }
            else
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// �ֽ����๦�ܲ���
        /// </summary>
        /// <param name="Bwh"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] GetType(int Bwh, string PramKey)
        {
            if (!GlobalUnit.Meter(Bwh).DgnProtocol.Loading)
            {
                return new string[] { };
            }
            Dictionary<string, string> DgnPram = GlobalUnit.Meter(Bwh).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return new string[] { };
            }
            return DgnPram[PramKey].Split('|');
        }

        #endregion

        #region -------------------��������-------------------
        /// <summary>
        /// ���ǰ������
        /// </summary>
        /// <returns></returns>
        private bool CheckCondition()
        {
            if (IMeterController == null || !isInitOk)
            {
                LogHelper.Instance.WriteError("RS485�������ǰ�����ȳ�ʼ��", null);
                return false;
            }
            return true;
        }
        #endregion

    }
}
