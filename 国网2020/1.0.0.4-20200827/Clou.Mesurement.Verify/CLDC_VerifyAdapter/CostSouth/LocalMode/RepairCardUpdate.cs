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
    /// 补卡参数更新
    /// </summary>
    public class RepairCardUpdate : VerifyBase
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

        public RepairCardUpdate(object plan)
            : base(plan)
        {


        }



        //当前密钥1|当前套电价文件设置值1|当前套电价文件读取值1|备用套电价文件设置值1|备用套电价文件读取值1|参数信息文件设置值1|参数信息文件读取值1|上1次编程记录1|测试密钥补卡不可参数更新1
        //当前密钥2|补卡后插卡状态字2|当前套电价文件读取值2|备用套电价文件设置值2|备用套电价文件读取值2|参数信息文件设置值2|参数信息文件读取值2|上1次编程记录2|参数更新标志位全置0不更新2
        //当前密钥3|补卡3|当前套电价文件读取值3|备用套电价文件设置值3|备用套电价文件读取值3|参数信息文件设置值3|参数信息文件读取值3|上1次编程记录3|参数更新标志位全置1全更新3

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥1","当前套电价文件设置值1","当前套电价文件读取值1","备用套电价文件设置值1","备用套电价文件读取值1",
                                         "参数信息文件设置值1","参数信息文件读取值1","上1次编程事件记录1","测试密钥补卡不可参数更新1",
                                         "当前密钥2","补卡后插卡状态字2","当前套电价文件设置值2","当前套电价文件读取值2","备用套电价文件设置值2","备用套电价文件读取值2","参数信息文件设置值2",
                                         "参数信息文件读取值2","上2次编程事件记录2","参数更新标志位全置0不更新2",
                                         "当前密钥3","补卡3","当前套电价文件设置值3","当前套电价文件读取值3","备用套电价文件设置值3","备用套电价文件读取值3","参数信息文件设置值3",
                                         "参数信息文件读取值3","上1次编程事件记录3","参数更新标志位全置1全更新3",
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
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strKhID = new string[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            bool[] WriteUserResult = new bool[BwCount];
            bool[] result = new bool[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevMac2 = new string[BwCount];
            string[] strGdCountQ = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 14];
            string[] strRedInfoData = new String[BwCount];
            string[] strRedpriceFile1Data = new String[BwCount];
            string[] strRedpriceFile2Data = new String[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            string[] status = new string[BwCount];
            bool[] blnRetTmp = new bool[BwCount];
            string[] strFsflChangeDate = new string[BwCount];
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
            Common.Memset(ref strData, "00002710");
            MessageController.Instance.AddMessage("正在进行钱包初始化,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("正在用户,请稍候....");
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
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                paraFile[10] = "112233445566"; //客户编号
                paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                strFsflChangeDate[i] = paraFile[3];


                walletFile[0] = "00000000";    //购电金额
                walletFile[1] = "00000000";    //购电次数

                for (int j = 0; j < 12; j++)   //费率1-12
                {
                    priceFile1[j] = "00088811";
                    priceFile2[j] = "00088811";
                }
                for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00008822";
                    priceFile2[j] = "00008822";
                }
                for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00034000";
                    priceFile2[j] = "00034000";
                }
                for (int j = 25; j < 31; j++)  //年第1-6结算日
                {
                    priceFile1[j] = "010101";
                    priceFile2[j] = "010101";
                }
                for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00002200";
                    priceFile2[j] = "00002200";
                }
                for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00066600";
                    priceFile2[j] = "00066600";
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
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            if (Stop) return;
            Common.Memset(ref strData, "00002710");
            MessageController.Instance.AddMessage("正在进行钱包初始化,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            #endregion

            try
            {
                //1-------------

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("测试密钥下，使用用户卡1进行补卡和参数更新,参数更新标志位全置1（更新全部参数）,请稍候....");
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
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strFsflChangeDate[i] = paraFile[3];


                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00044401";
                        priceFile2[j] = "00044402";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002300";
                        priceFile2[j] = "00002300";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00034000";
                        priceFile2[j] = "00034000";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "010101";
                        priceFile2[j] = "010101";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002200";
                        priceFile2[j] = "00002200";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00066600";
                        priceFile2[j] = "00066600";
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

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

                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref  strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedInfoData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值1", strRedInfoData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedInfoData[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strRedInfoData[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strReadTmp = strRedInfoData[i].Substring(8, 52);
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
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile1Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值1", strRedpriceFile1Data);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile1Data[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strRedpriceFile1Data[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile1Data[i].Substring(0, strRedpriceFile1Data[i].Length - 4);
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
                Common.Memset(ref  strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile2Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值1", strRedpriceFile2Data);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile2Data[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strRedpriceFile2Data[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile2Data[i].Substring(0, strRedpriceFile2Data[i].Length - 4);
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
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程事件记录1", strBcsj);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && strBcsj[i] == "00".PadLeft(100, '0'))
                    {
                        blnRet[i, 3] = true;
                    }
                    ResultDictionary["测试密钥补卡不可参数更新1"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
                UploadTestResult("测试密钥补卡不可参数更新1");


                //2--------------
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
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥2", MyStatus);


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下，使用用户卡1开户,请稍候....");
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
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strFsflChangeDate[i] = paraFile[3];


                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00044400";
                        priceFile2[j] = "00044400";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002300";
                        priceFile2[j] = "00002300";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00034045";
                        priceFile2[j] = "00034045";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "010101";
                        priceFile2[j] = "010101";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002200";
                        priceFile2[j] = "00002200";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00066600";
                        priceFile2[j] = "00066600";
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡2插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("用户卡2对表计进行补卡和参数更新，参数更新标志位全置0（不更新）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "00";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00005000";      //报警金额1
                    paraFile[6] = "00004000";      //报警金额2
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                    paraFile[10] = "112233445566"; //客户编号
                    paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    


                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00066606";
                        priceFile2[j] = "00099970";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002300";
                        priceFile2[j] = "00004500";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00012000";
                        priceFile2[j] = "00043000";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "010101";
                        priceFile2[j] = "010101";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00006600";
                        priceFile2[j] = "00004400";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "000567700";
                        priceFile2[j] = "00034650";
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);



                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值2", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值2", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值2", strpriceFile2Arr);

                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref  strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedInfoData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值2", strRedInfoData);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedInfoData[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strRedInfoData[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strReadTmp = strRedInfoData[i].Substring(8, 52);
                        string strParaFileTmp = strParaFileArr[i].Substring(8, 52);
                        if (strReadTmp != strParaFileTmp)
                        {
                            blnRet[i, 4] = true;
                        }
                    }
                }

                //当前套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取当前套电价文件....");
                Common.Memset(ref strRevCode, "DF010003000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile1Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值2", strRedpriceFile1Data);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile1Data[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strRedpriceFile1Data[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile1Data[i].Substring(0, strRedpriceFile1Data[i].Length - 4);
                        string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strpriceFile1Arr[i].Length - 4);
                        if (strRedDataTmp != strpriceFile1Tmp)
                        {
                            blnRet[i, 5] = true;
                        }
                    }
                }


                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref  strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile2Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值2", strRedpriceFile2Data);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile2Data[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strRedpriceFile2Data[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile2Data[i].Substring(0, strRedpriceFile2Data[i].Length - 4);
                        string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strpriceFile2Arr[i].Length - 4);
                        if (strRedDataTmp != strpriceFile2Tmp)
                        {
                            blnRet[i, 6] = true;
                        }
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上2次编程事件,请稍候....");
                strBcsj = MeterProtocolAdapter.Instance.ReadData("03300002", 50);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2次编程事件记录2", strBcsj);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                string[] strCkStatus = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "补卡后插卡状态字2", strCkStatus);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (!string.IsNullOrEmpty(strCkStatus[i]))
                    {
                        if ((Convert.ToInt32(strCkStatus[i], 16) & 0x0001) == 0x0001)
                        {
                            blnRet[i, 7] = true;
                        }
                    }

                    if (blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && strBcsj[i] == "00".PadLeft(100, '0'))
                    {
                        blnRet[i, 8] = true;
                    }
                    ResultDictionary["参数更新标志位全置0不更新2"][i] = blnRet[i, 8] ? "通过" : "不通过";
                }
                UploadTestResult("参数更新标志位全置0不更新2");


                //3--------------

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥3", MyStatus);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,使用用户卡2对表计进行补卡和参数更新，参数更新标志位全置1（更新全部参数）,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[1] = "8F";            //参数更新标志位
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[5] = "00006000";      //报警金额1
                    paraFile[6] = "00007000";      //报警金额2
                    paraFile[7] = "000002";        //电流互感器变比
                    paraFile[8] = "000002";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strFsflChangeDate[i] = paraFile[3];

                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000001";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00066600";
                        priceFile2[j] = "00099900";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00002301";
                        priceFile2[j] = "00004502";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00012000";
                        priceFile2[j] = "00043000";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "010101";
                        priceFile2[j] = "010101";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00006600";
                        priceFile2[j] = "00004400";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00056770";
                        priceFile2[j] = "00034650";
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                string strBkDate = DateTime.Now.ToString("yyMMddHHmmss");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值3", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值3", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值3", strpriceFile2Arr);

                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref  strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedInfoData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值3", strRedInfoData);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedInfoData[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strRedInfoData[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strFsflChang = strRedInfoData[i].Substring(20, 10);
                        string strReadTmp = strRedInfoData[i].Substring(30, 30);
                        string strParaFileTmp = strParaFileArr[i].Substring(30, 30);
                        if (strReadTmp != strParaFileTmp && strFsflChang == strFsflChangeDate[i])
                        {
                            blnRet[i, 9] = true;
                        }
                    }
                }

                //当前套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取当前套电价文件....");
                Common.Memset(ref strRevCode, "DF010003000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile1Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值3", strRedpriceFile1Data);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile1Data[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strRedpriceFile1Data[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile1Data[i].Substring(0, strRedpriceFile1Data[i].Length - 4);
                        string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strRedpriceFile1Data[i].Length - 4);
                        if (strRedDataTmp != strpriceFile1Tmp)
                        {
                            blnRet[i, 10] = true;
                        }
                    }
                }

                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedpriceFile2Data, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值3", strRedpriceFile2Data);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedpriceFile2Data[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strRedpriceFile2Data[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedpriceFile2Data[i].Substring(0, strRedpriceFile2Data[i].Length - 4);
                        string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strRedpriceFile2Data[i].Length - 4);
                        if (strRedDataTmp == strpriceFile2Tmp)
                        {
                            blnRet[i, 11] = true;
                        }
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次编程事件记录,请稍候....");
                strBcsj = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程事件记录3", strBcsj);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (!string.IsNullOrEmpty(strBcsj[i]) &&  strBcsj[i].Length == 100)
                    {
                        string strGxBj = "";
                        strGxBj = strBcsj[i].Substring(strBcsj[i].Length - 22, 2);
                        string strDateTmp = strBcsj[i].Substring(strBcsj[i].Length - 12, 12);
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strBkDate), DateTimes.FormatStringToDateTime(strDateTmp));
                        if (iErr < 301 && strGxBj=="0A")      // 编程事件记录未判断
                        {
                            blnRet[i, 12] = true;
                        }
                    }

                }


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11] && blnRet[i, 12])
                    {
                        blnRet[i, 13] = true;
                        ResultDictionary["参数更新标志位全置1全更新3"][i] = "通过";
                        ResultDictionary["补卡3"][i] = "正常应答";
                    }
                    else
                    {
                        ResultDictionary["参数更新标志位全置1全更新3"][i] = "不通过";
                        ResultDictionary["补卡3"][i] = "异常应答";
                    }
                }
                UploadTestResult("补卡3");
                UploadTestResult("参数更新标志位全置1全更新3");



                MessageController.Instance.AddMessage("正在处理结论,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 3] && blnRet[i, 8] && blnRet[i, 13])
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
