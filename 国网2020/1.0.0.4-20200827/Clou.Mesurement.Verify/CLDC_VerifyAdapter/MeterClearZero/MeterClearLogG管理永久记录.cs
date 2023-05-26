﻿
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.MeterClearZero
{
    /// <summary>
    /// 管理永久记录
    /// </summary>
    class MeterClearLogG : VerifyBase
    {
        public MeterClearLogG(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻","结论", "不合格原因" };
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
            MessageController.Instance.AddMessage("正在读取事件清零前总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

           

            //设置1次
            for (int it = 0; it < 1; it++)
            {
                MessageController.Instance.AddMessage("正在第" + (it + 1) + "次清零....");
                //MeterProtocolAdapter.Instance.ClearEventLog("FFFFFFFF");

                  MeterProtocolAdapter.Instance.ClearEnergy();
           
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            }


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取事件清零后总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
          
            //清零上一次记录
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次事件清零记录发生时刻");
            string[] strLoseTimeQ1 = MeterProtocolAdapter.Instance.ReadData("03300101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ1);

         

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountH[i] == "" || strLoseCountQ[i] == "" || strLoseTimeQ1[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (strLoseCountQ[i].Contains("F") || strLoseCountH[i].Contains("F") || strLoseTimeQ1[i].Contains("F"))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "次数值带F值";
                    continue;
                }
                else if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "事件清零次数不匹配";
                    
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}
