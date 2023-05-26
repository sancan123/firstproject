using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Function.VoltageQualifiedRate
{
    class HightVoltageCheck:VerifyBase
    {
        public HightVoltageCheck(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "事件产生时间", "结论", "不合格原因" };
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
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取高电压事件产生前高电压事件总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03BD0400", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string writedata = FormatWriteData("01", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04091102");
            Common.Memset(ref strData, "04091102" + writedata);
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            MessageController.Instance.AddMessage("开始升低电压，请稍候......");
            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U*1.3F, 0, 1, 1, "1.0", true, false);
            if (!ret)
            {
                throw new Exception("升源失败");
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3650);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取高电压事件产生后高电压事件总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03BD0400", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            MessageController.Instance.AddMessage("正在读取高电压事件产生后高电压事件发生时间");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03BD0401",6);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strLoseTimeQ[i]))
                {
                    strLoseTimeQ[i] = strLoseTimeQ[i].Substring(0, 12);
                }
            }



            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生时间", strLoseTimeQ);
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) +1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "校时次数不匹配";
                }
            }
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
             writedata = FormatWriteData("20", "NN", 1, 0);
            Common.Memset(ref strDataCode, "04091102");
            Common.Memset(ref strData, "04091102" + writedata);
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

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

