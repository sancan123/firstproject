using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;
using System.Threading;
using System.Globalization;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 阶梯电量
    /// </summary>
    public class LadderElectric : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "每月第一结算日","切换前当前月度组合有功总累计用电量","切换后当前月度组合有功总累计用电量",
                                         //"年第一结算日","切换前当前年结算周期组合有功总累计用电量","切换后当前年结算周期组合有功总累计用电量",
                                         "结论" };
            return true;
        }

        string strPlan = "";

        public LadderElectric(object plan)
            : base(plan)
        {
            
        }
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strRevData = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strCode = new string[BwCount];

            bool[] rstTmp = new bool[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] status3 = new string[BwCount];
            string[] strID = new string[BwCount];


            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            #region 准备步骤

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取每月第1结算日,请稍候....");
            string[] strJsrTime = MeterProtocolAdapter.Instance.ReadData("04000B01", 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字20秒,请稍候....");
            bool blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前当前月度组合有功总累计用电量,请稍候....");
            string[] strDLQ = MeterProtocolAdapter.Instance.ReadData("000C0000", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "切换前当前月度组合有功总累计用电量", Common.StringConverToDecimaByIb(strDLQ,2));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置每月第1结算日为01日00时,请稍候....");
            Common.Memset(ref strID, "04000B01");
            Common.Memset(ref strCode, "0100");
            Common.Memset(ref strData, "04000B01" + "0100");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "每月第一结算日", strCode);



            #endregion



            //1-------------



            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间到每月结算日前5秒....");
            DateTime dtNextMonth = DateTime.Parse(DateTime.Now.AddMonths(1).ToString("yyyy-MM") + "-01");
            string strTime = dtNextMonth.AddDays(-1).ToString("yyMMdd") + "235955";
            result = MeterProtocolAdapter.Instance.WriteDateTime(strTime);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前当前月度组合有功总累计用电量,请稍候....");
            string[] strDLH = MeterProtocolAdapter.Instance.ReadData("000C0000", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "切换后当前月度组合有功总累计用电量", Common.StringConverToDecimaByIb(strDLH,2));

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在恢复每月第一结算日....");
            Common.Memset(ref strCode, "04000B01");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strData[i] = "04000B01" + strJsrTime[i];
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            MessageController.Instance.AddMessage("正在计算结果,请稍候....");
            try
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strDLQ[i]) && !string.IsNullOrEmpty(strDLH[i]))
                    {
                        if (strDLQ[i] != strDLH[i])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            catch (Exception)
            { }

            UploadTestResult("结论");
        }
    }
}
