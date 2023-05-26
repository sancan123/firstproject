using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 本地模式下正确参数密钥下装测试
    /// </summary>
    public class RightParameterUpdateKey : VerifyBase
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

        public RightParameterUpdateKey(object plan)
            : base(plan)
        {
        }
        //密钥状态字（更新前）|密钥状态字（更新后）|密钥更新总次数（更新前）|密钥更新总次数（更新后）|上1次密钥更新记录|正确参数密钥更新

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "密钥状态字（更新前）","密钥状态字（更新后）","密钥更新总次数（更新前）","密钥更新总次数（更新后）",
                                         "上1次密钥更新记录","正确参数密钥更新",
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
            string[] strData = new string[BwCount];
            string[] strCode = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int[] iFlag = new int[BwCount];
            bool[,] blnRet = new bool[BwCount, 1];
            bool[] result = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] FkStatus = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strRevCode = new string[BwCount];

            #region

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            #endregion


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            string[] SumNoQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新前）", SumNoQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            string[] status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新前）", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            string UpdateKeyDate = DateTime.Now.ToString("yyMMddHHmmss");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新后总次数");
            string[] SumNoH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新后）", SumNoH);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新后）", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次密钥更新记录");
            string[] strRedData = MeterProtocolAdapter.Instance.ReadData("03301201", 15);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次密钥更新记录", strRedData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn && string.IsNullOrEmpty(strRedData[i])) continue;
                if (strRedData[i].Length < 30 || string.IsNullOrEmpty(SumNoH[i]) || string.IsNullOrEmpty(SumNoQ[i])) continue;
                string datetmp = strRedData[i].ToString().Substring(18, 12);
                if (datetmp != "000000000000")
                {
                    int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(UpdateKeyDate), DateTimes.FormatStringToDateTime(datetmp));

                    if (iDateErr < 360 && (Convert.ToInt32(SumNoQ[i]) + 1) == Convert.ToInt32(SumNoH[i]))
                    {
                        blnRet[i, 0] = true;
                    }
                    
                }
                ResultDictionary["正确参数密钥更新"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("正确参数密钥更新");



            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (blnRet[i, 0] && status[i]=="0001FFFF")
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
