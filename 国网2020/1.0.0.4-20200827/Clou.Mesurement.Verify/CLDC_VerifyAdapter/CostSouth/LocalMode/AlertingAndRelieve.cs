using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 远程报警及解除
    /// </summary>
    public class AlertingAndRelieve : VerifyBase
    {
        protected override string ItemKey
        {
           // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get{return null;}
        }

        public AlertingAndRelieve(object plan)
            : base(plan)
        {
        }

        //预拉闸报警状态位前-后1|远程报警1|预拉闸报警状态位前-后2|远程报警解除2


        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "预拉闸报警状态位前一后1", "远程报警1", "预拉闸报警状态位前一后2", "远程报警解除2", "结论" };
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
            string[] strData = new string[BwCount];//明文
            string strDateTime = "";
            int[] iFlag = new int[BwCount];
            string[] strCode = new string[BwCount];
            string[] status3 = new string[BwCount];
            string[] statusTmp = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 2];
            bool[] result = new bool[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            

            #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

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
            MessageController.Instance.AddMessage("正在进行设置报警金额1为0,请稍候....");
            Common.Memset(ref strCode, "04001001");
            Common.Memset(ref strData, "00000000");
            Common.Memset(ref strPutApdu, "04D6811008");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2为0,请稍候....");
            Common.Memset(ref strCode, "04001002");
            Common.Memset(ref strData, ("00000000"));
            Common.Memset(ref strPutApdu, "04D6811408");
            result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strCode, strData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "000186A0");
            bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行远程开户并充值100元,请稍候....");
            Common.Memset(ref strData, "00002710" + "00000001" + "112233445566");
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            #endregion

            #region 步骤1

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
           string[] status3Q = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(status3Q[i]))
                    {
                        if ((Convert.ToInt32(status3Q[i], 16) & 0x0080) == 0x0080)
                        {
                            statusTmp[i] = "有";
                        }
                        else
                        {
                            statusTmp[i] = "无";
                        }
                    }
                    else
                    {
                        statusTmp[i] = "异常";
                    }
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
            strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "2A00" + strDateTime);
            bool[] blnBjRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            string[] strStatus2Q = new string[BwCount];
            
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            statusTmp[i] += "-有";
                            strStatus2Q[i] = "有";
                            blnRet[i, 0] = true;
                        }
                        else
                        {
                            strStatus2Q[i] = "无";
                            statusTmp[i] += "-无";
                        }
                    }
                    else
                    {
                        strStatus2Q[i] = "异常";
                        statusTmp[i] += "-异常";
                    }
                    ResultDictionary["远程报警1"][i] = blnRet[i, 0] ? "通过" : "不通过";
                }
            }
            UploadTestResult("远程报警1");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "预拉闸报警状态位前一后1", statusTmp);
            #endregion
            //预拉闸报警状态位前-后|远程报警|预拉闸报警状态位前-后|远程报警解除

            #region 步骤2
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程报警解除命令,请稍候....");
            strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "2B00" + strDateTime);
            blnBjRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(status3[i]))
                    {
                        if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                        {
                            statusTmp[i] = strStatus2Q[i] + "-有";
                        }
                        else
                        {
                            statusTmp[i] = strStatus2Q[i] + "-无";
                            blnRet[i, 1] = true;
                        }
                    }
                    else
                    {
                        statusTmp[i] = strStatus2Q[i] + "-异常";
                    }
                    ResultDictionary["远程报警解除2"][i] = blnRet[i, 1] ? "通过" : "不通过";
                }
            }
            UploadTestResult("远程报警解除2");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "预拉闸报警状态位前一后2", statusTmp);
            #endregion

            #region 判断结论
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i,0]  &&  blnRet[i,1])
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
        }
            #endregion
    }
}