using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Function
{
 
    /// <summary>
    /// 自保电
    /// </summary>
    class  AutoEnPower : VerifyBase
    {
        public AutoEnPower(object plan)
            : base(plan)
        { }

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

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "继电器状态位前一后","结论" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
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
            Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            string[] status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                    {
                        ResultDictionary["继电器状态位前一后"][i] = "断";
                    }
                    else
                    {
                        ResultDictionary["继电器状态位前一后"][i] = "通";
                    }
                }
                else
                {
                    ResultDictionary["继电器状态位前一后"][i] = "异常";
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发跳闸心跳帧,请稍候....");
            result = MeterProtocolAdapter.Instance.SetBreakRelayTime(6);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 25);
            System.Windows.Forms.MessageBox.Show("请手动按键合闸后点击确定进入下一流程。");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                    {
                        ResultDictionary["继电器状态位前一后"][i] += "-断";
                    }
                    else
                    {
                        ResultDictionary["继电器状态位前一后"][i] += "-通";
                    }
                }
                else
                {
                    ResultDictionary["继电器状态位前一后"][i] += "-异常";
                }
            }
            UploadTestResult("继电器状态位前一后");

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
            Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
            Common.Memset(ref strData, "1C00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
            bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            //ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            MessageController.Instance.AddMessage("正在处理结果,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (ResultDictionary["继电器状态位前一后"][i] == "断-通")
                {
                    ResultDictionary["结论"][i] = "合格";
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                }

            }
            UploadTestResult("结论");

        }
    }
}
