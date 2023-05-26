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
    /// 预置卡参数更新
    /// </summary>
    public class ParamCardParamUpdate : VerifyBase
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

        public ParamCardParamUpdate(object plan)
            : base(plan)
        {


        }

        //当前密钥1|异常插卡次数 前一后1|正式密钥下参数更新1|
        //当前密钥2|参数信息文件2|购电信息文件2|




        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥1", "异常插卡总次数前一后1","正式密钥下参数更新1", 
                                         "当前密钥2", "参数信息文件设置值2", "参数信息文件读取值2","购电信息文件设置值2", "购电信息文件读取值2",
                                         "当前套电价文件设置值2", "当前套电价文件读取值2","备用套电价文件设置值2", "备用套电价文件读取值2",
                                         "上1次购电金额2","上1次购电后剩余金额2","上1次清零发生时刻2","上1次编程记录2","预置卡参数更新2","结论" };
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
            string[] paraFile = new string[9]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string strParaFile = ""; //参数信息文件
            string strwalletFile = "";//钱包文件
            string strpriceFile1 = "";//当前套电价文件
            string strpriceFile2 = "";//备用套电价文件
            string[] strYzkDqtfl1 = new string[BwCount];
            string[] strYzkBytfl1 = new string[BwCount];
            string[] strYzkBj1 = new string[BwCount];
            string[] strDqtfl1 = new string[BwCount];
            string[] strBytfl1 = new string[BwCount];
            string[] strBj1 = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            bool[] WriteUserResult = new bool[BwCount];
            bool[] result = new bool[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevMac2 = new string[BwCount];
            string[] strGdCountH = new string[BwCount];
            string[] strSyMoneyH = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] blnRecKeyRet = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] status = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strFfckCountQ = new string[BwCount];
            string[] strFfckCountH = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 10];
            string[] strRedData = new string[BwCount];
            int iSelectBwCount = 0;

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            MessageBox.Show("请把参数预置卡插入表后按确定");


            #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            

            //1--------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            bool[] BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("正在预置卡参数更新,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[0] = "00";            //保留
                paraFile[1] = "8F";            //参数更新标志位
                paraFile[2] = "00000000";      //保留
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[4] = "00";            //保留
                paraFile[5] = "00006000";      //报警金额1
                paraFile[6] = "00005000";      //报警金额2
                paraFile[7] = "000001";        //电流互感器变比
                paraFile[8] = "000001";        //电压互感器变比

                BuyMoney[i] = "00004E20";
                BuyCount[i] = "00000000";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = BuyCount[i];    //购电次数

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

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            //status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

            string[] strShowFfckCount = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strShowFfckCount[i] = strFfckCountQ[i] + "-" + strFfckCountH[i];
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "异常插卡总次数前一后1", strShowFfckCount);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(strFfckCountQ[i]) && !string.IsNullOrEmpty(strFfckCountH[i]) && Convert.ToInt32(strFfckCountQ[i]) + 1 == Convert.ToInt32(strFfckCountH[i]))
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["正式密钥下参数更新1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("正式密钥下参数更新1");


            //2--------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("正在预置卡参数更新,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

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
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0);
            string strJlDateTime = DateTime.Now.ToString("yyMMddHHmmss");

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
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(MyStatus[i])) continue;
                if (MyStatus[i].EndsWith("1FFFF"))
                {
                    MyStatus[i] = "正式密钥";
                }
                else
                {
                    MyStatus[i] = "测试密钥";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥2", MyStatus);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件设置值2", strParaFileArr);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电信息文件设置值2", strwalletFileArr);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件设置值2", strpriceFile1Arr);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件设置值2", strpriceFile2Arr);


            //参数信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
            Common.Memset(ref strRevCode, "DF01000100000030");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件读取值2", strRedData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strParaFileArr[i])) continue;
                if (strRedData[i].Length >= 96 && strParaFileArr[i].Length >= 64)
                {
                    string strReadTmp = strRedData[i].Substring(8, 52);
                    string strParaFileTmp = strParaFileArr[i].Substring(8, 52);
                    if (strReadTmp == strParaFileTmp)
                    {
                        blnRet[i, 1] = true;
                    }
                }
            }

            //购电信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取安全模块购电信息文件....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountH, out strSyMoneyH);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strRedData[i] = strSyMoneyH[i] + strGdCountH[i];
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电信息文件读取值2", strRedData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty(strSyMoneyH[i]) || string.IsNullOrEmpty(BuyMoney[i]) || string.IsNullOrEmpty(strGdCountH[i]) || string.IsNullOrEmpty(BuyMoney[i])) continue;
                if (strwalletFileArr[i] == strRedData[i])
                {
                    blnRet[i, 2] = true;
                }
            }

            //当前套电价文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前套电价文件....");
            Common.Memset(ref strRevCode, "DF010003000000C7");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件读取值2", strRedData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strpriceFile1Arr[i])) continue;
                if (strRedData[i].Length == 398 && strpriceFile1Arr[i].Length == 398)
                {
                    string strRedDataTmp = strRedData[i].Substring(0, strRedData[i].Length - 4);
                    string strpriceFile1Tmp = strpriceFile1Arr[i].Substring(0, strRedData[i].Length - 4);
                    if (strRedDataTmp[i] == strpriceFile1Tmp[i])
                    {
                        blnRet[i, 3] = true;
                    }
                }
            }
            //备用套电价文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取备用套电价文件....");
            Common.Memset(ref strRevCode, "DF010004000000C7");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件读取值2", strRedData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty(strRedData[i]) || string.IsNullOrEmpty(strpriceFile2Arr[i])) continue;
                if (strRedData[i].Length == 398 && strpriceFile2Arr[i].Length == 398)
                {
                    string strRedDataTmp = strRedData[i].Substring(0, strRedData[i].Length - 4);
                    string strpriceFile2Tmp = strpriceFile2Arr[i].Substring(0, strRedData[i].Length - 4);
                    if (strRedDataTmp[i] == strpriceFile2Tmp[i])
                    {
                        blnRet[i, 4] = true;
                    }
                }
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次清零发生时刻,请稍候....");
            string[] strMeterQLJL = MeterProtocolAdapter.Instance.ReadData("03300101", 10);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次清零发生时刻2", strMeterQLJL);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电金额,请稍候....");
            string[] strMeterGDJE = MeterProtocolAdapter.Instance.ReadData("03330301", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电金额2", strMeterGDJE);
            

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次购电后剩余金额,请稍候....");
            string[] strMeterGDHJE = MeterProtocolAdapter.Instance.ReadData("03330501", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次购电后剩余金额2", strMeterGDHJE);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次编程事件记录,请稍候....");
            string[] strMeterBCSJJL = MeterProtocolAdapter.Instance.ReadData("03300001", 10);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次编程记录2", strMeterBCSJJL);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strMeterQLJL[i]))
                {
                    string strDateTmp = strMeterQLJL[i].Substring(strMeterQLJL[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strJlDateTime), DateTimes.FormatStringToDateTime(strDateTmp));

                    if (iErr < 300
                        && strMeterGDJE[i] == "00020000"
                        && strMeterGDHJE[i] == "00020000"
                        && strMeterBCSJJL[i].Equals("00000000000000000000"))
                    {
                        blnRet[i, 5] = true;
                    }
                }
                if (blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5])
                {
                    ResultDictionary["预置卡参数更新2"][i] = "通过";
                }
                else
                {
                    ResultDictionary["预置卡参数更新2"][i] = "不通过";
                }
            }
            UploadTestResult("预置卡参数更新2");

            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3]
                    && blnRet[i, 4] && blnRet[i,5])
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
    }
}
