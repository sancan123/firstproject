using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 身份认证时效性设置测试
    /// </summary>
    public class RemoteIdentityTimeliness :VerifyBase
    {
        public RemoteIdentityTimeliness(object plan) : base(plan)
        { }
       
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "操作前时效性1", "操作后时效性1","不可设置时效0分钟",
                                         "操作前时效性2", "操作后时效性2","可设置时效9999分钟", 
                                         "操作前时效性3", "操作后时效性3","可设置时效30分钟", 
                                         "结论" };
            return true;
        }

        /// <summary>
        /// 开始检定业务
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            //只升电压
            PowerOn();
            //身份认证
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            string[] str_ID = new string[BwCount];
            string[] str_Data = new string[BwCount];
            string[] str_Apdu = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            bool[] bln_Rst = new bool[BwCount];
            string[] strSyTime = new string[BwCount];
            int[] iFlag = new int[BwCount];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            //步骤1---------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作前时效性1", strSyTime);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时效为0分钟,请稍候....");
            Common.Memset(ref str_ID,"070001FF");
            Common.Memset(ref str_Data,"0000");
            Common.Memset(ref str_Apdu, "04D6812B06");
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作后时效性1", strSyTime);

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    blnRet[i, 0] = bln_Rst[i];
                    ResultDictionary["不可设置时效0分钟"][i] = !blnRet[i, 0] ? "通过" : "不通过";
                }
            }
            UploadTestResult("不可设置时效0分钟");

            //步骤2----------

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作前时效性2", strSyTime);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时效为9999分钟,请稍候....");
            Common.Memset(ref str_ID, "070001FF");
            Common.Memset(ref str_Data, "9999");
            Common.Memset(ref str_Apdu, "04D6812B06");
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作后时效性2", strSyTime);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 1] = bln_Rst[i];
                ResultDictionary["可设置时效9999分钟"][i] = bln_Rst[i] ? "通过" : "不通过";
            }
            UploadTestResult("可设置时效9999分钟");


            //步骤3---------
            
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作前时效性3", strSyTime);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时效为30分钟,请稍候....");
            Common.Memset(ref str_ID,"070001FF");
            Common.Memset(ref str_Data, "0030");
            Common.Memset(ref str_Apdu, "04D6812B06");
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作后时效性3", strSyTime);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 2] = bln_Rst[i];
                ResultDictionary["可设置时效30分钟"][i] = bln_Rst[i] ? "通过" : "不通过";
            }
            UploadTestResult("可设置时效30分钟");



            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!blnRet[i,0] && blnRet[i, 1] && blnRet[i, 2] )
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            //通知界面
           UploadTestResult("结论");
        }
        
        protected override string ItemKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override string ResultKey
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
