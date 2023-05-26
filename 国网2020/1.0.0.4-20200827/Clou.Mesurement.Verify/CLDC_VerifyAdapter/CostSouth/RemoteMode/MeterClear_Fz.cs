using CLDC_DataCore;
using CLDC_DataCore.Function;
using System;
namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程清零(辅助功能)
    /// </summary>
    public class MeterClear_Fz : VerifyBase
    {
        protected override string ItemKey
        {
           // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get{return null;}
        }

        public MeterClear_Fz(object plan)
            : base(plan)
        {
        }


        /// 费控模式
        ///当前正向有功总
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "清零命令", "费控模式", "当前正向有功总", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] strID = new string[BwCount];
            int[] iFlag = new int[BwCount];

            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];

            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int iSelectBwCount = 0;

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
            bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);

            if (Stop) return;
            float[] energys = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取费控模式状态位");
            string[] status1 = MeterProtocolAdapter.Instance.ReadData("04000501", 2);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["清零命令"][i] = blnMeterClearRet[i] ? "正常应答" : "异常应答";
                    ResultDictionary["费控模式"][i] = ((Convert.ToInt32(status1[i], 16) & 0x0040) == 0x0040) ? "远程" : "本地";
                    ResultDictionary["当前正向有功总"][i] = energys[i].ToString();
                }
            }
            UploadTestResult("当前正向有功总");
            UploadTestResult("清零命令");
            UploadTestResult("费控模式");

            //判断结论
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnMeterClearRet[i] == true && energys[i] == 0f)
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
    }
}
