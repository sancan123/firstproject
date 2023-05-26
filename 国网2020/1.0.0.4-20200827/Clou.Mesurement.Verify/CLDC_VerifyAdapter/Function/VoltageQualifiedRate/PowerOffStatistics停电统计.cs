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
    class PowerOffStatistics:VerifyBase
    {
        public PowerOffStatistics(object plan)
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
            ResultNames = new string[] {"测试时间", "停电前本月停电次数", "停电前本月停电时长", "停电后本月停电次数", "停电后本月停电时长",  "结论" ,"不合格原因"};
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
            string[] strTd = new string[BwCount];
            string[] strTdQ = new string[BwCount];
            string[] strTdTimeQ = new string[BwCount];
            string[] strTdH = new string[BwCount];
            string[] strTdTimeH = new string[BwCount];

            MessageController.Instance.AddMessage("正在读取停电前本月停电次数与时长");
           strTd = MeterProtocolAdapter.Instance.ReadData("03840000", 6);
           strTdQ = GetHGL(strTd, 6, 6, "2");
           strTdTimeQ = GetHGL(strTd, 0, 6, "2");
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "停电前本月停电次数", strTdQ);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "停电前本月停电时长", strTdTimeQ);

           MessageController.Instance.AddMessage("开始停电2分钟");
           CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
           PowerOn();
           MessageController.Instance.AddMessage("正在读取停电后本月停电次数与时长");
           strTd = MeterProtocolAdapter.Instance.ReadData("03840000", 6);
           strTdH = GetHGL(strTd, 6, 6, "2");
           strTdTimeH = GetHGL(strTd, 0, 6, "2");
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "停电后本月停电次数", strTdH);
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "停电后本月停电时长", strTdTimeH);
           
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(strTd[i]) && !string.IsNullOrEmpty(strTdQ[i]) && !string.IsNullOrEmpty(strTdTimeQ[i]) && !string.IsNullOrEmpty(strTdH[i]) && !string.IsNullOrEmpty(strTdTimeH[i]))
                {
                    if (int.Parse(strTdH[i]) - int.Parse(strTdQ[i]) == 1)
                    {
                        if (int.Parse(strTdTimeH[i]) - int.Parse(strTdTimeQ[i]) == 2)
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["不合格原因"][i] = "时间不对";
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    else
                    {
                        ResultDictionary["不合格原因"][i] = "次数不对";
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "返回数据为空";
                }
            }

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
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
                        strRevData[i] = strData[i].Substring(startIndex, length);
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

