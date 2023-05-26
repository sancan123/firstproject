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
    /// （管）EL_VoltagePhaseImbalance（管）电压负序三相不平衡记录，（管）电三相不平衡记录
    /// </summary>
    class EL_VoltagePhaseImbalance:EventLogBase
    {
        public EL_VoltagePhaseImbalance(object plan)
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
            MessageController.Instance.AddMessage("正在读取电压负序三相不平衡记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03B90000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            
            //判断第一次读会次数是否有空，如果为空直接不处理，判断不合格
            if (Stop) return;
            MessageController.Instance.AddMessage("正在处理结果");
            int iCheckCount = 0, iFailCount = 0;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iCheckCount++;//检查表的个数

                if (strLoseCountQ[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "第一次读取电压负序三相不平衡记录总次数返回次数值为空";
                    iFailCount++;//检查不合格表的次数
                    continue;
                }

            }

            if (iFailCount == iCheckCount)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
                UploadTestResult("结论");
                return;
            }
              //设置10次校时
            Random rand = new Random();
           int randDays= rand.Next(100);
           for (int it = 0; it < 1; it++)
           {
               if (Stop) return;

               MessageController.Instance.AddMessage("正在进行第" + (it + 1) + "次产生电压负序三相不平衡记录...");

               CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, 0, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
               CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
           }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电压负序三相不平衡记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03B90000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            //上1次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电流不平衡发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03B90001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次失压发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03B90004", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次失压发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03B90007", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次失压发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("03B9000A", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            //日期设置会现在             
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));


            if (Stop) return;
        

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
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
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

            UploadTestResult("结论");
        }
    }
}

