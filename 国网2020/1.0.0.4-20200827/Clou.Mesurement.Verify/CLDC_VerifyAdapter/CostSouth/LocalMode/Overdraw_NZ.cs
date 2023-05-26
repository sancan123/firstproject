using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 透支功能
    /// </summary>
    public class Overdraw_NZ : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "保电状态1","报警金额1","报警金额2","透支金额1","剩余金额1","跳闸次数前一后1","保电状态不跳闸1",
                                         "保电状态2","透支金额限值2","透支金额2","剩余金额2","跳闸次数前一后2","保电解除可跳闸2",
                                         "透支金额限值3","剩余金额3","合闸允许金额限值3","合闸次数前一后3","剩余金额少于限值不可合闸3",
                                         "透支金额限值4","剩余金额4","合闸允许金额限值4","合闸次数前一后4","剩余金额超限值可合闸4",
                                         "透支金额限值5","透支金额5","跳闸次数前一后5","透支金额跳闸5",
                                         "合闸次数前一后6","透支金额6","合闸复电6",
                                         "透支金额限值7","透支金额7","跳闸次数前一后7","开关故障无法拉闸时操作7",
                                         "合闸金额限值8","剩余金额8","合闸次数前一后8","剩余金额小于限值不可合闸8",
                                         "合闸金额限值9","剩余金额9","合闸次数前一后9","剩余金额超限值可合闸9",
                                         "结论" };
            return true;
        }

        public Overdraw_NZ(object plan)
            : base(plan)
        {
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
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 10];
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
            string[] strBj1 = new string[BwCount];
            string[] strBj2 = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] str_ID = new string[BwCount];
            string[] str_Data = new string[BwCount];
            string[] str_Apdu = new string[BwCount];
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
            MessageController.Instance.AddMessage("正在设置时效为30分钟,请稍候....");
            Common.Memset(ref str_ID, "070001FF");
            Common.Memset(ref str_Data, "0030");
            Common.Memset(ref str_Apdu, "04D6812B06");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "0000000A");
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
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行远程开户,请稍候....");
            Common.Memset(ref strData, "00000000" + "00000000" + "112233445566");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);


            //设置备用套费率
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套费率,请稍候....");
            Common.Memset(ref strID, "040502FF");
            Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000");
            Common.Memset(ref strPutApdu, "04D6840414");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
            Common.Memset(ref strID, "04000108");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D6810A09");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置备用套阶梯时区表,请稍候....");
            Common.Memset(ref strID, "04070001");
            Common.Memset(ref strData, "04070001" + "010101" + "050102" + "110102" + "110102");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯时区切换时间,请稍候....");
            Common.Memset(ref strID, "0400010A");
            Common.Memset(ref strData, "0400010A" + DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
            Common.Memset(ref strID, "04060AFF");
            Common.Memset(ref strData, "00000010" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                       + "00010000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                       + "999999" + "999999" + "999999" + "999999" + "999999" + "999999");
            Common.Memset(ref strPutApdu, "04D684344A");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D684C009");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, "000005");
            Common.Memset(ref strPutApdu, "04D6811807");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strID, "04000307");
            Common.Memset(ref strData, ("000002"));
            Common.Memset(ref strPutApdu, "04D6811B07");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

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
            MessageController.Instance.AddMessage("正在设置时间到1月2日....");
            Common.Memset(ref strRevCode, "04000101");
            string strDatetime = DateTime.Now.ToString("yy") + "-01-02";
            DateTime dt = DateTime.Parse(strDatetime, DateTimeFormatInfo.CurrentInfo);
            Common.Memset(ref strData, "04000101" + DateTime.Now.ToString("yy") + "0102" + "0" + ((int)dt.DayOfWeek).ToString());
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion


            #region 步骤1

            if (Stop) return;
            Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数1....");
            int[] iLaZhaCount1Q = ReadMeterLaZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
            Common.Memset(ref strData, "3A00" + System.DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字3分钟,请稍候....");
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);


            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 3);


            if (Stop) return;
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数1....");
            int[] iLaZhaCount1H = ReadMeterLaZhaCount();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额1", Common.StringConverToDecima(strSyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取报警金额1,请稍候....");
            string[] strBjMoney1 = MeterProtocolAdapter.Instance.ReadData("04001001", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额1", Common.StringConverToDecima(strBjMoney1));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取报警金额2,请稍候....");
            string[] strBjMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2", Common.StringConverToDecima(strBjMoney2));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额....");
            string[] strTzMoney1 = MeterProtocolAdapter.Instance.ReadData("00900201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额1", Common.StringConverToDecima(strTzMoney1));


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                    {
                        ResultDictionary["保电状态1"][i] = "保电";
                    }
                    else
                    {
                        ResultDictionary["保电状态1"][i] = "非保电";
                    }

                    if (iLaZhaCount1Q[i] == iLaZhaCount1H[i])
                    {
                        blnRet[i, 0] = true;
                    }
                }
                else
                {
                    ResultDictionary["保电状态1"][i] = "异常";
                }
                ResultDictionary["保电状态不跳闸1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                ResultDictionary["跳闸次数前一后1"][i] = iLaZhaCount1Q[i].ToString() + "-" + iLaZhaCount1H[i].ToString();
            }


            UploadTestResult("跳闸次数前一后1");
            UploadTestResult("保电状态1");
            UploadTestResult("保电状态不跳闸1");

            #endregion

            #region 步骤2

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数2....");
            int[] iLaZhaCount2Q = ReadMeterLaZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            strTzMoney1 = Common.StringConverToDecima(strTzMoney1);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strTzMoney1[i]))
                {
                    string strMoneyTmp = "00000000";
                    if (strTzMoney1[i].Contains("."))
                    {
                        strMoneyTmp = strTzMoney1[i].Split('.')[1];
                        if (strMoneyTmp.Length == 2)
                        {
                            strTzMoney1[i] = strTzMoney1[i].Replace(".", "");
                        }
                        else if (strMoneyTmp.Length == 1)
                        {
                            strTzMoney1[i] = strTzMoney1[i].Replace(".", "") + "0";
                        }
                        int iMoneyTmp = Convert.ToInt32(strTzMoney1[i]) + 10;
                        strMoneyTmp = (iMoneyTmp).ToString("X2").PadLeft(8, '0');
                    }
                    else
                    {
                        strMoneyTmp = (Convert.ToInt32(strTzMoney1[i]) * 100 + 10).ToString("X2").PadLeft(8, '0');
                    }
                    BuyMoney[i] = strMoneyTmp;
                }
            }


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行充值,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //BuyMoney[i] = "000003E8";
                strGdCount[i] = "00000001";
                strKhID[i] = "112233445566";
                strData[i] = BuyMoney[i] + strGdCount[i] + strKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字3分钟,请稍候....");
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);


            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 3);


            if (Stop) return;
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额2", Common.StringConverToDecima(strSyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001003", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额限值2", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额2", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数2....");
            int[] iLaZhaCount2H = ReadMeterLaZhaCount();

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
                        ResultDictionary["保电状态2"][i] = "保电";
                    }
                    else
                    {
                        ResultDictionary["保电状态2"][i] = "非保电";
                    }

                    if (Convert.ToInt32(iLaZhaCount2H[i]) > Convert.ToInt32(iLaZhaCount2Q[i]))
                    {
                        blnRet[i, 1] = true;
                    }
                }
                else
                {
                    ResultDictionary["保电状态2"][i] = "异常";
                }
                ResultDictionary["保电解除可跳闸2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                ResultDictionary["跳闸次数前一后2"][i] = iLaZhaCount2Q[i].ToString() + "-" + iLaZhaCount2H[i].ToString();
            }
            UploadTestResult("保电状态2");
            UploadTestResult("跳闸次数前一后2");
            UploadTestResult("保电解除可跳闸2");
            #endregion

            #region 步骤3

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            int[] iHeZhaCount3Q = ReadMeterHeZhaCount();


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值100元,请稍候....");
            Common.Memset(ref strRevCode, "04001005");
            Common.Memset(ref strData, "04001005" + "00010000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行充值30元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                BuyMoney[i] = "00000BB8";
                strGdCount[i] = "00000002";
                strData[i] = BuyMoney[i] + strGdCount[i] + strKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额3", Common.StringConverToDecima(strSyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸允许金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001005", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "合闸允许金额限值3", Common.StringConverToDecima(strData));

            if (Stop) return;
            int[] iHeZhaCount3H = ReadMeterHeZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001003", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额限值3", Common.StringConverToDecima(strData));

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()))
                {
                    if (iHeZhaCount3Q[i] == iHeZhaCount3H[i])
                    {
                        blnRet[i, 2] = true;
                    }
                }
                ResultDictionary["剩余金额少于限值不可合闸3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                ResultDictionary["合闸次数前一后3"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
            }

            UploadTestResult("合闸次数前一后3");
            UploadTestResult("剩余金额少于限值不可合闸3");
            #endregion

            #region 步骤4

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            int[] iHeZhaCount4Q = ReadMeterHeZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行充值110元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                BuyMoney[i] = "00002AF8";
                strGdCount[i] = "00000003";
                strData[i] = BuyMoney[i] + strGdCount[i] + strKhID[i];
            }
            string strHzDate = DateTime.Now.ToString("yyMMddHHmmss");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额4", Common.StringConverToDecima(strSyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸允许金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001005", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "合闸允许金额限值4", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001003", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额限值4", Common.StringConverToDecima(strData));

            if (Stop) return;
            int[] iHeZhaCount4H = ReadMeterHeZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(iHeZhaCount4Q[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount4H[i].ToString()))
                {
                    if (Convert.ToInt32(iHeZhaCount4H[i]) > Convert.ToInt32(iHeZhaCount4Q[i]))
                    {
                        blnRet[i, 3] = true;
                    }
                }

                ResultDictionary["剩余金额超限值可合闸4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                ResultDictionary["合闸次数前一后4"][i] = iHeZhaCount4Q[i].ToString() + "-" + iHeZhaCount4H[i].ToString();
            }

            UploadTestResult("合闸次数前一后4");
            UploadTestResult("剩余金额超限值可合闸4");

            #endregion

            #region 步骤5

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "0000000A");
            blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置透支金额限值5,请稍候....");
            Common.Memset(ref strRevCode, "04001003");
            Common.Memset(ref strData, "04001003" + "00000500");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数5....");
            int[] iLaZhaCount5Q = ReadMeterLaZhaCount();

            if (Stop) return;
            Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);

            if (Stop) return;
            Common.Memset(ref strData, "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);




            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字3分钟,请稍候....");
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);


            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 3);


            if (Stop) return;
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额5", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001003", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额限值5", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数5....");
            if (Stop) return;
            int[] iLaZhaCount5H = ReadMeterLaZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;


                if (!string.IsNullOrEmpty(iLaZhaCount5Q[i].ToString()) && !string.IsNullOrEmpty(iLaZhaCount5H[i].ToString()))
                {
                    if (Convert.ToInt32(iLaZhaCount5H[i]) > Convert.ToInt32(iLaZhaCount5Q[i]))
                    {
                        blnRet[i, 4] = true;
                    }
                }
                ResultDictionary["透支金额跳闸5"][i] = blnRet[i, 4] ? "通过" : "不通过";
                ResultDictionary["跳闸次数前一后5"][i] = iLaZhaCount5Q[i].ToString() + "-" + iLaZhaCount5H[i].ToString();
            }
            UploadTestResult("跳闸次数前一后5");
            UploadTestResult("透支金额跳闸5");

            #endregion

            #region 步骤6


            if (Stop) return;
            int[] iHeZhaCount6Q = ReadMeterHeZhaCount();

            MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额6", Common.StringConverToDecima(strData));

            if (Stop) return;
            int[] iHeZhaCount6H = ReadMeterHeZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (!string.IsNullOrEmpty(iHeZhaCount6Q[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount6H[i].ToString()))
                {
                    if (Convert.ToInt32(iHeZhaCount6H[i]) > Convert.ToInt32(iHeZhaCount6Q[i]))
                    {
                        blnRet[i, 5] = true;
                    }
                }
                ResultDictionary["合闸复电6"][i] = blnRet[i, 5] ? "通过" : "不通过";
                ResultDictionary["合闸次数前一后6"][i] = iHeZhaCount6Q[i].ToString() + "-" + iHeZhaCount6H[i].ToString();
            }
            UploadTestResult("合闸次数前一后6");
            UploadTestResult("合闸复电6");

            #endregion

            #region 步骤7

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, "000100");
            Common.Memset(ref strPutApdu, "04D6811807");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数7....");
            int[] iLaZhaCount7Q = ReadMeterLaZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字3分钟,请稍候....");
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);

            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 3);


            if (Stop) return;
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900201", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额7", Common.StringConverToDecima(strData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取透支金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001003", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "透支金额限值7", Common.StringConverToDecima(strData));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数7....");
            int[] iLaZhaCount7H = ReadMeterLaZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (!string.IsNullOrEmpty(iLaZhaCount7Q[i].ToString()) && !string.IsNullOrEmpty(iLaZhaCount7H[i].ToString()))
                {
                    if (Convert.ToInt32(iLaZhaCount7H[i]) > Convert.ToInt32(iLaZhaCount7Q[i]))
                    {
                        blnRet[i, 6] = true;
                    }
                }

                ResultDictionary["开关故障无法拉闸时操作7"][i] = blnRet[i, 6] ? "通过" : "不通过";
                ResultDictionary["跳闸次数前一后7"][i] = iLaZhaCount7Q[i].ToString() + "-" + iLaZhaCount7H[i].ToString();
            }
            UploadTestResult("跳闸次数前一后7");
            UploadTestResult("开关故障无法拉闸时操作7");

            #endregion

            #region 步骤8

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            int[] iHeZhaCount9Q = ReadMeterHeZhaCount();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值=100,请稍候....");
            Common.Memset(ref strRevCode, "04001005");
            Common.Memset(ref  strData, "04001005" + "00010000");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行远程开户并充值60,请稍候....");
            Common.Memset(ref strData, "00001770" + "00000001" + "112233445566");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额8", Common.StringConverToDecima(strData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001005", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "合闸金额限值8", Common.StringConverToDecima(strData));

            if (Stop) return;
            int[] iHeZhaCount9H = ReadMeterHeZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (!string.IsNullOrEmpty(iHeZhaCount9Q[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount9H[i].ToString()))
                {
                    if (iHeZhaCount9H[i] == iHeZhaCount9Q[i])
                    {
                        blnRet[i, 7] = true;
                    }
                }

                ResultDictionary["剩余金额小于限值不可合闸8"][i] = blnRet[i, 7] ? "通过" : "不通过";
                ResultDictionary["合闸次数前一后8"][i] = iHeZhaCount9Q[i].ToString() + "-" + iHeZhaCount9H[i].ToString();
            }

            UploadTestResult("剩余金额小于限值不可合闸8");
            UploadTestResult("合闸次数前一后8");

            #endregion

            #region 步骤9

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            int[] iHeZhaCount10Q = ReadMeterHeZhaCount();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行充值100元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                BuyMoney[i] = "00002710";
                strGdCount[i] = "00000002";
                strData[i] = BuyMoney[i] + strGdCount[i] + strKhID[i];
            }
            strHzDate = DateTime.Now.ToString("yyMMddHHmmss");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额....");
            strData = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额9", Common.StringConverToDecima(strData));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸金额限值....");
            strData = MeterProtocolAdapter.Instance.ReadData("04001005", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "合闸金额限值9", Common.StringConverToDecima(strData));

            if (Stop) return;
            int[] iHeZhaCount10H = ReadMeterHeZhaCount();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;


                if (!string.IsNullOrEmpty(iHeZhaCount10Q[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount10H[i].ToString()))
                {
                    if (Convert.ToInt32(iHeZhaCount10H[i]) > Convert.ToInt32(iHeZhaCount10Q[i]))
                    {
                        blnRet[i, 8] = true;
                    }
                }
                ResultDictionary["剩余金额超限值可合闸9"][i] = blnRet[i, 8] ? "通过" : "不通过";
                ResultDictionary["合闸次数前一后9"][i] = iHeZhaCount10Q[i].ToString() + "-" + iHeZhaCount10H[i].ToString();
            }

            UploadTestResult("剩余金额超限值可合闸9");
            UploadTestResult("合闸次数前一后9");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, "000001");
            Common.Memset(ref strPutApdu, "04D6811807");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strID, "04000307");
            Common.Memset(ref strData, ("000001"));
            Common.Memset(ref strPutApdu, "04D6811B07");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            #endregion

            #region 处理结果

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4]
                    && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8])
                {
                    ResultDictionary["结论"][i] = "合格";
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                }
            }
            UploadTestResult("结论");
            #endregion


        }

        /// <summary>
        /// 读取拉闸次数
        /// </summary>
        /// <returns></returns>
        private int[] ReadMeterLaZhaCount()
        {
            //if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数....");
            string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1D000001", 3);
            int[] iLaZhaCount = new int[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strlaZhaCount[i])) continue;
                string strTmp = strlaZhaCount[i];
                if (strTmp != "000000")
                {
                    strTmp = strTmp.TrimStart('0');
                    iLaZhaCount[i] = Convert.ToInt32(strTmp);
                }
                else
                {
                    iLaZhaCount[i] = 0;
                }
            }
            return iLaZhaCount;
        }


        /// <summary>
        /// 读取合闸次数
        /// </summary>
        /// <returns></returns>
        private int[] ReadMeterHeZhaCount()
        {
            //if (Stop) return;
            MessageController.Instance.AddMessage("正在读取合闸次数....");
            string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1E000001", 3);
            int[] iLaZhaCount = new int[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strlaZhaCount[i])) continue;
                string strTmp = strlaZhaCount[i];
                if (strTmp != "000000")
                {
                    strTmp = strTmp.TrimStart('0');
                    iLaZhaCount[i] = Convert.ToInt32(strTmp);
                }
                else
                {
                    iLaZhaCount[i] = 0;
                }
            }
            return iLaZhaCount;
        }



    }

}
