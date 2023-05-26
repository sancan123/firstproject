using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;


namespace CLDC_VerifyAdapter.EventLog
{
    class EL_ErrorLoadSwitch:EventLogBase
    {
     
        //拉闸记录

        public EL_ErrorLoadSwitch(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论", "不合格原因" };
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
            //先合闸
            if (Stop) return;
            Helper.EquipHelper.Instance.DirectRemoteControl();
            //再隔离
            if (Stop) return;
            Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 0);

        
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表拉闸记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("1D000001", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            string[] strDataCode = new string[BwCount];
            string[] strHzDate = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在下发跳闸延迟时间为1分钟,请稍候....");
            string writedata = FormatWriteData("0001", "NNNN", 2, 0);
            Common.Memset(ref strDataCode, "04001401");
            Common.Memset(ref strData, "04001401" + writedata);
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表拉闸以便形成拉闸记录");

            Helper.EquipHelper.Instance.BreakRelay();
          //  CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib * 0.02F, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 70);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表拉闸记录产生后拉闸记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("1D000001", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电表拉闸记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);
            Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表合闸");
            Helper.EquipHelper.Instance.DirectRemoteControl();
            MessageController.Instance.AddMessage("正在升电压电流");
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            PowerOn();
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
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "次数不匹配";
                }
            }
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
