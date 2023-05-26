using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using System.Threading;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 端子座温度剧变
    /// </summary>
    class EL_UpheavalTerminalTemper:EventLogBase
    {
        float[] m_AlarmTemperature;
        float[] m_ResumeTemperature;
        float[] m_DelayTime;
        float[][] m_MeterTemperature;
        float[][] m_StandardTemperature;
        float[][] m_TargetTemperature;
        bool[] m_PositionFinishTestFlag;
        string[] m_TemperatureKey;
        string[] m_TemperatureChangeKey;
        bool[][] m_SetTemperatureFlag;
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
            m_SetTemperatureFlag = new bool[BwCount][];
            Result = new bool[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                m_MeterTemperature[i] = new float[8];
                m_StandardTemperature[i] = new float[16];
                m_TargetTemperature[i] = new float[8]; 
                if (GlobalUnit.clfs == Cus_Clfs.单相)
                {
                    m_SetTemperatureFlag[i] = new bool[] { true, true, true, true, false, false, false, false };
                }
                else
                {
                    m_SetTemperatureFlag[i] = new bool[] { true, true, true, true, true, true, true, true };
                }
            }
            if (GlobalUnit.clfs == Cus_Clfs.单相)
            {
                m_TemperatureKey = new string[] {"02810001", "02810002", "02810007", "02810008",
                                                "02810003", "02810004","02810005", "02810006" };
                m_TemperatureChangeKey = new string[] {"0281000A", "0281000B", "02810010", "02810011", 
                                                "0281000C", "0281000D","0281000E", "0281000F" };
            }
            else
            {
                m_TemperatureKey = new string[] { "02810001", "02810002", "02810003", "02810004",
                                                "02810005", "02810006", "02810007", "02810008"};
                m_TemperatureChangeKey = new string[] {"0281000A", "0281000B", "0281000C", "0281000D",
                                                "0281000E", "0281000F", "02810010", "02810011"}; 
            }
        }
        public EL_UpheavalTerminalTemper(object plan)
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

        void SetNoActionTemperature(Object obj)
        {
            int Index = (int)obj;

            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = m_StandardTemperature[Index][i * 2 + 1] + m_AlarmTemperature[Index] - 5;
                }
                else
                {
                    m_TargetTemperature[Index][i] = 0;
                }
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], m_TargetTemperature[Index]);
        }

        void SetAlarmTemperature(Object obj)
        {
            int Index = (int)obj;

            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = m_StandardTemperature[Index][i * 2 + 1] + m_AlarmTemperature[Index] * 3;
                }
                else
                {
                    m_TargetTemperature[Index][i] = 0;
                }
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], m_TargetTemperature[Index]);
        }

        void SetResumtTemperature(Object obj)
        {
            int Index = (int)obj;

            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = m_StandardTemperature[Index][i * 2 + 1];
                }
                else
                {
                    m_TargetTemperature[Index][i] = 0;
                }
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], m_TargetTemperature[Index]);
        }

        private void ReadMeterTemperature(int Index,string[] Key)
        {
            for (int i = 0; i < Key.Length; i++)
            {
                m_MeterTemperature[Index][i] = MeterProtocolAdapter.Instance.ReadData(Key[i], 2, 1, Index);
            }
        }

        private bool JudgeStartTemperature(int Index)
        {
            bool bFlag = true;
            ReadMeterTemperature(Index, m_TemperatureKey);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    bFlag &= m_MeterTemperature[Index][i] < 50;
                }
            }
            return bFlag;
        }

        private bool JudgeAlarmTemperature(int Index)
        {
            bool bFlag = false;
            ReadMeterTemperature(Index, m_TemperatureChangeKey);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    bFlag |= m_MeterTemperature[Index][i] > m_AlarmTemperature[Index];
                }
            }
            return bFlag;
        }
        private bool JudgeResumeTemperature(int Index)
        {
            bool bFlag = true;
            ReadMeterTemperature(Index, m_TemperatureChangeKey);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    bFlag &= m_MeterTemperature[Index][i] < m_ResumeTemperature[Index];
                }
            }
            return bFlag;
        }

        private void PositionTest(Object obj)
        {
            int Index = (int)obj;
            int TestDelay = 0;
            float AlarmCount = MeterProtocolAdapter.Instance.ReadData("035A0000", 3, 0, Index);

            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置起始温度");
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, new bool[] { false, false, false, false, false, false, false, false }, new float[16]);
            while (!Stop)
            {
                if (JudgeStartTemperature(Index))
                {
                    break;
                }
                Thread.Sleep(1000);
            }
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置误动作温度");
            SetNoActionTemperature(Index);
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "读取电表告警事件");
            Result[Index] = true;
            TestDelay = ((int)m_DelayTime[Index] / 60 + 60) * 2 + 2;
            while (!Stop && TestDelay > 0)
            {
                TestDelay--;

                float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("035A0000", 3, 0, Index);
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
                        break;
                    }
                    Thread.Sleep(1000);
                }
                MessageController.Instance.AddMessage("表位" + (Index + 1) + "读取电表告警事件");
                TestDelay = (int)m_DelayTime[Index] / 60 + 60 + 2;
                Result[Index] = false;
                reasonS[Index] = "未读到报警事件";
                while (!Stop && TestDelay > 0)
                {
                    TestDelay--;

                    float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("035A0000", 3, 0, Index);
                    if (tmpAlarmCount > AlarmCount)
                    {
                        UInt64 AlarmFlag_0 = Convert.ToUInt64(MeterProtocolAdapter.Instance.ReadDataByPos("04001104", 8, Index), 16);
                        string tmpAlarmFlag = MeterProtocolAdapter.Instance.ReadDataByPos("04001501", 12, Index);
                        UInt64 AlarmFlag_1 = Convert.ToUInt64(tmpAlarmFlag.Substring(4, 2), 16);
                        Result[Index] = true;
                        if (0x200000000000 == (AlarmFlag_0 & 0x200000000000))
                        {
                            if (0x20 != (AlarmFlag_1 & 0x20))
                            {
                                Result[Index] = false;
                                reasonS[Index] = "电表主动上报状态字错误," + tmpAlarmFlag;
                            }
                        }
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
                    TestDelay = (int)m_DelayTime[Index] / 60 + 60 + 2;
                    Result[Index] = false;
                    reasonS[Index] = "未读到恢复事件";
                    while (!Stop && TestDelay > 0)
                    {
                        TestDelay--;
                        string strAlarmTime = MeterProtocolAdapter.Instance.ReadDataByPos("035A0001", 12, Index);
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

            MessageController.Instance.AddMessage("设置电表模式字");
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            Common.Memset(ref iFlag, 1);
            bool[] result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);
         //   iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("0000200040000000", "NNNNNNNNNNNNNNNN", 8, 0);
            CLDC_DataCore.Function.Common.Memset(ref strDataCode, "04001104");
            CLDC_DataCore.Function.Common.Memset(ref strData, "04001104" + "0000200040000000");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度超限告警事件测试阈值");
            m_AlarmTemperature = MeterProtocolAdapter.Instance.ReadData("04091401", 2, 1);
            m_ResumeTemperature = MeterProtocolAdapter.Instance.ReadData("04091401", 2, 1);
            m_DelayTime = MeterProtocolAdapter.Instance.ReadData("04091402", 1, 0);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度超限告警事件产生前端子座温度超限告警总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("035A0000", 3);
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
            MessageController.Instance.AddMessage("正在读取端子座温度超限告警事件产生后端子座温度超限告警总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("035A0000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次端子座温度超限告警发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("035A0001", 6);
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

        /// <summary>
        /// 格式化写字符串
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
                bool blnIsNum = true;           //判断读取的数据是不是数字
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
    }
}