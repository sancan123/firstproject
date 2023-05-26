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
   
     class CombinationError : DgnBase
    {
        public CombinationError(object plan)
            : base(plan)
        { }
        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            base.Verify();
            bool bPowerOn = PowerOn();
            string[] arrStrResultKey = new string[BwCount];
            MessageController.Instance.AddMessage("正在进行通信测试...");
            string[] address = MeterProtocolAdapter.Instance.ReadAddress();
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", address);
            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(address[i]))
                {
                    Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr = address[i];
                }
                ResultDictionary["结论"][i] = (string.IsNullOrEmpty(address[i]) == false) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            Adapter.Instance.UpdateMeterProtocol();
            ReadMeterNo();
        }
    }
}


