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
    class ClockCheck:VerifyBase
    {


           #region ----------构造函数----------

        public ClockCheck(object plan)
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
            ResultNames = new string[] { "测试时间", "写入时间", "电表时间", "结论", "不合格原因" };
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
           string dateTime = DateTime.Now.ToString("yyMMdd") +"250000";

            MessageController.Instance.AddMessage("正在写入时间");
        Result=    MeterProtocolAdapter.Instance.WriteDateTime(dateTime);


            MessageController.Instance.AddMessage("正在读取电表时间");
       
            arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

     
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["写入时间"][i] =dateTime;
                    ResultDictionary["电表时间"][i] = arrReadData[i].ToString("yyMMddHHmmss");
                    if (Result[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        ResultDictionary["不合格原因"][i] = "成功写入非法时间";
                    }
                    else
                    {
                        if (arrReadData[i].Hour == 25)
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            ResultDictionary["不合格原因"][i] = "成功写入非法时间";
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
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);


         




        }

      

    }
}
