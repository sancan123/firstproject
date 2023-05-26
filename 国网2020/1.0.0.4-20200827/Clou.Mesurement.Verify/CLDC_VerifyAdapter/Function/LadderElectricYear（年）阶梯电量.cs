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
    /// （年）阶梯电量
    /// </summary>
    public class LadderElectricYear : VerifyBase
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
            ResultNames = new string[] { "测试时间",
                                         //"年第一结算日","切换前当前年结算周期组合有功总累计用电量","切换后当前年结算周期组合有功总累计用电量",
                                         "结论","不合格原因" };
            return true;
        }

        string strPlan = "";

        public LadderElectricYear(object plan)
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
            MessageController.Instance.AddMessage("正在设置时间为当前时间");
            string Time1 = DateTime.Now.ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(Time1);
            #region 准备步骤

            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
            Common.Memset(ref strID, "04060AFF");

            Common.Memset(ref strData, "00000010" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                       + "00010000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                       + "010101" + "010101" + "010101" + "010101" + "010101" + "010101");
            Common.Memset(ref strPutApdu, "04D684344A");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);


            
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
            Common.Memset(ref strPutApdu, "04D684C009");
            result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

            //走字
            if (Stop) return;
            MessageController.Instance.AddMessage("正在走字20秒,请稍候....");
            bool blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
            PowerOn();

         
            //读当前阶梯
             if (Stop) return;
            MessageController.Instance.AddMessage("正在读当前阶梯,请稍候....");
            string[] strJt = MeterProtocolAdapter.Instance.ReadData("02800034", 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读正向有功总电能,请稍候....");
            string[] strZG = MeterProtocolAdapter.Instance.ReadData("000D0000", 8);
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时,请稍候....");
     
            Time1 = DateTime.Now.AddYears(1).ToString("yy") + "0101235950";
            result = MeterProtocolAdapter.Instance.WriteDateTime(Time1);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *50);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读正向有功总电能,请稍候....");
            string[] strZG2 = MeterProtocolAdapter.Instance.ReadData("000D0000",8 );
            #endregion


            MessageController.Instance.AddMessage("正在设置时间为当前时间");
            Time1 = DateTime.Now.ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(Time1);
            //1-------------



            MessageController.Instance.AddMessage("正在计算结果,请稍候....");
            try
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(strZG2[i]) && !string.IsNullOrEmpty(strZG[i]))
                    {
                        if (strZG2[i] != strZG[i])
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
