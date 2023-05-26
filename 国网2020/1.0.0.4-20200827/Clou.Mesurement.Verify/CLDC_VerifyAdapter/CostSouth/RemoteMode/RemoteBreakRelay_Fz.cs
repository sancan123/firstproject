using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 拉闸功能(辅助功能)
    /// </summary>
    public class RemoteBreakRelay_Fz : VerifyBase
    {
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public RemoteBreakRelay_Fz(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "跳闸命令应答", "应答后继电器命令状态位", "结论" };
            return true;
        }

        public override void Verify()
        {

            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];
            string[] strDataPut = new string[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 4];
            bool[] result = new bool[BwCount];
            bool[] blnFhkg = new bool[BwCount];
            string[] strHzDate = new string[BwCount];
            int[] iFlag = new int[BwCount];

            //准备
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            if (Stop) return;
            Common.Memset(ref strDataPut, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strDataPut);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                    {
                        ResultDictionary["应答后继电器命令状态位"][i] = "断";
                        blnRet[i, 0] = true;
                    }
                    else
                    {
                        ResultDictionary["应答后继电器命令状态位"][i] = "通";
                    }
                }
                else
                {
                    ResultDictionary["应答后继电器命令状态位"][i] = "异常";
                }
                if (result[i])
                {
                    ResultDictionary["跳闸命令应答"][i] = "正常应答";
                }
                else
                {
                    ResultDictionary["跳闸命令应答"][i] = "异常应答";
                }
                if (result[i] && blnRet[i, 0])
                {
                    blnRet[i, 1] = true;
                }
            }

            UploadTestResult("跳闸命令应答");
            UploadTestResult("应答后继电器命令状态位");
            

            MessageController.Instance.AddMessage("正在处理结果,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 1])
                {
                    ResultDictionary["结论"][i] = "合格";
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                }

            }
            UploadTestResult("结论");

            MessageController.Instance.AddMessage("正在关源倒计时....");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 300);
        }
    }
}
