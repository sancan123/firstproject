using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 公钥下一类参数更新
    /// </summary>
    public class PublicParam1Update : VerifyBase
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

        public PublicParam1Update(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥", "当前套第1阶梯表设置值", "当前套第1阶梯表读取值", "记录编程事件记录", "测试密钥下更新一类参数", "结论" };
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
            string[] strSetData = new string[BwCount];
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
            bool[,] blnRet = new bool[BwCount, 6];


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
                        blnRet[i, 0] = true;
                    }
                }
                else
                {
                    MyStatus[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前密钥", MyStatus);


            //设置报警金额1限值
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套第一张阶梯表,请稍候....");
            string strJlDateTime = DateTime.Now.ToString("yyMMddHHmmss");
            Common.Memset(ref strID, "04060AFF");
            Common.Memset(ref strSetData, "00001100000022000000330000004400000055000000660000110000000200000003000000040000000500000006000000070000010101020202030303040404050505060606");

            Common.Memset(ref strData, "00001100" + "00002200" + "00003300" + "00004400" + "00005500" + "00006600" +
                                       "00110000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000" +
                                       "010101" + "020202" + "030303" + "040404" + "050505" + "060606");
            Common.Memset(ref strPutApdu, "04D684344A");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag,strPutApdu, strRand2, strID, strData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套第1阶梯表设置值", strSetData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D684C009");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取当前套第1阶梯表,请稍候....");
            Common.Memset(ref strRevCode, "DF01000400340046");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRevData, out strRevMac);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套第1阶梯表读取值", strRevData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSetData[i] == strRevData[i]) blnRet[i, 1] = true;
            }


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次编程事件记录,请稍候....");
            string[] strReadInfo = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "记录编程事件记录", strReadInfo);
            //
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    continue;
                ResultDictionary["测试密钥下更新一类参数"][i] = "不通过";
                if (!string.IsNullOrEmpty(strReadInfo[i]))
                {
                    string strDateTmp = strReadInfo[i].Substring(strReadInfo[i].Length - 12, 12);
                    if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strJlDateTime), DateTimes.FormatStringToDateTime(strDateTmp));
                    string strJlData = strReadInfo[i].Substring(0, 80);
                    if (strJlData.Equals("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF0400010C04060AFF04000109")
                        && iErr < 300)
                    {
                        blnRet[i, 2] = true;

                    }
                }
                ResultDictionary["测试密钥下更新一类参数"][i] = blnRet[i, 2] ? "通过" : "不通过";
            }
            UploadTestResult("测试密钥下更新一类参数");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
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
