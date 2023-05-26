using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CLDC_VerifyAdapter.Function.OperationFution
{
    class PowerSalesContract: VerifyBase
    {
        public PowerSalesContract(object plan)
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
            ResultNames = new string[] { "测试时间","售电方", "购电方", "用电容量", "对应的费率时段和电价", "结论","不合格原因" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            string[] strReadData = new string[BwCount];
            if (Stop) return;
            PowerOn();
            MessageController.Instance.AddMessage("读取售电方");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04001701", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "售电方", strReadData);

            MessageController.Instance.AddMessage("读取购电方");
            string[] strGDF = MeterProtocolAdapter.Instance.ReadData("04001702", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "购电方", strGDF);

            MessageController.Instance.AddMessage("读取用电容量");
            string[] strYDRL = MeterProtocolAdapter.Instance.ReadData("04001703", 8);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "用电容量", strYDRL);

            MessageController.Instance.AddMessage("读取对应的费率时段和电价");
            string[] strFLDJ = MeterProtocolAdapter.Instance.ReadData("04001704", 8);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "对应的费率时段和电价", strFLDJ);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strReadData[i]) && !string.IsNullOrEmpty(strGDF[i]) && !string.IsNullOrEmpty(strYDRL[i]) && !string.IsNullOrEmpty(strFLDJ[i]))
                {

                    if (strReadData[i] == "FFFFFFFF" || strGDF[i] == "FFFFFFFF" || strYDRL[i] == "FFFFFFFFFFFFFFFF" || strFLDJ[i] == "FFFFFFFFFFFFFFFF")
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = "返回数据全为F";
                    }
                    else  if (float.Parse(strReadData[i]) != 0 && float.Parse(strGDF[i]) != 0 && float.Parse(strYDRL[i]) != 0 && float.Parse(strFLDJ[i]) != 0)
                    {
                            ResultDictionary["结论"][i] = "合格";
                    }
                    else
                   {
                            ResultDictionary["结论"][i] = "不合格";
                            ResultDictionary["不合格原因"][i] = "返回数据为0";
                   }
                   
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "返回数据为空";
                }

            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);





        }
    }
}
