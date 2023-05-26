﻿using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    ///  （管）事件清零，模拟触发10次，读取1,4，7，10
    /// </summary>
    class EL_ClearEventG : EventLogBase
    {


        public EL_ClearEventG(object plan)
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

            if (Stop) return;
            base.Verify();
            PowerOn();
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号

            if (Stop) return;
            //清零，让产生记录   
            MeterProtocolAdapter.Instance.ClearEnergy();
            if (Stop) return;
            //设置编程事件，产生记录             
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
            //读取事件清零记录总次数
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取事件清零记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300300", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", strLoseCountQ);

            

            for (int it = 0; it < 1; it++)
            {
              
                MessageController.Instance.AddMessage("正在第" + (it + 1) + "次清零....");                
                MeterProtocolAdapter.Instance.ClearEventLog("FFFFFFFF");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);
            }

            
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取事件清零记录产生后清零总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300300", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            //上一次
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次事件清零记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03300301", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            //判断有没有清记录和不该清清零记录
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取清零事件产生后清零总次数");
            string[] strCleanEnergyCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取编程事件产生总次数");
            string[] stProgramCountH = MeterProtocolAdapter.Instance.ReadData("03300000", 3);
            ////上4次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上4次电表过流记录发生时刻");
            //string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03300304", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            ////上7次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上7次电表过流记录发生时刻");
            //string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03300307", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            ////上10次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上10次电表过流记录发生时刻");
            //string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330030A", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上1次事件清零记录结束时刻");
            //string[] strLoseTimeH = MeterProtocolAdapter.Instance.ReadData("03300301", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录结束时刻", strLoseTimeH);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" || strCleanEnergyCountH[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (strCleanEnergyCountH[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清事件记录后，永久清零记录不应该为空";
                    continue;
                }
                if (strCleanEnergyCountH[i] == "000000")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清事件记录后，永久清零记录不应该为0";
                    continue;
                }
                if (stProgramCountH[i] != "000000")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "清事件记录后，编程记录应该为0";
                    continue;
                }
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














    }
}