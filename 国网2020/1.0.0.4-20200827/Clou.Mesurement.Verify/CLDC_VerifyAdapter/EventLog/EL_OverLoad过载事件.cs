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
    /// 过载事件记录
    /// </summary>
    class EL_OverLoad:EventLogBase
    {
        //设置过载阀值
        //读取过载事件产生前过载总次数
        //让电能表过载以便形成断相记录
        public EL_OverLoad(object plan)
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
            string strEventCount = "", strEventTime1 = "", strEventTime4 = "", strEventTime7 = "", strEventTimeA = "";
            if (!string.IsNullOrEmpty(VerifyPara))
            {
                if (VerifyPara.ToUpper() == "A")
                {
                    strEventCount = "1C010001";
                    strEventTime1 = "1C010101";
                    strEventTime4 = "1C010104";
                    strEventTime7 = "1C010107";
                    strEventTimeA = "1C01010A";

                }
                else if (VerifyPara.ToUpper() == "B")
                {
                    strEventCount = "1C020001";
                    strEventTime1 = "1C020101";
                    strEventTime4 = "1C020104";
                    strEventTime7 = "1C020107";
                    strEventTimeA = "1C02010A";
                }
                else if (VerifyPara.ToUpper() == "C")
                {
                    strEventCount = "1C030001";
                    strEventTime1 = "1C030101";
                    strEventTime4 = "1C030104";
                    strEventTime7 = "1C030107";
                    strEventTimeA = "1C03010A";
                }
            }
            //身份认证
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            string[] strDataCode = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strFaZhi = new string[BwCount];//误差超限阀值


            int[] iFlag = new int[BwCount];

       
            
            
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            //设置过载阀值
            Common.Memset(ref strDataCode, "04090B01");
            Common.Memset(ref strData, "04090B01" + "001000");//Imax 0.1倍
            bool[] bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

            string[] str04090D01 = MeterProtocolAdapter.Instance.ReadData("04090B01", 3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取过载事件产生前过载总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表过载以便形成断相记录");
            if (!string.IsNullOrEmpty(VerifyPara))
            {
                if (VerifyPara.ToUpper() == "A")
                {
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Imax * 0.15f, GlobalUnit.Itr, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)Cus_PowerFangXiang.正向有功);
                }
                else if (VerifyPara.ToUpper() == "B")
                {
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Imax * 0.15f, GlobalUnit.Itr, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)Cus_PowerFangXiang.正向有功);
                }
                else if (VerifyPara.ToUpper() == "C")
                {
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr, GlobalUnit.Itr, GlobalUnit.Imax * 0.15f, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)Cus_PowerFangXiang.正向有功);
                }
            }
                       ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 63);
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取过载事件产生后过载总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            //上1次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次过载发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData(strEventTime1, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次过载发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData(strEventTime4, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次过载发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData(strEventTime7, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次过载发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData(strEventTimeA, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            //恢复原来阀值
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strDataCode, "04090B01");
            Common.Memset(ref strData, "04090B01" + "012000");//Imax 0.1倍
            MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            
           

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
