using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 交互终端身份认证
    /// </summary>
    public class IdentityAuthentication : VerifyBase
    {
        protected override string ItemKey
        {
            get { return null; }
        }

        protected override string ResultKey
        {
            get { return null; }
        }

        public IdentityAuthentication(object plan)
            : base(plan)
        {
        }

        
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //这里要解析检定参数

            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "随机数2", "ESAM序列号", "操作前身份认证状态", "操作后身份认证状态", "密钥状态", "通过交互终端和表身份认证", "结论" };

            return true;
        }
        /// <summary>
        /// 开始检定业务
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            //只升电压
            PowerOn();
            string[] strRand1 = new string[BwCount];
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] status = new string[BwCount];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            bool[] result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            //string[] status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(status[i])) continue;
            //    if ((Convert.ToInt32(status[i], 16) & 0x2000) == 0x2000)
            //    {
            //        status[i] = "有效";
            //    }
            //    else
            //    {
            //        status[i] = "无效";
            //    }
            //}
            Common.Memset(ref status, "无效");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作前身份认证状态", status);


            MessageController.Instance.AddMessage("正在电表与交互终端身份认证,请稍候....");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckTerminalIdentity(out strRand1,out strRand2, out strEsamNo);

            //读状态字3
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x2000) == 0x2000)
                    {
                        status[i] = "有效";
                    }
                    else
                    {
                        status[i] = "无效";
                    }
                }
                else
                {
                    status[i] = "异常";
                }
            }

            //通知界面
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "随机数2", strRand2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "ESAM序列号", strEsamNo);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作后身份认证状态", status);

            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (iFlag[i] == 0)
                    {
                        ResultDictionary["密钥状态"][i] = "测试密钥";
                    }
                    else if (iFlag[i] == 1)
                    {
                        ResultDictionary["密钥状态"][i] = "正式密钥";
                    }
                    else
                    {
                        ResultDictionary["密钥状态"][i] = "其他";
                    }
                    if (iFlag[i] != 2 && status[i] == "有效")
                    {
                        ResultDictionary["通过交互终端和表身份认证"][i] = "通过";
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["通过交互终端和表身份认证"][i] = "不通过";
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            //通知界面
            UploadTestResult("密钥状态");
            UploadTestResult("通过交互终端和表身份认证");
            UploadTestResult("结论");
            Helper.EquipHelper.Instance.PowerOff();
        }
       
    }
}
