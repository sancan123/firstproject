
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 管理单元清零事件永久记录
    /// </summary>
    class EL_ManagerClearEnergyForever : EventLogBase
    {
        public EL_ManagerClearEnergyForever(object plan)
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

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取管理单元清零事件永久记录产生前清零总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行电量清零...");
            MeterProtocolAdapter.Instance.ClearEnergy();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行事件清零...");
            MeterProtocolAdapter.Instance.ClearEventLog("FFFFFFFF");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取 管理单元清零事件永久记录产生后清零总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次清零时发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03300101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录发生时刻", strLoseTimeQ);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上1次清零结束时刻");
            //string[] strLoseTimeH = MeterProtocolAdapter.Instance.ReadData("03300101", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录结束时刻", strLoseTimeH);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i].Contains("F") || strLoseCountH[i].Contains("F") || strLoseTimeQ[i].Contains("F"))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
                else if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
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
