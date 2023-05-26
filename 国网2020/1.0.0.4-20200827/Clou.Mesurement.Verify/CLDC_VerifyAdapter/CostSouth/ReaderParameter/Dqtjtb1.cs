using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.CostSouth.ReaderParameter
{
    /// <summary>
    /// 当前套第1张阶梯表（读取）
    /// </summary>
    public class Dqtjtb1 : VerifyBase
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

        public Dqtjtb1(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "读取值", "结论" };
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
            string[] strData = new string[BwCount];
            string strReaderData = "";
            int[] iFlag = new int[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];

            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取当前套第1张阶梯表,请稍候....");
            strRevData = MeterProtocolAdapter.Instance.ReadData("040606FF", 140);
            strRevData = Common.ConverJtb(strRevData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "读取值", strRevData);

            for (int i = 0; i < BwCount; i++)
            {
                strReaderData += strRevData[i] + ",";
            }

            string strFile = "";
            if (VerifyProcess.Instance.CurrentKey.Substring(0, 2) == "09")
            {
                strFile = "Parameter\\ReaderParameterData_Local.ini";
            }
            else
            {
                strFile = "Parameter\\ReaderParameterData_Remote.ini";
            }
            CLDC_DataCore.Function.File.WriteInIString(strFile, "Price", "040606FF", strReaderData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strRevData[i]))
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
