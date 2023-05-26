﻿using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 开户卡参数更新
    /// </summary>
    public class InitCardParamUpdate : VerifyBase
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

        public InitCardParamUpdate(object plan)
            : base(plan)
        {

        }

        //当前密钥1|当前套电价文件设置值1|当前套电价文件读取值1|备用套电价文件设置值1|备用套电价文件读取值1|参数信息文件设置值1|参数信息文件读取值1|上1次编程记录1|测试密钥开户卡不可参数更新1
        //当前密钥2|开户状态位前-后2|当前套电价文件读取值2|备用套电价文件设置值2|备用套电价文件读取值2|参数信息文件设置值2|参数信息文件读取值2|上1次编程记录2|参数更新标志位全置0不更新2
        //当前密钥3|开户状态位前-后3|当前套电价文件读取值3|备用套电价文件设置值3|备用套电价文件读取值3|参数信息文件设置值3|参数信息文件读取值3|上1次编程记录3|参数更新标志位全置1全更新3

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥1","当前套电价文件设置值1","当前套电价文件读取值1","备用套电价文件设置值1","备用套电价文件读取值1",
                                         "参数信息文件设置值1","参数信息文件读取值1","上1次编程记录1","测试密钥开户卡不可参数更新1",
                                         "当前密钥2","开户状态位前一后2","当前套电价文件读取值2","备用套电价文件设置值2","备用套电价文件读取值2","参数信息文件设置值2",
                                         "参数信息文件读取值2","上1次编程记录2","参数更新标志位全置0不更新2",
                                         "当前密钥3","开户状态位前一后3","当前套电价文件读取值3","备用套电价文件设置值3","备用套电价文件读取值3","参数信息文件设置值3",
                                         "参数信息文件读取值3","上1次编程记录3","参数更新标志位全置1全更新3",
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
            string[] strUkDqtfl1 = new string[BwCount];
            string[] strUkBytfl1 = new string[BwCount];
            string[] strUkBj1 = new string[BwCount];
            string[] strDqtfl1 = new string[BwCount];
            string[] strBytfl1 = new string[BwCount];
            string[] strBj1 = new string[BwCount];
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
            bool[] BlnIniRet = new bool[BwCount];
            string[] strData = new string[BwCount];
            bool[] result = new bool[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] blnRecKeyRet = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 13];
            string[] MyStatus = new string[BwCount];
            string[] strRedData = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] status = new string[BwCount];
            int iSelectBwCount = 0;

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

            //1------------------------


            try
            {
                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("测试密钥下,使用开户卡对表计进行参数更新,参数更新标志位全置1（更新全部参数）,请稍候....");
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

                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00025600";
                        priceFile2[j] = "00025600";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000178";
                        priceFile2[j] = "00000178";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00013200";
                        priceFile2[j] = "00013200";
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
                        priceFile1[j] = "00010230";
                        priceFile2[j] = "00010340";
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
                if (Stop) return;
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


                //2--------------------


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
                MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
                string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["开户状态位前一后2"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["开户状态位前一后2"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户状态位前一后2"][i] = "异常";
                    }
                }


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在开户卡参数更新,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;
                    paraFile[0] = "00";            //保留
                    paraFile[1] = "00";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00003000";      //报警金额1
                    paraFile[6] = "00002000";      //报警金额2
                    paraFile[7] = "000005";        //电流互感器变比
                    paraFile[8] = "000008";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                    paraFile[10] = "665544332211"; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00030340";
                        priceFile2[j] = "00030230";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000200";
                        priceFile2[j] = "00000200";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00020200";
                        priceFile2[j] = "00020500";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "020202";
                        priceFile2[j] = "020202";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000200";
                        priceFile2[j] = "00000200";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00020000";
                        priceFile2[j] = "00020000";
                    }
                    for (int j = 44; j < 50; j++) //年第1-6结算日
                    {
                        priceFile1[j] = "020202";
                        priceFile2[j] = "020202";
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
                            ResultDictionary["开户状态位前一后2"][i] += "-未开户";
                        }
                        else
                        {
                            ResultDictionary["开户状态位前一后2"][i] += "-开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户状态位前一后2"][i] += "-异常";
                    }
                }
                UploadTestResult("开户状态位前一后2");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值2", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值2", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值2", strpriceFile2Arr);

                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadParaFileH, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值2", strReadParaFileH);
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
                            blnRet[i, 4] = true;
                        }
                    }
                }

                //当前套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取当前套电价文件....");
                Common.Memset(ref strRevCode, "DF010003000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadPriceFile1H, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值2", strReadPriceFile1H);
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
                            blnRet[i, 5] = true;
                        }
                    }
                }

                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strReadPriceFile2H, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值2", strReadPriceFile2H);
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
                    if (blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6] && strBcsj[i] == "00".PadLeft(100, '0') && ResultDictionary["开户状态位前一后2"][i] == "未开户-开户")
                    {
                        blnRet[i, 7] = true;
                    }
                    ResultDictionary["参数更新标志位全置0不更新2"][i] = blnRet[i, 7] ? "通过" : "不通过";

                }
                UploadTestResult("参数更新标志位全置0不更新2");



                //3----------------
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥3", MyStatus);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 0);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                Common.Memset(ref strData, "00002710");
                blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
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
                        if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                        {
                            ResultDictionary["开户状态位前一后3"][i] = "未开户";
                        }
                        else
                        {
                            ResultDictionary["开户状态位前一后3"][i] = "开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户状态位前一后3"][i] = "异常";
                    }
                }
               


                if (Stop) return;
                Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
                BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
                MessageController.Instance.AddMessage("正在开户卡参数更新,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (Stop) return;

                    paraFile[0] = "00";            //保留
                    paraFile[1] = "8F";            //参数更新标志位
                    paraFile[2] = "00000000";      //保留
                    paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                    paraFile[4] = "00";            //保留
                    paraFile[5] = "00003000";      //报警金额1
                    paraFile[6] = "00002000";      //报警金额2
                    paraFile[7] = "000005";        //电流互感器变比
                    paraFile[8] = "000008";        //电压互感器变比
                    paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');   //表号
                    paraFile[10] = "112233665544"; //客户编号
                    paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                    walletFile[0] = "00000000";    //购电金额
                    walletFile[1] = "00000000";    //购电次数

                    for (int j = 0; j < 12; j++)   //费率1-12
                    {
                        priceFile1[j] = "00040020";
                        priceFile2[j] = "00040060";
                    }
                    for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000300";
                        priceFile2[j] = "00000300";
                    }
                    for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00030000";
                        priceFile2[j] = "00030000";
                    }
                    for (int j = 25; j < 31; j++)  //年第1-6结算日
                    {
                        priceFile1[j] = "030303";
                        priceFile2[j] = "030303";
                    }
                    for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                    {
                        priceFile1[j] = "00000300";
                        priceFile2[j] = "00000300";
                    }
                    for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                    {
                        priceFile1[j] = "00030000";
                        priceFile2[j] = "00030000";
                    }
                    for (int j = 44; j < 50; j++) //年第1-6结算日
                    {
                        priceFile1[j] = "030303";
                        priceFile2[j] = "030303";
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
                string strIniUserDate = DateTime.Now.ToString("yyMMddHHmmss");
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
                            ResultDictionary["开户状态位前一后3"][i] += "-未开户";
                        }
                        else
                        {
                            ResultDictionary["开户状态位前一后3"][i] += "-开户";
                        }
                    }
                    else
                    {
                        ResultDictionary["开户状态位前一后3"][i] += "-异常";
                    }
                }
                UploadTestResult("开户状态位前一后3");


                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值3", strParaFileArr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值3", strpriceFile1Arr);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值3", strpriceFile2Arr);

                //参数信息文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
                Common.Memset(ref strRevCode, "DF01000100000030");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值3", strRedData);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                    if (strRedData[i].Length >= 96 && strParaFileArr[i].Length >= 90)
                    {
                        string strReadTmp = strRedData[i].Substring(8, 52);
                        string strParaFileTmp = strParaFileArr[i].Substring(8, 52);
                        if (strReadTmp == strParaFileTmp)
                        {
                            blnRet[i, 8] = true;
                        }
                    }
                }

                //当前套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取当前套电价文件....");
                Common.Memset(ref strRevCode, "DF010003000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值3", strRedData);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                    if (strRedData[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedData[i].Substring(0, strRedData[i].Length - 4);
                        string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strpriceFile1Arr[i].Length - 4);
                        if (strRedDataTmp == strpriceFile1Tmp)
                        {
                            blnRet[i, 9] = true;
                        }
                    }
                }


                //备用套电价文件
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取备用套电价文件....");
                Common.Memset(ref strRevCode, "DF010004000000C7");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值3", strRedData);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                    if (strRedData[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                    {
                        string strRedDataTmp = strRedData[i].Substring(0, strRedData[i].Length - 4);
                        string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strpriceFile2Arr[i].Length - 4);
                        if (strRedDataTmp == strpriceFile2Tmp)
                        {
                            blnRet[i, 10] = true;
                        }
                    }
                }


                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取上1次编程事件记录,请稍候....");
                strBcsj = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程记录3", strBcsj);
                
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strBcsj[i]) && strBcsj[i].Length == 100)
                    {
                        string strGxBj = "";  //编程事件里面的更新标志
                        strGxBj = strBcsj[i].Substring(strBcsj[i].Length - 22, 2);
                        string strDateTmp = strBcsj[i].Substring(strBcsj[i].Length - 12, 12);
                        int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strIniUserDate), DateTimes.FormatStringToDateTime(strDateTmp));
                        if (iErr < 301 && ResultDictionary["开户状态位前一后3"][i] == "未开户-开户" && strGxBj == "8F")      
                        {
                            blnRet[i, 11] = true;
                        }
                    }
                }
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnRet[i, 8] && blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11])
                    {
                        blnRet[i, 12] = true;
                        ResultDictionary["参数更新标志位全置1全更新3"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["参数更新标志位全置1全更新3"][i] = "不通过";
                    }
                }
                UploadTestResult("参数更新标志位全置1全更新3");



                //处理结果
                MessageController.Instance.AddMessage("正在处理结论,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {

                        if (blnRet[i, 3] && blnRet[i, 7] && blnRet[i, 12])
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
