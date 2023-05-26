
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// GPS授时试验
    /// 试验方法，先读取GPS时间，然后将电能表时间修改到GPS时间。
    /// </summary>
    class Dgn_GPS : DgnBase
    {
        public Dgn_GPS(object plan) : base(plan) { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }

        /// <summary>
        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
            base.Verify();
            if (!PowerOn()) return;
            string[] arrStrResultKey = new string[BwCount];
            string gpsTime = string.Empty;

            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] strMeterTime = new string[BwCount];
            string[] strShowData = new string[BwCount];

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();


            if (Stop) return ;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取GPS时间...");
            DateTime dateGPS = Helper.EquipHelper.Instance.ReadGpsTime();
         //   DateTime dateGPS = DateTime.Now;
            if (Stop) return ;
            MessageController.Instance.AddMessage("开始写表时间......");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = dateGPS.ToString("yyMMdd") + "0" + (int)dateGPS.DayOfWeek;
                strSetData[i] += dateGPS.ToString("HHmmss");
                strShowData[i] = dateGPS.ToString("yyMMddHHmmss");
                strData[i] = strCode[i] + strSetData[i];
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            MessageController.Instance.AddMessage("正在读取表内时间...");
            DateTime[] readTime = MeterProtocolAdapter.Instance.ReadDateTime();


            MessageController.Instance.AddMessage("正在处理结果.....");
            for (int bw = 0; bw < result.Length; bw++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(bw).YaoJianYn)
                    continue;
                ResultDictionary["检定数据"][bw] = readTime[bw].ToString();
                ResultDictionary["结论"][bw] = bln_Rst[bw] ? CLDC_DataCore.Const.Variable.CTG_HeGe : CLDC_DataCore.Const.Variable.CTG_BuHeGe;
            }
            UploadTestResult("检定数据");
            UploadTestResult("结论");
        }
    }
}
