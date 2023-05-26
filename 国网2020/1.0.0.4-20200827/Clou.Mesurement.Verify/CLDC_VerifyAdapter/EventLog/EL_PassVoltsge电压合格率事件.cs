using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 电压合格率事件记录
    /// </summary>
    class EL_PassVoltsge:EventLogBase
    {
        public EL_PassVoltsge(object plan)
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
            ResultNames = new string[] { "事件产生前合格率", "电表电压正常时合格率", "理论电压正常时合格率", "电表电压超上限合格率", "理论电压超上限合格率", "电表电压超下限合格率", "理论电压超下限合格率", "结论" };
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
             MessageController.Instance.AddMessage("正在读取电压下限值");
             float[] strVoltageDown = MeterProtocolAdapter.Instance.ReadData("04000E04", 2, 1);
             VoltageDown = ConvertArryToOne(strVoltageDown);

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
             MessageController.Instance.AddMessage("正在读取本日电压合格率记录");
             string[] strLogQ = MeterProtocolAdapter.Instance.ReadData("03700000", 27);
             MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前合格率", GetHGL(strLogQ));

             ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);

             if (Stop) return;
             MessageController.Instance.AddMessage("正在读取本日电压合格率记录");
             string[] strLogUn = MeterProtocolAdapter.Instance.ReadData("03700000", 27);
             strLogUn = GetHGL(strLogUn);
             MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表电压正常时合格率", strLogUn);
             MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "理论电压正常时合格率", strHGL_Un);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表超电压上限值以便形成记录");
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(VoltageUp + 2, VoltageUp + 2, VoltageUp + 2, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 130);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取本日电压合格率记录");
            string[] strLogUp = MeterProtocolAdapter.Instance.ReadData("03700000", 27);
            strLogUp = GetHGL(strLogUp);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表电压超上限合格率",strLogUp);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "理论电压超上限合格率", strHGL_Up);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在让电能表超电压下限值以便形成记录");
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(VoltageDown - 2, VoltageDown - 2, VoltageDown - 2, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取本日电压合格率记录");
            string[] strLogDown = MeterProtocolAdapter.Instance.ReadData("03700000", 27);
            strLogDown = GetHGL(strLogDown);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电表电压超下限合格率", strLogDown);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "理论电压超下限合格率", strHGL_Down);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在对时...");
            strDate = DateTime.Now.ToString("yyMMddHHmmss");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(strDate);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strLogUn[i]) && !string.IsNullOrEmpty(strLogUp[i]) && !string.IsNullOrEmpty(strLogDown[i]))
                {
                    if (Math.Abs(HGL_Un - float.Parse(strLogUn[i])) <= 3 && Math.Abs(HGL_Up - float.Parse(strLogUp[i])) <= 3 && Math.Abs(HGL_Down - float.Parse(strLogDown[i])) <= 3)
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
        public string[] GetHGL(string[] strData)
        {
            string[] strRevData = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    strRevData[i] = strData[i].Substring(strData[i].Length - 12, 6);
                    strRevData[i] = (float.Parse(strRevData[i]) / 100).ToString();
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
