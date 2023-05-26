using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 远程开户
    /// </summary>
    public class RemoteInitUser : VerifyBase
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

        public RemoteInitUser(object plan)
            : base(plan)
        {

        }
        
        //开户前远程开户状态位1|开户后远程开户状态位1|测试密钥下不可远程开户1|开户前远程开户状态位2|开户后远程开户状态位2|正式密钥下购电次数＞1不可远程开户2|囤积金额限值3
        //剩余金额3|充值200超囤积不可远程开户3|开户下发客户编号4|开户下发购电次数4|开户下发购电金额4|开户前远程开户状态位4|开户后远程开户状态位4|开户后客户编号4|开户前购电次数4
        //开户后购电次数4|开户前剩余金额4|开户后剩余金额4|上2次购电日期4|上2次购电金额4|正式密钥下开户且不充值4|开户下发客户编号5|开户下发购电次数5|开户下发购电金额5|开户前远程开户状态位5
        //开户后远程开户状态位5|开户后客户编号5|开户前购电次数5|开户后购电次数5|开户前剩余金额5|开户后剩余金额5|上1次购电日期5|上1次购电后总购电次数5|上1次购电金额5
        //上1次购电前剩余金额5|上1次购电后剩余金额5|上1次购电后累计购电金额5|正式密钥下开户且充值5|第1次开户下发购电次数6|第1次开户下发购电金额6|第1次开户下发客户编号6
        //第2次开户下发购电次数6|第2次开户下发购电金额6|第2次开户下发客户编号6|第2次开户前远程开户状态位6|第2次开户后远程开户状态位6|第2次开户后客户编号6|第2次开户前购电次数6
        //第2次开户后购电次数6|第2次开户前剩余金额6|第2次开户后剩余金额6|上1次购电日期6|上1次购电后总购电次数6|上1次购电金额6|上1次购电前剩余金额6|上1次购电后剩余金额6
        //上1次购电后累计购电金额6|已开户下再次远程开户充值6|刷开户卡前远程开户状态位7|刷开户卡前本地开户状态位7|刷开户卡后本地开户状态位7|已远程开户再本地开户7
        protected override bool CheckPara()
        {

            ResultNames = new string[] { "开户前远程开户状态位1","开户后远程开户状态位1","测试密钥下不可远程开户1",
                                         "开户前远程开户状态位2","开户后远程开户状态位2","正式密钥下购电次数＞1不可远程开户2",
                                         "远程开户状态位前一后3","囤积金额限值3","剩余金额3","充值200超囤积不可远程开户3",
                                         "开户下发客户编号4","开户下发购电次数4","开户下发购电金额4","开户前远程开户状态位4","开户后远程开户状态位4","开户后客户编号4","开户前购电次数4",
                                         "开户后购电次数4","开户前剩余金额4","开户后剩余金额4","上2次购电日期4","上2次购电金额4","正式密钥下开户且不充值4",
                                         "开户下发客户编号5","开户下发购电次数5","开户下发购电金额5","开户前远程开户状态位5","开户后远程开户状态位5","开户后客户编号5","开户前购电次数5",
                                         "开户后购电次数5","开户前剩余金额5","开户后剩余金额5","上1次购电日期5","上1次购电后总购电次5","上1次购电金额5","上1次购电前剩余金额5","上1次购电后剩余金额5","上1次购电后累计购电金额5","正式密钥下开户且充值5",
                                         "第1次开户下发购电次数6","第1次开户下发购电金额6","第1次开户下发客户编号6","第2次开户下发购电次数6","第2次开户下发购电金额6","第2次开户下发客户编号6","第2次开户前远程开户状态位6","第2次开户后远程开户状态位6",
                                         "第2次开户后客户编号6","第2次开户前购电次数6","第2次开户后购电次数6","第2次开户前剩余金额6","第2次开户后剩余金额6","上1次购电日期6","上1次购电后总购电次数6","上1次购电金额6","上1次购电前剩余金额6","上1次购电后剩余金额6","上1次购电后累计购电金额6","已开户下再次远程开户充值6",
                                         "刷开户卡前远程开户状态位7","刷开户卡前本地开户状态位7","刷开户卡后本地开户状态位7","已远程开户再本地开户7",
                                         "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
            string[] strSyMoneyH = new string[BwCount]; //当前剩余金额
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] strBuyMoney = new string[BwCount];
            string[] strBuyCount = new string[BwCount];
            string[] strGdCountQ = new string[BwCount]; //购电次数
            string[] strGdCountH = new string[BwCount]; //购电次数
            string[] FkStatus = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] status3 = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 7];
            int[] iFlag = new int[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
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
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            int iSelectBwCount = 0;
            string[] strErrInfo = new string[BwCount];
            string strIniUserDate = "";

            string[] strCardNo = new string[BwCount];
            string[] strParaInfo = new string[BwCount];
            bool[] blnSendTerminal = new bool[BwCount];
            string CardType = "03";

           #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置囤积金额限值=0元,请稍候....");
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
            #endregion

            //1--------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户前远程开户状态位1"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户前远程开户状态位1"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户前远程开户状态位1"][i] = "异常";
                }
            }
            UploadTestResult("开户前远程开户状态位1");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在测试密钥状态下进行购电次数=0的远程开户,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
            bool[] blnKhRet = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户后远程开户状态位1"][i] = "未开户";
                        ResultDictionary["开户前远程开户状态位2"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户后远程开户状态位1"][i] = "开户";
                        ResultDictionary["开户前远程开户状态位2"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户后远程开户状态位1"][i] = "异常";
                    ResultDictionary["开户前远程开户状态位2"][i] = "异常";
                }
            }
            UploadTestResult("开户后远程开户状态位1");
            UploadTestResult("开户前远程开户状态位2");


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnKhRet[i] && ResultDictionary["开户前远程开户状态位1"][i] == "未开户" && ResultDictionary["开户后远程开户状态位1"][i] == "未开户")
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["测试密钥下不可远程开户1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("测试密钥下不可远程开户1");

            //2--------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            //购电金额+购电次数+客户编号
            Common.Memset(ref strData, "00002710" + "00000002" + "112233445566");
            MessageController.Instance.AddMessage("正在正式密钥下,发送购电次数＞1、购电金额=100的远程开户命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户后远程开户状态位2"][i] = "未开户";
                        ResultDictionary["远程开户状态位前一后3"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户后远程开户状态位2"][i] = "开户";
                        ResultDictionary["远程开户状态位前一后3"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户后远程开户状态位2"][i] = "异常";
                    ResultDictionary["远程开户状态位前一后3"][i] = "异常";
                }
            }
            UploadTestResult("开户后远程开户状态位2");
            
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!result[i] && ResultDictionary["开户后远程开户状态位2"][i] == "未开户")
                    {
                        blnRet[i, 1] = true;
                    }
                    ResultDictionary["正式密钥下购电次数＞1不可远程开户2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                }
            }
            UploadTestResult("正式密钥下购电次数＞1不可远程开户2");

            //3------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置囤积金额限值=299元,请稍候....");
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00029900");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            //购电金额+购电次数+客户编号
            Common.Memset(ref strData, "00004E20" + "00000001" + "112233445566");
            MessageController.Instance.AddMessage("正在正式密钥下，发送购电次数=1、购电金额=200的远程开户命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
            string[] strTjMoneyXz = MeterProtocolAdapter.Instance.ReadData("04001004",4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值3", Common.StringConverToDecima(strTjMoneyXz));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            string[] strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额3", Common.StringConverToDecima(strSyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["远程开户状态位前一后3"][i] += "-未开户";
                    }
                    else
                    {
                        ResultDictionary["远程开户状态位前一后3"][i] += "-开户";
                    }
                }
                else
                {
                    ResultDictionary["远程开户状态位前一后3"][i] += "-异常";
                }
            }
            UploadTestResult("远程开户状态位前一后3");

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (ResultDictionary["远程开户状态位前一后3"][i] == "未开户-未开户" && !result[i])
                    {
                        blnRet[i, 2] = true;
                    }
                    ResultDictionary["充值200超囤积不可远程开户3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                }
            }
            UploadTestResult("充值200超囤积不可远程开户3");


            //4------------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户前远程开户状态位4"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户前远程开户状态位4"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户前远程开户状态位4"][i] = "异常";
                }
            }
            UploadTestResult("开户前远程开户状态位4");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前购电次数4", strGdCountQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前剩余金额4", Common.HexConverToDecimalism(strSyMoneyQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置囤积金额限值=0元,请稍候....");
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "112233445566");
            Common.Memset(ref strBuyCount, "00000000");
            Common.Memset(ref strBuyMoney, "100");
            Common.Memset(ref strData, "00002710" + "00000000" + "112233445566");
            MessageController.Instance.AddMessage("正在正式密钥下，发送购电次数=0、购电金额=100的远程开户命令（只开户不充值）,请稍候....");
            bool[] BintUser = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发客户编号4", strKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发购电次数4", strBuyCount);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发购电金额4", strBuyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户后远程开户状态位4"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户后远程开户状态位4"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户后远程开户状态位4"][i] = "异常";
                }
            }
            UploadTestResult("开户后远程开户状态位4");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上2次购电日期,请稍候....");
            string[] strBuyDateLast2 = MeterProtocolAdapter.Instance.ReadData("03330102", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2次购电日期4", strBuyDateLast2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
           string[] strBuyMoneyLast2 = MeterProtocolAdapter.Instance.ReadData("03330302", 4);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2次购电金额4", Common.StringConverToDecima(strBuyMoneyLast2));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后客户编号4", strRedMeterKhID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountH, out strSyMoneyH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后购电次数4", strGdCountH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后剩余金额4", Common.HexConverToDecimalism(strSyMoneyH));

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (ResultDictionary["开户后远程开户状态位4"][i] == "开户"
                        && strSyMoneyQ[i] == strSyMoneyH[i] && strRedMeterKhID[i] == strKhID[i]
                        && strGdCountQ[i] == strGdCountH[i] && strGdCountH[i]=="00000000"
                        && strBuyDateLast2[i] == "0000000000" && strBuyMoneyLast2[i]=="00000000")
                    {
                        blnRet[i, 3] = true;
                    }
                    ResultDictionary["正式密钥下开户且不充值4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
            }
            UploadTestResult("正式密钥下开户且不充值4");

            //5--------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在恢复密钥为公钥状态....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17,  strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData,"00004E20");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag,1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前购电次数5", strGdCountQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前剩余金额5", Common.HexConverToDecimalism(strSyMoneyQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户前远程开户状态位5"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户前远程开户状态位5"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户前远程开户状态位5"][i] = "异常";
                }
            }
            UploadTestResult("开户前远程开户状态位5");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行正式密钥状态下发购电次数=1、客户编号与上步骤不一致的远程开户命令（开户、充值）,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "665544332211");
            Common.Memset(ref strBuyCount, "00000001");
            Common.Memset(ref strBuyMoney, "100");
            Common.Memset(ref strData,"00002710" + "00000001" + "665544332211");
            BintUser = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发客户编号5", strKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发购电次数5", strBuyCount);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户下发购电金额5", strBuyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["开户后远程开户状态位5"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["开户后远程开户状态位5"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["开户后远程开户状态位5"][i] = "异常";
                }
            }
            UploadTestResult("开户后远程开户状态位5");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后客户编号5", strRedMeterKhID);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
            string[] strBuyDateLast1 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期5", strBuyDateLast1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
            string[] strBuyCountZLast1 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数5", strBuyCountZLast1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
            string[] strBuyMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额5", Common.StringConverToDecima(strBuyMoneyLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
            string[] strSyMoneyQLast1 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额5", Common.StringConverToDecima(strSyMoneyQLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            string[] strSyMoneyHLast1 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额5", Common.StringConverToDecima(strSyMoneyHLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
            string[] strSyMoneyZLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额5", Common.StringConverToDecima(strSyMoneyZLast1));


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountH, out strSyMoneyH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后购电次数5", strGdCountH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后剩余金额5", Common.HexConverToDecimalism(strSyMoneyH));

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (ResultDictionary["开户后远程开户状态位5"][i] == "开户" && strRedMeterKhID[i] == strKhID[i]
                        && strBuyCountZLast1[i] == "0001" && strSyMoneyZLast1[i] == "00030000" && strSyMoneyH[i]=="00007530")
                    {
                        blnRet[i, 4] = true;
                    }
                    ResultDictionary["正式密钥下开户且充值5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                }
            }
            UploadTestResult("正式密钥下开户且充值5");

            //6--------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在恢复密钥为公钥状态....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00004E20");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17,  strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在正式密钥下，发送购电次数=0、购电金额=150、客户编号为445566332211的远程开户命令（只开户不充值）,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "445566332211");
            Common.Memset(ref strBuyCount, "00000000");
            Common.Memset(ref strBuyMoney, "150");
            Common.Memset(ref strData, "00003A98" + "00000000" + "445566332211");
            BintUser = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户下发客户编号6", strKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户下发购电次数6", strBuyCount);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户下发购电金额6", strBuyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户前购电次数6", strGdCountQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户前剩余金额6", Common.HexConverToDecimalism(strSyMoneyQ));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["第2次开户前远程开户状态位6"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["第2次开户前远程开户状态位6"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["第2次开户前远程开户状态位6"][i] = "异常";
                }
            }
            UploadTestResult("第2次开户前远程开户状态位6");

            //充值
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送购电次数=1、购电金额=200、客户编号为112233445566的远程开户命令（开户、充值）,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "445566332211");
            Common.Memset(ref strBuyCount, "00000001");
            Common.Memset(ref strBuyMoney, "200");
            Common.Memset(ref strData, "00004E20" + "00000001" + "445566332211");
            strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");
            BintUser = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户下发客户编号6", strKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户下发购电次数6", strBuyCount);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户下发购电金额6", strBuyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["第2次开户后远程开户状态位6"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["第2次开户后远程开户状态位6"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["第2次开户后远程开户状态位6"][i] = "异常";
                }
            }
            UploadTestResult("第2次开户后远程开户状态位6");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后客户编号6", strRedMeterKhID);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
            strBuyDateLast1 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期6", strBuyDateLast1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
            strBuyCountZLast1 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数6", strBuyCountZLast1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
            strBuyMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额6", Common.StringConverToDecima(strBuyMoneyLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
            strSyMoneyQLast1 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额6", Common.StringConverToDecima(strSyMoneyQLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            strSyMoneyHLast1 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额6", Common.StringConverToDecima(strSyMoneyHLast1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
            strSyMoneyZLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额6", Common.StringConverToDecima(strSyMoneyZLast1));


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountH, out strSyMoneyH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后购电次数6", strGdCountH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后剩余金额6", Common.HexConverToDecimalism(strSyMoneyH));


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (strBuyDateLast1[i] == "0000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strIniUserDate), DateTimes.FormatStringToDateTime(strBuyDateLast1[i]));
                    if (ResultDictionary["第2次开户后远程开户状态位6"][i] == "开户" && strRedMeterKhID[i] == strKhID[i]
                        && strBuyCountZLast1[i] == "0001" && strSyMoneyZLast1[i] == "00040000" && strSyMoneyH[i] == "00009C40" && iErr < 300)
                    {
                        blnRet[i, 5] = true;
                    }
                    ResultDictionary["已开户下再次远程开户充值6"][i] = blnRet[i, 5] ? "通过" : "不通过";
                }
            }
            UploadTestResult("已开户下再次远程开户充值6");

            //7-------------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                    {
                        ResultDictionary["刷开户卡前远程开户状态位7"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["刷开户卡前远程开户状态位7"][i] = "开户";
                    }
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                    {
                        ResultDictionary["刷开户卡前本地开户状态位7"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["刷开户卡前本地开户状态位7"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["刷开户卡前远程开户状态位7"][i] = "异常";
                    ResultDictionary["刷开户卡前本地开户状态位7"][i] = "异常";
                }
            }
            UploadTestResult("刷开户卡前远程开户状态位7");
            UploadTestResult("刷开户卡前本地开户状态位7");


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            bool[] BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
            bool[] blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);
            MessageController.Instance.AddMessage("正在进行购电次数等于0的本地开户,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[0] = "00";            //保留
                paraFile[1] = "80";            //参数更新标志位
                paraFile[2] = "00000000";      //保留
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[4] = "00";            //保留
                paraFile[5] = "00002000";      //报警金额2
                paraFile[6] = "00002000";      //报警金额1
                paraFile[7] = "000001";        //电流互感器变比
                paraFile[8] = "000001";        //电压互感器变比
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                paraFile[10] = strKhID[i]; //客户编号
                paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

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

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");

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
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在交互终端与表身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentityAuthentication(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取卡片参数信息,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthReadTerminalToCardInfo(strCardNo, strRand2, out strParaInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
            blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                    {
                        ResultDictionary["刷开户卡后本地开户状态位7"][i] = "未开户";
                    }
                    else
                    {
                        ResultDictionary["刷开户卡后本地开户状态位7"][i] = "开户";
                    }
                }
                else
                {
                    ResultDictionary["刷开户卡后本地开户状态位7"][i] = "异常";
                }
            }
            UploadTestResult("刷开户卡后本地开户状态位7");

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                     if( ResultDictionary["刷开户卡前远程开户状态位7"][i] == "开户" && ResultDictionary["刷开户卡前本地开户状态位7"][i] == "未开户"
                         && ResultDictionary["刷开户卡后本地开户状态位7"][i] == "开户")
                     {
                         blnRet[i, 6] = true;
                     }
                     ResultDictionary["已远程开户再本地开户7"][i] = blnRet[i, 6] ? "通过" : "不通过";
                }
            }
            UploadTestResult("已远程开户再本地开户7");                               

            //----------------处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }

            //通知界面
            UploadTestResult("结论");
        }
    }
}
