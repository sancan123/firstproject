using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;


namespace CLDC_VerifyAdapter.Function.RateTimeFunction
{
    class RatePolishing:VerifyBase
    {
        public RatePolishing(object plan)
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
            ResultNames = new string[] { "测试时间", "日时段数(前)", "第二套时段表(前)","日时段数(后)","第二套时段表(后)" ,"结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();
            string[] Fail = new string[BwCount];
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表运行状态字3");
            string[] str_DBZTZ = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

            MessageController.Instance.AddMessage("正在读取日时段数");
            string[] flt_SDS = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数(前)", flt_SDS);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (string.IsNullOrEmpty(flt_SDS[i]))
                    {

                        Fail[i] =  "读取日时段数失败";
                    }


                }
            }






            MessageController.Instance.AddMessage("正在读取第二套时段表");
            string[] str_SDB2 = MeterProtocolAdapter.Instance.ReadData("04020001", 24);
            for (int i = 0; i < BwCount; i++)
            {
                str_SDB2[i] = Common.SortMin(str_SDB2[i]);

            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (string.IsNullOrEmpty(str_SDB2[i]))
                    {

                        Fail[i] = Fail[i] + "读取第二套时段表失败";
                    }


                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时段表(前)", str_SDB2);

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("09", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000203");
            Common.Memset(ref strData, "04000203" + writedata);
            MessageController.Instance.AddMessage("正在设置日时段数");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);



            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (!bResult[i])
                    {

                        Fail[i] = Fail[i] + "设置日时段数失败";
                    }


                }
            }
            MessageController.Instance.AddMessage("正在读取日时段数");
            string[] flt_SDS1 = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日时段数(后)", flt_SDS1);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (string.IsNullOrEmpty(flt_SDS1[i]))
                    {

                        Fail[i] = Fail[i] + "读取日时段数失败";
                    }


                }
            }
            MessageController.Instance.AddMessage("正在读取第二套时段表");
            string[] str_SDB21 = MeterProtocolAdapter.Instance.ReadData("04020001", 27);
            for (int i = 0; i < BwCount; i++)
            {
                str_SDB21[i] = Common.SortMin(str_SDB21[i]);

            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (string.IsNullOrEmpty(str_SDB21[i]))
                    {

                        Fail[i] = Fail[i] + "读取第二套时段表失败";
                    }


                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二套时段表(后)", str_SDB21);


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
                        if (str_SDB21[i].Substring(48, 6) == str_SDB2[i].Substring(42, 6))
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                       
                    }
                }

            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", Fail);






         
        }
        public string ConvertArryToOne(string[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                return Data[i];

            }
            return "";
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