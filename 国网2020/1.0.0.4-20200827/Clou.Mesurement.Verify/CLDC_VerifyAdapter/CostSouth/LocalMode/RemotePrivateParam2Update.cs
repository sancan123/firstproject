using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 私钥下二类参数更新测试
    /// </summary>
    public class RemotePrivateParam2Update : VerifyBase
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

        public RemotePrivateParam2Update(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "年时区数设置值","年时区数读取值","日时段表数设置值","日时段表数读取值", 
                                         "日时段数设置值","日时段数读取值", "费率数设置值", "费率数读取值",
                                         "第一套时区表设置值","第一套时区表读取值", "第一套第1日时段表设置值", "第一套第1日时段表读取值",
                                         "第一套第2日时段表设置值","第一套第2日时段表读取值", "每月第1结算日设置值", "每月第1结算日读取值",
                                         "记录编程事件记录","结论"};
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();


            //身份认证
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            string[] PutData = new string[BwCount];
            string[] DataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 9];
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] result = new bool[BwCount];
            string[] strRedData = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strOutMac1 = new string[BwCount];
            string[] FkStatus = new string[BwCount];


            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                DataCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, DataCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);


            //1---------设置年时区数
            Common.Memset(ref DataCode, "04000201");
            Common.Memset(ref strData, "02");
            Common.Memset(ref PutData, "04000201" + "02");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "年时区数设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置年时区数,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取年时区数,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04000201", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "年时区数读取值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 0] = true;
                }
                
            }
            if (Stop) return;
            //2---------设置日时段表数
            Common.Memset(ref DataCode, "04000202");
            Common.Memset(ref strData, "02");
            Common.Memset(ref PutData, "04000202" + "02");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段表数设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置日时段表数,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取日时段表数,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04000202", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段表数读取值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 1] = true;
                }
            }
            if (Stop) return;
            //3---------设置日时段数
            Common.Memset(ref DataCode, "04000203");
            Common.Memset(ref strData, "08");
            Common.Memset(ref PutData, "04000203" + "08");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置日时段数,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取日时段数,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数读取值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 2] = true;
                }
            }
            if (Stop) return;
            //4---------费率数
            Common.Memset(ref DataCode, "04000204");
            Common.Memset(ref strData, "04");
            Common.Memset(ref PutData, "04000204" + "04");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "费率数设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置费率数,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取费率数,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04000204", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "费率数读取值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 3] = true;
                }
            }

            //5---------设置第一套时区表
            Common.Memset(ref DataCode, "04010000");
            Common.Memset(ref strData, "122801" + "062202");
            Common.Memset(ref PutData, "04010000" + "122801" + "062202");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套时区表设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置第一套时区表,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取第一套时区表,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04010000", 6);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strRedData[i]) || strRedData[i].Length < 12) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 4] = true;
                }
                else if (strRedData[i].Substring(6, 6) + strRedData[i].Substring(0, 6) == strData[i])
                {
                    blnRet[i, 4] = true;
                    strRedData[i] = strRedData[i].Substring(6, 6) + strRedData[i].Substring(0, 6);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套时区表读取值", strRedData);

            if (Stop) return;
            //6---------设置第一套第1日时段表
            Common.Memset(ref DataCode, "04010001");
            Common.Memset(ref strData, "000001" + "080002" + "160003");
            Common.Memset(ref PutData, "04010001" + "000001" + "080002" + "160003");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套第1日时段表设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置第一套第1日时段表,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取第一套第1日时段表,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04010001", 9);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strRedData[i]) || strRedData[i].Length < 18) continue;
                string strTmp = strRedData[i].Substring(12, 6) + strRedData[i].Substring(6, 6) + strRedData[i].Substring(0, 6);
                if (strTmp == strData[i])
                {
                    blnRet[i, 5] = true;
                    strRedData[i] = strTmp;
                }
                else if (strData[i] == strRedData[i])
                {
                    blnRet[i, 5] = true;
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套第1日时段表读取值", strRedData);
            if (Stop) return;
            //7--------设置第一套第2日时段表
            Common.Memset(ref DataCode, "04010002");
            Common.Memset(ref strData, "000001" + "060002" + "120003" + "180004");
            Common.Memset(ref PutData, "04010002" + "000001" + "060002" + "120003" + "180004");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套第2日时段表设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置第一套第2日时段表,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取第一套第2日时段表,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04010002", 12);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strRedData[i]) || strRedData[i].Length < 24) continue;
                string strDataTmp = strRedData[i].Substring(18, 6) + strRedData[i].Substring(12, 6) + strRedData[i].Substring(6, 6) + strRedData[i].Substring(0, 6);
                if (strDataTmp == strData[i])
                {
                    blnRet[i, 6] = true;
                    strRedData[i] = strDataTmp;
                }
                else if (strRedData[i] == strData[i])
                {
                    blnRet[i, 6] = true;
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一套第2日时段表读取值", strRedData);

            if (Stop) return;
            //8---------每月第1结算日
            Common.Memset(ref DataCode, "04000B01");
            Common.Memset(ref PutData, "04000B01" + "0100");
            Common.Memset(ref strData, "0100");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "每月第1结算日设置值", strData);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置每月第1结算日,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, PutData, DataCode);
            string strJlDateTime = DateTime.Now.ToString("yyMMddHHmmss");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取每月第1结算日,请稍候....");
            strRedData = MeterProtocolAdapter.Instance.ReadData("04000B01", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "每月第1结算日读取值", strRedData);

            string[] strBcjlInfo = MeterProtocolAdapter.Instance.ReadData("03300001", 50);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "记录编程事件记录", strBcjlInfo);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strRedData[i] == strData[i])
                {
                    blnRet[i, 7] = true;
                }
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty( strBcjlInfo[i])) continue;
                string strDateTmp = strBcjlInfo[i].Substring(strBcjlInfo[i].Length - 12, 12);
                if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strJlDateTime), DateTimes.FormatStringToDateTime(strDateTmp));
                string strJlData = strBcjlInfo[i].Substring(0, 80);
                if (strJlData.Contains("0400020104000202040002030400020404010000040100010401000204000B01")
                    && iErr < 300)
                {
                    blnRet[i, 8] = true;

                }
            }


            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5]
                        && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8])
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
    }
}
