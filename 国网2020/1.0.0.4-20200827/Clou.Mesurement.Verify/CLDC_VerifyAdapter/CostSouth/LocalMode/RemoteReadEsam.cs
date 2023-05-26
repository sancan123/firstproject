using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 数据回抄
    /// </summary>
    public class RemoteReadEsam : VerifyBase
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

        public RemoteReadEsam(object plan)
            : base(plan)
        {


        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "参数信息文件回抄结果", "第1次回抄参数信息文件", "第1次回抄购电信息文件", "第1次回抄运行信息文件",
                                       "参数信息文件回抄1次值","参数信息文件回抄2次值","参数信息文件回抄3次值","参数信息文件回抄4次值","连续4次回抄参数文件",
                                       "购电信息文件回抄1次值","购电信息文件回抄2次值","购电信息文件回抄3次值","购电信息文件回抄4次值","连续4次回抄购电文件",
                                       "运行信息文件回抄1次值","运行信息文件回抄2次值","运行信息文件回抄3次值","运行信息文件回抄4次值","连续4次回抄运行文件",
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
            string[] strRevCode = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevMac2 = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 13];
            bool[] result = new bool[BwCount];
            string[] strRedIniParaInfo = new string[BwCount];
            string[] strRedIniBuyInfo = new string[BwCount];
            string[] strRedIniRunInfo = new string[BwCount];
            string[] strRedParaInfo = new string[BwCount];
            string[] strRedBuyInfo = new string[BwCount];
            string[] strRedRunInfo = new string[BwCount];
            int[] iFlag = new int[BwCount];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            //1------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在数据回抄标识不合法下，回抄安全模块中的参数信息文件,请稍候....");
            Common.Memset(ref strRevCode, "AB01000100000030");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedIniParaInfo, out strRevMac1);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty( strRedIniParaInfo[i])) 
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["参数信息文件回抄结果"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("参数信息文件回抄结果");

            //2----------
            //参数信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄安全模块中的参数信息文件,请稍候....");
            Common.Memset(ref strRevCode,"DF01000100000030");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedIniParaInfo, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次回抄参数信息文件", strRedIniParaInfo);

            //购电信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄安全模块中的购电信息文件,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000008");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedIniBuyInfo, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次回抄购电信息文件", strRedIniBuyInfo);

            //运行信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄安全模块中的运行信息文件,请稍候....");
            Common.Memset(ref strRevCode, "DF01000600000032");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedIniRunInfo, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第1次回抄运行信息文件", strRedIniRunInfo);


            //3----------------
            //参数信息文件
            Common.Memset(ref strRevCode, "DF01000100000030");
            for (int j = 0; j < 4; j++)
            {                        
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (j + 1) + "次回抄安全模块中的参数信息文件,请稍候....");
                
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedParaInfo, out strRevMac1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件回抄" + (j + 1) + "次值", strRedParaInfo);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedIniParaInfo[i]) || string.IsNullOrEmpty(strRedParaInfo[i])) continue;
                    if (strRedParaInfo[i] == strRedIniParaInfo[i])
                    {
                        blnRet[i, 1 + j] = true;
                    }
                }
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4])
                {
                    ResultDictionary["连续4次回抄参数文件"][i] = "通过";
                }
                else
                {
                    ResultDictionary["连续4次回抄参数文件"][i] = "不通过";
                }
            }
            UploadTestResult("连续4次回抄参数文件");

            //4----------------
            //购电信息文件               
            Common.Memset(ref strRevCode, "DF01000200000008");
            //Common.Memset(ref strRevCode, "DF010003000000C7");

            for (int j = 0; j < 4; j++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (j + 1) + "次回抄安全模块中的购电信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedBuyInfo, out strRevMac1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电信息文件回抄" + (j + 1) + "次值", strRedBuyInfo);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedIniBuyInfo[i]) || string.IsNullOrEmpty(strRedBuyInfo[i])) continue;
                    if (strRedBuyInfo[i] == strRedIniBuyInfo[i])
                    {
                        blnRet[i, 5 + j] = true;
                    }
                }
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8])
                {
                    ResultDictionary["连续4次回抄购电文件"][i] = "通过";
                }
                else
                {
                    ResultDictionary["连续4次回抄购电文件"][i] = "不通过";
                }
            }
            UploadTestResult("连续4次回抄购电文件");

            //5--------------------
            //运行信息文件  
            Common.Memset(ref strRevCode, "DF01000600000032");
            //Common.Memset(ref strRevCode, "DF010004000000C7");
            for (int j = 0; j < 4; j++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (j + 1) + "次回抄安全模块中的运行信息文件,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out strRedRunInfo, out strRevMac1);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "运行信息文件回抄" + (j + 1) + "次值", strRedRunInfo);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (string.IsNullOrEmpty(strRedIniBuyInfo[i]) || string.IsNullOrEmpty(strRedBuyInfo[i])) continue;
                    if (strRedBuyInfo[i] == strRedIniBuyInfo[i])
                    {
                        blnRet[i, 9 + j] = true;
                    }
                }
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11] && blnRet[i, 12])
                {
                    ResultDictionary["连续4次回抄运行文件"][i] = "通过";
                }
                else
                {
                    ResultDictionary["连续4次回抄运行文件"][i] = "不通过";
                }
            }
            UploadTestResult("连续4次回抄运行文件");

            //----------------处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] &&
                        blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9] && blnRet[i, 10] && blnRet[i, 11]
                        && blnRet[i,12])
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
