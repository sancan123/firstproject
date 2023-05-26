using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using System.Windows.Forms;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 拉闸功能
    /// </summary>
    public class RemoteBreakRelay : VerifyBase
    {
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public RemoteBreakRelay(object plan)
            : base(plan)
        {
        }


        //拉闸延时30分钟1|远程拉闸命令1|上1次跳闸记录1|跳闸次数前-后1|跳闸信号输出1|拉闸延时中断电后重新上电电能表立刻跳闸1|
        //拉闸状态下受远程拉闸命令2|
        //拉闸延时2分钟|下发拉闸延时时间3|上1次跳闸记录3|跳闸延时3|跳闸次数前-后3|跳闸信号输出3|延时拉闸3



        protected override bool CheckPara()
        {
            ResultNames = new string[] { "拉闸延时30分钟1","远程拉闸命令1","上1次跳闸记录1","跳闸次数前一后1",
                                         "跳闸信号输出1（外置）","表内电流前一后1（内置）","继电器状态位前一后1","继电器命令状态位前一后1","电能表功率前一后1（内置）","拉闸延时中断电后重新上电电能表立刻跳闸1",
                                         "拉闸状态下受远程拉闸命令2","拉闸延时2分钟","下发拉闸延时时间3",
                                         "上1次跳闸记录3","跳闸延时3","跳闸次数前一后3","跳闸信号输出3（外置）","表内电流前一后3（内置）","继电器状态位前一后3","继电器命令状态位前一后3","电能表功率前一后3（内置）",
                                         "负荷开关误动作总次数前一后3（内置）","模拟开关误动作发生时刻3（内置）","上1次开关误动作记录发生时刻3（内置）","模拟开关误动作结束时刻3（内置）","上1次开关误动作记录结束时刻3（内置）","开关误动作记录3（内置）","延时拉闸3",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {

            if (!GlobalUnit.IsNZLoadRelayControl)
            {
                if (GlobalUnit.IsDan)
                {
                    Helper.EquipHelper.Instance.SetRelayControl(6);
                }
                else
                {
                    Helper.EquipHelper.Instance.SetRelayControl(7);
                }
            }


            base.Verify();
            if (Stop) return;
            PowerOn();

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];
            string[] strDataPut = new string[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 4];
            bool[] result = new bool[BwCount];
            bool[] blnFhkg = new bool[BwCount];
            string[] strGdCount = new string[BwCount];
            string[] strHzDate = new string[BwCount];
            int[] iFlag = new int[BwCount];
            int iCount = BwCount / 6;
            string[] strWDZBTime = new string[BwCount];
            string[] strWDZETime = new string[BwCount];
            string[] strRevCode = new string[BwCount];

            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行钱包初始化,请稍候....");
            Common.Memset(ref strData, "00002710");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

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
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            //if (Stop) return;
            //Common.Memset(ref strData, "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            //Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
            //MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
            //bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);




            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置表
                //1------------

                if (Stop) return;
                Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 4);
                if (Stop) return;
                int[] iLaZhaCount1Q = ReadMeterLaZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                string[] strMeterIbQ = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbQ = Common.StringConverToDecimaByIb(strMeterIbQ);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterIbQ[i]))
                    {
                        ResultDictionary["表内电流前一后1（内置）"][i] = strMeterIbQ[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后1（内置）"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
                string[] strMeterGLQ = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
                strMeterGLQ = Common.StringConverToDecimaByGL(strMeterGLQ);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterGLQ[i]))
                    {
                        ResultDictionary["电能表功率前一后1（内置）"][i] = strMeterGLQ[i];
                    }
                    else
                    {
                        ResultDictionary["电能表功率前一后1（内置）"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] = "通";
                        }
                        //if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        //{
                        //    ResultDictionary["继电器状态位前一后1"][i] = "断";
                        //}
                        //else
                        //{
                        //    ResultDictionary["继电器状态位前一后1"][i] = "通";
                        //}
                    }
                    else
                    {
                        //ResultDictionary["继电器状态位前一后1"][i] = "异常";
                        ResultDictionary["继电器命令状态位前一后1"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为30分钟,请稍候....");
                Common.Memset(ref strDataPut, "04001401" + "0030");
                Common.Memset(ref strCode, "04001401");
                bool[] resultTime = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["拉闸延时30分钟1"][i] = resultTime[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("拉闸延时30分钟1");

                if (Stop) return;
                Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));

                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
                if (Stop) return;
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["远程拉闸命令1"][i] = result[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("远程拉闸命令1");


                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                MessageController.Instance.AddMessage("正在断电,请稍候....");
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOff();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 14);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 4);
                string strTzdate = DateTime.Now.ToString("yyMMddHHmmss");

                if (Stop) return;
                int[] iLaZhaCount1H = ReadMeterLaZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                string[] strMeterIbH = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbH = Common.StringConverToDecimaByIb(strMeterIbH);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterIbH[i]))
                    {
                        ResultDictionary["表内电流前一后1（内置）"][i] += "-" + strMeterIbH[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后1（内置）"][i] += "-异常";
                    }
                    ResultDictionary["跳闸次数前一后1"][i] = iLaZhaCount1Q[i].ToString() + "-" + iLaZhaCount1H[i].ToString();
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
                string[] strMeterGLH = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
                strMeterGLH = Common.StringConverToDecimaByGL(strMeterGLH);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterGLH[i]))
                    {
                        ResultDictionary["电能表功率前一后1（内置）"][i] += "-" + strMeterGLH[i];
                    }
                    else
                    {
                        ResultDictionary["电能表功率前一后1（内置）"][i] += "-异常";
                    }
                }
                UploadTestResult("电能表功率前一后1（内置）");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] += "-断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] += "-通";
                        }

                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后1"][i] += "-异常";
                    }
                }


                //UploadTestResult("继电器状态位前一后1");
                UploadTestResult("继电器命令状态位前一后1");
                UploadTestResult("跳闸次数前一后1");
                UploadTestResult("表内电流前一后1（内置）");

                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "跳闸信号输出1（外置）", strData);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取跳闸事件记录,请稍候....");
                string[] strTzjlInfoH = MeterProtocolAdapter.Instance.ReadData("1D00FF01", 38);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次跳闸记录1", strTzjlInfoH);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        continue;

                    if (!string.IsNullOrEmpty(strTzjlInfoH[i]) && strTzjlInfoH[i].ToString().Length >= 68
                        && !string.IsNullOrEmpty(iLaZhaCount1Q[i].ToString()) && !string.IsNullOrEmpty(iLaZhaCount1H[i].ToString()))
                    {
                        string strDateTmp = strTzjlInfoH[i].Substring(strTzjlInfoH[i].Length - 12, 12);
                        if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strTzdate), DateTimes.FormatStringToDateTime(strDateTmp));
                        {
                            if (iErr < 180 && strMeterIbQ[i] != "0" && strMeterIbH[i] == "0" && Convert.ToInt32(iLaZhaCount1H[i]) == Convert.ToInt32(iLaZhaCount1Q[i]) + 1
                                && ResultDictionary["继电器命令状态位前一后1"][i] == "通-断"
                                && strMeterGLQ[i] != "0" && strMeterGLQ[i] != "-0" && (strMeterGLH[i] == "0" || strMeterGLH[i] == "-0"))
                            {
                                blnRet[i, 0] = true;
                            }
                        }
                    }
                    ResultDictionary["拉闸延时中断电后重新上电电能表立刻跳闸1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                }
                UploadTestResult("拉闸延时中断电后重新上电电能表立刻跳闸1");



                //2----------------------------------------
                if (Stop) return;
                PowerOn();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (result[i])
                    {
                        strData[i] = "正常应答";
                        blnRet[i, 1] = true;
                    }
                    else
                    {
                        strData[i] = "异常应答";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "拉闸状态下受远程拉闸命令2", strData);


                //拉闸状态下受远程拉闸命令2|


                //3----------------

                //先发送合闸命令

                if (Stop) return;
                int[] iLaZhaCount3Q = ReadMeterLaZhaCount();

                if (Stop) return;
                Common.Memset(ref strData, "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
                bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                strMeterIbQ = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbQ = Common.StringConverToDecimaByIb(strMeterIbQ);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterIbQ[i]))
                    {
                        ResultDictionary["表内电流前一后3（内置）"][i] = strMeterIbQ[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后3（内置）"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
                strMeterGLQ = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
                strMeterGLQ = Common.StringConverToDecimaByGL(strMeterGLQ);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterGLQ[i]))
                    {
                        ResultDictionary["电能表功率前一后3（内置）"][i] = strMeterGLQ[i];
                    }
                    else
                    {
                        ResultDictionary["电能表功率前一后3（内置）"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后3"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后3"][i] = "通";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后3"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后3"][i] = "通";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后3"][i] = "异常";
                        ResultDictionary["继电器状态位前一后3"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为2分钟,请稍候....");
                Common.Memset(ref strCode, "04001401");
                Common.Memset(ref strDataPut, "04001401" + "0002");
                Common.Memset(ref strData, "0002");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发拉闸延时时间3", strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["拉闸延时2分钟"][i] = result[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("拉闸延时2分钟");


                if (Stop) return;
                Common.Memset(ref strDataPut, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");

                //Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内负荷开关误动作总次数,请稍候....");
                string[] strMeterWDZQ = MeterProtocolAdapter.Instance.ReadData("03360000", 3);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterWDZQ[i]))
                    {
                        ResultDictionary["负荷开关误动作总次数前一后3（内置）"][i] = strMeterWDZQ[i];
                    }
                    else
                    {
                        ResultDictionary["负荷开关误动作总次数前一后3（内置）"][i] = "异常";
                    }
                }

                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strDataPut);
                MessageController.Instance.AddMessage("请观察拉闸延时中，背光应点亮，“拉闸”字符闪烁。拉闸后，背光应熄灭，拉闸指示灯亮。");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                strTzdate = DateTime.Now.ToString("yyMMddHHmmss");//记录
                Common.Memset(ref strWDZBTime, DateTime.Now.ToString("yyMMddHHmmss"));
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 9);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取拉闸延时时间,请稍候....");
                string[] strTzysDate = MeterProtocolAdapter.Instance.ReadData("04001401", 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发拉闸延时时间3", strTzysDate);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取跳闸事件记录,请稍候....");
                string[] strTzjlInfo = MeterProtocolAdapter.Instance.ReadData("1D00FF01", 34);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次跳闸记录3", strTzjlInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内负荷开关误动作总次数,请稍候....");
                string[] strMeterWDZH = MeterProtocolAdapter.Instance.ReadData("03360000", 3);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterWDZH[i]))
                    {
                        ResultDictionary["负荷开关误动作总次数前一后3（内置）"][i] += "-" + strMeterWDZH[i];
                    }
                    else
                    {
                        ResultDictionary["负荷开关误动作总次数前一后3（内置）"][i] += "-异常";
                    }
                }
                UploadTestResult("负荷开关误动作总次数前一后3（内置）");

                if (Stop) return;
                int[] iLaZhaCount3H = ReadMeterLaZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                strMeterIbH = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbH = Common.StringConverToDecimaByIb(strMeterIbH);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
                strMeterGLH = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
                strMeterGLH = Common.StringConverToDecimaByGL(strMeterGLH);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterGLH[i]))
                    {
                        ResultDictionary["电能表功率前一后3（内置）"][i] += "-" + strMeterGLH[i];
                    }
                    else
                    {
                        ResultDictionary["电能表功率前一后3内置)"][i] += "-异常";
                    }
                }
                UploadTestResult("电能表功率前一后3（内置）");

                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOff();


                for (int i = 0; i < iCount; i++)
                {
                    bool BlnBj = false;
                    for (int j = 0; j < 6; j++)
                    {
                        int iBw = (6 * i) + j;
                        if (Helper.MeterDataHelper.Instance.Meter(iBw).YaoJianYn)
                        {
                            BlnBj = true;
                            break;
                        }
                    }

                    if (BlnBj)
                    {
                        MessageBox.Show("正在准备检测继电器状态,请在台体上没有电流的情况下准备好第" + (i * 6 + 1) + "-" + ((i + 1) * 6) + "表位中要检表位的检测环境后按确定。");
                        if (Stop) return;
                        PowerOn();
                        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 9);

                    }

                    for (int j = 0; j < 6; j++)
                    {
                        int iBw = (6 * i) + j;

                        if (!Helper.MeterDataHelper.Instance.Meter(iBw).YaoJianYn) continue;
                        MessageController.Instance.AddMessage("正在读取第" + (iBw + 1) + "表位状态字3,请稍候....");
                        if (Stop) return;
                        status3[iBw] = MeterProtocolAdapter.Instance.ReadDataByPos("04000503", 2, iBw);
                        
                    }

                    if (BlnBj)
                    {
                        if (Stop) return;
                        Helper.EquipHelper.Instance.PowerOff();
                        MessageBox.Show("表继电器状态检测完成,请把表重新挂回台体后按确定。");
                        for (int j = 0; j < 6; j++)
                        {
                            int iBw = (6 * i) + j;
                            strWDZETime[iBw] = DateTime.Now.ToString("yyMMddHHmmss");
                        }
                    }
                }

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后3"][i] += "-断";

                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后3"][i] += "-通";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器状态位前一后3"][i] += "-异常";
                    }
                }
                UploadTestResult("继电器状态位前一后3");

                if (Stop) return;
                PowerOn();

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 9);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内上1次开关误动作记录发生时刻,请稍候....");
                string[] strMeterWDZInfo = MeterProtocolAdapter.Instance.ReadData("03360001", 12);
                string[] strMeterWDZBTime = new string[BwCount];
                string[] strMeterWDZETime = new string[BwCount];

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterWDZInfo[i]) && strMeterWDZInfo[i].Length >= 24)
                    {
                        strMeterWDZBTime[i] = strMeterWDZInfo[i].Substring(strMeterWDZInfo[i].Length - 12, 12);
                        strMeterWDZETime[i] = strMeterWDZInfo[i].Substring(0, 12);
                        ResultDictionary["上1次开关误动作记录发生时刻3（内置）"][i] = strMeterWDZBTime[i];
                        ResultDictionary["上1次开关误动作记录结束时刻3（内置）"][i] = strMeterWDZETime[i];
                    }
                    else
                    {
                        ResultDictionary["上1次开关误动作记录发生时刻3（内置）"][i] = "异常";
                        ResultDictionary["上1次开关误动作记录结束时刻3（内置）"][i] = "异常";
                    }

                    if (!string.IsNullOrEmpty(strMeterWDZBTime[i]) && strMeterWDZBTime[i] != "000000000000" && !string.IsNullOrEmpty(strMeterWDZETime[i])
                        && strMeterWDZETime[i] != "000000000000" && strWDZETime[i] != null)
                    {
                        int iBErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strMeterWDZBTime[i]), DateTimes.FormatStringToDateTime(strWDZBTime[i]));
                        int iEErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strMeterWDZETime[i]), DateTimes.FormatStringToDateTime(strWDZETime[i]));

                        if (iBErr < 180 && iEErr < 180)
                        //if (iBErr < 180)
                        {
                            ResultDictionary["开关误动作记录3（内置）"][i] = "正确";
                        }
                        else
                        {
                            ResultDictionary["开关误动作记录3（内置）"][i] = "错误";
                        }
                    }
                    else
                    {
                        ResultDictionary["开关误动作记录3（内置）"][i] = "异常";
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "模拟开关误动作发生时刻3（内置）", strWDZBTime);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "模拟开关误动作结束时刻3（内置）", strWDZETime);
                UploadTestResult("上1次开关误动作记录发生时刻3（内置）");
                UploadTestResult("上1次开关误动作记录结束时刻3（内置）");
                UploadTestResult("开关误动作记录3（内置）");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["跳闸延时3"][i] = "正确";
                            ResultDictionary["继电器命令状态位前一后3"][i] += "-断";

                        }
                        else
                        {
                            ResultDictionary["跳闸延时3"][i] = "错误";
                            ResultDictionary["继电器命令状态位前一后3"][i] += "-通";
                        }

                    }
                    else
                    {
                        ResultDictionary["跳闸延时3"][i] = "异常";
                        ResultDictionary["继电器命令状态位前一后3"][i] += "-异常";
                    }

                    if (!string.IsNullOrEmpty(strMeterIbH[i]))
                    {
                        ResultDictionary["表内电流前一后3（内置）"][i] += "-" + strMeterIbH[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后3（内置）"][i] += "-异常";
                    }

                    ResultDictionary["跳闸次数前一后3"][i] = iLaZhaCount3Q[i] + "-" + iLaZhaCount3H[i].ToString();

                }
                UploadTestResult("继电器命令状态位前一后3");
                UploadTestResult("跳闸延时3");
                UploadTestResult("跳闸次数前一后3");
                UploadTestResult("表内电流前一后3（内置）");
                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "跳闸信号输出3（外置）", strData);



                //拉闸延时2分钟|下发拉闸延时时间3|上1次跳闸记录3|跳闸延时3|跳闸次数前-后3|跳闸信号输出3|延时拉闸3

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["延时拉闸3"][i] = "不通过";
                    if (string.IsNullOrEmpty(strTzjlInfo[i]) || strTzjlInfo[i].ToString().Length < 68) continue;
                    string strDateTmp = strTzjlInfo[i].Substring(strTzjlInfo[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strTzdate), DateTimes.FormatStringToDateTime(strDateTmp));

                    if (iErr < 180 && strMeterIbQ[i] != "0" && strMeterIbH[i] == "0" && ResultDictionary["继电器命令状态位前一后3"][i] == "通-断" && ResultDictionary["继电器状态位前一后3"][i] == "通-断"
                        && Convert.ToInt32(iLaZhaCount3H[i]) == Convert.ToInt32(iLaZhaCount3Q[i]) + 1
                        && strMeterGLQ[i] != "0" && strMeterGLQ[i] != "-0" && (strMeterGLH[i] == "0" || strMeterGLH[i] == "-0") && Convert.ToInt32(strMeterWDZH[i]) == Convert.ToInt32(strMeterWDZQ[i]) + 1
                        && ResultDictionary["开关误动作记录3（内置）"][i] == "正确")
                    {
                        blnRet[i, 3] = true;
                    }

                    ResultDictionary["延时拉闸3"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }

                UploadTestResult("延时拉闸3");


                //4----------------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "1C00" + strDateTime);
                bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 0);

                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 3])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }

                }
                UploadTestResult("结论");



                #endregion

            }
            else
            {
                #region 外置表
                //1------------


                if (Stop) return;
                int[] iLaZhaCount1Q = ReadMeterLaZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] = "通";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x001) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后1"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后1"][i] = "通";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后1"][i] = "异常";
                        ResultDictionary["继电器状态位前一后1"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为30分钟,请稍候....");
                Common.Memset(ref strDataPut, "04001401" + "0030");
                Common.Memset(ref strCode, "04001401");
                bool[] resultTime = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["拉闸延时30分钟1"][i] = resultTime[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("拉闸延时30分钟1");

                if (Stop) return;
                Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));

                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
                if (Stop) return;
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["远程拉闸命令1"][i] = result[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("远程拉闸命令1");
                if (Stop) return;
                bool blnResult = Helper.EquipHelper.Instance.SetRelayControl(1);

                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                MessageController.Instance.AddMessage("正在断电,请稍候....");
                Helper.EquipHelper.Instance.PowerOff();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 14);
                if (Stop) return;
                PowerOn();
                string strTzdate = DateTime.Now.ToString("yyMMddHHmmss");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                int[] iLaZhaCount1H = ReadMeterLaZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] += "-断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后1"][i] += "-通";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后1"][i] += "-断";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后1"][i] += "-通";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后1"][i] += "-异常";
                        ResultDictionary["继电器状态位前一后1"][i] += "-异常";
                    }
                }
                UploadTestResult("继电器命令状态位前一后1");
                //UploadTestResult("继电器状态位前一后1");

                //清除表位清除预付费跳闸状态
                //Helper.EquipHelper.Instance.ClearBwStatus(Helper.MeterDataHelper.Instance.GetYaoJian(), 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取负荷控制开关输出信号,请稍候....");
                CLDC_DeviceDriver.stError[] stStatus = Helper.EquipHelper.Instance.ReadWcb(false);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    blnFhkg[i] = stStatus[i].statusTypeIsOnErr_Yfftz;
                    //if (GlobalUnit.DeviceManufacturers.ToString().Contains("科陆"))
                    //{
                    //    if (GlobalUnit.CheckControlType == "0")
                    //    {
                    //        if (GlobalUnit.IsDan)
                    //        {
                    //            ResultDictionary["跳闸信号输出1"][i] = !blnFhkg[i] ? "是" : "否";
                    //        }
                    //        else
                    //        {
                    //            ResultDictionary["跳闸信号输出1"][i] = blnFhkg[i] ? "是" : "否";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (GlobalUnit.IsDan)
                    //        {
                    //            ResultDictionary["跳闸信号输出1"][i] = blnFhkg[i] ? "是" : "否";
                    //        }
                    //        else
                    //        {
                    //            ResultDictionary["跳闸信号输出1"][i] = !blnFhkg[i] ? "是" : "否";
                    //        }
                    //    }
                    //}
                    //else
                    {
                        ResultDictionary["跳闸信号输出1（外置）"][i] = blnFhkg[i] ? "是" : "否";
                    }


                    ResultDictionary["跳闸次数前一后1"][i] = iLaZhaCount1Q[i].ToString() + "-" + iLaZhaCount1H[i].ToString();
                }

                UploadTestResult("跳闸次数前一后1");
                UploadTestResult("跳闸信号输出1（外置）");

                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电能表功率前一后1（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内电流前一后1（内置）", strData);

                //拉闸延时30分钟1|远程拉闸命令1|上1次跳闸记录1|跳闸次数前-后1|跳闸信号输出1|拉闸延时中断电后重新上电电能表立刻跳闸1|


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取跳闸事件记录,请稍候....");
                string[] strTzjlInfoH = MeterProtocolAdapter.Instance.ReadData("1D00FF01", 38);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次跳闸记录1", strTzjlInfoH);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strTzjlInfoH[i]) || strTzjlInfoH[i].ToString().Length < 68
                        || string.IsNullOrEmpty(iLaZhaCount1Q[i].ToString()) || string.IsNullOrEmpty(iLaZhaCount1H[i].ToString())) continue;
                    string strDateTmp = strTzjlInfoH[i].Substring(strTzjlInfoH[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strTzdate), DateTimes.FormatStringToDateTime(strDateTmp));
                    //if (GlobalUnit.DeviceManufacturers.ToString().Contains("科陆"))
                    //{
                    //    if (GlobalUnit.IsDan)
                    //    {
                    //        if (iErr < 180 && !blnFhkg[i] && Convert.ToInt32(iLaZhaCount1H[i]) == Convert.ToInt32(iLaZhaCount1Q[i]) + 1
                    //            && ResultDictionary["继电器命令状态位测试前一测试后1"][i]=="通-断")
                    //        {
                    //            blnRet[i, 0] = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (iErr < 180 && blnFhkg[i] && Convert.ToInt32(iLaZhaCount1H[i]) == Convert.ToInt32(iLaZhaCount1Q[i]) + 1
                    //            && ResultDictionary["继电器命令状态位测试前一测试后1"][i] == "通-断")
                    //        {
                    //            blnRet[i, 0] = true;
                    //        }
                    //    }
                    //}
                    //else
                    {
                        if (iErr < 180 && blnFhkg[i] && Convert.ToInt32(iLaZhaCount1H[i]) == Convert.ToInt32(iLaZhaCount1Q[i]) + 1
                            && ResultDictionary["继电器命令状态位前一后1"][i] == "通-断" && ResultDictionary["继电器状态位前一后1"][i] == "通-断")
                        {
                            blnRet[i, 0] = true;
                        }
                        ResultDictionary["拉闸延时中断电后重新上电电能表立刻跳闸1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                    }
                }

                UploadTestResult("拉闸延时中断电后重新上电电能表立刻跳闸1");

                //2----------------------------------------
                if (Stop) return;
                Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (result[i])
                    {
                        strData[i] = "正常应答";
                        blnRet[i, 1] = true;
                    }
                    else
                    {
                        strData[i] = "异常应答";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "拉闸状态下受远程拉闸命令2", strData);

                //拉闸状态下受远程拉闸命令2|


                //3----------------

                //先发送合闸命令

                if (Stop) return;
                int[] iLaZhaCount3Q = ReadMeterLaZhaCount();



                if (Stop) return;
                Common.Memset(ref strData, "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
                bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后3"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后3"][i] = "通";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后3"][i] = "断";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后3"][i] = "通";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后3"][i] = "异常";
                        ResultDictionary["继电器状态位前一后3"][i] = "异常";
                    }
                }

                PowerOn();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为2分钟,请稍候....");
                Common.Memset(ref strCode, "04001401");
                Common.Memset(ref strDataPut, "04001401" + "0002");
                Common.Memset(ref strData, "0002");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发拉闸延时时间3", strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["拉闸延时2分钟"][i] = result[i] ? "正常应答" : "异常应答";
                }

                UploadTestResult("拉闸延时2分钟");

                blnResult = Helper.EquipHelper.Instance.SetRelayControl(1);

                if (Stop) return;
                Common.Memset(ref strDataPut, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");

                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strDataPut);
                MessageController.Instance.AddMessage("请观察拉闸延时中，背光应点亮，“拉闸”字符闪烁。拉闸后，背光应熄灭，拉闸指示灯亮。");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                strTzdate = DateTime.Now.ToString("yyMMddHHmmss");//记录
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取拉闸延时时间,请稍候....");
                string[] strTzysDate = MeterProtocolAdapter.Instance.ReadData("04001401", 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发拉闸延时时间3", strTzysDate);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取跳闸事件记录,请稍候....");
                string[] strTzjlInfo = MeterProtocolAdapter.Instance.ReadData("1D00FF01", 34);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次跳闸记录3", strTzjlInfo);

                //清除表位清除预付费跳闸状态
                //Helper.EquipHelper.Instance.ClearBwStatus(Helper.MeterDataHelper.Instance.GetYaoJian(), 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取负荷控制开关输出信号,请稍候....");
                stStatus = Helper.EquipHelper.Instance.ReadWcb(false);


                if (Stop) return;
                int[] iLaZhaCount3H = ReadMeterLaZhaCount();



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["跳闸延时3"][i] = "正确";
                            ResultDictionary["继电器命令状态位前一后3"][i] += "-断";

                        }
                        else
                        {
                            ResultDictionary["跳闸延时3"][i] = "错误";
                            ResultDictionary["继电器命令状态位前一后3"][i] += "-通";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["继电器状态位前一后3"][i] += "-断";

                        }
                        else
                        {
                            ResultDictionary["继电器状态位前一后3"][i] += "-通";
                        }
                    }
                    else
                    {
                        ResultDictionary["跳闸延时3"][i] = "异常";
                        ResultDictionary["继电器命令状态位前一后3"][i] += "-异常";
                        ResultDictionary["继电器状态位前一后3"][i] += "-异常";
                    }
                    blnFhkg[i] = stStatus[i].statusTypeIsOnErr_Yfftz;
                    //if (GlobalUnit.DeviceManufacturers.ToString().Contains("科陆"))
                    //{
                    //    if (GlobalUnit.IsDan)
                    //    {
                    //        ResultDictionary["跳闸信号输出3"][i] = !blnFhkg[i] ? "是" : "否";
                    //    }
                    //    else
                    //    {
                    //        ResultDictionary["跳闸信号输出3"][i] = blnFhkg[i] ? "是" : "否";
                    //    }
                    //}
                    //else
                    {
                        ResultDictionary["跳闸信号输出3（外置）"][i] = blnFhkg[i] ? "是" : "否";
                    }
                    ResultDictionary["跳闸次数前一后3"][i] = iLaZhaCount3Q[i] + "-" + iLaZhaCount3H[i].ToString();


                }
                UploadTestResult("继电器命令状态位前一后3");
                UploadTestResult("继电器状态位前一后3");
                UploadTestResult("跳闸延时3");
                UploadTestResult("跳闸信号输出3（外置）");
                UploadTestResult("跳闸次数前一后3");

                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电能表功率前一后3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "负荷开关误动作总次数前一后3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "模拟开关误动作发生时刻3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次开关误动作记录发生时刻3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "模拟开关误动作结束时刻3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次开关误动作记录结束时刻3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开关误动作记录3（内置）", strData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内电流前一后3（内置）", strData);

                //拉闸延时2分钟|下发拉闸延时时间3|上1次跳闸记录3|跳闸延时3|跳闸次数前-后3|跳闸信号输出3|延时拉闸3

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["延时拉闸3"][i] = "不通过";
                    if (string.IsNullOrEmpty(strTzjlInfo[i]) || strTzjlInfo[i].ToString().Length < 68) continue;
                    string strDateTmp = strTzjlInfo[i].Substring(strTzjlInfo[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strTzdate), DateTimes.FormatStringToDateTime(strDateTmp));
                    //if (GlobalUnit.DeviceManufacturers.ToString().Contains("科陆"))
                    //{
                    //    if (GlobalUnit.IsDan)
                    //    {
                    //        if (iErr < 180 && !blnFhkg[i]  && ResultDictionary["继电器命令状态位测试前一测试后3"][i] == "通-断")
                    //        {
                    //            blnRet[i, 3] = true;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (iErr < 180 && blnFhkg[i] && ResultDictionary["继电器命令状态位测试前一测试后3"][i] == "通-断")
                    //        {
                    //            blnRet[i, 3] = true;
                    //        }
                    //    }
                    //}
                    //else
                    {
                        if (iErr < 180 && blnFhkg[i] && ResultDictionary["继电器命令状态位前一后3"][i] == "通-断" && ResultDictionary["继电器状态位前一后3"][i] == "通-断")
                        {
                            blnRet[i, 2] = true;
                        }
                    }
                    ResultDictionary["延时拉闸3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                }

                UploadTestResult("延时拉闸3");

                //--合闸---

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "1C00" + strDateTime);
                bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);




                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }

                }
                UploadTestResult("结论");
                #endregion
            }
        }


        /// <summary>
        /// 读取拉闸次数
        /// </summary>
        /// <returns></returns>
        private int[] ReadMeterLaZhaCount()
        {
            //if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数....");
            string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1D000001", 3);
            int[] iLaZhaCount = new int[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strlaZhaCount[i])) continue;
                string strTmp = strlaZhaCount[i];
                if (strTmp != "000000")
                {
                    strTmp = strTmp.TrimStart('0');
                    iLaZhaCount[i] = Convert.ToInt32(strTmp);
                }
                else
                {
                    iLaZhaCount[i] = 0;
                }
            }
            return iLaZhaCount;
        }


        /// <summary>
        /// 读取合闸次数
        /// </summary>
        /// <returns></returns>
        private int[] ReadMeterHeZhaCount()
        {
            //if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸次数....");
            string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1E000001", 3);
            int[] iLaZhaCount = new int[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strlaZhaCount[i])) continue;
                string strTmp = strlaZhaCount[i];
                if (strTmp != "000000")
                {
                    strTmp = strTmp.TrimStart('0');
                    iLaZhaCount[i] = Convert.ToInt32(strTmp);
                }
                else
                {
                    iLaZhaCount[i] = 0;
                }
            }
            return iLaZhaCount;
        }

    }
}
