
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
    /// 分钟冻结检定
    /// </summary>
    public class Freeze_Minute : FreezeBase
    {
        public Freeze_Minute(object plan) : base(plan) { }


        protected override bool CheckPara()
        {
            ResultNames = new string[] { "分钟冻结前上一次冻结总电量", "分钟冻结前电量", "分冻结后电量", "分冻结后上一次冻结总电量", "结论" };
            return true;
        }

        /// <summary>
        /// 日冻结检定
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            #region  --------------局部变量---------------
            string[][] lstFreezePW = new string[][] { new string[BwCount], new string[BwCount] }; //存储两次冻结模式字
            bool[] bReturnData = new bool[BwCount];
            #endregion



            if (Stop) return;
            base.Verify();
            if (Stop) return;
            if (!PowerOn(true))
            {
                MessageController.Instance.AddMessage("源输出失败", 6, 1);
                return;
            }
            try
            {
                string[] strID = new string[BwCount];
                string[] strData = new string[BwCount];
                string[] strSetData = new string[BwCount];
                int[] iFlag = new int[BwCount];
                string[] strShowData = new string[BwCount];
                string[] strCode = new string[BwCount];
                string[] strRand1 = new string[BwCount];//随机数
                string[] strRand2 = new string[BwCount];//随机
                string[] strEsamNo = new string[BwCount];//Esam序列号

                MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

                if (Stop) return;
                iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strID[i] = "0400010C";
                    strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strData[i] += DateTime.Now.ToString("HHmmss");
                }
                bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

                if (Stop) return;
                FreezeDeal();

                #region -----------------恢复电表原来状态---------------
                //for (int j = 0; j < BwCount; j++)
                //{
                //    //恢复表原来的冻结模式
                //    if (!Adapter.Adpater485.WritePatternWord(3, lstFreezePW[0][j]))
                //    {
                //        MessageController.Instance.AddMessage(string.Format("hu恢复第[0]表位冻结模式字失败", j + 1));
                //       // return;
                //    }


                //}
                string str_MeterTime = DateTime.Now.ToString("yyMMddHHmmss");

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
                bReturnData = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

                //恢复表时间为当前时间
             //   bReturnData = MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                //bool bResult = GetArrValue(bReturnData);
                //if (!bResult)
                //{
                //    MessageController.Instance.AddMessage("写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
                //    return;
                //}

                #endregion
                MessageController.Instance.AddMessage("分冻结检定完毕");
                
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
        /// <returns></returns>
        private void FreezeDeal()
        {
            #region -------------------声明和初始化局部变量-----------------
            bool[] bReturn = new bool[BwCount];
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = null;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze curResult;
            DateTime dtm_meterTime = DateTime.Now;
            string str_MeterTime = string.Empty;
            string str_DateTime = string.Empty;
            string str_Result = string.Empty;
            string str_TempValues = string.Empty;
            //维数 0=冻结前上一次冻结电量，1=冻结前当前表电量，2=冻结后当前表电量，3=冻结后上一次冻结电量
            float[][] flt_DL = new float[][] { new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount], new float[BwCount] };
            float[] flt_TempDL = new float[1] { -1F };
            bool bln_FlagDL = false;
            #endregion

            try
            {
                #region ----------------读取冻结前数据------------------
                if (Stop) return;
                MessageController.Instance.AddMessage("分钟冻结前读取上一次冻结总电量");
                string strTime = DateTime.Now.AddMinutes(-1).ToString("yyMMddHHmm") + "01";
                string[] strDLQ = MeterProtocolAdapter.Instance.ReadLoadRecord("06110601", 5, strTime);
                flt_DL[0] = GetDL(strDLQ);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "分钟冻结前上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[0]));
                if (Stop) return;
                MessageController.Instance.AddMessage("分钟冻结前读取当前电表总电量");
                flt_DL[1] = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "分钟冻结前电量", ConvertArray.ConvertFloat2Str(flt_DL[1]));
                bln_FlagDL = false;
                //判断冻结电量是否与当前电量相等
                //for (int j = 0; j < flt_DL[0].Length; j++)
                //
                //    if (flt_DL[0][j] == flt_DL[1][j])// && flt_DL[0][j] != 0
                //    {
                //        bln_FlagDL = true;
                //        break;
                //    }
                //}
                //冻结电量与当前电量相等，则进行走字
                //if (bln_FlagDL)
                {
                    MessageController.Instance.AddMessage("最大电流进行走字30S，请稍候......");
                    //升源
                    if (Stop) return;
                    if (!PowerOn())
                    {
                        throw new Exception("升源失败");
                    }
                    
                    if (Stop) return;
                    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
                    MessageController.Instance.AddMessage("正在设置冻结测试参数");
                    if (!CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功))
                    {
                        Check.Require(false, "控制源输出失败");
                    }
                    //读冻结模式字
                    MessageController.Instance.AddMessage("等待电表启动......");
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 120);
                    //Thread.Sleep(5000);  //延时源稳定5S
                }
                #endregion

                MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

                #region -------------------分钟冻结处理----------------

                if (Stop) return;
                MessageController.Instance.AddMessage("分钟冻结后读取当前电表总电量");
                flt_DL[2] = MeterProtocolAdapter.Instance.ReadData("00010000", 4,2);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "分钟冻结后电量", ConvertArray.ConvertFloat2Str(flt_DL[2]));
                if (Stop) return;
                MessageController.Instance.AddMessage("分钟冻结后读取上一次冻结总电量");
                strTime = DateTime.Now.AddMinutes(-1).ToString("yyMMddHHmm") + "01";
                string[] strDLH = MeterProtocolAdapter.Instance.ReadLoadRecord("06110601", 4, strTime);
                flt_DL[3] = GetDL(strDLH);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "分钟冻结后上一次冻结总电量", ConvertArray.ConvertFloat2Str(flt_DL[3]));


                #endregion

                string[] arrStrResultKey = new string[BwCount];
                #region --------------------处理结论------------------
                for (int j = 0; j < BwCount; j++)
                {
                    //强制停止
                    if (Stop) return;
                    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                    {
                        continue;
                    }

                    if (flt_DL[0][j] < flt_DL[3][j] && Math.Abs(flt_DL[2][j] - flt_DL[3][j]) <= 0.01)
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
        private void ReadDL(bool p_bln_IsSpecial,int int_Times, ref float[] p_flt_AllBWDL)
        {
            float[] flt_TempDL = new float[1] { -1F };
            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            if (p_bln_IsSpecial)
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadSpecialEnergy(4, int_Times);
            }
            else
            {
                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergy(0x00);
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
                if (flt_TempDL[j] < 0F)
                {
                    MessageController.Instance.AddMessage(string.Format("表位[0]返回的数据不符合要求", j + 1));
                    continue;
                }
                else
                {
                    p_flt_AllBWDL[j] = flt_TempDL[j];
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
            strReturnData = MeterProtocolAdapter.Instance.ReadPatternWord(3);
            
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
        /// 截取电量值并转换成Float
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private float[] GetDL(string[] strData)
        {
            float[] RevData = new float[strData.Length];
            for (int i = 0; i < BwCount; i++)
            {
                if (!string.IsNullOrEmpty(strData[i])&& strData[i].Length >=10)
                {
                    RevData[i] = float.Parse(strData[i].Substring(0, 10)) / 10000;
                }
            }
            return RevData;
        }
    }
}
