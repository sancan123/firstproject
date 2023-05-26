using CLDC_DataCore;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using System;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 防伪造卡攻击
    /// </summary>
    public class CounterfeitCardAttack : VerifyBase
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

        public CounterfeitCardAttack(object plan)
            : base(plan)
        {
        }


        //非法插卡总次数(攻击前)|非法插卡总次数(攻击后)|防伪造卡攻击

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "非法插卡总次数（攻击前）", "非法插卡总次数（攻击后）","防伪造卡攻击", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            bool[] result = new bool[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[,] blnRet = new bool[BwCount, 1];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
            string[] strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

            MessageBox.Show("请把非法卡插入表后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取非法插卡总次数,请稍候....");
            string[] strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (string.IsNullOrEmpty(strFfckCountQ[i]))
                {
                    ResultDictionary["非法插卡总次数（攻击前）"][i] = "异常";
                }
                else
                {
                    ResultDictionary["非法插卡总次数（攻击前）"][i] = strFfckCountQ[i];
                }
                if (string.IsNullOrEmpty(strFfckCountH[i]))
                {
                    ResultDictionary["非法插卡总次数（攻击后）"][i] = "异常";
                }
                else
                {
                    ResultDictionary["非法插卡总次数（攻击后）"][i] = strFfckCountH[i];
                }
                if (!string.IsNullOrEmpty(strFfckCountQ[i]) && !string.IsNullOrEmpty(strFfckCountH[i]) && Convert.ToInt32(strFfckCountQ[i]) + 1 == Convert.ToInt32(strFfckCountH[i]))
                {
                    blnRet[i, 0] = true;
                }

            }
            UploadTestResult("非法插卡总次数（攻击前）");
            UploadTestResult("非法插卡总次数（攻击后）");

            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i,0])
                    {
                        ResultDictionary["防伪造卡攻击"][i] = "通过";
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["防伪造卡攻击"][i] = "不通过";
                    }
                }
            }
            UploadTestResult("防伪造卡攻击");
            //通知界面
            UploadTestResult("结论");

        }
    }
}
