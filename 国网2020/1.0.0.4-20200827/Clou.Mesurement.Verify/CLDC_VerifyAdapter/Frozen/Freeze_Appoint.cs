
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 约定冻结检定 (1两套时区表切换冻结,2两套日时段表切换冻结,3两套费率电价切换冻结,4两套阶梯切换冻结)
    /// </summary>
    public class Freeze_Appoint : FreezeBase
    {


        MeterBasicInfo curMeter = null;
        MeterFreeze curResult;
        public Freeze_Appoint(object plan) : base(plan) { }

        protected override bool CheckPara()
        {
           ResultNames = new string[] { "两套时区表切换冻结前上一次冻结总电量","两套时区表切换冻结前电量","两套时区表切换冻结后电量","两套时区表切换冻结后上一次冻结总电量",
                                        "两套日时段表切换冻结前上一次冻结总电量","两套日时段表切换冻结前电量","两套日时段表切换冻结后电量","两套日时段表切换冻结后上一次冻结总电量",
                                        //"两套费率电价切换冻结前上一次冻结总电量","两套费率电价切换冻结前电量","两套费率电价切换冻结后电量","两套费率电价切换冻结后上一次冻结总电量",
                                        ////"两套阶梯切换冻结前上一次冻结总电量","两套阶梯切换冻结前电量","两套阶梯切换冻结后电量","两套阶梯切换冻结后上一次冻结总电量",
                                         "结论" };
           return true;
        }


        /// <summary>
        /// 约定冻结检定
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            #region -----------------局部变量------------------
            string str_Result = string.Empty;
            string str_AllResult = Variable.CTG_HeGe;
            DateTime dtm_meterTime = DateTime.Now;
            string[] str_PatternWord = new string[BwCount];
            bool[,] AllResult = new bool[BwCount,4];
            bool[] bReturn = new bool[BwCount];
            string[][] lstFreezePW = new string[][] { new string[BwCount], new string[BwCount] }; //存储两次冻结模式字
            bool bResult;
            #endregion

            base.Verify();
            if (!PowerOn(true))
            {
                MessageController.Instance.AddMessage("源输出失败");
                return;
            }
            MessageController.Instance.AddMessage("等待电表启动......");
            Thread.Sleep(5000);    //延时5S

            //读冻结模式字
            string[] arrStrResultKey = new string[BwCount];

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            //bool blocalMeter = GetlocalMeter();
            MessageController.Instance.AddMessage("开始读取冻结模式字");
            str_PatternWord = MeterProtocolAdapter.Instance.ReadPatternWord(4);

            FillPatternWord(ref lstFreezePW[0]);
            //Comm.GlobalUnit.g_MsgControl.OutMessage("读取约定冻结模式字完毕");
            //bReturn = MeterProtocolAdapter.Instance.WritePatternWord(4, "FF");

            MessageController.Instance.AddMessage("开始读取约定冻结模式字");

            str_PatternWord = MeterProtocolAdapter.Instance.ReadPatternWord(4);

            FillPatternWord(ref lstFreezePW[1]);

            int iCount = 3;
            //if (blocalMeter) //如果是本地表多两项选择。 //两套费率电价切换，两套阶梯切换只有本地表能测，所以移除本试验
            //{
            //    iCount = 5;
            //}
            //else
            //{
            //    iCount = 3;
            //}
            for (int i = 1; i < iCount; i++)
            {
                bool[] TempResult = new bool[BwCount];
                FreezeDeal(i, ref  TempResult);

                for (int j = 0; j < TempResult.Length; j++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn == false) continue;
                    AllResult[j,i-1] = TempResult[j];
                }
                if (Stop) return;
            }

            //for (int j = 0; j < BwCount; j++)
            //{
            //    //强制停止
            //    if (Stop) return;
            //    //恢复表原来的冻结模式
            //    bReturn = MeterProtocolAdapter.Instance.WritePatternWord(4, lstFreezePW[0][j]);

            //}
            //恢复表时间为当前时间
            
            DateTime readTime = DateTime.Now;

            bReturn = MeterProtocolAdapter.Instance.WriteDateTime(readTime.ToString("yyMMddHHmmss"));


            //结论
            for (int j = 0; j < BwCount; j++)
            {
                //强制停止
                if (Stop) return;

                if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                {
                    continue;
                }
                if (AllResult[j, 0] && AllResult[j, 1])
                {
                    ResultDictionary["结论"][j] = "合格";
                    
                }
                else
                {
                    ResultDictionary["结论"][j] = "不合格";
                }
            }
            UploadTestResult("结论");
        }



        /// <summary>
        /// 切换约定冻结项
        /// </summary>
        /// <param name="str_Type">类型</param>
        /// <param name="str_Result">结果</param>
        /// <returns></returns>
        private void FreezeDeal(int int_Type, ref bool[] str_Result)
        {
            string[] strName = new string[] { "两套时区表切换", "两套日时段表切换", "两套费率电价切换", "两套阶梯切换" };

            #region  --------------局部变量---------------
            bool bResult;
            DateTime dtm_meterTime = DateTime.Now;
            bool[] bReturn = new bool[BwCount];
            float[] flt_TempDL = new float[1] { -1F };
            for (int i = 0; i < str_Result.Length; i++)
            {
                str_Result[i] = false;
            }
            string str_Msg = string.Empty;
            string str_SwitchTime = string.Empty;
            string str_TempValues = string.Empty;
            bool bln_FlagDL = false;
            string str_Freeze = string.Empty;
            //维数 0=冻结前上一次冻结电量，1=冻结前当前表电量，2=冻结后当前表电量，3=冻结后上一次冻结电量
            float[][] flt_DL = new float[][] { new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount] };
            #endregion

            switch (int_Type)
            {
                case 1:
                    str_Msg = "两套时区表切换";
                    break;
                case 2:
                    str_Msg = "两套日时段表切换";
                    break;
                case 3:
                    str_Msg = "两套费率电价切换";
                    break;
                case 4:
                    str_Msg = "两套阶梯切换";
                    break;
            }
            if (Stop) return;
            GlobalUnit.g_MsgControl.OutMessage(str_Msg, false);
            MeterProtocolAdapter.Instance.ReadEnergy((int)Cus_PowerFangXiang.正向有功, 0, 0);
            GlobalUnit.g_MsgControl.OutMessage("正在读取冻结前电表总电量......", false);
            ReadDL(false, int_Type, 1, ref flt_DL[0]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int_Type - 1] + "冻结前电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
            if (Stop) return;
            GlobalUnit.g_MsgControl.OutMessage("正在读取上一次电表冻结总电量......", false);
            ReadDL(true, int_Type, 1, ref flt_DL[1]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int_Type - 1] + "冻结前上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[1]));

            //判断冻结电量是否与当前电量相等
            //for (int j = 0; j < flt_DL[0].Length; j++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
            //    if (flt_DL[0][j] == flt_DL[1][j])//&& flt_DL[0][j] != 0
            //    {
            //        bln_FlagDL = true;
            //        break;
            //    }
            //}
            //冻结电量与当前电量相等，则进行走字
            //if (bln_FlagDL)
            {
                GlobalUnit.g_MsgControl.OutMessage("最大电流进行走字20S，请稍候......", false);
                //升源
                if (Stop) return;
                if (!PowerOn())
                {
                    PowerOn();
                }
                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 20);
                if (Stop) return;
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
                if (Stop) return;
                GlobalUnit.g_MsgControl.OutMessage("正在设置冻结测试参数", false);
                if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U,(int) CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功))
                {
                    //Check.Require(false, "控制源输出失败");
                }
                //读冻结模式字
                GlobalUnit.g_MsgControl.OutMessage("等待电表启动......", false);
                Thread.Sleep(5000);  //延时源稳定5S
                if (Stop) return;
            }

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            //当前年份+1；
            string year = dtm_meterTime.AddYears(int_Type).ToString("yy");
            str_SwitchTime = year + "01010000";
            // tm_meterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime("131001010000");
            //切换时间
            //str_SwitchTime = dtm_meterTime.AddMinutes(1).ToString("yyMMddHHmm");
            
            GlobalUnit.g_MsgControl.OutMessage(string.Format("正在{0}......", str_Msg), false);
            //bReturn = MeterProtocolAdapter.Instance.WriteSwitchTime(str_Type, str_SwitchTime);
            bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(int_Type, str_SwitchTime);
            if (Stop) return;
            string str_MeterTime = string.Format("{0}1231235955", dtm_meterTime.AddYears(int_Type - 1).ToString("yy"));
            
            bReturn = MeterProtocolAdapter.Instance.WriteDateTime(str_MeterTime);


            if (Stop) return;
            //冻结
            //GlobalUnit.g_MsgControl.OutMessage("正在下发冻结指令......", false);
            //str_Freeze = string.Format("{0}{1}{2}{3}", dtm_meterTime.AddSeconds(10).Month.ToString("D2"), dtm_meterTime.AddSeconds(10).Day.ToString("D2"),
            //        dtm_meterTime.AddSeconds(10).Hour.ToString("D2"), dtm_meterTime.AddSeconds(10).Minute.ToString("D2"));
            //bReturn = MeterProtocolAdapter.Instance.FreezeCmd(str_Freeze);
            //bResult = GetArrValue(bReturn, ref str_Result);
            //if (!bResult)
            //{
            //    //Stop = true;
            //    GlobalUnit.g_MsgControl.OutMessage(str_Msg + "冻结失败", true, Cus_MessageType.运行时消息);
            //    //return;
            //}
            if (Stop) return;
            GlobalUnit.g_MsgControl.OutMessage("延时60S，请稍候......", false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);        //延时15S
            if (Stop) return;
            GlobalUnit.g_MsgControl.OutMessage("读取当前电表总电量", false);
            ReadDL(false, int_Type, 1, ref flt_DL[2]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int_Type - 1] + "冻结后电量", ConvertArray.ConvertFloat2Str(flt_DL[2]));
            if (Stop) return;
            GlobalUnit.g_MsgControl.OutMessage("读取上一次冻结总电量", false);
            ReadDL(true, int_Type, 1, ref flt_DL[3]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, strName[int_Type - 1] + "冻结后上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[3]));

            //上报检定数据
            for (int j = 0; j < BwCount; j++)
            {
                //强制停止
                //if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                {
                    continue;
                }
                if (Math.Abs(flt_DL[2][j] - flt_DL[3][j]) < 0.01)
                {
                    str_Result[j] = true;

                }
                else
                {
                    str_Result[j] = false;
                }


            }
            //恢复时段表
            if (int_Type == 2)
            {
                year = dtm_meterTime.AddYears(int_Type + 1).ToString("yy");
                str_SwitchTime = year + "01010000";
                // tm_meterTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime("131001010000");
                //切换时间
                //str_SwitchTime = dtm_meterTime.AddMinutes(1).ToString("yyMMddHHmm");
                if (Stop) return;
                GlobalUnit.g_MsgControl.OutMessage(string.Format("正在{0}......", str_Msg), false);
                //bReturn = MeterProtocolAdapter.Instance.WriteSwitchTime(str_Type, str_SwitchTime);
                bReturn = MeterProtocolAdapter.Instance.WriteFreezeInterval(int_Type, str_SwitchTime);
                //bResult = GetArrValue(bReturn, ref str_Result);
                //if (!bResult)
                //{
                //    //Stop = true;
                //    GlobalUnit.g_MsgControl.OutMessage(string.Format("切换{0}失败", str_Msg), true, Cus_MessageType.运行时消息);
                //    //return;
                //}
                str_MeterTime = string.Format("{0}1231235955", dtm_meterTime.AddYears(int_Type).ToString("yy"));
                if (Stop) return;
                bReturn = MeterProtocolAdapter.Instance.WriteDateTime(str_MeterTime);
                if (Stop) return;
                GlobalUnit.g_MsgControl.OutMessage("延时60S，恢复时段表请稍候......", false);
                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="p_bln_IsSpecial">true=冻结电量，false=实际电量</param>
        /// <param name="p_flt_AllBWDL">存储所有表位电量</param>
        /// <returns></returns>
        private void ReadDL(bool p_bln_IsSpecial, int int_Type, int int_Times, ref float[] p_flt_AllBWDL)
        {
            float[] flt_TempDL = new float[1] { -1F };
            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            if (p_bln_IsSpecial)
            {
                if (int_Type == 1)
                    int_Type = 7;//两套时区表切换
                else if (int_Type == 2)
                    int_Type = 8;//两套日时段表切换
                else if (int_Type == 3)
                    int_Type = 9;//两套费率电价切换
                else if (int_Type == 4)
                    int_Type = 10;//两套阶梯切换
                dicEnergy = MeterProtocolAdapter.Instance.ReadSpecialEnergy(int_Type, int_Times);
            }
            else
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((int)Cus_PowerFangXiang.正向有功, 0);
            }
            for (int j = 0; j < BwCount; j++)
            {
                //强制停止
                //if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn || !dicEnergy.ContainsKey(j))
                {
                    continue;
                }

                flt_TempDL = dicEnergy[j];
                if (flt_TempDL[0] < 0F)
                {
                    GlobalUnit.g_MsgControl.OutMessage(string.Format("表位[0]返回的数据不符合要求", j + 1));
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
            for (int i = 1; i < 5; i++)
            {
                for (int k = 0; k < BwCount; k++)
                {
                    //强制停止
                    if (Stop) return;
                    curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                    if (!curMeter.YaoJianYn) continue;
                    strCurKey = string.Format("0040{0}", i);
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
