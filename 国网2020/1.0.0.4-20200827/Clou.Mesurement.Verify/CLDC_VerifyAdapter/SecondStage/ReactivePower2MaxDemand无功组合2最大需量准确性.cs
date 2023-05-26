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
    class ReactivePower2MaxDemand:VerifyBase
    {


           #region ----------构造函数----------

        public ReactivePower2MaxDemand(object plan)
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
            ResultNames = new string[] { "标准需量1", "实际需量1", "标准需量2", "实际需量2", "组合无功最大需量", "需量误差", "结论", "不合格原因" };
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
            PowerOn();
            MessageController.Instance.AddMessage("正在读取组合无功2特征字");
            string[] flt_TzzW1 = MeterProtocolAdapter.Instance.ReadData("04000603", 1);
            Cus_PowerFangXiang Wglfx1 = Cus_PowerFangXiang.第二象限无功;
            Cus_PowerFangXiang Wglfx2 = Cus_PowerFangXiang.第三象限无功;
           
            
           int demandPeriod = 15;               //需量周期
           int slipTimes = 1;                 //滑差时间
           int slipPage = 1;                   //滑差次数
           int maxMinute = demandPeriod + slipTimes * slipPage;
           int maxTime = maxMinute * 60;
           string[] arrStrResultKey = new string[BwCount];

           bool[] clearDemand = MeterProtocolAdapter.Instance.ClearDemand();
           if (Wglfx1 == Cus_PowerFangXiang.第二象限无功)
           {
               MessageController.Instance.AddMessage("开始做第二象限无功最大需量");
               if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, (int)Wglfx1, "0.5C", true, false) == false)
               {
                   Stop = true;
                   MessageController.Instance.AddMessage("开始做最大需量");

                   return;
               }
           }
           else
           {
               MessageController.Instance.AddMessage("开始做第三象限无功最大需量");
               if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, (int)Wglfx1, "-0.5L", true, false) == false)
               {
                   Stop = true;
                   MessageController.Instance.AddMessage("开始做最大需量");

                   return;
               }
           }
         
           
           m_StartTime = DateTime.Now;
           int PastMinute = 0;
           float[] meterXL = new float[BwCount];    //被检表最大需量
        
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
                   MessageController.Instance.AddMessage(string.Format("检定需要{0}分，已经进行{1}分)",  maxMinute, curPorcess));
               }
               float curStandMeterP = GlobalUnit.g_StrandMeterP[0];
                 standMeterP = Math.Abs(curStandMeterP);
                    //每一次分钟读取一次需量数据
                 if (tempMinute > PastMinute)
                 {
                     PastMinute = tempMinute;
                     byte curPD = (byte)((byte)Wglfx1 - 1);
                     arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand(curPD, 0x00);
                     for (int i = 0; i < BwCount; i++)
                     {
                         if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                         {
                             float readXl = arrDemandValue[i] * 1000;
                             if (meterXL[i] < readXl)
                                 meterXL[i] = readXl;
                             ResultDictionary["标准需量1"][i] = standMeterP.ToString();
                             ResultDictionary["实际需量1"][i] = readXl.ToString();

                         }
                     }
                     MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量1", ResultDictionary["标准需量1"]);
                     MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量1", ResultDictionary["实际需量1"]);
                 }
           }
           PowerOn();
           arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(Wglfx1 - 1), 0x00);
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
               arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(Wglfx1 - 1), 0x00);
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   float readXl = arrDemandValue[i] * 1000;
                   if (meterXL[i] < readXl)
                       meterXL[i] = readXl;
                   ResultDictionary["标准需量1"][i] = standMeterP.ToString();
                   ResultDictionary["实际需量1"][i] = readXl.ToString();

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量1", ResultDictionary["标准需量1"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量1", ResultDictionary["实际需量1"]);

           if (Wglfx2 == Cus_PowerFangXiang.第四象限无功)
           {
               MessageController.Instance.AddMessage("开始做第四象限无功最大需量");
               if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, (int)Wglfx2, "-0.5C", true, false) == false)
               {
                   Stop = true;
                   MessageController.Instance.AddMessage("开始做最大需量");

                   return;
               }
           }
           else
           {
               MessageController.Instance.AddMessage("开始做第三象限无功最大需量");
               if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, (int)Wglfx2, "-0.5L", true, false) == false)
               {
                   Stop = true;
                   MessageController.Instance.AddMessage("开始做最大需量");

                   return;
               }
           }
          
          
           m_StartTime = DateTime.Now;
           int PastMinute1 = 0;
           float[] meterXL1 = new float[BwCount];    //被检表最大需量

           float standMeterP1 = 0;
           float[] arrDemandValue1 = new float[0];
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
                   MessageController.Instance.AddMessage(string.Format("检定需要{0}分，已经进行{1}分)", maxMinute, curPorcess));
               }
               float curStandMeterP = GlobalUnit.g_StrandMeterP[0];
               standMeterP1 = Math.Abs(curStandMeterP);
               //每一次分钟读取一次需量数据
               if (tempMinute > PastMinute1)
               {
                   PastMinute1 = tempMinute;
                   byte curPD = (byte)((byte)Wglfx2 - 1);
                   arrDemandValue1 = MeterProtocolAdapter.Instance.ReadDemand(curPD, 0x00);
                   for (int i = 0; i < BwCount; i++)
                   {
                       if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                       {
                           float readXl = arrDemandValue1[i] * 1000;
                           if (meterXL1[i] < readXl)
                               meterXL1[i] = readXl;
                           ResultDictionary["标准需量2"][i] = standMeterP1.ToString();
                           ResultDictionary["实际需量2"][i] = readXl.ToString();

                       }
                   }
                   MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量2", ResultDictionary["标准需量2"]);
                   MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量2", ResultDictionary["实际需量2"]);
               }
           }
            PowerOn();
           arrDemandValue1 = MeterProtocolAdapter.Instance.ReadDemand((byte)(Wglfx2- 1), 0x00);
         
           if (standMeterP1 < 5)
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

               standMeterP1 = GlobalUnit.U * ClfsBs * GlobalUnit.Ib;
               //standMeterP = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
           }

           if (!bResult)
               arrDemandValue1 = MeterProtocolAdapter.Instance.ReadDemand((byte)(Wglfx2 - 1), 0x00);
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   float readXl = arrDemandValue1[i] * 1000;
                   if (meterXL1[i] < readXl)
                       meterXL1[i] = readXl;
                   ResultDictionary["标准需量2"][i] = standMeterP1.ToString();
                   ResultDictionary["实际需量2"][i] = readXl.ToString();

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准需量2", ResultDictionary["标准需量2"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "实际需量2", ResultDictionary["实际需量2"]);


           float[] DemandValueW1 = MeterProtocolAdapter.Instance.ReadDemand((byte)3, 0x00);

           float XLwc = 0;
           float curLevel = Number.SplitKF(Helper.MeterDataHelper.Instance.Meter(0).Mb_chrBdj, IsYouGong);
           float errorLevel = GetErrorLevel(curLevel);
           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   XLwc = (DemandValueW1[i] - (standMeterP + standMeterP1)) / (standMeterP + standMeterP1);

                   ResultDictionary["组合无功最大需量"][i] = (DemandValueW1[i] * 1000).ToString();
                   ResultDictionary["需量误差"][i] = XLwc.ToString("0.0000");
                   if (XLwc > errorLevel)
                   {
                       ResultDictionary["结论"][i] = "不合格";
                       ResultDictionary["不合格原因"][i] = "需量超差";
                   }
                   else if (meterXL[i] == 0 || meterXL1[i] ==0)
                   {
                       ResultDictionary["结论"][i] = "不合格";
                       ResultDictionary["不合格原因"][i] = "没有需量";
                   }
                 //  else if ((meterXL[i] + meterXL1[i]) != DemandValueW1[i])
                //   {
                //       ResultDictionary["结论"][i] = "不合格";
               //        ResultDictionary["不合格原因"][i] = "无功象限需量之和不等于无功组合2需量";
               //    }
                   else
                   {
                       ResultDictionary["结论"][i] = "合格";
                   }

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "组合无功最大需量", ResultDictionary["组合无功最大需量"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "需量误差", ResultDictionary["需量误差"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

        }

   

    }
}

