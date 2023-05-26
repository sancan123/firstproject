using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;
using System.Threading;
using System.Globalization;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 
    /// </summary>
    public class SaveData : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "A类掉电前","A类掉电后",
                                         "B类掉电前","B类掉电后",
                                         "C类掉电前","C类掉电后",
                                         "D类掉电前","D类掉电后",
                                         "结论" };
            return true;
        }

        string strPlan = "";

        public SaveData(object plan)
            : base(plan)
        {
            
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
            string[] strPutApdu = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strCode = new string[BwCount];

            bool[] rstTmp = new bool[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            string[] strID = new string[BwCount];

            //1-----读取掉电前ABCD类数据--------

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取A类数据中的(当前)正向有功总电能,请稍候....");
            float[] flt_DLQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A类掉电前", ConvertArray.ConvertFloat2Str(flt_DLQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取B类中的上一次冻结总电量数据,请稍候....");
            string strTime = DateTime.Now.AddMinutes(-1).ToString("yyMMddHHmm") + "01";
            string[] strDJDLQ = MeterProtocolAdapter.Instance.ReadLoadRecord("06110601", 5, strTime);
            strDJDLQ = GetDL(strDJDLQ);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B类掉电前", strDJDLQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取C类数据中的上1次掉电发生时刻正向有功总电能,请稍候....");
            float[] flt_DDDLQ = MeterProtocolAdapter.Instance.ReadData("03110201", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C类掉电前", ConvertArray.ConvertFloat2Str(flt_DDDLQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取D类中的日期时间,请稍候....");
            DateTime[] TimeQ = MeterProtocolAdapter.Instance.ReadDateTime();
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "D类掉电前", ConvertArray.ConvertDateTime2String(TimeQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在掉电5分钟,请稍候....");
            Helper.EquipHelper.Instance.PowerOff();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 5);
            if (Stop) return;
            PowerOn();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *20);
            //2-----读取掉电后ABCD类数据--------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取A类数据中的(当前)正向有功总电能,请稍候....");
            float[] flt_DLH = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "A类掉电后", ConvertArray.ConvertFloat2Str(flt_DLH));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取B类中的上一次冻结总电量数据,请稍候....");
            strTime = DateTime.Now.AddMinutes(-2).ToString("yyMMddHHmm") + "01";
            string[] strDJDLH = MeterProtocolAdapter.Instance.ReadLoadRecord("06110601", 5, strTime);
            strDJDLH = GetDL(strDJDLH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "B类掉电后", strDJDLH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取C类数据中的上1次掉电发生时刻正向有功总电能,请稍候....");
            float[] flt_DDDLH = MeterProtocolAdapter.Instance.ReadData("03110201", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "C类掉电后", ConvertArray.ConvertFloat2Str(flt_DDDLH));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取D类中的日期时间,请稍候....");
            DateTime[] TimeH = MeterProtocolAdapter.Instance.ReadDateTime();
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "D类掉电后", ConvertArray.ConvertDateTime2String(TimeH));

            MessageController.Instance.AddMessage("正在计算结果,请稍候....");
            try
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(flt_DLQ[i].ToString()) && !string.IsNullOrEmpty(flt_DLH[i].ToString()) 
                        && !string.IsNullOrEmpty(strDJDLQ[i]) && !string.IsNullOrEmpty(strDJDLH[i])
                        && !string.IsNullOrEmpty(flt_DDDLQ[i].ToString()) && !string.IsNullOrEmpty(flt_DDDLH[i].ToString())
                        && !string.IsNullOrEmpty(TimeQ[i].ToString()) && !string.IsNullOrEmpty(TimeH[i].ToString()))
                    {
                        if (flt_DLQ[i] == flt_DLH[i] && strDJDLQ[i] == strDJDLH[i] && flt_DDDLQ[i] == flt_DDDLH[i])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            catch (Exception)
            { }

            UploadTestResult("结论");
        }

        /// <summary>
        /// 截取电量值并转换成Float
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string[] GetDL(string[] strData)
        {
            float[] RevData = new float[strData.Length];
            string[] strRevData = new string[strData.Length];
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]) && strData[i].Length >= 10)
                {
                    RevData[i] = float.Parse(strData[i].Substring(0, 10)) / 10000;
                    strRevData[i] = RevData[i].ToString();
                }
            }
            return strRevData;
        }
    }
}
