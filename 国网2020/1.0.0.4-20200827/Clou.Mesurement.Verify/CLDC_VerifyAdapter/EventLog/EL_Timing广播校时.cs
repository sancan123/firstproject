using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 广播校时记录，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_Timing : EventLogBase
    {
        public EL_Timing(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "上4次事件记录发生时刻", "上7次事件记录发生时刻", "上10次事件记录发生时刻", "结论", "不合格原因" };
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
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号


            if (Stop) return;
          //  MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取广播校时事件产生前广播校时总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03305100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

             //设置10次校时
            Random rand = new Random();
           int randDays= rand.Next(400);
           iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            for (int it = 0; it < 10; it++)
            {
                if (Stop) return;
               
                string strDt = DateTime.Now.AddDays(it + randDays).ToString("yyMMddHHmmss");
                MeterProtocolAdapter.Instance.WriteDateTime(strDt);

                string datetime = DateTime.Now.AddDays(it + randDays).AddMinutes(5).ToString("yyMMddHHmmss");
                MessageController.Instance.AddMessage("正在第"+(it + 1) + "次广播校时");
                MeterProtocolAdapter.Instance.BroadCastTime(CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(datetime));
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 40);

                //先判断第一次是否合格，如果不合格就不做下面任务
                if (it == 0)
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取广播校时事件产生后广播校时总次数");
                    string[] strLoseCount1 = MeterProtocolAdapter.Instance.ReadData("03305100", 3);
                    bool isHeGe = true;
                    for (int i = 0; i < BwCount; i++)
                    {

                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (strLoseCount1[i] == "" )
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            reasonS[i] = "次数值为空";
                            continue;
                        }
                        if (string.IsNullOrEmpty(strLoseCountQ[i]) && string.IsNullOrEmpty(strLoseCount1[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 != Convert.ToInt32(strLoseCount1[i]))
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            reasonS[i] = "次数不匹配";
                            isHeGe = false;
                        }
                       
                    }
                    if (isHeGe==false)
                    { 
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
                    UploadTestResult("结论");
                    return;
                    }
                }
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取广播校时事件产生后广播校时总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03305100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            //上1次次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电表广播校时记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03305101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次清零时发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03305104", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次清零时发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03305107", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次清零时发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330510A", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

        
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" || strLoseTimeQ4[i] == "" || strLoseTimeQ7[i] == "" || strLoseTimeQ10[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 10 == Convert.ToInt32(strLoseCountH[i]))
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
           
            //日期设置为现在   
            MessageController.Instance.AddMessage("正在设置日期为现在...");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

            UploadTestResult("结论");
        }
    }
}
