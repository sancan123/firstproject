
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 整点冻结
    /// </summary>
    public class Freeze_Whole_Point : FreezeBase
    {
        public Freeze_Whole_Point(object plan) : base(plan) { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "整点冻结前上一次冻结总电量", "整点冻结前电量", "整点冻结后电量", "整点冻结后上一次冻结总电量",
                                         "结论" };
            return true;
        }

        /// <summary>
        /// 整点冻结检定
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            #region  --------------局部变量---------------
            string[][] lstFreezePW = new string[][] { new string[BwCount], new string[BwCount] };               //存储两次冻结模式字
            string[][] str_StartTime = new string[][] { new string[BwCount], new string[BwCount] };             //存储两次冻结开始时间
            string[][] str_Interval = new string[][] { new string[BwCount], new string[BwCount] };              //存储两次冻结时间间隔
            bool[] bReturn = new bool[BwCount];
            bool bResult = false;
            MeterBasicInfo curMeter = null;
            MeterFreeze curResult;
            string str_Time = string.Empty;
            string str_MeterTime = string.Empty;
            string str_TempValues = string.Empty;
            string str_Result = string.Empty;
            DateTime dtm_meterTime = DateTime.Now;
            //维数 0=冻结前上一次冻结电量，1=冻结前当前表电量，2=冻结后当前表电量，3=冻结后上一次冻结电量
            float[][] flt_DL = new float[][] { new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount] };
            bool bln_FlagDL = false;
            #endregion

            string[] arrStrResultKey = new string[BwCount];
            if (Stop) return;
            base.Verify();
            if (Stop) return;
            if (!PowerOn(true))
            {
                MessageController.Instance.AddMessage("源输出失败");
                return;
            }
            try
            {
                MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

                #region ------------------正式冻结前保存电表原有数据-------------------
                //读冻结模式字
                MessageController.Instance.AddMessage("等待电表启动......");
                Thread.Sleep(5000);  //延时源稳定5S
                MessageController.Instance.AddMessage("开始读取冻结模式字");
                if (Stop) return;
                FillPatternWord(ref lstFreezePW[0]);  //04 00 09 05 FF
                if (Stop) return;
                MessageController.Instance.AddMessage("开始读取整点冻结起始时间");
                FillFreezeTime(ref str_StartTime[0], 5);
                if (Stop) return;
                MessageController.Instance.AddMessage("开始读取整点冻结间隔时间");
                FillFreezeTime(ref str_Interval[0], 6);

                if (Stop) return;
                MessageController.Instance.AddMessage("开始设置整点冻结间隔时间");
                bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(6, "60");
                if (Stop) return;
                str_Time = DateTime.Now.AddHours(1).ToString("yyMMddhh") + "00";
                MessageController.Instance.AddMessage("开始设置整点冻结起始时间");
                bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(5, str_Time);
                if (Stop) return;
                MessageController.Instance.AddMessage("整点冻结前读取上一次冻结总电量");
                flt_DL[0] = MeterProtocolAdapter.Instance.ReadData("05040101", 4, 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "整点冻结前上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
                if (Stop) return;
                MessageController.Instance.AddMessage("整点冻结前读取当前电表总电量");
                flt_DL[1] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "整点冻结前电量", ConvertArray.ConvertFloat2Str(flt_DL[1]));
                //判断冻结电量是否与当前电量相等
                //for (int j = 0; j < flt_DL[0].Length; j++)
                //{
                //    if (flt_DL[0][j] == flt_DL[1][j])
                //    {
                //        bln_FlagDL = true;
                //        break;
                //    }
                //}
                //冻结电量与当前电量相等，则进行走字
                //if (!Stop && bln_FlagDL)
                {
                    MessageController.Instance.AddMessage("最大电流进行走字20S，请稍候......");
                    //升源
                    if (Stop) return;
                    if (!PowerOn())
                    {
                        MessageController.Instance.AddMessage("升源失败");
                        return;
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

                #region ------------------冻结处理------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("开始进行整点冻结");
                bReturn = MeterProtocolAdapter.Instance.FreezeCmd("99999900");


                //设置电表时间为下一整点前5秒
                str_MeterTime = string.Format("{0}{1}{2}{3}5955", dtm_meterTime.ToString().Substring(2, 2), dtm_meterTime.Month.ToString("D2"),
                    dtm_meterTime.Day.ToString("D2"), dtm_meterTime.Hour.ToString("D2"));
                MessageController.Instance.AddMessage("恢复电表时间为当前时间");

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
                bReturn = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("延时10S,请稍候......");
                Thread.Sleep(10000);
                #endregion
                #region -------------------读取冻结后的数据-----------------
                if (Stop) return;
                MessageController.Instance.AddMessage("整点冻结后读取上一次冻结总电量");
                flt_DL[2] = MeterProtocolAdapter.Instance.ReadData("05040101", 4, 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "整点冻结后上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[2]));
                if (Stop) return;
                MessageController.Instance.AddMessage("整点冻结后读取当前电表总电量");
                flt_DL[3] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "整点冻结后电量", ConvertArray.ConvertFloat2Str(flt_DL[3]));

                if (Stop) return;
                //恢复冻结模式字
                MessageController.Instance.AddMessage("开始恢复整点冻结模式字");
                bReturn = MeterProtocolAdapter.Instance.WritePatternWord(5, lstFreezePW[0][0]);
                if (Stop) return;
                //恢复起始冻结时间
                MessageController.Instance.AddMessage("开始恢复整点冻结起始时间");
                bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(5, str_StartTime[0][0]);
                if (Stop) return;
                //恢复冻结间隔
                MessageController.Instance.AddMessage("开始恢复整点冻结间隔时间");
                bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(6, str_Interval[0][0]);

                #endregion
                #region -----------------------结果处理--------------------

                //处理结论
                for (int j = 0; j < BwCount; j++)
                {
                    //强制停止
                    if (Stop) return;
                    arrStrResultKey[j] = ItemKey;

                    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                    {
                        continue;
                    }
                    if (flt_DL[0][j] < flt_DL[3][j] && flt_DL[1][j] < flt_DL[2][j] && flt_DL[2][j] == flt_DL[3][j])
                    {
                        ResultDictionary["结论"][j] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["结论"][j] = Variable.CTG_BuHeGe;
                    }
                    
                }
                UploadTestResult("结论");
                #endregion

                if (Stop) return;
                //恢复表时间为当前时间
                MessageController.Instance.AddMessage("恢复电表时间为当前时间");
                bReturn = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));

                MessageController.Instance.AddMessage("整点冻结检定完毕");
                
            }
            catch (System.Exception e)
            {
                MessageController.Instance.AddMessage(e.Message, 6, 2);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="p_bln_IsSpecial">true=冻结电量，false=实际电量</param>
        /// <param name="p_flt_AllBWDL">存储所有表位电量</param>
        /// <returns></returns>
        private void ReadDL(bool p_bln_IsSpecial, int int_Times, ref float[] p_flt_AllBWDL)
        {
            float[] flt_TempDL = new float[1] { -1F };
            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            if (p_bln_IsSpecial)
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadSpecialEnergy(5, int_Times);
            }
            else
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((int)Cus_PowerFangXiang.正向有功, 0);
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
            string str_TempValue = string.Empty;
            string[] strReturnData = new string[BwCount];
            strReturnData = MeterProtocolAdapter.Instance.ReadPatternWord(5);

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
                    //return;
                }
                p_str_PW[k] = str_TempValue;
            }
        }

        /// <summary>
        /// 存储整点冻结起始时间
        /// </summary>
        /// <param name="p_str_StartTime"></param>
        /// <returns></returns>
        private void FillFreezeTime(ref string[] p_str_StartTime, int type)
        {
            string[] str_TempPW = new string[1] { "" };
            string str_TempValue = string.Empty;
            string[] strReturnData = new string[BwCount];
            // strReturnData = MeterProtocolAdapter.Instance.ReadFreezeTime(4);
            strReturnData = MeterProtocolAdapter.Instance.ReadFreezeTime(type);
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
                    continue;
                }
                p_str_StartTime[k] = str_TempValue;
            }
        }

        /// <summary>
        /// 存储整点冻结时间间隔
        /// </summary>
        /// <param name="p_str_Interval"></param>
        /// <returns></returns>
        private void FillFreezeInterval(ref string[] p_str_Interval)
        {
            string[] str_TempPW = new string[1] { "" };
            string str_TempValue = string.Empty;
            string[] strReturnData = new string[BwCount];
            strReturnData = MeterProtocolAdapter.Instance.ReadFreezeInterval();

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
                    // return;
                }
                p_str_Interval[k] = str_TempValue;
            }
        }

        /// <summary>
        /// 清理数据节点
        /// </summary>
        protected override void ClearItemData()
        {
            string strKey = ItemKey;
            MeterBasicInfo curMeter = null;
            string strCurKey = string.Empty;
            for (int i = 1; i < 4; i++)
            {
                for (int k = 0; k < BwCount; k++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                    if (!curMeter.YaoJianYn) continue;
                    strCurKey = string.Format("0050{0}", i);
                    if (curMeter.MeterFreezes.ContainsKey(strCurKey))
                    {
                        curMeter.MeterFreezes.Remove(strCurKey);
                    }
                }
            }
            base.ClearItemData();
        }
    }
}
