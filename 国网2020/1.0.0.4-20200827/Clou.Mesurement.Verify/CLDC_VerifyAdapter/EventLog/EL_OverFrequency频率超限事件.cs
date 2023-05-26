using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 频率超限事件
    /// </summary>
    class EL_OverFrequency:EventLogBase
    {
        public EL_OverFrequency(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论", "不合格原因" };
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

            VerifyPara = "上限";
            string strEventCount = "", strEventTime1 = "";

            float f = 53;
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置频率阈值上限");
            Common.Memset(ref strDataCode, "04093501");
            Common.Memset(ref strData, "04093501" + "5000");
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置频率超上限判定延时时间");
            Common.Memset(ref strDataCode, "04093502");
            Common.Memset(ref strData, "04093502" + "10");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置频率阈值下限");
            Common.Memset(ref strDataCode, "04093601");
            Common.Memset(ref strData, "04093601" + "5000");
          bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在设置频率超下限判定延时时间");
            Common.Memset(ref strDataCode, "04093602");
            Common.Memset(ref strData, "04093602" + "10");
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            if (!string.IsNullOrEmpty(VerifyPara))
            {
                if (VerifyPara.ToUpper() == "上限")
                {
                    strEventCount = "03B30000";
                    strEventTime1 = "03B30001";
                    f = 53;
                }
                else//下限
                {
                    strEventCount = "03B40000";
                    strEventTime1 = "03B40001";
                    f = 49;
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在频率超限总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            for (int it = 0; it < 1; it++)
            {
                if (Stop) return;

                MessageController.Instance.AddMessage("正在进行第" + (it + 1) + "次让频率超限...");

                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, f, "1.0", true, false, false, (int)Cus_PowerFangXiang.正向有功);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
                string[] strLoseCountQ2 = MeterProtocolAdapter.Instance.ReadData("02800002", 3);
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取频率超限总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次频率超限发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData(strEventTime1, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "")
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
