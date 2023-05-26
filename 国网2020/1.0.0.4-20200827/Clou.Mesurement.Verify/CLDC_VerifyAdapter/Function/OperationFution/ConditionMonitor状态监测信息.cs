using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CLDC_VerifyAdapter.Function.OperationFution
{
    class ConditionMonitor : VerifyBase
    {
        public ConditionMonitor(object plan)
            : base(plan)
        { }

        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] {"测试时间" ,"管理单元上电运行时间", "时钟电池运行时间", "计量单元上电运行时间","结论" ,"不合格原因"};
            return true;
        }

        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            string[] strReadData = new string[BwCount];
            if (Stop) return;
            PowerOn();


            string[] GLDYYXSJ = new string[BwCount];
            string[] SZDCYXSJ = new string[BwCount];
            string[] JLDYYXSJ = new string[BwCount];

            MessageController.Instance.AddMessage("正在读取管理单元上电运行时间");
            GLDYYXSJ = MeterProtocolAdapter.Instance.ReadData("02800085", 4);


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "管理单元上电运行时间", GLDYYXSJ);
            MessageController.Instance.AddMessage("正在读取时钟电池运行时间");
            SZDCYXSJ = MeterProtocolAdapter.Instance.ReadData("0280000A", 4);


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "时钟电池运行时间", SZDCYXSJ);
            MessageController.Instance.AddMessage("正在读取计量单元上电运行时间");
            JLDYYXSJ = MeterProtocolAdapter.Instance.ReadData("02800031", 4);


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "计量单元上电运行时间", JLDYYXSJ);



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(GLDYYXSJ[i]) && !string.IsNullOrEmpty(SZDCYXSJ[i]) && !string.IsNullOrEmpty(JLDYYXSJ[i]))
                {
                    if (float.Parse(GLDYYXSJ[i]) != 0 && float.Parse(SZDCYXSJ[i]) != 0 && float.Parse(JLDYYXSJ[i]) != 0 )
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = "返回数据为0";
                    }
                }
                else
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "返回数据为空";
                }

            }
            UploadTestResult("结论");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);

        }
    }
}