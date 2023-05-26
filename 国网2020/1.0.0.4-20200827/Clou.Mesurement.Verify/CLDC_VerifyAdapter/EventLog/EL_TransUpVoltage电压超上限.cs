using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 电压超上限
    /// </summary>
    class EL_TransUpVoltage:EventLogBase
    {
        public EL_TransUpVoltage(object plan)
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
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "上4次事件记录发生时刻", "上7次事件记录发生时刻", "上10次事件记录发生时刻", "结论", "不合格原因" };
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
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strDataCode = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
         
            float HGL_Un = 100;
            float HGL_Up = 33.33F;
            float HGL_Down = 20;

            string[] strHGL_Un = new string[BwCount];
            string[] strHGL_Up = new string[BwCount];
            string[] strHGL_Down = new string[BwCount];

            Common.Memset(ref strHGL_Un, HGL_Un.ToString());
            Common.Memset(ref strHGL_Up, HGL_Up.ToString());
            Common.Memset(ref strHGL_Down, HGL_Down.ToString());


            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();


         //   if (Stop) return;
         //   MessageController.Instance.AddMessage("正在读取电压上限值");
         //   string[] strVoltageUp = MeterProtocolAdapter.Instance.ReadData("040A0203", 3);//040A0203,3,
         ////   VoltageUp = ConvertArryToOne(strVoltageUp);

         //   if (Stop) return;
         //   MessageController.Instance.AddMessage("正在读取电压下限值");
         //   float[] strVoltageDown = MeterProtocolAdapter.Instance.ReadData("04000E04", 2, 1);
         //   VoltageDown = ConvertArryToOne(strVoltageDown);

    
            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电压超上限事件产生前电压超上限总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03560000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            if (Stop) return;

            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * 1.16f, GlobalUnit.U * 1.16f, GlobalUnit.U * 1.16f, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)Cus_PowerFangXiang.正向有功);
           
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 63);
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);

            //上1次次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电压超上限事件产生后电压超上限总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03560000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电压超上限发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03560001", 12);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);



            //上4次次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次电压超上限发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03560004", 12);
          
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次电压超上限发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03560007", 12);
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!string.IsNullOrEmpty(strLoseTimeQ7[i]))
            //    {
            //        strLoseTimeQ7[i] = strLoseTimeQ7[i].Substring(0, 12);
            //    }
            //}
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);

            //上10次次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次电压超上限发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0356000A", 12);
             
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "次数不匹配";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

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
                if (Data[i] > 0)
                {
                    return Data[i];
                }
            }
            return 0;
        }


    }
}

