
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 最大需量功能
    /// </summary>
    class FC_MaxDemandFunction : FunctionBase
    {
        public FC_MaxDemandFunction(object plan)
            : base(plan) 
        {
            
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
            int _MaxStartTime;
            DateTime dtFreezeTime;
            string strFreezeTime = "";
            ResultNames = new string[] { "正向有功需量", "反向有功需量", "第一象限需量", "第二象限需量", "第三象限需量", "第四象限需量",
                                         "组合无功1", "组合无功2", "正向有功需量上1月", "反向有功需量上1月", "第一象限需量上1月", "第二象限需量上1月", 
                                         "第三象限需量上1月", "第四象限需量上1月", "合无功1上1月", "组合无功1上1月", "结论" };
            string[] strReadData = null;
            string[] strWgZdz1 = new string[BwCount];
            string[] strWgZdz2 = new string[BwCount];

            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            string keyitem = ((int)CLDC_Comm.Enum.Cus_FunctionItem.最大需量功能).ToString().PadLeft(3, '0');

            Dictionary<int, float[]> dicDemandZx = new Dictionary<int, float[]>();  //正向有功需量
            Dictionary<int, float[]> dicDemandFx = new Dictionary<int, float[]>();  //反向有功需量
            Dictionary<int, float[]> dicDemandXiangxian1 = new Dictionary<int, float[]>();  //第一象限需量
            Dictionary<int, float[]> dicDemandXiangxian2 = new Dictionary<int, float[]>();  //第二象限需量
            Dictionary<int, float[]> dicDemandXiangxian3 = new Dictionary<int, float[]>();  //第三象限需量
            Dictionary<int, float[]> dicDemandXiangxian4 = new Dictionary<int, float[]>();  //第四象限需量
            Dictionary<int, float[]> dicDemandZhwg1 = new Dictionary<int, float[]>();  //组合无功1
            Dictionary<int, float[]> dicDemandZhwg2 = new Dictionary<int, float[]>();  //组合无功2

            Dictionary<int, float[]> dicDemandSyZx = new Dictionary<int, float[]>();  //正向有功需量上1月
            Dictionary<int, float[]> dicDemandSyFx = new Dictionary<int, float[]>();  //反向有功需量上1月
            Dictionary<int, float[]> dicDemandSyXiangxian1 = new Dictionary<int, float[]>();  //第一象限需量上1月
            Dictionary<int, float[]> dicDemandSyXiangxian2 = new Dictionary<int, float[]>();  //第二象限需量上1月
            Dictionary<int, float[]> dicDemandSyXiangxian3 = new Dictionary<int, float[]>();  //第三象限需量上1月
            Dictionary<int, float[]> dicDemandSyXiangxian4 = new Dictionary<int, float[]>();  //第四象限需量上1月
            Dictionary<int, float[]> dicDemandSyZhwg1 = new Dictionary<int, float[]>();  //组合无功1上1月
            Dictionary<int, float[]> dicDemandSyZhwg2 = new Dictionary<int, float[]>();  //组合无功1上1月

            //Dictionary<int, float[]> dicDemandZx1 = new Dictionary<int, float[]>(); //上1结算日正向有功需量
            //Dictionary<int, float[]> dicDemandFx1 = new Dictionary<int, float[]>(); //上1结算日反向有功需量
            //Dictionary<int, float[]> dicDemandZh1 = new Dictionary<int, float[]>(); //上1结算日组合有功需量
            //Dictionary<int, float[]> dicDemandZh2 = new Dictionary<int, float[]>(); //电表实际组合有功需量
            for (int i = 1; i <= 19; i++)
            {
                strCurItem = ItemKey + i.ToString("D2");
                ClearItemData(strCurItem);
            }
           
            //初始化设备
            if (!InitEquipment())
            {
                return;
            }

            if (Stop) return;                   //假如当前停止检定，则退出
            //获取所有表的表地址
            string[] address = MeterProtocolAdapter.Instance.ReadAddress();

            //清空所有表需量
            bool[] clearResult = MeterProtocolAdapter.Instance.ClearDemand();

            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo MeterFirstInfo = null;
            int int_Index = GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh();
            if (int_Index == -1) return;
            MeterFirstInfo = GlobalUnit.g_CUS.DnbData.GetMeterBasicInfoByBwh(int_Index + 1);

            if (Stop) return;                   //假如当前停止检定，则退出
            bool[] bResult = new bool[BwCount];

            MessageController.Instance.AddMessage("读取每月第1结算日");
            #region
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000B01", 4);
            strCurItem = ItemKey + "01";

            strFreezeTime = strReadData[GlobalUnit.FirstYaoJianMeter];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;

                curMeter = Helper.MeterDataHelper.Instance.Meter(i);

                bResult[i] = true;

                if (strFreezeTime != strReadData[i])
                {
                    MessageController.Instance.AddMessage("有表位每月第1结算日不一致，试验终止");
                    //Stop = true;
                    //break;
                }
            }
            if (Stop) return;
            #endregion



            MessageController.Instance.AddMessage("读取组合无功特征字1");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000602", 2);
            strCurItem = ItemKey + "02";

            strWgZdz1 = WuGongMeterStatus(strReadData, strCurItem);//无功状态字1


            strReadData = MeterProtocolAdapter.Instance.ReadData("04000603", 2);
            strCurItem = ItemKey + "03";
            strWgZdz2 = WuGongMeterStatus(strReadData, strCurItem);//无功状态字2


            PowerFangXiang = Cus_PowerFangXiang.正向有功;
            if (!Walk(Cus_PowerFangXiang.正向有功))
            {
                return;
            }


            PowerFangXiang = Cus_PowerFangXiang.反向有功;
            if (!Walk(Cus_PowerFangXiang.反向有功))
                return;
            PowerFangXiang = Cus_PowerFangXiang.正向有功;

            bool[] result;
            bool bReturnResult;
            string strMessageText = "";
            if (!Stop)
            {
                strCurItem = ItemKey + "04";
                dicDemandZx = ReadDemand(strCurItem, XuliangFangxiang.正向有功最大需量, 0);

                strCurItem = ItemKey + "06";
                dicDemandFx = ReadDemand(strCurItem, XuliangFangxiang.反向有功最大需量, 0);

                strCurItem = ItemKey + "08";
                dicDemandXiangxian1 = ReadDemand(strCurItem, XuliangFangxiang.第一象限无功最大需量, 0);

                strCurItem = ItemKey + "10";
                dicDemandXiangxian2 = ReadDemand(strCurItem, XuliangFangxiang.第二象限无功最大需量, 0);

                strCurItem = ItemKey + "12";
                dicDemandXiangxian3 = ReadDemand(strCurItem, XuliangFangxiang.第三象限无功最大需量, 0);

                strCurItem = ItemKey + "14";
                dicDemandXiangxian4 = ReadDemand(strCurItem, XuliangFangxiang.第四象限无功最大需量, 0);

                strCurItem = ItemKey + "16";
                dicDemandZhwg1 = ReadDemand(strCurItem, XuliangFangxiang.组合无功1最大需量, 0);

                strCurItem = ItemKey + "18";
                dicDemandZhwg2 = ReadDemand(strCurItem, XuliangFangxiang.组合无功2最大需量, 0);

                #region 将电表时间修改结算日前1分钟

                strFreezeTime = DateTime.Now.Year.ToString("D2") + DateTime.Now.Month.ToString("D2") + strFreezeTime + "0000";
                dtFreezeTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(strFreezeTime);

                dtFreezeTime = dtFreezeTime.AddSeconds(-60);

                MessageController.Instance.AddMessage("将电表时间修改到结算日前1分钟");




                string str_MeterTime = dtFreezeTime.ToString("yyMMddHHmmss");
                string[] strID = new string[BwCount];
                string[] strData = new string[BwCount];
                string[] strSetData = new string[BwCount];
                int[] iFlag = new int[BwCount];
                string[] strShowData = new string[BwCount];
                string[] strCode = new string[BwCount];
                string[] strRand1 = new string[BwCount];//随机数
                string[] strRand2 = new string[BwCount];//随机
                string[] strEsamNo = new string[BwCount];//Esam序列号
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = str_MeterTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += str_MeterTime.Substring(6, 6);
                    strShowData[i] = str_MeterTime;
                    strData[i] = strCode[i] + strSetData[i];
                }
              result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                bReturnResult = true;
                strMessageText = "";
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
                    //return;
                }

                _MaxStartTime = 90;
                m_StartTime = DateTime.Now;
                while (true)
                {
                    //每一秒刷新一次数据
                    long _PastTime = base.VerifyPassTime;
                    System.Threading.Thread.Sleep(1000);

                    float pastMinute = _PastTime / 60F;
                    GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = string.Format("运行过每月第1结算日需要", PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                    MessageController.Instance.AddMessage(strDes);

                    if ((_PastTime >= _MaxStartTime) || Stop)
                    {
                        GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }
                }

                #endregion


                strCurItem = ItemKey + "05";
                dicDemandSyZx = ReadDemand(strCurItem, XuliangFangxiang.正向有功最大需量, 1);

                strCurItem = ItemKey + "07";
                dicDemandSyFx = ReadDemand(strCurItem, XuliangFangxiang.反向有功最大需量, 1);

                strCurItem = ItemKey + "09";
                dicDemandSyXiangxian1 = ReadDemand(strCurItem, XuliangFangxiang.第一象限无功最大需量, 1);

                strCurItem = ItemKey + "11";
                dicDemandSyXiangxian2 = ReadDemand(strCurItem, XuliangFangxiang.第二象限无功最大需量, 1);

                strCurItem = ItemKey + "13";
                dicDemandSyXiangxian3 = ReadDemand(strCurItem, XuliangFangxiang.第三象限无功最大需量, 1);

                strCurItem = ItemKey + "15";
                dicDemandSyXiangxian4 = ReadDemand(strCurItem, XuliangFangxiang.第四象限无功最大需量, 1);

                strCurItem = ItemKey + "17";
                dicDemandSyZhwg1 = ReadDemand(strCurItem, XuliangFangxiang.组合无功1最大需量, 1);

                strCurItem = ItemKey + "19";
                dicDemandSyZhwg2 = ReadDemand(strCurItem, XuliangFangxiang.组合无功2最大需量, 1);
            }

            #region 恢复表时间
            MessageController.Instance.AddMessage("恢复电表时间");
            #region

            string str_MeterTime1 = DateTime.Now.ToString("yyMMddHHmmss");
            string[] strID1 = new string[BwCount];
            string[] strData1 = new string[BwCount];
            string[] strSetData1 = new string[BwCount];
            int[] iFlag1 = new int[BwCount];
            string[] strShowData1 = new string[BwCount];
            string[] strCode1 = new string[BwCount];
            string[] strRand11 = new string[BwCount];//随机数
            string[] strRand21 = new string[BwCount];//随机
            string[] strEsamNo1 = new string[BwCount];//Esam序列号
            iFlag1 = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand11, out strRand21, out strEsamNo1);
            for (int i = 0; i < BwCount; i++)
            {
                strCode1[i] = "0400010C";
                strSetData1[i] = str_MeterTime1.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData1[i] += str_MeterTime1.Substring(6, 6);
                strShowData1[i] = str_MeterTime1;
                strData1[i] = strCode1[i] + strSetData1[i];
            }
       result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag1, strRand21, strData1, strCode1);



        //    result = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            bReturnResult = true;

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
                //return;
            }

            #endregion
            #endregion
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter.Mb_chrAddr = address[i];


                //dicDemandZh2[i] = new float[5];
                //switch (strRun[i])
                //{
                //    case "正向有功加反向有功":
                //        dicDemandZh2[i][0] = dicDemandZx[i][0] + dicDemandFx[i][0];
                //        dicDemandZh2[i][1] = dicDemandZx[i][1] + dicDemandFx[i][1];
                //        dicDemandZh2[i][2] = dicDemandZx[i][2] + dicDemandFx[i][2];
                //        dicDemandZh2[i][3] = dicDemandZx[i][3] + dicDemandFx[i][3];
                //        dicDemandZh2[i][4] = dicDemandZx[i][4] + dicDemandFx[i][4];
                //        break;
                //    case "正向有功":
                //        dicDemandZh2[i][0] = dicDemandZx[i][0];
                //        dicDemandZh2[i][1] = dicDemandZx[i][1];
                //        dicDemandZh2[i][2] = dicDemandZx[i][2];
                //        dicDemandZh2[i][3] = dicDemandZx[i][3];
                //        dicDemandZh2[i][4] = dicDemandZx[i][4];
                //        break;
                //    case "反向有功":
                //        dicDemandZh2[i][0] = dicDemandFx[i][0];
                //        dicDemandZh2[i][1] = dicDemandFx[i][1];
                //        dicDemandZh2[i][2] = dicDemandFx[i][2];
                //        dicDemandZh2[i][3] = dicDemandFx[i][3];
                //        dicDemandZh2[i][4] = dicDemandFx[i][4];
                //        break;
                //    case "正向有功减反向有功":
                //        dicDemandZh2[i][0] = dicDemandZx[i][0] - dicDemandFx[i][0];
                //        dicDemandZh2[i][1] = dicDemandZx[i][1] - dicDemandFx[i][1];
                //        dicDemandZh2[i][2] = dicDemandZx[i][2] - dicDemandFx[i][2];
                //        dicDemandZh2[i][3] = dicDemandZx[i][3] - dicDemandFx[i][3];
                //        dicDemandZh2[i][4] = dicDemandZx[i][4] - dicDemandFx[i][4];
                //        break;
                //}
                ResultDictionary["正向有功需量"][i] = dicDemandZx[i].ToString();
                ResultDictionary["反向有功需量"][i] = dicDemandFx[i].ToString();
                ResultDictionary["第一象限需量"][i] = dicDemandXiangxian1[i].ToString();
                ResultDictionary["第二象限需量"][i] = dicDemandXiangxian2[i].ToString();
                ResultDictionary["第三象限需量"][i] = dicDemandXiangxian3[i].ToString();
                ResultDictionary["第四象限需量"][i] = dicDemandXiangxian4[i].ToString();
                ResultDictionary["组合无功1"][i] = dicDemandZhwg1[i].ToString();
                ResultDictionary["组合无功2"][i] = dicDemandZhwg2[i].ToString();
                ResultDictionary["正向有功需量上1月"][i] = dicDemandSyZx[i].ToString();
                ResultDictionary["反向有功需量上1月"][i] = dicDemandSyFx[i].ToString();
                ResultDictionary["第一象限需量上1月"][i] = dicDemandSyXiangxian1[i].ToString();
                ResultDictionary["第二象限需量上1月"][i] = dicDemandSyXiangxian2[i].ToString();
                ResultDictionary["第三象限需量上1月"][i] = dicDemandSyXiangxian3[i].ToString();
                ResultDictionary["第四象限需量上1月"][i] = dicDemandSyXiangxian4[i].ToString();
                ResultDictionary["组合无功1上1月"][i] = dicDemandSyZhwg1[i].ToString();
                ResultDictionary["组合无功2上1月"][i] = dicDemandSyZhwg2[i].ToString();

                if (!CompareEnergy(dicDemandZx[i], dicDemandSyZx[i]) ||
                    !CompareEnergy(dicDemandFx[i], dicDemandSyFx[i]) ||
                    !CompareEnergy(dicDemandXiangxian1[i], dicDemandSyXiangxian1[i]) ||
                    !CompareEnergy(dicDemandXiangxian2[i], dicDemandSyXiangxian2[i]) ||
                    !CompareEnergy(dicDemandXiangxian3[i], dicDemandSyXiangxian3[i]) ||
                    !CompareEnergy(dicDemandXiangxian4[i], dicDemandSyXiangxian4[i]) ||
                    !CompareEnergy(dicDemandZhwg1[i], dicDemandSyZhwg1[i]) ||
                    !CompareEnergy(dicDemandZhwg2[i], dicDemandSyZhwg2[i])
                    )
                {
                    bResult[i] = false;
                }

                string strResult = bResult[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;

                ResultDictionary["结论"][i] = strResult;
            }

            UploadTestResult( "正向有功需量");
            UploadTestResult( "反向有功需量");
            UploadTestResult( "第一象限需量");
            UploadTestResult( "第二象限需量");
            UploadTestResult( "第三象限需量");
            UploadTestResult( "第四象限需量");
            UploadTestResult( "组合无功1");
            UploadTestResult( "组合无功2");
            UploadTestResult( "正向有功需量上1月");
            UploadTestResult( "反向有功需量上1月");
            UploadTestResult( "第一象限需量上1月");
            UploadTestResult( "第二象限需量上1月");
            UploadTestResult( "第三象限需量上1月");
            UploadTestResult( "第四象限需量上1月");
            UploadTestResult( "组合无功1上1月");
            UploadTestResult( "组合无功2上1月");
            UploadTestResult( "结论");

        }
        
        private bool CompareEnergy(float[] f1, float[] f2)
        {
            if (f1 == null)
                return false;
            if (f1.Length == 0)
                return false;
            if (f1.Length != f2.Length)
                return false;
            for (int i = 0; i < f1.Length; i++)
                if (f1[i] != f2[i])
                    return false;
            return true;
        }
        private bool Walk(Cus_PowerFangXiang fangxiang)
        {
            if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)fangxiang, FangXiangStr + "0.5L", true, false) == false)
            {
                MessageController.Instance.AddMessage("控制源输出失败");
                return false;
            }
            System.Threading.Thread.Sleep(300);
            int _MaxStartTime = 16*60;
            m_StartTime = DateTime.Now;
            while (true)
            {
                //每一秒刷新一次数据
                long _PastTime = base.VerifyPassTime;
                System.Threading.Thread.Sleep(1000);

                float pastMinute = _PastTime / 60F;
                GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                string strDes = string.Format("{0}运行时间",PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                MessageController.Instance.AddMessage(strDes);

                if ((_PastTime >= _MaxStartTime) || Stop)
                {
                    GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                    break;
                }
            }

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            System.Threading.Thread.Sleep(300);
            return true;
        }
        //zxr 解析无功组合方式特征字
        private string[] WuGongMeterStatus(string[] strReadData, string strCurItem)
        {
            string[] strRun = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;
                //解析无功组合方式状态字
                if (strReadData[i].Length == 2)
                {
                    strRun[i] = string.Empty;
                    string upZj = strReadData[i].Substring(0, 1);
                    string downZj = strReadData[i].Substring(1, 1);
                    switch (downZj)
                    {
                        case "1":
                            strRun[i] = "第一象限加";
                            break;
                        case "2":
                            strRun[i] = "第一象限减";
                            break;
                        case "3"://见鬼了
                            break;
                        case "4":
                            strRun[i] = "第二象限加";
                            break;
                        case "5":
                            strRun[i] = "第一象限加第二象限";
                            break;
                        case "6"://见鬼了
                            break;
                        case "7"://见鬼了
                            break;
                        case "8":
                            strRun[i] = "第二象限减";
                            break;
                        default:
                            break;
                    }

                    switch (upZj)
                    {
                        case "1":
                            if (strRun[i] == string.Empty)
                            {
                                strRun[i] += "第三象限加";
                            }
                            else
                            {
                                strRun[i] += "第三象限";
                            }
                            break;
                        case "2":
                            if (strRun[i] == string.Empty)
                            {
                                strRun[i] += "第三象限减";
                            }
                            else
                            {
                                strRun[i] += "第三象限";
                            }
                            break;
                        case "3"://见鬼了
                            break;
                        case "4":
                            strRun[i] += "第四象限";
                            break;
                        case "5":
                            strRun[i] += "第三象限加第四象限";
                            break;
                        case "6"://见鬼了
                            break;
                        case "7"://见鬼了
                            break;
                        case "8":
                            strRun[i] += "第四象限";
                            break;
                        default:
                            break;
                    }
                }

            }
            return strRun;
        }

        private string[] YouGongMeterStatus(string[] strReadData, string strCurItem)
        {
            string[] strRun = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                if (strReadData[i] == null || strReadData[i] == "")
                    continue;
                byte byt_Run;
                string str_Tmp;
                byt_Run = Convert.ToByte(strReadData[i].Substring(0, 2), 16);
                str_Tmp = Convert.ToString(byt_Run, 2);
                if (str_Tmp.Length < 8)
                    str_Tmp = "0000" + str_Tmp;

                if (str_Tmp.Substring(str_Tmp.Length - 1, 1) == "1")
                {
                    strRun[i] = "正向有功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 3, 1) == "1")
                {
                    if (strRun[i].Length == 0)
                        strRun[i] = "反向有功";
                    else
                        strRun[i] += "加反向有功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 2, 1) == "1")
                {
                    strRun[i] += "减正向有功";
                }

                if (str_Tmp.Substring(str_Tmp.Length - 4, 1) == "1")
                {
                    strRun[i] += "减反向有功";
                }
            }
            return strRun;
        }

        private Dictionary<int, float[]> ReadDemand(string strCurItem, XuliangFangxiang fangxiang, int int_FreezeTimes)
        {           

            Dictionary<int, float[]> dicDemand = new Dictionary<int, float[]>();

            string strMessage;
            string strEnergyType;
            //switch (fangxiang)
            //{
            //    case XuliangFangxiang.正向有功最大需量:
            //        strEnergyType = "组合有功";
            //        break;
            //    case XuliangFangxiang.反向有功最大需量:
            //        strEnergyType = "正向有功";
            //        break;
            //    case Cus_PowerFangXiang.正向无功:
            //        strEnergyType = "正向无功";
            //        break;
            //    case Cus_PowerFangXiang.反向有功:
            //        strEnergyType = "反向有功";
            //        break;
            //    case Cus_PowerFangXiang.反向无功:
            //        strEnergyType = "反向无功";
            //        break;
            //    case Cus_PowerFangXiang.第一象限无功:
            //        strEnergyType = "第一象限无功";
            //        break;
            //    case Cus_PowerFangXiang.第二象限无功:
            //        strEnergyType = "第二象限无功";
            //        break;
            //    case Cus_PowerFangXiang.第三象限无功:
            //        strEnergyType = "第三象限无功";
            //        break;
            //    case Cus_PowerFangXiang.第四象限无功:
            //        strEnergyType = "第四象限无功";
            //        break;
            //    default:
            //        strEnergyType = "正向有功";
            //        break;
            //}
            strEnergyType = fangxiang.ToString();
            if(int_FreezeTimes ==0)
                strMessage = string.Format("读取【当前{0}】需量", strEnergyType);
            else
                strMessage = string.Format("读取【上{0}结算日{1}】需量",int_FreezeTimes, strEnergyType);
            MessageController.Instance.AddMessage(strMessage);
            dicDemand = MeterProtocolAdapter.Instance.ReadDemands((byte)((int)fangxiang), int_FreezeTimes);
            
            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                if (dicDemand.ContainsKey(j) == false) continue;
                //总结论
                if (!curMeter.MeterFunctions.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                    curMeter.MeterFunctions.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterFunctions[strCurItem];
                }
            }
            return dicDemand;
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
