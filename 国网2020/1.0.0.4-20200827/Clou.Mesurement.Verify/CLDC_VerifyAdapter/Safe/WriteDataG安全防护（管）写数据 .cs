
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Helper;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// 安全防护（管）写数据，不认证看看可否广播校时
    /// </summary>
    public class WriteDataG : VerifyBase
    {

        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        public WriteDataG(object plan)
            : base(plan)
        {
            ResultNames = new string[] { "测试时间","不合格原因", "结论" };
        }




        #region ----------开始检定----------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
            if (Stop) return;
            PowerOn();
            base.Verify();

            #region 不用认证的写数据
            Random rand = new Random();
            int randDays = rand.Next(600);
            DateTime dt = DateTime.Now.AddDays(randDays);
            string[]  str_DataFirst =new string[BwCount];
            bool[] bReturn = new bool[BwCount];
            bool[] bWriteTime = new bool[BwCount];
            //时间差2分钟
            if (Stop) return;

            MessageController.Instance.AddMessage("正在修改电表时间为当前某日时间早7分钟...");
            string strSetTime = dt.AddMinutes(-7).ToString("yyMMddHHmmss");
            MeterProtocolAdapter.Instance.WriteDateTime(strSetTime);

            if (Stop) return;
            string strBroadCastTime = dt.ToString("yyMMddHHmmss");
       
            MessageController.Instance.AddMessage("正在进行将电表时间广播校时到" + strBroadCastTime);
            DateTime dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
            MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表时间...");
            DateTime[] strMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电脑时间...");
            DateTime strSystemTime = DateTime.Now.AddDays(randDays);

            #region ----------------执行冻结操作----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("开始进行瞬时冻结");
            bReturn = MeterProtocolAdapter.Instance.FreezeCmd("99999999");

            MessageController.Instance.AddMessage("延时5S,请稍候......");
            if (Stop) return;
            Thread.Sleep(5000);
            #endregion


            #endregion
            #region 需要认证的写数据

            //日期设置会现在             
           bWriteTime= MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

            #endregion
            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                double Err1 = CLDC_DataCore.Function.DateTimes.DateDiffSeconds(strMeterTime[i], strSystemTime);
            
                //2次读电表后的值
                str_DataFirst[i] = strMeterTime[i].ToString("yyyy-MM-dd HH:mm:ss");

              
                    if (Math.Abs(Err1) > 60)
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "广播校时后误差超过60s";
                        continue;
                    }
                    else if (!bReturn[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "瞬时冻结不成功";
                        continue;
                    }
                    else if (!bWriteTime[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "校时不成功";
                        continue;
                    }
                    
                   else  
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
               
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
        #endregion

       



    }
}

