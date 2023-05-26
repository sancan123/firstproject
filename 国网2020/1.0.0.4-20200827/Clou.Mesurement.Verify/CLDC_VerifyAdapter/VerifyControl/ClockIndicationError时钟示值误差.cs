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

namespace CLDC_VerifyAdapter
{
    class ClockIndicationError:VerifyBase
    {


           #region ----------构造函数----------

        public ClockIndicationError(object plan)
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
            ResultNames = new string[] { "测试时间", "标准时间", "电表时间", "误差", "结论","不合格原因" };
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
            MessageController.Instance.AddMessage("正在读取GPS时间");
            readTime = Helper.EquipHelper.Instance.ReadGpsTime();

            MessageController.Instance.AddMessage("正在写入当前时间");
            MeterProtocolAdapter.Instance.WriteDateTime(readTime.ToString("yyMMddHHmmss"));


            MessageController.Instance.AddMessage("正在读取电表时间");
            GPSTime = readTime;
            arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();

     
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(arrReadData[i], GPSTime);
                     ResultDictionary["标准时间"][i]  = GPSTime.ToString("yyMMddHHmmss");
                     ResultDictionary["电表时间"][i] = arrReadData[i].ToString("yyMMddHHmmss");
                     ResultDictionary["误差"][i] = Err1.ToString();
                    if (Err1 <= 60)
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    }
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准时间", ResultDictionary["标准时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间", ResultDictionary["电表时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差", ResultDictionary["误差"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);



         




        }

      

    }
}
