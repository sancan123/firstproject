using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using System.Threading;

namespace CLDC_VerifyAdapter.EventLog
{
   
    /// <summary>
    /// 端子座温度不平衡
    /// </summary>
    class EL_ImbalanceTerminalTemper : EventLogBase
    {
        float[] m_AlarmTemperature;
        float[] m_ResumeTemperature;
        float[] m_DelayTime;
        float[][] m_MeterTemperature;
        float[][] m_StandardTemperature;
        float[][] m_TargetTemperature;
        bool[] m_PositionFinishTestFlag;
        string[] m_TemperatureKey;
        bool[] Result;

        private void initPara(int BwCount)
        {
            m_PositionFinishTestFlag = new bool[BwCount];
            m_AlarmTemperature = new float[BwCount];
            m_ResumeTemperature = new float[BwCount];
            m_DelayTime = new float[BwCount];
            m_MeterTemperature = new float[BwCount][];
            m_StandardTemperature = new float[BwCount][];
            m_TargetTemperature = new float[BwCount][]; 
            Result = new bool[BwCount];
            for (int i = 0; i < BwCount;i++ )
            {
                m_MeterTemperature[i] = new float[8];
                m_StandardTemperature[i] = new float[16];
                m_TargetTemperature[i] = new float[8];
            }
            if (GlobalUnit.clfs == Cus_Clfs.单相)
            {
                m_TemperatureKey = new string[] {"02810001", "02810002", "02810007", "02810008",
                                                "02810003", "02810004","02810005", "02810006" };
            }
            else
            {
                m_TemperatureKey = new string[] { "02810001", "02810002", "02810003", "02810004",
                                                "02810005", "02810006", "02810007", "02810008"};
            }
        }

        public EL_ImbalanceTerminalTemper(object plan)
            : base(plan)
        {

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

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论", "不合格原因" };
            return true;
        }

        void AdjustTemperature(Object obj)
        {
            int Index = (int)obj;

            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            float[] AdjustTemperature = new float[8];

            bool[] TestFlag = new bool[] { true, false, false, false, false, false, false, false };
            AdjustTemperature[0] = m_StandardTemperature[Index][1] + (m_TargetTemperature[Index][0] - m_MeterTemperature[Index][0]);
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, TestFlag, AdjustTemperature);
        }

