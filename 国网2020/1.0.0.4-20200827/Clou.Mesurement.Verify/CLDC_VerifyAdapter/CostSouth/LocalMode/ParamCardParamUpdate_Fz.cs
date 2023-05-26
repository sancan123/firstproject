using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 预置卡参数更新(辅助功能)
    /// </summary>
    public class ParamCardParamUpdate_Fz : VerifyBase
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

        public ParamCardParamUpdate_Fz(object plan)
            : base(plan)
        {


        }


        protected override bool CheckPara()
        {

            ResultNames = new string[] {  "参数信息文件", "购电信息文件","当前套电价文件","备用套电价文件","插卡状态字","结论" };
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
            string[] paraFile = new string[9]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string strParaFile = ""; //参数信息文件
            string strwalletFile = "";//钱包文件
            string strpriceFile1 = "";//当前套电价文件
            string strpriceFile2 = "";//备用套电价文件
            string[] strYzkDqtfl1 = new string[BwCount];
            string[] strYzkBytfl1 = new string[BwCount];
            string[] strYzkBj1 = new string[BwCount];
            string[] strDqtfl1 = new string[BwCount];
            string[] strBytfl1 = new string[BwCount];
            string[] strBj1 = new string[BwCount];
            string[] strRevData = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            bool[] WriteUserResult = new bool[BwCount];
            bool[] result = new bool[BwCount];
            string[] strRevMac1 = new string[BwCount];
            string[] strRevMac2 = new string[BwCount];
            string[] strGdCountH = new string[BwCount];
            string[] strSyMoneyH = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[] blnRecKeyRet = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            string[] status = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strFfckCountQ = new string[BwCount];
            string[] strFfckCountH = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 10];
            string[] strRedData = new string[BwCount];


            MessageBox.Show("请把参数预置卡插入表后按确定");


            #region 准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);


            //参数信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取安全模块参数信息文件....");
            Common.Memset(ref strRevCode, "DF01000100000030");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "参数信息文件", strRedData);


            //购电信息文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取安全模块购电信息文件....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRevMac1, out strRevMac2, out strGdCountH, out strSyMoneyH);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电信息文件", strRedData);

            //当前套电价文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取当前套电价文件....");
            Common.Memset(ref strRevCode, "DF010003000000C7");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前套电价文件", strRedData);

            //备用套电价文件
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取备用套电价文件....");
            Common.Memset(ref strRevCode, "DF010004000000C7");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strRedData, out strRevMac1);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "备用套电价文件", strRedData);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 0] = true;
                    }
                }
                else
                {
                    status[i] = "异常";
                }
                ResultDictionary["插卡状态字"][i] = status[i];

            }
            UploadTestResult("插卡状态字");


            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (blnRet[i, 0])
                {
                    ResultDictionary["结论"][i] = "合格";
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                }
            }

            //通知界面
            UploadTestResult("结论");

        }
    }
}
