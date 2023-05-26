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
    /// 对时（恢复）
    /// </summary>
   public class SetTime:VerifyBase
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

        public SetTime(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "设置值", "表里值", "结论" };
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
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] strMeterTime = new string[BwCount];
            string[] strShowData = new string[BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += DateTime.Now.ToString("HHmmss");
                strShowData[i] = DateTime.Now.ToString("yyMMddHHmmss");
                strData[i] = strCode[i] + strSetData[i];
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设置值", strShowData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取表计时间,请稍候....");
            DateTime[] MeterTime = MeterProtocolAdapter.Instance.ReadDateTime();


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(MeterTime[i].ToString()))
                {
                    strMeterTime[i] = MeterTime[i].ToString();
                    DateTime dateTimeTmp = DateTimes.FormatStringToDateTime(strShowData[i]);
                    int Second = DateTimes.DateDiff(dateTimeTmp, MeterTime[i]);
                    if (Second < System.Math.Abs(60))
                    {
                        blnRet[i, 0] = true;
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表里值", strMeterTime);


            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0])
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
