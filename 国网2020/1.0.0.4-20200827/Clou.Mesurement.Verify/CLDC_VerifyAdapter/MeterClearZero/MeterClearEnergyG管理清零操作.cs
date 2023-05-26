﻿
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.MeterClearZero
{
    /// <summary>
    /// 管理清零操作
    /// </summary>
    class MeterClearEnergyG : VerifyBase
    {
        public MeterClearEnergyG(object plan)
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
            ResultNames = new string[] { "测试时间", "结论", "不合格原因" };
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
            string[] revCode = new string[BwCount];
           
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            bool[] fResult = new bool[BwCount];//清零操作
                float f=0.3F;



            if (Stop) return;
            //设置1次编程记录
            for (int it = 0; it < 1; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表编程以便形成编程记录");
                //  DateTime time = DateTime.Now
                MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
                MessageController.Instance.AddMessage("正在下发身份认证失效命令,请稍候....");
                if (Stop) return;
                MeterProtocolAdapter.Instance.SouthCmdNoData("070002FF");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);

            }
      

            //产生电量，给清零做铺垫
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Imax * f, GlobalUnit.Imax * f, GlobalUnit.Imax * f, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, GlobalUnit.U, GlobalUnit.Imax * f, 0, GlobalUnit.Imax * f, 1, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Imax * f, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 62);
            PowerOn();
             if (Stop) return;
             MessageController.Instance.AddMessage("正在进行第" + 1 + "次清零...");
         
             MessageController.Instance.AddMessage("正在清零");
             fResult = MeterProtocolAdapter.Instance.ClearEnergy();
             ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
            
            //读电能量
            if (Stop) return;
            float[] flt_DL = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);


            //读需量
            if (Stop) return;
            float[] flt_XL = new float[BwCount];
            if (GlobalUnit.Clfs != CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                flt_XL = MeterProtocolAdapter.Instance.ReadData("01080000", 8, 2);
            }
                
       
            //读冻结量，分钟，电压
            if (Stop) return;
           // string[] flt_DJ = MeterProtocolAdapter.Instance.ReadDataWithCode("06110101", 2, out revCode);
           
            string strTime = DateTime.Now.AddMinutes(-2).ToString("yyMMddHHmm") + "01";
            string[] flt_DJ = MeterProtocolAdapter.Instance.ReadLoadRecord("06110101", 5, strTime);
           // string[] strData = new string[BwCount];
            //string[] RevData = new string[BwCount];
            //bool[] result = MeterProtocolAdapter.Instance.SouthCmdData("078002FF", strData, out RevData);
           //读事件记录
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表编程记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300000", 3);
           
            //读负荷记录，正向总,只是回数据标识就可
            if (Stop) return;
            string strTime1 = DateTime.Now.AddMinutes(-2).ToString("yyMMddHHmm") + "01";
            string[] flt_FH = MeterProtocolAdapter.Instance.ReadLoadRecord("06100601", 5, strTime1);
            
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (flt_DL[i] != 0 )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零后，读取电能量数据不对";
                    continue;
                }
                if (flt_XL[i] != 0 && GlobalUnit.Clfs != CLDC_Comm.Enum.Cus_Clfs.单相)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零后，读取需量数据不对";
                    continue;
                }
                if (flt_DJ[i] != "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零后，读取冻结量不应该有数据！";
                    continue;
                }
                  
                
                if (flt_FH[i] != ""  )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零后，读取负荷记录数据不对";
                    continue;
                }
                if (  strLoseCountQ[i] != "000000")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清零后，读取事件记录数据不对";                   
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
    }
}
