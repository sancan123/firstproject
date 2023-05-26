using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;
using CLDC_SafeFileProtocol.Protocols;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 交互终端补卡
    /// </summary>
    public class TerminalRepairCard :VerifyBase
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

        public TerminalRepairCard(object plan)
            : base(plan){


        }
            //本地开户状态位1|未开户不可补卡1|本地开户状态位2|卡1开户并购电2|
            //--卡2客户编号3|卡2表号3|表内客户编号3|表内表号3|表号不一致不可补卡3|
            //--卡2客户编号4|卡2表号4|表内客户编号4|表内表号4|客户编号不一致不可补卡4|
            //--卡2购电次数5|表内购电次数5|卡购电次数比表内大于等于2不可补卡5|
        //--囤积金额限值6|剩余金额6|充值100超囤积不可补卡6|
            //--卡2购电次数7|表内购电次数7|卡2购电金额7|剩余金额补卡前-补卡后7|购电次数相等可补卡不可购电7|
            //--购电次数购电前-购电后8|剩余金额购电前-购电后8|卡2购电成功8|
            //--卡1失效9|
            //--卡3购电次数10|卡3购电金额10|购电次数补卡前-补卡后10|剩余金额补卡前-补卡后10|卡3补卡并购电10|
            //--卡2失效11|
            //--卡2购电次数12|卡2购电金额12|表内购电次数12|剩余金额补卡前-补卡后12|卡购电次数比表内小可补卡不充值12|




        protected override bool CheckPara()
        {

            ResultNames = new string[] { "本地开户状态位1","未开户不可补卡1","本地开户状态位2","卡1开户并购电2",
                                        "卡2客户编号3","卡2表号3","表内客户编号3","表内表号3","表号不一致不可补卡3",
                                        "卡2客户编号4","卡2表号4","表内客户编号4","表内表号4","客户编号不一致不可补卡4",
                                        "卡2购电次数5","表内购电次数5","卡购电次数比表内大于等于2不可补卡5",
                                        "囤积金额限值6","剩余金额6","充值100超囤积不可补卡6",
                                        "卡2购电次数7","表内购电次数7","卡2购电金额7","剩余金额补卡前一补卡后7","卡2补卡7","购电次数相等可补卡不可购电7",
                                        "购电次数购电前一购电后8","剩余金额购电前一购电后8","卡2购电成功8",
                                        "卡1失效9",
                                        "卡3购电次数10","卡3购电金额10","购电次数补卡前一补卡后10","剩余金额补卡前一补卡后10","卡3补卡并购电10",
                                        "卡2失效11",
                                        "卡2购电次数12","卡2购电金额12","表内购电次数12","剩余金额补卡前一补卡后12","卡购电次数比表内小可补卡不充值12",
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
            bool[,] blnRet = new bool[BwCount, 12];
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
            bool[] rstTmp = new bool[BwCount];
            string[] strRedMeterKhID = new string[BwCount];
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
            string[] strParaInfo = new string[BwCount];
            bool[] blnSendTerminal = new bool[BwCount];
            bool[] bWriteRet = new bool[BwCount];
            string[] strErrInfo = new string[BwCount];
            int iSelectBwCount = 0;



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
            Common.Memset(ref iFlag, 1);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            Common.Memset(ref strRevCode,"DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoneyQ);



            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            bool[] BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            //步骤1----
            MessageController.Instance.AddMessage("用户卡1对表计补卡,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                strGdMoney[i] = "00002710";
                GdkGdCount[i] = "00000000";
                strKhID[i] = "112233445566";
                GdkKhID[i] = strKhID[i];

                paraFile[0] = "00";
                paraFile[1] = "80";
                paraFile[2] = "00000000";
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[4] = "00";
                paraFile[5] = "00002000";
                paraFile[6] = "00002000";
                paraFile[7] = "000001";
                paraFile[8] = "000001";
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[10] = GdkKhID[i]; //客户编号
                paraFile[11] = "03";       //卡类型 01=开户 02=购电 03=补卡

                walletFile[0] = "00000000";    //购电金额
                walletFile[1] = "00000000";    //购电次数

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
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
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                    {
                        status3[i] = "开户";
                    }
                    else
                    {
                        status3[i] = "未开户";
                    }
                }
                else
                {
                    status3[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "本地开户状态位1", status3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney);



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSyMoneyQ[i] == strSyMoney[i] && status3[i] == "未开户")
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["未开户不可补卡1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("未开户不可补卡1");



            //2-----------------------------
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
            bool[] blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);

            MessageController.Instance.AddMessage("用户卡1对表计开户,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = "00000001";    //购电次数
                paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

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
            //MessageBox.Show("请把用户卡插入表后按确定");

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
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            //远程充值第一次
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000002";
                BuyMoney[i] = "00002710";
                strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            //远程充值第二次
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000003";
                BuyMoney[i] = "00002710";
                strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄插卡后剩余金额,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                    {
                        status3[i] = "开户";
                    }
                    else
                    {
                        status3[i] = "未开户";
                    }
                }
                else
                {
                    status3[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "本地开户状态位2", status3);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSyMoney[i] == "00009C40" && status3[i] == "开户")
                {
                    blnRet[i, 1] = true;
                }
                ResultDictionary["卡1开户并购电2"][i] = blnRet[i, 1] ? "成功" : "失败";
            }
            UploadTestResult("卡1开户并购电2");

            MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡2插入表后按确定");


            //3----------------
            //读取卡片序列号
            if (Stop) return;
            MessageController.Instance.AddMessage("用户卡2通过交互终端对表计补卡,卡内表号与表内不一致、卡内客户编号与表内一致,请稍候....");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
            blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                GdkMeterNo[i] = "000000112233";
                paraFile[9] = GdkMeterNo[i];
                paraFile[11] = "03";       //卡类型 01=开户 02=购电 03=补卡


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

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2客户编号3", GdkKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2表号3", GdkMeterNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取表内客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号3", strRedMeterKhID);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            //result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);


            //MessageController.Instance.AddMessage("正在读取表内表号,请稍候....");
            //if (Stop) return;
            //Common.Memset(ref strRevCode, "DF010001001E0006");
            //bool[] blnResultTmp = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac1);
            strRevData = MeterProtocolAdapter.Instance.ReadData("04000402", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号3", strRevData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 2] = !blnSendTerminal[i];
                ResultDictionary["表号不一致不可补卡3"][i] = blnRet[i, 2] ? "通过" : "不通过";
            }
            UploadTestResult("表号不一致不可补卡3");


            //4----------------
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            MessageController.Instance.AddMessage("用户卡2通过交互终端对表计补卡,卡内表号与表内一致、卡内客户编号与表内不一致,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                strMeterNo[i]=Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "665544332200";
                paraFile[9] = strMeterNo[i];
                paraFile[10] = GdkKhID[i];


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);

                strParaFileArr[i] = strParaFile;
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

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2客户编号4", GdkKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2表号4", GdkMeterNo);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            //result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取表内客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号4", strRedMeterKhID);

            //if (Stop) return;
            //Common.Memset(ref strRevCode, "DF010001001E0006");
            //MessageController.Instance.AddMessage("正在读取表内表号,请稍候....");
            //blnResultTmp = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac1);
            strRevData = MeterProtocolAdapter.Instance.ReadData("04000402", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号4", strRevData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 3] = !blnSendTerminal[i];
                ResultDictionary["客户编号不一致不可补卡4"][i] = blnRet[i, 3] ? "通过" : "不通过";
            }
            UploadTestResult("客户编号不一致不可补卡4");



            //5----------------
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2通过交互终端对表计补卡，卡内购电次数=表内购电次数+2，购电金额100,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";
                paraFile[10] = GdkKhID[i];
                GdkGdCount[i] = "00000005";
                walletFile[1] = GdkGdCount[i];

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
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数5",Common.StringConverToIntger( GdkGdCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数5",Common.StringConverToIntger( strGdCountQ));

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnSendTerminal[i])
                {
                    blnRet[i, 4] = true;
                }
                ResultDictionary["卡购电次数比表内大于等于2不可补卡5"][i] = blnRet[i, 4] ? "通过" : "不通过";
            }
            UploadTestResult("卡购电次数比表内大于等于2不可补卡5");


           

            //6---------------------------------------

            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00049900");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("设囤积金额限值499，用户卡2通过交互终端对表计补卡，卡内购电次数=表内购电次数+1，购电金额100,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                paraFile[10] = GdkKhID[i];
                GdkGdCount[i] = "00000004";
                walletFile[1] = GdkGdCount[i];

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
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            string[]strSyMoney6 = new string[BwCount];
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额6", Common.HexConverToDecimalism(strSyMoney6));


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
            string[] strTjMoneyXz = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值6", Common.StringConverToDecima(strTjMoneyXz));

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnSendTerminal[i])
                {
                    blnRet[i, 6] = true;
                }
                ResultDictionary["充值100超囤积不可补卡6"][i] = blnRet[i, 6] ? "通过" : "不通过";
            }
            UploadTestResult("充值100超囤积不可补卡6");

            //--|||

            //7---------------------------------------
            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("囤积金额设为0；用户卡2通过交互终端对表计补卡，并充值100元，购电次数=表内购电次数,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                paraFile[10] = GdkKhID[i];
                GdkGdCount[i] = "00000003";
                walletFile[1] = GdkGdCount[i];

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
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数7", Common.StringConverToIntger(GdkGdCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数7", Common.StringConverToIntger(strGdCountQ));

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电金额7", Common.HexConverToDecimalism(BuyMoney));

            strSyMoney6 = Common.HexConverToDecimalism(strSyMoney6);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnSendTerminal[i])
                {
                    ResultDictionary["卡2补卡7"][i] = "成功";
                }
                else
                {
                    ResultDictionary["卡2补卡7"][i] = "失败";
                }
                if (blnSendTerminal[i] && strSyMoney6[i] == strSyMoney[i])
                {
                    ResultDictionary["购电次数相等可补卡不可购电7"][i] = "通过";
                     blnRet[i, 7] = true;
                }
                else
                {
                    ResultDictionary["购电次数相等可补卡不可购电7"][i] = "不通过";
                }
                ResultDictionary["剩余金额补卡前一补卡后7"][i] = strSyMoney6[i] + "-" + strSyMoney[i];
            }
            UploadTestResult("卡2补卡7");
            UploadTestResult("购电次数相等可补卡不可购电7");
            UploadTestResult("剩余金额补卡前一补卡后7");

            //8--------------------------


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            MessageController.Instance.AddMessage("用户卡2通过交互终端对表计补卡,并充值100元，购电次数=表内购电次数+1,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "02";
                GdkGdCount[i] = "00000004";
                walletFile[1] = GdkGdCount[i];


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
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");

            string[] strGdCount8 = new string[BwCount];
            string[] strSyMoney8 = new string[BwCount];
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCount8, out strSyMoney8);
            strSyMoney8 = Common.HexConverToDecimalism(strSyMoney8);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnSendTerminal[i] && strSyMoney8[i] == "500" && strGdCount8[i] == "00000004")
                {
                    blnRet[i, 8] = true;
                }
                ResultDictionary["卡2购电成功8"][i] = blnRet[i, 8] ? "通过" : "不通过";
                ResultDictionary["购电次数购电前一购电后8"][i] = strGdCountQ[i] + "-" + strGdCount8[i];
                ResultDictionary["剩余金额购电前一购电后8"][i] = strSyMoney[i] + "-" + strSyMoney8[i];
            }

            UploadTestResult("卡2购电成功8");
            UploadTestResult("购电次数购电前一购电后8");
            UploadTestResult("剩余金额购电前一购电后8");


            //9------------------

            if (Stop) return;
            MessageBox.Show("请把用户卡1插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();

            MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
            blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡1对表计充值100元,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                BuyMoney[i] = "00002710";
                GdkGdCount[i] = "00000005";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

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

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnSendTerminal[i])
                {
                    blnRet[i, 9] = true;
                }
                ResultDictionary["卡1失效9"][i] = blnRet[i, 9] ? "通过" : "不通过";
            }
            UploadTestResult("卡1失效9");


            //MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡3插入表后按确定");

            //步骤10----
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);


            if (Stop) return;

            string[] strGdCountQ10 = new string[BwCount];
            string[] strSyMoneyQ10 = new string[BwCount];
            MessageController.Instance.AddMessage("正在回抄插卡后表内金额及购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ10, out strSyMoneyQ10);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡3通过交互终端对表计补卡,并充值100元，购电次数=表内购电次数+1,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "03";
                GdkGdCount[i] = "00000005";
                walletFile[1] = GdkGdCount[i];

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

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡3购电次数10", Common.StringConverToIntger(GdkGdCount));

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡3购电金额10", Common.HexConverToDecimalism(BuyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄插卡后表内金额及购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCount, out strSyMoney);

            strSyMoneyQ10 = Common.HexConverToDecimalism(strSyMoneyQ10);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strGdCount[i]) && !string.IsNullOrEmpty(strGdCount8[i]))
                {
                    if (blnSendTerminal[i] && strSyMoney[i] == "600" && Convert.ToInt32(strGdCount[i], 16) == Convert.ToInt32(strGdCount8[i], 16) + 1)
                    {
                        blnRet[i, 10] = true;
                    }
                }

                ResultDictionary["卡3补卡并购电10"][i] = blnRet[i, 10] ? "成功" : "失败";
                ResultDictionary["购电次数补卡前一补卡后10"][i] = strGdCountQ10[i] + "-" + strGdCount[i];
                ResultDictionary["剩余金额补卡前一补卡后10"][i] = strSyMoneyQ10[i] + "-" + strSyMoney[i];
            }

            UploadTestResult("卡3补卡并购电10");
            UploadTestResult("购电次数补卡前一补卡后10");
            UploadTestResult("剩余金额补卡前一补卡后10");

            //--


            //11---------------------

            MessageBox.Show("请把用户卡2插入表后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            string[] strSyMoneyQ11 = new string[BwCount];
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoneyQ11);


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("再次插用户卡2,充值100元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                GdkGdCount[i] = "00000006";
                walletFile[0] = "00002710";    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次数

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


            MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
            blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);

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
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!blnSendTerminal[i])
                {
                    blnRet[i, 11] = true;
                }
                ResultDictionary["卡2失效11"][i] = blnRet[i, 11] ? "通过" : "不通过";

            }
            UploadTestResult("卡2失效11");

            //12----------------
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2通过交互终端对表计补卡，卡内购电次数=表内购电次数-1，购电金额100,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";
                paraFile[10] = GdkKhID[i];
                GdkGdCount[i] = "00000004";
                paraFile[11] = "03";
                walletFile[1] = GdkGdCount[i];

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
            MessageController.Instance.AddMessage("正在回抄表内购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountQ, out strSyMoney);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数12", Common.StringConverToIntger(GdkGdCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数12", Common.StringConverToIntger(strGdCountQ));

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电金额12", Common.HexConverToDecimalism(BuyMoney));

            strSyMoneyQ11 = Common.HexConverToDecimalism(strSyMoneyQ11);
            strSyMoney = Common.HexConverToDecimalism(strSyMoney);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnSendTerminal[i] && strSyMoney[i] == "600")
                {
                    blnRet[i, 5] = true;
                }
                ResultDictionary["卡购电次数比表内小可补卡不充值12"][i] = blnRet[i, 5] ? "通过" : "不通过";
                ResultDictionary["剩余金额补卡前一补卡后12"][i] = strSyMoneyQ11[i] + "-" + strSyMoney[i];
                
            }
            UploadTestResult("剩余金额补卡前一补卡后12");
            UploadTestResult("卡购电次数比表内小可补卡不充值12");

            //--卡2购电次数12|表内购电次数12|剩余金额补卡前-补卡后12|卡购电次数比表内小可补卡不充值12|


            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4]
                        && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9]
                        && blnRet[i, 10] && blnRet[i, 11])
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
