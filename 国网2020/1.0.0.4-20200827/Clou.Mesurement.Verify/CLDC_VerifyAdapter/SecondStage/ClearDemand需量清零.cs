using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.SecondStage
{
    class ClearDemand:VerifyBase
    {


           #region ----------构造函数----------

        public ClearDemand(object plan)
            : base(plan)
        {
        }

        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试前需量", "测试后需量",  "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];



        
            MessageController.Instance.AddMessage("正在读取需量");
            float[] readDemand = MeterProtocolAdapter.Instance.ReadDemand(0x00, (byte)0);
         
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {                 
                    ResultDictionary["测试前需量"][i] = readDemand[i].ToString();                                 
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试前需量", ResultDictionary["测试前需量"]);
            MessageController.Instance.AddMessage("正在清需量");
            bool[] clearResult = MeterProtocolAdapter.Instance.ClearDemand();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            MessageController.Instance.AddMessage("正在读取需量");
            float[] readDemand1 = MeterProtocolAdapter.Instance.ReadDemand(0x00, (byte)0);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["测试后需量"][i] = readDemand1[i].ToString();
                    if (readDemand1[i] == 0)
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = "清需量失败";
                    }
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试后需量", ResultDictionary["测试后需量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
        
          



        }

      

    }
}
