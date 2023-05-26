using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Function.OperationFution
{
    class InstallationSite: VerifyBase
    {
        public InstallationSite(object plan)
            : base(plan)
        { }

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

        protected override bool CheckPara()
        {
            ResultNames = new string[] {"测试时间", "经度", "纬度", "高度","结论" ,"不合格原因"};
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            string[] strReadData = new string[BwCount];
            if (Stop) return;
            PowerOn();
            MessageController.Instance.AddMessage("设置电表安装位置为北纬23°10′26″,东经113°26′14″,高度10m");
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] JD = new string[BwCount];
            string[] WD = new string[BwCount];
            string[] GD = new string[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("0023102601132614001000", "NNNNNNNNNNNNNNNNNNNNNN", 11, 0);
            Common.Memset(ref strDataCode, "0400040F");
            Common.Memset(ref strData, "0400040F" + writedata);
            MessageController.Instance.AddMessage("正在设置电表安装位置");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            MessageController.Instance.AddMessage("正在读取电表安装位置");
            string[] strAZWZ = MeterProtocolAdapter.Instance.ReadData("0400040F", 11);
            JD = GetSite(strAZWZ, 0);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "经度", JD);
            WD = GetSite(strAZWZ, 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "纬度", WD);
            GD = GetSite(strAZWZ, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "高度", GD);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strAZWZ[i]))
                {
                    if (JD[i] == "23.1026" && WD[i] == "113.2614" && GD[i] == "10")
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] ="电表安装位置设置不对";
                    }
                   
                }
                else
                {
                    ResultDictionary["不合格原因"][i] = "返回数据为空";
                    ResultDictionary["结论"][i] = "不合格";
                }

            }
            UploadTestResult("结论");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

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

        private string[] GetSite(string[] str, int h)
        {

            string[] Site = new string[BwCount];
            string[]  StrSite = new string[BwCount];
            for (int i = 0; i < str.Length; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(str[i]) && str[i].Length>=22)
                {
                    switch (h)
                    {
                        case 0:
                            StrSite[i] = str[i].Substring(14, 8);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                        case 1:
                            StrSite[i] = str[i].Substring(6, 8);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                        case 2:
                            StrSite[i] = str[i].Substring(0, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 100).ToString();
                            break;
                    }
                }
            }
            return Site;

        }




    }
}

