using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 参数预置卡初始化
    /// </summary>
    public class ParamCardInitizalition : VerifyBase
    {
        //正式密钥下不可初始化1|测试密钥下购电次数非0不可初始化2

        //下发透支金额限值等于0|下发预置金额3|初始化前开关状态3|初始化后开关状态3|剩余金额和透支金额限值为0初始化后应拉闸3|
        //初始化后购电次数3|初始化后剩余金额3|初始化后本地开户状态位3|初始化后远程开户状态位3|初始化前保电状态位3|
        //初始化后保电状态字3|初始化前客户编号3|初始化后客户编号3|
        //初始化后非法插卡总次数3|初始化后异常插卡总次数3|初始化后正向有功总3|初始化后正向无功总3|初始化后反向有功总3|初始化后反向无功总3|
        //初始化前电表清零总次数3|初始化后电表清零总次数3|上1次电表清零发生时刻3|预置金额为0时初始化3|

        //预置金额值4|初始化前开关状态4|初始化后开关状态4|剩余金额不为0初始化后应合闸4|初始化后购电次数4|初始化后剩余金额4|
        //初始化后本地开户状态位4|初始化后远程开户状态位4|初始化前保电状态位4|初始化后保电状态字4|初始化前客户编号4|初始化后客户编号4|
        //初始化后非法插卡总次数4|初始化后异常插卡总次数4|初始化前电表清零总次数4|初始化后电表清零总次数4|上1次电表清零记录4|上1次购电日期4|
        //上1次购电后总购电次数4|上1次购电金额4|上1次购电前剩余金额4|上1次购电后剩余金额4|上1次购电后累计购电金额4|预置金额非0时初始化4|

        //下发囤积金额5|下发预置金额5|初始化前远程报警位5|初始化后远程报警位5|初始化后购电次数5|初始化后剩余金额5|初始化后本地开户状态位5|
        //初始化后远程开户状态位5|初始化前保电状态位5|初始化后保电状态字5|初始化前客户编号5|初始化后客户编号5|初始化后非法插卡总次数5|
        //初始化后异常插卡总次数5|初始化前电表清零总次数5|初始化后电表清零总次数5|上1次电表清零记录5|上1次购电日期5|
        //上1次购电后总购电次数日期5|上1次购电金额5|上1次购电前剩余金额5|上1次购电后剩余金额5|上1次购电后累计购电金额5|预置金额不受囤积金额限制影响初始化操作正确5|

        //保电解除6|操作后保电状态位6


        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] {"正式密钥下不可初始化1","测试密钥下购电次数非0不可初始化2",
        "下发透支金额限值等于0","下发预置金额3","初始化前继电器状态位3","初始化后继电器状态位3","剩余金额和透支金额限值为0初始化后应拉闸3",
        "初始化后购电次数3","初始化后剩余金额3","初始化后本地开户状态位3","初始化后远程开户状态位3","初始化前保电状态位3",
        "初始化后保电状态字3","初始化前客户编号3","初始化后客户编号3",
        "初始化后非法插卡总次数3","初始化后异常插卡总次数3","初始化后正向有功总3","初始化后正向无功总3","初始化后反向有功总3","初始化后反向无功总3",
        "初始化前电表清零总次数3","初始化后电表清零总次数3","上1次电表清零发生时刻3","预置金额为0时初始化3",
        "预置金额值4","初始化前继电器状态位4","初始化后继电器状态位4","剩余金额不为0初始化后应合闸4","初始化后购电次数4","初始化后剩余金额4",
        "初始化后本地开户状态位4","初始化后远程开户状态位4","初始化前保电状态位4","初始化后保电状态字4","初始化前客户编号4","初始化后客户编号4",
        "初始化后非法插卡总次数4","初始化后异常插卡总次数4","初始化前电表清零总次数4","初始化后电表清零总次数4","上1次电表清零记录4","上1次购电日期4",
        "上1次购电后总购电次数4","上1次购电金额4","上1次购电前剩余金额4","上1次购电后剩余金额4","上1次购电后累计购电金额4","预置金额非0时初始化4",
        "下发囤积金额5","下发预置金额5","初始化前远程报警位5","初始化后远程报警位5","初始化后购电次数5","初始化后剩余金额5","初始化后本地开户状态位5",
        "初始化后远程开户状态位5","初始化前保电状态位5","初始化后保电状态字5","初始化前客户编号5","初始化后客户编号5","初始化后非法插卡总次数5",
        "初始化后异常插卡总次数5","初始化前电表清零总次数5","初始化后电表清零总次数5","上1次电表清零记录5","上1次购电日期5",
        "上1次购电后总购电次数日期5","上1次购电金额5","上1次购电前剩余金额5","上1次购电后剩余金额5","上1次购电后累计购电金额5","预置金额不受囤积金额限制影响初始化操作正确5",
        "保电解除6","操作后保电状态位6","结论" };
            return true;
        }

        /// <summary>
        /// 开始检定业务
        /// </summary>
        public override void Verify()
        {

            base.Verify();
            if (Stop) return;
            //只升电压
            PowerOn();

            //读取表地址
            string[] strRand1 = new string[BwCount];
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strOutData = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 35];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            string[] status = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] strTjMoney = new string[BwCount];
            string[] paraFile = new string[9]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string strParaFile = ""; //参数信息文件
            string strwalletFile = "";//钱包文件
            string strpriceFile1 = "";//当前套电价文件
            string strpriceFile2 = "";//备用套电价文件
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            bool[] WriteUserResult = new bool[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strShowData = new string[BwCount];
            MessageBox.Show("请把参数预置卡插入表后按确定");

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
            string strDatetime = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDatetime += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDatetime);
            bool[] blnSetDateRetZb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在测试密钥状态下进行购电次数＝0、预置金额=100的钱包初始化,请稍候....");
            Common.Memset(ref strData, "00002710");
            Common.Memset(ref BuyCount, "00000000");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData, BuyCount);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额1为0,请稍候....");
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, "00000000");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2为1,请稍候....");
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, ("00000100"));
            Common.Memset(ref strPutApdu, "04D6811408");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            #endregion


            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置
                //步骤1-----------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在正式密钥状态下发行参数预置卡,请稍候....");

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

                    walletFile[0] = "00004E20";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00050000";
                        priceFile2[j] = "00050000";
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

                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
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
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                //|

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 0] = true;
                    }
                    ResultDictionary["正式密钥下不可初始化1"][i] = blnRet[i, 0] ? "不通过" : "通过";
                }
                UploadTestResult("正式密钥下不可初始化1");

                //2---------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复为公钥,请稍候....");
                bool[] blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次数≠0的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    walletFile[1] = "00000001";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
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
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 1] = true;
                    }
                    ResultDictionary["测试密钥下购电次数非0不可初始化2"][i] = blnRet[i, 1] ? "不通过" : "通过";
                }
                UploadTestResult("测试密钥下购电次数非0不可初始化2");

                //3--------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref  strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRetJC3 = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置透支金额限值为0,请稍候....");
                Common.Memset(ref strRevCode, "04001003");
                Common.Memset(ref strData, "04001003" + "00000000");
                bool[] blnSetTzRet3 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["下发透支金额限值等于0"][i] = blnSetTzRet3[i] ? "成功" : "失败";
                    blnRet[i, 2] = blnSetTzRet3[i];

                }
                UploadTestResult("下发透支金额限值等于0");



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号3", strMeterKhID);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ3 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数3", Common.StringConverToIntger(strRedQLCountQ3));


                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 0.1f, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                PowerOn();


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位3"][i] = "非保电";
                            blnRet[i, 4] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位3"][i] = "保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前保电状态位3"][i] = "异常";
                    }
                }
                Common.Memset(ref strShowData, "该项不启用");
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前继电器状态位3", strShowData);

                UploadTestResult("初始化前保电状态位3");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次数=0的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                //记录下清零时间
                string strQLdate = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strGdCounth = MeterProtocolAdapter.Instance.ReadData("03330201", 4);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号3", strMeterKhIDH);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;


                    if (!string.IsNullOrEmpty(strSyMoney[i]))
                    {
                        ResultDictionary["初始化后剩余金额3"][i] = strSyMoney[i];
                        if (strSyMoney[i] == "00000000")
                        {
                            blnRet[i, 7] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(strGdCounth[i]))
                    {
                        ResultDictionary["初始化后购电次数3"][i] = strGdCounth[i];
                        if (strGdCounth[i] == "0000")
                        {
                            blnRet[i, 8] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字3"][i] = "非保电";
                            blnRet[i, 6] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字3"][i] = "保电";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位3"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位3"][i] = "未开户";
                            blnRet[i, 9] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位3"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位3"][i] = "未开户";
                            blnRet[i, 10] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后保电状态字3"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位3"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位3"][i] = "异常";
                    }
                    ResultDictionary["下发预置金额3"][i] = "0";
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后继电器状态位3", strShowData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额和透支金额限值为0初始化后应拉闸3", strShowData);

                UploadTestResult("下发预置金额3");
                UploadTestResult("初始化后保电状态字3");
                UploadTestResult("初始化后剩余金额3");
                UploadTestResult("初始化后购电次数3");
                UploadTestResult("初始化后本地开户状态位3");
                UploadTestResult("初始化后远程开户状态位3");



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数3", Common.StringConverToIntger(strFfCount));
                //3|3|3|3|3|3|
                //3|3|3||
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数3", Common.StringConverToIntger(strYcCount));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后正向有功总电量,请稍候....");
                float[] energysZxyg = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后正向有功总3", GetConvertEnegy(energysZxyg));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后反向有功总电量,请稍候....");
                float[] energysZxwg = MeterProtocolAdapter.Instance.ReadEnergy((byte)1, (byte)0);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后反向有功总3", GetConvertEnegy(energysZxwg));

                if (!GlobalUnit.IsDan)
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取初始化后正向无功总电量,请稍候....");
                    float[] energysFxyg = MeterProtocolAdapter.Instance.ReadEnergy((byte)2, (byte)0);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后正向无功总3", GetConvertEnegy(energysFxyg));

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取初始化后反向无功总电量,请稍候....");
                    float[] energysFxwg = MeterProtocolAdapter.Instance.ReadEnergy((byte)3, (byte)0);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后反向无功总3", GetConvertEnegy(energysFxwg));
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后电表清零总次数,请稍候....");
                string[] strRedQLCountQH3 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数3", Common.StringConverToIntger(strRedQLCountQH3));

                if (Stop) return;
                string[] strMeterQLJL = new string[BwCount];
                Common.Memset(ref strMeterQLJL, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录3,请稍候....");
                strMeterQLJL = MeterProtocolAdapter.Instance.ReadData("03300101", 106);



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL[i]) || strMeterQLJL[i].ToString().Length < 68) continue;
                    string strDateTmp = strMeterQLJL[i].Substring(strMeterQLJL[i].Length - 12, 12);
                    strMeterQLJL[i] = strMeterQLJL[i].Substring(strMeterQLJL[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 300)
                    {
                        blnRet[i, 11] = true;
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零发生时刻3", strMeterQLJL);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 2] && blnRet[i, 4] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11])
                    {
                        ResultDictionary["预置金额为0时初始化3"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["预置金额为0时初始化3"][i] = "不通过";
                    }
                }
                UploadTestResult("预置金额为0时初始化3");

                //4--------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ4 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数4", Common.StringConverToIntger(strRedQLCountQ3));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref  strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRetJC = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                //Helper.EquipHelper.Instance.SetRelayControl(2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDQ4 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号4", strMeterKhIDQ4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {

                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位4"][i] = "非保电";
                            blnRet[i, 13] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位4"][i] = "保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前保电状态位4"][i] = "异常";
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前继电器状态位4", strShowData);
                UploadTestResult("初始化前保电状态位4");

                UploadTestResult("初始化前保电状态位4");


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次=0,预置金额为500的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    BuyMoney[i] = "0000C350";
                    walletFile[0] = BuyMoney[i];    //预置金额500
                    walletFile[1] = "00000000";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                //记录下清零时间
                string strQLdate4 = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney4 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney4 = Common.StringConverToDecima(strSyMoney4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strGdCounth4 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH4 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号4", strMeterKhIDH4);

                if (Stop) return;
                string[] strMeterQLJL4 = new string[BwCount];
                Common.Memset(ref strMeterQLJL4, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录,请稍候....");
                strMeterQLJL4 = MeterProtocolAdapter.Instance.ReadData("03300101", 106);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零记录4", strMeterQLJL4);

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                PowerOn();

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (!string.IsNullOrEmpty(strSyMoney4[i]))
                    {
                        ResultDictionary["初始化后剩余金额4"][i] = strSyMoney4[i];
                        ResultDictionary["预置金额非0时初始化4"][i] = "不通过";
                        if (strSyMoney4[i] == "500")
                        {
                            blnRet[i, 16] = true;
                            ResultDictionary["预置金额非0时初始化4"][i] = "通过";
                        }
                    }

                    if (!string.IsNullOrEmpty(strGdCounth4[i]))
                    {
                        ResultDictionary["初始化后购电次数4"][i] = strGdCounth4[i];
                        if (strGdCounth4[i] == "0000")
                        {
                            blnRet[i, 17] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(status3[i]))
                    {

                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字4"][i] = "非保电";
                            blnRet[i, 15] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字4"][i] = "保电";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位4"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位4"][i] = "未开户";
                            blnRet[i, 18] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位4"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位4"][i] = "未开户";
                            blnRet[i, 19] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后保电状态字4"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位4"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位4"][i] = "异常";
                    }

                    ResultDictionary["预置金额值4"][i] = "500";

                    //if (strMeterQLJL4[i].Length > 35)
                    //{
                    //    string strTmp = strMeterQLJL4[i].Trim('F');
                    //    strTmp = strTmp.Trim('0');
                    //    if (strTmp.Length < 21)
                    //    {
                    blnRet[i, 20] = true;
                    //    }
                    //}
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后继电器状态位4", strShowData);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额不为0初始化后应合闸4", strShowData);


                UploadTestResult("预置金额值4");
                UploadTestResult("预置金额非0时初始化4");
                UploadTestResult("初始化后保电状态字4");
                UploadTestResult("初始化后剩余金额4");
                UploadTestResult("初始化后购电次数4");
                UploadTestResult("初始化后本地开户状态位4");
                UploadTestResult("初始化后远程开户状态位4");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount4 = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数4", Common.StringConverToIntger(strFfCount4));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount4 = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数4", Common.StringConverToIntger(strYcCount4));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后电表清零总次数,请稍候....");
                string[] strRedQLCountQH4 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数4", Common.StringConverToIntger(strRedQLCountQH4));


                if (Stop) return;
                string[] strMeterSycGdRq4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                strMeterSycGdRq4 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期4", strMeterSycGdRq4);

                if (Stop) return;
                string[] strMeterSycGdCsRq4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数日期,请稍候....");
                strMeterSycGdCsRq4 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数4", strMeterSycGdCsRq4);

                if (Stop) return;
                string[] strMeterSycGdJe4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strMeterSycGdJe4 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额4", Common.StringConverToDecima(strMeterSycGdJe4));

                if (Stop) return;
                string[] strMeterSycGdqSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strMeterSycGdqSy4 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额4", Common.StringConverToDecima(strMeterSycGdqSy4));

                if (Stop) return;
                string[] strMeterSycGdhSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strMeterSycGdhSy4 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额4", Common.StringConverToDecima(strMeterSycGdhSy4));

                if (Stop) return;
                string[] strMeterSycGdhLjSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strMeterSycGdhLjSy4 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额4", Common.StringConverToDecima(strMeterSycGdhLjSy4));



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL4[i]) || strMeterQLJL4[i].Length < 68) continue;
                    string strDateTmp = strMeterQLJL4[i].Substring(strMeterQLJL4[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate4), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 300)
                    {
                        blnRet[i, 21] = true;
                    }
                }


                //5---------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strDataTmp += DateTime.Now.ToString("HHmmss");
                Common.Memset(ref strRevCode, "0400010C");
                Common.Memset(ref strData, strDataTmp);
                bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程报警,请稍候....");
                string strDateTime5 = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime5);
                bool[] blnBjRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ5 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数5", Common.StringConverToIntger(strRedQLCountQ5));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            ResultDictionary["初始化前远程报警位5"][i] = "有";
                            blnRet[i, 22] = true;

                        }
                        else
                        {
                            ResultDictionary["初始化前远程报警位5"][i] = "无";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前远程报警位5"][i] = "异常";
                    }


                }

                UploadTestResult("初始化前远程报警位5");



                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref  strData, "3A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                if (Stop) return;
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位5"][i] = "保电";
                            blnRet[i, 23] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位5"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前保电状态位5"][i] = "异常";
                    }
                }
                UploadTestResult("初始化前保电状态位5");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDQ5 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号5", strMeterKhIDQ5);


                //5----------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值为1000,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strRevCode[i] = "04001004";
                    strTjMoney[i] = "1000";
                    strData[i] = strRevCode[i] + "00100000";
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (result[i])
                    {
                        ResultDictionary["下发囤积金额5"][i] = "1000-成功";
                        blnRet[i, 24] = true;
                    }
                    else
                    {
                        ResultDictionary["下发囤积金额5"][i] = "1000-失败";
                    }


                }
                UploadTestResult("下发囤积金额5");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次==0,预置金额为1001的参数预置卡,请稍候....");



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    BuyMoney[i] = "00018704";
                    walletFile[0] = BuyMoney[i];    //预置金额500
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                //记录下清零时间
                string strQLdate5 = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发预置金额5", Common.HexConverToDecimalism(BuyMoney));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney5 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后剩余金额5", Common.StringConverToDecima(strSyMoney5));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH5 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号5", strMeterKhIDH5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountH5 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数5",Common.StringConverToIntger(strRedQLCountH5));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strBuyCountZ5 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后购电次数5", Common.StringConverToIntger(strBuyCountZ5));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount5 = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数5", Common.StringConverToIntger(strFfCount5));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount5 = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数5", Common.StringConverToIntger(strYcCount5));



                if (Stop) return;
                string[] strMeterSycGdRq5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                strMeterSycGdRq5 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期5", strMeterSycGdRq5);

                if (Stop) return;
                string[] strMeterSycGdCsRq5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数日期,请稍候....");
                strMeterSycGdCsRq5 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数日期5", strMeterSycGdCsRq5);

                if (Stop) return;
                string[] strMeterSycGdJe5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strMeterSycGdJe5 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额5", Common.StringConverToDecima(strMeterSycGdJe5));

                if (Stop) return;
                string[] strMeterSycGdqSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strMeterSycGdqSy5 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额5", Common.StringConverToDecima(strMeterSycGdqSy5));

                if (Stop) return;
                string[] strMeterSycGdhSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strMeterSycGdhSy5 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额5", Common.StringConverToDecima(strMeterSycGdhSy5));

                if (Stop) return;
                string[] strMeterSycGdhLjSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strMeterSycGdhLjSy5 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额5", Common.StringConverToDecima(strMeterSycGdhLjSy5));



                if (Stop) return;
                string[] strMeterQLJL5 = new string[BwCount];
                Common.Memset(ref strMeterQLJL5, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录,请稍候....");
                strMeterQLJL5 = MeterProtocolAdapter.Instance.ReadData("03300101", 106);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零记录5", strMeterQLJL5);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL4[i]) || strMeterQLJL4[i].Length < 68) continue;
                    string strDateTmp = strMeterQLJL5[i].Substring(strMeterQLJL5[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate5), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 180)
                    {
                        blnRet[i, 25] = true;
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
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            ResultDictionary["初始化后远程报警位5"][i] = "有";
                            ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "不通过";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程报警位5"][i] = "无";
                            ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "通过";
                            blnRet[i, 26] = true;

                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位5"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位5"][i] = "未开户";
                            blnRet[i, 27] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位5"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位5"][i] = "未开户";
                            blnRet[i, 28] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字5"][i] = "保电";
                            blnRet[i, 29] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字5"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后远程报警位5"][i] = "异常";
                        ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位5"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位5"][i] = "异常";
                        ResultDictionary["初始化后保电状态字5"][i] = "异常";
                    }

                    //if (strMeterQLJL5[i].Length > 35)
                    //{
                    //    string strTmp = strMeterQLJL4[i].Trim('F');
                    //    strTmp = strTmp.Trim('0');
                    //    if (strTmp.Length < 21)
                    //    {
                    blnRet[i, 30] = true;
                    //    }
                    //}
                }
                UploadTestResult("初始化后远程报警位5");
                UploadTestResult("初始化后本地开户状态位5");
                UploadTestResult("初始化后远程开户状态位5");

                UploadTestResult("初始化后保电状态字5");
                UploadTestResult("预置金额不受囤积金额限制影响初始化操作正确5");




                //6----------------------
                //保电解除6|操作后保电状态位6
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (blnSetJcbdRet[i])
                    {
                        ResultDictionary["保电解除6"][i] = "正常应答";
                        blnRet[i, 31] = true;
                    }
                    else
                    {
                        ResultDictionary["保电解除6"][i] = "异常应答";

                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["操作后保电状态位6"][i] = "有";
                        }
                        else
                        {
                            ResultDictionary["操作后保电状态位6"][i] = "无";
                            blnRet[i, 32] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["操作后保电状态位6"][i] = "异常";
                    }
                }
                UploadTestResult("保电解除6");
                UploadTestResult("操作后保电状态位6");


                //处理结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!blnRet[i, 0] && !blnRet[i, 1] && blnRet[i, 2]  && blnRet[i, 4]
                            && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9]
                            && blnRet[i, 10] && blnRet[i, 11]  && blnRet[i, 13] 
                            && blnRet[i, 15] && blnRet[i, 16] && blnRet[i, 17] && blnRet[i, 18] && blnRet[i, 19]
                            && blnRet[i, 20] && blnRet[i, 21] && blnRet[i, 22] && blnRet[i, 23] && blnRet[i, 24]
                            && blnRet[i, 25] && blnRet[i, 26] && blnRet[i, 27] && blnRet[i, 28]
                            && blnRet[i, 29] && blnRet[i, 30] && blnRet[i, 31] && blnRet[i, 32])
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
                #endregion
            }
            else
            {
                #region 外置
                //步骤1-----------------------
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在正式密钥状态下发行参数预置卡,请稍候....");

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

                    walletFile[0] = "00004E20";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00050000";
                        priceFile2[j] = "00050000";
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

                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
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
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                //|

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 0] = true;
                    }
                    ResultDictionary["正式密钥下不可初始化1"][i] = blnRet[i, 0] ? "不通过" : "通过";
                }
                UploadTestResult("正式密钥下不可初始化1");

                //2---------------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复为公钥,请稍候....");
                bool[] blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次数≠0的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    walletFile[1] = "00000001";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
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
                MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]) && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 1] = true;
                    }
                    ResultDictionary["测试密钥下购电次数非0不可初始化2"][i] = blnRet[i, 1] ? "不通过" : "通过";
                }
                UploadTestResult("测试密钥下购电次数非0不可初始化2");

                //3--------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref  strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRetJC3 = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置透支金额限值为0,请稍候....");
                Common.Memset(ref strRevCode, "04001003");
                Common.Memset(ref strData, "04001003" + "00000000");
                bool[] blnSetTzRet3 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    ResultDictionary["下发透支金额限值等于0"][i] = blnSetTzRet3[i] ? "成功" : "失败";
                    blnRet[i, 2] = blnSetTzRet3[i];

                }
                UploadTestResult("下发透支金额限值等于0");



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号3", strMeterKhID);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ3 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数3", Common.StringConverToIntger(strRedQLCountQ3));


                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 0.1f, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                PowerOn();


                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            ResultDictionary["初始化前继电器状态位3"][i] = "合闸";
                            blnRet[i, 3] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前继电器状态位3"][i] = "拉闸";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位3"][i] = "非保电";
                            blnRet[i, 4] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位3"][i] = "保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前继电器状态位3"][i] = "异常";
                        ResultDictionary["初始化前保电状态位3"][i] = "异常";
                    }
                }
                UploadTestResult("初始化前继电器状态位3");
                UploadTestResult("初始化前保电状态位3");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次数=0的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                //记录下清零时间
                string strQLdate = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strGdCounth = MeterProtocolAdapter.Instance.ReadData("03330201", 4);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号3", strMeterKhIDH);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取运行状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;


                    if (!string.IsNullOrEmpty(strSyMoney[i]))
                    {
                        ResultDictionary["初始化后剩余金额3"][i] = strSyMoney[i];
                        if (strSyMoney[i] == "00000000")
                        {
                            blnRet[i, 7] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(strGdCounth[i]))
                    {
                        ResultDictionary["初始化后购电次数3"][i] = strGdCounth[i];
                        if (strGdCounth[i] == "0000")
                        {
                            blnRet[i, 8] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            ResultDictionary["初始化后继电器状态位3"][i] = "合闸";
                            ResultDictionary["剩余金额和透支金额限值为0初始化后应拉闸3"][i] = "否";
                        }
                        else
                        {
                            ResultDictionary["初始化后继电器状态位3"][i] = "拉闸";
                            ResultDictionary["剩余金额和透支金额限值为0初始化后应拉闸3"][i] = "是";
                            blnRet[i, 5] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字3"][i] = "非保电";
                            blnRet[i, 6] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字3"][i] = "保电";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位3"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位3"][i] = "未开户";
                            blnRet[i, 9] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位3"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位3"][i] = "未开户";
                            blnRet[i, 10] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后继电器状态位3"][i] = "异常";
                        ResultDictionary["剩余金额和透支金额限值为0初始化后应拉闸3"][i] = "异常";
                        ResultDictionary["初始化后保电状态字3"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位3"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位3"][i] = "异常";
                    }
                    ResultDictionary["下发预置金额3"][i] = "0";
                }
                UploadTestResult("下发预置金额3");
                UploadTestResult("初始化后继电器状态位3");
                UploadTestResult("剩余金额和透支金额限值为0初始化后应拉闸3");
                UploadTestResult("初始化后保电状态字3");
                UploadTestResult("初始化后剩余金额3");
                UploadTestResult("初始化后购电次数3");
                UploadTestResult("初始化后本地开户状态位3");
                UploadTestResult("初始化后远程开户状态位3");



                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数3", Common.StringConverToIntger(strFfCount));
                //3|3|3|3|3|3|
                //3|3|3||
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数3", Common.StringConverToIntger(strYcCount));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后正向有功总电量,请稍候....");
                float[] energysZxyg = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后正向有功总3", GetConvertEnegy(energysZxyg));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后反向有功总电量,请稍候....");
                float[] energysZxwg = MeterProtocolAdapter.Instance.ReadEnergy((byte)1, (byte)0);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后反向有功总3", GetConvertEnegy(energysZxwg));

                if (!GlobalUnit.IsDan)
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取初始化后正向无功总电量,请稍候....");
                    float[] energysFxyg = MeterProtocolAdapter.Instance.ReadEnergy((byte)2, (byte)0);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后正向无功总3", GetConvertEnegy(energysFxyg));

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取初始化后反向无功总电量,请稍候....");
                    float[] energysFxwg = MeterProtocolAdapter.Instance.ReadEnergy((byte)3, (byte)0);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后反向无功总3", GetConvertEnegy(energysFxwg));
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后电表清零总次数,请稍候....");
                string[] strRedQLCountQH3 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数3", Common.StringConverToIntger(strRedQLCountQH3));

                if (Stop) return;
                string[] strMeterQLJL = new string[BwCount];
                Common.Memset(ref strMeterQLJL, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录3,请稍候....");
                strMeterQLJL = MeterProtocolAdapter.Instance.ReadData("03300101", 106);



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL[i]) || strMeterQLJL[i].ToString().Length < 68) continue;
                    string strDateTmp = strMeterQLJL[i].Substring(strMeterQLJL[i].Length - 12, 12);
                    strMeterQLJL[i] = strMeterQLJL[i].Substring(strMeterQLJL[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 300)
                    {
                        blnRet[i, 11] = true;
                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零发生时刻3", strMeterQLJL);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11])
                    {
                        ResultDictionary["预置金额为0时初始化3"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["预置金额为0时初始化3"][i] = "不通过";
                    }
                }
                UploadTestResult("预置金额为0时初始化3");

                //4--------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ4 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数4", Common.StringConverToIntger(strRedQLCountQ3));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref  strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRetJC = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                //Helper.EquipHelper.Instance.SetRelayControl(2);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDQ4 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号4", strMeterKhIDQ4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            ResultDictionary["初始化前继电器状态位4"][i] = "合闸";
                        }
                        else
                        {
                            ResultDictionary["初始化前继电器状态位4"][i] = "拉闸";
                            blnRet[i, 12] = true;
                        }

                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位4"][i] = "非保电";
                            blnRet[i, 13] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位4"][i] = "保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前继电器状态位4"][i] = "异常";
                        ResultDictionary["初始化前保电状态位4"][i] = "异常";
                    }

                }
                UploadTestResult("初始化前继电器状态位4");
                UploadTestResult("初始化前保电状态位4");


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次=0,预置金额为500的参数预置卡,请稍候....");

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    BuyMoney[i] = "0000C350";
                    walletFile[0] = BuyMoney[i];    //预置金额500
                    walletFile[1] = "00000000";    //购电次数
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);

                //记录下清零时间
                string strQLdate4 = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney4 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                strSyMoney4 = Common.StringConverToDecima(strSyMoney4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strGdCounth4 = MeterProtocolAdapter.Instance.ReadData("03330201", 4);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH4 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号4", strMeterKhIDH4);

                if (Stop) return;
                string[] strMeterQLJL4 = new string[BwCount];
                Common.Memset(ref strMeterQLJL4, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录,请稍候....");
                strMeterQLJL4 = MeterProtocolAdapter.Instance.ReadData("03300101", 106);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零记录4", strMeterQLJL4);

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                PowerOn();

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (!string.IsNullOrEmpty(strSyMoney4[i]))
                    {
                        ResultDictionary["初始化后剩余金额4"][i] = strSyMoney4[i];
                        ResultDictionary["预置金额非0时初始化4"][i] = "不通过";
                        if (strSyMoney4[i] == "500")
                        {
                            blnRet[i, 16] = true;
                            ResultDictionary["预置金额非0时初始化4"][i] = "通过";
                        }
                    }

                    if (!string.IsNullOrEmpty(strGdCounth4[i]))
                    {
                        ResultDictionary["初始化后购电次数4"][i] = strGdCounth4[i];
                        if (strGdCounth4[i] == "0000")
                        {
                            blnRet[i, 17] = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                        {
                            ResultDictionary["初始化后继电器状态位4"][i] = "合闸";
                            ResultDictionary["剩余金额不为0初始化后应合闸4"][i] = "是";
                            blnRet[i, 14] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后继电器状态位4"][i] = "拉闸";
                            ResultDictionary["剩余金额不为0初始化后应合闸4"][i] = "否";
                        }

                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字4"][i] = "非保电";
                            blnRet[i, 15] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字4"][i] = "保电";
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位4"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位4"][i] = "未开户";
                            blnRet[i, 18] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位4"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位4"][i] = "未开户";
                            blnRet[i, 19] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后继电器状态位4"][i] = "异常";
                        ResultDictionary["剩余金额不为0初始化后应合闸4"][i] = "异常";
                        ResultDictionary["初始化后保电状态字4"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位4"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位4"][i] = "异常";
                    }

                    ResultDictionary["预置金额值4"][i] = "500";

                    //if (strMeterQLJL4[i].Length > 35)
                    //{
                    //    string strTmp = strMeterQLJL4[i].Trim('F');
                    //    strTmp = strTmp.Trim('0');
                    //    if (strTmp.Length < 21)
                    //    {
                    blnRet[i, 20] = true;
                    //    }
                    //}
                }
                UploadTestResult("预置金额值4");
                UploadTestResult("初始化后继电器状态位4");
                UploadTestResult("预置金额非0时初始化4");
                UploadTestResult("剩余金额不为0初始化后应合闸4");
                UploadTestResult("初始化后保电状态字4");
                UploadTestResult("初始化后剩余金额4");
                UploadTestResult("初始化后购电次数4");
                UploadTestResult("初始化后本地开户状态位4");
                UploadTestResult("初始化后远程开户状态位4");

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount4 = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数4", Common.StringConverToIntger(strFfCount4));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount4 = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数4", Common.StringConverToIntger(strYcCount4));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取初始化后电表清零总次数,请稍候....");
                string[] strRedQLCountQH4 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数4", Common.StringConverToIntger(strRedQLCountQH4));


                if (Stop) return;
                string[] strMeterSycGdRq4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                strMeterSycGdRq4 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期4", strMeterSycGdRq4);

                if (Stop) return;
                string[] strMeterSycGdCsRq4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数日期,请稍候....");
                strMeterSycGdCsRq4 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数4", strMeterSycGdCsRq4);

                if (Stop) return;
                string[] strMeterSycGdJe4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strMeterSycGdJe4 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额4", Common.StringConverToDecima(strMeterSycGdJe4));

                if (Stop) return;
                string[] strMeterSycGdqSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strMeterSycGdqSy4 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额4", Common.StringConverToDecima(strMeterSycGdqSy4));

                if (Stop) return;
                string[] strMeterSycGdhSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strMeterSycGdhSy4 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额4", Common.StringConverToDecima(strMeterSycGdhSy4));

                if (Stop) return;
                string[] strMeterSycGdhLjSy4 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strMeterSycGdhLjSy4 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额4", Common.StringConverToDecima(strMeterSycGdhLjSy4));



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL4[i]) || strMeterQLJL4[i].Length < 68) continue;
                    string strDateTmp = strMeterQLJL4[i].Substring(strMeterQLJL4[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate4), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 300)
                    {
                        blnRet[i, 21] = true;
                    }
                }


                //5---------------

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strDataTmp += DateTime.Now.ToString("HHmmss");
                Common.Memset(ref strRevCode, "0400010C");
                Common.Memset(ref strData, strDataTmp);
                bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发远程报警,请稍候....");
                string strDateTime5 = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                Common.Memset(ref strData, "2A00" + strDateTime5);
                bool[] blnBjRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountQ5 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前电表清零总次数5", Common.StringConverToIntger(strRedQLCountQ5));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            ResultDictionary["初始化前远程报警位5"][i] = "有";
                            blnRet[i, 22] = true;

                        }
                        else
                        {
                            ResultDictionary["初始化前远程报警位5"][i] = "无";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前远程报警位5"][i] = "异常";
                    }

                    

                }
                UploadTestResult("初始化前远程报警位5");




                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref  strData, "3A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetBdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                if (Stop) return;
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["初始化前保电状态位5"][i] = "保电";
                            blnRet[i, 23] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化前保电状态位5"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化前保电状态位5"][i] = "异常";
                    }
                }
                UploadTestResult("初始化前保电状态位5");


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDQ5 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化前客户编号5", strMeterKhIDQ5);


                //5----------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值为1000,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strRevCode[i] = "04001004";
                    strTjMoney[i] = "1000";
                    strData[i] = strRevCode[i] + "00100000";
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (result[i])
                    {
                        ResultDictionary["下发囤积金额5"][i] = "1000-成功";
                        blnRet[i, 24] = true;
                    }
                    else
                    {
                        ResultDictionary["下发囤积金额5"][i] = "1000-失败";
                    }


                }
                UploadTestResult("下发囤积金额5");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在测试密钥状态下发行购电次==0,预置金额为1001的参数预置卡,请稍候....");



                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    BuyMoney[i] = "00018704";
                    walletFile[0] = BuyMoney[i];    //预置金额500
                    priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间


                    ISafeFileProtocol isafe = new SouthSafeFile();
                    int iresult = isafe.GetParamCardFileParam(paraFile, out strParaFile);
                    iresult = isafe.GetParamCardFileMoney(walletFile, out strwalletFile);
                    iresult = isafe.GetParamCardFilePrice1(priceFile1, out strpriceFile1);
                    iresult = isafe.GetParamCardFilePrice2(priceFile2, out strpriceFile2);

                    strParaFileArr[i] = strParaFile;
                    strwalletFileArr[i] = strwalletFile;
                    strpriceFile1Arr[i] = strpriceFile1;
                    strpriceFile2Arr[i] = strpriceFile2;
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr);
                //MessageBox.Show("请把卡插入表后按确定");

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(0);
                //记录下清零时间
                string strQLdate5 = DateTime.Now.ToString("yyMMddHHmmss");

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "下发预置金额5", Common.HexConverToDecimalism(BuyMoney));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取剩余金额....");
                string[] strSyMoney5 = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后剩余金额5", Common.StringConverToDecima(strSyMoney5));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                string[] strMeterKhIDH5 = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后客户编号5", strMeterKhIDH5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表清零总次数,请稍候....");
                string[] strRedQLCountH5 = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后电表清零总次数5", Common.StringConverToIntger(strRedQLCountH5));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数,请稍候....");
                string[] strBuyCountZ5 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后购电次数5", Common.StringConverToIntger(strBuyCountZ5));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
                string[] strFfCount5 = MeterProtocolAdapter.Instance.ReadData("03301400", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后非法插卡总次数5", Common.StringConverToIntger(strFfCount5));
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                string[] strYcCount5 = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "初始化后异常插卡总次数5", Common.StringConverToIntger(strYcCount5));



                if (Stop) return;
                string[] strMeterSycGdRq5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电日期,请稍候....");
                strMeterSycGdRq5 = MeterProtocolAdapter.Instance.ReadData("03330101", 5);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电日期5", strMeterSycGdRq5);

                if (Stop) return;
                string[] strMeterSycGdCsRq5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后总购电次数日期,请稍候....");
                strMeterSycGdCsRq5 = MeterProtocolAdapter.Instance.ReadData("03330201", 2);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后总购电次数日期5", strMeterSycGdCsRq5);

                if (Stop) return;
                string[] strMeterSycGdJe5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
                strMeterSycGdJe5 = MeterProtocolAdapter.Instance.ReadData("03330301", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额5", Common.StringConverToDecima(strMeterSycGdJe5));

                if (Stop) return;
                string[] strMeterSycGdqSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电前剩余金额,请稍候....");
                strMeterSycGdqSy5 = MeterProtocolAdapter.Instance.ReadData("03330401", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电前剩余金额5", Common.StringConverToDecima(strMeterSycGdqSy5));

                if (Stop) return;
                string[] strMeterSycGdhSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
                strMeterSycGdhSy5 = MeterProtocolAdapter.Instance.ReadData("03330501", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额5", Common.StringConverToDecima(strMeterSycGdhSy5));

                if (Stop) return;
                string[] strMeterSycGdhLjSy5 = new string[BwCount];
                MessageController.Instance.AddMessage("正在读取上1次购电后累计购电金额,请稍候....");
                strMeterSycGdhLjSy5 = MeterProtocolAdapter.Instance.ReadData("03330601", 4);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后累计购电金额5", Common.StringConverToDecima(strMeterSycGdhLjSy5));



                if (Stop) return;
                string[] strMeterQLJL5 = new string[BwCount];
                Common.Memset(ref strMeterQLJL5, "");
                MessageController.Instance.AddMessage("正在读取上1次电表清零记录,请稍候....");
                strMeterQLJL5 = MeterProtocolAdapter.Instance.ReadData("03300101", 106);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次电表清零记录5", strMeterQLJL5);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strMeterQLJL4[i]) || strMeterQLJL4[i].Length < 68) continue;
                    string strDateTmp = strMeterQLJL5[i].Substring(strMeterQLJL5[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strQLdate5), DateTimes.FormatStringToDateTime(strDateTmp));
                    if (iErr < 180)
                    {
                        blnRet[i, 25] = true;
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
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            ResultDictionary["初始化后远程报警位5"][i] = "有";
                            ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "不通过";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程报警位5"][i] = "无";
                            ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "通过";
                            blnRet[i, 26] = true;

                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) != 0x4000)
                        {
                            ResultDictionary["初始化后本地开户状态位5"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后本地开户状态位5"][i] = "未开户";
                            blnRet[i, 27] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x8000) != 0x8000)
                        {
                            ResultDictionary["初始化后远程开户状态位5"][i] = "开户";
                        }
                        else
                        {
                            ResultDictionary["初始化后远程开户状态位5"][i] = "未开户";
                            blnRet[i, 28] = true;
                        }
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["初始化后保电状态字5"][i] = "保电";
                            blnRet[i, 29] = true;
                        }
                        else
                        {
                            ResultDictionary["初始化后保电状态字5"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["初始化后远程报警位5"][i] = "异常";
                        ResultDictionary["预置金额不受囤积金额限制影响初始化操作正确5"][i] = "异常";
                        ResultDictionary["初始化后本地开户状态位5"][i] = "异常";
                        ResultDictionary["初始化后远程开户状态位5"][i] = "异常";
                        ResultDictionary["初始化后保电状态字5"][i] = "异常";
                    }

                    //if (strMeterQLJL5[i].Length > 35)
                    //{
                    //    string strTmp = strMeterQLJL4[i].Trim('F');
                    //    strTmp = strTmp.Trim('0');
                    //    if (strTmp.Length < 21)
                    //    {
                    blnRet[i, 30] = true;
                    //    }
                    //}
                }
                UploadTestResult("初始化后远程报警位5");
                UploadTestResult("初始化后本地开户状态位5");
                UploadTestResult("初始化后远程开户状态位5");

                UploadTestResult("初始化后保电状态字5");
                UploadTestResult("预置金额不受囤积金额限制影响初始化操作正确5");




                //6----------------------
                //保电解除6|操作后保电状态位6
                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    if (blnSetJcbdRet[i])
                    {
                        ResultDictionary["保电解除6"][i] = "正常应答";
                        blnRet[i, 31] = true;
                    }
                    else
                    {
                        ResultDictionary["保电解除6"][i] = "异常应答";

                    }
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["操作后保电状态位6"][i] = "有";
                        }
                        else
                        {
                            ResultDictionary["操作后保电状态位6"][i] = "无";
                            blnRet[i, 32] = true;
                        }
                    }
                    else
                    {
                        ResultDictionary["操作后保电状态位6"][i] = "异常";
                    }
                }
                UploadTestResult("保电解除6");
                UploadTestResult("操作后保电状态位6");


                //处理结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!blnRet[i, 0] && !blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4]
                            && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9]
                            && blnRet[i, 10] && blnRet[i, 11] && blnRet[i, 12] && blnRet[i, 13] && blnRet[i, 14]
                            && blnRet[i, 15] && blnRet[i, 16] && blnRet[i, 17] && blnRet[i, 18] && blnRet[i, 19]
                            && blnRet[i, 20] && blnRet[i, 21] && blnRet[i, 22] && blnRet[i, 23] && blnRet[i, 24]
                            && blnRet[i, 25] && blnRet[i, 26] && blnRet[i, 27] && blnRet[i, 28]
                            && blnRet[i, 29] && blnRet[i, 30] && blnRet[i, 31] && blnRet[i, 32])
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
                #endregion
            }
        }

        protected override string ItemKey
        {
            get
            {
                return null;
            }
        }

        protected override string ResultKey
        {
            get
            {
                return null;
            }
        }
        public ParamCardInitizalition(object plan)
            : base(plan)
        {

        }



        /// <summary>
        /// 转换电量
        /// </summary>
        /// <returns></returns>
        private string[] GetConvertEnegy(float[] enegy)
        {
            string[] strEnegy = new string[enegy.Length];
            for (int i = 0; i < enegy.Length; i++)
            {
                strEnegy[i] = enegy[i].ToString("F2");
            }
            return strEnegy;
        }

    }
}
