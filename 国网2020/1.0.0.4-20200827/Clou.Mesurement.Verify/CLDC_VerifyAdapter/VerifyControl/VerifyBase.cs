
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Function;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// ������ƻ���
    /// </summary>
    public abstract class VerifyBase
    {
        #region -------��������----------

        /// <summary>
        /// ̨����
        /// </summary>
        private int m_intTaiID;
        /// <summary>
        /// �춨��ʼʱ��,���ڼ춨��ʱ
        /// </summary>
        protected DateTime m_StartTime;

        /// <summary>
        /// �Ƿ��Ѿ���ɱ���Ŀ�춨
        /// ֻ��m_Stop=true��m_CheckOver=trueʱ���춨ֹͣ���������������
        /// </summary>
        protected bool m_CheckOver = false;

        /// <summary>
        /// �Ƿ�ֹͣ,ֹͣ����һ�������ⲿ����������״̬�����ġ��ڲ��춨һ�㲻��ʹ�ô˱�־��
        /// </summary>
        private bool m_Stop = false;


        /// <summary>
        /// �����������α��
        /// </summary>
        private float m_HGQ2DL = 5F;

        /// <summary>
        /// ��������ͨ����ȴ�ʱ��
        /// </summary>
        protected int m_WaitTime_SelectPulseChannel;
        /// <summary>
        /// ���ü춨������ȴ�ʱ��
        /// </summary>
        protected int m_WaitTime_SetTaskPara;

        /// <summary>
        /// ��ͨ��Դ��ȴ���ʱ��
        /// </summary>
        protected int m_WaitTime_PowerOn;

        /// <summary>
        /// ��ǰ���һ��������
        /// </summary>
        private int m_ErrAccordType;

        /// <summary>
        /// �����춨��ȴ�ʱ��
        /// </summary>
        protected int m_WaitTime_StartTask;
        /// <summary>
        /// ����188G��ȴ�ʱ��
        /// </summary>
        //protected int m_WaitTime_Set188G;
        /// <summary>
        /// ��׽������ʱ��ʾ��Ϣ�ȴ�ʱ��
        /// </summary>
        protected int m_WaitTime_ExceptionMsg = 300;

        /// <summary>
        /// ���ϸ�ԭ������
        /// </summary>
        protected string[] reasonS = new string[Adapter.Instance.BwCount];
        #endregion

        #region --------���캯��---------
        /// <summary>
        /// ���캯��
        /// <param name="plan">����</param>
        /// </summary>
        public VerifyBase(object plan)
        {
            //���浱ǰ��������
            //CurPlan = plan;
            VerifyPara = plan as string;
        }
        #endregion

        #region -----------�춨����------------
        /// <summary>
        /// ִ�м춨�������
        /// </summary>
        public void DoVerify()
        {
            GlobalUnit.ManualResult = new string[BwCount];
            GlobalUnit.ManualShuju = new string[BwCount];
            //
            //�������
            if (!CheckPara())
            {
                //Helper.LogHelper.Instance.WriteWarm("�������ݲ�����Ҫ��:" + CurPlan.ToString(), null);
                MessageController.Instance.AddMessage(string.Format("����������������,��������:{0}",VerifyPara),6,2);
                return;
            }
            //��ʼ����������
            //InitPlanData();
            //ֹͣ��־
            m_Stop = false;
            //��ɱ�־
            m_CheckOver = false;
            //�춨��ǰ������0
            GlobalUnit.g_CUS.DnbData.NowMinute = 0F;
            MessageController.Instance.AddMessage(string.Format("��ʼ�춨:{0}",VerifyProcess.Instance.CurrentName),6,90);
            try
            {
                //��¼��ʼ�춨ʱ��
                m_StartTime = DateTime.Now;
                //���ü�ⷽ������������Ҫ��������ʵ��
                Verify();
                //Helper.EquipHelper.Instance.PowerOff();
            }
            catch (Exception ex)
            {
                //TODO:�춨�е��쳣��׽������
                Helper.LogHelper.Instance.Loger.Error(ex.Message, ex);
                GlobalUnit.Logger.Error("ִ�м춨����:" + this.GetType().FullName, ex);
              //  MessageController.Instance.AddMessage(m_intTaiID.ToString() + "��װ�ü춨���̳���:"+ex.Message,6,2);
            }
            finally
            {
                //��ɺ�������
                Helper.LogHelper.Instance.WriteInfo("��ǰ��Ŀ�춨��ϣ���ԭ�춨��ʶ");
                m_Stop = true;
                m_CheckOver = true;
                Helper.LogHelper.Instance.WriteInfo("��ǰ��Ŀ�춨��ϣ��˳��춨��");
            }
        }

        /// <summary>
        /// ����ʱ
        /// <paramref name="seconds">����ʱʱ�����룩</paramref>
        /// <paramref name="msg">��ʾ���ܵ���Ϣ�����ڽ���{0}        5/20��</paramref>
        /// </summary>
        public void Countdown(float seconds, string msg)
        {
            DateTime startT = DateTime.Now;
            TimeSpan ts = DateTime.Now.Subtract(startT);
            while (ts.TotalSeconds < seconds && !Stop)
            {
                GlobalUnit.g_CUS.DnbData.NowMinute = (float)ts.TotalMinutes;

                string des = string.Format("���ڽ���{0}        {1}/{2}��", msg, (int)(seconds - ts.TotalSeconds), seconds);
                MessageController.Instance.AddMessage(des);

                Thread.Sleep(100);
                ts = DateTime.Now.Subtract(startT);
            }
            GlobalUnit.g_CUS.DnbData.NowMinute = (float)ts.TotalMinutes;
        }
        /// <summary>
        /// ��ʼ�춨[��Ҫ��������д]
        /// ������ID�Ϸ��Լ��
        /// ���ֲ������[��������Ҫʱ����������Ҫ��дCheckPara()����]
        /// ����ڵ�����[�Ѿ�Ĭ�ϴ�����Ŀ���ۣ���Ŀ����������Ҫ�������Լ����ش���]
        /// �����Ϣ���У����ü춨״̬,���ü춨��״̬
        /// </summary>
        public virtual void Verify()
        {
            GlobalUnit.IsJdb = false;
            GlobalUnit.IsXieBo = false;
            GlobalUnit.IsJxb = false;
            if (GlobalUnit.IsCL3112)
            {
                Helper.EquipHelper.Instance.FuncMstate(0xFE);
            }
            Helper.EquipHelper.Instance.SetErrCalcType(0);
            //if (GlobalUnit.ENCRYPTION_MACHINE_TYPE.Contains("����"))
            {
                if (Stop) return;
            //    Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
            //    Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter,0);
            }
            //add by wzs on 20200421
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "����ʱ��", setSameStrArryValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
   
            return;
        }
        /// <summary>
        /// �޲�����Դ:����ǰ����������Դ
        /// </summary>
        protected virtual bool PowerOn()
        {
            bool isYouGong = true;//(CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.�����й� || CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.�����й�);
            float xIb = 0F;
            //float.TryParse( CurPlan.OutPramerter.xIb.Replace("Ib", ""),out xIb); 
            //���������빦�����ط����������ĵ����ݲ����ϣ�Ĭ�ϰ��޵�����1.0���

            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U
                                                      , xIb * GlobalUnit.Ib
                                                      , (int)Cus_PowerYuanJian.H
                                                      , (int)Cus_PowerFangXiang.�����й�
                                                      , "1.0"
                                                      , isYouGong
                                                      , false);

            if (ret)
            {
                MessageController.Instance.AddMessage("���ڵȴ��๦�ܲ������ܱ���������ʱ��" + 5 + "�롣");
                Thread.Sleep(5 * 1000);
            }

            return ret;
        }

        /// <summary>
        /// �����Ϸ��Լ��[�ɾ���춨��ʵ��]
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckPara()
        {
            return true;
        }

        /// <summary>
        /// ����ǰ�ڵ�����,Ĭ������ǰ�춨���ͽ�������
        /// �����ܽ��ۺ͵�ǰ����Ľ���
        /// </summary>
        protected virtual void ClearItemData()
        {
            //MessageController.Instance.AddMessage("��ʼ����ǰ�춨��������");
            //�����ܽ���
            Helper.MeterDataHelper.Instance.ClearResultData(ResultKey, Cus_MeterDataType.�춨����);
            //����ַ������
            String powerDirectResult = string.Format("{0}{1}", ResultKey, (int)PowerFangXiang);
            Helper.MeterDataHelper.Instance.ClearResultData(powerDirectResult, Cus_MeterDataType.�춨����);
            //���������ɼ춨���Լ�����
        }

        /// <summary>
        /// ����ǰ�ڵ�����,Ĭ������ǰ�춨���ͽ�������
        /// �����ܽ��ۺ͵�ǰ����Ľ���
        /// </summary>
        protected virtual void ClearItemData(string strKey)
        {
            MessageController.Instance.AddMessage("��ʼ����ǰ�춨��������");
            //�����ܽ���
            Helper.MeterDataHelper.Instance.ClearResultData(strKey, Cus_MeterDataType.�춨����);
            //����ַ������
            String powerDirectResult = string.Format("{0}{1}", strKey, (int)PowerFangXiang);
            Helper.MeterDataHelper.Instance.ClearResultData(powerDirectResult, Cus_MeterDataType.�춨����);
            //���������ɼ춨���Լ�����
        }

        /// <summary>
        /// �ҽ�Ĭ������[��Ҫ����������д]
        /// </summary>
        protected virtual void DefaultItemData()
        {
            return;
        }

        /// <summary>
        /// ��ȡ������ֹͣ�춨״̬
        /// </summary>
        public bool Stop
        {
            set
            {
                m_Stop = value;
            }
            get { return m_Stop; }
        }
        #endregion

        #region ------------����-------------
        /// <summary>
        /// ���������α��
        /// </summary>
        public float HGQ2DL
        {
            set { m_HGQ2DL = value; }

        }

        /// <summary>
        /// ��ǰ�춨���Ƿ��Ѿ���ɼ춨
        /// </summary>
        public bool IsRunOver
        {
            get { return m_CheckOver; }
        }

        /// <summary>
        /// ��ȡ��ǰ�춨���й������޹�
        /// </summary>
        protected bool IsYouGong
        {
            get
            {
                bool _IsP = false;
                if (PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.�����й�
                    || PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.�����й� || PowerFangXiang == Cus_PowerFangXiang.����й�)
                    _IsP = true;
                return _IsP;
            }
        }
        /// <summary>
        /// ���ʷ���[������Ƿ���]
        /// </summary>
        protected string FangXiangStr
        {
            get
            {
                string _IsZ = string.Empty;
                if (PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.�����޹� ||
                    PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.�����й�)
                    _IsZ = "-";
                return _IsZ;

            }
        }
        /// <summary>
        /// ��ǰ�춨���ʷ���
        /// </summary>
        protected Cus_PowerFangXiang PowerFangXiang
        {
            get;
            set;
        }


        /// <summary>
        /// ���ر�λ����
        /// </summary>
        protected int BwCount
        {
            get
            {

                return Adapter.Instance.BwCount;
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        protected int ErrAccordType
        {
            set
            {
                m_ErrAccordType = value;
            }
            get { return m_ErrAccordType; }
        }

        /// <summary>
        /// ��ǰ�춨��������
        /// </summary>
        public object CurPlan
        {
            get;
            set;
        }
        private object m_CurPlan = null;


        /// <summary>
        /// ��ĿKey,��:P_1 ��ʾ��Ŀ�͵�һ����Ŀ
        /// ��ʾ��ǰ�춨��Ŀ�����ݽڵ�
        /// </summary> 
        protected abstract string ItemKey { get; }

        /// <summary>
        /// ��Ŀ���۽ڵ�,Ϊ��Ŀ�ܽ��۽��ֵ
        /// </summary>
        protected abstract string ResultKey { get; }

        /// <summary>
        /// �춨����ǰ��ʱ����λ����
        /// </summary>
        public long VerifyPassTime
        {
            get
            {
                return (long)((TimeSpan)(DateTime.Now - m_StartTime)).TotalSeconds;
            }
        }
        #endregion

        #region -------------����------------

        #region-----------����ָ�������µı�׼����----------
        /// <summary>
        /// ����ָ�������µı�׼����.(��Ԫ����)
        /// </summary>
        /// <param name="U">���ص�ѹ</param>
        /// <param name="I">���ص���</param>
        /// <param name="Clfs">������ʽ</param>
        /// <returns>��׼����</returns>
        protected float CalculatePower(float U, float I, Cus_Clfs Clfs)
        {
            float p = U * I;

            if (Clfs == Cus_Clfs.��������)
            {
                p *= 3F;
                return p;
            }
            else if (Clfs == Cus_Clfs.����)
            {
                return p;
            }
            else
            {
                p *= 1.732F;
                return p;
            }
        }
        /// <summary>
        /// ����ָ�������µı�׼����.(W)
        /// </summary>
        /// <param name="U">���ص�ѹ</param>
        /// <param name="I">���ص���</param>
        /// <param name="Clfs">������ʽ</param>
        /// <param name="Yj">Ԫ��H��ABC</param>
        /// <param name="Glys">����������0.5L</param>
        /// <param name="isP">true �й���false �޹�</param>
        /// <returns>��׼����</returns>
        protected float CalculatePower(float U, float I, Cus_Clfs Clfs, Cus_PowerYuanJian Yj, string Glys, bool isP)
        {
            float flt_GlysP = 1;
            float flt_GlysQ = 0;
            if (isP)
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysP);
                flt_GlysQ = (float)Math.Sqrt(1 - Math.Pow(flt_GlysP, 2));
            }
            else
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysQ);
                flt_GlysP = (float)Math.Sqrt(1 - Math.Pow(flt_GlysQ, 2));
            }
            float p = U * I * flt_GlysP;
            float q = U * I * flt_GlysQ;
            if (Cus_PowerYuanJian.H == Yj)
            {
                if (Clfs == Cus_Clfs.��������)
                {
                    p *= 3F;
                    q *= 3F;
                }
                else if (Clfs == Cus_Clfs.����)
                {

                }
                else
                {
                    p *= 1.732F;
                    q *= 1.732F;
                }
            }
            return isP ? p : q;
        }
        #endregion

        #region ----------���㵱ǰ�����±����������峣����С�ı���һ��������Ҫ��ʱ��----------
        /// <summary>
        /// �����һ�������Ҫ��ʱ��ms
        /// </summary>
        ///<remarks>
        ///������ڶ��ֳ����ĵ��ܱ��������ȳ�����ĵ��ܱ�Ϊ׼
        ///</remarks>
        /// <returns>��һ�������Ҫʱ�����ֵ,��λms</returns>
        protected int GetOneErrorTime(string PowerDianLiu, Cus_PowerYuanJian PowerYuanJian, string PowerYinSu, bool IsP)
        {
            MeterBasicInfo firstMeter = Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter);
            if (firstMeter == null) return 1000;//Ĭ�ϰ�һ�봦��
            //���㵱ǰ���ع���
            float current = Number.GetCurrentByIb(PowerDianLiu, firstMeter.Mb_chrIb,firstMeter.Mb_BlnHgq);
            float currentPower = CalculatePower(GlobalUnit.U, current, GlobalUnit.Clfs, PowerYuanJian, PowerYinSu, IsP);
            //����һ�ȴ���Ҫ��ʱ��,��λ����
            float needTime = 1000F / currentPower * 60F;
            return OnePulseNeedTime(IsYouGong, needTime);
        }
        /// <summary>
        /// ���㵱ǰ�����±����������峣����С�ı���һ��������Ҫ��ʱ�䣨ms��
        /// </summary>
        /// <param name="bYouGong">�й�/�޹�</param>
        /// <param name="OneKWHTime">һ�ȵ���Ҫ��ʱ��(��)</param>
        /// <returns>�Ժ���Ϊ��λ</returns>
        protected int OnePulseNeedTime(bool bYouGong, float OneKWHTime)
        {
            float _MinConst = 999999999;
            int _OnePulseTime = 99;
            int[] arrConst = Helper.MeterDataHelper.Instance.MeterConst(bYouGong);
            for (int i = 0; i < arrConst.Length; i++)
            {

                if (arrConst[i] < _MinConst)
                    _MinConst = arrConst[i];

            }
            if (_MinConst == 999999999) return 1;
            _OnePulseTime = (int)Math.Ceiling((OneKWHTime * 60 / _MinConst * 1000));
            return _OnePulseTime;
        }
        #endregion
        /// <summary>
        /// ����Ƿ�����
        /// </summary>
        /// <param name="lastError">ǰһ���</param>
        /// <param name="curError">��ǰ���</param>
        /// <param name="meterLevel">��ȼ�</param>
        /// <param name="m_WCJump">����ϵ��</param>
        /// <returns>T:����;F:������</returns>
        protected bool CheckJumpError(string lastError, string curError, float meterLevel, float m_WCJump)
        {
            bool result = false;
            if (Number.IsNumeric(lastError) && Number.IsNumeric(curError))
            {
                float _Jump = float.Parse(curError) - float.Parse(lastError);
                if (Math.Abs(_Jump) > meterLevel * m_WCJump)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// ��ȡָ�����ܱ�ǰ���ʷ����±�ȼ�
        /// </summary>
        /// <returns></returns>
        protected float MeterLevel(MeterBasicInfo meter)
        {
            string[] _DJ = Number.getDj(meter.Mb_chrBdj);
            return float.Parse(_DJ[IsYouGong ? 0 : 1]);                   //��ǰ��ĵȼ�
        }
        #endregion

        #region ----------��ȡ��������---------
        /// <summary>
        /// ��ȡ�춨����,��������Ϊ��ǰ�춨����
        /// </summary>
        /// <param name="arrData">�������飬ÿ��λ��Ӧ</param>
        /// <param name="arrWcCount">���������飬ÿ��λ��Ӧ</param>
        /// <param name="DemoWcLimit">��ǰ����ޣ�����DEMOʱ������ʾ����</param>
        /// <returns>�Ƿ��ȡ�ɹ�</returns>
        protected bool ReadData(ref string[] arrData, ref int[] arrWcCount, float DemoWcLimit)
        {
            if (!GlobalUnit.IsDemo)
            {
                MessageController.Instance.AddMessage("���ڶ�ȡ�춨����...");
                CLDC_DeviceDriver.stError[] arrError = Helper.EquipHelper.Instance.ReadWcb(true);
                MessageController.Instance.AddMessage("��ȡ�춨������ɣ��ȴ���������");
                arrData = new string[BwCount];
                arrWcCount = new int[BwCount];
                for (int i = 0; i < arrError.Length && i < BwCount; i++)
                {
                    arrData[i] = arrError[i].szError;
                    arrWcCount[i] = arrError[i].Index;
                }
            }
            else
            {
           //     arrData = Helper.VerifyDemoHelper.Instance.BasicError2(DemoWcLimit, ref arrWcCount);
            }
            /*
                ��ȡ������������������������һ����������Ϊ0�Ļ�����ʶȫ����û�г����
                */
            int[] currentWcNumCopy = (int[])arrWcCount.Clone();
            Array.Sort(currentWcNumCopy);
            if (currentWcNumCopy == null
                || currentWcNumCopy.Length == 0
                || currentWcNumCopy[currentWcNumCopy.Length - 1] == 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ----------������----------
        /// <summary>
        /// ͳһ������
        /// </summary>
        /// <param name="ex">�������</param>
        /// <returns>false[��Ҫ���ڴ������еķ���]</returns>
        protected bool CatchException(Exception ex)
        {
            //ֻ�е�ǰ״̬Ϊ��ֹͣ״̬�²���ʾ��
            //Console.WriteLine("Stop:" + Stop.ToString() + "====ForceVerifyStop:" + Comm.GlobalUnit.ForceVerifyStop.ToString());
            MessageController.Instance.AddMessage("�춨�����г��ִ���:"+ex.Message,6,2);

            if (!Stop)
            {
                //GlobalUnit.ForceVerifyStop = true;
                Stop = true;
                Thread.Sleep(m_WaitTime_ExceptionMsg);
            }
            return false;
        }
        #endregion

        
        #region ��ʼ����������
        /// <summary>
        /// ��ʼ���������ݣ���Ҫ�������еĹ���Դ����
        /// ��:��ǰ�Ĺ��ʷ����Ƿ����й���
        /// </summary>
        private void InitPlanData()
        {
            if (CurPlan is StPlan_Dgn)
            {
                StPlan_Dgn tagDgn = (StPlan_Dgn)CurPlan;
                PowerFangXiang = tagDgn.OutPramerter.GLFX;
            }
        }
        #endregion

        #region ��ʽ���ȴ�
        /// <summary>
        /// ��ʽ���ȴ���Ϣ���
        /// </summary>
        /// <param name="formatString">��ʾ������,������ֻ�ܰ���һ��{0}ռλ��</param>
        /// <param name="maxWaitTime">���ȵ�ʱ�䣬��λMS</param>
        protected void ShowWaitMessage(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                {
                    break;
                }
            }
        }



        protected void ShowWaitMessageForSelfHeating(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// ��ʽ���ȴ���Ϣ���
        /// </summary>
        /// <param name="formatString">��ʾ������,������ֻ�ܰ���һ��{0}ռλ��</param>
        /// <param name="maxWaitTime">���ȵ�ʱ�䣬��λMS</param>
        protected bool ShowWaitTimeMessage(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (maxtime < 1000)
                {
                    return true;
                }
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.ֹͣ�춨)
                {
                    return true;
                }
            }
            return true;
        }
        #endregion
        /// <summary>
        /// ���ݲ��������ȡ�������
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected string GetResultString(bool result)
        {
            return result ? "�ɹ�" : "ʧ��";
        }

        #region �¼ӵ�


        /// <summary>
        /// ���������
        /// </summary>
        protected CLDC_DataCore.WuChaDeal.WuChaContext m_WuChaContext;
        /// �����춨,����֪ͨ
        /// <summary>
        /// �����춨,����֪ͨ
        /// </summary>
        public virtual void FinishVerify()
        {
            //�ȴ�1��,�ȴ������ϴ����
            MessageController.Instance.NotifyVerifyFinished();
        }
        public string VerifyKey { get; set; }
        public string VerifyPara { get; set; }

        /// <summary>
        /// �����ֵ�
        /// </summary>
        private Dictionary<string, string[]> resultDictionary = new Dictionary<string, string[]>();
        /// <summary>
        /// �����ֵ�
        /// </summary>
        protected Dictionary<string, string[]> ResultDictionary
        {
            get { return resultDictionary; }
            set { resultDictionary = value; }
        }

        /// <summary>
        /// ���۵�����������
        /// </summary>
        /// <param name="arrayResultName"></param>
        protected string[] ResultNames
        {
            set
            {
                if (value != null)
                {
                    ResultDictionary.Clear();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (!resultDictionary.ContainsKey(value[i]))
                        {
                            resultDictionary.Add(value[i], new string[BwCount]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// �ϴ��춨����
        /// </summary>
        /// <param name="resultName"></param>
        public void UploadTestResult(string resultName)
        {
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, resultName, ResultDictionary[resultName]);
        }
        /// <summary>
        /// ת���춨����
        /// </summary>
        /// <param name="resultName">����������</param>
        /// <param name="arrayResult">����</param>
        public void ConvertTestResult(string resultName, bool[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i] ? "�ϸ�" : "���ϸ�";
                    }
                }
            }
            UploadTestResult(resultName);
        }

        /// <summary>
        /// ת���춨����
        /// </summary>
        /// <param name="resultName">����������</param>
        /// <param name="arrayResult">����</param>
        public void ConvertTestResult(string resultName, int[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString();
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// ת���춨����
        /// </summary>
        /// <param name="resultName">����������</param>
        /// <param name="arrayResult">����</param>
        public void ConvertTestResult(string resultName, string[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i];
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// ת���춨����
        /// </summary>
        /// <param name="resultName">����������</param>
        /// <param name="dotNumber">С���������</param>
        /// <param name="arrayResult">����</param>
        public void ConvertTestResult(string resultName, float[] arrayResult,int dotNumber=2)
        {
            string formatTemp="0";
            if(dotNumber>0)
            {
                formatTemp="0.".PadRight(2+dotNumber,'0');
            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString(formatTemp) ;
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// ת���춨����
        /// </summary>
        /// <param name="resultName">����������</param>
        /// <param name="dotNumber">С���������</param>
        /// <param name="arrayResult">����</param>
        public void ConvertTestResult(string resultName, double[] arrayResult, int dotNumber = 2)
        {
            string formatTemp = "0";
            if (dotNumber > 0)
            {
                formatTemp = "0.".PadRight(2 + dotNumber, '0');
            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString(formatTemp);
                    }
                }
            }
            UploadTestResult(resultName);
        }
        #endregion



        /// <summary>
        /// �л���Զ��ģʽ�µ�׼������
        /// </summary>
        /// <param name="iFlag"></param>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        public void ChangRemotePreparatoryWork(out int[] iFlag, out string[] strRand1, out string[] strRand2, out string[] strEsamNo)
        {
            iFlag = new int[BwCount];
            strRand1 = new string[BwCount];
            strRand2 = new string[BwCount];
            strEsamNo = new string[BwCount];

            string[] strRand1Tmp = new string[BwCount];
            string[] strRand2Tmp = new string[BwCount];
            string[] strEsamNoTmp = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strRevCode = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strERand1 = new string[BwCount];
            string[] strERand2 = new string[BwCount];


            //׼��

            #region �Ⱥ�����֤����ܽ��ж�ȡ��һϵ�в���

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("���ڼ����Կ״̬,���Ժ�....");
            string[] MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(MyStatus[i])) continue;
                iSelectBwCount++;
                if (MyStatus[i].EndsWith("1FFFF"))
                {
                    rstTmp[i] = true;
                    iFlag[i] = 1;
                }
            }

            if (Stop) return;
            int[] iFlagTmp = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
            

            //if (Stop) return;
            //MessageController.Instance.AddMessage("���ڽ���Ѱ��,���Ժ�....");
            //bool[] result = MeterProtocolAdapter.Instance.SouthFindCard(1);



            //// 1.��Կ�ָ�
            //if (Array.IndexOf(rstTmp, true) > -1)
            //{
            //    if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
            //    {
            //        for (int i = 0; i < BwCount; i++)
            //        {
            //            if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
            //            {
            //                if (Stop) return;
            //                if (rstTmp[i])
            //                {
            //                    MessageController.Instance.AddMessage("���ڵ�" + (i + 1) + "��λ��Կ�ָ�,���Ժ�....");
            //                    bool blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "00", 17, strRand2Tmp[i], strEsamNoTmp[i]);
            //                    iFlagTmp[i] = 0;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Stop) return;
            //        MessageController.Instance.AddMessage("������Կ�ָ�,���Ժ�....");
            //        bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2Tmp, strEsamNoTmp);
            //        ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 10);
            //        Common.Memset(ref iFlagTmp, 0);
            //    }
            //}



            //if (Stop) return;
            //Common.Memset(ref strRevCode, "DF010001002D0001");
            //MessageController.Instance.AddMessage("���ڶ�ȡ�ѿ�ģʽ״̬��");
            //result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlagTmp, strRand1Tmp, strRevCode, out FkStatus, out strOutMac1);

            //iSelectBwCount = 0;
            //Common.Memset(ref rstTmp, false);
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //    iSelectBwCount++;
            //    if (FkStatus[i] != "01")
            //    {
            //        rstTmp[i] = true;
            //    }
            //}

            //if (Array.IndexOf(rstTmp, true) > -1)
            //{
            //    Common.Memset(ref strData, "01" + "00000000" + "00000000");

            //    if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
            //    {
            //        for (int i = 0; i < BwCount; i++)
            //        {
            //            if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //            if (Stop) return;
            //            if (rstTmp[i])
            //            {
            //                MessageController.Instance.AddMessage("���ڶԵ�" + (i + 1) + "����·�ģʽ�л������л���Զ��ģʽ,���Ժ�....");
            //                bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlagTmp[i], strRand2Tmp[i], strData[i]);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Stop) return;
            //        MessageController.Instance.AddMessage("�����·�ģʽ�л������л���Զ��ģʽ,���Ժ�....");
            //        result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlagTmp, strRand2Tmp, strData);
            //        ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 10);
            //    }
            //}

            iFlag = iFlagTmp;
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
        }

        /// <summary>
        /// �л�������ģʽ�µ�׼������
        /// </summary>
        /// <param name="iFlag"></param>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        public void ChangLocalPreparatoryWork(out int[] iFlag, out string[] strRand1, out string[] strRand2, out string[] strEsamNo)
        {
            iFlag = new int[BwCount];
            strRand1 = new string[BwCount];
            strRand2 = new string[BwCount];
            strEsamNo = new string[BwCount];

            string[] strRand1Tmp = new string[BwCount];
            string[] strRand2Tmp = new string[BwCount];
            string[] strEsamNoTmp = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strRevCode = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strData = new string[BwCount];


            //׼��

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            int[] iFlagTmp = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);


            if (Stop) return;
            MessageController.Instance.AddMessage("���ڽ���Ѱ��,���Ժ�....");
            bool[] result = MeterProtocolAdapter.Instance.SouthFindCard(1);

            if (Stop) return;
            MessageController.Instance.AddMessage("���ڼ����Կ״̬,���Ժ�....");
            string[] MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(MyStatus[i])) continue;
                iSelectBwCount++;
                if (MyStatus[i].EndsWith("1FFFF"))
                {
                    rstTmp[i] = true;
                }
            }

            // 1.��Կ�ָ�
            if (Array.IndexOf(rstTmp, true) > -1)
            {
                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (Stop) return;
                            if (rstTmp[i])
                            {
                                MessageController.Instance.AddMessage("���ڵ�" + (i + 1) + "��λ��Կ�ָ�,���Ժ�....");
                                bool blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "00", 17, strRand2Tmp[i], strEsamNoTmp[i]);
                                iFlagTmp[i] = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("������Կ�ָ�,���Ժ�....");
                    bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2Tmp, strEsamNoTmp);
                    ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 10);
                    Common.Memset(ref iFlagTmp, 0);
                }
            }



            if (Stop) return;
            Common.Memset(ref strRevCode, "DF010001002D0001");
            MessageController.Instance.AddMessage("���ڶ�ȡ�ѿ�ģʽ״̬��");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlagTmp, strRand1Tmp, strRevCode, out FkStatus, out strOutMac1);

            iSelectBwCount = 0;
            Common.Memset(ref rstTmp, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iSelectBwCount++;
                if (FkStatus[i] != "00")
                {
                    rstTmp[i] = true;
                }
            }

            if (Array.IndexOf(rstTmp, true) > -1)
            {
                Common.Memset(ref strData, "00" + "00000000" + "00000000");

                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (Stop) return;
                        if (rstTmp[i])
                        {
                            MessageController.Instance.AddMessage("���ڶԵ�" + (i + 1) + "����·�ģʽ�л������л�������ģʽ,���Ժ�....");
                            bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlagTmp[i], strRand2Tmp[i], strData[i]);
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("�����·�ģʽ�л������л�������ģʽ,���Ժ�....");
                    result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlagTmp, strRand2Tmp, strData);
                    ShowWaitMessage("���ڵȴ�{0}��,���Ժ�....", 1000 * 10);
                }
            }

            iFlag = iFlagTmp;
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
        }

        /// <summary>
        /// ��ȡ���ַ�ͱ��
        /// </summary>
        public void ReadMeterAddrAndMeterNo()
        {
            if (GlobalUnit.IsDemo) return;

          //  if (!GlobalUnit.ReadMeterAddressAndNo) return;

            MeterBasicInfo FirstMeter = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);

          //  if (FirstMeter.DgnProtocol.ClassName != "CDLT6452007")
          //      return;




            GlobalUnit.g_MsgControl.OutMessage("���ڽ��С���ȡ���ַ������...");

            string[] address = MeterProtocolAdapter.Instance.ReadAddress();


            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //���統ǰֹͣ�춨�����˳�
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter.Mb_chrAddr = address[i];

            }
            Adapter.Instance.UpdateMeterProtocol();
            GlobalUnit.g_MsgControl.OutMessage("���ڽ��С���ȡ��š�����...");

            string[] meterno = MeterProtocolAdapter.Instance.ReadData("04000402", 6);

            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //���統ǰֹͣ�춨�����˳�
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter._Mb_MeterNo = meterno[i];
            }
            GlobalUnit.g_MsgControl.OutMessage();

          //  GlobalUnit.ReadMeterAddressAndNo = false;
        }



        #region �ز�����
        public void SwitchCarrierOr485(Cus_CommunType communType)
        {
            if (GlobalUnit.IsDemo) return;
            if (Stop) return ;   
            Helper.EquipHelper.Instance.PowerOff();
            Thread.Sleep(5000);
            GlobalUnit.g_IsOnPowerU = false;
            if (communType == Cus_CommunType.ͨѶ485)
            {
                GlobalUnit.g_MsgControl.OutMessage("������ͨ����̵����պ�", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(3);
                Thread.Sleep(300);
                //if (!GlobalUnit.g_IsOnPowerU)
                //    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, Cus_PowerFangXiang.�����й�);//ֻ�����ѹ
                GlobalUnit.g_CommunType = Cus_CommunType.ͨѶ485;

            }
            else if (communType == Cus_CommunType.ͨѶ�ز�)
            {
                GlobalUnit.g_MsgControl.OutMessage("�����ز�����̵����պ�", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(2);
                Thread.Sleep(300);
                if (!GlobalUnit.g_IsOnPowerU)
                    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.�����й�);//ֻ�����ѹ

                GlobalUnit.g_MsgControl.OutMessage("�ȴ���Դ�ȶ�...", false);

                Thread.Sleep(5000);

                GlobalUnit.g_MsgControl.OutMessage("���ڽ����ز�����춨...", false);

                if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierType.IndexOf("2041") != -1)
                {
                    //����ʼ����������
                    GlobalUnit.g_MsgControl.OutMessage("��ʼ��������...", false);
                    Dictionary<string, int> _2041ID = new Dictionary<string, int>();
                    for (int iBw = 0; iBw < BwCount; iBw++)
                    {

                        //����ȡָ����λ�����Ϣ��
                        MeterBasicInfo curMeterTmp = Helper.MeterDataHelper.Instance.Meter(iBw);

                        //���ж��Ƿ�Ҫ�졿
                        if (!curMeterTmp.YaoJianYn)
                        {
                            continue;
                        }
                        if (!_2041ID.ContainsKey(curMeterTmp.AVR_CARR_PROTC_NAME))
                        {
                            _2041ID.Add(curMeterTmp.AVR_CARR_PROTC_NAME, iBw);
                        }
                    }
                   
                        Helper.EquipHelper.Instance.Init2041();
                        Thread.Sleep(1000);
                                        GlobalUnit.g_MsgControl.OutMessage("��Ӵӽڵ�...", false);
                    AddCarrierNodes();
                    int Overtime = 0;
                    if (Stop) return;  
                    if (GlobalUnit.CarrierInfo.CarrierName == "�е绪��" || GlobalUnit.CarrierInfo.CarrierName == "�е绪��2016") //����ǿ�����ȴ�����ʱ��
                    {
                    //    for (int k = 0; k < 300; k++)
                    //    {
                    //        if (Stop) return;
                    //        Overtime += 1;
                     //       Thread.Sleep(1000);
                    //        GlobalUnit.g_MsgControl.OutMessage("�ȴ��ز�����300��,�Ѿ���ȥ" + Overtime + "��...", false);
                     //   }
                        ShowWaitMessage("�ȴ��ز�����300��,ʣ��{0}��...", 300000);
                    }
                    if (Stop) return;  
                    if (GlobalUnit.CarrierInfo.CarrierName != "�е绪��2016")
                    {
                       // foreach (int item in _2041ID.Values)
                     //   {
                            Helper.EquipHelper.Instance.PauseRouter();
                            Thread.Sleep(1000);
                       // }
                    }
                    else
                    {
                        Helper.EquipHelper.Instance.StarCarrier();
                    }
                    Thread.Sleep(500);
                }
                GlobalUnit.g_CommunType = Cus_CommunType.ͨѶ�ز�;
            }
            else if (communType == Cus_CommunType.ͨѶ����)
            {
                GlobalUnit.g_MsgControl.OutMessage("�������߹���̵����պ�", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(2);
                Thread.Sleep(300);
                if (!GlobalUnit.g_IsOnPowerU)
                    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.�����й�);//ֻ�����ѹ

                GlobalUnit.g_MsgControl.OutMessage("�ȴ���Դ�ȶ�...", false);

                Thread.Sleep(5000);

                GlobalUnit.g_MsgControl.OutMessage("���ڽ�����������춨...", false);


                if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierType.IndexOf("2041") != -1)
                {
                    //����ʼ����������
                    GlobalUnit.g_MsgControl.OutMessage("��ʼ��������...", false);
                    Dictionary<string, int> _2041ID = new Dictionary<string, int>();
                    for (int iBw = 0; iBw < BwCount; iBw++)
                    {

                        //����ȡָ����λ�����Ϣ��
                        MeterBasicInfo curMeterTmp = Helper.MeterDataHelper.Instance.Meter(iBw);

                        //���ж��Ƿ�Ҫ�졿
                        if (!curMeterTmp.YaoJianYn)
                        {
                            continue;
                        }
                        if (!_2041ID.ContainsKey(curMeterTmp.AVR_CARR_PROTC_NAME))
                        {
                            _2041ID.Add(curMeterTmp.AVR_CARR_PROTC_NAME, iBw);
                        }
                    }
                   
                        Helper.EquipHelper.Instance.Init2041();
                        Thread.Sleep(1000);
                    

                    GlobalUnit.g_MsgControl.OutMessage("��Ӵӽڵ�...", false);
                    AddCarrierNodes();
                  
                        Helper.EquipHelper.Instance.PauseRouter();
                        Thread.Sleep(500);
                    
                    Thread.Sleep(500);
                }
                if (Stop) return;  
                int _MaxStartTime = 180;
                m_StartTime = DateTime.Now;
                while (true)
                {
                    //ÿһ��ˢ��һ������
                    long _PastTime = VerifyPassTime;
                    System.Threading.Thread.Sleep(1000);

                    float pastMinute = _PastTime / 60F;
                    CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = string.Format("�ȴ�������Ҫ", PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "�֣��Ѿ�����" + pastMinute.ToString("F2") + "��";

                    CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(strDes, true);

                    if ((_PastTime >= _MaxStartTime) || Stop)
                    {
                        CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }

                    if (Stop) return;
                }
                if (Stop) return;  
                GlobalUnit.g_CommunType = Cus_CommunType.ͨѶ����;
            }
        }

        /// <summary>
        /// ����ز��ӽڵ�S
        /// </summary>
        private void AddCarrierNodes()
        {
            MeterBasicInfo curMeter;
            string strAddress;
            for (int iBw = 0; iBw < BwCount; iBw++)
            {
                //��ǿ��ֹͣ��
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
                strAddress = curMeter.Mb_chrAddr;
                if (strAddress == "")
                {
                    continue;
                }
                Helper.EquipHelper.Instance.AddCarrierNode(1, strAddress.PadLeft(12, '0'));

            }
        }







        #endregion


        public string[] setSameStrArryValue(string s)
        {
            string[] strs = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    strs[i] = s;
                }
            }
            return strs;


        }

    }
}
