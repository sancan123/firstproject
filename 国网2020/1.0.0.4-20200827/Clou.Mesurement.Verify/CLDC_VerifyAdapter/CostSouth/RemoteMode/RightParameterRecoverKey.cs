using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程模式下正确参数密钥恢复测试
    /// </summary>
    public class RightParameterRecoverKey:VerifyBase
    {
        protected override string ItemKey
        {
           // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get{return null;}
        }

        public RightParameterRecoverKey(object plan)
            : base(plan)
        {
        }
        //密钥状态字（更新前）|密钥状态字（更新后）|密钥更新总次数（更新前）|密钥更新总次数（更新前）|上1次密钥更新记录|正确参数密钥恢复
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "密钥状态字（更新前）","密钥状态字（更新后）","密钥更新总次数（更新前）","密钥更新总次数（更新后）",
                                         "上1次密钥更新记录","正确参数密钥恢复",
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
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] result = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 1];
            int iSelectBwCount = 0;
            string[] strRevCode = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] status = new string[BwCount];

            #region
            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            //result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //Common.Memset(ref iFlag, 1);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            //result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn && !string.IsNullOrEmpty(status[i]))
                {
                    iSelectBwCount++;
                    if (!status[i].EndsWith("1FFFF"))
                    {
                        rstTmp[i] = true;
                    }
                }
            }

            if (Array.IndexOf(rstTmp, true) > -1)
            {
                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (Stop) return;
                            if (rstTmp[i])
                            {
                                MessageController.Instance.AddMessage("正在第" + (i + 1) + "表位密钥更新,请稍候....");
                                bool blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "01", 17, strRand2[i], strEsamNo[i]);
                                iFlag[i] = 1;
                            }
                        }
                    }
                }
                else
                {

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                    bool[] blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 1);
                }
            }


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新前总次数");
            string[] SumNoQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新前）", SumNoQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥状态字");
            status = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥状态字（更新前）", status);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            string UpdateKeyDate = DateTime.Now.ToString("yyMMddHHmmss");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取密钥更新后总次数");
            string[] SumNoH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "密钥更新总次数（更新后）", SumNoH);

            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);
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
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strRedData[i]) && strRedData[i].Length >= 30 && !string.IsNullOrEmpty(SumNoH[i]) && !string.IsNullOrEmpty(SumNoQ[i]))
                {
                    string datetmp = strRedData[i].ToString().Substring(18, 12);
                    if (datetmp != "000000000000")
                    {
                        int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(UpdateKeyDate), DateTimes.FormatStringToDateTime(datetmp));
                        if (iDateErr < 360 && (Convert.ToInt32(SumNoQ[i]) + 1) == Convert.ToInt32(SumNoH[i]))
                        {
                            blnRet[i, 0] = true;
                        }
                    }
                }
                ResultDictionary["正确参数密钥恢复"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("正确参数密钥恢复");



            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && status[i] == "00000000")
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
