
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 蓝牙连接测试类
    /// </summary>
    class Dgn_BlueToothTest : DgnBase
    {
        public Dgn_BlueToothTest(object plan)
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

            string[] address_MAC = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                address_MAC[i] = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr_MAC;
            }
            

            MessageController.Instance.AddMessage("正在进行蓝牙连接...");
            bool[] bResult = MeterProtocolAdapter.Instance.ConnectBlueTooth(address_MAC);

            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["检定数据"][i] = bResult[i].ToString();
                if (bResult[i])
                {
                  
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                  
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", ResultDictionary["检定数据"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

           
            
        }
    }
}
/*===========================================================================================================*/
