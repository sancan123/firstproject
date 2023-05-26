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
    /// 当前套第1张阶梯表（恢复）
    /// </summary>
   public class Dqtjtb1:VerifyBase
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
            string[] strRevCode = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];

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
            string strReaderFileData = CLDC_DataCore.Function.File.ReadInIString(strFile, "Price", "040606FF", "");
            string[] DataTmp = strReaderFileData.Split(',');
            for (int i = 0; i < BwCount; i++)
            {
                if (i <= DataTmp.Length && !string.IsNullOrEmpty(DataTmp[i]))
                {
                    strSetData[i] = DataTmp[i];
                    strData[i] =  DataTmp[i];
                }
            }
           
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "原始值", strSetData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套第1张阶梯表,请稍候....");
            Common.Memset(ref strID, "04060AFF");
            Common.Memset(ref strPutApdu, "04D684344A");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-50).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D684C009");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行读取当前套第1张阶梯表,请稍候....");
            strRevData = MeterProtocolAdapter.Instance.ReadData("040606FF", 140);
            strRevData = Common.ConverJtb(strRevData);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前值", strRevData);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSetData[i] == strRevData[i])
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
