
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore;

namespace CLDC_VerifyAdapter.Multi
{

    class Dgn_UFallOrStep : DgnBase
    {
        public  Cus_DgnItem Item{get;set;}
        public  Cus_VolFallOffType VolType{get;set;}
        public  int TestTime{get;set;}

         public Dgn_UFallOrStep(object plan)
            : base(plan)
        {
        }

         public override void Verify()
         {
             base.Verify();
             //throw new Exception("硬件设备目前还不支持电压跌落，");
#if OLD
            //Adapter.Adpater485.setMeterPara();
            float[] startDL = new float[BwCount];
            Dictionary<int, float[]> dicStartEnergy = new Dictionary<int, float[]>();//起始电量
            Dictionary<int, float[]> dicEndEnergy = new Dictionary<int, float[]>();//终止电量
            //if (!Adapter.Adpater485.ReadEnergy(DefaultPD, enmTariffType.总))
            if (!Helper.Rs485Helper.Instance.ReadEnergy(DefaultPD, ClInterface.enmTariffType.总, ref dicStartEnergy))
            {
                Comm.MessageController.Instance.AddMessage("读取电量失败", false);
                return;
            }
            //Array.Copy(Control485.CurResurnFloat, startDL, BwCount);
            //Helper.EquipHelper.Instance.set
            if (!Adapter.ComAdpater.SetVotFalloff(VolType))
            {
                Comm.MessageController.Instance.AddMessage("发送电压跌落命令失败", false);
                return;
            }
            //
            if (!Adapter.Adpater485.ReadEnergy(DefaultPD, enmTariffType.总))
            {
                Comm.MessageController.Instance.AddMessage("读取电量失败!", false);
                return;
            }
            MeterBasicInfo curMeter;
            MeterDgn curResult;
            string strKey = ((int)Item).ToString();
            for (int k = 0; k < BwCount; k++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (curMeter.YaoJianYn)
                {
                    //挂接结论
                    if (!curMeter.MeterDgns.ContainsKey(strKey))
                    {
                        curResult = new MeterDgn();
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = Item.ToString();
                        curMeter.MeterDgns.Add(strKey, curResult);
                    }
                    else
                        curResult = curMeter.MeterDgns[strKey];
                    if (Control485.CurResurnFloat[k] - startDL[k] < 0.1)
                        curResult.Md_chrValue = Variable.CTG_HeGe;
                    else
                        curResult.Md_chrValue = Variable.CTG_BuHeGe;
                    //上报数据
                    
                    GlobalUnit.g_DataControl.OutUpdateData(k, new string[] { strKey }, new object[] { curResult }, Cus_MeterDataType.多功能数据);
                }
                
            }
#endif
             //Adapter.Adpater485.setMeterPara();
             string[] arrStrResultKey = new string[BwCount];
             float[] startDL = new float[BwCount];
             float[] endDL = new float[BwCount];
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
             Dictionary<int, float[]> dicStartEnergy = new Dictionary<int, float[]>();//起始电量
             Dictionary<int, float[]> dicEndEnergy = new Dictionary<int, float[]>();//终止电量
             //if (!Adapter.Adpater485.ReadEnergy(DefaultPD, enmTariffType.总))
             //升源
             if (!PowerOn())
             {
                 MessageController.Instance.AddMessage("源输出失败", 6 , 2);
                 return;
             }
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
            //读取各表位总电量
            MessageController.Instance.AddMessage("正在读取试验前电量...");
             //dicStartEnergy= MeterProtocolAdapter.Instance.ReadEnergy(0);
             startDL = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
             //Array.Copy(Control485.CurResurnFloat, startDL, BwCount);
             //Helper.EquipHelper.Instance.set
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
             //发送电压跌落命令
             if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetVotFalloff((byte)VolType))
             {
                MessageController.Instance.AddMessage("发送"+Item.ToString()+"命令失败");
                 return;
             }
            MessageController.Instance.AddMessage("正在进行" + Item.ToString() + "，请等待" + (TestTime / 1000F) + "秒...");
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
             Thread.Sleep(TestTime);
             //再次读取总电量
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
            MessageController.Instance.AddMessage("正在读取试验后电量...");
             //dicEndEnergy = MeterProtocolAdapter.Instance.ReadEnergy(0);
             endDL = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
             if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
             {
                 return;
             }
             MeterBasicInfo curMeter;
             MeterDgn curResult;
             string strKey = base.ItemKey;
             for (int k = 0; k < BwCount; k++)
             {
                 arrStrResultKey[k] = ItemKey;
                 curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                 if (curMeter.YaoJianYn)
                 {
                     //挂接结论
                     if (!curMeter.MeterDgns.ContainsKey(strKey))
                     {
                         curResult = new MeterDgn();
                         curResult.Md_PrjID = strKey;
                         curResult.Md_PrjName = Item.ToString();
                         curMeter.MeterDgns.Add(strKey, curResult);
                     }
                     else
                         curResult = curMeter.MeterDgns[strKey];
                     if (endDL[k] - startDL[k] < 0.1)
                         curResult.Md_chrValue = Variable.CTG_HeGe;
                     else
                         curResult.Md_chrValue = Variable.CTG_BuHeGe;
                     //上报数据
                     
                     
                 }
                 
             }
            MessageController.Instance.AddMessage("试验完毕...");
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
         }
    }
}
