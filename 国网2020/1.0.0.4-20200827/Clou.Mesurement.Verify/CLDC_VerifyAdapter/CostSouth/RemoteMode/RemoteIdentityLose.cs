using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using System;
using CLDC_DataCore.Function;
namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 身份认证失效测试
    /// </summary>
    public class RemoteIdentityLose :VerifyBase
    {
        public RemoteIdentityLose(object plan) : base(plan)
        { }
        
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //这里要解析检定参数

            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "操作前身份认证状态1", "操作后身份认证状态1","下发身份认证失效命令1",
                                         "身份认证失效后不可密钥更新2", "身份认证失效后不可远程拉闸2", "身份认证失效后不可设置日期时间2",
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
            bool[,] blnRet = new bool[BwCount, 6];
            string[] str_ID = new string[BwCount];
            string[] str_Data = new string[BwCount];
            string[] str_Apdu = new string[BwCount];
            string strID = "";
            string[] strSyTime = new string[BwCount];
            bool[] result = new bool[BwCount];
            int[] iFlag = new int[BwCount];

            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            //1-----------------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            string[] status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x2000) == 0x2000)
                    {
                        status[i] = "有效";
                        blnRet[i, 0] = true;
                    }
                    else
                    {
                        status[i] = "无效";
                    }
                }
                else
                {
                    status[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作前身份认证状态1", status);

            MessageController.Instance.AddMessage("正在下发身份认证失效命令,请稍候....");
            if (Stop) return;
            result = MeterProtocolAdapter.Instance.SouthCmdNoData("070002FF");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x2000) == 0x2000)
                    {
                        status[i] = "有效";
                    }
                    else
                    {
                        status[i] = "无效";
                        blnRet[i, 1] = true;
                    }
                }
                else
                {
                    status[i] = "异常";
                }
                if (result[i])
                {
                    ResultDictionary["下发身份认证失效命令1"][i] = "通过";
                    blnRet[i, 2] = true;
                }
                else
                {
                    ResultDictionary["下发身份认证失效命令1"][i] = "不通过";
                }
            }
            UploadTestResult("下发身份认证失效命令1");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "操作后身份认证状态1", status);

           //2--------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 3] = result[i];
                ResultDictionary["身份认证失效后不可密钥更新2"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("身份认证失效后不可密钥更新2");


            if (Stop) return;
            Common.Memset(ref str_Data,"1A00" + DateTime.Now.ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, str_Data);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 4] = result[i];
                ResultDictionary["身份认证失效后不可远程拉闸2"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("身份认证失效后不可远程拉闸2");


            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            Common.Memset(ref str_ID, "0400010C");
            Common.Memset(ref str_Data, "0400010C" + DateTime.Now.ToString("yyMMdd") + (int)DateTime.Now.DayOfWeek + DateTime.Now.ToString("HHmmss").PadLeft(2, '0'));
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 5] = result[i];
                ResultDictionary["身份认证失效后不可设置日期时间2"][i] = !result[i] ? "通过" : "不通过";
            }
            UploadTestResult("身份认证失效后不可设置日期时间2");

          //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && !blnRet[i, 3] && !blnRet[i, 4] && !blnRet[i, 5])
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
