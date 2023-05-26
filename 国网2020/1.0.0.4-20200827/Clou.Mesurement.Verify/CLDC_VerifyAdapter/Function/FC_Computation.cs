
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
    /// 计量功能
    /// </summary>
    class FC_Computation : FunctionBase
    {
        public FC_Computation(object plan)
            : base(plan) 
        {
            
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "每月第一结算日", "组合有功特征字", "正向有功电量", "反向有功电量", "组合有功电量", "上1结算日正向有功电量", "上1结算日反向有功电量", "上1结算日组合有功电量", "电表实际组合有功电量", "结论" };
            return true;
        }


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
            
            string[] strReadData = null;
            string[] strRun = new string[BwCount];

            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            string keyitem = ((int)CLDC_Comm.Enum.Cus_FunctionItem.计量功能).ToString().PadLeft(3, '0');
            Dictionary<int, float[]> dicEnergyZx = new Dictionary<int, float[]>();  //正向有功电量
            Dictionary<int, float[]> dicEnergyFx = new Dictionary<int, float[]>();  //反向有功电量
            Dictionary<int, float[]> dicEnergyZh = new Dictionary<int, float[]>();  //组合有功电量
            Dictionary<int, float[]> dicEnergyZx1 = new Dictionary<int, float[]>(); //上1结算日正向有功电量
            Dictionary<int, float[]> dicEnergyFx1 = new Dictionary<int, float[]>(); //上1结算日反向有功电量
            Dictionary<int, float[]> dicEnergyZh1 = new Dictionary<int, float[]>(); //上1结算日组合有功电量
            Dictionary<int, float[]> dicEnergyZh2 = new Dictionary<int, float[]>(); //电表实际组合有功电量

            
            //初始化设备
            if (Stop) return;  
            if (!InitEquipment())
            {
                return;
            }
                        if (Stop) return;                   //假如当前停止检定，则退出
            bool[] bResult = new bool[BwCount];

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            string ItemKey = ((int)CLDC_Comm.Enum.Cus_FunctionItem.计量功能).ToString().PadLeft(3, '0');

            if (Stop) return;  
            MessageController.Instance.AddMessage("读取每月第1结算日");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000B01", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "每月第一结算日", strReadData);
            strCurItem = ItemKey + "01";

            strFreezeTime = strReadData[GlobalUnit.FirstYaoJianMeter];
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                                
                if (strReadData[i] == null || strReadData[i] == "")
                    continue;

                if (strFreezeTime != strReadData[i])
                {
                    MessageController.Instance.AddMessage("有表位每月第1结算日不一致，试验终止");
                    Stop = true;
                    break;
                }
            }

            if (Stop) return;  
            MessageController.Instance.AddMessage("读取组合有功特征字");
            strReadData = MeterProtocolAdapter.Instance.ReadData("04000601", 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "组合有功特征字", strReadData);
            strCurItem = ItemKey + "02";
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
            if (Stop) return;  
            PowerFangXiang = Cus_PowerFangXiang.正向有功;
            if (!Walk(Cus_PowerFangXiang.正向有功))
                return;

            if (Stop) return;  
            PowerFangXiang = Cus_PowerFangXiang.反向有功;
            if (!Walk(Cus_PowerFangXiang.反向有功))
                return;
            PowerFangXiang = Cus_PowerFangXiang.正向有功;

            #region 将电表时间修改结算日前1分钟
            if (Stop) return;  
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
            if (Stop) return;  
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = str_MeterTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += str_MeterTime.Substring(6, 6);
                strShowData[i] = str_MeterTime;
                strData[i] = strCode[i] + strSetData[i];
            }
            if (Stop) return;  
            bool[] result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
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

            _MaxStartTime = 60;
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
                    CLDC_DataCore.Const. GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                    break;
                }
            }

            #endregion

            if (Stop) return;  
            strCurItem = ItemKey + "03";
            dicEnergyZh = ReadEnergy(strCurItem, Cus_PowerFangXiang.组合有功, 0);

            if (Stop) return;  
            strCurItem = ItemKey + "05";
            dicEnergyZx = ReadEnergy(strCurItem, Cus_PowerFangXiang.正向有功, 0);

            if (Stop) return;  
            strCurItem = ItemKey + "07";
            dicEnergyFx = ReadEnergy(strCurItem, Cus_PowerFangXiang.反向有功, 0);

            if (Stop) return;  
            strCurItem = ItemKey + "04";
            dicEnergyZh1 = ReadEnergy(strCurItem, Cus_PowerFangXiang.组合有功, 1);

            if (Stop) return;  
            strCurItem = ItemKey + "06";
            dicEnergyZx1 = ReadEnergy(strCurItem, Cus_PowerFangXiang.正向有功, 1);

            if (Stop) return;  
            strCurItem = ItemKey + "08";
            dicEnergyFx1 = ReadEnergy(strCurItem, Cus_PowerFangXiang.反向有功, 1);

            if (Stop) return;  
            MessageController.Instance.AddMessage("恢复电表时间");
            str_MeterTime = DateTime.Now.ToString("yyMMddHHmmss");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = str_MeterTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += str_MeterTime.Substring(6, 6);
                strShowData[i] = str_MeterTime;
                strData[i] = strCode[i] + strSetData[i];
            }
            if (Stop) return;  
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

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


            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                
                dicEnergyZh2[i] = new float[5];
                switch (strRun[i])
                {
                    case "正向有功加反向有功":
                        dicEnergyZh2[i][0] = dicEnergyZx[i][0] + dicEnergyFx[i][0];
                        dicEnergyZh2[i][1] = dicEnergyZx[i][1] + dicEnergyFx[i][1];
                        dicEnergyZh2[i][2] = dicEnergyZx[i][2] + dicEnergyFx[i][2];
                        dicEnergyZh2[i][3] = dicEnergyZx[i][3] + dicEnergyFx[i][3];
                        dicEnergyZh2[i][4] = dicEnergyZx[i][4] + dicEnergyFx[i][4];
                        break;
                    case "正向有功":
                        dicEnergyZh2[i][0] = dicEnergyZx[i][0];
                        dicEnergyZh2[i][1] = dicEnergyZx[i][1];
                        dicEnergyZh2[i][2] = dicEnergyZx[i][2];
                        dicEnergyZh2[i][3] = dicEnergyZx[i][3];
                        dicEnergyZh2[i][4] = dicEnergyZx[i][4];
                        break;
                    case "反向有功":
                        dicEnergyZh2[i][0] = dicEnergyFx[i][0];
                        dicEnergyZh2[i][1] = dicEnergyFx[i][1];
                        dicEnergyZh2[i][2] = dicEnergyFx[i][2];
                        dicEnergyZh2[i][3] = dicEnergyFx[i][3];
                        dicEnergyZh2[i][4] = dicEnergyFx[i][4];
                        break;
                    case "正向有功减反向有功":
                        dicEnergyZh2[i][0] = dicEnergyZx[i][0] - dicEnergyFx[i][0];
                        dicEnergyZh2[i][1] = dicEnergyZx[i][1] - dicEnergyFx[i][1];
                        dicEnergyZh2[i][2] = dicEnergyZx[i][2] - dicEnergyFx[i][2];
                        dicEnergyZh2[i][3] = dicEnergyZx[i][3] - dicEnergyFx[i][3];
                        dicEnergyZh2[i][4] = dicEnergyZx[i][4] - dicEnergyFx[i][4];
                        break;
                }

                ResultDictionary["正向有功电量"][i] = dicEnergyZx[i][0].ToString();
                ResultDictionary["反向有功电量"][i] = dicEnergyFx[i][0].ToString();
                ResultDictionary["组合有功电量"][i] = dicEnergyZh[i][0].ToString();
                ResultDictionary["上1结算日正向有功电量"][i] = dicEnergyZx1[i][0].ToString();
                ResultDictionary["上1结算日反向有功电量"][i] = dicEnergyFx1[i][0].ToString();
                ResultDictionary["上1结算日组合有功电量"][i] = dicEnergyZh1[i][0].ToString();
                ResultDictionary["电表实际组合有功电量"][i] = dicEnergyZh2[i][0].ToString();

                if (!CompareEnergy(dicEnergyZx[i], dicEnergyZx1[i]) || !CompareEnergy(dicEnergyFx[i], dicEnergyFx1[i]) ||
                    !CompareEnergy(dicEnergyZh[i], dicEnergyZh1[i]) || !CompareEnergy(dicEnergyZh[i], dicEnergyZh2[i]))
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
            }

            UploadTestResult("正向有功电量");
            UploadTestResult("反向有功电量");
            UploadTestResult("组合有功电量");
            UploadTestResult("上1结算日正向有功电量");
            UploadTestResult("上1结算日反向有功电量");
            UploadTestResult( "上1结算日组合有功电量");
            UploadTestResult( "电表实际组合有功电量");
            UploadTestResult("结论");
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
           {
               //if (f1[i] != f2[i])
               //    return false;
               if (Math.Abs(f1[i] - f2[i]) > 0.02)
                   return false;
           }

            return true;
        }
        private bool Walk(Cus_PowerFangXiang fangxiang)
        { 
            if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)fangxiang, FangXiangStr + "1.0", true, false) == false)
            {
                MessageController.Instance.AddMessage("控制源输出失败");
                return false;
            }
            System.Threading.Thread.Sleep(300);
            int _MaxStartTime = 30;
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
            if(int_FreezeTimes ==0)
                strMessage = string.Format("读取【当前{0}】电量", strEnergyType);
            else
                strMessage = string.Format("读取【上{0}结算日{1}】电量",int_FreezeTimes, strEnergyType);
            MessageController.Instance.AddMessage(strMessage);
            dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((byte)((int)fangxiang), int_FreezeTimes);
            
            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                if (dicEnergy.ContainsKey(j) == false) continue;
       
            }
            return dicEnergy;
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
