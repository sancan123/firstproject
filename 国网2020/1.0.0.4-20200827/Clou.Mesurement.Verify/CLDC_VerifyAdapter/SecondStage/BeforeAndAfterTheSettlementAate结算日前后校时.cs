using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 结算日前后校时
    /// </summary>
    class BeforeAndAfterTheSettlementAate:VerifyBase
    {


           #region ----------构造函数----------

        public BeforeAndAfterTheSettlementAate(object plan)
            : base(plan)
        {
        }

        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "第一次校时前时间", "第一次校时时间", "第一次校时后时间", "第二次校时前时间", "第二次校时时间", "第二次校时后时间", "结论", "不合格原因" };
            return true;
      
        }

        #endregion                
        public override void Verify()
        {

            bool[] bResult = new bool[BwCount];


            base.Verify();
            bool bPowerOn = PowerOn();
            string[] str_Data = new string[BwCount];

            string[] str_DataFirst = new string[BwCount];
            string[] str_DataSecond = new string[BwCount];
            string[] strShowData = new string[BwCount];

            Random rand = new Random();
            int randDays = rand.Next(100);
            DateTime dt =DateTime.Parse(DateTime.Now.AddMonths(randDays).ToString("yyyy-MM-") + "01 00:00:00").AddMinutes(-17);
          
            if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间月底最后1天为23:43:00");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(dt.ToString("yyMMdd") + ("234300"));

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
            DateTime[] strMeterTimeBefore1 = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < strMeterTimeBefore1.Length; i++)
            {
                strShowData[i] = strMeterTimeBefore1[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时前时间", strShowData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发结算日前广播校时命令");
            string strBeforDt = dt.ToString("yyyy-MM-dd") + (" 23:50:00");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时时间", setSameStrArryValue(strBeforDt));
            MeterProtocolAdapter.Instance.BroadCastTime(DateTime.Parse(strBeforDt));


            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);



            //读零点前时间
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
            DateTime[] dateTimeQ = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < dateTimeQ.Length; i++)
            {
                strShowData[i] = dateTimeQ[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时后时间", strShowData);


            //零点后处理
            if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间为00:09:55");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(dt.AddDays(1).ToString("yyMMdd") + ("000955"));//235955
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            DateTime[] strMeterTimeBefore2 = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < strMeterTimeBefore2.Length; i++)
            {
                strShowData[i] = strMeterTimeBefore2[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时前时间", strShowData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发结算日后广播校时命令");
            string strAfterDt2 = dt.AddDays(1).ToString("yyyy-MM-dd") + (" 00:17:00");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时时间", setSameStrArryValue(strAfterDt2));

            MeterProtocolAdapter.Instance.BroadCastTime(DateTime.Parse(strAfterDt2));

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            //读零点后时间
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
            DateTime[] dateTimeH = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < dateTimeH.Length; i++)
            {
                strShowData[i] = dateTimeH[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时后时间", strShowData);



            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                //计算时间

                double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(dateTimeQ[i], DateTime.Parse(strBeforDt));
                double Err2 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(dateTimeH[i], DateTime.Parse(strAfterDt2));
                str_DataFirst[i] = strBeforDt;
                str_DataSecond[i] = strAfterDt2;


                if (Err1 <= 30 && Err2 <= 30)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {

                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    if (Math.Abs(Err1) > 30)
                    {
                        reasonS[i] = "第一次校时后误差超过30s";
                    }
                    if (Math.Abs(Err2) > 30)
                    {
                        reasonS[i] = "第二次校时后误差超过30s";
                    }
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);


            //日期设置为现在   
            MessageController.Instance.AddMessage("正在设置日期为现在...");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

            UploadTestResult("结论");
           


        }

        /// <summary>
        /// 格式化写字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <param name="len"></param>
        /// <param name="pointindex"></param>
        /// <returns></returns>
        private string FormatWriteData(string data, string strformat, int len, int pointindex)
        {
            string formatdata = "";
            try
            {
                if (data == "" || data == null) return "";
                formatdata = data;
                bool blnIsNum = true;           //判断读取的数据是不是数字
                List<char> splitChar = new List<char>(new char[] { '.', 'N' });
                for (int i = 0; i < strformat.Length; i++)
                {
                    if (!splitChar.Contains(strformat[i]))
                    {
                        blnIsNum = false;
                        break;
                    }
                }
                if (pointindex != 0)
                {
                    if (blnIsNum)
                    {
                        int left = len * 2 - pointindex;
                        int right = pointindex;
                        formatdata = float.Parse(formatdata).ToString();
                        string[] newdata = formatdata.Split('.');
                        if (newdata.Length == 1)
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            formatdata = newdata[0] + "".PadRight(right, '0');
                        }
                        else
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            if (newdata[1].Length <= right)
                            {
                                newdata[1] = newdata[1].PadRight(right, '0');
                            }
                            else
                            {
                                newdata[1] = newdata[1].Substring(0, right);
                            }
                            formatdata = newdata[0] + newdata[1];
                        }
                    }
                    else
                    {
                        formatdata = formatdata.Replace(".", "");
                        formatdata = formatdata.Replace("-", "");
                        if (formatdata.Length <= len * 2)
                        {
                            formatdata = formatdata.PadRight(len * 2, '0');
                        }
                        else
                        {
                            formatdata = formatdata.Substring(0, len * 2);
                        }
                    }
                }
                else
                {
                    if (formatdata.Length <= len * 2)
                    {
                        formatdata = formatdata.PadLeft(len * 2, '0');
                    }
                    else
                    {
                        formatdata = formatdata.Substring(0, len * 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.Instance.WriteInfo(ex.StackTrace);
            }
            return formatdata;
        }

    }
}
