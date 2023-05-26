
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Multi
{
    class Dgn_LeapYear : DgnBase
    {

        public Dgn_LeapYear(object plan) : base(plan) { }

        /// <summary>
        /// 重写基类检定函数
        /// </summary>
        /// <param name="ItemNumber">项目序号</param>
        public override void Verify()
        {
            base.Verify();
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("源输出失败", 6,2);
                return;
            }
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
            string LeapYear = "080228235955";
            string[] arrStrResultKey = new string[BwCount];
            bool[] Result = new bool[BwCount];
            string strKey = ItemKey;
            MessageController.Instance.AddMessage("正在把表时间设置到2008-2-28 23:59:55");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            //System.Windows.Forms.MessageBox.Show("请确认打开电表编程键");

            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = LeapYear.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += LeapYear.Substring(6, 6);
                strShowData[i] = LeapYear;
                strData[i] = strCode[i] + strSetData[i];
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
            bool bResult = true;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    if (!bln_Rst[i])
                        bResult = false;
                }
            }
            if (!bResult)
            {
                MessageController.Instance.AddMessage("修改电表时间失败，是否打开编程开关?", 6, 2);
                return;
            }
            MessageController.Instance.AddMessage("等待6秒");
            Thread.Sleep(6000);
            MessageController.Instance.AddMessage("开始读取表时间...");
            DateTime[] readDateTime = MeterProtocolAdapter.Instance.ReadDateTime();
            //分析结果
            for (int i = 0; i < BwCount; i++)
            {
                Result[i] = (readDateTime[i].Month == 2 && readDateTime[i].Day == 29);
            }
            MessageController.Instance.AddMessage("开始检测29日到3月1日跳转");
            LeapYear = "080229235955";
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = LeapYear.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += LeapYear.Substring(6, 6);
                strShowData[i] = LeapYear;
                strData[i] = strCode[i] + strSetData[i];
            }
             MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
         //   MeterProtocolAdapter.Instance.WriteDateTime(LeapYear);
            MessageController.Instance.AddMessage("等待6秒");
            Thread.Sleep(6000);
            MessageController.Instance.AddMessage("开始读取表时间...");
            readDateTime = MeterProtocolAdapter.Instance.ReadDateTime();

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                //DateTime dt = DateTime.Parse(meterTime[i]);
                Result[i] = (readDateTime[i].Month == 3 && readDateTime[i].Day == 1);
                //挂结论
                MeterDgn _LeatResult = new MeterDgn();
                if (Result[i])
                    _LeatResult.Md_chrValue = Variable.CTG_HeGe;
                else
                    _LeatResult.Md_chrValue = Variable.CTG_BuHeGe;
                _LeatResult.Md_PrjID = strKey;
                _LeatResult.Md_PrjName = Cus_DgnItem.闰年判断功能.ToString();
                Helper.MeterDataHelper.Instance.Meter(i).MeterDgns.Add(strKey, _LeatResult);
                
                
                arrStrResultKey[i] = ItemKey;
            }
            //GPS对时一次
            DateTime gpsTime = Helper.EquipHelper.Instance.ReadGpsTime();
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = LeapYear.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += LeapYear.Substring(6, 6);
                strShowData[i] = LeapYear;
                strData[i] = strCode[i] + strSetData[i];
            }
      MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
          //  MeterProtocolAdapter.Instance.WriteDateTime(gpsTime.ToString("yyMMddhhmmss"));
        //    GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }

        /// <summary>
        /// 清理检定数据
        /// </summary>
        protected override void ClearItemData()
        {
            string strKey = ItemKey;
            MeterBasicInfo curMter = null;
            for (int i = 0; i < BwCount; i++)
            {
                curMter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMter.MeterDgns.ContainsKey(strKey))
                    curMter.MeterDgns.Remove(strKey);
            }
            base.ClearItemData();
        }
    }
}
