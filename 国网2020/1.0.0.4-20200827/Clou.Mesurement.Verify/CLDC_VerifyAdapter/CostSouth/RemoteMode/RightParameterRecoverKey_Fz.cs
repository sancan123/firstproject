using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 密钥恢复(辅助功能)
    /// </summary>
    public class RightParameterRecoverKey_Fz : VerifyBase
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
        //
        public RightParameterRecoverKey_Fz(object plan)
            : base(plan)
        {
        }
        //密钥状态(下装前)|密钥状态(下装后)|密钥恢复
        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "密钥状态(下装前)", "密钥状态(下装后)", "密钥恢复", "结论" };
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



            #region 准备工作

            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            bool[] blnRecKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            #endregion


            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查恢复前密钥状态,请稍候....");
            string[] statusQ = MeterProtocolAdapter.Instance.ReadData("04000508", 4);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查恢复后密钥状态,请稍候....");
            string[] statusH = MeterProtocolAdapter.Instance.ReadData("04000508", 4);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["密钥状态(下装前)"][i] = statusQ[i];
                ResultDictionary["密钥状态(下装后)"][i] = statusH[i];
                ResultDictionary["密钥恢复"][i] = blnUpKeyRet[i] ? "成功" : "失败";
            }

            UploadTestResult("密钥状态(下装前)");
            UploadTestResult("密钥状态(下装后)");
            UploadTestResult("密钥恢复");

            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnUpKeyRet[i])
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
