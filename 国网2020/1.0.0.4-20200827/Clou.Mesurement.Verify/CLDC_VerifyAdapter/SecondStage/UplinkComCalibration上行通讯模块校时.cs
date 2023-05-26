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
    class UplinkComCalibration:VerifyBase
    {


           #region ----------构造函数----------

        public UplinkComCalibration(object plan)
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
            ResultNames = new string[] {"测试时间", "写入时间", "电表时间", "结论","不合格原因" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();
           CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = true;
           CLDC_DataCore.Const.GlobalUnit.Flag_IsZD2016 = true;
           bool[] Result = new bool[BwCount];
           bool[] Result1 = new bool[BwCount];
           string[] Fail = new string[BwCount];
           bool[] Meter = new bool[BwCount];
           DateTime[] arrReadData = new DateTime[BwCount];
           DateTime[] arrReadData1 = new DateTime[BwCount];
           DateTime GPSTime = DateTime.Now;
           DateTime readTime = DateTime.Now;

           SwitchCarrierOr485(Cus_CommunType.通讯载波);
    
         string dateTime = DateTime.Now.ToString("yyMMddHHmmss");
        
        MessageController.Instance.AddMessage("正在写入时间");
              for (int i = 0; i < BwCount; i++)
            {

                Meter[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn;


            }

        MeterBasicInfo curMeter;
        for (int iBw = 0; iBw < BwCount; iBw++)
        {
          
            if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                break;
            }
            //【获取指定表位电表信息】
            curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);
            //【判断是否要检】
            if (!curMeter.YaoJianYn)
            {
                continue;
            }
            GlobalUnit.g_MsgControl.OutMessage("正在载波试验第" + (iBw + 1) + "表位...", false);
            for (int i = 0; i < BwCount; i++)
            {
                if (i == iBw)
                {
                    Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = true;
                }
                else
                {
                    Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = false;
                }                     
            }
            GlobalUnit.Carrier_Cur_BwIndex = iBw;
            Thread.Sleep(1000);


            string str_MeterTime1 = dateTime;
            string[] strID1 = new string[BwCount];
            string[] strData1 = new string[BwCount];
            string[] strSetData1 = new string[BwCount];
            int[] iFlag1 = new int[BwCount];
            string[] strShowData1 = new string[BwCount];
            string[] strCode1 = new string[BwCount];
            string[] strRand11 = new string[BwCount];//随机数
            string[] strRand21 = new string[BwCount];//随机
            string[] strEsamNo1 = new string[BwCount];//Esam序列号
    
            iFlag1 = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand11, out strRand21, out strEsamNo1);
        
            for (int i = 0; i < BwCount; i++)
            {
                strCode1[i] = "0400010C";
                strSetData1[i] = str_MeterTime1.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData1[i] += str_MeterTime1.Substring(6, 6);
                strShowData1[i] = str_MeterTime1;
                strData1[i] = strCode1[i] + strSetData1[i];

            }
            Result1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag1, strRand21, strData1, strCode1);




          //  Result1 = MeterProtocolAdapter.Instance.WriteDateTime(dateTime);
            Result[iBw] = Result1[iBw];
            for (int i = 0; i < BwCount; i++)
            {

                Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = Meter[i];

            }
        }     
        MessageController.Instance.AddMessage("正在读取电表时间");

     //   arrReadData = MeterProtocolAdapter.Instance.ReadDateTime();
        for (int iBw = 0; iBw < BwCount; iBw++)
        {
            Thread.Sleep(1000);
            GlobalUnit.Carrier_Cur_BwIndex = iBw;
            if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                break;
            }
            //【获取指定表位电表信息】
            curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);
            //【判断是否要检】
            if (!curMeter.YaoJianYn)
            {
                continue;
            }
            GlobalUnit.g_MsgControl.OutMessage("正在载波试验第" + (iBw + 1) + "表位...", false);
            for (int i = 0; i < BwCount; i++)
            {
                if (i == iBw)
                {
                    Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = true;
                }
                else
                {
                    Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = false;
                }
                //    Meter[i] = Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn;


            }
            GlobalUnit.Carrier_Cur_BwIndex = iBw;
            arrReadData1 = MeterProtocolAdapter.Instance.ReadDateTime();
            arrReadData[iBw] = arrReadData1[iBw];
            for (int i = 0; i < BwCount; i++)
            {

                Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn = Meter[i];

            }
        }

     
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["写入时间"][i] = dateTime;
                    ResultDictionary["电表时间"][i] = arrReadData[i].ToString("yyMMddHHmmss");
                    if (Result[i])
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;

                    }
                    else
                    {                      
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            ResultDictionary["不合格原因"][i] = "写入时间失败";
                     }
                   
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "写入时间", ResultDictionary["写入时间"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表时间", ResultDictionary["电表时间"]);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

            GlobalUnit.g_CommunType = Cus_CommunType.通讯485;
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = false;
            CLDC_DataCore.Const.GlobalUnit.Flag_IsZD2016 = false;
         




        }

      

    }
}
