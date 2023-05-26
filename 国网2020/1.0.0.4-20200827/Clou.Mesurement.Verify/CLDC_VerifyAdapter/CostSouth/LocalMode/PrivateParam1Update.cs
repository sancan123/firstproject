using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 私钥下一类参数更新
    /// </summary>
    public class PrivateParam1Update : VerifyBase
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

        public PrivateParam1Update(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥", 
                                        "报警金额1限值设置值","报警金额1限值读取值","报警金额2限值设置值","报警金额2限值读取值",
                                         "电流互感器变比设置值","电流互感器变比读取值","电压互感器变比设置值","电压互感器变比读取值",
                                        "备用套费率设置值","备用套费率读取值",
                                        "记录编程事件记录","正式密钥下更新一类参数","结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 7];


            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            #endregion

            //1--------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            bool[] blnUpKey = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
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
                        blnRet[i, 0] = true;
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
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥", MyStatus);

            //身份认证
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            //设置报警金额1限值
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额1限值5元,请稍候...."); 
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, "00000500");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额1限值设置值", strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取报警金额1限值,请稍候....");
            Common.Memset(ref strRevCode, "DF01000100100004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额1限值读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strData[i] == strRevData[i]) blnRet[i, 1] = true;
            }

            //设置报警金额2限值
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2限值1元,请稍候....");
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, "00000100");
            Common.Memset(ref strPutApdu, "04D6811408");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2限值设置值", strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取报警金额2限值,请稍候....");
            Common.Memset(ref strRevCode, "DF01000100140004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2限值读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strData[i] == strRevData[i]) blnRet[i, 2] = true;
            }


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流互感器变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, "000015");
            Common.Memset(ref strPutApdu, "04D6811807");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电流互感器变比设置值", strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取电流互感器,请稍候....");
            Common.Memset(ref strRevCode, "DF01000100180003");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电流互感器变比读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strData[i] == strRevData[i]) blnRet[i,3] = true;
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strID, "04000307");
            Common.Memset(ref strData, ("000020"));
            Common.Memset(ref strPutApdu, "04D6811B07");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压互感器变比设置值", strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取电压互感器,请稍候....");
            Common.Memset(ref strRevCode, "DF010001001B0003");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压互感器变比读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strData[i] == strRevData[i]) blnRet[i, 4] = true;
            }


            //设置备用套费率
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套费率,请稍候....");
            Common.Memset(ref strID, "040502FF");
            Common.Memset(ref strData, "00006000" + "00005000" + "00004000" + "00003000");
            Common.Memset(ref strPutApdu, "04D6840414");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套费率设置值", strData);
            string strJlDateTime = DateTime.Now.ToString("yyMMddHHmmss");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取备用套费率,请稍候....");
            Common.Memset(ref strRevCode, "DF01000400040010");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套费率读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strRevData[i]) || strRevData[i].Length < 32) continue;
                if (strData[i] == strRevData[i].Substring(0, 32)) blnRet[i, 5] = true;
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次编程事件记录,请稍候....");
            string [] strReadInfo = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "记录编程事件记录", strReadInfo);
            //
            for (int i = 0; i < BwCount ; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn )
                    continue;
                ResultDictionary["正式密钥下更新一类参数"][i] = "不通过";
                if (!string.IsNullOrEmpty(strReadInfo[i]))
                {
                    string strDateTmp = strReadInfo[i].Substring(strReadInfo[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strJlDateTime), DateTimes.FormatStringToDateTime(strDateTmp));
                    string strJlData = strReadInfo[i].Substring(0, 80);
                    if (strJlData.Equals("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF04001001040010020400030604000307040502FF")
                        && iErr < 300 && blnUpKey[i])
                    {
                        blnRet[i, 6] = true;

                    }
                }
                ResultDictionary["正式密钥下更新一类参数"][i] = blnRet[i, 6] ? "通过":"不通过";
            }
            UploadTestResult("正式密钥下更新一类参数");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] && blnRet[i, 6])
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
    }
}
