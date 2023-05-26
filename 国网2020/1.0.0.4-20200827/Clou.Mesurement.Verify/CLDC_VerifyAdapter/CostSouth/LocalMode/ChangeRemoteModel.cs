using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 本地模式切换远程模式
    /// </summary>
    public class ChangeRemoteModel : VerifyBase
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

        public ChangeRemoteModel(object plan)
            : base(plan)
        {
        }
        //密钥状态1|费控模式状态位切换前-切换后1|预跳闸远程报警状态位切换前-切换后1|开关状态切换前-切换后1|
        //剩余金额切换前-切换后1|模式切换命令下发时间1|上1次两套费控模式切换时间1|液晶显示切换1|
        //正式密钥下本地切远程1|
        //密钥状态2|费控模式状态位切换前-切换后2|预跳闸远程报警状态位切换前-切换后2|保电状态位切换前-切换后2|
        //剩余金额切换前-切换后2|模式切换命令下发时间2|上1次两套费控模式切换时间2||液晶显示切换2|
        //测试密钥下本地切远程2


        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "密钥状态1","费控模式状态位切换前一切换后1","预跳闸远程报警状态位切换前一切换后1","继电器状态位切换前一切换后1",
                                        "剩余金额切换前一切换后1","远程开户状态位切换前一切换后1","模式切换命令下发时间1","上1次两套费控模式切换时间1",
                                        "正式密钥下本地切远程1",
                                        "密钥状态2","费控模式状态位切换前一切换后2","预跳闸远程报警状态位切换前一切换后2","保电状态位切换前一切换后2",
                                        "剩余金额切换前一切换后2","模式切换命令下发时间2","上1次两套费控模式切换时间2",
                                        "测试密钥下本地切远程2",
                                            "结论" };
            return true;
        }

        public override void Verify()
        {


            base.Verify();
            if (Stop) return;
            PowerOn();

            string[] strRand1 = new string[BwCount];//随机数1
            string[] strRand2 = new string[BwCount];//随机数2
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] strRevCode = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strMoney = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 3];
            string[] status3 = new string[BwCount];
            string[] status1 = new string[BwCount];
            string[] strMac = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] outData = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] status = new string[BwCount];
            string[] strFkstatusQ = new string[BwCount];
            string[] strFkstatusH = new string[BwCount];
            string[] strHzStatusQ = new string[BwCount];
            string[] strYcbjStatusQ = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount];
            string[] strEnerZQ = new string[BwCount];
            string[] strZH1Q = new string[BwCount];
            string[] strHzStatusH = new string[BwCount];
            string[] strYcbjStatusH = new string[BwCount];
            string[] strSyMoneyH = new string[BwCount];
            string[] strEnerZH = new string[BwCount];
            string[] strZH1H = new string[BwCount];
            string[] strBdStatusQ = new string[BwCount];
            string[] strBdStatusH = new string[BwCount];
            string[] strChangDateArr = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string strChangDate = "";
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] strShowData = new string[BwCount];

            #region 准备



            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


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
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化100元,请稍候....");
            Common.Memset(ref strData, "00002710");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            #endregion

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置
                                //1---------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                Common.Memset(ref iFlag, 1);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行购电次数=0的远程开户,请稍候....");
                //购电金额+购电次数+客户编号
                Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
                bool[] blnKhRet = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);



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

                //Helper.EquipHelper.Instance.SetRelayControl(1);

                if (Stop) return;
                Common.Memset(ref  strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime);
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMac, out strRevMac, out BuyCount, out strSyMoneyQ);
                strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取当前正向有功总电能,请稍候....");
                //strEnerZQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取组合无功1总电能,请稍候....");
                //strZH1Q = MeterProtocolAdapter.Instance.ReadData("00030000", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {

                        strYcbjStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";

                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] = "开户";
                        }
                    }
                    else
                    {
                        strYcbjStatusQ[i] = "异常";
                        ResultDictionary["远程开户状态位切换前一切换后1"][i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000000");
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                bool[] bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                if (Stop) return;
                PowerOn();


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMac, out strRevMac, out BuyCount, out strSyMoneyH);
                strSyMoneyH = Common.HexConverToDecimalism(strSyMoneyH);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        strYcbjStatusH[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-开户";
                        }
                    }
                    else
                    {
                        strYcbjStatusH[i] = "异常";
                        ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }
                    ResultDictionary["费控模式状态位切换前一切换后1"][i] = strFkstatusQ[i] + "-" + strFkstatusH[i];
                    ResultDictionary["预跳闸远程报警状态位切换前一切换后1"][i] = strYcbjStatusQ[i] + "-" + strYcbjStatusH[i];
                    ResultDictionary["剩余金额切换前一切换后1"][i] = strSyMoneyQ[i] + "-" + strSyMoneyH[i];
                    ResultDictionary["模式切换命令下发时间1"][i] = strChangDate;
                    ResultDictionary["上1次两套费控模式切换时间1"][i] = strChangDateArr[i];

                }


                Common.Memset(ref strShowData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位切换前一切换后1", strShowData);

                UploadTestResult("费控模式状态位切换前一切换后1");
                UploadTestResult("预跳闸远程报警状态位切换前一切换后1");
                UploadTestResult("剩余金额切换前一切换后1");
                UploadTestResult("模式切换命令下发时间1");
                UploadTestResult("上1次两套费控模式切换时间1");
                UploadTestResult("远程开户状态位切换前一切换后1");


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDate), DateTimes.FormatStringToDateTime(strChangDateArr[i]));
                    if (iErr < 360 && ResultDictionary["预跳闸远程报警状态位切换前一切换后1"][i].Equals("有-无")
                        && ResultDictionary["远程开户状态位切换前一切换后1"][i] == "开户-开户" && strSyMoneyH[i] == "0" && bChangRet[i])
                    {
                        blnRet[i, 0] = true;
                        ResultDictionary["正式密钥下本地切远程1"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["正式密钥下本地切远程1"][i] = "不通过";
                    }
                }
                UploadTestResult("正式密钥下本地切远程1");


                //2----电能表为本地费控模式,测试密钥下,保电状态,下发远程模式切换命令

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                Common.Memset(ref iFlag, 0);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

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

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000000");
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);               


                if (Stop) return;
                MessageController.Instance.AddMessage("正在测试密钥状态下进行钱包初始化,请稍候....");
                string[] strGdCount = new string[BwCount];
                Common.Memset(ref strData, "00002710");
                result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime);
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        strYcbjStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";

                    }
                    else
                    {
                        strHzStatusQ[i] = "异常";
                        strYcbjStatusQ[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        strBdStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000) ? "保电" : "非保电";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }

                if (Stop) return;
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000000");
                bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表运行状态字3");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            strBdStatusH[i] = "保电";
                            blnRet[i, 1] = true;
                        }
                        else
                        {
                            strBdStatusH[i] = "非保电";

                        }
                    }
                    else
                    {
                        strBdStatusH[i] = "异常";
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
                        strYcbjStatusH[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";
                    }
                    else
                    {
                        strYcbjStatusH[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }

                    ResultDictionary["费控模式状态位切换前一切换后2"][i] = strFkstatusQ[i] + "-" + strFkstatusH[i];
                    ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = strYcbjStatusQ[i] + "-" + strYcbjStatusH[i];
                    ResultDictionary["保电状态位切换前一切换后2"][i] = strBdStatusQ[i] + "-" + strBdStatusH[i];
                    ResultDictionary["剩余金额切换前一切换后2"][i] = strSyMoneyQ[i] + "-" + strSyMoneyH[i];
                    ResultDictionary["模式切换命令下发时间2"][i] = strChangDate;
                    ResultDictionary["上1次两套费控模式切换时间2"][i] = strChangDateArr[i];


                }



                UploadTestResult("费控模式状态位切换前一切换后2");
                UploadTestResult("预跳闸远程报警状态位切换前一切换后2");
                UploadTestResult("保电状态位切换前一切换后2");
                UploadTestResult("剩余金额切换前一切换后2");
                UploadTestResult("模式切换命令下发时间2");
                UploadTestResult("上1次两套费控模式切换时间2");

                //for (int i = 0; i < BwCount; i++)
                //{
                //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //    strData[i] = bChangRet[i] ? "成功" : "失败";
                //}


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDate), DateTimes.FormatStringToDateTime(strChangDateArr[i]));
                    if (iErr < 360 && strYcbjStatusH[i] == "无" && strSyMoneyH[i] == "0"
                        && ResultDictionary["保电状态位切换前一切换后2"][i] == "保电-保电"
                        && bChangRet[i])
                    {
                        blnRet[i, 2] = true;
                        ResultDictionary["测试密钥下本地切远程2"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["测试密钥下本地切远程2"][i] = "不通过";
                    }
                }
                UploadTestResult("测试密钥下本地切远程2");

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
            else
            {
                #region 外置
                //1---------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                Common.Memset(ref iFlag, 1);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行购电次数=0的远程开户,请稍候....");
                //购电金额+购电次数+客户编号
                Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
                bool[] blnKhRet = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);



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

                //Helper.EquipHelper.Instance.SetRelayControl(1);

                if (Stop) return;
                Common.Memset(ref  strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime);
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMac, out strRevMac, out BuyCount, out strSyMoneyQ);
                strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取当前正向有功总电能,请稍候....");
                //strEnerZQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取组合无功1总电能,请稍候....");
                //strZH1Q = MeterProtocolAdapter.Instance.ReadData("00030000", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0040) != 0x0040)
                        {
                            ResultDictionary["继电器状态位切换前一切换后1"][i] = "合闸";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位切换前一切换后1"][i] = "拉闸";
                        }

                        strYcbjStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";

                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] = "开户";
                        }
                    }
                    else
                    {
                        strHzStatusQ[i] = "异常";
                        strYcbjStatusQ[i] = "异常";
                        ResultDictionary["远程开户状态位切换前一切换后1"][i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000000");
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                bool[] bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                if (Stop) return;
                PowerOn();


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMac, out strRevMac, out BuyCount, out strSyMoneyH);
                strSyMoneyH = Common.HexConverToDecimalism(strSyMoneyH);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取当前正向有功总电能,请稍候....");
                //strEnerZH = MeterProtocolAdapter.Instance.ReadData("05090101", 4);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在读取组合无功1总电能,请稍候....");
                //strZH1H = MeterProtocolAdapter.Instance.ReadData("05090301", 4);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            ResultDictionary["继电器状态位切换前一切换后1"][i] += "-合闸";
                        }
                        else
                        {
                            ResultDictionary["继电器状态位切换前一切换后1"][i] += "-拉闸";
                        }
                        strYcbjStatusH[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-开户";
                        }
                    }
                    else
                    {
                        strHzStatusH[i] = "异常";
                        strYcbjStatusH[i] = "异常";
                        ResultDictionary["远程开户状态位切换前一切换后1"][i] += "-异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }
                    ResultDictionary["费控模式状态位切换前一切换后1"][i] = strFkstatusQ[i] + "-" + strFkstatusH[i];
                    ResultDictionary["预跳闸远程报警状态位切换前一切换后1"][i] = strYcbjStatusQ[i] + "-" + strYcbjStatusH[i];
                    ResultDictionary["剩余金额切换前一切换后1"][i] = strSyMoneyQ[i] + "-" + strSyMoneyH[i];
                    ResultDictionary["模式切换命令下发时间1"][i] = strChangDate;
                    ResultDictionary["上1次两套费控模式切换时间1"][i] = strChangDateArr[i];




                    //ResultDictionary["正向有功总电能切换前-切换时刻"][i] = strEnerZQ[i] + "-" + strEnerZH[i];
                    //ResultDictionary["组合无功1总电能切换前-切换时刻"][i] = strZH1Q[i] + "-" + strZH1H[i];
                }
                UploadTestResult("费控模式状态位切换前一切换后1");
                UploadTestResult("预跳闸远程报警状态位切换前一切换后1");
                UploadTestResult("继电器状态位切换前一切换后1");
                UploadTestResult("剩余金额切换前一切换后1");
                UploadTestResult("模式切换命令下发时间1");
                UploadTestResult("上1次两套费控模式切换时间1");
                UploadTestResult("远程开户状态位切换前一切换后1");

                //密钥状态1|费控模式状态位切换前-切换后1|预跳闸远程报警状态位切换前-切换后1|开关状态切换前-切换后1|
                //剩余金额切换前-切换后1|模式切换命令下发时间1|上1次两套费控模式切换时间1|液晶显示切换1|
                //正式密钥下本地切远程1|




                //UploadTestResult("正向有功总电能切换前-切换时刻");
                //UploadTestResult("组合无功1总电能切换前-切换时刻");

                //for (int i = 0; i < BwCount; i++)
                //{
                //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //    //strData[i] = bChangRet[i] ? "成功" : "失败";
                //}

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDate), DateTimes.FormatStringToDateTime(strChangDateArr[i]));
                    if (iErr < 360 && ResultDictionary["预跳闸远程报警状态位切换前一切换后1"][i].Equals("有-无")
                        && ResultDictionary["继电器状态位切换前一切换后1"][i].Equals("拉闸-合闸") && ResultDictionary["远程开户状态位切换前一切换后1"][i] == "开户-开户"
                        && strSyMoneyH[i] == "0"
                        && bChangRet[i])
                    {
                        blnRet[i, 0] = true;
                        ResultDictionary["正式密钥下本地切远程1"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["正式密钥下本地切远程1"][i] = "不通过";
                    }
                }
                UploadTestResult("正式密钥下本地切远程1");


                //2----电能表为本地费控模式,测试密钥下,保电状态,下发远程模式切换命令

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                Common.Memset(ref iFlag, 0);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

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

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程模式切换本地模式命令,请稍候....");
                Common.Memset(ref strData, "00" + "00000000" + "00000000");
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在测试密钥状态下进行钱包初始化,请稍候....");
                string[] strGdCount = new string[BwCount];
                Common.Memset(ref strData, "00002710");
                result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime);
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        strHzStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040) ? "断" : "通";
                        strYcbjStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";

                    }
                    else
                    {
                        strHzStatusQ[i] = "异常";
                        strYcbjStatusQ[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusQ[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        strBdStatusQ[i] = ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000) ? "保电" : "非保电";
                    }
                    else
                    {
                        strFkstatusQ[i] = "异常";
                    }
                }

                if (Stop) return;
                strChangDate = DateTime.Now.ToString("yyMMddHHmm");
                MessageController.Instance.AddMessage("正在下发本地模式切换远程模式命令,请稍候....");
                Common.Memset(ref strData, "01" + "00000000" + "00000000");
                bChangRet = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次两套费控模式切换时间,请稍候....");
                strChangDateArr = MeterProtocolAdapter.Instance.ReadData("05090001", 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表运行状态字3");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取费控模式状态位");
                status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            strBdStatusH[i] = "保电";
                            blnRet[i, 1] = true;
                        }
                        else
                        {
                            strBdStatusH[i] = "非保电";

                        }
                    }
                    else
                    {
                        strBdStatusH[i] = "异常";
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
                        // strHzStatusH[i] = ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010) ? "断" : "通";
                        strYcbjStatusH[i] = ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080) ? "有" : "无";
                    }
                    else
                    {
                        strHzStatusH[i] = "异常";
                        strYcbjStatusH[i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(status1[i]))
                    {
                        strFkstatusH[i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    }
                    else
                    {
                        strFkstatusH[i] = "异常";
                    }

                    ResultDictionary["费控模式状态位切换前一切换后2"][i] = strFkstatusQ[i] + "-" + strFkstatusH[i];
                    ResultDictionary["预跳闸远程报警状态位切换前一切换后2"][i] = strYcbjStatusQ[i] + "-" + strYcbjStatusH[i];
                    ResultDictionary["保电状态位切换前一切换后2"][i] = strBdStatusQ[i] + "-" + strBdStatusH[i];
                    ResultDictionary["剩余金额切换前一切换后2"][i] = strSyMoneyQ[i] + "-" + strSyMoneyH[i];
                    ResultDictionary["模式切换命令下发时间2"][i] = strChangDate;
                    ResultDictionary["上1次两套费控模式切换时间2"][i] = strChangDateArr[i];


                }
                UploadTestResult("费控模式状态位切换前一切换后2");
                UploadTestResult("预跳闸远程报警状态位切换前一切换后2");
                UploadTestResult("保电状态位切换前一切换后2");
                UploadTestResult("剩余金额切换前一切换后2");
                UploadTestResult("模式切换命令下发时间2");
                UploadTestResult("上1次两套费控模式切换时间2");

                //for (int i = 0; i < BwCount; i++)
                //{
                //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //    strData[i] = bChangRet[i] ? "成功" : "失败";
                //}


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strChangDate), DateTimes.FormatStringToDateTime(strChangDateArr[i]));
                    if (iErr < 360 && strYcbjStatusH[i] == "无" && strSyMoneyH[i] == "0"
                        && ResultDictionary["保电状态位切换前一切换后2"][i] == "保电-保电"
                        && bChangRet[i])
                    {
                        blnRet[i, 2] = true;
                        ResultDictionary["测试密钥下本地切远程2"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["测试密钥下本地切远程2"][i] = "不通过";
                    }
                }
                UploadTestResult("测试密钥下本地切远程2");

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
    }
}
