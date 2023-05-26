using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 用户卡充值
    /// </summary>
    public class UserCardInMoney : VerifyBase
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

        public UserCardInMoney(object plan)
            : base(plan)
        {

        }
        //输出列
        //远程开户状态字1|本地开户状态字1|本地未开户不可本地购电1|远程开户状态字2|本地开户状态字2|未开户不可本地购电2|远程开户状态字3|本地开户状态字3
        //卡客户编号3|表客户编号3|剩余金额前-后3|异常插卡总次数前-后3|客户编号不一致不可本地购电3|远程开户状态字4|本地开户状态字4|卡内表号4|表内表号4
        //剩余金额前-后4|异常插卡总次数前-后4|表号不一致不可本地购电4|卡购电次数5|购电次数前-后5|剩余金额前-后5|异常插卡记录5|购电次数比表内大2不可本地购电5|卡购电次数6
        //购电次数前-后6|剩余金额前-后6|上1次购电日期前-后6|购电次数与表内相等不可本地购电6|囤积金额限值7|购电次数前-后7|剩余金额前-后7|上1次购电日期7|上1次购电后总购电次数7|上1次购电金额7
        //上1次购电前剩余金额7|上1次购电后剩余金额7|上1次购电后累计购电金额7|购电次数比表内大1可本地购电7|囤积金额限值8|购电金额8|剩余金额前-后8|购电超囤积不可本地购电8


        protected override bool CheckPara()
        {

            ResultNames = new string[] { "远程开户状态字1","本地开户状态字1","本地未开户不可本地购电1","远程开户状态字2","本地开户状态字2","未开户不可本地购电2",
                                         "远程开户状态字3","本地开户状态字3","卡客户编号3","表客户编号3","剩余金额前一后3","异常插卡总次数前一后3","客户编号不一致不可本地购电3",
                                         "远程开户状态字4","本地开户状态字4","卡内表号4","表内表号4","剩余金额前一后4","异常插卡总次数前一后4","表号不一致不可本地购电4",
                                         "卡购电次数5","购电次数前一后5","剩余金额前一后5","异常插卡记录5","购电次数比表内大2不可本地购电5",
                                         "卡购电次数6","购电次数前一后6","剩余金额前一后6","上1次购电日期前一后6","购电次数与表内相等不可本地购电6",
                                         "囤积金额限值7","购电次数前一后7","剩余金额前一后7","上1次购电日期7","上1次购电后总购电次数7","上1次购电金额7",
                                         "上1次购电前剩余金额7","上1次购电后剩余金额7","上1次购电后累计购电金额7","购电次数比表内大1可本地购电7",
                                         "囤积金额限值8","购电金额8","剩余金额前一后8","购电超囤积不可本地购电8",
                                         "结论"};
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
            string[] strSyMoneyQ = new string[BwCount]; //剩余金额
            string[] strGdCountQ = new string[BwCount]; //购电次数
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strGdMoney = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] GdkKhID = new string[BwCount];
            string[] GdkMeterNo = new string[BwCount];
            string[] strMeterNo = new string[BwCount];
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] GdkGdCount = new string[BwCount];
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
            bool[,] blnRet = new bool[BwCount, 9];
            int[] iFlag = new int[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            bool[] rstTmp = new bool[BwCount];
            string[] strYcckInfo = new string[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            bool[] blnSfRet = new bool[BwCount];
            bool[] blnRecKeyRet = new bool[BwCount];
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
            string[] strFfckCountQ = new string[BwCount];
            string[] strFfckCountH = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            int iSelectBwCount = 0;

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            string[] strTjMoney = new string[BwCount];
            MessageController.Instance.AddMessage("正在设置囤积金额限值为0,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "04001004";
                strTjMoney[i] = "00000000";
                strData[i] = strRevCode[i] + strTjMoney[i];
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额1为0,请稍候....");
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, "00000000");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2为0,请稍候....");
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, ("00000000"));
            Common.Memset(ref strPutApdu, "04D6811408");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

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


            try
            {
                if (Stop) return;
                //1--------------
                MessageController.Instance.AddMessage("正在远程开户（不充值，客户编号为112233445566，表购电次数为0）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strKhID[i] = "112233445566";
                    strData[i] = "00000000" + "00000000" + strKhID[i];
                }
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);


                //--------卡片充值
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡中客户编号与表内一致、表号与表内一致、购电次数=表内购电次数+1、购电金额=100,请稍候....");
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
                    paraFile[10] = strKhID[i]; //客户编号
                    paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00002710";    //购电金额
                    walletFile[1] = "00000001";    //购电次数
                    //购电次数

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
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

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
                            ResultDictionary["远程开户状态字1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态字1"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态字1"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态字1"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户状态字1"][i] = "异常";
                        ResultDictionary["本地开户状态字1"][i] = "异常";
                    }
                    if (!string.IsNullOrEmpty(strSyMoneyQ[i]))
                    {
                        if (strSyMoneyQ[i] == "00002710" && ResultDictionary["远程开户状态字1"][i] == "开户" && ResultDictionary["本地开户状态字1"][i] == "未开户")
                        {
                            blnRet[i, 0] = true;
                        }
                        ResultDictionary["本地未开户不可本地购电1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                    }
                    else
                    {
                        ResultDictionary["本地未开户不可本地购电1"][i] = "失败";
                    }
                }
                UploadTestResult("远程开户状态字1");
                UploadTestResult("本地开户状态字1");
                UploadTestResult("本地未开户不可本地购电1");

                //2--------------------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥密钥恢复,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);

                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strRedMeterKhIDQ = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                Common.Memset(ref strData, "00002710");
                blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
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
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作，购电卡中客户编号与表内一致（112233445566）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

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
                string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);

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
                            ResultDictionary["远程开户状态字2"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态字2"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态字2"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态字2"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户状态字2"][i] = "异常";
                        ResultDictionary["本地开户状态字2"][i] = "异常";
                    }

                    if (strSyMoneyQ[i] == "00002710" && ResultDictionary["远程开户状态字2"][i] == "未开户"
                        && ResultDictionary["本地开户状态字2"][i] == "未开户"
                        && strRedMeterKhID[i] == strRedMeterKhIDQ[i]
                        && strGdCountQ[i] == "00000000")
                    {
                        blnRet[i, 1] = true;
                    }
                    ResultDictionary["未开户不可本地购电2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                }
                UploadTestResult("远程开户状态字2");
                UploadTestResult("本地开户状态字2");
                UploadTestResult("未开户不可本地购电2");


                //3--------------本地开户
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行远程开户并充值100元,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strData[i] = "00002710" + "00000001" + strKhID[i];
                }
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行购电次数=2的远程充值100元,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strData[i] = "00002710" + "00000002" + strKhID[i];
                }
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在进行本地开户（不充值）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                    paraFile[11] = "01";         //开户卡
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

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

                //充值-------------------------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡中客户编号与表内不一致、表号与表内一致，购电次数=表内购电次数+1,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    GdkKhID[i] = "665544332211";
                    paraFile[10] = GdkKhID[i]; //客户编号
                    paraFile[11] = "02"; //客户编号

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
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                        {
                            ResultDictionary["远程开户状态字3"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态字3"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态字3"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态字3"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户状态字3"][i] = "异常";
                        ResultDictionary["本地开户状态字3"][i] = "异常";
                    }
                }
                UploadTestResult("远程开户状态字3");
                UploadTestResult("本地开户状态字3");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strRedMeterKhIDHm = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡客户编号3", GdkKhID);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表客户编号3", strRedMeterKhIDHm);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                string[] strShowFfckCount = new string[BwCount];
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strShowFfckCount[i] = strFfckCountQ[i] + "-" + strFfckCountH[i];
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "异常插卡总次数前一后3", strShowFfckCount);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strFfckCountH[i]) && !string.IsNullOrEmpty(strFfckCountQ[i])
                       && !string.IsNullOrEmpty(GdkKhID[i]) && !string.IsNullOrEmpty(strRedMeterKhIDHm[i]))
                    {
                        if (Convert.ToInt32(strFfckCountH[i], 16) == Convert.ToInt32(strFfckCountQ[i], 16) + 1
                            && GdkKhID[i] != strRedMeterKhIDHm[i]
                            && strSyMoney[i] == "300"
                            && strGdCount[i] == "00000002")
                        {
                            blnRet[i, 2] = true;
                        }
                    }
                    ResultDictionary["剩余金额前一后3"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["客户编号不一致不可本地购电3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                }
                UploadTestResult("剩余金额前一后3");
                UploadTestResult("客户编号不一致不可本地购电3");

                //4--------------------------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                strSyMoneyQ = strSyMoney;


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡中客户编号与表内一致、表号与表内不一致，购电次数=表内购电次数+1,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    GdkMeterNo[i] = "000000112233";
                    paraFile[9] = GdkMeterNo[i];
                    paraFile[10] = GdkKhID[i]; //客户编号

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
                MessageController.Instance.AddMessage("正在回抄表内表号....");
                Common.Memset(ref strRevCode, "DF010001001E0006");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strMeterNo, out strOutMac1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡内表号4", GdkMeterNo);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号4", strMeterNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);

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
                            ResultDictionary["远程开户状态字4"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["远程开户状态字4"][i] = "开户";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["本地开户状态字4"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["本地开户状态字4"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["远程开户状态字4"][i] = "异常";
                        ResultDictionary["本地开户状态字4"][i] = "异常";
                    }
                }
                UploadTestResult("远程开户状态字4");
                UploadTestResult("本地开户状态字4");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (!string.IsNullOrEmpty(strFfckCountH[i]) && !string.IsNullOrEmpty(strFfckCountQ[i])
                         && !string.IsNullOrEmpty(GdkKhID[i]) && !string.IsNullOrEmpty(strRedMeterKhIDHm[i]))
                    {
                        if (Convert.ToInt32(strFfckCountH[i], 16) == Convert.ToInt32(strFfckCountQ[i], 16) + 1
                            && strSyMoney[i] == "300"
                            && strGdCount[i] == "00000002")
                        {
                            blnRet[i, 3] = true;
                        }
                    }

                    ResultDictionary["剩余金额前一后4"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["异常插卡总次数前一后4"][i] = strFfckCountQ[i] + "-" + strFfckCountH[i];
                    ResultDictionary["表号不一致不可本地购电4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                }
                UploadTestResult("剩余金额前一后4");
                UploadTestResult("异常插卡总次数前一后4");
                UploadTestResult("表号不一致不可本地购电4");

                //5---------------------------------
                strGdCountQ = strGdCount;
                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡客户编号和表号与表内一致，购电次数=（表内购电次数+2）,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    GdkKhID[i] = "112233445566";
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                    paraFile[10] = GdkKhID[i];


                    GdkGdCount[i] = "00000004";
                    walletFile[0] = "00004E20";    //购电金额
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
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                //记录下插卡时间
                string strstartDatetime = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡记录,请稍候....");
                string[] strYcJilu = MeterProtocolAdapter.Instance.ReadData("03301301", 36);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "异常插卡记录5", strYcJilu);

                //正向有功总
                if (Stop) return;
                string[] strEnergyZxygz = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                //反向有功电量
                if (Stop) return;
                string[] strEnergyFxygz = MeterProtocolAdapter.Instance.ReadData("00020000", 4);

                //判定插卡时间
                bool[] bTime = new bool[BwCount];
                bool[] bYgDl = new bool[BwCount];
                bool[] bWgDl = new bool[BwCount];
                bool[] bCkZcs = new bool[BwCount];
                bool[] bCkSyMoney = new bool[BwCount];
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strYcJilu[i]) && strYcJilu[i].Length == 72)
                    {
                        string strDateTmp = strYcJilu[i].Substring(72 - 12, 12);
                        if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strstartDatetime), DateTimes.FormatStringToDateTime(strDateTmp));
                        if (iErr < 120)
                        {
                            bTime[i] = true;
                        }
                        //插卡时总购电次数
                        string strCKGdCount = strYcJilu[i].Substring(24, 4);
                        if(strCKGdCount.Equals("0002"))
                        {
                            bCkZcs[i] = true;
                        }
                        //插卡时剩余金额
                        string strCKSyMoney = strYcJilu[i].Substring(16, 8);
                        if (strCKSyMoney.Equals("00030000"))
                        {
                            bCkSyMoney[i] = true;
                        }

                        string strZxygzJl = strYcJilu[i].Substring(8, 8);
                        string strFxygzJl = strYcJilu[i].Substring(0, 8);
                        //正向有功电量
                        if (Convert.ToInt32(strEnergyZxygz[i]) - Convert.ToInt32(strZxygzJl) > 1)
                        {
                            bYgDl[i] = false;
                        }
                        else
                        {
                            bYgDl[i] = true;
                        }
                        //反向有功电量
                        if (Convert.ToInt32(strEnergyFxygz[i]) - Convert.ToInt32(strFxygzJl) > 1)
                        {
                            bWgDl[i] = false;
                        }
                        else
                        {
                            bWgDl[i] = true;
                        }
                       

                    }

                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数5", GdkGdCount);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strGdCountQ[i]) && !string.IsNullOrEmpty(strGdCount[i])
                        && !string.IsNullOrEmpty(strSyMoneyQ[i]) && !string.IsNullOrEmpty(strSyMoney[i]))
                    {
                        if (strSyMoneyQ[i] == strSyMoney[i] && strGdCountQ[i] == strGdCount[i] 
                            && strSyMoney[i] == "300" && strGdCount[i] == "00000002"
                            && bTime[i] && bWgDl[i] && bYgDl[i]
                            && bCkZcs[i] && bCkSyMoney[i])
                        {
                            blnRet[i, 4] = true;
                        }
                    }
                    ResultDictionary["购电次数前一后5"][i] = strGdCountQ[i] + "-" + strGdCount[i];
                    ResultDictionary["剩余金额前一后5"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电次数比表内大2不可本地购电5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                }
                UploadTestResult("购电次数前一后5");
                UploadTestResult("剩余金额前一后5");
                UploadTestResult("购电次数比表内大2不可本地购电5");

                //6----------------------------------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("上1次购电日期,请稍候....");
                string[] strDateQ = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);
                strSyMoneyQ = Common.HexConverToDecimalism(strSyMoneyQ);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：购电卡客户编号和表号与表内一致，购电次数=表内购电次数,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    walletFile[0] = "00004E20";    //购电金额
                    GdkGdCount[i] = "00000002";
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
                MessageController.Instance.AddMessage("正在回抄剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡购电次数6", GdkGdCount);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);


                if (Stop) return;
                MessageController.Instance.AddMessage("上1次购电日期,请稍候....");
                string[] strDateH = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strGdCountQ[i]) && !string.IsNullOrEmpty(strGdCount[i])
                        && !string.IsNullOrEmpty(strSyMoneyQ[i]) && !string.IsNullOrEmpty(strSyMoney[i])
                        && !string.IsNullOrEmpty(strDateQ[i]) && !string.IsNullOrEmpty(strDateH[i]))
                    {
                        if (strGdCountQ[i] == strGdCount[i] && strSyMoneyQ[i] == strSyMoney[i] && strDateQ[i] == strDateH[i]
                             && strSyMoney[i] == "300" && strGdCount[i] == "00000002")
                        {
                            blnRet[i, 5] = true;
                        }
                    }
                    ResultDictionary["购电次数前一后6"][i] = strGdCountQ[i] + "-" + strGdCount[i];
                    ResultDictionary["剩余金额前一后6"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["上1次购电日期前一后6"][i] = strDateQ[i] + "-" + strDateH[i];
                    ResultDictionary["购电次数与表内相等不可本地购电6"][i] = blnRet[i, 5] ? "通过" : "不通过";
                }
                UploadTestResult("购电次数前一后6");
                UploadTestResult("剩余金额前一后6");
                UploadTestResult("上1次购电日期前一后6");
                UploadTestResult("购电次数与表内相等不可本地购电6");

                //7--------------------------    
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strRevCode[i] = "04001004";
                    strTjMoney[i] = "700";
                    strData[i] = strRevCode[i] + "00070000";
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值7", strTjMoney);

                strGdCountQ = strGdCount;
                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard(); ;
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作（200元）,购电金额<囤积金额限值,购电卡客户编号和表号与表内一致，购电次数=（表内购电次数+1),请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[10] = GdkKhID[i]; //客户编号

                    GdkGdCount[i] = "00000003";
                    BuyMoney[i] = "00004E20";
                    walletFile[0] = BuyMoney[i];    //购电金额
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
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                //记录下插卡时间
                strstartDatetime = DateTime.Now.ToString("yyMMddHHmmss");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                string[] strBuyDate = MeterProtocolAdapter.Instance.ReadData("03330101", 5);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期7", strBuyDate);


                //判定插卡时间
                bool[] b8Time = new bool[BwCount];
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    {
                        string strDateTmp = strBuyDate[i];
                        if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "0000000000") continue;
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strstartDatetime), DateTimes.FormatStringToDateTime(strDateTmp));
                        if (iErr < 180)
                        {
                            b8Time[i] = true;
                        }
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strS1GdCount = MeterProtocolAdapter.Instance.ReadData("03330201", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数7", strS1GdCount);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                string[] strSyS1Money = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额7", Common.StringConverToDecima(strSyS1Money));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                string[] strSyS1GdqMoney = MeterProtocolAdapter.Instance.ReadData("03330401", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额7", Common.StringConverToDecima(strSyS1GdqMoney));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                string[] strSyS1GdhMoney = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额7", Common.StringConverToDecima(strSyS1GdhMoney));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                string[] strSyS1GdhLeijiMoney = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额7", Common.StringConverToDecima(strSyS1GdhLeijiMoney));


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (strS1GdCount[i] == "0003" && strSyS1Money[i] == "00020000" && strSyS1GdqMoney[i] == "00030000"
                        && strSyS1GdhMoney[i] == "00050000" && strSyS1GdhLeijiMoney[i] == "00050000" && b8Time[i])
                    {
                        blnRet[i, 6] = true;
                    }

                    ResultDictionary["购电次数前一后7"][i] = strGdCountQ[i] + "-" + strGdCount[i];
                    ResultDictionary["剩余金额前一后7"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电次数比表内大1可本地购电7"][i] = blnRet[i, 6] ? "通过" : "不通过";
                }
                UploadTestResult("购电次数前一后7");
                UploadTestResult("剩余金额前一后7");
                UploadTestResult("购电次数比表内大1可本地购电7");

                //8---------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
                strTjMoney = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值8", Common.StringConverToDecima(strTjMoney));

                strSyMoneyQ = strSyMoney;

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在通过购电卡对表计进行购电操作：（购电金额+剩余金额）＞囤积金额限值,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                    paraFile[10] = GdkKhID[i]; //客户编号

                    GdkGdCount[i] = "00000004";
                    strGdMoney[i] = "00004E84";
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


                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电金额8", Common.HexConverToDecimalism(strGdMoney));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在回抄剩余金额及购电次数....");
                Common.Memset(ref strRevCode, "DF01000200000004");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);
                strSyMoney = Common.HexConverToDecimalism(strSyMoney);


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (strGdCount[i] == "00000003" && strSyMoney[i] == "500")
                    {
                        blnRet[i, 7] = true;
                    }

                    ResultDictionary["剩余金额前一后8"][i] = strSyMoneyQ[i] + "-" + strSyMoney[i];
                    ResultDictionary["购电超囤积不可本地购电8"][i] = blnRet[i, 7] ? "通过" : "不通过";
                }
                UploadTestResult("剩余金额前一后8");
                UploadTestResult("购电超囤积不可本地购电8");


                //-------------结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5]
                            && blnRet[i, 6] && blnRet[i, 7])
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
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
