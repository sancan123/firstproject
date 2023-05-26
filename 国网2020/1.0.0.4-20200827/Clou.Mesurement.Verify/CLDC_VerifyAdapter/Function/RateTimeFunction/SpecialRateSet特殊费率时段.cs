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
namespace CLDC_VerifyAdapter.Function.RateTimeFunction
{
    class SpecialRateSet:VerifyBase
    {


           #region ----------构造函数----------

        public SpecialRateSet(object plan)
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
            ResultNames = new string[] {"测试时间", "公共假日数(前)", "第一公共假日日期及日时段表号(前)", "公共假日数(后)", "第一公共假日日期及日时段表号(后)",  "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];


           MessageController.Instance.AddMessage("正在读取公共假日数");
           string[] str_GGJRS = MeterProtocolAdapter.Instance.ReadData("04000205", 2);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "公共假日数(前)", str_GGJRS);
           MessageController.Instance.AddMessage("正在读取第一公共假日日期及日时段表号");
           string[] str_GGJRB = MeterProtocolAdapter.Instance.ReadData("04030001", 4);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一公共假日日期及日时段表号(前)", str_GGJRB);
                 
            if (Stop) return;
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];          
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("0001", "NNNN", 2, 0);
            Common.Memset(ref strDataCode, "04000205");
            Common.Memset(ref strData, "04000205" + writedata);
            MessageController.Instance.AddMessage("正在设置公共假日数");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResult[i] == false)
                    {
                        Result[i] = false;
                        Fail[i] = "设置公共假日数失败";
                    }
                    else
                    {
                        Result[i] = true;
                    }

                }
            }

            string dateTime = DateTime.Now.ToString("yy") + "122901";
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            writedata = FormatWriteData(dateTime, "NNNNNNNN", 4, 0);
            Common.Memset(ref strDataCode, "04030001");
            Common.Memset(ref strData, "04030001" + writedata);
            MessageController.Instance.AddMessage("正在设置第一公共假日日期及日时段表号");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResult[i] == false)
                    {
                        Result[i] = false;
                        Fail[i] = Fail[i]+ "设置第一公共假日日期及日时段表号失败";
                    }
                    else
                    {
                        Result[i] = true;
                    }

                }
            }
            MessageController.Instance.AddMessage("正在设置12月29号7点...");
            string Time = DateTime.Now.ToString("yy") + "1229070000";
            MeterProtocolAdapter.Instance.WriteDateTime(Time);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

            MessageController.Instance.AddMessage("正在读取公共假日数");
            string[] str_GGJRS1 = MeterProtocolAdapter.Instance.ReadData("04000205", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "公共假日数(后)", str_GGJRS1);
            MessageController.Instance.AddMessage("正在读取第一公共假日日期及日时段表号");
            string[] str_GGJRB1 = MeterProtocolAdapter.Instance.ReadData("04030001", 4);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一公共假日日期及日时段表号(后)", str_GGJRB1);
            System.Windows.Forms.MessageBox.Show("请查看表上是否显示费率为尖。按确定之后填写结论！");

            Form_Ariticial fs = new Form_Ariticial("费率时段设置");
            fs.ShowDialog();
            string[] bResultXs = GlobalUnit.ManualResult;
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (bResultXs[i] == "不合格")
                    {

                        Fail[i] = Fail[i] + ",表上显示错误失败";
                    }


                }
            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(Fail[i]))
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                }

            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", Fail);





            Time = DateTime.Now.ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(Time);
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置第一公共假日日期及日时段表号");
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(str_GGJRB[i]))
                {
                    strData[i] = "04030001" + "99999999";
                }
            }
            Common.Memset(ref strDataCode, "04030001");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置公共假日数");
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(str_GGJRS[i]))
                {
                    strData[i] = "04000205" + "0000";
                }
            }
            Common.Memset(ref strDataCode, "04000205");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

           


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
        public string ConvertArryToOne(string[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                  return Data[i];
                
            }
            return "";
        }

    }
}
