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
    class RatePeriodSet:VerifyBase
    {


           #region ----------构造函数----------

        public RatePeriodSet(object plan)
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
            ResultNames = new string[] { "写入时间", "电表时间", "结论" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];

           DateTime[] arrReadData = new DateTime[BwCount];
           DateTime GPSTime = DateTime.Now;
           DateTime readTime = DateTime.Now;
           string dateTime = DateTime.Now.ToString("yyMMdd") + "250000";

           MessageController.Instance.AddMessage("正在读取日时段数(每日切换数)");
           string[] Rsds = MeterProtocolAdapter.Instance.ReadData("04000203", 1);


           MessageController.Instance.AddMessage("正在读取第二套第1日时段表数据");
           string[] Rsdb = MeterProtocolAdapter.Instance.ReadData("04020001", 288);
            arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

     
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (Result[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CMG_BuHeGe;
                    }
                    else
                    {
                        if (arrReadData[i].Hour == 25)
                        {
                            ResultDictionary["结论"][i] = Variable.CMG_BuHeGe;
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                        }
                    }
                   
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "写入时间", ResultDictionary["写入时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间", ResultDictionary["电表时间"]);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);



         




        }

      

    }
}
