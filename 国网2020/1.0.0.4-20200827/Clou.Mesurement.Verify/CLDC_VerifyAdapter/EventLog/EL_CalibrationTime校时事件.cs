using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 校时事件记录，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_CalibrationTime:EventLogBase
    {
        public EL_CalibrationTime(object plan)
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
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "上4次事件记录发生时刻", "上7次事件记录发生时刻", "上10次事件记录发生时刻", "结论", "不合格原因" };
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
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时事件产生前掉电总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
              
            //设置10次校时
            for (int it = 0; it < 10; it++)
            {
                string datetime = DateTime.Now.AddHours(it+1).ToString("yyMMddHHmmss");//每次对时加1小时
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = datetime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += datetime.Substring(6, 6);
                    strShowData[i] = datetime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (it + 1) + "次校时....");
                bool[] result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时事件产生后掉电总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            //上一次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次校时发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03300401", 16);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strLoseTimeQ[i]))
                {
                    strLoseTimeQ[i] = strLoseTimeQ[i].Substring(0, 12);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次校时发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03300404", 16);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strLoseTimeQ4[i]))
                {
                    strLoseTimeQ4[i] = strLoseTimeQ4[i].Substring(0, 12);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次校时发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03300407", 16);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strLoseTimeQ7[i]))
                {
                    strLoseTimeQ7[i] = strLoseTimeQ7[i].Substring(0, 12);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次校时发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330040A", 16);
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strLoseTimeQ10[i]))
                {
                    strLoseTimeQ10[i] = strLoseTimeQ10[i].Substring(0, 12);
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            //日期设置会现在             
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

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
                    reasonS[i] = "校时次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}

