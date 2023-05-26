using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 用户卡开户
    /// </summary>
    public class UserCardInitUser : VerifyBase
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

        public UserCardInitUser(object plan)
            : base(plan)
        {

        }
        //本地开户状态位前-后1|测试密钥下不可本地开户1|
        //开户卡客户编号2|表计客户编号2|本地开户状态位前-后2|正式密钥下表号不一致不可本地开户2|
        //本地开户状态位前-后3|正式密钥下购电次数＞1不可本地开户3|
        //囤积金额限值4|剩余金额前-后4|本地开户状态位前-后4|充值200超囤积不可本地开户4
        //开户卡客户编号5|开户卡购电次数5|开户卡购电金额5|开户前本地开户状态位5|开户后本地开户状态位5|开户后客户编号5|开户前购电次数5|开户后购电次数5|开户前剩余金额5|开户后剩余金额5
        //上2次购电日期5|上2次购电金额5|正式密钥下开户且不充值5|开户卡客户编号6|开户卡购电次数6|开户卡购电金额6|开户前本地开户状态位6|开户后本地开户状态位6|开户后客户编号6|开户前购电次数6
        //开户后购电次数6|开户前剩余金额6|开户后剩余金额6|上1次购电日期6|上1次购电后总购电次数6|上1次购电金额6|上1次购电前剩余金额6|上1次购电后剩余金额6|上1次购电后累计购电金额6
        //正式密钥下开户且充值6|第1次开户卡购电次数7|第1次开户卡购电金额7|第1次开户卡客户编号7|第2次开户卡购电次数7|第2次开户卡购电金额7|第2次开户卡客户编号7|第2次开户前本地开户状态位7
        //第2次开户后本地开户状态位7|第2次开户后客户编号7|第2次开户前购电次数7|第2次开户后购电次数7|第2次开户前剩余金额7|第2次开户后剩余金额7|上1次购电日期7|上1次购电后总购电次数7
        //上1次购电金额7|上1次购电前剩余金额7|上1次购电后剩余金额7|上1次购电后累计购电金额7|已开户下再次本地开户充值7|远程开户前本地开户状态位8|远程开户前远程开户状态位8|远程开户后远程开户状态位8
        //已本地开户再远程开户8|开户卡刷卡次数9|开户卡购电金额9|刷卡前本地开户状态位9|刷卡后本地开户状态位9|刷卡前购电次数9|刷卡后购电次数9
        //刷卡前剩余金额9|刷卡后剩余金额9|刷卡后上1次购电后累计购电金额9|表购电次数大于1开户卡购电次数为0或1时仍可开户9

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "本地开户状态位前一后1","测试密钥下不可本地开户1",
                                         "开户卡客户编号2","表计客户编号2","本地开户状态位前一后2","正式密钥下表号不一致不可本地开户2",
                                         "本地开户状态位前一后3","正式密钥下购电次数＞1不可本地开户3",
                                         "囤积金额限值4","剩余金额前一后4","本地开户状态位前一后4","充值200超囤积不可本地开户4",
                                         "开户卡客户编号5","开户卡购电次数5","开户卡购电金额5","开户前本地开户状态位5","开户后本地开户状态位5","开户后客户编号5",
                                         "开户前购电次数5","开户后购电次数5","开户前剩余金额5","开户后剩余金额5","上2次购电日期5","上2次购电金额5","正式密钥下开户且不充值5",
                                         "开户卡客户编号6","开户卡购电次数6","开户卡购电金额6","开户前本地开户状态位6","开户后本地开户状态位6","开户后客户编号6","开户前购电次数6",
                                         "开户后购电次数6","开户前剩余金额6","开户后剩余金额6","上1次购电日期6","上1次购电后总购电次数6","上1次购电金额6","上1次购电前剩余金额6","上1次购电后剩余金额6",
                                         "上1次购电后累计购电金额6","正式密钥下开户且充值6",
                                         "第1次开户卡购电次数7","第1次开户卡购电金额7","第1次开户卡客户编号7","第2次开户卡购电次数7","第2次开户卡购电金额7","第2次开户卡客户编号7","第2次开户前本地开户状态位7",
                                         "第2次开户后本地开户状态位7","第2次开户后客户编号7","第2次开户前购电次数7","第2次开户后购电次数7","第2次开户前剩余金额7","第2次开户后剩余金额7","上1次购电日期7",
                                         "上1次购电后总购电次数7","上1次购电金额7","上1次购电前剩余金额7","上1次购电后剩余金额7","上1次购电后累计购电金额7","已开户下再次本地开户充值7",
                                         "远程开户前本地开户状态位8","远程开户前远程开户状态位8","远程开户后远程开户状态位8","已本地开户再远程开户8","开户卡刷卡次数9",
                                         "开户卡购电金额9","刷卡前本地开户状态位9","刷卡后本地开户状态位9","刷卡前购电次数9","刷卡后购电次数9",
                                         "刷卡前剩余金额9","刷卡后剩余金额9","刷卡后上1次购电后累计购电金额9","表购电次数大于1开户卡购电次数＜2时仍可开户9",
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
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
            string[] strGdCountQ = new string[BwCount];
            string[] strSyMoneyH = new string[BwCount]; //钱包初始化剩余金额
            string[] strGdCountH = new string[BwCount];
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] strGdkKhID = new string[BwCount];
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] strGdMoney = new string[BwCount];
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strData = new string[BwCount];
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
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 9];
            int[] iFlag = new int[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            string[] strErrCountQ = new string[BwCount];
            string[] strErrCountH = new string[BwCount];
            bool[] blnsfRet = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strRedMeterKhID = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strErrInfo = new string[BwCount];
            string strIniUserDate = "";
            string[] strUserCardMeterNo = new string[BwCount];

            #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

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


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置囤积金额限值=0元,请稍候....");
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            try
            {
                //1------------
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
                            ResultDictionary["本地开户状态位前一后1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位前一后1"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位前一后1"][i] = "异常";
                    }
                }


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;
                MessageController.Instance.AddMessage("测试密钥下,正在进行发送购电次数=0,购电金额=0的本地开户,请稍候....");
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
                    paraFile[7] = "000001";        //电流互感器变比
                    paraFile[8] = "000001";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    strKhID[i] = "112233445566";
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
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态位前一后1"][i] += "-未开户";
                            ResultDictionary["本地开户状态位前一后2"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位前一后1"][i] += "-开户";
                            ResultDictionary["本地开户状态位前一后2"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位前一后1"][i] += "-异常";
                        ResultDictionary["本地开户状态位前一后2"][i] = "异常";
                    }
                }
                UploadTestResult("本地开户状态位前一后1");

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (ResultDictionary["本地开户状态位前一后1"][i].Equals("未开户-未开户"))
                        {
                            blnRet[i, 0] = true;
                        }
                        ResultDictionary["测试密钥下不可本地开户1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                    }
                }
                UploadTestResult("测试密钥下不可本地开户1");




                //2---------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                Common.Memset(ref iFlag, 1);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                if (Stop) return;
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电次数=0、购电金额=0、表号与表内不一致的本地开户,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    strUserCardMeterNo[i] = "000000112233";
                    paraFile[9] = strUserCardMeterNo[i];  //表号

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
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strRedMeterNo = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡客户编号2", strUserCardMeterNo);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表计客户编号2", strRedMeterNo);

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
                            ResultDictionary["本地开户状态位前一后2"][i] += "-未开户";
                            ResultDictionary["正式密钥下表号不一致不可本地开户2"][i] = "通过";
                            ResultDictionary["本地开户状态位前一后3"][i] = "未开户";
                            blnRet[i, 1] = true;
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位前一后2"][i] += "-开户";
                            ResultDictionary["正式密钥下表号不一致不可本地开户2"][i] = "不通过";
                            ResultDictionary["本地开户状态位前一后3"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位前一后2"][i] += "-异常";
                        ResultDictionary["正式密钥下表号不一致不可本地开户2"][i] = "不通过";
                        ResultDictionary["本地开户状态位前一后3"][i] = "异常";
                    }
                }
                UploadTestResult("本地开户状态位前一后2");
                UploadTestResult("正式密钥下表号不一致不可本地开户2");

                //开户卡客户编号2|表计客户编号2|本地开户状态位前-后2|正式密钥下表号不一致不可本地开户2|


                //3-------------------------------

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电金额=200、购电次数大于1的本地开户,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    strKhID[i] = "112233445566";
                    paraFile[10] = strKhID[i]; //客户编号

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
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态位前一后3"][i] += "-未开户";
                            ResultDictionary["本地开户状态位前一后4"][i] = "未开户";
                            blnRet[i, 2] = true;
                        }
                        else
                        {
                            ResultDictionary["本地开户状态位前一后3"][i] += "-开户";
                            ResultDictionary["本地开户状态位前一后4"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户状态位前一后3"][i] += "-异常";
                        ResultDictionary["本地开户状态位前一后4"][i] = "异常";
                    }
                    ResultDictionary["正式密钥下购电次数＞1不可本地开户3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                }
                UploadTestResult("本地开户状态位前一后3");
                UploadTestResult("正式密钥下购电次数＞1不可本地开户3");

                //本地开户状态位前-后3|正式密钥下购电次数＞1不可本地开户3|


                //4----------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
               string [] strSyMoneyQ4 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值=299元,请稍候....");
                Common.Memset(ref strRevCode, "04001004");
                Common.Memset(ref strData, "04001004" + "00029900");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,购电次数=1、购电金额=200、购电金额超囤积金额的本地开户,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                    walletFile[0] = "00004E20";    //购电次数
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
                MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
                string[] strTjMoneyXz = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值4", Common.StringConverToDecima(strTjMoneyXz));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
               string[]  strSyMoneyH4 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

               strSyMoneyQ4 = Common.StringConverToDecima(strSyMoneyQ4);
               strSyMoneyH4 = Common.StringConverToDecima(strSyMoneyH4);
               string[] strMoneyData = new string[BwCount];
               for (int i = 0; i < BwCount ; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   strMoneyData[i] = strSyMoneyQ4[i] + "-"+ strSyMoneyH4[i];
               }

               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额前一后4", strMoneyData);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                            {
                                ResultDictionary["开户前本地开户状态位5"][i] = "未开户";
                                ResultDictionary["本地开户状态位前一后4"][i] += "-未开户";
                                blnRet[i, 3] = true;
                            }
                            else
                            {
                                ResultDictionary["开户前本地开户状态位5"][i] = "开户";
                                ResultDictionary["本地开户状态位前一后4"][i] += "-开户";
                            }
                        }
                        else
                        {
                            ResultDictionary["开户前本地开户状态位5"][i] = "异常";
                            ResultDictionary["本地开户状态位前一后4"][i] += "-异常";
                        }
                        ResultDictionary["充值200超囤积不可本地开户4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                    }
                }
                UploadTestResult("本地开户状态位前一后4");
                UploadTestResult("充值200超囤积不可本地开户4");
                UploadTestResult("开户前本地开户状态位5");

                //囤积金额限值4|剩余金额前-后4|本地开户状态位前-后4|充值200超囤积不可本地开户4
                //5------------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值=0元,请稍候....");
                Common.Memset(ref strRevCode, "04001004");
                Common.Memset(ref strData, "04001004" + "00000000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前购电次数5", strGdCountQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前剩余金额5", Common.HexConverToDecimalism(strSyMoneyQ));

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电次数=0、购电金额=100的本地开户（只开户不充值）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                    strGdCount[i] = "00000000";
                    strGdMoney[i] = "100";

                    walletFile[0] = "00002710";    //购电次数
                    walletFile[1] = "00000000";    //购电次数

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

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡客户编号5", strKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡购电次数5", strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡购电金额5", strGdMoney);

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
                            ResultDictionary["开户后本地开户状态位5"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["开户后本地开户状态位5"][i] = "未开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户后本地开户状态位5"][i] = "异常";
                    }
                }
                UploadTestResult("开户后本地开户状态位5");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后客户编号5", strRedMeterKhID);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上2次购电日期,请稍候....");
                string[] strBuyDateLast2 = MeterProtocolAdapter.Instance.ReadData("03330102", 5);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2次购电日期5", strBuyDateLast2);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
                string[] strBuyMoneyLast2 = MeterProtocolAdapter.Instance.ReadData("03330302", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2次购电金额5", Common.StringConverToDecima(strBuyMoneyLast2));

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
                        if (ResultDictionary["开户后本地开户状态位5"][i] == "开户"
                            && strSyMoneyQ[i] == strSyMoneyH[i] && strRedMeterKhID[i] == strKhID[i]
                            && strGdCountQ[i] == strGdCountH[i] && strGdCountH[i] == "00000000"
                            && strBuyDateLast2[i] == "0000000000" && strBuyMoneyLast2[i] == "00000000")
                        {
                            blnRet[i, 4] = true;
                        }
                        ResultDictionary["正式密钥下开户且不充值5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                    }
                }
                UploadTestResult("正式密钥下开户且不充值5");
                //6--------------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥恢复为公钥....");
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
                MessageController.Instance.AddMessage("正在进行密钥更新....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

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
                            ResultDictionary["开户前本地开户状态位6"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["开户前本地开户状态位6"][i] = "未开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户前本地开户状态位6"][i] = "异常";
                    }
                }
                UploadTestResult("开户前本地开户状态位6");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前购电次数6", strGdCountQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户前剩余金额6", Common.HexConverToDecimalism(strSyMoneyQ));

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥状态下,购电次数=1、购电金额=100、客户编号与上步骤不一致的本地开户（开户、充值）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    strKhID[i] = "665544332211";
                    paraFile[10] = strKhID[i]; //客户编号

                    strGdCount[i] = "00000001";
                    strGdMoney[i] = "100";

                    walletFile[0] = "00002710";    //购电金额
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡客户编号6", strKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡购电次数6", strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡购电金额6", strGdMoney);

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
                            ResultDictionary["开户后本地开户状态位6"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["开户后本地开户状态位6"][i] = "未开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户后本地开户状态位6"][i] = "异常";
                    }
                }
                UploadTestResult("开户后本地开户状态位6");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后客户编号6", strRedMeterKhID);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                string[] strBuyDateLast1 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期6", strBuyDateLast1);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strBuyCountZLast1 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数6", strBuyCountZLast1);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                string[] strBuyMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额6", Common.StringConverToDecima(strBuyMoneyLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                string[] strSyMoneyQLast1 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额6", Common.StringConverToDecima(strSyMoneyQLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                string[] strSyMoneyHLast1 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额6", Common.StringConverToDecima(strSyMoneyHLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                string[] strSyMoneyZLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额6", Common.StringConverToDecima(strSyMoneyZLast1));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountH, out strSyMoneyH);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后购电次数6", strGdCountH);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户后剩余金额6", Common.HexConverToDecimalism(strSyMoneyH));

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (ResultDictionary["开户后本地开户状态位6"][i] == "开户" && strRedMeterKhID[i] == strKhID[i]
                            && strBuyCountZLast1[i] == "0001" && strSyMoneyZLast1[i] == "00030000" && strSyMoneyH[i] =="00007530")
                        {
                            blnRet[i, 5] = true;
                        }
                        ResultDictionary["正式密钥下开户且充值6"][i] = blnRet[i, 5] ? "通过" : "不通过";
                    }
                }
                UploadTestResult("正式密钥下开户且充值6");

                //7--------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥恢复为公钥....");
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
                MessageController.Instance.AddMessage("正在进行密钥更新....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电次数=0、购电金额=150、客户编号为445566332211的本地开户（只开户不充值）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    strKhID[i] = "445566332211";
                    paraFile[10] = strKhID[i]; //客户编号

                    strGdCount[i] = "00000000";
                    strGdMoney[i] = "150";

                    walletFile[0] = "00003A98";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

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
                strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户卡客户编号7", strKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户卡购电次数7", strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次开户卡购电金额7", strGdMoney);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户前购电次数7", strGdCountQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户前剩余金额7", Common.HexConverToDecimalism(strSyMoneyQ));

                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["第2次开户前本地开户状态位7"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["第2次开户前本地开户状态位7"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["第2次开户前本地开户状态位7"][i] = "异常";
                    }
                }
                UploadTestResult("第2次开户前本地开户状态位7");


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电次数=1、购电金额=200、客户编号为445566332211的本地开户（开户、充值）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;


                    strGdCount[i] = "00000001";
                    strGdMoney[i] = "200";

                    walletFile[0] = "00004E20";    //购电金额
                    walletFile[1] = "00000001";    //购电次数

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

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
                strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户卡客户编号7", strKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户卡购电次数7", strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户卡购电金额7", strGdMoney);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["第2次开户后本地开户状态位7"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["第2次开户后本地开户状态位7"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["第2次开户后本地开户状态位7"][i] = "异常";
                    }
                }
                UploadTestResult("第2次开户后本地开户状态位7");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后客户编号7", strRedMeterKhID);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                strBuyDateLast1 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期7", strBuyDateLast1);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                strBuyCountZLast1 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数7", strBuyCountZLast1);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strBuyMoneyLast1 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额7", Common.StringConverToDecima(strBuyMoneyLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strSyMoneyQLast1 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额7", Common.StringConverToDecima(strSyMoneyQLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strSyMoneyHLast1 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额7", Common.StringConverToDecima(strSyMoneyHLast1));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strSyMoneyZLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额7", Common.StringConverToDecima(strSyMoneyZLast1));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountH, out strSyMoneyH);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后购电次数7", strGdCountH);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第2次开户后剩余金额7", Common.HexConverToDecimalism(strSyMoneyH));


                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (strBuyDateLast1[i] == "0000000000") continue;
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strIniUserDate), DateTimes.FormatStringToDateTime(strBuyDateLast1[i]));
                        if (ResultDictionary["第2次开户后本地开户状态位7"][i] == "开户" && strRedMeterKhID[i] == strKhID[i]
                            && strBuyCountZLast1[i] == "0001" && strSyMoneyZLast1[i] == "00040000" && strSyMoneyH[i]=="00009C40"
                            && iErr < 300)
                        {
                            blnRet[i, 6] = true;
                        }
                        ResultDictionary["已开户下再次本地开户充值7"][i] = blnRet[i, 6] ? "通过" : "不通过";
                    }
                }
                UploadTestResult("已开户下再次本地开户充值7");

                //8---------------------------------------

                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户前远程开户状态位8"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户前远程开户状态位8"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["远程开户前本地开户状态位8"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户前本地开户状态位8"][i] = "未开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户前远程开户状态位8"][i] = "异常";
                        ResultDictionary["远程开户前本地开户状态位8"][i] = "异常";
                    }
                }
                UploadTestResult("远程开户前远程开户状态位8");
                UploadTestResult("远程开户前本地开户状态位8");


                //远程开户
                //购电金额+购电次数+客户编号
                MessageController.Instance.AddMessage("正式密钥状态下,正在进行购电次数=1的远程开户,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strData[i] = "00000000" + "00000001" + strKhID[i];
                }
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);

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
                            ResultDictionary["远程开户后远程开户状态位8"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户后远程开户状态位8"][i] = "开户";
                            blnRet[i, 7] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户后远程开户状态位8"][i] = "异常";
                    }
                    ResultDictionary["已本地开户再远程开户8"][i] = blnRet[i, 7] ? "通过" : "不通过";
                }
                UploadTestResult("远程开户后远程开户状态位8");
                UploadTestResult("已本地开户再远程开户8");


                //9------------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥恢复为公钥....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                Common.Memset(ref strData, "00000000");
                result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥更新....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);


                //远程开户
                //购电金额+购电次数+客户编号
                for (int i = 0; i < BwCount; i++)
                {
                    strKhID[i] = "665544332211";
                    strData[i] = "00000000" + "00000000" + strKhID[i];
                }
                MessageController.Instance.AddMessage("正式密钥状态下,正在进行购电次数=0的远程开户,请稍候....");
                bool[] bIniUserRet1 = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行第1次远程充值100元,请稍候....");
                //购电金额+购电次数+客户编号
                for (int i = 0; i < BwCount; i++)
                {
                    BuyCount[i] = "00000001";
                    BuyMoney[i] = "00002710";
                    strData[i] = BuyMoney[i] + BuyCount[i] + strKhID[i];
                }
                bool[] bIniUserRet2 = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行第2次远程充值100元,请稍候....");
                //购电金额+购电次数+客户编号
                for (int i = 0; i < BwCount; i++)
                {
                    BuyCount[i] = "00000002";
                    BuyMoney[i] = "00002710";
                    strData[i] = BuyMoney[i] + BuyCount[i] + strKhID[i];
                }
                bool[] bIniUserRet3 = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行第3次远程充值100元,请稍候....");
                //购电金额+购电次数+客户编号
                for (int i = 0; i < BwCount; i++)
                {
                    BuyCount[i] = "00000003";
                    BuyMoney[i] = "00002710";
                    strData[i] = BuyMoney[i] + BuyCount[i] + strKhID[i];
                }
                bool[] bIniUserRet4 = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

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
                            ResultDictionary["刷卡前本地开户状态位9"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["刷卡前本地开户状态位9"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["刷卡前本地开户状态位9"][i] = "异常";
                    }
                }
                UploadTestResult("刷卡前本地开户状态位9");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);
                strGdCountQ = Common.StringConverToIntger(strGdCountQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "刷卡前剩余金额9", strSyMoneyQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "刷卡前购电次数9", strGdCountQ);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正式密钥下,正在进行购电次数等于1,购电金额=50,的本地开户,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[10] = strKhID[i];

                    strGdCount[i] = "00000001";
                    strGdMoney[i] = "50";

                    walletFile[0] = "00001388";
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

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡刷卡次数9", strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户卡购电金额9", strGdMoney);

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
                            ResultDictionary["刷卡后本地开户状态位9"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["刷卡后本地开户状态位9"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["刷卡后本地开户状态位9"][i] = "异常";
                    }
                }
                UploadTestResult("刷卡后本地开户状态位9");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取购电后购电次数,请稍候....");
                strGdCount = MeterProtocolAdapter.Instance.ReadData("03330201", 2);
                strGdCount = Common.StringConverToIntger(strGdCount);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "刷卡后购电次数9", strGdCount);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取购电后剩余金额,请稍候....");
                strSyMoney = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
                strSyMoney =  Common.StringConverToDecima(strSyMoney);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "刷卡后剩余金额9", strSyMoney);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strSyMoneyZLast1 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                strSyMoneyZLast1 = Common.StringConverToDecima(strSyMoneyZLast1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "刷卡后上1次购电后累计购电金额9", strSyMoneyZLast1);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strGdCountQ[i]) && !string.IsNullOrEmpty(strSyMoneyQ[i]) && !string.IsNullOrEmpty(strGdCount[i]) && !string.IsNullOrEmpty(strSyMoney[i]))
                        if (strGdCountQ[i] == strGdCount[i] && strSyMoneyQ[i] == strSyMoney[i] && strSyMoneyZLast1[i] == "300"
                            && ResultDictionary["刷卡后本地开户状态位9"][i] == "开户" && ResultDictionary["刷卡前本地开户状态位9"][i] == "未开户")
                        {
                            blnRet[i, 8] = true;
                        }
                    ResultDictionary["表购电次数大于1开户卡购电次数＜2时仍可开户9"][i] = blnRet[i, 8] ? "通过" : "不通过";
                }
                UploadTestResult("表购电次数大于1开户卡购电次数＜2时仍可开户9");

                //-------------结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5]
                            && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8])
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
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
