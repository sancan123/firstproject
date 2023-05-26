using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.CostSouth.ReaderParameter
{
    /// <summary>
    /// 电流互感器变比（读取）
    /// </summary>
    public class Dlhgqbb : VerifyBase
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

        public Dlhgqbb(object plan)
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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电流互感器变比,请稍候....");
            string[] strRedData = MeterProtocolAdapter.Instance.ReadData("04000306", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "读取值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                strReaderData += strRedData[i] + ",";
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
            CLDC_DataCore.Function.File.WriteInIString(strFile, "Parameter1", "04000306", strReaderData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strRedData[i]))
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
