using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程模式切换本地模式
    /// </summary>
    public class ChangeLocalMode : VerifyBase
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

        public ChangeLocalMode(object plan)
            : base(plan)
        {
        }

        //密钥状态1|正式密钥下不可远程切本地1|
        //密钥状态2|费控模式状态位切换前-切换后2|预跳闸远程报警状态位切换前-切换后2|保电状态位切换前-切换后2|跳闸信号输出2|
        //模式切换命令下发时间2|上1次两套费控模式切换时间2|液晶显示切换2|测试密钥下可远程切本地2|远程切本地操作正确2|
        //保电下切换后保电状态3|非保电下切换后保电状态3|远程切本地不清保电3

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "密钥状态1","正式密钥下不可远程切本地1",
                                         "密钥状态2","费控模式状态位切换前一切换后2","预跳闸远程报警状态位切换前一切换后2","保电状态位切换前一切换后2","继电器状态位切换前一切换后2",
                                         "模式切换命令下发时间2","上1次两套费控模式切换时间2","测试密钥下可远程切本地2","远程切本地操作正确2",
                                         "保电下切换后保电状态3","非保电下切换后保电状态3","远程切本地不清保电3",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {
            
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand2 = new string[BwCount];//随机数
            string[] strRand1 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] strRevCode = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 3];
            string[] status = new string[BwCount];
            string[] status2 = new string[BwCount];
            string[] strMac = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int[] iFlag = new int[BwCount];
            string[] outData = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            bool[] result = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strBjStatus = new string[BwCount];
            string[] strBdStatus = new string[BwCount];
            string[] status1 = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strPutApdu = new string[BwCount];

            string[] strHzStatusQ = new string[BwCount];
            string[] strHzStatusH = new string[BwCount];
            string[] FkStatus = new string[BwCount];
           

            #region 准备

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(status[i])) continue;
                iSelectBwCount++;
                if (status[i].EndsWith("1FFFF"))
                {
                    rstTmp[i] = true;
                }
            }

            if (Array.IndexOf(rstTmp, true) > -1)
            {
                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (Stop) return;
                        if (rstTmp[i])
                        {
                            MessageController.Instance.AddMessage("正在第" + (i + 1) + "表位密钥恢复,请稍候....");
                            bool blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "00", 17, strRand2[i], strEsamNo[i]);
                            iFlag[i] = 0;
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                    bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 0);
                }
            }

            if (Stop) return;
            Common.Memset(ref strRevCode, "DF010001002D0001");
            MessageController.Instance.AddMessage("正在读取费控模式状态字");
            bool[] Ret = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out FkStatus, out strRevMac);

            iSelectBwCount = 0;
            Common.Memset(ref rstTmp, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iSelectBwCount++;
                if (FkStatus[i] == "01")
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
                            MessageController.Instance.AddMessage("正在对第" + (i + 1) + "块表下发模式切换到本地模式命令,请稍候....");
                            bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlag[i], strRand2[i], strData[i]);
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发模式切换命令切换到本地模式,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                }
            }

            

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额1,请稍候....");
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, "00000000");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, ("00000000"));
            Common.Memset(ref strPutApdu, "04D6811408");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候...."); 
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001005");
            Common.Memset(ref strData, "04001005" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            PowerOn();

            //if (Stop) return;
            //string[] strHzDate = new string[BwCount];
            //MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
            //string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            //Common.Memset(ref strData, "1C00" + strDateTime);
            //Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
            //bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程报警解除命令,请稍候....");
            string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "2B00" + strDateTime);
            bool[] blnBjRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            Common.Memset(ref strRevCode, "DF010001002D0001");
            MessageController.Instance.AddMessage("正在读取费控模式状态字");
            Ret = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out FkStatus, out strRevMac);

            iSelectBwCount = 0;
            Common.Memset(ref rstTmp, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iSelectBwCount++;
                if (FkStatus[i] != "01")
                {
                    rstTmp[i] = true;
                }
            }

            if (Array.IndexOf(rstTmp, true) > -1)
            {
                Common.Memset(ref strData, "01" + "00000000" + "00000000");

                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (Stop) return;
                        if (rstTmp[i])
                        {
                            MessageController.Instance.AddMessage("正在对第" + (i + 1) + "块表下发模式切换命令,请稍候....");
                            bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlag[i], strRand2[i], strData[i]);
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发模式切换命令切换到远程模式,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);



            #endregion

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置
                //1--------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);



                if (Stop) return;
                MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
                MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(MyStatus[i]))
                    {
                        if (MyStatus[i].EndsWith("1FFFF"))
                        {
                            MyStatus[i] = "正式密钥";
                        }
                        else
                        {
                            MyStatus[i] = "测试密钥";
                        }
                    }
                    else
                    {
                        MyStatus[i] = "异常";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态1", MyStatus);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (MyStatus[i] != "正式密钥")
                        {
                            ResultDictionary["正式密钥下不可远程切本地1"][i] = "异常";
                            blnRet[i, 0] = false;
                        }
                        else
                        {
                            blnRet[i, 0] = !result[i];
                            ResultDictionary["正式密钥下不可远程切本地1"][i] = !result[i] ? "通过" : "不通过";
                        }
                    }
                }

                UploadTestResult("正式密钥下不可远程切本地1");


                //2---------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字1,请稍候....");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status1[i]))
                        {
                            if ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040)
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] = "远程";
                            }
                            else
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] = "本地";
                            }
                        }
                        else
                        {
                            ResultDictionary["费控模式状态位切换前一切换后2"][i] = "异常";
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] = "非保电";
                            }
                            if ((Convert.ToInt32(status[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "有";
                            }
                            else
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "无";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电状态位切换前一切换后2"][i] = "异常";
                            ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "异常";
                        }
                    }
                }




                if (Stop) return;
                MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
                MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(MyStatus[i]))
                    {
                        if (MyStatus[i].EndsWith("1FFFF"))
                        {
                            MyStatus[i] = "正式密钥";
                        }
                        else
                        {
                            MyStatus[i] = "测试密钥";
                        }
                    }
                    else
                    {
                        MyStatus[i] = "异常";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态2", MyStatus);

                //Helper.EquipHelper.Instance.SetRelayControl(1);
                if (Stop) return;
                PowerOn();

                //发送切换本地模式命令
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,过程中请查看液晶显示切换,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                string strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                string[] strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] += "-保电";
                                ResultDictionary["非保电下切换后保电状态3"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] += "-非保电";
                                ResultDictionary["非保电下切换后保电状态3"][i] = "非保电";
                            }

                            if ((Convert.ToInt32(status[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-有";
                            }
                            else
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-无";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电状态位切换前一切换后2"][i] += "-异常";
                            ResultDictionary["非保电下切换后保电状态3"][i] = "异常";
                            ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-异常";
                        }
                    }
                }

                Common.Memset(ref strShowData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位切换前一切换后2", strShowData);

                UploadTestResult("预跳闸远程报警状态位切换前一切换后2");
                UploadTestResult("保电状态位切换前一切换后2");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字1,请稍候....");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status1[i]))
                        {
                            if ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040)
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-远程";

                            }
                            else
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-本地";
                            }
                        }
                        else
                        {
                            ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-异常";
                        }
                    }
                }

                UploadTestResult("费控模式状态位切换前一切换后2");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDateArr[i]), DateTimes.FormatStringToDateTime(strChangDate));

                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if (result[i] && ResultDictionary["费控模式状态位切换前一切换后2"][i] == "远程-本地"
                            && ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] == "无-有"
                            && ResultDictionary["保电状态位切换前一切换后2"][i] == "非保电-非保电"
                            && iErr < 300)
                        {
                            blnRet[i, 1] = true;

                        }

                    }
                    if (MyStatus[i] != "测试密钥")
                    {
                        ResultDictionary["测试密钥下可远程切本地2"][i] = "异常";
                        ResultDictionary["远程切本地操作正确2"][i] = "异常";
                    }
                    else
                    {
                        ResultDictionary["测试密钥下可远程切本地2"][i] = blnRet[i, 1] ? "成功" : "失败";

                        ResultDictionary["远程切本地操作正确2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                    }

                    ResultDictionary["模式切换命令下发时间2"][i] = strChangDate;

                    ResultDictionary["上1次两套费控模式切换时间2"][i] = strChangDateArr[i];




                }

                UploadTestResult("测试密钥下可远程切本地2");
                UploadTestResult("远程切本地操作正确2");
                UploadTestResult("模式切换命令下发时间2");
                UploadTestResult("上1次两套费控模式切换时间2");




                //3-------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
                Common.Memset(ref strData, "04001401" + "0000");
                Common.Memset(ref strRevCode, "04001401");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,过程中请查看液晶显示切换,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下切换后保电状态3"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电下切换后保电状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下切换后保电状态3"][i] = "异常";
                        }
                    }

                }
                UploadTestResult("保电下切换后保电状态3");

                UploadTestResult("非保电下切换后保电状态3");


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    ResultDictionary["远程切本地不清保电3"][i] = "不通过";

                    if (result[i] && ResultDictionary["保电下切换后保电状态3"][i].Equals("保电"))
                    {
                        blnRet[i, 2] = true;
                        ResultDictionary["远程切本地不清保电3"][i] = "通过";
                    }

                }
                UploadTestResult("远程切本地不清保电3");



                //处理结果
                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {

                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                }
                UploadTestResult("结论");

                #endregion
            }
            else
            {
                #region 外置
                //1--------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);



                if (Stop) return;
                MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
                MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(MyStatus[i]))
                    {
                        if (MyStatus[i].EndsWith("1FFFF"))
                        {
                            MyStatus[i] = "正式密钥";
                        }
                        else
                        {
                            MyStatus[i] = "测试密钥";
                        }
                    }
                    else
                    {
                        MyStatus[i] = "异常";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态1", MyStatus);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (MyStatus[i] != "正式密钥")
                        {
                            ResultDictionary["正式密钥下不可远程切本地1"][i] = "异常";
                            blnRet[i, 0] = false;
                        }
                        else
                        {
                            blnRet[i, 0] = !result[i];
                            ResultDictionary["正式密钥下不可远程切本地1"][i] = !result[i] ? "通过" : "不通过";
                        }
                    }
                }

                UploadTestResult("正式密钥下不可远程切本地1");


                //2---------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字1,请稍候....");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status1[i]))
                        {
                            if ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040)
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] = "远程";
                            }
                            else
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] = "本地";
                            }
                        }
                        else
                        {
                            ResultDictionary["费控模式状态位切换前一切换后2"][i] = "异常";
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] = "非保电";
                            }
                            if ((Convert.ToInt32(status[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "有";
                            }
                            else
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "无";
                            }
                            if ((Convert.ToInt32(status[i], 16) & 0x0010) != 0x0010)
                            {
                                ResultDictionary["继电器状态位切换前一切换后2"][i] = "合闸";
                            }
                            else
                            {
                                ResultDictionary["继电器状态位切换前一切换后2"][i] = "拉闸";
                            }

                            strHzStatusQ[i] = ((Convert.ToInt32(status[i], 16) & 0x0040) == 0x0040) ? "断" : "通";
                        }
                        else
                        {
                            ResultDictionary["保电状态位切换前一切换后2"][i] = "异常";
                            ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = "异常";
                            strHzStatusQ[i] = "异常";
                        }
                    }
                }




                if (Stop) return;
                MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
                MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(MyStatus[i]))
                    {
                        if (MyStatus[i].EndsWith("1FFFF"))
                        {
                            MyStatus[i] = "正式密钥";
                        }
                        else
                        {
                            MyStatus[i] = "测试密钥";
                        }
                    }
                    else
                    {
                        MyStatus[i] = "异常";
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态2", MyStatus);

                //Helper.EquipHelper.Instance.SetRelayControl(1);
                if (Stop) return;
                PowerOn();

                //发送切换本地模式命令
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,过程中请查看液晶显示切换,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                string strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                string[] strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] += "-保电";
                                ResultDictionary["非保电下切换后保电状态3"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态位切换前一切换后2"][i] += "-非保电";
                                ResultDictionary["非保电下切换后保电状态3"][i] = "非保电";
                            }

                            if ((Convert.ToInt32(status[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-有";
                            }
                            else
                            {
                                ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-无";
                            }
                            if ((Convert.ToInt32(status[i], 16) & 0x0010) != 0x0010)
                            {
                                ResultDictionary["继电器状态位切换前一切换后2"][i] += "-合闸";
                            }
                            else
                            {
                                ResultDictionary["继电器状态位切换前一切换后2"][i] += "-拉闸";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电状态位切换前一切换后2"][i] += "-异常";
                            ResultDictionary["非保电下切换后保电状态3"][i] = "异常";
                            ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] += "-异常";
                        }
                    }
                }
                UploadTestResult("预跳闸远程报警状态位切换前一切换后2");
                UploadTestResult("继电器状态位切换前一切换后2");
                UploadTestResult("保电状态位切换前一切换后2");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字1,请稍候....");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status1[i]))
                        {
                            if ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040)
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-远程";

                            }
                            else
                            {
                                ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-本地";
                            }
                        }
                        else
                        {
                            ResultDictionary["费控模式状态位切换前一切换后2"][i] += "-异常";
                        }
                    }
                }

                UploadTestResult("费控模式状态位切换前一切换后2");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDateArr[i]), DateTimes.FormatStringToDateTime(strChangDate));

                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if (result[i] && ResultDictionary["费控模式状态位切换前一切换后2"][i] == "远程-本地"
                            && ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] == "无-有"
                            && ResultDictionary["保电状态位切换前一切换后2"][i] == "非保电-非保电"
                            && ResultDictionary["继电器状态位切换前一切换后2"][i] == "合闸-拉闸"
                            && iErr < 300)
                        {
                            blnRet[i, 1] = true;

                        }

                    }
                    if (MyStatus[i] != "测试密钥")
                    {
                        ResultDictionary["测试密钥下可远程切本地2"][i] = "异常";
                        ResultDictionary["远程切本地操作正确2"][i] = "异常";
                    }
                    else
                    {
                        ResultDictionary["测试密钥下可远程切本地2"][i] = blnRet[i, 1] ? "成功" : "失败";

                        ResultDictionary["远程切本地操作正确2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                    }

                    ResultDictionary["模式切换命令下发时间2"][i] = strChangDate;

                    ResultDictionary["上1次两套费控模式切换时间2"][i] = strChangDateArr[i];




                }

                UploadTestResult("测试密钥下可远程切本地2");
                UploadTestResult("远程切本地操作正确2");
                UploadTestResult("模式切换命令下发时间2");
                UploadTestResult("上1次两套费控模式切换时间2");




                //3-------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
                Common.Memset(ref strData, "04001401" + "0000");
                Common.Memset(ref strRevCode, "04001401");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,过程中请查看液晶显示切换,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000001");
                result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status[i]))
                        {
                            if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下切换后保电状态3"][i] = "保电";
                            }
                            else
                            {
                                ResultDictionary["保电下切换后保电状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下切换后保电状态3"][i] = "异常";
                        }
                    }

                }
                UploadTestResult("保电下切换后保电状态3");

                UploadTestResult("非保电下切换后保电状态3");


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    ResultDictionary["远程切本地不清保电3"][i] = "不通过";

                    if (result[i] && ResultDictionary["保电下切换后保电状态3"][i].Equals("保电"))
                    {
                        blnRet[i, 2] = true;
                        ResultDictionary["远程切本地不清保电3"][i] = "通过";
                    }

                }
                UploadTestResult("远程切本地不清保电3");



                //处理结果
                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {

                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                }
                UploadTestResult("结论");

                #endregion
            }
        }
    }
}
