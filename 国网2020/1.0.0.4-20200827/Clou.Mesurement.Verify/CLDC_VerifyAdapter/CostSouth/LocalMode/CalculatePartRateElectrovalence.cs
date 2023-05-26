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
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 分时费率电价结算
    /// </summary>
    public class CalculatePartRateElectrovalence : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //理论费率1电能增量|理论费率2电能增量|理论费率3电能增量|理论费率4电能增量|实际费率1电能增量|
        //实际费率2电能增量|实际费率3电能增量|实际费率4电能增量|理论电能总增量|实际电能总增量|
        //剩余电能量递减准确度|起始剩余金额|结束理论剩余金额|结束实际剩余金额|理论金额差值|实际金额差值|剩余金额递减准确度

        //P+费率1起码|P+费率1止码|P+费率2起码|P+费率2止码|P+费率3起码|P+费率3止码|P+费率4起码|P+费率4止码
        //P+费率1增量|P+费率2增量|P+费率3增量|P+费率4增量
        //剩余金额起始值
        //剩余金额实际止值
        //剩余金额标准止值
        //电费结算实际值
        //电费结算标准值
        //电费结算

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "设置当前套和备用套阶梯值和阶梯电价","设置当前套费率电价","设置当前套日时段表","设置第1套时区表","设置ＣＴ变比","设置ＰＴ变比","设置为6月23日",
                                         "P＋费率1起码", "P＋费率1止码", "P＋费率2起码", "P＋费率2止码",
                                         "P＋费率3起码","P＋费率3止码","P＋费率4起码","P＋费率4止码",
                                         "P＋费率1增量","P＋费率2增量","P＋费率3增量","P＋费率4增量",
                                         "剩余金额起始值","剩余金额实际止值","剩余金额标准止值","电费结算实际值","电费结算标准值","电费结算",
                                         "结论" };
            return true;
        }
        string strPlan = "";
        public CalculatePartRateElectrovalence(object plan)
            : base(plan)
        {
            strPlan = plan.ToString();
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
            string[] strReadStartMoneyQ = new string[BwCount];//起始剩余金额
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
            bool[,] blnRet = new bool[BwCount, 12];
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
            string[] strApdu = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strErrInfo = new string[BwCount];

            bool[] bSetTjtdj1 = new bool[BwCount]; //设置当前套和备用套阶梯值和阶梯电价(第1张)
            bool[] bSetTjtdj2 = new bool[BwCount]; //设置当前套和备用套阶梯值和阶梯电价(第2张)
            bool[] bChangSetTjtdj = new bool[BwCount];

            bool[] bSetFldj = new bool[BwCount];//设置当前套费率电价
            bool[] bChangSetFldj = new bool[BwCount];

            bool[] bSetRsdb1 = new bool[BwCount];//设置当前套日时段表(第1日)
            bool[] bSetRsdb2 = new bool[BwCount];//设置当前套日时段表(第2日)
            bool[] bChangSetRsdb = new bool[BwCount];

            bool[] bSetSqb = new bool[BwCount];//设置第1套时区表
            bool[] bSetCT = new bool[BwCount];//设置CT
            bool[] bSetPT = new bool[BwCount];//设置PT
            bool[] bSetTime = new bool[BwCount];//设置为1月2日

            try
            {

                #region 方案配置参数

                //strPlan = 钱包初始化金额|电压变比|电流变比|走字电流|走字时间|误差值
                string[] ArrPlan = strPlan.Split('|');
                string strMoneyTmp = ArrPlan[0];
                string strVoltageScale = ArrPlan[1].PadLeft(6, '0');
                string strCurrentScale = ArrPlan[2].PadLeft(6, '0');
                float Current = 0;
                int ZzTime = (int)(float.Parse(ArrPlan[4]) * 60);
                float ErrMoney = float.Parse(ArrPlan[5]);

                string strMoney = "";
                if (strMoneyTmp.Contains("."))
                {
                    strMoney = strMoney.Split('.')[1];
                    if (strMoney.Length == 2)
                    {
                        strMoneyTmp = strMoneyTmp.Replace(".", "");
                    }
                    else if (strMoney.Length == 1)
                    {
                        strMoneyTmp = strMoneyTmp.Replace(".", "") + "0";
                    }
                    int iMoneyTmp = Convert.ToInt32(strMoneyTmp);
                    strMoney = (iMoneyTmp).ToString("X2").PadLeft(8, '0');
                }
                else
                {
                    strMoney = (Convert.ToInt32(strMoneyTmp) * 100).ToString("X2").PadLeft(8, '0');
                }

                if (ArrPlan[3] == "Imax")
                {
                    Current = GlobalUnit.Imax;
                }
                else if (ArrPlan[3] == "0.5Imax")
                {
                    Current = GlobalUnit.Imax / 2;
                }
                else
                {
                    Current = int.Parse(ArrPlan[3].Replace("Ib", "")) * GlobalUnit.Ib;
                }



                #endregion

                #region 准备步骤
                //准备工作
                ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
                Common.Memset(ref strRevCode, "04001003");
                Common.Memset(ref strData, "04001003" + "00000000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                //Common.Memset(ref strData, "000186A0");
                Common.Memset(ref strData, strMoney);
                bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

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
                Common.Memset(ref strData, "00006000" + "00005000" + "00004000" + "00003000");
                Common.Memset(ref strPutApdu, "04D6840414");
                bSetFldj = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
                Common.Memset(ref strID, "04000108");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D6810A09");
                bChangSetFldj = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套第1张阶梯值和阶梯电价全为0,请稍候....");
                Common.Memset(ref strID, "04060AFF");
                Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "000000" + "000000" + "000000" + "000000" + "000000" + "000000");
                Common.Memset(ref strPutApdu, "04D684344A");
                bSetTjtdj1 = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套第2张阶梯值和阶梯电价全为0,请稍候....");
                Common.Memset(ref strID, "04060BFF");
                Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "000000" + "000000" + "000000" + "000000" + "000000" + "000000");
                Common.Memset(ref strPutApdu, "04D6847A4A");
                bSetTjtdj2 = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
                //Common.Memset(ref bSetTjtdj2, true);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
                Common.Memset(ref strID, "04000109");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D684C009");
                bChangSetTjtdj = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
                Common.Memset(ref strID, "04000306");
                //Common.Memset(ref strData, "000003");
                Common.Memset(ref strData, strCurrentScale);
                Common.Memset(ref strPutApdu, "04D6811807");
                bSetCT = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
                Common.Memset(ref strID, "04000307");
                //Common.Memset(ref strData, ("000002"));
                Common.Memset(ref strData, strVoltageScale);
                Common.Memset(ref strPutApdu, "04D6811B07");
                bSetPT = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                //3--------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置第一套时区表,请稍候....");
                Common.Memset(ref strID, "04010000");
                Common.Memset(ref strRevData, "04010000" + "122801" + "062202");
                bSetSqb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strID);

                //4----------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置第一套第1日时段表,请稍候....");
                Common.Memset(ref strID, "04010001");
                Common.Memset(ref strRevData, "04010001" + "000004" + "060001" + "120002" + "180003");
                bSetRsdb1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strID);

                //5----------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置第一套第2日时段表,请稍候....");
                Common.Memset(ref strID, "04010002");
                Common.Memset(ref strRevData, "04010002" + "000001" + "080002" + "160003" + "180004");
                bSetRsdb2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置两套时区表切换时间,请稍候....");
                Common.Memset(ref strID, "04000106");
                Common.Memset(ref strRevData, "04000106" + "9999999999");
                bool[] bSetSqb2 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置两套日时段表切换时间,请稍候....");
                Common.Memset(ref strID, "04000107");
                Common.Memset(ref strRevData, "04000107" + "9999999999");
                bool[] bSetSqb3 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strRevData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间到6月23日....");
                Common.Memset(ref strRevCode, "04000101");

                string strDatetime = DateTime.Now.ToString("yy") + "-06-23";
                DateTime dt = DateTime.Parse(strDatetime, DateTimeFormatInfo.CurrentInfo);

                Common.Memset(ref strData, "04000101" + DateTime.Now.ToString("yy") + "0623" + "0" + ((int)dt.DayOfWeek).ToString());
                bSetTime = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
                System.Threading.Thread.Sleep(200);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    ResultDictionary["设置当前套和备用套阶梯值和阶梯电价"][i] = (bSetTjtdj1[i] && bSetTjtdj2[i] && bChangSetTjtdj[i]) ? "成功" : "失败";
                    ResultDictionary["设置当前套费率电价"][i] = (bSetFldj[i] && bChangSetFldj[i]) ? "成功" : "失败";
                    ResultDictionary["设置当前套日时段表"][i] = (bSetRsdb1[i] && bSetRsdb2[i]) ? "成功" : "失败";
                    ResultDictionary["设置第1套时区表"][i] = (bSetSqb[i] && bSetSqb2[i] && bSetSqb3[i]) ? "成功" : "失败";
                    ResultDictionary["设置ＣＴ变比"][i] = bSetCT[i] ? "成功" : "失败";
                    ResultDictionary["设置ＰＴ变比"][i] = bSetPT[i] ? "成功" : "失败";
                    ResultDictionary["设置为6月23日"][i] = bSetTime[i] ? "成功" : "失败";
                }
                UploadTestResult("设置当前套和备用套阶梯值和阶梯电价");
                UploadTestResult("设置当前套费率电价");
                UploadTestResult("设置当前套日时段表");
                UploadTestResult("设置第1套时区表");
                UploadTestResult("设置ＣＴ变比");
                UploadTestResult("设置ＰＴ变比");
                UploadTestResult("设置为6月23日");

                #endregion

                //读取起始剩余金额
                if (Stop) return;
                strReadStartMoneyQ = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间....");
                Common.Memset(ref strRevCode, "04000102");
                Common.Memset(ref strData, "04000102"+"010000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取起码1,请稍候....");
                float[] QiMaF1 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)1);       
                //P+费率1起码
                if (Stop) return;

                MessageController.Instance.AddMessage("正在走字,请稍候....");

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率1起码", GetConvertEnegy(QiMaF1));

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current, 1, 1, "1.0", true, false);
                int iTime = ZzTime;

                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * iTime);

                //降电流
                    if (Stop) return;
                PowerOn();
                if(Stop)
                MessageController.Instance.AddMessage("正在读取止码1,请稍候....");
                float[] ZiMaF1 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)1);

                
                //|P＋费率1止码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率1止码", GetConvertEnegy(ZiMaF1));


                //费率2
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间....");
                Common.Memset(ref strRevCode, "04000102");
                Common.Memset(ref strData, "04000102"+"100000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                
                //blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib*5, 1, 1, "1.0", true, false);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取起码2,请稍候....");
                float[] QiMaF2 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)2);
                //|P＋费率2起码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率2起码", GetConvertEnegy(QiMaF2));

                MessageController.Instance.AddMessage("正在走字,请稍候....");
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current, 1, 1, "1.0", true, false);

                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * iTime);
      

                //降电流
                    if (Stop) return;
                PowerOn();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取止码2,请稍候....");
                float[] ZiMaF2 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)2);
                //|P＋费率2止码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率2止码", GetConvertEnegy(ZiMaF2));


                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间....");
                Common.Memset(ref strRevCode, "04000102");
                Common.Memset(ref strData, "04000102" + "170000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取起码3,请稍候....");
                //费率3
                float[] QiMaF3 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)3);
                //|P＋费率3起码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率3起码", GetConvertEnegy(QiMaF3));

                MessageController.Instance.AddMessage("正在走字,请稍候....");

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current, 1, 1, "1.0", true, false);
                
                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * iTime);

                    if (Stop) return;
                PowerOn();
                //降电流
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取止码3,请稍候....");
                float[] ZiMaF3 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)3);
                //P＋费率3止码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率3止码", GetConvertEnegy(ZiMaF3));


                //费率4
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间....");
                Common.Memset(ref strRevCode, "04000102");
                Common.Memset(ref strData, "04000102" + "220000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取起码4,请稍候....");
                //
                float[] QiMaF4 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)4);
                // |P＋费率4起码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率4起码", GetConvertEnegy(QiMaF4));

                MessageController.Instance.AddMessage("正在走字,请稍候....");

                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, Current, 1, 1, "1.0", true, false);

                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * iTime);

                    if (Stop) return;
                //降电流
                PowerOn();

                ShowWaitMessage("降电流、正在等待{0}秒,请稍候....", 1000 * 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取止码4,请稍候....");
                float[] ZiMaF4 = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)4);

                //P＋费率4止码
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率4止码", GetConvertEnegy(ZiMaF4));

                if (Stop) return;
                MessageController.Instance.AddMessage("正在对时,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strID[i] = "0400010C";
                    strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strData[i] += DateTime.Now.ToString("HHmmss");
                }
                bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

                string[] strLlSyMoney = new string[BwCount];
                
                MessageController.Instance.AddMessage("正在读取剩余金额....");

                strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                MessageController.Instance.AddMessage("正在计算结果,请稍候....");
                
                string[] strFsZl1 = new string[BwCount];//分时增量1
                string[] strFsZl2 = new string[BwCount];//分时增量2
                string[] strFsZl3 = new string[BwCount];//分时增量3
                string[] strFsZl4 = new string[BwCount];//分时增量4

                string[] strZzl = new string[BwCount];//标准电能总增量
                string[] strFsZzl = new string[BwCount];//分时费率总增量

                string[] strSyDldjZqd = new string[BwCount]; //剩余电量准去度
                string[] strLyMoneyCz = new string[BwCount]; //理论金额差值
                string[] strSjMoneyCz = new string[BwCount]; //实际金额差值
                string[] strSyJinEZqd = new string[BwCount];//剩余金额递减准确度；
               
                float fMeterZzl = 0f;

                Double fLyJineChazhi = 0f;
                Double fSjjineChazhi = 0f;

                int SunBianb = int.Parse(strVoltageScale) * int.Parse(strCurrentScale);
                //float iErr = 0.01f * 0.60f * SunBianb;

                //计算分时费率实际总增量
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    //理论差值
                    fLyJineChazhi =((ZiMaF1[i] - QiMaF1[i]) * 0.6f + (ZiMaF2[i] - QiMaF2[i]) * 0.5f + (ZiMaF3[i] - QiMaF3[i]) * 0.4f + (ZiMaF4[i] - QiMaF4[i]) * 0.3f) * SunBianb;
                    strLyMoneyCz[i]  =  fLyJineChazhi.ToString("0.00");

                    strFsZl1[i] = (ZiMaF1[i] - QiMaF1[i]).ToString("F2");
                    strFsZl2[i] = (ZiMaF2[i] - QiMaF2[i]).ToString("F2");
                    strFsZl3[i] = (ZiMaF3[i] - QiMaF3[i]).ToString("F2");
                    strFsZl4[i] = (ZiMaF4[i] - QiMaF4[i]).ToString("F2");
                    //分时费率总增量
                     fMeterZzl= (ZiMaF1[i] - QiMaF1[i] + ZiMaF2[i] - QiMaF2[i] + ZiMaF3[i] - QiMaF3[i] + ZiMaF4[i] - QiMaF4[i]);
                     
                    strFsZzl[i] = fMeterZzl.ToString("F2");

                    strLlSyMoney[i] = (Convert.ToDouble(strMoneyTmp) - ((ZiMaF1[i] - QiMaF1[i]) * 0.6 + (ZiMaF2[i] - QiMaF2[i]) * 0.5 + (ZiMaF3[i] - QiMaF3[i]) * 0.4 + (ZiMaF4[i] - QiMaF4[i]) * 0.3) * SunBianb).ToString("0.00");
                    //实际剩余金额
                    strSyMoney[i] = string.IsNullOrEmpty(strSyMoney[i]) ? "0" : (Convert.ToSingle(strSyMoney[i]) / 100).ToString("0.00");
                    strReadStartMoneyQ[i] = string.IsNullOrEmpty(strReadStartMoneyQ[i]) ? "0" : (Convert.ToSingle(strReadStartMoneyQ[i]) / 100).ToString("0.00");
                    //实际金额差值
                    fSjjineChazhi = Convert.ToSingle(strReadStartMoneyQ[i]) - Convert.ToSingle(strSyMoney[i]);
                    strSjMoneyCz[i] = fSjjineChazhi.ToString("0.00");
                    fLyJineChazhi = Convert.ToDouble(fLyJineChazhi.ToString("0.00"));
                    fSjjineChazhi = Convert.ToDouble(fSjjineChazhi.ToString("0.00"));
                    if (Math.Abs( Convert.ToSingle(fLyJineChazhi - fSjjineChazhi)) <= ErrMoney && strReadStartMoneyQ[i] != strSyMoney[i])
                    {
                        strSyJinEZqd[i] = "通过";
                    }
                    else
                    {
                        strSyJinEZqd[i] = "不通过";
                    }
                }

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率1增量", strFsZl1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率2增量", strFsZl2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率3增量", strFsZl3);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "P＋费率4增量", strFsZl4);

                //起始剩余金额
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额起始值", strReadStartMoneyQ);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额标准止值", strLlSyMoney);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额实际止值", strSyMoney);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算标准值", strLyMoneyCz);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算实际值", strSjMoneyCz);

                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电费结算", strSyJinEZqd);
                //剩余金额递减准确度




                MessageController.Instance.AddMessage("正在处理结果");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;

                    if (!string.IsNullOrEmpty(strLlSyMoney[i]) && !string.IsNullOrEmpty(strSyMoney[i])
                        && strSyJinEZqd[i].Equals("通过") && strReadStartMoneyQ[i] != strSyMoney[i])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            catch (Exception )
            { }
            UploadTestResult("结论");

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