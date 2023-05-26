using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;
using CLDC_SafeFileProtocol.Protocols;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 交互终端充值
    /// </summary>
    public class TerminalInMoney : VerifyBase
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

        public TerminalInMoney(object plan)
            : base(plan)
        {

        }
        //输出列
        //远程开户状态位1|本地开户状态位1|未开户不可购电1|本地开户状态位2|卡内客户编号2|表内客户编号2|剩余金额前-后2|应答(客户编号不匹配)2|客户编号不一致不可购电2
        //本地开户状态位3|卡内表号3|表内表号3|剩余金额前-后3|表号不一致不可购电3|卡购电次数4|表内购电次数4|剩余金额前-后4|应答(充值次数错误)4|购电次数比表内大2不可远程充值4
        //囤积金额限值5|购电次数前-后5|剩余金额前-后5|上1次购电日期5|上1次购电后总购电次数5|上1次购电金额5|上1次购电前剩余金额5|上1次购电后剩余金额5|上1次购电后累计购电金额5|购电次数比表内大1可远程充值5
        //卡购电次数6|表内购电次数6|剩余金额前-后6|返写信息文件长度6|购电次数与表内相等不可购电6
        //|囤积金额限值7|购电金额7|剩余金额前-后7|应答(购电超囤积)7|购电超囤积不可购电7




        protected override bool CheckPara()
        {

            ResultNames = new string[] { "远程开户状态位1","本地开户状态位1","未开户不可购电1","本地开户状态位2","卡内客户编号2","表内客户编号2","剩余金额前一后2","应答客户编号不匹配2","客户编号不一致不可购电2",
                                         "本地开户状态位3","卡内表号3","表内表号3","剩余金额前一后3","表号不一致不可购电3",
                                         "卡购电次数4","表内购电次数4","剩余金额前一后4","应答充值次数错误4","购电次数比表内大2不可远程充值4",
                                         "囤积金额限值5","购电次数前一后5","剩余金额前一后5","上1次购电日期5","上1次购电后总购电次数5","上1次购电金额5","上1次购电前剩余金额5","上1次购电后剩余金额5","上1次购电后累计购电金额5","购电次数比表内大1可远程充值5",
                                         "卡购电次数6","表内购电次数6","剩余金额前一后6","返写信息文件长度6","购电次数与表内相等不可购电6",
                                         "囤积金额限值7","购电金额7","剩余金额前一后7","应答购电超囤积7","购电超囤积不可购电7",
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
            string[] strRevData = new string[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //剩余金额
            string[] strGdCountQ = new string[BwCount]; //购电次数
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strGdMoney = new string[BwCount];
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] GdkKhID = new string[BwCount];
            string[] GdkGdCount = new string[BwCount]; //购电次数
            string[] GdkMeterNo = new string[BwCount];
            string[] strMeterNo = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strCardNo = new string[BwCount];
            string[] ParaFileArr = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strMac1 = new string[BwCount];
            string[] strMac2 = new string[BwCount];
            string[] strMac3 = new string[BwCount];
            string[] strMac4 = new string[BwCount];
            string CardType = "03";
            bool[,] blnRet = new bool[BwCount, 7];
            int[] iFlag = new int[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] status = new string[BwCount];
            string[] status3 = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] strRedMeterKhID = new string[BwCount];
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strParaFileArrCmd = new string[BwCount];  //参数信息文件
            string[] strwalletFileArrCmd = new string[BwCount];//钱包文件
            string[] strpriceFile1ArrCmd = new string[BwCount];//当前套电价文件
            string[] strpriceFile2ArrCmd = new string[BwCount];//备用套电价文件
            string[] strfileReplyArrCmd = new string[BwCount];
            string[] strControlFilePlainArrCmd = new string[BwCount];  //合闸明文
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
            string[] strReadReplyInfo = new string[BwCount];
            string[] strFfckCountQ = new string[BwCount];
            string[] strFfckCountH = new string[BwCount];
            string[] strParaInfo = new string[BwCount];
            bool[] blnSendTerminal = new bool[BwCount];
            bool[] bWriteRet = new bool[BwCount];
            string[] strErrInfo = new string[BwCount];


            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoneyQ);

            try
            {
                //1-----------------------------

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                bool[] BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
                bool[] blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在用购电卡（购电次数=1，购电金额100）通过交互终端进行购电操作,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000001";    //购电次数

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
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);

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
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCount, out strSyMoney);


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
                            ResultDictionary["远程开户状态位1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态位1"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态位1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位1"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户状态位1"][i] = "异常";
                        ResultDictionary["本地开户状态位1"][i] = "异常";
                    }
                    if (ResultDictionary["远程开户状态位1"][i] == "未开户" && ResultDictionary["本地开户状态位1"][i] == "未开户")
                    {
                        blnRet[i, 0] = true;
                    }
                
                    ResultDictionary["未开户不可购电1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                }
                UploadTestResult("远程开户状态位1");
                UploadTestResult("本地开户状态位1");
                UploadTestResult("未开户不可购电1");


                //2-------------------------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;
                MessageController.Instance.AddMessage("正在交互终端开户并充值200（客户编号为112233445566）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00004E20";    //购电金额
                    walletFile[1] = "00000001";    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                strSyMoneyQ = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoneyQ = Common.StringConverToDecima(strSyMoneyQ);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;

                MessageController.Instance.AddMessage("正在进行购电卡客户编号与表内不一致,表号与表内一致的购电操作,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    GdkKhID[i] = "665544332211";
                    paraFile[10] = GdkKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdMoney[i] = "00002710";
                    GdkGdCount[i] = "00000002";
                    walletFile[0] = strGdMoney[i];    //购电金额
                    walletFile[1] = GdkGdCount[i];    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡内客户编号2", GdkKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号2", strRedMeterKhID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney = Common.StringConverToDecima(strSyMoney);

                bool[] blnErrRet = new bool[BwCount];

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    blnErrRet[i] = false;
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length <=4)
                    {
                        if ((Convert.ToInt32(strfileReplyArr[i], 16) & 0x0010) == 0x0010)
                        {
                            ResultDictionary["应答客户编号不匹配2"][i] = "是";
                            blnErrRet[i] = true;
                        }
                        else
                        {
                            ResultDictionary["应答客户编号不匹配2"][i] = "无";
                            blnErrRet[i] = false;
                        }
                    }
                    else
                    {
                        ResultDictionary["应答客户编号不匹配2"][i] = "异常";
                        blnErrRet[i] = false;
                    }
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态位2"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位2"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位2"][i] = "异常";
                    }

                    if (strSyMoneyQ[i] == "300" && ResultDictionary["本地开户状态位2"][i] == "开户" && blnErrRet[i])
                    {
                        blnRet[i, 1] = true;
                    }
                    ResultDictionary["剩余金额前一后2"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["客户编号不一致不可购电2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                }
                UploadTestResult("本地开户状态位2");
                UploadTestResult("剩余金额前一后2");
                UploadTestResult("应答客户编号不匹配2");
                UploadTestResult("客户编号不一致不可购电2");


                //3----------------------------

                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在用购电卡通过交互终端对表计进行购电操作：购电卡中客户编号与表内一致、表号与表内不一致，购电次数=表内购电次数+1（=2），购电金额100,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    GdkMeterNo[i] = "000000112233";

                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = GdkMeterNo[i];  //表号
                    GdkKhID[i] = "112233445566";
                    paraFile[10] = GdkKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdMoney[i] = "00002710";
                    GdkGdCount[i] = "00000002";
                    walletFile[0] = strGdMoney[i];    //购电金额
                    walletFile[1] = GdkGdCount[i];    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内表号....");
                Common.Memset(ref strRevCode, "DF010001001E0006");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMeterNo, out strRevMac1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡内表号3", GdkMeterNo);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号3", strMeterNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney = Common.StringConverToDecima(strSyMoney);

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
                            ResultDictionary["本地开户状态位3"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位3"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位3"][i] = "异常";
                    }
                }
                UploadTestResult("本地开户状态位3");


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (strSyMoney[i] == "300" && ResultDictionary["本地开户状态位3"][i] == "开户")
                    {
                        blnRet[i, 2] = true;
                    }
                    ResultDictionary["剩余金额前一后3"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["表号不一致不可购电3"][i] = blnRet[i, 2] ? "通过" : "不通过";

                }
                UploadTestResult("剩余金额前一后3");
                UploadTestResult("表号不一致不可购电3");


                //4---------------------------

                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在用购电卡通过交互终端对表计进行购电操作：购电卡客户编号和表号与表内一致，购电次数=（表内购电次数+2），购电金额100,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    GdkKhID[i] = "112233445566";
                    paraFile[10] = GdkKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdMoney[i] = "00002710";
                    GdkGdCount[i] = "00000003";
                    walletFile[0] = strGdMoney[i];    //购电金额
                    walletFile[1] = GdkGdCount[i];    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out BuyCount, out strSyMoney);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数4", GdkGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数4", BuyCount);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);

                for (int i = 0; i < BwCount; i++)
                {
                    blnErrRet[i] = false;
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length == 114)
                    {
                        strfileReplyArr[i] = strfileReplyArr[i].Substring(strfileReplyArr[i].Length - 4, 4);
                        if ((Convert.ToInt32(strfileReplyArr[i], 16) & 0x0020) == 0x0020)
                        {
                            ResultDictionary["应答充值次数错误4"][i] = "是";
                            blnErrRet[i] = true;
                        }
                        else
                        {
                            ResultDictionary["应答充值次数错误4"][i] = "无";
                            blnErrRet[i] = false;
                        }
                    }
                    else
                    {
                        ResultDictionary["应答充值次数错误4"][i] = "异常";
                        blnErrRet[i] = false;
                    }

                    if (strSyMoney[i] == "300" && BuyCount[i] == "00000001" && blnErrRet[i])
                    {
                        blnRet[i, 3] = true;
                    }
                    ResultDictionary["剩余金额前一后4"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电次数比表内大2不可远程充值4"][i] = blnRet[i, 3] ? "通过" : "不通过";

                }
                UploadTestResult("应答充值次数错误4");
                UploadTestResult("剩余金额前一后4");
                UploadTestResult("购电次数比表内大2不可远程充值4");

                //5----------------------------------------------
                if (Stop) return;
                Common.Memset(ref strRevCode, "04001004");
                Common.Memset(ref strData, "700");
                Common.Memset(ref strRevData, "04001004" + "00070000");
                MessageController.Instance.AddMessage("正在设置囤积金额限值700元,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strRevCode);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值5", strData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strSyMoneyQ[i] = strSyMoney[i]; 
                }

                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                strGdCountQ = MeterProtocolAdapter.Instance.ReadData("03330201", 4);


                MessageController.Instance.AddMessage("正在进行,购电金额<囤积金额,购电卡客户编号与表内一致,购电次数=（表内购电次数+1）的购电操作,请稍候....");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    strGdMoney[i] = "00004E20";
                    GdkGdCount[i] = "00000002";
                    walletFile[0] = strGdMoney[i];    //购电金额
                    walletFile[1] = GdkGdCount[i];    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out BuyCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);


                string[] strShowData = new string[BwCount];
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                string[] strBuyDate = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期5", strBuyDate);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strGdCountLast1 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数5", Common.StringConverToIntger(strGdCountLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strShowData = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额5", Common.StringConverToDecima(strShowData));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strShowData = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额5", Common.StringConverToDecima(strShowData));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strShowData = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额5", Common.StringConverToDecima(strShowData));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                string[] strMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额5", Common.StringConverToDecima(strMoneyLast1));

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (strSyMoney[i] == "500" && strMoneyLast1[i] == "00050000" && strGdCountLast1[i]=="0002")
                    {
                        blnRet[i, 4] = true;
                    }
                    ResultDictionary["购电次数前一后5"][i] = strGdCountQ[i] + "-" + strGdCountLast1[i];
                    ResultDictionary["剩余金额前一后5"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电次数比表内大1可远程充值5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                }
                UploadTestResult("购电次数前一后5");
                UploadTestResult("剩余金额前一后5");
                UploadTestResult("购电次数比表内大1可远程充值5");

                //6----------------------
                strSyMoneyQ = strSyMoney;


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡客户编号和表号与表内一致，购电次数=表内购电次数,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "80";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    BuyCount[i] = "00000002";
                    walletFile[0] = "00004E20";    //购电金额
                    walletFile[1] = "00000002";    //购电次数


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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数6", GdkGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数6", BuyCount);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney = Common.StringConverToDecima(strSyMoney);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnErrRet[i] = false;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]))
                    {
                        if (strfileReplyArr[i].Length == 108)
                        {
                            ResultDictionary["返写信息文件长度6"][i] = "正确";
                            blnErrRet[i] = true;
                        }
                        else
                        {
                            ResultDictionary["返写信息文件长度6"][i] = "错误";
                            blnErrRet[i] = false;
                        }
                    }
                    else
                    {
                        ResultDictionary["返写信息文件长度6"][i] = "异常";
                        blnErrRet[i] = false;
                    }
                    if (strSyMoney[i] == "500" && blnErrRet[i])
                    {
                        blnRet[i, 5] = true;
                    }
                    ResultDictionary["剩余金额前一后6"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电次数与表内相等不可购电6"][i] = blnRet[i, 5] ? "通过" : "不通过";

                }
                UploadTestResult("返写信息文件长度6");
                UploadTestResult("剩余金额前一后6");
                UploadTestResult("购电次数与表内相等不可购电6");

                //7--------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行,购电金额＞囤积金额的购电操作,请稍候....");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
                strData = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值7", Common.StringConverToDecima(strData));

                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                    walletFile[0] = "00004E84";    //购电金额
                    walletFile[1] = "00000003";    //购电次数

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
                MessageController.Instance.AddMessage("正在模拟交互终端下发参数文件,请稍候....");
                blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);

                Common.Memset(ref BuyMoney, "201");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电金额7", BuyMoney);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney = Common.StringConverToDecima(strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnErrRet[i] = false;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length <= 4)
                    {
                        if ((Convert.ToInt32(strfileReplyArr[i], 16) & 0x0040) == 0x0040)
                        {
                            ResultDictionary["应答购电超囤积7"][i] = "是";
                            blnErrRet[i] = true;
                        }
                        else
                        {
                            ResultDictionary["应答购电超囤积7"][i] = "无";
                        }
                    }
                    else
                    {
                        ResultDictionary["应答购电超囤积7"][i] = "异常";
                    }

                    if (strSyMoney[i] == "500" && blnErrRet[i])
                    {
                        blnRet[i, 6] = true;
                    }
                    ResultDictionary["购电超囤积不可购电7"][i] = blnRet[i, 6] ? "通过" : "不通过";
                    ResultDictionary["剩余金额前一后7"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                }
                UploadTestResult("剩余金额前一后7");
                UploadTestResult("应答购电超囤积7");
                UploadTestResult("购电超囤积不可购电7");

                MessageController.Instance.AddMessage("正在处理结果");
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
                UploadTestResult("结论");
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
