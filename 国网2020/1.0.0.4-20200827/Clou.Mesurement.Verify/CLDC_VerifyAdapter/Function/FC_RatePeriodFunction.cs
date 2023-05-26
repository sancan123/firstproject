
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 费率时段功能
    /// </summary>
    class FC_RatePeriodFunction : FunctionBase
    {
        public FC_RatePeriodFunction(object plan)
            : base(plan) 
        {
            
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "两套时区表切换时间", "两套日时段切换时间", "约定冻结数据模式字", "当前正向有功电量",
                                         "切换前运行时区","切换前运行时段", "切换前时区表切换时间", "切换前时段表切换时间", "切换前时区切换正向有功", "切换前时段切换正向有功", 
                                         "切换后运行时区","切换后运行时段", "切换后时区表切换时间", "切换后时段表切换时间", "切换后时区切换正向有功", "切换后时段切换正向有功", 
                                         "恢复后运行时区","恢复后运行时段", "恢复后时区表切换时间", "恢复后时段表切换时间", "恢复后时区切换正向有功", "恢复后时段切换正向有功", 
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
            string[] strReadData = null;
            string strZoneTime = "";            //两套时区表切换时间
            string strPeriodTime = "";          //两套时段表切换时间
            string[] arrBeforeSwitchInfo = null;
            string[] arrAfterSwitchInfo = null;
            string[] arrFinshSwitchInfo = null;

            string[] strRun = new string[BwCount];
            Dictionary<int, float[]> dicEnergyZx = new Dictionary<int, float[]>();  //正向有功电量
            string keyitem = ((int)CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能).ToString().PadLeft(3, '0');
            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];

            //初始化设备
            if (!InitEquipment())
            {
                return;
            }

            if (Stop) return;                   //假如当前停止检定，则退出
            bool[] arrResult = new bool[BwCount];
            bool bResult;

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            MessageController.Instance.AddMessage("读取两套时区表切换时间");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000106", 10);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "两套时区表切换时间", strReadData);
            strCurItem =  "01";

            strZoneTime = strReadData[CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;

                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    _Result.Mf_PrjID = strCurItem;
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
                _Result.Mf_PrjName = CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能.ToString();
                _Result.Mf_chrValue = strReadData[i];
                strResultKey[i] = strCurItem;
                objResultValue[i] = _Result;
                arrResult[i] = true;

                if (strZoneTime != strReadData[i])
                {
                    MessageController.Instance.AddMessage("有表位两套时区表切换时间不一致，试验终止");
                }
            }

            MessageController.Instance.AddMessage("读取两套时段表切换时间");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000107", 10);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "两套日时段切换时间", strReadData);
            strCurItem =  "02";

            strPeriodTime = strReadData[CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;

                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    _Result.Mf_PrjID = strCurItem;
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
                _Result.Mf_PrjName = CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能.ToString();
                _Result.Mf_chrValue = strReadData[i];
                strResultKey[i] = strCurItem;
                objResultValue[i] = _Result;

                if (strPeriodTime != strReadData[i])
                {
                    MessageController.Instance.AddMessage("有表位两套时段表切换时间不一致，试验终止");
                    //Stop = true;
                    //break;
                }
            }
            

            MessageController.Instance.AddMessage("读取约定冻结模式字");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000904", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "约定冻结数据模式字", strReadData);
            strCurItem = "03";
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;
                byte byt_Run;
                string str_Tmp;
                if (strReadData[i] == null || strReadData[i] == "")
                    continue;
                byt_Run = Convert.ToByte(strReadData[i].Substring(0, 2), 16);
                str_Tmp = Convert.ToString(byt_Run, 2).PadLeft(8, '0');


                if (str_Tmp.Substring(str_Tmp.Length - 1, 1) == "1")
                {
                    strRun[i] = "正向有功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 2, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "反向有功";
                    else
                        strRun[i] += "|反向有功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 3, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "组合无功1";
                    else
                        strRun[i] += "|组合无功1";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 4, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "组合无功2";
                    else
                        strRun[i] += "加组合无功2";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 5, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "四象限无功";
                    else
                        strRun[i] += "|四象限无功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 6, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "正向有功最大需量";
                    else
                        strRun[i] += "|正向有功最大需量";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 7, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "反向有功最大需量";
                    else
                        strRun[i] += "|反向有功最大需量";
                }


                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    _Result.Mf_PrjID = strCurItem;
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
                _Result.Mf_PrjName = CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能.ToString();
                _Result.Mf_chrValue = strRun[i];
                strResultKey[i] = strCurItem;
                objResultValue[i] = _Result;
            }
        

            strCurItem = "04";
            dicEnergyZx = ReadEnergy(strCurItem, Cus_PowerFangXiang.正向有功, 0);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                strReadData[i] = dicEnergyZx[i][0].ToString();
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "当前正向有功电量", strReadData);

            string gpsTime = DateTime.Now.ToString("yyMMddHHmmss");
            //将GPS时间写到表中
            
            MeterProtocolAdapter.Instance.WriteDateTime(gpsTime); // 统一表对时
            gpsTime = DateTime.Now.AddYears(1).ToString("yyMMddHHmmss");

            strCurItem = "05";
            arrBeforeSwitchInfo = ReadSwitchInfo(strCurItem);    //读取上一次切换时间


            bResult = StartSwitch("进行两套时区表切换", gpsTime, 1);  //先切换时区表时间

            //gpsTime = DateTime.Now.ToString("yyMMddHHmmss");
            ////将GPS时间写到表中
            //MeterProtocolAdapter.Instance.WriteDateTime(gpsTime); // 统一表对时
            //gpsTime = DateTime.Now.AddYears(1).ToString("yyMMddHHmmss");

            //bResult = StartSwitch("进行两套时段表切换", gpsTime, 2);  //再切时段表时间

            strCurItem = "06";
            arrAfterSwitchInfo = ReadSwitchInfo(strCurItem);

            gpsTime = DateTime.Now.ToString("yyMMddHHmmss");
            //将GPS时间写到表中
        
            MeterProtocolAdapter.Instance.WriteDateTime(gpsTime); // 统一表对时
            gpsTime = DateTime.Now.AddYears(1).ToString("yyMMddHHmmss");

            bResult = StartSwitch("恢复两套时区表切换", gpsTime, 1);

            //gpsTime = DateTime.Now.ToString("yyMMddHHmmss");
            ////将GPS时间写到表中
            //MeterProtocolAdapter.Instance.WriteDateTime(gpsTime); // 统一表对时
            //gpsTime = DateTime.Now.AddYears(1).ToString("yyMMddHHmmss");

            //bResult = StartSwitch("恢复两套时区表切换", gpsTime, 2);

            strCurItem ="07";
            arrFinshSwitchInfo = ReadSwitchInfo(strCurItem);

            MessageController.Instance.AddMessage("恢复两套时区表切换时间");
            MeterProtocolAdapter.Instance.WriteSwitchTime(1, strZoneTime);

            MessageController.Instance.AddMessage("恢复两套时段表切换时间");
            MeterProtocolAdapter.Instance.WriteSwitchTime(2, strPeriodTime);


            if (Stop) return;
            MessageController.Instance.AddMessage("恢复电表时间");

            DateTime readTime = DateTime.Now;
            //if (GlobalUnit.g_SystemConfig.methodAndBasis.getValue(Variable.CTC_TM_GPSGETT) != "取电脑时间")
            {
                MessageController.Instance.AddMessage("开始读取GPS时间...");
                readTime = Helper.EquipHelper.Instance.ReadGpsTime();  //读取GPS时间
            }

            bool[] arrSetTimeResult = MeterProtocolAdapter.Instance.WriteDateTime(readTime.ToString("yyMMddHHmmss"));

            if (Stop) return;
            bool bReturnResult = true;
            string strMessageText = "";
            for (int i = 0; i < CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    if (!arrSetTimeResult[i])
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
                //Stop = true;
                MessageController.Instance.AddMessage(strMessageText);
                MessageController.Instance.AddMessage("有电能表写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止");
                //return;
            }
            if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                string[] arrBefore = null;
                string[] arrAfter = null;
                string[] arrFinsh = null;

                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);



                if (arrBeforeSwitchInfo[i].IndexOf("|") == -1 || arrAfterSwitchInfo[i].IndexOf("|") == -1 || arrFinshSwitchInfo[i].IndexOf("|") == -1)
                {
                    arrResult[i] = false;
                }
                else
                {
                    arrBefore = arrBeforeSwitchInfo[i].Split('|');
                    arrAfter = arrAfterSwitchInfo[i].Split('|');
                    arrFinsh = arrFinshSwitchInfo[i].Split('|');

                    if ((arrBefore.Length < 0) || (arrAfter.Length < 0) || (arrFinsh.Length < 0))
                        arrResult[i] = false;
                    else if (arrBefore[0] != arrFinsh[0] || arrBefore[0] == arrAfter[0])
                        arrResult[i] = false;
                }
                string strResult = arrResult[i] ? CLDC_DataCore.Const.Variable.CTG_HeGe : CLDC_DataCore.Const.Variable.CTG_BuHeGe;
                ResultDictionary["结论"][i] = strResult;
            }
            UploadTestResult("结论");
        }

        /// <summary>
        /// 开始切换
        /// </summary>
        /// <param name="strCurItem"></param>
        /// <param name="strSwitchInfo"></param>
        /// <param name="strSetSwitchTime"></param>
        /// <param name="int_SwitchType">切换类型</param>
        /// <returns></returns>
        private bool StartSwitch(string strSwitchInfo, string strSetSwitchTime, int int_SwitchType)
        {
            DateTime dtMeterTime = DateTime.Now;

            GlobalUnit.g_MsgControl.OutMessage(strSwitchInfo, false);

            //strSetSwitchTime += "00";
            dtMeterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strSetSwitchTime);

            dtMeterTime = dtMeterTime.AddSeconds(-15);

            MessageController.Instance.AddMessage("设置两套时区表切换时间");
            MeterProtocolAdapter.Instance.WriteSwitchTime(1, strSetSwitchTime);

            MessageController.Instance.AddMessage("设置两套时段表切换时间");
            MeterProtocolAdapter.Instance.WriteSwitchTime(2, strSetSwitchTime);

            #region 将电表时间修改切换时间前15秒

            MessageController.Instance.AddMessage("正在进行" + strSwitchInfo + ",将电表时间修改到" + dtMeterTime.ToString("yyMMddHHmmss"));

            bool[] result = MeterProtocolAdapter.Instance.WriteDateTime(dtMeterTime.ToString("yyMMddHHmmss"));
            bool bReturnResult = true;
            string strMessageText = "";
            for (int i = 0; i < CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
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
                //Stop = true;
                MessageController.Instance.AddMessage(strMessageText);
                MessageController.Instance.AddMessage("有电能表写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止");
                //return false;
            }
            #endregion

            int _MaxStartTime = 60;
            m_StartTime = DateTime.Now;
            while (true)
            {
                //每一秒刷新一次数据
                long _PastTime = base.VerifyPassTime;
                System.Threading.Thread.Sleep(1000);

                float pastMinute = _PastTime / 60F;
                CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                string strDes = string.Format("正在进行" + strSwitchInfo + ",电表时间从" + dtMeterTime.ToString() + "运行,需要", PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                MessageController.Instance.AddMessage(strDes);

                if ((_PastTime >= _MaxStartTime) || Stop)
                {
                    CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                    break;
                }

                if (Stop) return false;
            }


            return true;
        }

        private string[] ReadSwitchInfo(string strCurItem)
        {
            string[] strResult = new string[BwCount];
            string[] arrStatusZone = new string[BwCount];
            string[] arrStatusPeriod = new string[BwCount];
            string[] strReadData = null;
            string[] strZoneTime = null;
            string[] strPeriodTime = null;
            string[] strZoneEnergy = null;
            string[] strPeriodEnergy = null;

            string strTitleName = "";
            if (strCurItem == "05")
            {
                strTitleName = "切换前";
            }
            else if(strCurItem == "06")
            {
                strTitleName = "切换后";
            }
            else
            {
                strTitleName = "恢复后";
            }

            MessageController.Instance.AddMessage("读取状态运行字3");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000503", 4);

            for (int i = 0; i < CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                byte byt_Run;
                string str_Tmp;
                if (strReadData[i] == null || strReadData[i] == "")
                    continue;

                int warningValue = Convert.ToInt32(strReadData[i], 16);

                if ((warningValue & 0x01) == 0x01)
                {
                    arrStatusPeriod[i] = "第二套";
                }
                else
                {
                    arrStatusPeriod[i] = "第一套";
                }

                if ((warningValue & 0x20) == 0x20)
                {
                    arrStatusZone[i] = "第二套";
                }
                else
                {
                    arrStatusZone[i] = "第一套";
                }
            }

            if (Stop) return null;
            MessageController.Instance.AddMessage("读取上1次两套时区表切换时间");
            strZoneTime = MeterProtocolAdapter.Instance.ReadData("05020001", 10);
            if (Stop) return null;
            MessageController.Instance.AddMessage("读取上1次两套时段表切换时间");
            strPeriodTime = MeterProtocolAdapter.Instance.ReadData("05030001", 10);
            if (Stop) return null;
            MessageController.Instance.AddMessage("读取上1次两套时区表切换电量");
            strZoneEnergy = MeterProtocolAdapter.Instance.ReadData("05020101", 40);
            if (Stop) return null;
            MessageController.Instance.AddMessage("读取上1次两套时段表切换电量");
            strPeriodEnergy = MeterProtocolAdapter.Instance.ReadData("05030101", 40);
            if (Stop) return null;
            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                //结论
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    _Result.Mf_PrjID = strCurItem;
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
                _Result.Mf_PrjName = CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能.ToString();
                if (strZoneEnergy[j].Length > 8)
                {
                    strZoneEnergy[j] = strZoneEnergy[j].Substring(strZoneEnergy[j].Length - 8, 8);
                    strZoneEnergy[j] = (Convert.ToDouble(strZoneEnergy[j]) / 100).ToString("F2");
                }
                if (strPeriodEnergy[j].Length > 8)
                {
                    strPeriodEnergy[j] = strPeriodEnergy[j].Substring(strPeriodEnergy[j].Length - 8, 8);
                    strPeriodEnergy[j] = (Convert.ToDouble(strPeriodEnergy[j]) / 100).ToString("F2");
                }
                _Result.Mf_chrValue = arrStatusZone[j] + "|" + arrStatusPeriod[j] + "|" + strZoneTime[j] + "|" + strPeriodTime[j] + "|" + strZoneEnergy[j] + "|" + strPeriodEnergy[j];
                strResult[j] = _Result.Mf_chrValue;

                ResultDictionary[strTitleName + "运行时区"][j] = arrStatusZone[j];
                ResultDictionary[strTitleName + "运行时段"][j] = arrStatusPeriod[j];
                ResultDictionary[strTitleName + "时区表切换时间"][j] = strZoneTime[j];
                ResultDictionary[strTitleName + "时段表切换时间"][j] = strPeriodTime[j];
                ResultDictionary[strTitleName + "时区切换正向有功"][j] = strZoneEnergy[j];
                ResultDictionary[strTitleName + "时段切换正向有功"][j] = strPeriodEnergy[j];
            }
            UploadTestResult(strTitleName + "运行时区");
            UploadTestResult(strTitleName + "运行时段");
            UploadTestResult(strTitleName + "时区表切换时间");
            UploadTestResult(strTitleName + "时段表切换时间");
            UploadTestResult(strTitleName + "时区切换正向有功");
            UploadTestResult(strTitleName + "时段切换正向有功");

            return strResult;
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



        private Dictionary<int, float[]> ReadEnergy(string strCurItem, Cus_PowerFangXiang fangxiang, int int_FreezeTimes)
        {

            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();

            string strMessage;
            string strEnergyType;
            switch (fangxiang)
            {
                case Cus_PowerFangXiang.组合有功:
                    strEnergyType = "组合有功";
                    break;
                case Cus_PowerFangXiang.正向有功:
                    strEnergyType = "正向有功";
                    break;
                case Cus_PowerFangXiang.正向无功:
                    strEnergyType = "正向无功";
                    break;
                case Cus_PowerFangXiang.反向有功:
                    strEnergyType = "反向有功";
                    break;
                case Cus_PowerFangXiang.反向无功:
                    strEnergyType = "反向无功";
                    break;
                case Cus_PowerFangXiang.第一象限无功:
                    strEnergyType = "第一象限无功";
                    break;
                case Cus_PowerFangXiang.第二象限无功:
                    strEnergyType = "第二象限无功";
                    break;
                case Cus_PowerFangXiang.第三象限无功:
                    strEnergyType = "第三象限无功";
                    break;
                case Cus_PowerFangXiang.第四象限无功:
                    strEnergyType = "第四象限无功";
                    break;
                default:
                    strEnergyType = "正向有功";
                    break;
            }
            if (int_FreezeTimes == 0)
                strMessage = string.Format("读取【当前{0}】电量", strEnergyType);
            else
                strMessage = string.Format("读取【上{0}结算日{1}】电量", int_FreezeTimes, strEnergyType);
            MessageController.Instance.AddMessage(strMessage);
            dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((byte)((int)fangxiang), int_FreezeTimes);

            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                if (dicEnergy.ContainsKey(j) == false) continue;
                //总结论
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    _Result.Mf_PrjID = strCurItem;
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
                _Result.Mf_PrjName = CLDC_Comm.Enum.Cus_FunctionItem.费率时段功能.ToString();

                _Result.Mf_chrValue = "";
                if (dicEnergy[j].Length >= 5)
                    _Result.Mf_chrValue = string.Format("{0}", dicEnergy[j][0].ToString("F2"));

            }
            return dicEnergy;
        }
    }
}
