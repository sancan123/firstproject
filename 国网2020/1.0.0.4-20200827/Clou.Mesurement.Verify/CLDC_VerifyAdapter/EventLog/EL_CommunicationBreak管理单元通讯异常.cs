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
    /// 管理单元通讯异常事件
    /// </summary>
    class EL_CommunicationBreak : EventLogBase
    {
        public EL_CommunicationBreak(object plan)
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
            ResultNames = new string[] {"测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "上4次事件记录发生时刻", "上7次事件记录发生时刻", "上10次事件记录发生时刻", "结论", "不合格原因" };
            return true;
        }

        public override void Verify()
        {
           
            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元通讯异常记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03830000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            MessageController.Instance.AddMessage("正在让管理单元通讯异常准备");
            System.Windows.Forms.MessageBox.Show("请手动更换管理单元为工装模块，更换完成后再点击确定。");
            GlobalUnit.IsErrChkSum = true;
               //设置10次 错误帧校验
            for (int i = 0; i < 10; i++)
            {
                MessageController.Instance.AddMessage("正在发送第" + (i + 1) + "次错误帧校验");
                string[] address = MeterProtocolAdapter.Instance.ReadAddress();
            }

            GlobalUnit.IsErrChkSum = false;
            MessageController.Instance.AddMessage("正在让管理单元通讯正常");
            System.Windows.Forms.MessageBox.Show("请手动更换管理单元为正常模块，更换完成后再点击确定。");
            //MessageController.Instance.AddMessage("正在让管理单元通讯异常以便形成管理单元通讯异常记录");
            //System.Windows.Forms.MessageBox.Show("请手动拔出管理单元后点击确定。");
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 100);
            //System.Windows.Forms.MessageBox.Show("请手动装回管理单元后点击确定。");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元通讯异常后产生管理单元通讯异常记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03830000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次管理单元通讯异常记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03830001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);


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
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            }
            UploadTestResult("结论");
        }
    }
}


