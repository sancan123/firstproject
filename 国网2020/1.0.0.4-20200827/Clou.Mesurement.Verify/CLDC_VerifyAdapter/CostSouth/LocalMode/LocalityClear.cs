using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 本地清零
    /// </summary>
    public class LocalityClear : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //本地模式不可远程清零1
        //本地开户状态清零前一清零后2|远程开户状态清零前一清零后2|异常插卡总次数清零前一清零后2|非法插卡总次数清零前一清零后2|远程报警状态清零前一清零后2|合闸状态清零前一清零后2|保电状态清零前一清零后2
        //表内客户编号清零前一清零后2|上1次跳闸时刻清零前一清零后2|上1次合闸时刻清零前一清零后2|上1次编程事件清零前一清零后2|校时总次数清零前一清零后2|上2次购电金额清零前一清零后2
        //密钥更新总次数清零前一清零后2|当前正向有功总电能清零前一清零后2|上1结算日正向有功总电能清零前一清零后2|清零总次数清零前一清零后2|初始化命令下发时刻2|上1次清零发生时刻2|本地模式钱包初始化清零2
        //保电下清零后状态3|非保电下清零后状态3|初始化命令3|本地清零不清保电3


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "本地模式不可远程清零1",
                                        "本地开户状态清零前一清零后2","远程开户状态清零前一清零后2","异常插卡总次数清零前一清零后2","非法插卡总次数清零前一清零后2","远程报警状态清零前一清零后2","继电器状态位清零前一清零后2","保电状态清零前一清零后2",
                                         "表内客户编号清零前一清零后2","上1次跳闸时刻清零前一清零后2","上1次编程事件清零前一清零后2","校时总次数清零前一清零后2","上2次购电金额清零前一清零后2",
                                         "密钥更新总次数清零前一清零后2","当前正向有功总电能清零前一清零后2","上1结算日正向有功总电能清零前一清零后2","清零总次数清零前一清零后2",
                                         "初始化命令下发时刻2","上1次清零发生时刻2","本地模式钱包初始化清零2",
                                         "保电下清零后状态3","非保电下清零后状态3","初始化命令3","本地清零不清保电3",
                                         "结论" };
            return true;
        }

        public LocalityClear(object plan)
            : base(plan)
        {
        }
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();


            try
            {
                string[] strRand1 = new string[BwCount];//随机数
                string[] strRand2 = new string[BwCount];//随机数
                string[] strEsamNo = new string[BwCount];//Esam序列号
                string[] strRevData = new string[BwCount];
                string[] strOutMac1 = new string[BwCount];
                string[] strOutMac2 = new string[BwCount];
                string[] strRevCode = new string[BwCount];
                string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
                string[] strSyMoney = new string[BwCount]; //当前剩余金额
                string[] strKhID = new string[BwCount]; //当前客户编号
                string[] strGdkKhID = new string[BwCount];
                string[] strGdCount = new string[BwCount]; //购电次数
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
                bool[,] blnRet = new bool[BwCount, 3];
                int[] iFlag = new int[BwCount];
                bool[] WriteUserResult = new bool[BwCount];
                bool[] BlnIniRet = new bool[BwCount];
                bool[] blnsfRet = new bool[BwCount];
                string[] MyStatus = new string[BwCount];
                string[] FkStatus = new string[BwCount];
                string[] DataTmp = new string[BwCount];
                string[] BuyMoney = new string[BwCount];
                string[] BuyCount = new string[BwCount];
                string[] outData = new string[BwCount];
                bool[] result = new bool[BwCount];
                string[] status3 = new string[BwCount];
                string[] strCode = new string[BwCount];
                int iSelectBwCount = 0;
                string[] strRevMac = new string[BwCount];
                string[] strErrInfo = new string[BwCount];
                string strDate = "";
                string CardType = "03";
                string[] strParaInfo = new string[BwCount];
                string[] strCardNo = new string[BwCount];
                string[] strShowData = new string[BwCount];

                

                #region 准备步骤
                //准备工作
                ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
                Common.Memset(ref strRevCode, "04001003");
                Common.Memset(ref strData, "04001003" + "00000000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strRevCode[i] = "04001004";
                    strData[i] = strRevCode[i] + "00000000";
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                Common.Memset(ref strData, "0000C350");
                result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行远程开户,请稍候....");
                //购电金额+购电次数+客户编号
                for (int i = 0; i < BwCount; i++)
                {
                    strKhID[i] = "112233445566";
                    strData[i] = "00000000" + "00000000" + strKhID[i];
                }
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在读取卡片序列号,请稍候....");
                bool[] blnRedCardNo = MeterProtocolAdapter.Instance.SouthReadUserCardNum(out strCardNo);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行本地开户,请稍候....");
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
                MessageController.Instance.AddMessage("正在进行操作卡文件....");
                WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
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
                bool[] blnSendTerminal = MeterProtocolAdapter.Instance.SouthTerminalSendParam(strCardNo, CardType, strParaInfo, out strfileReplyArr);


                Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 21);



                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置每月第1结算日,请稍候....");
                Common.Memset(ref strRevCode, "04000B01");
                Common.Memset(ref strData, "04000B01" + "0100");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间为2015年6月30日23:58:00,请稍候....");
                string strDatetime = "0400010C" + "150630" + "02" + "235800";
                Common.Memset(ref strRevCode, "0400010C");
                Common.Memset(ref strData, strDatetime);
                bool[] blnSetDateRetZb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


                MessageController.Instance.AddMessage("正在走字5分钟以便形成记录,请稍候....");
                bool blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);
                bool blnIsBreak = false;
                while (!blnIsBreak) //走字5分钟
                {
                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 300);
                    blnIsBreak = true;
                }
                PowerOn();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strData[i] += DateTime.Now.ToString("HHmmss");
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

                MessageBox.Show("请插入非法卡，以便形成记录。");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

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


                #endregion

                if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
                {
                    #region 内置
                    //1---------------------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        blnRet[i, 0] = blnMeterClearRet[i];
                        ResultDictionary["本地模式不可远程清零1"][i] = !blnRet[i, 0] ? "通过" : "不通过";
                    }

                    UploadTestResult("本地模式不可远程清零1");
                    //2------------------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值0元,请稍候....");
                    Common.Memset(ref strRevCode, "04001005");
                    Common.Memset(ref strData, "04001005" + "00000000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行远程充值100元,请稍候....");
                    //购电金额+购电次数+客户编号
                    for (int i = 0; i < BwCount; i++)
                    {
                        BuyCount[i] = "00000001";
                        BuyMoney[i] = "00002710";
                        strData[i] = BuyMoney[i] + BuyCount[i] + strKhID[i];
                    }
                    result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultBdjc = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                    string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                    Common.Memset(ref strData, "2A00" + strDateTime);
                    bool[] resultBj = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    //拉闸延时 
                    MessageController.Instance.AddMessage("跳闸延时时间,请稍候....");
                    for (int i = 0; i < BwCount; i++)
                    {
                        strCode[i] = "04001401";
                        strData[i] = "04001401" + "0000";

                    }
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                    System.Threading.Thread.Sleep(200);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                    Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultLz = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] = "未开户";
                                }
                                else
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] = "开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] = "未开户";
                                }
                                else
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] = "开户";
                                }

                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] = "非保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] = "保电";
                                }

                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] = "报警";
                                }
                                else
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] = "无报警";
                                }
                            }
                        }
                        else
                        {
                            ResultDictionary["远程开户状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["本地开户状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] = "异常";
                        }
                    }

                    if (Stop) return;
                    //钱包初始化之前读取 客户编号
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strReadKhIdQ = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountQ = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateQ = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateQ = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjQ = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
                    string[] strBuyMoneyQ = MeterProtocolAdapter.Instance.ReadData("03330302", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行密钥恢复,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 0);

                    if (Stop) return;
                    strDate = DateTime.Now.ToString("yyMMddHHmmss");
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref  strData, "0000C350");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);



                    if (Stop) return;
                    //钱包初始化之前读取 客户编号
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strReadKhIdH = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountH = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateH = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateH = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjH = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
                    string[] strBuyMoneyH = MeterProtocolAdapter.Instance.ReadData("03330302", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次清零发生时刻,请稍候....");
                    string[] strQlDate = MeterProtocolAdapter.Instance.ReadData("03300101", 6);


                    //异常插卡总次数  异常插卡总次数清零前一清零后2

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                    if (Stop) return;
                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0.05f, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] += "-未开户";
                                }
                                else
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] += "-开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] += "-未开户";
                                }
                                else
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] += "-开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] += "-非保电";
                                    ResultDictionary["非保电下清零后状态3"][i] = "非保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] += "-保电";
                                    ResultDictionary["非保电下清零后状态3"][i] = "保电";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] += "-报警";
                                }
                                else
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] += "-无报警";
                                }
                            }
                            else
                            {
                                ResultDictionary["远程开户状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["本地开户状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["非保电下清零后状态3"][i] = "异常";
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-异常";
                            }
                        }
                    }


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (strDate != "00".PadLeft(12, '0') && strQlDate[i] != "00".PadLeft(12, '0') && !string.IsNullOrEmpty(strQlCountH[i]) && !string.IsNullOrEmpty(strQlCountQ[i]))
                            {
                                int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDate), DateTimes.FormatStringToDateTime(strQlDate[i]));
                                if (ResultDictionary["远程开户状态清零前一清零后2"][i] == "开户-未开户" && ResultDictionary["本地开户状态清零前一清零后2"][i] == "开户-未开户"
                                   && ResultDictionary["远程报警状态清零前一清零后2"][i] == "报警-无报警"
                                   && ResultDictionary["保电状态清零前一清零后2"][i] == "非保电-非保电" && strReadKhIdQ[i] == strReadKhIdH[i] && strTzDateH[i] == "00".PadLeft(12, '0')
                                   && strBcsjH[i] == "00".PadLeft(12, '0') && strXsCountH[i] == "00".PadLeft(6, '0') && strBuyMoneyH[i] == "00".PadLeft(8, '0')
                                   && strUpdateKeyCountH[i] == "000000" && strMeterEnergyH[i] == "00".PadLeft(8, '0') && strJsrMeterEnergyH[i] == "00".PadLeft(8, '0')
                                   && strFFCountH[i] == "000000" && strYcCountH[i] == "000000"
                                   && Convert.ToInt32(strQlCountH[i]) == Convert.ToInt32(strQlCountQ[i]) + 1 && iDateErr <= 300)
                                {
                                    blnRet[i, 1] = true;
                                }
                            }
                            ResultDictionary["非法插卡总次数清零前一清零后2"][i] = strFFCountQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["异常插卡总次数清零前一清零后2"][i] = strYcCountQ[i] + "-" + strYcCountH[i];
                            ResultDictionary["表内客户编号清零前一清零后2"][i] = strReadKhIdQ[i] + "-" + strReadKhIdH[i];
                            ResultDictionary["上1次跳闸时刻清零前一清零后2"][i] = strTzDateQ[i] + "-" + strTzDateH[i];
                            //ResultDictionary["上1次合闸时刻清零前一清零后2"][i] = strHzDateQ[i] + "-" + strHzDateH[i];
                            ResultDictionary["上1次编程事件清零前一清零后2"][i] = strBcsjQ[i] + "-" + strBcsjH[i];
                            ResultDictionary["校时总次数清零前一清零后2"][i] = strXsCountQ[i] + "-" + strXsCountH[i];
                            ResultDictionary["上2次购电金额清零前一清零后2"][i] = strBuyMoneyQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["密钥更新总次数清零前一清零后2"][i] = strUpdateKeyCountQ[i] + "-" + strUpdateKeyCountH[i];
                            ResultDictionary["当前正向有功总电能清零前一清零后2"][i] = strMeterEnergyQ[i] + "-" + strMeterEnergyH[i];
                            ResultDictionary["上1结算日正向有功总电能清零前一清零后2"][i] = strJsrMeterEnergyQ[i] + "-" + strJsrMeterEnergyH[i];
                            ResultDictionary["清零总次数清零前一清零后2"][i] = strQlCountQ[i] + "-" + strQlCountH[i];
                            ResultDictionary["初始化命令下发时刻2"][i] = strDate;
                            ResultDictionary["上1次清零发生时刻2"][i] = strQlDate[i];
                            ResultDictionary["本地模式钱包初始化清零2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                        }
                    }

                    Common.Memset(ref strShowData, "该项不启用");
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位清零前一清零后2", strShowData);


                    UploadTestResult("远程开户状态清零前一清零后2");
                    UploadTestResult("本地开户状态清零前一清零后2");
                    UploadTestResult("远程报警状态清零前一清零后2");
                    UploadTestResult("保电状态清零前一清零后2");
                    UploadTestResult("表内客户编号清零前一清零后2");
                    UploadTestResult("上1次跳闸时刻清零前一清零后2");
                    UploadTestResult("上1次编程事件清零前一清零后2");
                    UploadTestResult("上1结算日正向有功总电能清零前一清零后2");
                    UploadTestResult("校时总次数清零前一清零后2");
                    UploadTestResult("非法插卡总次数清零前一清零后2");
                    UploadTestResult("异常插卡总次数清零前一清零后2");
                    UploadTestResult("上2次购电金额清零前一清零后2");
                    UploadTestResult("密钥更新总次数清零前一清零后2");
                    UploadTestResult("当前正向有功总电能清零前一清零后2");
                    UploadTestResult("清零总次数清零前一清零后2");
                    UploadTestResult("初始化命令下发时刻2");
                    UploadTestResult("上1次清零发生时刻2");
                    UploadTestResult("本地模式钱包初始化清零2");



                    //3----------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref  strData, "0000C350");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "保电";

                            }
                            else
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下清零后状态3"][i] = "异常";
                        }

                        if (ResultDictionary["保电下清零后状态3"][i] == "保电" && ResultDictionary["非保电下清零后状态3"][i] == "非保电" && result[i])
                        {
                            blnRet[i, 2] = true;
                        }
                        ResultDictionary["初始化命令3"][i] = result[i] ? "正常应答" : "异常应答";
                        ResultDictionary["本地清零不清保电3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                    }

                    UploadTestResult("保电下清零后状态3");
                    UploadTestResult("非保电下清零后状态3");
                    UploadTestResult("初始化命令3");
                    UploadTestResult("本地清零不清保电3");

                    //4-----------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    //判断结论
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
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
                    //1---------------------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        blnRet[i, 0] = blnMeterClearRet[i];
                        ResultDictionary["本地模式不可远程清零1"][i] = !blnRet[i, 0] ? "通过" : "不通过";
                    }

                    UploadTestResult("本地模式不可远程清零1");
                    //2------------------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值0元,请稍候....");
                    Common.Memset(ref strRevCode, "04001005");
                    Common.Memset(ref strData, "04001005" + "00000000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行远程充值100元,请稍候....");
                    //购电金额+购电次数+客户编号
                    for (int i = 0; i < BwCount; i++)
                    {
                        BuyCount[i] = "00000001";
                        BuyMoney[i] = "00002710";
                        strData[i] = BuyMoney[i] + BuyCount[i] + strKhID[i];
                    }
                    result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultBdjc = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                    string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                    Common.Memset(ref strData, "2A00" + strDateTime);
                    bool[] resultBj = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    //拉闸延时 
                    MessageController.Instance.AddMessage("跳闸延时时间,请稍候....");
                    for (int i = 0; i < BwCount; i++)
                    {
                        strCode[i] = "04001401";
                        strData[i] = "04001401" + "0000";

                    }
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                    System.Threading.Thread.Sleep(200);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                    Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultLz = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] = "未开户";
                                }
                                else
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] = "开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] = "未开户";
                                }
                                else
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] = "开户";
                                }

                                if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                                {
                                    ResultDictionary["继电器状态位清零前一清零后2"][i] = "通";
                                }
                                else
                                {
                                    ResultDictionary["继电器状态位清零前一清零后2"][i] = "断";
                                }

                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] = "非保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] = "保电";
                                }

                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] = "报警";
                                }
                                else
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] = "无报警";
                                }
                            }
                        }
                        else
                        {
                            ResultDictionary["远程开户状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["本地开户状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["继电器状态位清零前一清零后2"][i] = "异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] = "异常";
                        }
                    }

                    if (Stop) return;
                    //钱包初始化之前读取 客户编号
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strReadKhIdQ = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountQ = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateQ = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateQ = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjQ = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
                    string[] strBuyMoneyQ = MeterProtocolAdapter.Instance.ReadData("03330302", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行密钥恢复,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 0);

                    if (Stop) return;
                    strDate = DateTime.Now.ToString("yyMMddHHmmss");
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref  strData, "0000C350");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);



                    if (Stop) return;
                    //钱包初始化之前读取 客户编号
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strReadKhIdH = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountH = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateH = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateH = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjH = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上2次购电金额,请稍候....");
                    string[] strBuyMoneyH = MeterProtocolAdapter.Instance.ReadData("03330302", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次清零发生时刻,请稍候....");
                    string[] strQlDate = MeterProtocolAdapter.Instance.ReadData("03300101", 6);


                    //异常插卡总次数  异常插卡总次数清零前一清零后2

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                    if (Stop) return;
                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0.05f, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x8000) == 0x8000)
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] += "-未开户";
                                }
                                else
                                {
                                    ResultDictionary["远程开户状态清零前一清零后2"][i] += "-开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] += "-未开户";
                                }
                                else
                                {
                                    ResultDictionary["本地开户状态清零前一清零后2"][i] += "-开户";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                                {
                                    ResultDictionary["继电器状态位清零前一清零后2"][i] += "-通";
                                }
                                else
                                {
                                    ResultDictionary["继电器状态位清零前一清零后2"][i] += "-断";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] += "-非保电";
                                    ResultDictionary["非保电下清零后状态3"][i] = "非保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态清零前一清零后2"][i] += "-保电";
                                    ResultDictionary["非保电下清零后状态3"][i] = "保电";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] += "-报警";
                                }
                                else
                                {
                                    ResultDictionary["远程报警状态清零前一清零后2"][i] += "-无报警";
                                }
                            }
                            else
                            {
                                ResultDictionary["远程开户状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["本地开户状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["继电器状态位清零前一清零后2"][i] += "-异常";
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-异常";
                                ResultDictionary["非保电下清零后状态3"][i] = "异常";
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-异常";
                            }
                        }
                    }


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (strDate != "00".PadLeft(12, '0') && strQlDate[i] != "00".PadLeft(12, '0') && !string.IsNullOrEmpty(strQlCountH[i]) && !string.IsNullOrEmpty(strQlCountQ[i]))
                            {
                                int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDate), DateTimes.FormatStringToDateTime(strQlDate[i]));
                                if (ResultDictionary["远程开户状态清零前一清零后2"][i] == "开户-未开户" && ResultDictionary["本地开户状态清零前一清零后2"][i] == "开户-未开户"
                                   && ResultDictionary["远程报警状态清零前一清零后2"][i] == "报警-无报警" && ResultDictionary["继电器状态位清零前一清零后2"][i] == "断-通"
                                   && ResultDictionary["保电状态清零前一清零后2"][i] == "非保电-非保电" && strReadKhIdQ[i] == strReadKhIdH[i] && strTzDateH[i] == "00".PadLeft(12, '0')
                                   && strBcsjH[i] == "00".PadLeft(12, '0') && strXsCountH[i] == "00".PadLeft(6, '0') && strBuyMoneyH[i] == "00".PadLeft(8, '0')
                                   && strUpdateKeyCountH[i] == "000000" && strMeterEnergyH[i] == "00".PadLeft(8, '0') && strJsrMeterEnergyH[i] == "00".PadLeft(8, '0')
                                   && strFFCountH[i] == "000000" && strYcCountH[i] == "000000"
                                   && Convert.ToInt32(strQlCountH[i]) == Convert.ToInt32(strQlCountQ[i]) + 1 && iDateErr <= 300)
                                {
                                    blnRet[i, 1] = true;
                                }
                            }
                            ResultDictionary["非法插卡总次数清零前一清零后2"][i] = strFFCountQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["异常插卡总次数清零前一清零后2"][i] = strYcCountQ[i] + "-" + strYcCountH[i];
                            ResultDictionary["表内客户编号清零前一清零后2"][i] = strReadKhIdQ[i] + "-" + strReadKhIdH[i];
                            ResultDictionary["上1次跳闸时刻清零前一清零后2"][i] = strTzDateQ[i] + "-" + strTzDateH[i];
                            //ResultDictionary["上1次合闸时刻清零前一清零后2"][i] = strHzDateQ[i] + "-" + strHzDateH[i];
                            ResultDictionary["上1次编程事件清零前一清零后2"][i] = strBcsjQ[i] + "-" + strBcsjH[i];
                            ResultDictionary["校时总次数清零前一清零后2"][i] = strXsCountQ[i] + "-" + strXsCountH[i];
                            ResultDictionary["上2次购电金额清零前一清零后2"][i] = strBuyMoneyQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["密钥更新总次数清零前一清零后2"][i] = strUpdateKeyCountQ[i] + "-" + strUpdateKeyCountH[i];
                            ResultDictionary["当前正向有功总电能清零前一清零后2"][i] = strMeterEnergyQ[i] + "-" + strMeterEnergyH[i];
                            ResultDictionary["上1结算日正向有功总电能清零前一清零后2"][i] = strJsrMeterEnergyQ[i] + "-" + strJsrMeterEnergyH[i];
                            ResultDictionary["清零总次数清零前一清零后2"][i] = strQlCountQ[i] + "-" + strQlCountH[i];
                            ResultDictionary["初始化命令下发时刻2"][i] = strDate;
                            ResultDictionary["上1次清零发生时刻2"][i] = strQlDate[i];
                            ResultDictionary["本地模式钱包初始化清零2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                        }
                    }
                    UploadTestResult("远程开户状态清零前一清零后2");
                    UploadTestResult("本地开户状态清零前一清零后2");
                    UploadTestResult("远程报警状态清零前一清零后2");
                    UploadTestResult("继电器状态位清零前一清零后2");
                    UploadTestResult("保电状态清零前一清零后2");
                    UploadTestResult("表内客户编号清零前一清零后2");
                    UploadTestResult("上1次跳闸时刻清零前一清零后2");
                    UploadTestResult("上1次编程事件清零前一清零后2");
                    UploadTestResult("上1结算日正向有功总电能清零前一清零后2");
                    UploadTestResult("校时总次数清零前一清零后2");
                    UploadTestResult("非法插卡总次数清零前一清零后2");
                    UploadTestResult("异常插卡总次数清零前一清零后2");
                    UploadTestResult("上2次购电金额清零前一清零后2");
                    UploadTestResult("密钥更新总次数清零前一清零后2");
                    UploadTestResult("当前正向有功总电能清零前一清零后2");
                    UploadTestResult("清零总次数清零前一清零后2");
                    UploadTestResult("初始化命令下发时刻2");
                    UploadTestResult("上1次清零发生时刻2");
                    UploadTestResult("本地模式钱包初始化清零2");



                    //3----------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref  strData, "0000C350");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "保电";

                            }
                            else
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下清零后状态3"][i] = "异常";
                        }

                        if (ResultDictionary["保电下清零后状态3"][i] == "保电" && ResultDictionary["非保电下清零后状态3"][i] == "非保电" && result[i])
                        {
                            blnRet[i, 2] = true;
                        }
                        ResultDictionary["初始化命令3"][i] = result[i] ? "正常应答" : "异常应答";
                        ResultDictionary["本地清零不清保电3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                    }

                    UploadTestResult("保电下清零后状态3");
                    UploadTestResult("非保电下清零后状态3");
                    UploadTestResult("初始化命令3");
                    UploadTestResult("本地清零不清保电3");

                    //4-----------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    //判断结论
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
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
            catch (Exception ex)
            {
            }
        }
    }
}
