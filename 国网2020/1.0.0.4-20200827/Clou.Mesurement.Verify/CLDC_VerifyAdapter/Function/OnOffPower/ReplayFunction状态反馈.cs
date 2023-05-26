using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using System.Windows.Forms;

namespace CLDC_VerifyAdapter.Function.OnOffPower
{
    class ReplayFunction: VerifyBase
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

        public ReplayFunction(object plan)
            : base(plan)
        {
        }

        //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|合闸允许命令


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "继电器状态位测试前一测试后","表内电流前一后（内置）","合闸信号输出","命令下发时间","上1次合闸记录","合闸次数前一后","合闸允许命令",
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
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 2];
            int[] iFlag = new int[BwCount];
            string[] status3 = new string[BwCount];
            string[] statusTmp = new string[BwCount];
            string[] strFhkg = new string[BwCount];
            bool[] blnFhkg = new bool[BwCount];
            string[] strHzDate = new string[BwCount];
            string[] strDataPut = new string[BwCount];
            bool[] result = new bool[BwCount];

            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

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
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
            Common.Memset(ref strCode, "04001401");
            Common.Memset(ref strDataPut, "04001401" + "0000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);

            if (Stop) return;
            int[] iHeZhaCount3Q = ReadMeterHeZhaCount();


            if (Stop) return;
            Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                    {
                        statusTmp[i] = "断";
                    }
                    else
                    {
                        statusTmp[i] = "通";
                    }
                }
                else
                {
                    statusTmp[i] = "异常";
                }
            }

            Helper.EquipHelper.Instance.SetRelayControl(2);

            if (Stop) return;
            Common.Memset(ref strData, "1B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发合闸允许命令,请稍候....");
            string strXfTime = DateTime.Now.ToString("yyMMddHHmmss");
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                string[] strMeterIbQ = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbQ = Common.StringConverToDecimaByIb(strMeterIbQ);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterIbQ[i]))
                    {
                        ResultDictionary["表内电流前一后（内置）"][i] = strMeterIbQ[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后（内置）"][i] = "异常";
                    }
                }

                MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);

                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 0.1f, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                int[] iHeZhaCount3H = ReadMeterHeZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
                string[] strMeterIbH = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
                strMeterIbH = Common.StringConverToDecimaByIb(strMeterIbH);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strMeterIbH[i]))
                    {
                        ResultDictionary["表内电流前一后（内置）"][i] += "-" + strMeterIbH[i];
                    }
                    else
                    {
                        ResultDictionary["表内电流前一后（内置）"][i] += "-异常";
                    }
                }
                UploadTestResult("表内电流前一后（内置）");

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                //status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    //if (!string.IsNullOrEmpty(status3[i]))
                    //{
                    //    if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                    //    {
                    //        statusTmp[i] += "-通";
                    //        blnRet[i, 0] = true;
                    //    }
                    //    else
                    //    {
                    //        statusTmp[i] += "-断";
                    //    }
                    //}
                    //else
                    //{
                    //    statusTmp[i] += "-异常";
                    //}

                    ResultDictionary["合闸信号输出"][i] = "该项不启用";
                    ResultDictionary["继电器状态位测试前一测试后"][i] = "该项不启用";
                    ResultDictionary["合闸允许命令"][i] = bln_Rst[i] ? "成功" : "失败";
                    ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
                }
                UploadTestResult("合闸信号输出");
                UploadTestResult("继电器状态位测试前一测试后");
                UploadTestResult("合闸次数前一后");
                UploadTestResult("合闸允许命令");

                //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|合闸允许命令

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取合闸事件记录,请稍候....");
                string[] strHzsjjl = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 38);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjjl);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strHzDate);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strHzsjjl[i]) || strHzsjjl[i].ToString().Length < 68) continue;
                    string strDateTmp = strHzsjjl[i].Substring(strHzsjjl[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate[i]), DateTimes.FormatStringToDateTime(strDateTmp));

                    if (iErr < 600 && ResultDictionary["合闸允许命令"][i].Equals("成功") && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1
                        && strMeterIbQ[i] == "0" && strMeterIbH[i] != "0")
                    {
                        blnRet[i, 0] = true;
                    }
                }

                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0])
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
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 0.1f, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                #region 外置
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取负荷控制开关输出信号,请稍候....");
                CLDC_DeviceDriver.stError[] stStatus = Helper.EquipHelper.Instance.ReadWcb(false);

                if (Stop) return;
                int[] iHeZhaCount3H = ReadMeterHeZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            statusTmp[i] += "-通";
                            blnRet[i, 0] = true;
                        }
                        else
                        {
                            statusTmp[i] += "-断";
                        }
                    }
                    else
                    {
                        statusTmp[i] += "-异常";
                    }
                    blnFhkg[i] = stStatus[i].statusTypeIsOnErr_Yfftz;
                    {
                        strFhkg[i] = blnFhkg[i] ? "是" : "否";
                    }
                    ResultDictionary["合闸信号输出"][i] += strFhkg[i];

                    ResultDictionary["合闸允许命令"][i] = bln_Rst[i] ? "成功" : "失败";
                    ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
                }
                UploadTestResult("合闸信号输出");
                UploadTestResult("合闸次数前一后");
                UploadTestResult("合闸允许命令");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位测试前一测试后", statusTmp);
                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内电流前一后（内置）", strData);
                //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|合闸允许命令

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取合闸事件记录,请稍候....");
                string[] strHzsjjl = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 38);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjjl);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strHzDate);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strHzsjjl[i]) || strHzsjjl[i].ToString().Length < 68) continue;
                    string strDateTmp = strHzsjjl[i].Substring(strHzsjjl[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate[i]), DateTimes.FormatStringToDateTime(strDateTmp));

                    if (iErr < 180 && ResultDictionary["合闸允许命令"][i].Equals("成功") && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1
                        && ResultDictionary["合闸信号输出"][i] == "是")
                    {
                        blnRet[i, 1] = true;
                    }
                }


                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0] && blnRet[i, 1])
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


