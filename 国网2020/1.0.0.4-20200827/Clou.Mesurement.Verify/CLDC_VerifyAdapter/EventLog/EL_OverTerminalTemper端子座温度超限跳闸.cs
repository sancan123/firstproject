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
    /// 端子座温度超限跳闸
    /// </summary>
    class EL_OverTerminalTemper : EventLogBase
    {
        float[] m_AlarmTemperature;
        float[] m_ResumeTemperature;
        float[] m_DelayTime;
        float[] m_TripCurrent;
        float[][] m_MeterTemperature;
        float[][] m_StandardTemperature;
        float[][] m_TargetTemperature;
        bool[] m_PositionFinishTestFlag;
        string[] m_TemperatureKey;
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
            m_TripCurrent = new float[BwCount];
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
            }
            else
            {
                m_TemperatureKey = new string[] { "02810001", "02810002", "02810003", "02810004",
                                                "02810005", "02810006", "02810007", "02810008"};
            }
        }

        public EL_OverTerminalTemper(object plan)
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
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    AdjustTemperature[i] = m_StandardTemperature[Index][i * 2 + 1] + (m_TargetTemperature[Index][i] - m_MeterTemperature[Index][i]);
                }

            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], AdjustTemperature);
        }

        void SetCurrentTemperature(Object obj)
        {
            int Index = (int)obj;

            Helper.EquipHelper.Instance.ReadTemperature(Index + 1, out m_StandardTemperature[Index]);
            float[] AdjustTemperature = new float[8];
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    AdjustTemperature[i] = m_StandardTemperature[Index][i * 2 + 1];
                }
            }
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], AdjustTemperature);
        }


        void SetNoActionTemperature(Object obj)
        {
            int Index = (int)obj;

            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = m_AlarmTemperature[Index] - 5;
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

            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = m_AlarmTemperature[Index] + 5;
                }
                else
                {
                    m_TargetTemperature[Index][i] = 0;
                }
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], m_TargetTemperature[Index]);
        }

        void SetResumeTemperature(Object obj)
        {
            int Index = (int)obj;

            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    m_TargetTemperature[Index][i] = 50;
                }
                else
                {
                    m_TargetTemperature[Index][i] = 0;
                }
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, m_SetTemperatureFlag[Index], m_TargetTemperature[Index]);
        }

        private void ReadMeterTemperature(int Index)
        {
            for (int i = 0; i < m_TemperatureKey.Length; i++)
            {
                m_MeterTemperature[Index][i] = MeterProtocolAdapter.Instance.ReadData(m_TemperatureKey[i], 2, 1, Index);
            }
        }

        private bool JudgeNoActionTemperature(int Index)
        {
            bool bFlag = false;
            ReadMeterTemperature(Index);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    bFlag |= m_MeterTemperature[Index][i] > m_AlarmTemperature[Index] - 10;
                }
            }
            return bFlag;
        }

        private bool JudgeAlarmTemperature(int Index)
        {
            bool bFlag = false;
            ReadMeterTemperature(Index);
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
            ReadMeterTemperature(Index);
            for (int i = 0; i < 8; i++)
            {
                if (m_SetTemperatureFlag[Index][i])
                {
                    bFlag &= m_MeterTemperature[Index][i] < m_ResumeTemperature[Index];
                }
            }
            return bFlag;
        }

        private void LowTemperatureNoActionTest(Object obj)
        {
            int Index = (int)obj;
            int TestDelay = 0;
            float AlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置起始温度");
            SetResumeTemperature(Index);
            while (!Stop)
            {
                if (JudgeResumeTemperature(Index))
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

                float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);
                if (tmpAlarmCount > AlarmCount)
                {
                    Result[Index] = false;
                    reasonS[Index] = "温度低于阈值时误动作";
                    break;
                }
                Thread.Sleep(1000);
            }
            Helper.EquipHelper.Instance.SetTemperature(Index + 1, new bool[] { false, false, false, false, false, false, false, false }, new float[16]);
            m_PositionFinishTestFlag[Index] = true;
        }

        private void LowCurrentNoActionTest(Object obj)
        {
            int Index = (int)obj;
            int TestDelay = 0;
            float AlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);
            MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置误动作温度");
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
            Result[Index] = true;
            TestDelay = (int)(m_DelayTime[Index]) * 2 + 2;
            while (!Stop && TestDelay > 0)
            {
                TestDelay--;

                float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);
                if (tmpAlarmCount > AlarmCount)
                {
                    Result[Index] = false;
                    reasonS[Index] = "电流低于阈值时误动作";
                    break;
                }
                Thread.Sleep(1000);
            }

            Helper.EquipHelper.Instance.SetTemperature(Index + 1, new bool[] { false, false, false, false, false, false, false, false }, new float[16]);
            m_PositionFinishTestFlag[Index] = true;
        }

        private void TripTest(Object obj)
        {
            int Index = (int)obj;
            int TestDelay = 0;
            float AlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);

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
            reasonS[Index] = "未读到跳闸事件";
            while (!Stop && TestDelay > 0)
            {
                TestDelay--;

                float tmpAlarmCount = MeterProtocolAdapter.Instance.ReadData("03590000", 3, 0, Index);
                if (tmpAlarmCount > AlarmCount)
                {
                    Result[Index] = true;
                    reasonS[Index] = "";
                    string AlarmFlag_0 = MeterProtocolAdapter.Instance.ReadDataByPos("04001601", 2, Index);
                    string AlarmFlag_1 = MeterProtocolAdapter.Instance.ReadDataByPos("04000503", 2, Index);
                    string jlzt1 = CLDC_DataCore.Function.Common.HexStrToBinStr(AlarmFlag_0);
                    string dbzt3 = CLDC_DataCore.Function.Common.HexStrToBinStr(AlarmFlag_1);
                    if (jlzt1.Substring(11, 1) == "0" || dbzt3.Substring(9, 1) == "0" || dbzt3.Substring(11, 1) == "0")
                    {
                        Result[Index] = false;
                        reasonS[Index] = "继电器状态错误";
                    }



                    break;
                }
                Thread.Sleep(1000);
            }
            if (Result[Index])
            {
                MessageController.Instance.AddMessage("表位" + (Index + 1) + "设置恢复温度");
                SetResumeTemperature(Index);
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
                    string strAlarmTime = MeterProtocolAdapter.Instance.ReadDataByPos("03580001", 12, Index);
                    if (!strAlarmTime.Contains("000000000000"))
                    {
                        Result[Index] = true;
                        reasonS[Index] = "";
                        break;
                    }
                    Thread.Sleep(1000);
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

            #region 直接合闸

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string strDateTime = "";
            int[] iFlag = new int[BwCount];
            string[] strCode = new string[BwCount];
            string[] status3 = new string[BwCount];
            string[] statusTmp = new string[BwCount];
            string[] statusTmp1 = new string[BwCount];
            string[] strHzDate = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            bool[] result = new bool[BwCount];
            string[] strFhkg = new string[BwCount];
            bool[] blnFhkg = new bool[BwCount];
            string[] strDataPut = new string[BwCount];
            bool[] blnYaojianMeter = new bool[BwCount];


            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);


            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
            Common.Memset(ref strCode, "04001401");
            Common.Memset(ref strDataPut, "04001401" + "0000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
            strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "1A00" + strDateTime);
            bool[] blnTzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *15);

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置




                if (Stop) return;
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
                strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "1C00" + strDateTime);
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                #endregion

            }
            else
            {
                #region 外置

                if (Stop) return;
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
                strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "1C00" + strDateTime);
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);


                #endregion
            }
            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度超限跳闸事件测试阈值");
            m_AlarmTemperature = MeterProtocolAdapter.Instance.ReadData("04091301", 2, 1);
            m_ResumeTemperature = MeterProtocolAdapter.Instance.ReadData("04091302", 2, 1);
            m_TripCurrent = MeterProtocolAdapter.Instance.ReadData("04091303", 3, 3);
            m_DelayTime = MeterProtocolAdapter.Instance.ReadData("04091304", 1, 0);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取端子座温度超限跳闸事件产生前端子座温度超限跳闸总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03590000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            bool TestFinishFlag = false;
            //温度低于阈值误动作测试
            MessageController.Instance.AddMessage("正在进行温度低于阈值误动作测试");
            float Current = 0;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Current < m_TripCurrent[i])
                    {
                        Current = m_TripCurrent[i];
                    }
                }
            }
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current + 1, 1, 1, "1.0", true, false);

            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    m_PositionFinishTestFlag[i] = false;
                    new Thread(new ParameterizedThreadStart(LowTemperatureNoActionTest)).Start(i);
                }
                else
                {
                    m_PositionFinishTestFlag[i] = true;
                }
            }
            TestFinishFlag = false;
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
            //电流低于阈值误动作测试
            MessageController.Instance.AddMessage("正在进行电流低于阈值误动作测试");
            Current = 999;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Current > m_TripCurrent[i])
                    {
                        Current = m_TripCurrent[i];
                    }
                }
            }
            if (999 != Current)
            {
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current - 1, 1, 1, "1.0", true, false);
            }

            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    m_PositionFinishTestFlag[i] = false;
                    new Thread(new ParameterizedThreadStart(LowCurrentNoActionTest)).Start(i);
                }
                else
                {
                    m_PositionFinishTestFlag[i] = true;
                }
            }
            TestFinishFlag = false;
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
            //跳闸测试
            MessageController.Instance.AddMessage("正在进行跳闸测试");
            Current = 0;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Current < m_TripCurrent[i])
                    {
                        Current = m_TripCurrent[i];
                    }
                }
            }

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current + 1, 1, 1, "1.0", true, false);

            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn & Result[i])
                {
                    m_PositionFinishTestFlag[i] = false;
                    new Thread(new ParameterizedThreadStart(TripTest)).Start(i);
                }
                else
                {
                    m_PositionFinishTestFlag[i] = true;
                }
            }

            TestFinishFlag = false;
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
            MessageController.Instance.AddMessage("正在读取端子座温度超限跳闸事件产生后端子座温度超限告警总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03590000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次端子座温度超限跳闸发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03590001", 6);
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