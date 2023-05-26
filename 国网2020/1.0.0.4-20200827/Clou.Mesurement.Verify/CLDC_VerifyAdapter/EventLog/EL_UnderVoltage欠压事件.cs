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
    /// 欠压事件记录
    /// </summary>
    class EL_UnderVoltage : EventLogBase
    {      
        public EL_UnderVoltage(object plan)
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
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论" };
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
            MessageController.Instance.AddMessage("正在读取电表欠压记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("11010001", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表欠压以便形成欠压记录");
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * 0.75F, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib * 0.06f, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false , (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 100);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表欠压记录产生后欠压记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("11010001", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电表欠压记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("11010101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            if (Stop) return;
  


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }
            UploadTestResult("结论");
        }



    }
}