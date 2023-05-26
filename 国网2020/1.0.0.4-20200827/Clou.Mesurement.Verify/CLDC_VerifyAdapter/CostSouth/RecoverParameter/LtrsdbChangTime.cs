using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.CostSouth.RecoverParameter
{
    /// <summary>
    /// 两套日时段表切换时间（恢复）
    /// </summary>
   public class LtrsdbChangTime:VerifyBase
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

        public LtrsdbChangTime(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "原始值", "当前值", "结论" };
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
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];


            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            string strFile = "";
            if (VerifyProcess.Instance.CurrentKey.Substring(0, 2) == "99")
            {
                strFile = "Parameter\\ReaderParameterData_Local.ini";
            }
            else
            {
                strFile = "Parameter\\ReaderParameterData_Remote.ini";
            }
            string strReaderFileData = CLDC_DataCore.Function.File.ReadInIString(strFile, "Parameter2", "04000107", "");
            string[] DataTmp = strReaderFileData.Split(',');
            for (int i = 0; i < BwCount; i++)
            {
                if (i <= DataTmp.Length && !string.IsNullOrEmpty(DataTmp[i]))
                {
                    strSetData[i] = DataTmp[i];
                    strData[i] = "04000107" + DataTmp[i];
                }
            }
            Common.Memset(ref strID, "04000107");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "原始值", strSetData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置两套日时段表切换时间,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取两套日时段表切换时间,请稍候....");
            string[] strRedData = MeterProtocolAdapter.Instance.ReadData("04000107", 5);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSetData[i] == strRedData[i])
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
