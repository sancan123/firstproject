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
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth
{
    public class RemoteOneParmsUpdateFz : VerifyBase
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

        public RemoteOneParmsUpdateFz(object plan)
            : base(plan)
        {
        }

        //参数更新|参数名称
        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "参数更新", "参数名称", "结论" };
            return true;
        }

        public override void Verify()
        {
            try
            {
                base.Verify();
                PowerOn();

                #region 准备工作
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
                string[] strID = new string[BwCount];
                string[] strPutApdu = new string[BwCount];
                string[] strErrInfo = new string[BwCount];

                //准备
                if (Stop) return;
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
                Common.Memset(ref strID, "04000108");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D6810A09");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
                Common.Memset(ref strID, "04060AFF");
                //Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000" + "000000000" + "00000000");
                //Common.Memset(ref strPutApdu, "04D684341C");
                Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000"
                          + "000000" + "000000" + "000000" + "000000" + "000000" + "000000");
                Common.Memset(ref strPutApdu, "04D684344A");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                //if (Stop) return;
                //MessageController.Instance.AddMessage("正在进行设置备用套阶梯电价,请稍候....");
                //Common.Memset(ref strID, "04060AFF");
                //Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000" + "00000000");
                //Common.Memset(ref strPutApdu, "04D6844C20");
                //result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
                Common.Memset(ref strID, "04000109");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D684C009");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
                Common.Memset(ref strID, "04000306");
                Common.Memset(ref strData, "000010");
                Common.Memset(ref strPutApdu, "04D6811807");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
                Common.Memset(ref strID, "04000307");
                Common.Memset(ref strData, ("000010"));
                Common.Memset(ref strPutApdu, "04D6811B07");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);



                #endregion 


            }
            catch (Exception)
            {
                
                throw;
            }
            
        }


    }
}
