
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
    /// 计-电量清零事件记录，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_ClearEnergyJ : EventLogBase
    {
        public EL_ClearEnergyJ(object plan)
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


            //设置1次校时
            for (int it = 0; it < 1; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表校时");
                //  DateTime time = DateTime.Now
                MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生前清零总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            float f=0.3F;

            //产生电量，给清零做铺垫
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Imax * f, GlobalUnit.Imax * f, GlobalUnit.Imax * f, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, GlobalUnit.U, GlobalUnit.Imax * f, 0, GlobalUnit.Imax * f, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Imax * f, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

            }

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            PowerOn();
            //设置10次清零
            for (int i = 0; i < 1;i++ )
            {
              

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行第" + (i + 1) + "次清零...");
              
             
                MeterProtocolAdapter.Instance.ClearEnergy();
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);
              
            }
          

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生后清零总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            //上一次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次清零时发生时刻");
            string[] strLoseTimeQ1 = MeterProtocolAdapter.Instance.ReadData("03300101", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ1);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次清零时发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03300104", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次清零时发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03300104", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次清零时发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330010A", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            //读取校时记录次数，判断是否被清空
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时事件总次数");
            string[] strTimeCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            //读取电量是否为空
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strTimeCountQ[i]!="000000")
                 {
                     ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                     reasonS[i] = "校时记录没被清空";
                     continue;
                 }
                if (flt_DL[i] != 0)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "电量没被清空";
                    continue;
                }
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ1[i] == "" || strLoseTimeQ4[i] == "" || strLoseTimeQ7[i] == "" || strLoseTimeQ10[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值带F值";
                    continue;
                }
                if (strLoseCountQ[i].Contains("F") || strLoseCountH[i].Contains("F") || strLoseTimeQ1[i].Contains("F") || strLoseTimeQ4[i].Contains("F") || strLoseTimeQ7[i].Contains("F") || strLoseTimeQ10[i].Contains("F"))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值带F值";
                    continue;
                }

                else if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
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
