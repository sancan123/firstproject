﻿ 
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 阶梯表编程事件，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_StepTimeProgram : EventLogBase
    {

        public EL_StepTimeProgram(object plan)
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
            ResultNames = new string[] { "测试时间", "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "上4次事件记录发生时刻", "上7次事件记录发生时刻", "上10次事件记录发生时刻", "结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
            string[] strID = new string[BwCount];
            string[] strRevData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            bool[] bSetRsdb1 = new bool[BwCount];
            string[] strData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] strPutApdu = new string[BwCount];
            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表阶梯表编程事件总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03301000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);
            //判断第一次读会次数是否有空，如果为空直接不处理，判断不合格
            if (Stop) return;
            MessageController.Instance.AddMessage("正在处理结果");
            int iCheckCount = 0, iFailCount = 0;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iCheckCount++;//检查表的个数

                if (strLoseCountQ[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回次数值为空";
                    iFailCount++;//检查不合格表的次数
                    continue;
                }

            }

            if (iFailCount == iCheckCount)
            {
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);
                UploadTestResult("结论");
                return;
            }

             //设置10次校时
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            for (int it = 0; it <1; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表阶梯表编程以便形成编程记录");
                //  DateTime time = DateTime.Now

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
                Common.Memset(ref strID, "04060AFF");

                Common.Memset(ref strData, "00000010" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                           + "00010000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                           + "000000" + "000000" + "000000" + "000000" + "000000" + "000000");
                Common.Memset(ref strPutApdu, "04D684344A");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
               
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表编程记录产生后阶梯表编程记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03301000", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            
            //上1次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次电表编程记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03301001", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //上4次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上4次电表阶梯表编程记录发生时刻");
            string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03301004", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            //上7次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上7次电表阶梯表编程记录发生时刻");
            string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03301007", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            //上10次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上10次电表阶梯表编程记录发生时刻");
            string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330100A", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" || strLoseTimeQ4[i] == "" || strLoseTimeQ7[i] == "" || strLoseTimeQ10[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) +1 == Convert.ToInt32(strLoseCountH[i]))
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


    }
}
