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
    /// 负荷开关误动作,目前无法实现
    /// </summary>
    class EL_MisoperationOfLoadSwitch: EventLogBase
    {
        public EL_MisoperationOfLoadSwitch(object plan)
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
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "事件记录发生时刻", "结论" };
            return true;
        }

        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取负荷开关误动作记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03810000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在形成负荷开关误动作记录");
         //   System.Windows.Forms.MessageBox.Show("请手动更换管理单元后点击确定。");
        //    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 100);
         //   System.Windows.Forms.MessageBox.Show("请手动装回电能表盖后点击确定。");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取负荷开关误动作记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03810000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次负荷开关误动作记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03810001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录发生时刻", strLoseTimeQ);


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


