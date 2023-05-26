using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;
using System.Windows.Forms;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 合闸复电功能测试(交互终端)
    /// </summary>
    public class TerminalRecoverElectricity : VerifyBase
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

        public TerminalRecoverElectricity(object plan)
            : base(plan)
        {
        }

        //继电器命令状态位测试前一测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前一后|合闸复电(交互终端)


        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "继电器状态位测试前一测试后","表内电流前一后（内置）", "合闸信号输出",
                                         "命令下发时间","上1次合闸记录", "合闸次数前一后","合闸复电（交互终端）",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {
            if (GlobalUnit.IsNZLoadRelayControl)
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
            PowerOn();

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string strParaFile = ""; //参数信息文件
            string strwalletFile = "";//钱包文件
            string strpriceFile1 = "";//当前套电价文件
            string strpriceFile2 = "";//备用套电价文件
            string strControlFilePlain = ""; //合闸明文
            bool[,] blnRet = new bool[BwCount, 3];
            string[] strRevCode = new string[BwCount];
            string[] status = new string[BwCount];
            string[] strMac = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] result = new bool[BwCount];
            string[] outData = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strJdqStatus = new string[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            string[] strCardNo = new string[BwCount];
            string[] strFhkg = new string[BwCount];
            bool[] blnFhkg = new bool[BwCount];

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            //准备        
            #region 准备
            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
            Common.Memset(ref strData, "04001401" + "0000");
            Common.Memset(ref strRevCode, "04001401");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            bool blnResult = Helper.EquipHelper.Instance.SetRelayControl(1);

            if (Stop) return;
            Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            int[] iHeZhaCount3Q = ReadMeterHeZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3");
            status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x0040) != 0x0040)
                    {
                        strJdqStatus[i] = "通";
                    }
                    else
                    {
                        strJdqStatus[i] = "断";
                        blnRet[i, 0] = true;
                    }
                }
            }

            #endregion

            blnResult = Helper.EquipHelper.Instance.SetRelayControl(2);

            #region    通过交互终端下发合闸复电命令
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            bool[] BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            MessageController.Instance.AddMessage("正在通过交互终端下发合闸复电命令,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[0] = "00";            //保留
                paraFile[1] = "8F";            //参数更新标志位
                paraFile[2] = "00000000";      //保留
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[4] = "00";            //保留
                paraFile[5] = "00005000";      //报警金额1
                paraFile[6] = "00004000";      //报警金额2
                paraFile[7] = "000001";        //电流互感器变比
                paraFile[8] = "000001";        //电压互感器变比
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                paraFile[10] = "112233445566"; //客户编号
                paraFile[11] = "00";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                walletFile[0] = "00000000";    //购电金额
                walletFile[1] = "00000000";    //购电次数


                for (int j = 0; j < 12; j++)   //费率1-12
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00000100";
                    priceFile2[j] = "00000100";
                }
                for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 25; j < 31; j++)  //年第1-6结算日
                {
                    priceFile1[j] = "010101";
                    priceFile2[j] = "010101";
                }
                for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00000100";
                    priceFile2[j] = "00000100";
                }
                for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 44; j < 50; j++) //年第1-6结算日
                {
                    priceFile1[j] = "010101";
                    priceFile2[j] = "010101";
                }
                priceFile1[50] = "0000000000"; //保留
                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间
                ControlFilePlain[0] = "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");

                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            bool[] WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在交互终端与表身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentityAuthentication(iFlag, out strRand1, out strRand2, out strEsamNo);
            if (Stop) return;
            result = MeterProtocolAdapter.Instance.SouthReadUserCardMAC(strRand2, out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strControlFilePlainArr);

            if (Stop) return;
            string strHzDate = DateTime.Now.ToString("yyMMddHHmmss");
            MessageController.Instance.AddMessage("正在模拟交互终端下发合闸复电命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthTerminalSendHzfd(strControlFilePlainArr);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

            #endregion


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
                //MessageController.Instance.AddMessage("正在读取合闸状态字3");
                //status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    //if (!string.IsNullOrEmpty(status[i]))
                    //{
                    //    if ((Convert.ToInt32(status[i], 16) & 0x0010) != 0x0010)
                    //    {
                    //        strJdqStatus[i] += "-通";
                    //        blnRet[i, 1] = true;
                    //    }
                    //    else
                    //    {
                    //        strJdqStatus[i] += "-断";
                    //    }
                    //}
                    //else
                    //{
                    //    strJdqStatus[i] += "-异常";
                    //}

                    ResultDictionary["合闸信号输出"][i] = "该项不启用";
                    ResultDictionary["继电器状态位测试前一测试后"][i] = "该项不启用";
                    ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
                }
                UploadTestResult("合闸信号输出");
                UploadTestResult("继电器状态位测试前一测试后");
                UploadTestResult("合闸次数前一后");


                //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|合闸复电(交互终端)

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取合闸事件记录");
                string[] strHzsjInfo = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 34);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjInfo);

                Common.Memset(ref strData, strHzDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strData);


                //判断结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["合闸复电（交互终端）"][i] = "不通过";
                    if (!string.IsNullOrEmpty(strHzsjInfo[i]) && strHzsjInfo[i].Length >= 68 && !string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString()))
                    {
                        string strdateTmp = strHzsjInfo[i].Substring(strHzsjInfo[i].Length - 12, 12);
                        if (strdateTmp != "000000000000")
                        {
                            int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate), DateTimes.FormatStringToDateTime(strdateTmp));
                            if (blnRet[i, 0] && iErr < 600 && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1
                                && strMeterIbQ[i] == "0" && strMeterIbH[i] != "0")
                            {
                                ResultDictionary["结论"][i] = "合格";
                                ResultDictionary["合闸复电（交互终端）"][i] = "通过";
                            }
                        }
                    }
                }
                UploadTestResult("合闸复电（交互终端）");
                UploadTestResult("结论");
                #endregion
            }
            else
            {
                #region 外置

                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 0.1f, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取负荷控制开关输出信号,请稍候....");
                CLDC_DeviceDriver.stError[] stStatus = Helper.EquipHelper.Instance.ReadWcb(false);

                if (Stop) return;
                int[] iHeZhaCount3H = ReadMeterHeZhaCount();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取合闸状态字3");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x0010) != 0x0010)
                        {
                            strJdqStatus[i] += "-通";
                            blnRet[i, 1] = true;
                        }
                        else
                        {
                            strJdqStatus[i] += "-断";
                        }
                    }
                    else
                    {
                        strJdqStatus[i] += "-异常";
                    }
                    blnFhkg[i] = stStatus[i].statusTypeIsOnErr_Yfftz;
                    {
                        strFhkg[i] = blnFhkg[i] ? "是" : "否";
                    }
                    if (strFhkg[i].Contains("是"))
                    {
                        blnRet[i, 2] = true;
                    }
                    ResultDictionary["合闸信号输出"][i] = strFhkg[i];
                    ResultDictionary["合闸复电（交互终端）"][i] = ResultDictionary["合闸信号输出"][i].Equals("是") ? "成功" : "失败";
                    ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
                }
                UploadTestResult("合闸信号输出");
                UploadTestResult("合闸次数前一后");
                UploadTestResult("合闸复电（交互终端）");

                //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|合闸复电(交互终端)

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位测试前一测试后", strJdqStatus);
                Common.Memset(ref strData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内电流前一后（内置）", strData);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取合闸事件记录");
                string[] strHzsjInfo = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 34);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjInfo);

                Common.Memset(ref strData, strHzDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strData);


                //判断结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["结论"][i] = "不合格";
                    if (!string.IsNullOrEmpty(strHzsjInfo[i]) && strHzsjInfo[i].Length >= 68 && !string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString()))
                    {
                        string strdateTmp = strHzsjInfo[i].Substring(strHzsjInfo[i].Length - 12, 12);
                        if (strdateTmp != "000000000000")
                        {
                            int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate), DateTimes.FormatStringToDateTime(strdateTmp));
                            if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && iErr < 180 && ResultDictionary["合闸信号输出"][i].Equals("是") && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1)
                            {
                                ResultDictionary["结论"][i] = "合格";
                            }
                        }
                    }
                }
                UploadTestResult("结论");
                #endregion
            }
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
