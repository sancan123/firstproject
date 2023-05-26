using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 红外认证功能
    /// </summary>
    public class InfraredAuth : VerifyBase
    {
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


        public InfraredAuth(object plan)
            : base(plan)
        {
        }


        //日期时间不可设置(未红外认证)|下发日期时间|读取日期时间|日期时间设置(已红外认证)

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "日期时间不可设置（未红外认证）", "下发日期时间", "读取日期时间", "日期时间设置（已红外认证）", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();

            bool[,] blnRet = new bool[BwCount, 3];
            string[] str_ID = new string[BwCount];
            string[] str_Data = new string[BwCount];
            string[] strERand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            string[] strRand1 = new string[BwCount];
            string[] strRand2 = new string[BwCount];
            string[] strERand1 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strData = new string[BwCount];//明文
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];

            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);


            GlobalUnit.g_CommunType = Cus_CommunType.通讯红外;
            //1------------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("不经过红外认证，直接通过红外口发送设置日期和时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                str_ID[i] = "0400010C";
                str_Data[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                str_Data[i] += DateTime.Now.ToString("HHmmss");
                strRand2[i] = "00000000";
            }
            Common.Memset(ref iFlag, 0);
            bool[] bln_Rst1 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!bln_Rst1[i])
                {
                    ResultDictionary["日期时间不可设置（未红外认证）"][i] = "通过";
                    blnRet[i, 0] = true;
                }
                else
                {
                    ResultDictionary["日期时间不可设置（未红外认证）"][i] = "不通过";
                }
            }
            UploadTestResult("日期时间不可设置（未红外认证）");




            //2------------------


            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行红外认证查询,请稍候....");
            bool[] bln_Rst2 = MeterProtocolAdapter.Instance.SouthInfraredRand(out strEsamNo, out strRand1, out strERand1, out strRand2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行红外认证,请稍候....");
            bool[] bln_Rst3 = MeterProtocolAdapter.Instance.SouthInfraredAuth(iFlag, strEsamNo, strRand1, strERand1, strRand2, out strERand2);

            if (Stop) return;
            // 3----------------
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            string strDatetime = "49-06-07";
            DateTime dt = DateTime.Parse(strDatetime, DateTimeFormatInfo.CurrentInfo);
            string strSetDatatime = "49060701080000";
            MessageController.Instance.AddMessage("正在通过红外口发送设置日期和时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                str_ID[i] = "0400010C";
                str_Data[i] = "0400010C" + strSetDatatime;
            }
            //
            bool[] bln_Rst4 = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (bln_Rst4[i])
                {
                    blnRet[i, 1] = true;
                }

                ResultDictionary["下发日期时间"][i] = strSetDatatime;
            }
            UploadTestResult("下发日期时间");

            GlobalUnit.g_CommunType = Cus_CommunType.通讯485;
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);


            if (Stop) return;
            string[] strMeterDatetime = MeterProtocolAdapter.Instance.ReadData("0400010C", 7);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["读取日期时间"][i] = strMeterDatetime[i];
                if (strMeterDatetime[i].Length == 14)
                {
                    strMeterDatetime[i] = strMeterDatetime[i].Substring(0, 6) + strMeterDatetime[i].Substring(14 - 6, 6);

                    int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime("490607080000"), DateTimes.FormatStringToDateTime(strMeterDatetime[i]));
                    if (iErr <= 300)
                    {
                        blnRet[i, 2] = true;
                    }
                    ResultDictionary["日期时间设置（已红外认证）"][i] = blnRet[i, 2] ? "成功" : "失败";
                }
            }

            UploadTestResult("读取日期时间");
            UploadTestResult("日期时间设置（已红外认证）");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strRevCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


            MessageController.Instance.AddMessage("正在处理结果,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {

                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            UploadTestResult("结论");

        }

    }
}
