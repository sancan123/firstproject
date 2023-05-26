using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 掉电事件记录，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_Acdump:EventLogBase
    {
        public EL_Acdump(object plan)
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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取掉电事件产生前掉电总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03110000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

              //设置10次校时
            for (int it = 0; it < 10; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表掉电以便形成掉电记录");
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
             //   CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * 0.5F, GlobalUnit.U * 0.5f, GlobalUnit.U * 0.5F, GlobalUnit.Ib * 0.02F, GlobalUnit.Ib * 0.02F, GlobalUnit.Ib * 0.02F, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                PowerOn();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
              
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取掉电事件产生后掉电总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03110000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            //上1次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次掉电发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03110101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次掉电发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03110104", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);
            //上7次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次掉电发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03110107", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);
            //上10次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次掉电发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0311010A", 6);
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
                    reasonS[i] = "掉电次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }


    }
}
