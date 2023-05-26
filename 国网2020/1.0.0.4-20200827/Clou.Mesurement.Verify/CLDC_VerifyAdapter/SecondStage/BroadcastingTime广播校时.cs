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
   
    class BroadcastingTime : DgnBase
    {
        public BroadcastingTime(object plan)
            : base(plan)
        { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "事件记录发生时刻", "事件结束时刻", "结论" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            bool[] bResult = new bool[BwCount];

            
            base.Verify();
            bool bPowerOn = PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间为当前时间早2分钟");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.AddMinutes(-2).ToString("yyMMddHHmmss"));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取下发广播校时命令前总次数");
            string[] strTimeCountQ = MeterProtocolAdapter.Instance.ReadData("032F0400",3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strTimeCountQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取下发广播校时命令前广播校时记录");
            string[] strTimeLogQ = MeterProtocolAdapter.Instance.ReadData("032F0401",12);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录发生时刻", strTimeLogQ);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发广播校时命令");
            MeterProtocolAdapter.Instance.BroadCastTime(DateTime.Now);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 65);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取下发广播校时命令后总次数");
            string[] strTimeCountH = MeterProtocolAdapter.Instance.ReadData("032F0400",3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strTimeCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取下发广播校时命令前广播校时记录",12);
            string[] strTimeLogH = MeterProtocolAdapter.Instance.ReadData("032F0401");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件结束时刻", strTimeLogH);


            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strTimeCountQ[i]) || string.IsNullOrEmpty(strTimeCountH[i])) continue;
                ResultDictionary["结论"][i] = Convert.ToInt32(strTimeCountQ[i] + 1) == Convert.ToInt32(strTimeCountH[i]) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            //日期设置为现在   
            MessageController.Instance.AddMessage("正在设置日期为现在...");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
        }
    }
}
