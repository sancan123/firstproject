using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 身份认证时效时间测试
    /// </summary>
   public class RemoteIdentityTime : VerifyBase
    {
               public RemoteIdentityTime(object plan) : base(plan)
        { }
        
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //这里要解析检定参数

            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "身份认证时效性设置值1","身份认证时效性表计值1", "身份认证时效性设置1",
                                         "日期时间设置值2", "日期时间表计值2","日期时间误差±10s",
                                         "时效时间递减为0后不可设置日期时间3",
                                         "时效剩余时间起值4","时效剩余时间（15s后）4","时效剩余时间（75s后）4","时效剩余时间（135s后）4","时效剩余时间递减4",
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


            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            //MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            //strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);

            //步骤1--------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时效为2分钟,请稍候....");
            Common.Memset(ref str_ID,"070001FF");
            Common.Memset(ref str_Data,"0002");
            Common.Memset(ref str_Apdu, "04D6812B06");
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "身份认证时效性设置值1", str_Data);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "身份认证时效性表计值1", strSyTime);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strSyTime[i] == str_Data[i])
                {
                    blnRet[i, 0] = true;
                }
                ResultDictionary["身份认证时效性设置1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("身份认证时效性设置1");

            //步骤2------------------------------------
            if (Stop) return;
            string[] strDate = new string[BwCount];
            string[] strDateTmp = new string[BwCount];

            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            Common.Memset(ref str_ID, "0400010C");
            Common.Memset(ref str_Data,"0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek +DateTime.Now.ToString("HHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);


            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            Common.Memset(ref strDateTmp, DateTime.Now.ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在读取表计时间,请稍候....");
            DateTime[] MeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if(!string.IsNullOrEmpty(MeterTime[i].ToString()))
                {
                    DateTime dateTimeTmp = DateTimes.FormatStringToDateTime(strDateTmp[i]);
                    str_Data[i] = MeterTime[i].ToString();
                    int Second = DateTimes.DateDiff(dateTimeTmp, MeterTime[i]);
                    strSyTime[i] = System.Math.Abs(Second).ToString();
                    if (Second <= 10)
                    {
                        blnRet[i, 1] = true;
                    }
                }
                ResultDictionary["日期时间误差±10s"][i] = blnRet[i, 1] ? "通过" : "不通过";
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日期时间设置值2", strDateTmp);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "日期时间表计值2", str_Data);
            UploadTestResult("日期时间误差±10s");

            //3-----------------
            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);

            //4---------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在再次设置时间,请稍候....");
            Common.Memset(ref str_ID, "0400010C");
            Common.Memset(ref str_Data, "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek + DateTime.Now.ToString("HHmmss"));
            Common.Memset(ref strDateTmp, DateTime.Now.ToString("yyMMddHHmmss"));
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 2] = !bln_Rst[i];
                ResultDictionary["时效时间递减为0后不可设置日期时间3"][i] = !bln_Rst[i] ? "通过" : "不通过";
            }
            UploadTestResult("时效时间递减为0后不可设置日期时间3");

            //5----------------------------
            if (Stop) return;
            Common.Memset(ref strDate, DateTime.Now.ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时效为30分钟,请稍候....");
            Common.Memset(ref str_ID, "070001FF");
            Common.Memset(ref str_Data, "0030");
            Common.Memset(ref str_Apdu, "04D6812B06");
            bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, str_ID, str_Data);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            string[] strSyTimeIni = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "时效剩余时间起值4", strSyTimeIni);

            
            if (Stop) return;
            MessageController.Instance.AddMessage("正在等待身份认证时效剩余时间过15秒后,读取身份认证时效剩余时间,请稍候....");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 14);
            if (Stop) return;
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "时效剩余时间（15s后）4", strSyTime);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(strSyTimeIni[i]) && !string.IsNullOrEmpty(strSyTime[i]))
                    {
                        if (strSyTime[i] == strSyTimeIni[i])
                            blnRet[i, 3] = true;
                    }
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在等待身份认证时效剩余时间过75秒后,读取身份认证时效剩余时间,请稍候....");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            if (Stop) return;
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "时效剩余时间（75s后）4", strSyTime);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(strSyTimeIni[i]) && !string.IsNullOrEmpty(strSyTime[i]))
                    {
                        if (strSyTime[i] == "0029")
                        {
                            blnRet[i, 4] = true;
                        }
                    }
                }
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在等待身份认证时效剩余时间过135秒后,读取身份认证时效剩余时间,请稍候....");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取身份认证时效剩余时间,请稍候....");
            strSyTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "时效剩余时间（135s后）4", strSyTime);
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (!string.IsNullOrEmpty(strSyTimeIni[i]) && !string.IsNullOrEmpty(strSyTime[i]))
                    {
                        if (strSyTime[i]=="0028") 
                        {
                            blnRet[i, 5] = true;
                        }
                    }
                }
            }


            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5] )
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                    if (blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5])
                    {
                        ResultDictionary["时效剩余时间递减4"][i] = "通过";
                    }
                    else
                    {
                        ResultDictionary["时效剩余时间递减4"][i] = "不通过";
                    }
                }
            }
            //通知界面
            UploadTestResult("时效剩余时间递减4");
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