        void SetCurrentTemperature(Object obj)
        {
            int Index = (int)obj;

            bool[] TestFlag = new bool[] { true, false, false, false, false, false, false, false };
            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            float[] AdjustTemperature = new float[8];
            AdjustTemperature[0] = m_StandardTemperature[Index][1];

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, TestFlag, AdjustTemperature);
        }

        void SetNoActionTemperature(Object obj)
        {
            int Index = (int)obj;
            bool[] TestFlag = new bool[] { true, false, false, false, false, false, false, false };
            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);

            m_TargetTemperature[Index][0] = m_StandardTemperature[Index][3] + m_AlarmTemperature[Index] - 5;
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, TestFlag, m_TargetTemperature[Index]);
        }

        void SetAlarmTemperature(Object obj)
        {
            int Index = (int)obj;
            bool[] TestFlag = new bool[] { true, false, false, false, false, false, false, false };
            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);

            m_TargetTemperature[Index][0] = m_StandardTemperature[Index][3] + m_AlarmTemperature[Index] + 5;
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, TestFlag, m_TargetTemperature[Index]);
        }

        void SetResumtTemperature(Object obj)
        {
            int Index = (int)obj;

            bool[] TestFlag = new bool[] { true, false, false, false, false, false, false, false };
            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);

            m_TargetTemperature[Index][0] = m_StandardTemperature[Index][3];
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, TestFlag, m_TargetTemperature[Index]);
        }

        private void ReadMeterTemperature(int Index)
        {
            for (int i = 0; i < m_TemperatureKey.Length; i++)
            {
                m_MeterTemperature[Index][i] = MeterProtocolAdapter.Instance.ReadData(m_TemperatureKey[i], 2, 1, Index);
            }
        }

        private float getDefTemperature(int Index)
        {
            float Result = 0;
            float Max = -999;
            float Min = 999;
            for (int i = 0; i < 8; i++)
            {
                float tmpTemperature = m_MeterTemperature[Index][i];
                if (-1 != tmpTemperature)
                {
                    if (tmpTemperature > Max)
                    {
                        Max = tmpTemperature;
                    }
                    if (tmpTemperature < Min)
                    {
                        Min = tmpTemperature;
                    }
                }
            }
            Result = Max - Min;
            return Result;
        }

        private bool JudgeStartTemperature(int Index)
        {
            bool bFlag = true;
            ReadMeterTemperature(Index);
            for (int i = 0; i < 8; i++)
            {
                bFlag &= m_MeterTemperature[Index][0] < 50;
            }
            return bFlag;
        }

        private bool JudgeNoActionTemperature(int Index)
        {
            ReadMeterTemperature(Index);
            bool bFlag = getDefTemperature(Index) > m_AlarmTemperature[Index] - 10;
            return bFlag;
        }

        private bool JudgeAlarmTemperature(int Index)
        {
            ReadMeterTemperature(Index);
            bool bFlag = getDefTemperature(Index) > m_AlarmTemperature[Index];
            return bFlag;
        }
        private bool JudgeResumeTemperature(int Index)
        {
            ReadMeterTemperature(Index);
            bool bFlag = getDefTemperature(Index) < m_ResumeTemperature[Index];
            return bFlag;
        }

        private void PositionTest(Object obj)
        {
            int Index = (int)obj;
            int TestDelay = 0;
            float AlarmCount = MeterProtocolAdapter.Instance.ReadData("035B0000", 3, 0, Index);

            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置起始温度");
            SetResumtTemperature(Index);
            while (!Stop)
            {
                if (JudgeStartTemperature(Index) && JudgeResumeTemperature(Index))
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置误动作温度");
            SetNoActionTemperature(Index);
            TestDelay = 300;
            while (!Stop && TestDelay > 0)
            {
                TestDelay--;

                if (JudgeNoActionTemperature(Index))
                {
                    SetCurrentTemperature(Index);
                    break;
                }
                if (0 == (TestDelay % 60))
                {
                    AdjustTemperature(Index);
                }
                Thread.Sleep(1000);
            }
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "读取电表告警事件");
            Result[Index] = true;
            TestDelay = (int)(m_DelayTime[Index]) * 2 + 2;
            while (!Stop && TestDelay > 0)
            {
                TestDelay--;

                float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("035B0000", 3, 0, Index);
                if (tmpAlarmCount > AlarmCount)
                {
                    Result[Index] = false;
                    reasonS[Index] = "电表误动作";
                    break;
                }
                Thread.Sleep(1000);
            }
            if (Result[Index])
            {
                MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置告警温度");
                SetAlarmTemperature(Index);
                TestDelay = 300;
                while (!Stop && TestDelay > 0)
                {
                    TestDelay--;

                    if (JudgeAlarmTemperature(Index))
                    {
                        SetCurrentTemperature(Index);
                        break;
                    }
                    if (0 == (TestDelay % 60))
                    {
                        AdjustTemperature(Index);
                    }
                    Thread.Sleep(1000);
                }
                MessageController.Instance.AddMessage("表位" + (Index + 1) + "读取电表告警事件");
                TestDelay = (int)(m_DelayTime[Index]) + 2;
                Result[Index] = false;
                reasonS[Index] = "未读到报警事件";
                while (!Stop && TestDelay > 0)
                {
                    TestDelay--;

                    float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("035B0000", 3, 0, Index);
                    if (tmpAlarmCount > AlarmCount)
                    {
                        Result[Index] = true;
                        reasonS[Index] = "";
                        break;
                    }
                    Thread.Sleep(1000);
                }
                if (Result[Index])
                {
                    MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置恢复温度");
                    SetResumtTemperature(Index);
                    TestDelay = 1200;
                    while (!Stop && TestDelay > 0)
                    {
                        TestDelay--;

                        if (JudgeResumeTemperature(Index))
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    MessageController.Instance.AddMessage("表位" + (Index + 1) + "读取恢复事件");
                    TestDelay = (int)(m_DelayTime[Index]) + 2;
                    Result[Index] = false;
                    reasonS[Index] = "未读到恢复事件";
                    while (!Stop && TestDelay > 0)
                    {
                        TestDelay--;
                        string strAlarmTime = MeterProtocolAdapter.Instance.ReadDataByPos("035B0001", 12, Index);
                        if (!strAlarmTime.Contains("000000000000"))
                        {
                            Result[Index] = true;
                            reasonS[Index] = "";
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, new bool[] { false, false, false, false, false, false, false, false }, new float[16]);
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "测试完成");
            m_PositionFinishTestFlag[Index] = true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
            initPara(BwCount);
            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度不平衡事件测试阈值");
            m_AlarmTemperature = MeterProtocolAdapter.Instance.ReadData("04091501", 2, 1);
            m_ResumeTemperature = MeterProtocolAdapter.Instance.ReadData("04091502", 2, 1);
            m_DelayTime = MeterProtocolAdapter.Instance.ReadData("04091503", 1, 0);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度不平衡事件产生前端子座温度不平衡总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("035B0000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;


            
            //进行测试
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    m_PositionFinishTestFlag[i] = false;
                    new Thread(new ParameterizedThreadStart(PositionTest)).Start(i);
                }
                else
                {
                    m_PositionFinishTestFlag[i] = true;
                }
            }

            bool TestFinishFlag = false;
            while (!Stop && !TestFinishFlag)
            {
                TestFinishFlag = true;
                for (int i = 0; i < BwCount; i++)
                {
                    TestFinishFlag &= m_PositionFinishTestFlag[i];
                }
                Thread.Sleep(1000);
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度不平衡事件产生后端子座温度不平衡总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("035B0000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次端子座温度不平衡发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("035B0001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);
            MessageController.Instance.AddMessage("正在处理结果");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Result[i])
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}
