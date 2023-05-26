using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Windows.Forms;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程切换本地模式（辅助功能）
    /// </summary>
    public   class ChangeLocalMode_Fz: VerifyBase
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

        public ChangeLocalMode_Fz(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "费控模式(切换前)", "费控模式(切换后)", "结论" };
            return true;
        }

        public override void Verify()
        {


            try
            {


                
                base.Verify();
                if (Stop) return;
                PowerOn();

                string[] strRand1 = new string[BwCount];//随机数
                string[] strRand2 = new string[BwCount];//随机数
                string[] strEsamNo = new string[BwCount];//Esam序列号
                string[] strRevData = new string[BwCount];
                string[] strOutMac1 = new string[BwCount];
                string[] strOutMac2 = new string[BwCount];
                string[] strRevCode = new string[BwCount];
                int[] iFlag = new int[BwCount];
                bool[] result = new bool[BwCount];

                string[] MyStatus = new string[BwCount];
                string[] FkStatus = new string[BwCount];

                int iSelectBwCount = 0;
                bool[] rstTmp = new bool[BwCount];
                string[] strRevMac = new string[BwCount];
                string[] strData = new string[BwCount];

                #region 准备工作
                ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);
                #endregion

                //读取切换之前的模式

                string[] FkStatusQ = new string[BwCount];

                if (Stop) return;
                Common.Memset(ref strRevCode, "DF010001002D0001");
                MessageController.Instance.AddMessage("正在读取切换前费控模式状态字");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out FkStatusQ, out strRevMac);

                iSelectBwCount = 0;
                Common.Memset(ref rstTmp, false);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    iSelectBwCount++;
                    if (FkStatusQ[i] == "01") rstTmp[i] = true;
                }

                if (Array.IndexOf(rstTmp, true) > -1)
                {
                    Common.Memset(ref strData, "00" + "00000000" + "00000000");

                    if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                    {
                        for (int i = 0; i < BwCount; i++)
                        {
                            if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                            if (Stop) return;
                            if (rstTmp[i])
                            {
                                MessageController.Instance.AddMessage("正在对第" + (i + 1) + "块表下发模式切为本地模式命令,请稍候....");
                                bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlag[i], strRand2[i], strData[i]);
                            }
                        }
                    }
                    else
                    {
                        if (Stop) return;
                        MessageController.Instance.AddMessage("下发模式切为本地模式命令,请稍候....");
                        result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);
                        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    }
                }


                //
                //MessageBox.Show("请检查电表保电、报警和拉闸状态、以及保电状态!");

                string[] FkStatusH = new string[BwCount];
                if (Stop) return;
                Common.Memset(ref strRevCode, "DF010001002D0001");
                MessageController.Instance.AddMessage("正在读取切换后费控模式状态字");
                result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand1, strRevCode, out FkStatusH, out strRevMac);



                //处理结论
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                    ResultDictionary["费控模式(切换前)"][i] = FkStatusQ[i].Equals("00") ? "本地" : "远程";
                    ResultDictionary["费控模式(切换后)"][i] = FkStatusH[i].Equals("01") ? "远程" : "本地";

                    if (ResultDictionary["费控模式(切换后)"][i].Equals("本地"))
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }

                }
                //通知界面
                UploadTestResult("费控模式(切换前)");
                UploadTestResult("费控模式(切换后)");
                UploadTestResult("结论");
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
