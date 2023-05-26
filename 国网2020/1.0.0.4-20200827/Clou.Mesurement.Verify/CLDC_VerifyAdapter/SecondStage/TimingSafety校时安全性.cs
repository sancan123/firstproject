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
    class TimingSafety:VerifyBase
    {


           #region ----------构造函数----------

        public TimingSafety(object plan)
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
            ResultNames = new string[] { "测试时间", "写入时间(正常校时)", "电表时间(正常校时后)", "写入时间(明文校时)", "电表时间(明文校时后)", "结论", "不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           bool[] Result = new bool[BwCount];
       bool[] Result1 = new bool[BwCount];
           string[] Fail = new string[BwCount];

           DateTime[] arrReadData = new DateTime[BwCount];
           DateTime GPSTime = DateTime.Now;
           DateTime readTime = DateTime.Now;

           MessageController.Instance.AddMessage("正在正常校时");

           string dateTime = DateTime.Now.ToString("yyMMddHHmmss");


            MessageController.Instance.AddMessage("正在写入时间");
        Result=    MeterProtocolAdapter.Instance.WriteDateTime(dateTime);


            MessageController.Instance.AddMessage("正在读取电表时间");
       
            arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

     
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["写入时间(正常校时)"][i] = dateTime;
                    ResultDictionary["电表时间(正常校时后)"][i] = arrReadData[i].ToString("yyMMddHHmmss");
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "写入时间(正常校时)", ResultDictionary["写入时间(正常校时)"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间(正常校时后)", ResultDictionary["电表时间(正常校时后)"]);



            MessageController.Instance.AddMessage("正在明文校时");
            dateTime = DateTime.Now.AddHours(1).ToString("yyMMddHHmmss");


            MessageController.Instance.AddMessage("正在写入时间");
            Result1 = MeterProtocolAdapter.Instance.WriteDateTimeByMW(dateTime);


            MessageController.Instance.AddMessage("正在读取电表时间");

            arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["写入时间(明文校时)"][i] = dateTime;
                    ResultDictionary["电表时间(明文校时后)"][i] = arrReadData[i].ToString("yyMMddHHmmss");

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "写入时间(明文校时)", ResultDictionary["写入时间(明文校时)"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间(明文校时后)", ResultDictionary["电表时间(明文校时后)"]);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!Result[i] || Result1[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        if (!Result[i])
                        {
                            ResultDictionary["不合格原因"][i] = "不能写入正常时间";
                        }
                        if (Result1[i])
                        {
                            ResultDictionary["不合格原因"][i] = ResultDictionary["不合格原因"][i] + "明文写入时间成功";
                        }

                    }
                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                   

                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

        }

      

    }
}
