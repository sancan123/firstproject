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
 
    class PowerSupplyMode : DgnBase
    {
        public PowerSupplyMode(object plan)
            : base(plan)
        { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "临界电压A相的时间", "临界电压B相的时间", "临界电压C相的时间", "结论" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {


            if (Stop) return;
            base.Verify();

            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U * 0.59F, GlobalUnit.U * 0.59F, 0, 0, 0, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U * 0.59F, 0, 0, 0,1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                MessageController.Instance.AddMessage("正单相表无需测试此实验。...", 6, 1);
                return;
            }

            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间...");
            string[] strTime = MeterProtocolAdapter.Instance.ReadData("0400010C", 7);

            //临界电压的数据标识？？？
            //if (Stop) return;
            //MessageController.Instance.AddMessage("临界电压A相的时间...");
            //string[] strTimeA = MeterProtocolAdapter.Instance.ReadData("");

            //if (Stop) return;
            //MessageController.Instance.AddMessage("临界电压B相的时间...");
            //string[] strTimeB = MeterProtocolAdapter.Instance.ReadData("");

            //if (Stop) return;
            //MessageController.Instance.AddMessage("临界电压C相的时间...");
            //string[] strTimeC = MeterProtocolAdapter.Instance.ReadData("");

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "临界电压A相的时间", strTime);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "临界电压B相的时间", strTime);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "临界电压C相的时间", strTime);
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