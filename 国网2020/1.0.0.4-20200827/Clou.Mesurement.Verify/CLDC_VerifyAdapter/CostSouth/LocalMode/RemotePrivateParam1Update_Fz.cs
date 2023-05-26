using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 一类参数更新（辅助功能）
    /// </summary>
    public class RemotePrivateParam1Update_Fz : VerifyBase
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

        public RemotePrivateParam1Update_Fz(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "当前密钥1", "测试密钥下不可更新一类参数1", 
                                        "报警金额1限值设置值","报警金额1限值读取值","报警金额2限值设置值","报警金额2限值读取值",
                                        "备用套费率设置值","备用套费率读取值",
                                        "记录编程事件记录","正式密钥下更新一类参数","结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];

            string strChangFsflTime = "";    // 两套分时费率切换时间
            string strBjMoney1 = "";    // 报警金额1
            string strBjMoney2 = "";    // 报警金额2
            string strUbb = "";    // 电压变比
            string strIbb = "";    // 电流变比
            string strSfTime = ""; //身份认证时效性 



            #region 准备步骤
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 0);

            #endregion

            //1-------------------------- 两套分时费率切换时间
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
            Common.Memset(ref strID, "04000108");
            Common.Memset(ref strData, strChangFsflTime);
            Common.Memset(ref strPutApdu, "04D6810A09");
            bool[] blnChangFsflTime = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            //2-----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额1,请稍候....");
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, strBjMoney1);
            Common.Memset(ref strPutApdu, "04D6811008");
            bool[] blnBjMoney1 = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            //3-----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, strBjMoney2);
            Common.Memset(ref strPutApdu, "04D6811408");
            bool[] blnBjMoney2 = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            //4-----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, strIbb);
            Common.Memset(ref strPutApdu, "04D6811807");
            bool[] blnIbb = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            //5----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
            Common.Memset(ref strID, "04000307");
            Common.Memset(ref strData, strUbb);
            Common.Memset(ref strPutApdu, "04D6811B07");
            bool[] blnUbb = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

            //6----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置身份认证时效,请稍候....");
            Common.Memset(ref strID, "070001FF");
            Common.Memset(ref strData, strSfTime);
            Common.Memset(ref strPutApdu, "04D6812B06");
            bool[] blnSfTime = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4])
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
