using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Multicore
{
 /// <summary>
 /// 过电压
 /// </summary>
    class OverVoltage : DgnBase
    {
        public OverVoltage(object plan)
            : base(plan)
        { }
      
        protected override bool CheckPara()
        {
            ResultNames = new string[] {  "结论" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {


            if (Stop) return;
            base.Verify();
            float f = 1.3F;
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U * f, GlobalUnit.U * f, 0, 0, 0, 0, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U, GlobalUnit.U * f, 0, 0, 0, 0, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, 0, 0, 0, 0, 0, 0, 50, "1.0", true, false, false, 0);
    
            }

            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            PowerOn();
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间...");
            string[] strTime = MeterProtocolAdapter.Instance.ReadData("0400010C", 7);

            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["结论"][i] = !string.IsNullOrEmpty(strTime[i]) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

        }
    }
}
