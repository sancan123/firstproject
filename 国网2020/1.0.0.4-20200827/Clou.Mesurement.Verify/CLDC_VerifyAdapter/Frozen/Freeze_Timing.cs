
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 定时冻结检测
    /// </summary>
    public class Freeze_Timing : FreezeBase
    {
        public Freeze_Timing(object plan) : base(plan) { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "月冻结前上一次冻结总电量", "月冻结前电量", "月冻结后电量", "月冻结后上一次冻结总电量",
                                         "日冻结前上一次冻结总电量", "日冻结前电量", "日冻结后电量", "日冻结后上一次冻结总电量", 
                                         "小时冻结前上一次冻结总电量", "小时冻结前电量", "小时冻结后电量", "小时冻结后上一次冻结总电量",
                                         "结论" };
            return true;
        }

        

        /// <summary>
        /// 定时冻结检定
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            #region  --------------局部变量---------------
            string[][] lstFreezePW = new string[][] { new string[BwCount], new string[BwCount] }; //存储两次冻结模式字
            bool[] bReturn = new bool[BwCount];
            bool bResult;
            string str_AllResult = Variable.CTG_HeGe;
            //string allResult="";
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = null;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze curResult;
            #endregion

            bool[][] Result = new bool[][] {new bool[BwCount],new bool[BwCount],new bool[BwCount] };
            string str_MeterTime = DateTime.Now.ToString("yyMMddHHmmss");
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
            base.Verify();
            if (Stop) return;
            if (!PowerOn(true))
            {
                MessageController.Instance.AddMessage("源输出失败", 6, 1);
                return;
            }
            string[] arrStrResultKey = new string[BwCount];
            try
            {
                //读冻结模式字
                MessageController.Instance.AddMessage("等待电表启动......");
                //Thread.Sleep(5000);  //延时源稳定5S
                MessageController.Instance.AddMessage("开始读取冻结模式字");

                MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

                for (int i = 0; i < 3; i++)
                {
                    //0月冻结，1日冻结，2小时冻结
                    if (Stop) return;
                    FreezeDeal(i.ToString(),out Result[i]);
                }

                if (Stop) return;
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);


                //恢复表时间为当前时间
                MessageController.Instance.AddMessage("恢复电表时间为当前时间");


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
                bReturn = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                //bReturn = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                //bResult = GetArrValue(bReturn);
                //if (!bResult)
                //{
                //    MessageController.Instance.AddMessage("写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
                //    Stop = true;
                //    return;
                //}

                //结论
                for (int j = 0; j < BwCount; j++)
                {
                    //强制停止
                    if (Stop) return;
                    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                    {
                        continue;
                    }
                    if (Result[0][j] && Result[1][j] && Result[2][j])
                    {
                        ResultDictionary["结论"][j] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["结论"][j] = Variable.CTG_BuHeGe;
                    }
                }
                UploadTestResult("结论");

               MessageController.Instance.AddMessage("定时冻结检定完毕");
                
            }
            catch (System.Exception e)
            {
               MessageController.Instance.AddMessage(e.Message, 6, 2);
                Thread.Sleep(100);
            }
        }
        
        /// <summary>
        /// 冻结处理
        /// </summary>
        /// <param name="str_Type">冻结类别，0-月冻结 1-日冻结 2-小时冻结</param>
        /// <returns></returns>
        private void FreezeDeal(string str_Type,out bool[] bResults)
        {
            bool[] Result = new bool[BwCount];
            bResults = Result;
            string[] strName = new string[] {"月","日","小时" };

            #region -------------------初始化变量-------------------
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = null;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze curResult;
            DateTime dtm_meterTime = DateTime.Now;
            string str_MeterTime = string.Empty;
            string str_DateTime = string.Empty;
            string str_Msg = string.Empty;
            string str_Result = string.Empty;
            string str_Item = string.Empty;
            string str_TempValues = string.Empty;
            bool bln_FlagDL = false;                    //冻结电量是否与当前电量相等标志
            //维数 0=冻结前上一次冻结电量，1=冻结前当前表电量，2=冻结后当前表电量，3=冻结后上一次冻结电量
            float[][] flt_DL = new float[][] { new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount] };
            bool[] bReturn = new bool[BwCount];
            bool bResult;
            #endregion

            #region ------------------初始化冻结参数----------------------
            switch (str_Type)
            {
                case "0":
                    str_DateTime = "99010000";
                    str_Msg = "月冻结";
                    str_Item = "01";
                    break;
                case "1":
                    str_DateTime = "99990000";
                    str_Msg = "日冻结";
                    str_Item = "02";
                    break;
                case "2":
                    str_DateTime = "99999930";
                    str_Msg = "小时冻结";
                    str_Item = "03";
                    break;
            }
            #endregion

            #region ----------------读取冻结前数据------------------
           //MessageController.Instance.AddMessage(str_Msg + "前读取上一次冻结总电量");
           // ReadDL(true, 1, ref flt_DL[0]);
           //MessageController.Instance.AddMessage(str_Msg + "前读取当前电表总电量");
           // ReadDL(false, 1, ref flt_DL[1]);
            if (Stop) return;
            MessageController.Instance.AddMessage(strName[int.Parse(str_Type)]+ "冻结前读取上一次冻结总电量");
            flt_DL[0] = MeterProtocolAdapter.Instance.ReadData("05000101", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int.Parse(str_Type)]+"冻结前上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
            if (Stop) return;
            MessageController.Instance.AddMessage(strName[int.Parse(str_Type)]+"冻结前读取当前电表总电量");
            flt_DL[1] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int.Parse(str_Type)]+"冻结前电量", ConvertArray.ConvertFloat2Str(flt_DL[1]));
            //判断冻结电量是否与当前电量相等
            //for (int j = 0; j < flt_DL[0].Length; j++)
            //{
            //    if (flt_DL[0][j] == flt_DL[1][j])// && flt_DL[0][j] != 0
            //    {
            //        bln_FlagDL = true;
            //        break;
            //    }
            //}
            ////冻结电量与当前电量相等，则进行走字
            //if (bln_FlagDL)
            {
               MessageController.Instance.AddMessage("最大电流进行走字20S，请稍候......");
                //升源
               if (Stop) return;
                if (!PowerOn())
                {
                    throw new Exception("升源失败");
                }
                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
                if (Stop) return;
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
               MessageController.Instance.AddMessage("正在设置冻结测试参数");
               if (Stop) return;
               if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功))
                {
                    Check.Require(false, "控制源输出失败");
                }
                //读冻结模式字
                MessageController.Instance.AddMessage("等待电表启动......");
                Thread.Sleep(5000);  //延时源稳定5S
            }
            #endregion

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            #region ----------------执行冻结操作----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("开始进行" + str_Msg);
            bReturn = MeterProtocolAdapter.Instance.FreezeCmd(str_DateTime);
            //bResult = GetArrValue(bReturn);
            //if (!bResult)
            //{
            //   MessageController.Instance.AddMessage(str_Msg + "失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
            //    return;
            //}
            //if (!Adapter.Adpater485.ReadDateTime())
            //{
            //   MessageController.Instance.AddMessage("读取表时间失败");
            //    return;
            //}
            for (int iNum = 0; iNum < BwCount; iNum++)
            {
                //强制停止
                if (Stop) return;

                if (!Helper.MeterDataHelper.Instance.Meter(iNum).YaoJianYn)
                {
                    continue;
                }
               
                //switch (str_Type)
                //{
                //    case "0":
                //        //设置为下月初0时前1秒钟 yyMMddHHmmss
                //        str_MeterTime = new DateTime(dtm_meterTime.Year, dtm_meterTime.Month, 1).AddMonths(1).AddSeconds(-1).ToString("yyMMddHHmmss");
                //        break;
                //    case "1":
                //        //设置为下一日0时前1秒钟 yyMMddHHmmss
                //        str_MeterTime = string.Format("{0}{1}{2}235959", dtm_meterTime.ToString().Substring(2, 2), dtm_meterTime.Month.ToString("D2"), dtm_meterTime.Day.ToString("D2"));
                //        break;
                //    case "2":
                //        //设置为当前小时的半点前1秒钟 yyMMddHHmmss
                //        str_MeterTime = string.Format("{0}{1}{2}{3}2959", dtm_meterTime.Year.ToString().Substring(2, 2), dtm_meterTime.Month.ToString("D2"),
                //                                dtm_meterTime.Day.ToString("D2"), dtm_meterTime.AddMinutes(30).Hour.ToString("D2"));
                //        break;
                //}
                //if (!Adapter.Adpater485.WriteDateTime(str_MeterTime, false))
                //{
                //   MessageController.Instance.AddMessage("写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
                //    //return;
                //}
            }


            switch (str_Type)
            {
                case "0":
                    //设置为下月初0时前1秒钟 yyMMddHHmmss
                    str_MeterTime = new DateTime(dtm_meterTime.Year, dtm_meterTime.Month, 1).AddMonths(1).AddSeconds(-5).ToString("yyMMddHHmmss");
                    break;
                case "1":
                    //设置为下一日0时前1秒钟 yyMMddHHmmss
                    str_MeterTime = string.Format("{0}{1}{2}235955", dtm_meterTime.ToString().Substring(2, 2), dtm_meterTime.Month.ToString("D2"), dtm_meterTime.Day.ToString("D2"));
                    break;
                case "2":
                    //设置为当前小时的半点前1秒钟 yyMMddHHmmss
                    str_MeterTime = string.Format("{0}{1}{2}{3}2955", dtm_meterTime.Year.ToString().Substring(2, 2), dtm_meterTime.Month.ToString("D2"),
                                            dtm_meterTime.Day.ToString("D2"), dtm_meterTime.AddMinutes(30).Hour.ToString("D2"));
                    break;
            }


         //   string str_MeterTime = DateTime.Now.ToString("yyMMddHHmmss");
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



           MessageController.Instance.AddMessage("延时15S,请稍候......");
            Thread.Sleep(15000);
            #endregion

            #region -------------------读取冻结后的数据-----------------
           //MessageController.Instance.AddMessage(str_Msg + "后读取当前电表总电量");
           // ReadDL(false, 1, ref flt_DL[2]);
           //MessageController.Instance.AddMessage(str_Msg + "后读取上一次冻结总电量");
           // ReadDL(true, 1, ref flt_DL[3]);
            if (Stop) return;
            MessageController.Instance.AddMessage(strName[int.Parse(str_Type)]+"冻结后读取当前电表总电量");
            flt_DL[2] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int.Parse(str_Type)]+"冻结后电量", ConvertArray.ConvertFloat2Str(flt_DL[2]));
            if (Stop) return;
            MessageController.Instance.AddMessage(strName[int.Parse(str_Type)]+"冻结后读取上一次冻结总电量");
            flt_DL[3] = MeterProtocolAdapter.Instance.ReadData("05000101", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int.Parse(str_Type)]+"冻结后上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[3]));
            #endregion
          
           

            #region --------------------处理结论------------------
            for (int j = 0; j < BwCount; j++)
            {
                //强制停止
                if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                {
                    continue;
                }

                if (flt_DL[0][j] < flt_DL[3][j] && flt_DL[1][j] < flt_DL[2][j] && flt_DL[2][j] == flt_DL[3][j])
                {
                    //ResultDictionary["结论"][j] = Variable.CTG_HeGe;
                    Result[j] = true;  
                }
                else
                {
                    //ResultDictionary["结论"][j] = Variable.CTG_BuHeGe;
                    Result[j] = false; 
                }
            }
            bResults = Result;
            //UploadTestResult("结论");
            #endregion
          


        }

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="p_bln_IsSpecial">true=冻结电量，false=实际电量</param>
        /// <param name="p_flt_AllBWDL">存储所有表位电量</param>
        /// <returns></returns>
        private void ReadDL(bool p_bln_IsSpecial,int int_Times,ref float[] p_flt_AllBWDL)
        {
            float[] flt_TempDL = new float[1] { -1F };
            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            if (p_bln_IsSpecial)
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadSpecialEnergy(3, int_Times);
            }
            else
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergy(0x01);
            }
            for (int j = 0; j < BwCount; j++)
            {
                //强制停止
                if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn || !dicEnergy.ContainsKey(j))
                {
                    continue;
                }
                flt_TempDL = dicEnergy[j];
                if (flt_TempDL[0] < 0F)
                {
                    MessageController.Instance.AddMessage(string.Format("表位[0]返回的数据不符合要求", j + 1));
                    continue;
                }
                else
                {
                    p_flt_AllBWDL[j] = flt_TempDL[0];
                }
            }
        }

        /// <summary>
        /// 存储冻结模式字
        /// </summary>
        /// <param name="p_str_PW"></param>
        /// <returns></returns>
        private void FillPatternWord(ref string[] p_str_PW)
        {
            string[] str_TempPW = new string[1] { "" };
            string str_TempValue=string.Empty;
            string[] strReturnData = new string[BwCount];
            strReturnData = MeterProtocolAdapter.Instance.ReadPatternWord(1);
            
            for (int k = 0; k < BwCount; k++)
            {
                //强制停止
                if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    continue;
                }
                str_TempValue = strReturnData[k];
                if (str_TempValue == "")
                {
                    MessageController.Instance.AddMessage(string.Format("表位[0]读取冻结模式字失败", k + 1));
                    return;
                }
                p_str_PW[k] = str_TempValue;
            }
        }

        /// <summary>
        /// 清理数据节点
        /// </summary>
        protected override void ClearItemData()
        {
            string strKey = ItemKey;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = null;
            string strCurKey = string.Empty;
            for (int i = 1; i < 4; i++)
            {
                for (int k = 0; k < BwCount; k++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                    if (!curMeter.YaoJianYn) continue;
                    strCurKey = string.Format("0010{0}", i);
                    if (curMeter.MeterFreezes.ContainsKey(strCurKey))
                    {
                        curMeter.MeterFreezes.Remove(strCurKey);
                    }
                    //添加新节点
                }
            }
            base.ClearItemData();
        }
    }
}