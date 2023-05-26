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
    class EachLinkWc: VerifyBase
    {
        public EachLinkWc(object plan)
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
            ResultNames = new string[] {"测试时间", "出厂验收", "首次检定", "运行检验","退役鉴定", "结论","不合格原因" };
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
            string[] Time = new string[BwCount];
            string[] Imax10 = new string[BwCount];
            string[] Imax05 = new string[BwCount];
            string[] Itr10 = new string[BwCount];
            string[] Itr05 = new string[BwCount];
            string[] Imin = new string[BwCount];

            string[] CCYS = new string[BwCount];
            string[] SCJD= new string[BwCount];
            string[] YXJD = new string[BwCount];
            string[] TYJD = new string[BwCount];

            string[] CCYSDQ = new string[BwCount];
            string[] SCJDDQ = new string[BwCount];
            string[] YXJDDQ = new string[BwCount];
            string[] TYJDDQ = new string[BwCount];

            bool[] CC = new bool[BwCount];
            bool[] SC = new bool[BwCount];
            bool[] YX = new bool[BwCount];
            bool[] TY = new bool[BwCount];
            MessageController.Instance.AddMessage("正在读取出厂验收误差数据");
            CCYS = MeterProtocolAdapter.Instance.ReadData("04092701", 20);
            Time = GetSite(CCYS, 0);
            Imax10 = GetSite(CCYS, 1);
            Imax05 = GetSite(CCYS, 2);
            Itr10 = GetSite(CCYS, 3);
            Itr05 = GetSite(CCYS, 4);
            Imin = GetSite(CCYS, 5);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(CCYS[i]))
                {
                    if (Time[i] == "0000000000" || Time[i] == null || Imax10[i] == null || Imax05[i] == null || Itr10[i] == null || Itr05[i] == null || Imin[i] == null) 
                    {
                        CC[i] = false;
                    }
                    else
                    {
                        CC[i] = true;
                    }
                    CCYSDQ[i] = Time[i] + "|" + Imax10[i] + "|" + Imax05[i] + "|" + Itr10[i] + "|" + Itr05[i] + "|" + Imin[i] + "|";

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "出厂验收", CCYSDQ);

            MessageController.Instance.AddMessage("正在读取首次检定误差数据");
            SCJD = MeterProtocolAdapter.Instance.ReadData("04092702", 20);
            Time = GetSite(SCJD, 0);
            Imax10 = GetSite(SCJD, 1);
            Imax05 = GetSite(SCJD, 2);
            Itr10 = GetSite(SCJD, 3);
            Itr05 = GetSite(SCJD, 4);
            Imin = GetSite(SCJD, 5);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(SCJD[i]))
                {
                    if (Time[i] == "0000000000" || Time[i] == null || Imax10[i] == null || Imax05[i] == null || Itr10[i] == null || Itr05[i] == null || Imin[i] == null) 
                    {
                        SC[i] = false;
                    }
                    else
                    {
                        SC[i] = true;
                    }
                    SCJDDQ[i] = Time[i] + "|" + Imax10[i] + "|" + Imax05[i] + "|" + Itr10[i] + "|" + Itr05[i] + "|" + Imin[i] + "|";

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "首次检定", SCJDDQ);

            MessageController.Instance.AddMessage("正在读取运行检验误差数据");
            YXJD = MeterProtocolAdapter.Instance.ReadData("04092704", 20);
            Time = GetSite(YXJD, 0);
            Imax10 = GetSite(YXJD, 1);
            Imax05 = GetSite(YXJD, 2);
            Itr10 = GetSite(YXJD, 3);
            Itr05 = GetSite(YXJD, 4);
            Imin = GetSite(YXJD, 5);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(YXJD[i]))
                {
                    if (Time[i] == "0000000000" || Time[i] == null || Imax10[i] == null || Imax05[i] == null || Itr10[i] == null || Itr05[i] == null || Imin[i] == null) 
                    {
                        YX[i] = false;
                    }
                    else
                    {
                        YX[i] = true;
                    }
                    YXJDDQ[i] = Time[i] + "|" + Imax10[i] + "|" + Imax05[i] + "|" + Itr10[i] + "|" + Itr05[i] + "|" + Imin[i] + "|";

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "运行检验", YXJDDQ);

            MessageController.Instance.AddMessage("正在读取退役鉴定误差数据");
            TYJD = MeterProtocolAdapter.Instance.ReadData("04092703", 20);
            Time = GetSite(TYJD, 0);
            Imax10 = GetSite(TYJD, 1);
            Imax05 = GetSite(TYJD, 2);
            Itr10 = GetSite(TYJD, 3);
            Itr05 = GetSite(TYJD, 4);
            Imin = GetSite(TYJD, 5);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(TYJD[i]))
                {
                    if (Time[i] == "0000000000" || Time[i] == null || Imax10[i] == null || Imax05[i] == null || Itr10[i] == null || Itr05[i] == null || Imin[i] == null) 
                    {
                        TY[i] = false;
                    }
                    else
                    {
                        TY[i] = true;
                    }
                    TYJDDQ[i] = Time[i] + "|" + Imax10[i] + "|" + Imax05[i] + "|" + Itr10[i] + "|" + Itr05[i] + "|" + Imin[i] + "|";

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "退役鉴定", TYJDDQ);



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(CCYS[i]) && !string.IsNullOrEmpty(SCJD[i]) && !string.IsNullOrEmpty(YXJD[i]) && !string.IsNullOrEmpty(TYJD[i]))
                {
                    if (CC[i] == true && SC[i] == true && YX[i] == true && TY[i] == true)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = "读取数据有误";
                    }
                  
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "返回数据为空";
                }

            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
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
                if (!string.IsNullOrEmpty(str[i]) && str[i].Length>=40 && !str[i].Contains("F"))
                {
                    switch (h)
                    {
                        case 0:
                            StrSite[i] = str[i].Substring(0, 10);
                            Site[i] = StrSite[i];
                            break;
                        case 1:
                            StrSite[i] = str[i].Substring(10, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                        case 2:
                            StrSite[i] = str[i].Substring(16, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                        case 3:
                            StrSite[i] = str[i].Substring(22, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 100).ToString();
                            break;
                        case 4:
                            StrSite[i] = str[i].Substring(28, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                        case 5:
                            StrSite[i] = str[i].Substring(34, 6);
                            Site[i] = (float.Parse(StrSite[i]) / 10000).ToString();
                            break;
                    }
                }
            }
            return Site;

        }




    }
}

