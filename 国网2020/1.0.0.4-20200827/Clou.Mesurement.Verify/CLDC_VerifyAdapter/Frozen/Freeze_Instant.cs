
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 瞬时冻结检测
    /// </summary>
    public class Freeze_Instant : FreezeBase
    {
        public Freeze_Instant(object plan) : base(plan) { }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "冻结前电量", "第一次冻结电量", "第二次冻结电量", "第三次冻结电量","冻结后电量",
                                         "结论" };
            return true;
        }

        /// <summary>
        /// 瞬时冻结检定
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            #region  --------------局部变量---------------
            bool[] bReturn = new bool[BwCount];
            string[][] lstFreezePW = new string[][] { new string[BwCount], new string[BwCount] }; //存储两次冻结模式字
            float[][] flt_AllDL = new float[4][];
            float[] flt_TempDL = new float[1] { -1F };
            string str_Result = string.Empty;
            bool bResult;
            #endregion

            if (Stop) return;
            base.Verify();
            //升源
            string[] arrStrResultKey = new string[BwCount];
            if (Stop) return;
            if (!PowerOn(true))
            {
                MessageController.Instance.AddMessage("源输出失败", 6, 1);
                return;
            }
            //读冻结模式字
            MessageController.Instance.AddMessage("等待电表启动......");
            //Thread.Sleep(5000);  //延时源稳定5S
            //MessageController.Instance.AddMessage("开始读取冻结模式字");
            //FillPatternWord(ref lstFreezePW[0]);

            //Comm.MessageController.Instance.AddMessage("开始写冻结模式字");
            ////
            //bReturn = MeterProtocolAdapter.Instance.WritePatternWord(2, "FF");
            //bResult = GetArrValue(bReturn);
            //if (!bResult)
            //{
            //    Comm.MessageController.Instance.AddMessage("写冻结模式字失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
            //    //return;
            //}
            //MessageController.Instance.AddMessage("开始读取冻结模式字");
            //FillPatternWord(ref lstFreezePW[1]);

            //冻结处理
            if (Stop) return;
            FreezeDeal();
            
            
            // 不恢复冻结模式   zayfan
            ////for (int j = 0; j < BwCount; j++)
            ////{
            //    //恢复表原来的冻结模式
            //    if (!Adapter.Adpater485.WritePatternWord(2, lstFreezePW[0][j], false))
            //    {
            //        Stop = true;
            //        Comm.MessageController.Instance.AddMessage("写冻结模式字失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
            //        return;
            //    }

            //    //冻结处理
            //    FreezeDeal();

            //    //恢复表原来的冻结模式
            //    if (!Adapter.Adpater485.WritePatternWord(2, lstFreezePW[0][j], false))
            //    {
            //        //Stop = true;
            //        MessageController.Instance.AddMessage(string.Format("hu恢复第[0]表位冻结模式字失败", j + 1));
            //        //return;
            //    }
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
       
            
                //恢复表时间为当前时间
         //   bReturn = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            //bResult = GetArrValue(bReturn);
            //if (!bResult)
            //{
            //    Stop = true;
            //    MessageController.Instance.AddMessage("写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
            //    //return;
            //}
            //for (int i = 0; i < BwCount; i++)
            //{
            //    arrStrResultKey[i] = ItemKey;

            //}
            //}
            MessageController.Instance.AddMessage("瞬时冻结检定完毕");
            
        //    GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_FREEZE, arrStrResultKey);
        }

        /// <summary>
        /// 冻结处理
        /// </summary>
        /// <returns></returns>
        private void FreezeDeal()
        {
            #region -------------------初始化变量-------------------
            bool bResult;
            bool[] bReturn = new bool[BwCount];
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = null;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze curResult;
            DateTime dtm_meterTime = DateTime.Now;
            string str_MeterTime = string.Empty;
            string str_DateTime = string.Empty;
            string str_Result = string.Empty;
            string str_Item = string.Empty;
            string str_TempValues = string.Empty;
            bool bln_FlagDL = false;                    //冻结电量是否与当前电量相等标志
            //维数 0=冻结前上一次冻结电量，1=冻结前当前表电量，2=冻结后当前表电量，3=冻结后上一次冻结电量
            float[][] flt_DL = new float[][] { new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount] };
            #endregion

            #region ----------------读取冻结前数据------------------
            //if (Stop) return;
            //MessageController.Instance.AddMessage("瞬时冻结前读取上一次冻结总电量");
            //flt_DL[0] = MeterProtocolAdapter.Instance.ReadData("05010101", 4, 2);
            ////MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey,"冻结前上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
            if (Stop) return;
            MessageController.Instance.AddMessage("瞬时冻结前读取当前电表总电量");
            flt_DL[0] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "冻结前电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
            #endregion

            #region ----------------读取冻结前数据------------------
            //判断冻结电量是否与当前电量相等
            //bln_FlagDL = false;
            //for (int i = 0; i < flt_DL[0].Length; i++)
            //{
            //    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
            //    {
            //        if (flt_DL[0][i] == flt_DL[1][i])
            //        {
            //            bln_FlagDL = true;
            //            break;
            //        }
            //    }
            //}


            //冻结电量与当前电量相等，则进行走字
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
                //Thread.Sleep(5000);  //延时源稳定5S
            }
            #endregion

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();


            MessageController.Instance.AddMessage("瞬时冻结前读取当前电表总电量");
            if (Stop) return;
            flt_DL[1] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "冻结后电量", ConvertArray.ConvertFloat2Str(flt_DL[1]));

            #region ----------------执行冻结操作----------------------
            if (Stop) return;
            MessageController.Instance.AddMessage("开始进行瞬时冻结");
            bReturn = MeterProtocolAdapter.Instance.FreezeCmd("99999999");

            MessageController.Instance.AddMessage("延时5S,请稍候......");
            if (Stop) return;
            Thread.Sleep(5000);
            #endregion


            #region -------------------读取冻结后的数据-----------------

            if (Stop) return;
            MessageController.Instance.AddMessage("瞬时冻结后读取上1次冻结总电量");
            flt_DL[3] = MeterProtocolAdapter.Instance.ReadData("05010101", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次冻结电量", ConvertArray.ConvertFloat2Str(flt_DL[3]));

            if (Stop) return;
            bReturn = MeterProtocolAdapter.Instance.FreezeCmd("99999999");
            if (Stop) return;
            Thread.Sleep(2000);

            if (Stop) return;
            MessageController.Instance.AddMessage("瞬时冻结后读取上1次冻结总电量");
            flt_DL[4] = MeterProtocolAdapter.Instance.ReadData("05010101", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次冻结电量", ConvertArray.ConvertFloat2Str(flt_DL[4]));

            if (Stop) return;
            bReturn = MeterProtocolAdapter.Instance.FreezeCmd("99999999");
            Thread.Sleep(2000);
            if (Stop) return;
            MessageController.Instance.AddMessage("瞬时冻结后读取上1次冻结总电量");
            flt_DL[5] = MeterProtocolAdapter.Instance.ReadData("05010101", 4, 2);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第三次冻结电量", ConvertArray.ConvertFloat2Str(flt_DL[5]));


            //判断结论
            for (int k = 0; k < BwCount; k++)
            {
                //强制停止
                if (Stop) return;
                //判断
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    continue;
                }
                curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (flt_DL[1][k] == -1 || flt_DL[3][k] == -1 || flt_DL[4][k] == -1 || flt_DL[5][k] == -1)
                {
                    ResultDictionary["结论"][k] = Variable.CTG_BuHeGe;
                }
                else if (flt_DL[1][k] == flt_DL[3][k] &&flt_DL[1][k] == flt_DL[4][k]&& flt_DL[1][k] == flt_DL[5][k])
                {
                    ResultDictionary["结论"][k] = Variable.CTG_HeGe;
                }
                else
                {
                    ResultDictionary["结论"][k] = Variable.CTG_BuHeGe;
                }           
            }

            UploadTestResult("结论");

            #endregion

        }


        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="p_bln_IsSpecial">true=冻结电量，false=实际电量</param>
        /// <param name="int_Times">冻结电量次数</param>
        /// <param name="p_flt_AllBWDL">存储所有表位电量</param>
        /// <returns></returns>
        private void ReadDL(bool p_bln_IsSpecial,int int_Times, ref float[] p_flt_AllBWDL)
        {
            float[] flt_TempDL = new float[1] { -1F };

            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            if (p_bln_IsSpecial)
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadSpecialEnergy(6, int_Times);
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
            string str_TempValue = string.Empty;
            string[] str_ReturnData = new string[BwCount];
            str_ReturnData = MeterProtocolAdapter.Instance.ReadPatternWord(1);
            
            for (int k = 0; k < BwCount; k++)
            {
                //强制停止
                if (Stop) return;
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    continue;
                }
                str_TempValue = str_ReturnData[k];
                if (str_TempValue == "")
                {
                    MessageController.Instance.AddMessage(string.Format("表位[0]读取冻结模式字失败", k + 1));
                    //return;
                }
                p_str_PW[k] = str_TempValue;
            }
        }

    }
}
