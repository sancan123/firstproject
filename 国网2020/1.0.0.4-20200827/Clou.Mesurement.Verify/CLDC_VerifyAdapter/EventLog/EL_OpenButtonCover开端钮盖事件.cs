using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore;
namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 开端钮盖事件记录，单相没有

    /// </summary>
    class EL_OpenButtonCover : EventLogBase
    {
        public EL_OpenButtonCover(object plan)
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
            MessageController.Instance.AddMessage("正在读取电表开端钮盖记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300E00", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            
             //设置10次校时
           // for (int it = 0; it < 10; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表打开端钮盖以便形成开端钮盖记录");
                System.Windows.Forms.MessageBox.Show("请手动打出电能表盖后点击确定。");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
                System.Windows.Forms.MessageBox.Show("请手动装回电能表端钮盖后点击确定。");
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表开端钮盖后产生开端钮盖记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300E00", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            //上一次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电表开端钮盖记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03300E01", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            ////上4次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上4次电表开端钮盖记录发生时刻");
            //string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03300E04", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);
            ////上7次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上7次电表开端钮盖记录发生时刻");
            //string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03300E07", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);
            ////上10次
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上10次电表开端钮盖记录发生时刻");
            //string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("03300E0A", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
              //  if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" || strLoseTimeQ4[i] == "" || strLoseTimeQ7[i] == "" || strLoseTimeQ10[i] == "")
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
                    reasonS[i] = "校时次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}
