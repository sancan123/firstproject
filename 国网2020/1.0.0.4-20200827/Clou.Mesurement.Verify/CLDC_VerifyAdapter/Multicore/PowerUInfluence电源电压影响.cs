using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Multicore
{
    /// <summary>
    /// 2.2 电源电压影响试验
    /// </summary>
    class PowerUInfluence : DgnBase
    {
        public PowerUInfluence(object plan)
            : base(plan)
        { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "中断电压前时间", "中断电压后时间","不合格原因", "结论" };
            return true;
        }

        /// <summary>
        /// 测试
        /// </summary>
        public override void Verify()
        {
            bool[] bResult = new bool[BwCount];

            
            base.Verify();
            bool bPowerOn = PowerOn();
            ShowWaitMessage("正在等待源稳定{0}秒,请稍候....", 1000 * 5);

             if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间为当前时间");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
           
           // float[] uList = new float[] { 0.5F };//电压倍数
            if (GlobalUnit.IsDemo) return ; 

            if (Stop) return;
            DateTime dt = DateTime.Now;                   
            MessageController.Instance.AddMessage("正在读取电表时间...");
            string[] strTime = MeterProtocolAdapter.Instance.ReadData("0400010C", 7);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "中断电压前时间", strTime);

             float f = 0.5F;
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U * f, GlobalUnit.U * f, 0, 0, 0, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f,0, GlobalUnit.U * f, 0, 0, 0,1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                MessageController.Instance.AddMessage("正单相表无需测试此实验。...", 6, 1);
                return;
            }
            ShowWaitMessage("等源稳定", 1000*60);//等待1分钟

            if (Stop) return;     
            string[] strTimeL = MeterProtocolAdapter.Instance.ReadData("0400010C", 7);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "中断电压后时间", strTimeL);
            //MessageController.Instance.AddMessage("第"+i+"次，正在读取电表时间,请稍候....");
            //DateTime[] dateTimeQ = MeterProtocolAdapter.Instance.ReadDateTime();
            string[] strBuHeGeReason = new string[BwCount];
             f = 1.0f;
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U * f, GlobalUnit.U * f, 0, 0, 0, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, 0, GlobalUnit.U * f, 0, 0, 0, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                MessageController.Instance.AddMessage("正单相表无需测试此实验。...", 6, 1);
                return;
            }
           
            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //计算2个时间差，需要调试
                int time;
                try
                {
               
                    time = DateTime.ParseExact(strTimeL[i].Substring(0, 6) + strTimeL[i].Substring(8, 6), "yyMMddHHmmss", CultureInfo.InvariantCulture).CompareTo(DateTime.ParseExact(strTime[i].Substring(0, 6) + strTime[i].Substring(8, 6), "yyMMddHHmmss", CultureInfo.InvariantCulture));
                     if (time >= 57 && time <= 63)
                     {
                         ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                     }
                     else
                     {
                         ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                         strBuHeGeReason[i] = "中断前后，电表时钟误差超差";
                     }
                }
                catch (Exception e)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    strBuHeGeReason[i] = "电表时钟日期格式有误";
                }
                 
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", strBuHeGeReason);
            UploadTestResult("结论");
        }



    }
}
