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
using CLDC_Comm.Enum;
using System.Threading;     

namespace CLDC_VerifyAdapter.SecondStage
{
    class MaxDemandError:VerifyBase
    {


           #region ----------构造函数----------

        public MaxDemandError(object plan)
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
            ResultNames = new string[] { "标准需量", "实际需量", "需量误差", "结论", "不合格原因" };
            return true;
        }
        private float GetErrorLevel(float meterLevel)
        {
            //海南计算公式:表等级+0.05*额定功率/实际功率
            //标准功率
            float strandPower = CalculatePower(CLDC_DataCore.Const.GlobalUnit.U, CLDC_DataCore.Const.GlobalUnit.Ib, CLDC_DataCore.Const.GlobalUnit.Clfs);
            //负载功率
            float current = CLDC_DataCore.Const.GlobalUnit.Imax;
         
             current = CLDC_DataCore.Const.GlobalUnit.Ib;
            float currentPower = CalculatePower(CLDC_DataCore.Const.GlobalUnit.U, current, CLDC_DataCore.Const.GlobalUnit.Clfs);
            return meterLevel + 0.05F * strandPower / currentPower;
        }
        #endregion                
        public override void Verify()
        {
            base.Verify();
          
           int demandPeriod = 15;               //需量周期
           int slipTimes = 1;                 //滑差时间
           int slipPage = 1;                   //滑差次数
           int maxMinute = demandPeriod + slipTimes * slipPage;
           int maxTime = maxMinute * 60;
           string[] arrStrResultKey = new string[BwCount];

           bool bPowerOn = PowerOn();              //先按方案升源，把表点亮后清需量
           Check.Require(bPowerOn, "控制源输出失败！");
           bool[] clearDemand = MeterProtocolAdapter.Instance.ClearDemand();

           MessageController.Instance.AddMessage("开始做最大需量");
           if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false) == false)
           {
               Stop = true;
               MessageController.Instance.AddMessage("开始做最大需量");

               return;
           }
           m_StartTime = DateTime.Now;
           int PastMinute = 0;
           float[] meterXL = new float[BwCount];    //被检表最大需量
           Cus_PowerFangXiang CurFangXiang = Cus_PowerFangXiang.正向有功;
           float standMeterP = 0;
           float[] arrDemandValue = new float[0];
           while (true)
           {
               if (Stop || m_CheckOver) return;
               Thread.Sleep(2000);
               GlobalUnit.g_MsgControl.ClearCache();

               int pastTime = (int)VerifyPassTime;
               int tempMinute = (int)(VerifyPassTime / 55);
               if (VerifyPassTime >= maxTime)
               {
                   MessageController.Instance.AddMessage("检定时间达到方案预定时间，检定完成");
                   break;
               }
               else
               {
                   pastTime *= 100;
                   pastTime /= 60;
                   float curPorcess = pastTime / 100F;
                   GlobalUnit.g_CUS.DnbData.NowMinute = curPorcess;
                   MessageController.Instance.AddMessage(string.Format("{0}{1}检定需要{2}分，已经进行{3}分)", "正向有功", "最大需量1.0Ib", maxMinute, curPorcess));
               }
               float curStandMeterP = GlobalUnit.g_StrandMeterP[0];
                 standMeterP = Math.Abs(curStandMeterP);
                    //每一次分钟读取一次需量数据
                 if (tempMinute > PastMinute)
                 {
                     PastMinute = tempMinute;
                     byte curPD = (byte)((byte)CurFangXiang - 1);
                     arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand(curPD, 0x00);
                     for (int i = 0; i < BwCount; i++)
                     {
                         if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                         {
                             float readXl = arrDemandValue[i] * 1000;
                             if (meterXL[i] < readXl)
                                 meterXL[i] = readXl;
                             ResultDictionary["标准需量"][i] = standMeterP.ToString();
                             ResultDictionary["实际需量"][i] = readXl.ToString();

                         }
                     }
                     MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量", ResultDictionary["标准需量"]);
                     MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量", ResultDictionary["实际需量"]);
                 }
           }
           arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(CurFangXiang - 1), 0x00);
           bool bResult = true;
           for (int j = 0; j < BwCount; j++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
               {
                   if (arrDemandValue[j] == 0)
                   {
                       bResult = false;
                       break;
                   }
               }
           }
           if (standMeterP < 5)
           {
               float ClfsBs = 1f; //测量方式倍数
               if (GlobalUnit.Clfs == Cus_Clfs.三相四线)
               {
                   ClfsBs = 3f;
               }
               else if (GlobalUnit.Clfs == Cus_Clfs.三相三线)
               {
                   ClfsBs = 1.732f;
               }

               standMeterP = GlobalUnit.U * ClfsBs *  GlobalUnit.Ib;
               //standMeterP = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
           }

           if (!bResult)
               arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(CurFangXiang - 1), 0x00);
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   float readXl = arrDemandValue[i] * 1000;
                   if (meterXL[i] < readXl)
                       meterXL[i] = readXl;
                   ResultDictionary["标准需量"][i] = standMeterP.ToString();
                   ResultDictionary["实际需量"][i] = readXl.ToString();

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量", ResultDictionary["标准需量"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量", ResultDictionary["实际需量"]);
            float XLwc = 0;
               float curLevel = Number.SplitKF(Helper.MeterDataHelper.Instance.Meter(0).Mb_chrBdj, IsYouGong);
                    float errorLevel = GetErrorLevel(curLevel);
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   XLwc = (meterXL[i] - standMeterP) / standMeterP;

                   ResultDictionary["需量误差"][i] = XLwc.ToString("0.0000");
                   if (XLwc > errorLevel)
                   {
                       ResultDictionary["结论"][i] = "不合格";
                       ResultDictionary["不合格原因"][i] = "需量超差";
                   }
                   else if (meterXL[i] ==0)
                   {
                       ResultDictionary["结论"][i] = "不合格";
                       ResultDictionary["不合格原因"][i] = "没有需量";
                   }
                   else
                   {
                       ResultDictionary["结论"][i] = "合格";
                   }

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "需量误差", ResultDictionary["需量误差"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

        }

   

    }
}
