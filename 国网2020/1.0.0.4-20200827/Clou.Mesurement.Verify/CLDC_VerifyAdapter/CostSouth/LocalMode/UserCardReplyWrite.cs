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
    /// 用户卡返写
    /// </summary>
    public class UserCardReplyWrite : VerifyBase
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

        public UserCardReplyWrite(object plan)
            : base(plan)
        {


        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "本地开户1","开户日期时间1","客户编号（返写）1","剩余金额（返写）1","透支金额（返写）1", "返写日期时间（返写）1","开户返写1",
                                         "购电结果2","卡购电次数2","表购电次数2","剩余金额（返写）2","购电次数（返写）2","电流互感器变比（返写）2","电压互感器变比（返写）2","卡购电次数小于表内时返写2",
                                         "购电结果3","卡购电次数3","表购电次数3","剩余金额（返写）3","购电次数（返写）3","卡购电次数大于表内＋1时返写3",
                                         "购电结果4","参数更新4","操作日期时间4","备用电价费率1","备用电价结算日1","剩余金额（返写）4","返写日期时间（返写）4","卡购电次数与表内相等并更新参数时返写4",
                                         "购电结果5","参数更新5","操作日期时间5","备用阶梯值1","备用阶梯电价1","剩余金额（返写）5","返写日期时间（返写）5","购电并更新参数时返写5",
                                         "补卡6","操作日期时间6","表号（返写）6","客户编号（返写）6","剩余金额（返写）6","购电次数（返写）6","透支金额（返写）6",
                                         "电流互感器变比（返写）6","电压互感器变比（返写）6","返写日期时间（返写）6","补卡时返写6",
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
            bool[,] blnRet = new bool[BwCount, 6];
            int[] iFlag = new int[BwCount];
            string[] strRedReplyInfo = new string[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            string[] strGdCount = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] status = new string[BwCount];
            string[] status3 = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] strData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] strAqInfo = new string[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            string[] strIBB = new string[BwCount];
            string[] strUBB = new string[BwCount];
            string[] strMeterNo = new string[BwCount];
            string[] strKhID = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strSyMoney = new string[BwCount];
            string[] strBuyCount = new string[BwCount];
            string[] strTjMoney = new string[BwCount];
            string[] strOpenDate = new string[BwCount];
            string[] strRepIBB = new string[BwCount];
            string[] strRepUBB = new string[BwCount];
            string[] strRepMeterNo = new string[BwCount];
            string[] strRepKhID = new string[BwCount];
            string[] strRepSyMoney = new string[BwCount];
            string[] strRepBuyCount = new string[BwCount];
            string[] strRepTjMoney = new string[BwCount];
            string[] strRepReplyDate = new string[BwCount];
            string[] strRepCheckDate = new string[BwCount];
            int iSelectBwCount=0;
            string[] strErrInfo = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strTzMoney = new string[BwCount];
            string[] strSetByjtz1 = new string[BwCount];
            string[] strSetByjtdj1 = new string[BwCount];
            string[] strSetBydjfl1 = new string[BwCount];
            string[] strSetBydjjsr1 = new string[BwCount];
            string[] strRevByjtz1 = new string[BwCount];
            string[] strRevdByjtdj1 = new string[BwCount];
            string[] strRevBydjfl1 = new string[BwCount];
            string[] strRevBydjjsr1 = new string[BwCount];
            string[] strRevMac = new string[BwCount];


            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00000000");
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
            MessageController.Instance.AddMessage("正在进行客户编号为112233445566的远程开户,请稍候....");
            //购电金额+购电次数+客户编号
            Common.Memset(ref strKhID, "112233445566");
            Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
            bool[] blnKhRet = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥密钥恢复,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            Common.Memset(ref iFlag, 0);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00000000");
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
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strRevCode, "04000306");
            Common.Memset(ref strIBB, "000020");
            Common.Memset(ref strPutApdu, "04D6811807");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2,strPutApdu, strRevCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strRevCode, "04000307");
            Common.Memset(ref strUBB, ("000010"));
            Common.Memset(ref strPutApdu, "04D6811B07");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2,strPutApdu, strRevCode, strData);

            //设置备用套费率
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套费率,请稍候....");
            Common.Memset(ref strRevCode, "040502FF");
            Common.Memset(ref strData, "00011000"); //	费率1=1.1元/kWh
            Common.Memset(ref strPutApdu, "04D6840408");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strRevCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套阶梯值、阶梯电价、结算日,请稍候....");
            Common.Memset(ref strRevCode, "04060AFF");
            Common.Memset(ref strData, "00000210" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                        + "00031000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                        + "030201" + "000000" + "000000" + "000000" + "000000" + "000000");
            Common.Memset(ref strPutApdu, "04D684344A");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strRevCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表安全模块的运行信息文件,请稍候....");
            Common.Memset(ref strRevCode, "DF01000600000032");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strAqInfo, out strOutMac1);

            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoneyQ);

            #endregion

            try
            {
                //1---------------------------------------
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
                MessageController.Instance.AddMessage("正在开户并充值50元....");

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
                    strIBB[i] = "000001";
                    strUBB[i] = "000002";
                    paraFile[7] = strIBB[i];        //电流互感器变比
                    paraFile[8] = strUBB[i];        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    strKhID[i] = "112233445566";
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00001388";    //购电金额
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
                //MessageBox.Show("请把用户卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                Common.Memset(ref strOpenDate, DateTime.Now.ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);


                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在再次充值50元....");

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
                    strIBB[i] = "000001";
                    strUBB[i] = "000002";
                    paraFile[7] = strIBB[i];        //电流互感器变比
                    paraFile[8] = strUBB[i];        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    strKhID[i] = "112233445566";
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00001388";    //购电金额
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
                Common.Memset(ref strOpenDate, DateTime.Now.ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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
                            status3[i] = "0";
                            ResultDictionary["本地开户1"][i] = "成功";
                        }
                        else
                        {
                            status3[i] = "1";
                            ResultDictionary["本地开户1"][i] = "失败";
                        }
                    }
                    else
                    {
                        ResultDictionary["本地开户1"][i] = "异常";
                    }
                }
                UploadTestResult("本地开户1");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length >= 100)
                    {
                        strRepKhID[i] = strfileReplyArr[i].Substring(34, 12);
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepReplyDate[i] = strfileReplyArr[i].Substring(84, 10);
                        strTzMoney[i] = strfileReplyArr[i].Substring(62, 8);
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开户日期时间1", strOpenDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "客户编号（返写）1", strRepKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）1", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "返写日期时间（返写）1", strRepReplyDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额（返写）1", Common.StringConverToDecima(strTzMoney));

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]) && !string.IsNullOrEmpty(strRepReplyDate[i]) &&  strRepReplyDate[i] != "0000000000")
                    {
                        int iDiffErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strOpenDate[i]), DateTimes.FormatStringToDateTime(strRepReplyDate[i]));
                        if (status3[i] == "0" && iDiffErr <= 60 && strRepSyMoney[i] == "00002710")
                        {
                            blnRet[i, 0] = true;
                        }
                    }
                    ResultDictionary["开户返写1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                }
                UploadTestResult("开户返写1");

                //2----------------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在使用用户卡充值，卡购电次数＜表购电次数,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdCount[i] = "00000001";
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

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                Common.Memset(ref strRepIBB, "");
                Common.Memset(ref strRepUBB, "");
                Common.Memset(ref strRepSyMoney, "");
                Common.Memset(ref strRepBuyCount, "");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length >= 100)
                    {
                        strRepIBB[i] = strfileReplyArr[i].Substring(10, 6);
                        strRepUBB[i] = strfileReplyArr[i].Substring(16, 6);
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepBuyCount[i] = strfileReplyArr[i].Substring(54, 8);
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数,请稍候....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strBuyCount, out strSyMoney);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strSyMoney[i]) && !string.IsNullOrEmpty(status[i]) && !string.IsNullOrEmpty(strRepUBB[i]) && !string.IsNullOrEmpty(strRepIBB[i]))
                    {
                        if (strSyMoney[i] == "00002710" && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001
                            && strRepIBB[i] == strIBB[i] && strRepUBB[i] == strUBB[i])
                        {
                            blnRet[i, 1] = true;

                        }
                    }
                    ResultDictionary["购电结果2"][i] = blnRet[i, 1] ? "失败" : "异常";
                    ResultDictionary["卡购电次数小于表内时返写2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                }
                UploadTestResult("购电结果2");
                UploadTestResult("卡购电次数小于表内时返写2");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数2", Common.StringConverToIntger(strGdCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表购电次数2", Common.StringConverToIntger(strBuyCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）2", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电次数（返写）2", Common.StringConverToIntger(strRepBuyCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电流互感器变比（返写）2", Common.StringConverToIntger(strRepIBB));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压互感器变比（返写）2", Common.StringConverToIntger(strRepUBB));

                //3-------------------------------=
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在购电，卡购电次数=表购电次数+2,购电金额100元,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdCount[i] = "00000004";
                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000004";    //购电次数

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
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                Common.Memset(ref strRepSyMoney, "");
                Common.Memset(ref strRepBuyCount, "");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length >= 100)
                    {
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepBuyCount[i] = strfileReplyArr[i].Substring(54, 8);
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strBuyCount, out strSyMoneyQ);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strSyMoney[i]) && !string.IsNullOrEmpty(status[i]))
                    {
                        if (strSyMoney[i] == "00002710" && strRepBuyCount[i] == "00000002" && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                        {
                            blnRet[i, 2] = true;
                        }
                    }
                    ResultDictionary["购电结果3"][i] = blnRet[i, 2] ? "失败" : "异常";
                    ResultDictionary["卡购电次数大于表内＋1时返写3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                }
                UploadTestResult("购电结果3");
                UploadTestResult("卡购电次数大于表内＋1时返写3");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数3", Common.StringConverToIntger(strGdCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表购电次数3", Common.StringConverToIntger(strBuyCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）3", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电次数（返写）3", Common.StringConverToIntger(strRepBuyCount));

                //4----------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在购电，卡购电次数=表购电次数,购电金额100元,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[7] = "000020";
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000002";    //购电次数

                    strSetBydjfl1[i] = "00052000";
                    strSetBydjjsr1[i] = "050100";
                    priceFile2[0] = "00052000";   //	费率1:  5.2元/kWh
                    priceFile2[25] = "050100";    //结算日1: 5月1日0时


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
                Common.Memset(ref strOpenDate, DateTime.Now.ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);


                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref  strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strpriceFile2Arr, out strRevMac);

                Common.Memset(ref strRevBydjfl1, "");
                Common.Memset(ref strRevBydjjsr1, "");
                Common.Memset(ref strRepSyMoney, "");
                Common.Memset(ref strRepReplyDate, "");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && !string.IsNullOrEmpty(strpriceFile2Arr[i]) && strfileReplyArr[i].Length >= 100 && strpriceFile2Arr[i].Length >= 398)
                    {
                        strRevBydjfl1[i] = strpriceFile2Arr[i].Substring(8, 8);
                        strRevBydjjsr1[i] = strpriceFile2Arr[i].Substring(208, 6);
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepReplyDate[i] = strfileReplyArr[i].Substring(84, 10);
                    }
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strBuyCount, out strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {

                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strSyMoney[i]) && !string.IsNullOrEmpty(status[i]) && !string.IsNullOrEmpty(status[i]) && strRepReplyDate[i] != "0000000000"
                        && !string.IsNullOrEmpty(strRepReplyDate[i]))
                    {
                        int iDiffErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strOpenDate[i]), DateTimes.FormatStringToDateTime(strRepReplyDate[i]));
                        if (strSyMoney[i] == "00002710" && iDiffErr < 119 && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001
                            && strSetBydjfl1[i] == strRevBydjfl1[i] && strSetBydjjsr1[i] == strRevBydjjsr1[i])
                        {
                            blnRet[i, 3] = true;

                        }
                    }
                    ResultDictionary["购电结果4"][i] = blnRet[i, 3] ? "失败" : "异常";
                    ResultDictionary["参数更新4"][i] = blnRet[i, 3] ? "成功" : "失败";
                    ResultDictionary["卡购电次数与表内相等并更新参数时返写4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
                UploadTestResult("购电结果4");
                UploadTestResult("参数更新4");
                UploadTestResult("卡购电次数与表内相等并更新参数时返写4");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作日期时间4", strOpenDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用电价费率1", strRevBydjfl1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用电价结算日1", strRevBydjjsr1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）4", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "返写日期时间（返写）4", strRepReplyDate);

                //5------------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在购电，卡购电次数=表购电次数+1,购电金额100元,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000003";    //购电次数

                    strSetByjtz1[i] = "00000620";
                    strSetByjtdj1[i] = "00072000";
                    priceFile2[12] = "00000620";   //		阶梯值1：6.2kWh
                    priceFile2[18] = "00072000";    //  	阶梯电价1：7.2元/kWh

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
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                Common.Memset(ref strOpenDate, DateTime.Now.ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref  strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strpriceFile2Arr, out strRevMac);

                Common.Memset(ref strRevByjtz1, "");
                Common.Memset(ref strRevdByjtdj1, "");
                Common.Memset(ref strRepSyMoney, "");
                Common.Memset(ref strRepReplyDate, "");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && !string.IsNullOrEmpty(strpriceFile2Arr[i]) && strfileReplyArr[i].Length >= 100 && strpriceFile2Arr[i].Length >= 398)
                    {
                        strRevByjtz1[i] = strpriceFile2Arr[i].Substring(104, 8);
                        strRevdByjtdj1[i] = strpriceFile2Arr[i].Substring(152, 8);
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepReplyDate[i] = strfileReplyArr[i].Substring(84, 10);
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strBuyCount, out strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {

                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && strRepReplyDate[i] != "0000000000" && !string.IsNullOrEmpty(strRepReplyDate[i]))
                    {
                        int iDiffErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strOpenDate[i]), DateTimes.FormatStringToDateTime(strRepReplyDate[i]));
                        if (strSyMoney[i] == "00004E20" && iDiffErr <= 60 && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001
                            && strSetByjtz1[i] == strRevByjtz1[i] && strSetByjtdj1[i] == strRevdByjtdj1[i])
                        {
                            blnRet[i, 4] = true;

                        }
                    }
                    ResultDictionary["购电结果5"][i] = blnRet[i, 4] ? "成功" : "失败";
                    ResultDictionary["参数更新5"][i] = blnRet[i, 4] ? "成功" : "失败";
                    ResultDictionary["购电并更新参数时返写5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                }
                UploadTestResult("购电结果5");
                UploadTestResult("参数更新5");
                UploadTestResult("购电并更新参数时返写5");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作日期时间5", strOpenDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用阶梯值1", strRevByjtz1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用阶梯电价1", strRevdByjtdj1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）5", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "返写日期时间（返写）5", strRepReplyDate);

                //6-------------------------------
                MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡2插入表后按确定");
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在用用户卡2进行补卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    strGdCount[i] = "00000004";
                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000004";    //购电次数

                    strIBB[i] = "000005";
                    strUBB[i] = "000005";

                    paraFile[7] = strIBB[i];        //电流互感器变比
                    paraFile[8] = strUBB[i];        //电压互感器变比


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
                Common.Memset(ref strOpenDate, DateTime.Now.ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);


                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在回抄用户卡返写信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthReadUserCard(out strParaFileArr, out strwalletFileArr, out strpriceFile1Arr, out strpriceFile2Arr, out strfileReplyArr, out strControlFilePlainArr);

                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                Common.Memset(ref strRepMeterNo, "");
                Common.Memset(ref strRepKhID, "");
                Common.Memset(ref strRepSyMoney, "");
                Common.Memset(ref strRepBuyCount, "");
                Common.Memset(ref strRepTjMoney, "");
                Common.Memset(ref strRepIBB, "");
                Common.Memset(ref strRepUBB, "");
                Common.Memset(ref strRepReplyDate, "");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strfileReplyArr[i]) && strfileReplyArr[i].Length >= 100)
                    {
                        strRepMeterNo[i] = strfileReplyArr[i].Substring(22, 12);
                        strRepKhID[i] = strfileReplyArr[i].Substring(34, 12);
                        strRepSyMoney[i] = strfileReplyArr[i].Substring(46, 8);
                        strRepBuyCount[i] = strfileReplyArr[i].Substring(54, 8);
                        strRepTjMoney[i] = strfileReplyArr[i].Substring(62, 8);
                        strRepIBB[i] = strfileReplyArr[i].Substring(10, 6);
                        strRepUBB[i] = strfileReplyArr[i].Substring(16, 6);
                        strRepReplyDate[i] = strfileReplyArr[i].Substring(84, 10);
                    }
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄表内购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strBuyCount, out strSyMoney);



                for (int i = 0; i < BwCount; i++)
                {

                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && strRepReplyDate[i] != "0000000000" && !string.IsNullOrEmpty(strRepReplyDate[i])
                        && !string.IsNullOrEmpty(strRepUBB[i]) && !string.IsNullOrEmpty(strRepIBB[i]) && !string.IsNullOrEmpty(strUBB[i]) && !string.IsNullOrEmpty(strIBB[i]))
                    {
                        int iDiffErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strOpenDate[i]), DateTimes.FormatStringToDateTime(strRepReplyDate[i]));
                        if (strSyMoney[i] == "00007530" && iDiffErr < 119 && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001
                            && strRepIBB[i] != strIBB[i] && strRepUBB[i] != strUBB[i])
                        {
                            blnRet[i, 5] = true;

                        }
                    }
                    ResultDictionary["补卡6"][i] = blnRet[i, 5] ? "成功" : "失败";
                    ResultDictionary["补卡时返写6"][i] = blnRet[i, 5] ? "通过" : "不通过";
                }
                UploadTestResult("补卡6");
                UploadTestResult("补卡时返写6");

                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作日期时间6", strOpenDate);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表号（返写）6", strRepMeterNo);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "客户编号（返写）6", strRepKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额（返写）6", Common.HexConverToDecimalism(strRepSyMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电次数（返写）6", Common.StringConverToIntger(strRepBuyCount));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额（返写）6", Common.StringConverToDecima(strRepTjMoney));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电流互感器变比（返写）6", Common.StringConverToIntger(strRepIBB));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压互感器变比（返写）6", Common.StringConverToIntger(strRepUBB));
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "返写日期时间（返写）6", strRepReplyDate);


                //处理结果
                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
                UploadTestResult("结论");
            }
            catch (Exception ex) 
            {
                
            }
        }
    }
}
