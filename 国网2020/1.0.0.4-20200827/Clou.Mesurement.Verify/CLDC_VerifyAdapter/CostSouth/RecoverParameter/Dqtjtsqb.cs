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
    /// 当前套阶梯时区表（恢复）
    /// </summary>
   public class Dqtjtsqb:VerifyBase
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

        public Dqtjtsqb(object plan)
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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);
            string strFile = "";
            if (VerifyProcess.Instance.CurrentKey.Substring(0, 2) == "99")
            {
                strFile = "Parameter\\ReaderParameterData_Local.ini";
            }
            else
            {
                strFile = "Parameter\\ReaderParameterData_Remote.ini";
            }
            string strReaderFileData = CLDC_DataCore.Function.File.ReadInIString(strFile, "Parameter2", "04070000", "");
            string[] DataTmp = strReaderFileData.Split(',');
            for (int i = 0; i < BwCount; i++)
            {
                if (i <= DataTmp.Length && !string.IsNullOrEmpty(DataTmp[i]))
                {
                    strSetData[i] = DataTmp[i];
                    strData[i] = "04070001" + Common.SortMin(DataTmp[i]);  //当前套不能直接设置，要备用套阶梯时区表，先设置备用套，再设两套阶梯时区切换时间
                }
            }
            Common.Memset(ref strID, "04070001");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "原始值", strSetData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置备用套阶梯时区表,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯时区切换时间,请稍候....");
            Common.Memset(ref strID, "0400010A");
            Common.Memset(ref strData, "0400010A" + DateTime.Now.AddMinutes(-50).ToString("yyMMddHHmm"));
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前套阶梯时区表,请稍候....");
            string[] strRedData = MeterProtocolAdapter.Instance.ReadData("04070000", 12);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSetData[i] == Common.SortMax(strRedData[i]))
                {
                    strRedData[i] = Common.SortMax(strRedData[i]);
                }
                else if (strSetData[i] == Common.SortMin(strRedData[i]))
                {
                    strRedData[i] = Common.SortMin(strRedData[i]);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前值", strRedData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSetData[i] == strRedData[i] )
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
