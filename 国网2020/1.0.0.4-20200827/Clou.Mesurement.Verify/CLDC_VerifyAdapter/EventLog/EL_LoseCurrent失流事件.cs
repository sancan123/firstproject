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
    /// 失流事件记录，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_LoseCurrent : EventLogBase
    {
        public EL_LoseCurrent(object plan)
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

            //日期设置会现在             
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

            if (Stop) return;
            string strEventCount = "", strEventTime1 = "", strEventTime4 = "", strEventTime7 = "", strEventTimeA = "";
            if (!string.IsNullOrEmpty(VerifyPara))
            {
                if (VerifyPara.ToUpper() == "A")
                {
                    strEventCount = "18010001";
                    strEventTime1 = "18010101";
                    strEventTime4 = "18010104";
                    strEventTime7 = "18010107";
                    strEventTimeA = "1801010A";

                }
                else if (VerifyPara.ToUpper() == "B")
                {
                    strEventCount = "18020001";
                    strEventTime1 = "18020101";
                    strEventTime4 = "18020104";
                    strEventTime7 = "18020107";
                    strEventTimeA = "1802010A";
                }
                else if (VerifyPara.ToUpper() == "C")
                {
                    strEventCount = "18030001";
                    strEventTime1 = "18030101";
                    strEventTime4 = "18030104";
                    strEventTime7 = "18030107";
                    strEventTimeA = "1803010A";
                }
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取失流事件产生前失流总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            //判断第一次读会次数是否有空，如果为空直接不处理，判断不合格
            if (Stop) return;
            MessageController.Instance.AddMessage("正在处理结果");
            int iCheckCount = 0,iFailCount=0;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iCheckCount++;//检查表的个数

                if (strLoseCountQ[i] == "" )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回次数值为空";
                    iFailCount++;//检查不合格表的次数
                    continue;
                }
               
            }
          
            if(iFailCount==iCheckCount)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
                UploadTestResult("结论");
                return;
            }

             //设置10次校时
            Random rand = new Random();
            int randDays = rand.Next(100);
           for (int it = 0; it < 10; it++)
           {
               if (Stop) return;

               //string strDt = DateTime.Now.AddHours(it + randDays).ToString("yyMMddHHmmss");
               //MeterProtocolAdapter.Instance.WriteDateTime(strDt);
               MessageController.Instance.AddMessage("正在进行第" + (it + 1) + "次让电能表失流以便形成失流记录...");
               if (!string.IsNullOrEmpty(VerifyPara))
               {
                   if (VerifyPara.ToUpper() == "A")
                   {
                       Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib * 0.004f, GlobalUnit.Ib * 0.2f, GlobalUnit.Ib * 0.2f, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                   }
                   else if (VerifyPara.ToUpper() == "B")
                   {
                       Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib * 0.2f, GlobalUnit.Ib * 0.004f, GlobalUnit.Ib * 0.2f, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                   }
                   else if (VerifyPara.ToUpper() == "C")
                   {
                       Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib * 0.2f, GlobalUnit.Ib * 0.2f, GlobalUnit.Ib * 0.004f, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                   }
               }
                       ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);
                       CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Ib, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 63);
           }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取失流事件产生后失流总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData(strEventCount, 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            //上1次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次失流发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData(strEventTime1, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次失流发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData(strEventTime4, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次失流发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData(strEventTime7, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次失流发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData(strEventTimeA, 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

          

            if (Stop) return;   

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" || strLoseTimeQ4[i] == "" || strLoseTimeQ7[i] == "" || strLoseTimeQ10[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i])&& !string.IsNullOrEmpty(strLoseCountH[i])&&Convert.ToInt32(strLoseCountQ[i]) + 10 == Convert.ToInt32(strLoseCountH[i]))
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