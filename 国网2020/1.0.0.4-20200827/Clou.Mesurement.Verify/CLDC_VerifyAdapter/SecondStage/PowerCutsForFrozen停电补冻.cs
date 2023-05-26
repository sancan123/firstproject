using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using System.Threading;

namespace CLDC_VerifyAdapter.SecondStage
{
    class PowerCutsForFrozen:VerifyBase
    {


           #region ----------构造函数----------

        public PowerCutsForFrozen(object plan)
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
            ResultNames = new string[] {"测试后组合有功电量", "上1次电量", "上2次电量", "上3次电量","上4次电量", 
                "上5次电量", "上6次电量", "上7次电量", "上8次电量", 
                "上9次电量", "上10次电量", "上11次电量", "上12次电量", 
               "结论","不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();

           #region 走字 正向有功
           MessageController.Instance.AddMessage("最大电流进行正向有功走字20S，请稍候......");
           bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);
           if (!ret)
           {
               throw new Exception("升源失败");
           }
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
           if (Stop) return;
           CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
           #endregion
           PowerOn();
           if (Stop) return;
           MessageController.Instance.AddMessage("正在读取组向有功电量");
           float[] flt_DLZ1 = MeterProtocolAdapter.Instance.ReadData("00000000", 4, 2);

           for (int i = 0; i < BwCount; i++)
           {
               if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
               {
                   //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                   ResultDictionary["测试后组合有功电量"][i] = flt_DLZ1[i].ToString();

               }
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试后组合有功电量", ResultDictionary["测试后组合有功电量"]);
           DateTime dtMeterTime = DateTime.Now;
           string strTime = "";
           bool[] bReturn = new bool[BwCount];
           float[] flt_DL = new float[BwCount];
           bool[,] resultJL = new bool[BwCount, 12];
           string[] Fail = new string[BwCount];
           if (Stop) return;
           for (int i = 0; i < 13; i++)
           {
               if (Stop) return;
               MessageController.Instance.AddMessage("正在进行" + (i + 1) + "月结算日前20s校时");
               string dateTime = DateTime.Now.ToString();
               if(i==12)
               {
                   dateTime = DateTime.Now.AddYears(1).ToString("yy-")+ "01-01 00:00:00";

               }else
               {
                   dateTime = DateTime.Now.ToString("yy-") + (i + 1).ToString().PadLeft(2, '0') + "-01 00:00:00";
               }

               
               dtMeterTime = DateTime.Parse(dateTime);
               strTime = dtMeterTime.AddDays(-1).ToString("yyMMdd") + "235940";
               bReturn = MeterProtocolAdapter.Instance.WriteDateTime(strTime);
               MessageController.Instance.AddMessage("关源");
               Helper.EquipHelper.Instance.PowerOff();
               if (Stop) return;
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
               if (Stop) return;
               Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.正向有功, "1", true, false);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取上" + (i + 1).ToString() + "结算日电量");

    

           }
           for (int i = 0; i < 12; i++)
           {
               Thread.Sleep(1000);

               if (i == 9)
               {
                   flt_DL = MeterProtocolAdapter.Instance.ReadData("0000000A", 4, 2);
               }
               else if (i == 10)
               {
                   flt_DL = MeterProtocolAdapter.Instance.ReadData("0000000B", 4, 2);
               }
               else if (i == 11)
               {
                   flt_DL = MeterProtocolAdapter.Instance.ReadData("0000000C", 4, 2);
               }
               else
               {
                   string strDI = "0000000" + (i + 1).ToString();
                   flt_DL = MeterProtocolAdapter.Instance.ReadData(strDI, 4, 2);
               }
               for (int j = 0; j < BwCount; j++)
               {
                   if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                   {

                       if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                       {
                           if (flt_DL[j].ToString() == null || flt_DL[j].ToString() == "" || flt_DL[j].ToString() == "-1")
                           {
                               resultJL[j, i] = false;
                               Fail[j] = "上" + (i + 1).ToString() + "次读取不到电量" + "|" + Fail[j];
                           }
                           else if (flt_DL[j].ToString() == flt_DLZ1[j].ToString())
                           {
                               resultJL[j, i] = true;
                           }
                           else
                           {
                               resultJL[j, i] = false;
                               Fail[j] = "上" + (i + 1).ToString() + "次读取电量不准确" + "|" + Fail[j];
                           }
                           ResultDictionary["上" + (i + 1).ToString() + "次电量"][j] = flt_DL[j].ToString();

                       }

                   }
               }
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上" + (i + 1).ToString() + "次电量", ResultDictionary["上" + (i + 1).ToString() + "次电量"]);
           }

           for (int j = 0; j < BwCount; j++)
           {
               ResultDictionary["结论"][j] = "合格";
               for (int i = 0; i < 12; i++)
               {


                   if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                   {
                       if (resultJL[j, i] == false)
                       {
                           ResultDictionary["结论"][j] = "不合格";
                           break;
                       }
                   }
               }
           }

           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
        




        }



    }
}
