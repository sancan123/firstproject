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
    /// 管理单元更换事件
    /// </summary>
    class EL_ChangePart : EventLogBase
    {
        public EL_ChangePart(object plan)
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

        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元更换记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03810000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在让管理单元更换以便形成管理单元更换记录");
            System.Windows.Forms.MessageBox.Show("请手动更换管理单元后点击确定。");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            System.Windows.Forms.MessageBox.Show("请手动装回电能表盖后点击确定。");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元更换后产生管理单元更换记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03810000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            //上1次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次管理单元更换记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03810001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //  //上4次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上4次管理单元更换记录发生时刻");
            //string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03810004", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);
            ////上7次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上7次管理单元更换记录发生时刻");
            //string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03810007", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);
            ////上10次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上10次管理单元更换记录发生时刻");
            //string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0381000A", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == ""  )
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
                
                    reasonS[i] += "次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            
            UploadTestResult("结论");
        }
    }
}

