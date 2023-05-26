using CLDC_DataCore;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 软件比对
    /// </summary>
    public class CompareToProgram : VerifyBase
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

        public CompareToProgram(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "测试时间","比对指令", "结论" };
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
            string[] strRevData = new string[BwCount];
            string strMyIndex = ""; //密钥索引
            string CpuId = "";      //cpu编号
            string BdyzS = "";      //比对因子起始地址
            string BdsjS = "";      //比对数据起始地址
            string[] strData = new string[BwCount];
            string[] RevData = new string[BwCount];
            int[] iFlag = new int[BwCount];

            //准备

            #region 准备工作
            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);
            #endregion

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CpuId = "00";
                strMyIndex = "05";
                BdyzS = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrOther2.PadLeft(8, '0');
                BdsjS = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrOther4.PadLeft(8, '0');

                strData[i] = CpuId + strMyIndex + DxString(BdyzS) + DxString(BdsjS);

            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发对比数据");
            bool[] result = MeterProtocolAdapter.Instance.SouthCmdData("078002FF", strData, out RevData);


            //处理结果
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (result[i] && !string.IsNullOrEmpty(RevData[i]) && RevData[i].Length >= 128)
                    {
                        ResultDictionary["比对指令"][i] = "支持";
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["比对指令"][i] = "失败";
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }

            UploadTestResult("比对指令");
            UploadTestResult("结论");
        }

        private string DxString(string str_Keyinfo1)
        {
            int Len = str_Keyinfo1.Length / 2;
            string DxStr = "";
            for (int i = 0; i < Len; i++)
            {
                DxStr = str_Keyinfo1.Substring(i * 2, 2) + DxStr;
            }
            return DxStr;
        }

    }
}
