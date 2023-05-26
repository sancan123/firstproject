using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Function.VoltageQualifiedRate
{
    class VoltageComputingMethod:VerifyBase
    {
        public VoltageComputingMethod(object plan)
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
            ResultNames = new string[] { "测试时间","电压监测时间", "电压合格率",  "电压超上限时间",  "结论" ,"不合格原因"};
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();

            float VoltageUp = 0;
            float VoltageDown = 0;
            float VoltageCheckUp = 0;
            float VoltageCheckDown = 0;

            float HGL_Un = 100;
            float HGL_Up = 33.33F;
            float HGL_Down = 20;

            string[] strHGL_Un =new string[BwCount];
            string[] strHGL_Up =new string[BwCount];
            string[] strHGL_Down =new string[BwCount];

            Common.Memset(ref strHGL_Un,HGL_Un.ToString());
            Common.Memset(ref strHGL_Up,HGL_Up.ToString());
            Common.Memset(ref strHGL_Down,HGL_Down.ToString());


            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();


            if (Stop) return;
             MessageController.Instance.AddMessage("正在读取电压上限值");
             float[] strVoltageUp = MeterProtocolAdapter.Instance.ReadData("04000E03", 2, 1);
             VoltageUp = ConvertArryToOne(strVoltageUp);

        

     

             if (Stop) return;
             MessageController.Instance.AddMessage("正在设置时间...");
             string strDate = DateTime.Now.ToString("yyMMddHHmm") + "00";
             bool[] bResult = MeterProtocolAdapter.Instance.WriteDateTime(strDate);

             if (Stop) return;
             MessageController.Instance.AddMessage("正在进行事件清零...");
             MeterProtocolAdapter.Instance.ClearEventLog("FFFFFFFF");

             if (Stop) return;
            

             ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);

             string[] MonitorTime = new string[BwCount]; ;//电压监测时间
             string[] VoltagePer = new string[BwCount]; ;//电压合格率
          
             string[] UpPerTime = new string[BwCount]; ; //电压超上限时间
             string[] Uper = new string[BwCount];

            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表超电压上限值以便形成记录");
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(VoltageUp + 2, VoltageUp + 2, VoltageUp + 2, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 130);
            MessageController.Instance.AddMessage("正在读取本日电压合格率记录");
            string[] strLogUp = MeterProtocolAdapter.Instance.ReadData("03700000", 29);
          
          
          

          
            MonitorTime = GetHGL(strLogUp, 52, 6, "2");   
            VoltagePer= GetHGL(strLogUp, 46, 6, "0");  
           
            UpPerTime = GetHGL(strLogUp, 34, 6, "2");  
           
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压监测时间", MonitorTime);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压合格率", VoltagePer);
          
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电压超上限时间", UpPerTime);
        

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时...");
            strDate = DateTime.Now.ToString("yyMMddHHmmss");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(strDate);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(MonitorTime[i]) && !string.IsNullOrEmpty(VoltagePer[i]) && !string.IsNullOrEmpty(UpPerTime[i]))
                {
                    Uper[i] = (1 - int.Parse(UpPerTime[i]) / int.Parse(MonitorTime[i])).ToString();
                    if (float.Parse(Uper[i]) - float.Parse(VoltagePer[i]) <= 0.01)
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    }
                       
                  
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }
            UploadTestResult("结论");
        }

        /// <summary>
        /// 截取合格率
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string[] GetHGL(string[] strData , int startIndex,int length,string state)
        {
            string[] strRevData = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    if (strData[i].Length >= 58)
                    {
                        if (state == "0")
                        {
                            strRevData[i] = strData[i].Substring(strData[i].Length - startIndex, length);
                            strRevData[i] = (float.Parse(strRevData[i]) / 100).ToString();
                        }
                        else if (state == "1")
                        {
                            strRevData[i] = strData[i].Substring(strData[i].Length - startIndex, length);
                            strRevData[i] = (float.Parse(strRevData[i]) / 10).ToString();
                        }
                        else
                        {
                            strRevData[i] = strData[i].Substring(strData[i].Length - startIndex, length);
                        }
                    }
                   
                }
            }
            return strRevData;
        }


        public float ConvertArryToOne(float[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i]>0)
                {
                    return Data[i];
                }
            }
            return 0;
        }
    }
}

