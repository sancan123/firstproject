using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using System.Globalization;
using CLDC_SafeFileProtocol;
using CLDC_SafeFileProtocol.Protocols;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 开户卡通过交互终端参数更新
    /// </summary>
    public class TerminalInitCardParamUpdate : VerifyBase
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

        public TerminalInitCardParamUpdate(object plan)
            : base(plan)
        {


        }
        //当前密钥1|当前套电价文件设置值1|当前套电价文件读取值1|备用套电价文件设置值1|备用套电价文件读取值1|参数信息文件设置值1|参数信息文件读取值1|上1次编程记录1|测试密钥开户卡不可参数更新1
        //当前密钥2|开户结果2|当前套电价文件读取值2|备用套电价文件设置值2|备用套电价文件读取值2|参数信息文件设置值2|参数信息文件读取值2|上1次编程记录2|参数更新标志位全置1全更新2

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥1","当前套电价文件设置值1","当前套电价文件读取值1","备用套电价文件设置值1","备用套电价文件读取值1",
                                         "参数信息文件设置值1","参数信息文件读取值1","上1次编程记录1","测试密钥开户卡不可参数更新1",
                                         "当前密钥2","开户结果2","当前套电价文件设置值2","当前套电价文件读取值2","备用套电价文件设置值2","备用套电价文件读取值2","参数信息文件设置值2",
                                         "参数信息文件读取值2","上1次编程记录2","参数更新标志位全置1全更新2",
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
            string[] strRevDataPara = new string[BwCount];
            string[] strRevDataPrice1 = new string[BwCount];
            string[] strRevDataPrice2 = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //剩余金额
            string[] strGdCountQ = new string[BwCount]; //购电次数
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strGdMoney = new string[BwCount];
            string[] strKhID = new string[BwCount];
            string[] KhkGdCount = new string[BwCount]; //购电次数
            string[] strDqt = new string[BwCount];
            string[] strByt = new string[BwCount];
            string[] strSyMoneyH = new string[BwCount];
            string[] strCardNo = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strMac1 = new string[BwCount];
            string[] strMac2 = new string[BwCount];
            string[] strMac3 = new string[BwCount];
            string[] strMac4 = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
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
            string[] strParaFileArrCmd = new string[BwCount];  //参数信息文件
            string[] strwalletFileArrCmd = new string[BwCount];//钱包文件
            string[] strpriceFile1ArrCmd = new string[BwCount];//当前套电价文件
            string[] strpriceFile2ArrCmd = new string[BwCount];//备用套电价文件
            string[] strfileReplyArrCmd = new string[BwCount];
            string[] strControlFilePlainArrCmd = new string[BwCount];  //合闸明文
            bool[] BlnIniRet = new bool[BwCount];
            bool[] result = new bool[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] blnRecKeyRet = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 9];
            string[] MyStatus = new string[BwCount];
            string[] strRedData = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] strRepParaInfo = new string[BwCount];
            string[] strRepPrice1 = new string[BwCount];
            string[] strRepPrice2 = new string[BwCount];
            string[] status = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strParaInfo = new string[BwCount];
            bool[] blnSendTerminal = new bool[BwCount];
            bool[] bWriteRet = new bool[BwCount];
            string[] strReadParaFileH = new string[BwCount];  //参数信息文件
            string[] strReadPriceFile1H = new string[BwCount];//当前套电价文件
            string[] strReadPriceFile2H = new string[BwCount];//备用套电价文件


            #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);




            #endregion

            //1----------------------


            try
            {

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                //读取卡片序列号
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
                bool[] blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("测试密钥下,正在模拟开户卡通过交互终端更新参数,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "8F";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00002000";      //报警金额1
                    paraFile[6] = "00001000";      //报警金额2
                    paraFile[7] = "000023";        //电流互感器变比
                    paraFile[8] = "000035";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    strKhID[i] = "112233445566";
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdMoney[i] = "00004E20";
                    KhkGdCount[i] = "00000000";
                    walletFile[0] = strGdMoney[i];    //购电金额
                    walletFile[1] = KhkGdCount[i];    //购电次数


                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00011123";
                        priceFile2[j] = "00011165";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000122";
                        priceFile2[j] = "00000122";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00014400";
                        priceFile2[j] = "00014400";
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
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);

                string CardType = "03";
                if (Stop) return;
                MessageController.Instance.AddMessage("正在交互终端与表身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentityAuthentication(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片参数信息,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadTerminalToCardInfo(strCardNo, strRand2, out strParaInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片各信息,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCardMAC(strRand2, out strParaFileArrCmd, out strwalletFileArrCmd, out strpriceFile1ArrCmd, out strpriceFile2ArrCmd, out strControlFilePlainArrCmd);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strRevDataPara);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过交互终端下发当前套电价文件,请稍候....");
                bool[] PriceResult1 = MeterProtocolAdapter.Instance.SouthTerminalSendPrice1(strpriceFile1ArrCmd, out strRevDataPrice1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过交互终端下发备用套电价文件,请稍候....");
                bool[] PriceResult2 = MeterProtocolAdapter.Instance.SouthTerminalSendPrice2(strpriceFile2ArrCmd, out strRevDataPrice2);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

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
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥1", MyStatus);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值1", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值1", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值1", strpriceFile2Arr);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);


                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadParaFileH, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值1", strReadParaFileH);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strReadParaFileH[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strReadParaFileH[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strReadTmp = strReadParaFileH[i].Substring(8, 52);
                        string strParaFileTmp = strParaFileArr[i].Substring(8, 52);
                        if (strReadTmp != strParaFileTmp)
                        {
                            blnRet[i, 0] = true;
                        }
                    }
                }

                //当前套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取当前套电价文件....");
                Common.Memset(ref strRevCode, "DF010003000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadPriceFile1H, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值1", strReadPriceFile1H);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strReadPriceFile1H[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strReadPriceFile1H[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strReadPriceFile1H[i].Substring(0, strReadPriceFile1H[i].Length - 4);
                        string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strpriceFile1Arr[i].Length - 4);
                        if (strRedDataTmp != strpriceFile1Tmp)
                        {
                            blnRet[i, 1] = true;
                        }
                    }
                }


                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadPriceFile2H, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值1", strReadPriceFile2H);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strReadPriceFile2H[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strReadPriceFile2H[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strReadPriceFile2H[i].Substring(0, strReadPriceFile2H[i].Length - 4);
                        string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strpriceFile2Arr[i].Length - 4);
                        if (strRedDataTmp != strpriceFile2Tmp)
                        {
                            blnRet[i, 2] = true;
                        }
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                string[] strBcsj = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程记录1", strBcsj);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && strBcsj[i] == "00".PadLeft(100, '0'))
                    {
                        blnRet[i, 3] = true;
                    }
                    ResultDictionary["测试密钥开户卡不可参数更新1"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
                UploadTestResult("测试密钥开户卡不可参数更新1");


                //2--------------------------5

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
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
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥2", MyStatus);



                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

                if (Stop) return;
                MessageController.Instance.AddMessage("正式密钥下,正在模拟开户卡通过交互终端更新参数,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[1] = "8F";
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[5] = "00006600";      //报警金额1
                    paraFile[6] = "00007700";      //报警金额2
                    paraFile[7] = "000088";        //电流互感器变比
                    paraFile[8] = "000099";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00011000";
                        priceFile2[j] = "00011000";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00001100";
                        priceFile2[j] = "00001100";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00011000";
                        priceFile2[j] = "00011000";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "111111";
                        priceFile2[j] = "111111";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000110";
                        priceFile2[j] = "00000110";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00011230";
                        priceFile2[j] = "00011560";
                    }
                    for (int j = 44; j < 50; j++) //年第1-6结算日
                    {
                        priceFile1[j] = "111111";
                        priceFile2[j] = "111111";
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
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在交互终端与表身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentityAuthentication(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片参数信息,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadTerminalToCardInfo(strCardNo, strRand2, out strParaInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片各信息,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCardMAC(strRand2, out strParaFileArrCmd, out strwalletFileArrCmd, out strpriceFile1ArrCmd, out strpriceFile2ArrCmd, out strControlFilePlainArrCmd);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strRevDataPara);
                string strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过交互终端下发当前套电价文件,请稍候....");
                PriceResult1 = MeterProtocolAdapter.Instance.SouthTerminalSendPrice1(strpriceFile1ArrCmd, out strRevDataPrice1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过交互终端下发备用套电价文件,请稍候....");
                PriceResult2 = MeterProtocolAdapter.Instance.SouthTerminalSendPrice2(strpriceFile2ArrCmd, out strRevDataPrice2);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);


                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值2", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值2", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值2", strpriceFile2Arr);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内参数信息文件,请稍候....");
                Common.Memset(ref strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRepParaInfo, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值2", strRepParaInfo);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRepParaInfo[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strRepParaInfo[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strReadTmp = strRepParaInfo[i].Substring(32, 28);
                        string strParaFileTmp = strParaFileArr[i].Substring(32, 28);
                        if (strReadTmp == strParaFileTmp)
                        {
                            blnRet[i, 4] = true;
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内当前套电价,请稍候....");
                Common.Memset(ref strRevCode, "DF010003000000C7");

                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRepPrice1, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值2", strRepPrice1);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strReadPriceFile1H[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strRepPrice1[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRepPrice1[i].Substring(0, strRepPrice1[i].Length - 4);
                        string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strpriceFile1Arr[i].Length - 4);
                        if (strRedDataTmp == strpriceFile1Tmp)
                        {
                            blnRet[i, 5] = true;
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内备用套电价,请稍候....");
                Common.Memset(ref strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRepPrice2, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值2", strRepPrice2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strReadPriceFile2H[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strRepPrice2[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRepPrice2[i].Substring(0, strRepPrice2[i].Length - 4);
                        string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strpriceFile2Arr[i].Length - 4);
                        if (strRedDataTmp == strpriceFile2Tmp)
                        {
                            blnRet[i, 6] = true;
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                strBcsj = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程记录2", strBcsj);
                

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strBcsj[i]) && strBcsj[i].Length == 100)
                    {
                        string strInfoTmp = "FF".PadLeft(56, 'F') + "07A002FF07A003FF07A004FF";
                        string strInfo = strBcsj[i].Substring(0, strBcsj[i].Length - 20);
                        string strDateTmp = strBcsj[i].Substring(strBcsj[i].Length - 12, 12);
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strIniUserDate), DateTimes.FormatStringToDateTime(strDateTmp));
                        if (iErr < 301 && strInfoTmp == strInfo)     // 编程事件记录未判断
                        {
                            blnRet[i, 7] = true;
                        }
                    }
                }


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnSendTerminal[i])
                    {
                        blnRet[i, 8] = true;
                        ResultDictionary["参数更新标志位全置1全更新2"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["参数更新标志位全置1全更新2"][i] = "不通过";
                    }

                    if (blnSendTerminal[i])
                    {
                        ResultDictionary["开户结果2"][i] = "正常应答";
                    }
                    else
                    {
                        ResultDictionary["开户结果2"][i] = "异常应答";
                    }
                }
                UploadTestResult("开户结果2");
                UploadTestResult("参数更新标志位全置1全更新2");

                MessageController.Instance.AddMessage("正在处理结论,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;


                    if (blnRet[i, 3] && blnRet[i, 8])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }

                //通知界面
                UploadTestResult("结论");
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
