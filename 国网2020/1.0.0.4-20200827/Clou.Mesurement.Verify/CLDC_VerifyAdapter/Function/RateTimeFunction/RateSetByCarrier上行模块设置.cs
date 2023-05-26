using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;
using System.Threading;

namespace CLDC_VerifyAdapter.Function.RateTimeFunction
{
    class RateSetByCarrier:VerifyBase
    {
        public RateSetByCarrier(object plan)
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
            ResultNames = new string[] { "测试时间", "设置前日时段数", "设置后日时段数", "结论", "不合格原因" };
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

            MessageController.Instance.AddMessage("正在读取日时段数");
            string[] flt_SDS = MeterProtocolAdapter.Instance.ReadData("04000203", 1);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["设置前日时段数"][i] = flt_SDS[i];


            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设置前日时段数", ResultDictionary["设置前日时段数"]);
            if (Stop) return;
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = true;

            if (GlobalUnit.CarrierInfo.CarrierName == "中电华瑞2016")
            {
                GlobalUnit.Flag_IsZD2016 = true;
            }
            else
            {
                GlobalUnit.Flag_IsZD2016 = false;
            }
            SwitchCarrierOr485(Cus_CommunType.通讯载波);
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            bool[] result = new bool[BwCount];
            string[] strEnerZQ = new string[BwCount];//
            bool[] Meter = new bool[BwCount];
            bool[] Result1 = new bool[BwCount];
            bool[] Result = new bool[BwCount];
            string[] flt_SDSH = new string[BwCount];
            string[] flt_SDSH1 = new string[BwCount];
            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {

                Meter[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn;


            }
          
          
            if (Stop) return;
            string[] strDataCode = new string[BwCount];
            for (int iBw = 0; iBw < BwCount; iBw++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(iBw).YaoJianYn) continue;
                for (int i = 0; i < BwCount; i++)
                {
                    if (i == iBw)
                    {
                        Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = true;
                    }
                    else
                    {
                        Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = false;
                    }
                }
                GlobalUnit.Carrier_Cur_BwIndex = iBw;
                Thread.Sleep(1000);
                GlobalUnit.g_CommunType = Cus_CommunType.通讯485;
                MessageController.Instance.AddMessage("正在试验第"+(iBw+1)+"表位");
            
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                GlobalUnit.g_CommunType = Cus_CommunType.通讯载波;
                string writedata = FormatWriteData("16", "NN", 1, 0);
                Common.Memset(ref strDataCode, "04000203");
                Common.Memset(ref strData, "04000203" + writedata);
                MessageController.Instance.AddMessage("正在设置日时段数");
                Result1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

                Result[iBw] = Result1[iBw];
                for (int i = 0; i < BwCount; i++)
                {

                    Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = Meter[i];

                }
            }

            SwitchCarrierOr485(Cus_CommunType.通讯485);
            PowerOn();
            MessageController.Instance.AddMessage("正在读取日时段数");

            flt_SDSH = MeterProtocolAdapter.Instance.ReadData("04000203", 1);
                                               

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["设置后日时段数"][i] = flt_SDSH[i].ToString();


            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设置后日时段数", ResultDictionary["设置后日时段数"]);
           


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(flt_SDS[i]) && !string.IsNullOrEmpty(flt_SDSH[i]) && Result[i])
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                   
                  
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "写入参数失败";
                }
            }
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
           
            string writedata1 = FormatWriteData("08", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04000203");
            Common.Memset(ref strData, "04000203" + writedata1);
            MessageController.Instance.AddMessage("正在设置日时段数");
            Result1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
          
          
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

