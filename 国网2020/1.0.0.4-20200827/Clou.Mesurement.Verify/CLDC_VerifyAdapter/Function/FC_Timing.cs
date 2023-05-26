
using System;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 计时功能
    /// </summary>
    class FC_Timing : FunctionBase
    {
        public FC_Timing(object plan)
            : base(plan)
        {

        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "【23点50分前】校时前时间", "【23点50分前】校时时间", "【23点50分前】校时后时间","【23点50分前】结论",
                                         "【23点57分】校时前时间", "【23点57分】校时时间", "【23点57分】校时后时间","【23点57分】结论",
                                         "【零点1分】校时前时间", "【零点1分】校时时间", "【零点1分】校时后时间","【零点1分】结论",
                                         "【小于零点10分】校时前时间", "【小于零点10分】校时时间", "【小于零点10分】校时后时间","【小于零点10分】结论",
                                         "【大于零点10分】校时前时间", "【大于零点10分】校时时间", "【大于零点10分】校时后时间","【大于零点10分】结论",
                                         "【一天一次】校时前时间", "【一天一次】校时时间", "【一天一次】校时后时间","【一天一次】结论",
                                         "【一天重复】校时前时间", "【一天重复】校时时间", "【一天重复】校时后时间","【一天重复】结论",
                                         "结论"};
            return true;
            
        }

        private MeterFunction _Result = null;
        private MeterBasicInfo curMeter;
        /// <summary>
        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
            base.Verify();
            if (Stop) return;                   //假如当前停止检定，则退出
            string strCurItem = "";

            string ItemKey = ((int)CLDC_Comm.Enum.Cus_FunctionItem.计时功能).ToString().PadLeft(3, '0');
            string[] strRun = new string[BwCount];

            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];


            //初始化设备
            if (!InitEquipment())
            {
                return;
            }

            if (Stop) return;                   //假如当前停止检定，则退出
            bool[] bResult = new bool[BwCount];

            string[] arrBroadCastInfo = new string[] { "【23点50分前】广播校时",  "【23点57分】广播校时", "【零点1分】广播校时", 
                                                       "【小于零点10分】广播校时", "【大于零点10分】广播校时",
                                                        "【一天一次】广播校时", "【一天重复】广播校时" };
            string[] arrBroadCastTime = new string[] { "234600",  "235500", "235900",  "000500", "012800","034000", "042800"};
            string[] arrSetMeterTime = new string[]  { "234800",  "235700", "000100",  "000700", "013000","033000", "043000" };

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            #region 准备步骤：先当天校时一次

            if (Stop) return;
            MessageController.Instance.AddMessage("设置当前时间为23：45：00");
            string strTime = DateTime.Now.ToString("yyMMdd" + "234500");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(strTime);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在广播校时");
            if (GlobalUnit.g_CommunType == Cus_CommunType.通讯蓝牙)
            {
                MeterProtocolAdapter.Instance.BroadCastTimeByPoint(CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strTime));
            }
            else
            {
                MeterProtocolAdapter.Instance.BroadCastTime(CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strTime));
            }
            #endregion


            for (int i = 0; i < arrBroadCastTime.Length; i++)
            {
                if (Stop) return;
                strCurItem = ItemKey + (i + 1).ToString("D2");
                bResult = StartBroadCastTime(strCurItem, arrBroadCastInfo[i], arrSetMeterTime[i], arrBroadCastTime[i]);
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("恢复电表时间");
            bResult = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            if (Stop) return;
            bool bReturnResult = true;
            string strMessageText = "";
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    if (!bResult[i])
                    {
                        bReturnResult = false;
                        strMessageText += (i + 1).ToString() + "号,";
                    }
                }
            }
            if (!bReturnResult)
            {
                strMessageText = strMessageText.Trim(',');
                strMessageText += "表位修改时间失败，试验停止";
                Stop = true;
                MessageController.Instance.AddMessage(strMessageText);

                //if (GlobalUnit.NetState == CLDC_Comm.Enum.Cus_NetState.DisConnected)
                //    MessageController.Instance.AddMessage(strMessageText);

                MessageController.Instance.AddMessage("有电能表写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止");
                //return;
            }
            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (ResultDictionary["【23点50分前】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【23点57分】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【零点1分】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【小于零点10分】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【大于零点10分】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【一天一次】结论"][i] == Variable.CTG_HeGe
                    && ResultDictionary["【一天重复】结论"][i] == Variable.CTG_HeGe)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
            }

            UploadTestResult("结论");
        }


        private bool[] StartBroadCastTime(string strCurItem, string strBroadCastInfo, string strSetMeterTime, string strBroadCastTime)
        {
            int int_Item = 0;
            DateTime dtMeterTime = DateTime.Now;
            string strTime;
            bool[] bReturn = new bool[BwCount];
            DateTime[] dtBefMeterTime = new DateTime[BwCount];
            DateTime[] dtCurMeterTime = new DateTime[BwCount];
            string[] strCurMeterTime = new string[BwCount];
            string strMessage = "";
            string strResult = "";

            MessageController.Instance.AddMessage(strBroadCastInfo);

            strTime = DateTime.Now.Date.ToString("yyMMdd") + "000000";
            dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strTime);

            dtMeterTime = dtMeterTime.AddSeconds(-5);
            int_Item = Convert.ToInt32(strCurItem.Substring(3, 2));
            dtMeterTime = dtMeterTime.AddDays(int_Item);

            if (int_Item < 8)
            {
                #region 将电表时间修改0点的前1分钟

                MessageController.Instance.AddMessage("正在进行" + strBroadCastInfo + ",将电表时间修改到" + dtMeterTime.ToString("yyMMddHHmmss"));
                bool[] result = MeterProtocolAdapter.Instance.WriteDateTime(dtMeterTime.ToString("yyMMddHHmmss"));
                bool bReturnResult = true;
                string strMessageText = "";
                for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
                {
                    if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                    {
                        if (!result[i])
                        {
                            bReturnResult = false;
                            strMessageText += (i + 1).ToString() + "号,";
                        }
                    }
                }
                if (!bReturnResult)
                {
                    strMessageText = strMessageText.Trim(',');
                    strMessageText += "表位修改时间失败，试验停止";
                    Stop = true;
                    MessageController.Instance.AddMessage(strMessageText);

                    //if (GlobalUnit.NetState == CLDC_Comm.Enum.Cus_NetState.DisConnected)
                    //    MessageController.Instance.AddMessage(strMessageText);

                    MessageController.Instance.AddMessage("有电能表写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止");
                    //return null;
                }

                int _MaxStartTime = 20;
                m_StartTime = DateTime.Now;
                while (true)
                {
                    //每一秒刷新一次数据
                    long _PastTime = base.VerifyPassTime;
                    System.Threading.Thread.Sleep(1000);

                    float pastMinute = _PastTime / 60F;
                    GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = string.Format("正在进行" + strBroadCastInfo + ",电表时间从" + dtMeterTime.ToString() + "运行过零点需要", PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                    MessageController.Instance.AddMessage(strDes);

                    if ((_PastTime >= _MaxStartTime) || Stop)
                    {
                        GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }

                    if (Stop) return null;
                }
                #endregion
            }

            if (int_Item <= 2)
                dtMeterTime = dtMeterTime.AddDays(1);
            else if (int_Item == 7)
                dtMeterTime = dtMeterTime.AddDays(-1);

            strSetMeterTime = dtMeterTime.ToString("yyMMdd") + strSetMeterTime;
            strMessage = string.Format("将电表时间修改到{0}", strSetMeterTime);
            MessageController.Instance.AddMessage("正在进行" + strBroadCastInfo + "," + strMessage);
            bReturn = MeterProtocolAdapter.Instance.WriteDateTime(strSetMeterTime);

            if (Stop) return null;
            MessageController.Instance.AddMessage("正在进行" + strBroadCastInfo + "," + "读取电表时间");
            dtBefMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

            if (Stop) return null;
            strBroadCastTime = dtMeterTime.ToString("yyMMdd") + strBroadCastTime;
            strMessage = string.Format("将电表时间广播校时到{0}", strBroadCastTime);
            MessageController.Instance.AddMessage("正在进行" + strBroadCastInfo + "," + strMessage);
            dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
            if (GlobalUnit.g_CommunType == Cus_CommunType.通讯蓝牙)
            {
                MeterProtocolAdapter.Instance.BroadCastTimeByPoint(dtMeterTime);
            }
            else
            {
                MeterProtocolAdapter.Instance.BroadCastTime(dtMeterTime);
            }
            System.Threading.Thread.Sleep(2000);

            if (Stop) return null;
            MessageController.Instance.AddMessage("正在进行" + strBroadCastInfo + "," + "读取电表时间");
            dtCurMeterTime = MeterProtocolAdapter.Instance.ReadDateTime();

            if (Stop) return null;
            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                
                dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strBroadCastTime);
                int diffTime = CLDC_DataCore.Function.DateTimes.DateDiff(dtMeterTime, dtCurMeterTime[j]);
                if (int_Item == 1 || int_Item == 5 || int_Item == 6)
                {
                    if (diffTime <= 20)
                    {
                        strResult = "合格";
                        bReturn[j] = true;
                    }
                    else
                    {
                        strResult = "不合格";
                        bReturn[j] = false;
                    }
                }
                else
                {
                    if (diffTime <= 20)
                    {
                        strResult = "不合格";
                        bReturn[j] = false;
                    }
                    else
                    {
                        strResult = "合格";
                        bReturn[j] = true;
                    }
                }

                switch (strBroadCastInfo)
                {
                    case "【23点50分前】广播校时":
                        ResultDictionary["【23点50分前】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【23点50分前】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【23点50分前】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【23点50分前】结论"][j] = strResult;
                        break;
                    case "【23点57分】广播校时":
                        ResultDictionary["【23点57分】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【23点57分】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【23点57分】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【23点57分】结论"][j] = strResult;
                        break;
                    case "【零点1分】广播校时":
                        ResultDictionary["【零点1分】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【零点1分】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【零点1分】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【零点1分】结论"][j] = strResult;
                        break;
                    case "【小于零点10分】广播校时":
                        ResultDictionary["【小于零点10分】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【小于零点10分】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【小于零点10分】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【小于零点10分】结论"][j] = strResult;
                        break;
                    case "【大于零点10分】广播校时":
                        ResultDictionary["【大于零点10分】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【大于零点10分】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【大于零点10分】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【大于零点10分】结论"][j] = strResult;
                        break;
                    case "【一天一次】广播校时":
                        ResultDictionary["【一天一次】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【一天一次】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【一天一次】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【一天一次】结论"][j] = strResult;
                        break;
                    case "【一天重复】广播校时":
                        ResultDictionary["【一天重复】校时前时间"][j] = dtBefMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【一天重复】校时时间"][j] = strBroadCastTime;
                        ResultDictionary["【一天重复】校时后时间"][j] = dtCurMeterTime[j].ToString("yyMMddHHmmss");
                        ResultDictionary["【一天重复】结论"][j] = strResult;
                        break;
                    default:
                        break;
                }
                
                UploadTestResult("【23点50分前】校时前时间");
                UploadTestResult("【23点50分前】校时时间");
                UploadTestResult("【23点50分前】校时后时间");
                UploadTestResult("【23点50分前】结论"); 
                UploadTestResult("【23点57分】校时前时间");
                UploadTestResult("【23点57分】校时时间");
                UploadTestResult("【23点57分】校时后时间");
                UploadTestResult("【23点57分】结论");
                UploadTestResult("【零点1分】校时前时间");
                UploadTestResult("【零点1分】校时时间");
                UploadTestResult("【零点1分】校时后时间");
                UploadTestResult("【零点1分】结论");
                UploadTestResult("【小于零点10分】校时前时间");
                UploadTestResult("【小于零点10分】校时时间");
                UploadTestResult("【小于零点10分】校时后时间");
                UploadTestResult("【小于零点10分】结论");
                UploadTestResult("【大于零点10分】校时前时间");
                UploadTestResult("【大于零点10分】校时时间");
                UploadTestResult("【大于零点10分】校时后时间");
                UploadTestResult("【大于零点10分】结论");
                UploadTestResult("【一天一次】校时前时间");
                UploadTestResult("【一天一次】校时时间");
                UploadTestResult("【一天一次】校时后时间");
                UploadTestResult("【一天一次】结论");
                UploadTestResult("【一天重复】校时前时间");
                UploadTestResult("【一天重复】校时时间");
                UploadTestResult("【一天重复】校时后时间");
                UploadTestResult("【一天重复】结论");
            }

            return bReturn;
        }

        /// <summary>
        /// 初始化设备参数,计算每一块表需要检定的圈数
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (Stop) return false;                   //假如当前停止检定，则退出            
            MessageController.Instance.AddMessage("开始升电压...");
            if (Stop) return false;                   //假如当前停止检定，则退出
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("升电压失败! ");
                return false;
            }
            if (Stop) return false;                   //假如当前停止检定，则退出

            return true;
        }

    }
}
