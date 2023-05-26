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
    class MonthVoltageRate:VerifyBase
    {
        public MonthVoltageRate(object plan)
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
            ResultNames = new string[] { "测试时间","上1月", "上2月", "上3月", "上4月", "上5月", "上6月", "上7月", "上8月", "上9月", "上10月", "上11月", "上12月", "结论","不合格原因" };
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


             //if (Stop) return;
             //MessageController.Instance.AddMessage("正在读取电压考核上限值");
             //float[] strVoltageCheckUp = MeterProtocolAdapter.Instance.ReadData("04090C01", 2, 1);
             //VoltageCheckUp = ConvertArryToOne(strVoltageCheckUp);

             //if (Stop) return;
             //MessageController.Instance.AddMessage("正在读取电压考核下限值");
             //float[] strVoltageCheckDown = MeterProtocolAdapter.Instance.ReadData("04090C02", 2, 1);
             //VoltageCheckDown = ConvertArryToOne(strVoltageCheckDown);

             if (Stop) return;
             MessageController.Instance.AddMessage("正在设置时间...");
             string strDate = DateTime.Now.ToString("yyMMddHHmm") + "00";
             bool[] bResult = MeterProtocolAdapter.Instance.WriteDateTime(strDate);

             if (Stop) return;
             MessageController.Instance.AddMessage("正在进行事件清零...");
             MeterProtocolAdapter.Instance.ClearEventLog("FFFFFFFF");

             if (Stop) return;
            

             ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);

             string[] VoltagePer1 = new string[BwCount]; 
             string[] VoltagePer2 = new string[BwCount]; 
             string[] VoltagePer3 = new string[BwCount];
             string[] VoltagePer4 = new string[BwCount];
             string[] VoltagePer5 = new string[BwCount];
             string[] VoltagePer6 = new string[BwCount];
             string[] VoltagePer7 = new string[BwCount];
             string[] VoltagePer8 = new string[BwCount];
             string[] VoltagePer9= new string[BwCount];
             string[] VoltagePer10 = new string[BwCount];
             string[] VoltagePer11 = new string[BwCount];
             string[] VoltagePer12 = new string[BwCount];
             DateTime dtMeterTime = DateTime.Now;
             string strTime = "";
             MessageController.Instance.AddMessage("正在设置时间...");
             for (int i = 0; i < 12; i++)
             {
                 MessageController.Instance.AddMessage("正在进行" + (i + 1) + "结算日前5分钟校时");
                 string dateTime = DateTime.Now.ToString("yyyy-MM") + "-01 00:00:00";
                 dtMeterTime = DateTime.Parse(dateTime);
                 strTime = dtMeterTime.AddMonths(-i).AddDays(-1).ToString("yyMMdd") + "235500";
                 bResult = MeterProtocolAdapter.Instance.WriteDateTime(strTime);
                 if (Stop) return;
                 MessageController.Instance.AddMessage("正在让电能表超电压上限值以便形成记录");
                 CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(VoltageUp + 2, VoltageUp + 2, VoltageUp + 2, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
                 ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                 PowerOn();
                 ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 200);

             }
         

            if (Stop) return;        


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间...");
            strDate = DateTime.Now.ToString("yyMMddHHmm") + "00";
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(strDate);

            MessageController.Instance.AddMessage("正在读取上1月电压合格率记录");
            string[] strLogUp1 = MeterProtocolAdapter.Instance.ReadData("03700001", 27);
            VoltagePer1 = GetHGL(strLogUp1, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1月", VoltagePer1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上2月电压合格率记录");
            string[] strLogUp2 = MeterProtocolAdapter.Instance.ReadData("03700002", 27);
            VoltagePer2 = GetHGL(strLogUp2, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上2月", VoltagePer2);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上3月电压合格率记录");
            string[] strLogUp3 = MeterProtocolAdapter.Instance.ReadData("03700003", 27);
            VoltagePer3 = GetHGL(strLogUp3, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上3月", VoltagePer3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4月电压合格率记录");
            string[] strLogUp4 = MeterProtocolAdapter.Instance.ReadData("03700004", 27);
            VoltagePer4 = GetHGL(strLogUp4, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4月", VoltagePer4);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上5月电压合格率记录");
            string[] strLogUp5 = MeterProtocolAdapter.Instance.ReadData("03700005", 27);
            VoltagePer5 = GetHGL(strLogUp5, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上5月", VoltagePer5);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上6月电压合格率记录");
            string[] strLogUp6 = MeterProtocolAdapter.Instance.ReadData("03700006", 27);
            VoltagePer6 = GetHGL(strLogUp6, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上6月", VoltagePer6);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7月电压合格率记录");
            string[] strLogUp7 = MeterProtocolAdapter.Instance.ReadData("03700007", 27);
            VoltagePer7 = GetHGL(strLogUp7, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7月", VoltagePer7);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上8月电压合格率记录");
            string[] strLogUp8 = MeterProtocolAdapter.Instance.ReadData("03700008", 27);
            VoltagePer8 = GetHGL(strLogUp8, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上8月", VoltagePer8);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上9月电压合格率记录");
            string[] strLogUp9 = MeterProtocolAdapter.Instance.ReadData("03700009", 27);
            VoltagePer9 = GetHGL(strLogUp9, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上9月", VoltagePer9);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10月电压合格率记录");
            string[] strLogUp10 = MeterProtocolAdapter.Instance.ReadData("0370000A", 27);
            VoltagePer10 = GetHGL(strLogUp10, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10月", VoltagePer10);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上11月电压合格率记录");
            string[] strLogUp11 = MeterProtocolAdapter.Instance.ReadData("0370000B", 27);
            VoltagePer11 = GetHGL(strLogUp11, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上11月", VoltagePer11);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上12月电压合格率记录");
            string[] strLogUp12 = MeterProtocolAdapter.Instance.ReadData("0370000C", 27);
            VoltagePer12 = GetHGL(strLogUp12, 42, 6, "0");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上12月", VoltagePer12);
            if (Stop) return;

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(VoltagePer1[i]) && !string.IsNullOrEmpty(VoltagePer2[i]) && !string.IsNullOrEmpty(VoltagePer3[i]))
                {
                   
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                  
                }
                else
                {
                    ResultDictionary["不合格原因"][i] = "返回数据有问题";
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }
            UploadTestResult("结论");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
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
                    if (strData[i].Length >= 54)
                    {

                   
                    if (state == "0")
                    {
                        strRevData[i] = strData[i].Substring(startIndex, length);
                        strRevData[i] = (float.Parse(strRevData[i]) / 100).ToString();
                    }
                    else if (state == "1")
                    {
                        strRevData[i] = strData[i].Substring(startIndex, length);
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

