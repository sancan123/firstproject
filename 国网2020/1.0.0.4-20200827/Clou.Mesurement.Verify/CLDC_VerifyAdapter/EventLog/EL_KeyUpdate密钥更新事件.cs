 
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
    /// 密钥更新事件
    /// </summary>
    class EL_KeyUpdate : EventLogBase
    {

        public EL_KeyUpdate(object plan)
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

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            string[] strRand1 = new string[BwCount];//随机数1
            string[] strRand2 = new string[BwCount];//随机数2
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            if (Stop) return;
            //对时到现在
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet1 = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            MessageController.Instance.AddMessage("正在读取密钥更新记录总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
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

             //设置
        
            for (int it = 0; it <1; it++)
            {


                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥更新....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
              
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);

                if (Stop) return; 
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行密钥恢复....");
                bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 30);

            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电表密钥更新、恢复后产生后编程记录总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", strLoseCountH);
            
            //上1次发生时刻记录内容
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取上1次密钥更新记录发生时刻");
            string[] strLoseTimeQ = MeterProtocolAdapter.Instance.ReadData("03301201", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次事件记录发生时刻", strLoseTimeQ);

            ////上4次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上4次电表过流记录发生时刻");
            //string[] strLoseTimeQ4 = MeterProtocolAdapter.Instance.ReadData("03300004", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上4次事件记录发生时刻", strLoseTimeQ4);

            ////上7次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上7次电表过流记录发生时刻");
            //string[] strLoseTimeQ7 = MeterProtocolAdapter.Instance.ReadData("03300007", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上7次事件记录发生时刻", strLoseTimeQ7);


            ////上10次发生时刻记录内容
            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在读取上10次电表过流记录发生时刻");
            //string[] strLoseTimeQ10 = MeterProtocolAdapter.Instance.ReadData("0330000A", 6);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上10次事件记录发生时刻", strLoseTimeQ10);

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountQ[i] == "" || strLoseCountH[i] == "" || strLoseTimeQ[i] == "" )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 2 == Convert.ToInt32(strLoseCountH[i]))
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
