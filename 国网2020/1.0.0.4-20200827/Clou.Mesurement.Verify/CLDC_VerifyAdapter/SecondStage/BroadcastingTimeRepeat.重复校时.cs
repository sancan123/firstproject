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
    /// 重复校时
    /// </summary>
  public  class BroadcastingTimeRepeat:VerifyBase
    {
      public BroadcastingTimeRepeat(object plan)
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
      public override void Verify()
      {
          ResultNames = new string[] { "测试时间", "第一次校时时间", "第一次校时后时间", "第二次校时前时间", "第二次校时后时间", "结论", "不合格原因" };
          base.Verify();
          PowerOn();
          string[] strShowData = new string[BwCount];
          string[] str_Data = new string[BwCount];
          string[] str_DataFirst = new string[BwCount];
          string[] str_DataSecond = new string[BwCount];

          Random rand = new Random();
          int randDays = rand.Next(600);
          DateTime dt = DateTime.Now.AddDays(randDays);
          //1.----------------------
          if (Stop) return;
          MessageController.Instance.AddMessage("正在修改电表时间为当前时间早2分钟...");
          string strSetTime = dt.AddMinutes(-2).ToString("yyMMddHHmmss");
          MeterProtocolAdapter.Instance.WriteDateTime(strSetTime);

          if (Stop) return;
          string strBroadCastTime = dt.ToString("yyMMddHHmmss");
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时时间", setSameStrArryValue(strBroadCastTime));
          
       
          MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime);
          DateTime dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
          MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime);

          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *5);

          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电表时间...");
          DateTime[] strMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电脑时间...");
          DateTime strSystemTime = DateTime.Now.AddDays(randDays);

          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 9);

          //2.-----------------------,第2次不能广播较时
          if (Stop) return;
          MessageController.Instance.AddMessage("正在修改电表时间为当前时间早2分钟...");
          string strSetTime2 = DateTime.Now.AddDays(randDays).AddMinutes(-2).ToString("yyMMddHHmmss");
          MeterProtocolAdapter.Instance.WriteDateTime(strSetTime2);

          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
          DateTime[] strMeterTime2Before = MeterProtocolAdapter.Instance.ReadDateTime();
          for (int i = 0; i < strMeterTime2Before.Length; i++)
          {
              strShowData[i] = strMeterTime2Before[i].ToString();
          }


          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时前时间", strShowData);
          if (Stop) return;
          string strBroadCastTime2 = DateTime.Now.AddDays(randDays).ToString("yyMMddHHmmss");
          MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime2);
          DateTime dtMeterTime2 = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime2);
          MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime2);

          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电表时间...");
          DateTime[] strMeterTime2 = MeterProtocolAdapter.Instance.ReadDateTime();

          if (Stop) return;
          MessageController.Instance.AddMessage("正在读取电脑时间...");
          DateTime strSystemTime2 = DateTime.Now.AddDays(randDays);

          MessageController.Instance.AddMessage("正在处理结果...");
          for (int i = 0; i < BwCount; i++)
          {
              if (Stop) return;                   //假如当前停止检定，则退出
              if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
              double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime[i], strSystemTime);
              double Err2 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime2[i], strSystemTime2);
              //2次读电表后的值
              str_DataFirst[i] = strMeterTime[i].ToString("yyyy-MM-dd HH:mm:ss");
              str_DataSecond[i] = strMeterTime2[i].ToString("yyyy-MM-dd HH:mm:ss");

              if (Math.Abs(Err1) <= 60 && Math.Abs(Err2)>= 120)
              {
                  ResultDictionary["结论"][i] = Variable.CTG_HeGe;
              }
              else
              {
                  ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                  if (Math.Abs(Err1) > 10)
                  {
                      reasonS[i] = "第一次校时后误差超过10s";
                  }
                  if (Math.Abs(Err2) > 1 && Math.Abs(Err2) < 110)
                  {
                      reasonS[i] = "第二次校时不应该校时，一天只能广播校时一次";
                  }
              }
          }
       
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次校时后时间", str_DataFirst);
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次校时后时间", str_DataSecond);
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
         
          ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 1);
          MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

          //日期设置为现在   
          MessageController.Instance.AddMessage("正在设置日期为现在...");
          MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
        
          UploadTestResult("结论");
    
      }
    }
}
