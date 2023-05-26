using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    //钱包初始化(辅助功能)
    public class PurseInitizalition_Fz : VerifyBase
    {
        public PurseInitizalition_Fz(object plan)
            : base(plan)
        {
        }


        //购电次数	读取值
        //剩余金额	读取值
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            ResultNames = new string[] { "钱包初始化","购电次数","剩余金额", "结论" };
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

            //读取表地址
            string[] strRand1 = new string[BwCount];
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] strData = new string[BwCount];
            string[] MyStatus = new string[BwCount];
            int iSelectBwCount = 0;
            bool[] rstTmp = new bool[BwCount];
            string[] strRevCode = new string[BwCount];

            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];

            string[] BuyCount = new string[BwCount];

            string[] strSyMoney = new string[BwCount]; //当前剩余金额

            #region 步骤1 发送钱包初始化命令。

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            //步骤1-----------------------
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行钱包初始化,请稍候....");
            Common.Memset(ref strData, "00004E20");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["钱包初始化"][i] = result[i] ? "成功" : "失败";
            }
            UploadTestResult("钱包初始化");
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out BuyCount, out strSyMoney);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额", strSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电次数", BuyCount);


            #endregion


            #region 处理结论
            //处理结论
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (result[i])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            //通知界面
            UploadTestResult("结论");
            #endregion
        }

        protected override string ItemKey
        {
            get
            {
                return null;
            }
        }

        protected override string ResultKey
        {
            get
            {
                return null;
            }
        }
    }
}
