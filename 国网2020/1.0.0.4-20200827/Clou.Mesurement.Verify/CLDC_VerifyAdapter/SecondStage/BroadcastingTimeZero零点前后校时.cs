using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter
{
   /// <summary>
    /// 零点前后校时
   /// </summary>
    class BroadcastingTimeZero : VerifyBase
    {
        public BroadcastingTimeZero(object plan)
            : base(plan)
        { }

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
            ResultNames = new string[] { "测试时间", "第一次校时前时间", "第一次校时时间", "第一次校时后时间", "第二次校时前时间", "第二次校时时间", "第二次校时后时间", "结论", "不合格原因" };
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            bool[] bResult = new bool[BwCount];

            
            base.Verify();
            bool bPowerOn = PowerOn();
            string[] str_Data = new string[BwCount];

            string[] str_DataFirst = new string[BwCount];
            string[] str_DataSecond = new string[BwCount];
            string[] strShowData = new string[BwCount];

            Random rand = new Random();
            int randDays = rand.Next(500);
            DateTime dt = DateTime.Now.AddDays(randDays);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间为23:43:00");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(dt.ToString("yyMMdd") + ("234300"));
         
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
            DateTime[] strMeterTimeBefore1 = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < strMeterTimeBefore1.Length; i++)
            {
                strShowData[i] = strMeterTimeBefore1[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时前时间", strShowData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *2);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发零点前广播校时命令");
            string strBeforDt = dt.ToString("yyyy-MM-dd") + (" 23:50:00");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时时间", setSameStrArryValue(strBeforDt));
            MeterProtocolAdapter.Instance.BroadCastTime(DateTime.Parse(strBeforDt));
            

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);



            //读零点前时间
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
            DateTime[] dateTimeQ = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < dateTimeQ.Length; i++)
            {
                strShowData[i] = dateTimeQ[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时后时间", strShowData);


            //零点后处理
            if (Stop) return;
            MessageController.Instance.AddMessage("正在修改电表时间为23:59:55");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(dt.AddDays(1).ToString("yyMMdd") + ("000955"));//235955
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            
            DateTime[] strMeterTimeBefore2 = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < strMeterTimeBefore2.Length; i++)
            {
                strShowData[i] = strMeterTimeBefore2[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时前时间", strShowData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在下发零点后广播校时命令");
            string strAfterDt2 = dt.AddDays(1).ToString("yyyy-MM-dd") + (" 00:17:00");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时时间", setSameStrArryValue(strAfterDt2));

            MeterProtocolAdapter.Instance.BroadCastTime(DateTime.Parse(strAfterDt2));
            
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            //读零点后时间
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
            DateTime[] dateTimeH = MeterProtocolAdapter.Instance.ReadDateTime();
            for (int i = 0; i < dateTimeH.Length; i++)
            {
                strShowData[i] = dateTimeH[i].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时后时间", strShowData);

      

            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                
                //计算时间

                double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(dateTimeQ[i], DateTime.Parse(strBeforDt));
                double Err2 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(dateTimeH[i], DateTime.Parse(strAfterDt2));
                 str_DataFirst[i] = strBeforDt;
                 str_DataSecond[i] = strAfterDt2;


                 if (Err1 <= 30 && Err2 <= 30)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }else{
                 
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    if (Math.Abs(Err1) > 30)
                    {
                        reasonS[i] = "第一次校时后误差超过30s";
                    }
                    if (Math.Abs(Err2) > 30)
                    {
                        reasonS[i] = "第二次校时后误差超过30s";
                    }
                }
            }
            
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            //日期设置为现在   
            MessageController.Instance.AddMessage("正在设置日期为现在...");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
        
            UploadTestResult("结论");
            
        }
    }
}
