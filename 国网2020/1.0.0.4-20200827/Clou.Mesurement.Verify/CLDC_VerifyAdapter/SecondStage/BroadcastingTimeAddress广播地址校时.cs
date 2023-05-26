using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 广播地址校时，通过全9地址
    /// </summary>
    public class BroadcastingTimeAddress : VerifyBase
    {

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
      public BroadcastingTimeAddress(object plan)
            : base(plan)
        { }

      public override void Verify()
      {

          ResultNames = new string[] { "测试时间", "校时前时间", "校时时间", "校时后时间", "结论", "不合格原因" };
          base.Verify();
          PowerOn();


          string[] str_Data = new string[BwCount];
          string[] strShowData = new string[BwCount];

          Random rand = new Random();
          int randDays = rand.Next(600);
          DateTime dt = DateTime.Now.AddDays(randDays);
          //1.----------------------
          if (Stop) return;
          MessageController.Instance.AddMessage("正在修改电表时间为当前时间早7分钟...");
          string strSetTime = dt.AddMinutes(-7).ToString("yyMMddHHmmss");
          MeterProtocolAdapter.Instance.WriteDateTime(strSetTime);

          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
          DateTime[] strMeterTimeBefore = MeterProtocolAdapter.Instance.ReadDateTime();
          for (int i = 0; i < strMeterTimeBefore.Length; i++)
          {
              strShowData[i] = strMeterTimeBefore[i].ToString();
          }
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "校时前时间", strShowData);


          if (Stop) return;

          string strBroadCastTime = dt.ToString("yyMMddHHmmss");
          MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime);
          DateTime dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
          MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "校时时间", setSameStrArryValue(dtMeterTime.ToString("yyyy-MM-dd HH:mm:ss")));
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);

          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电表时间...");
          DateTime[] strMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();



          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电脑时间...");
          DateTime strSystemTime = dt;



          MessageController.Instance.AddMessage("正在处理结果...");
          for (int i = 0; i < BwCount; i++)
          {
              if (Stop) return;                   //假如当前停止检定，则退出
              if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
              double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime[i], strSystemTime);

              str_Data[i] = strMeterTime[i].ToString();
              if (Math.Abs(Err1) <= 30)
              {
                  ResultDictionary["结论"][i] = Variable.CTG_HeGe;
              }
              else
              {
                  ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                  if (Math.Abs(Err1) > 10)
                  {
                      reasonS[i] = "校时后误差超过30s";
                  }
              }
          }

          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "校时后时间", str_Data);
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
          //日期设置为现在   
          MessageController.Instance.AddMessage("正在设置日期为现在...");
          MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

          UploadTestResult("结论");

      }
    }
}
