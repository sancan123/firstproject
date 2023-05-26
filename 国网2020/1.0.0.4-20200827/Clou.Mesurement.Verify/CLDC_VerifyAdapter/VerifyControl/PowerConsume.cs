using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using System.Threading;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 功耗试验检定器
    /// </summary>
    class PowerConsume : VerifyBase
    {
        public PowerConsume(object plan)
            : base(plan)
        {
        }

        #region----------重写数据主键ItemKey
        /// <summary>
        /// 总结论主键
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.功耗试验);
            }
        }

        /// <summary>
        /// 重写数据主键
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}"                                          //Key:参见数据结构设计附2
                        , ((int)Cus_MeterResultPrjID.功耗试验).ToString()
                        , "11");
            }
        }
        /// <summary>
        /// 当前方案[试验]
        /// </summary>
        private new StPowerConsume CurPlan
        {
            get
            {
                return (StPowerConsume)base.CurPlan;
            }
        }
        protected override bool CheckPara()
        {
            string[] paras = CurPlan.PrjParm.Split(',');
            if (paras != null && paras.Length >= 3)
            {
                try
                {
                    U_P_Limit = float.Parse(paras[0]);
                    U_S_Limit = float.Parse(paras[1]);
                    I_S_Limit = float.Parse(paras[2]);
                }
                catch
                {
                    U_P_Limit = 1.5F;
                    U_S_Limit = 6F;
                    I_S_Limit = 1F;
                }
                
            }
            else
            {
                U_P_Limit = 1.5F;
                U_S_Limit = 6F;
                I_S_Limit = 1F;
            }
            return true;
        }
        #endregion

        #region 声明
        private float U_P_Limit = 1.5F;
        private float U_S_Limit = 6F;
        private float I_S_Limit = 1F;
        private bool[] bBiaoweiBz = null;
        #endregion

        #region----------检定控制----------
        /// <summary>
        /// 开始功耗检定
        /// </summary>
        /// <param name="ItemNumber">方案序号</param>
        public override void Verify()
        {
            if (GlobalUnit.IsDemo)
            {
                return;
            }

            base.Verify();

            //add by zxr 20140923 增加选择表位进行读取操作处理
            bBiaoweiBz = new bool[BwCount];
            for (int o = 0; o < BwCount; o++)
            {
                bBiaoweiBz[o] = false;
            }

            //if (CLDC_DeviceDriver.Setting.ConfigHelper.specialInfor.selectBiaowei == "0")
            //{
            //    for (int i = 0; i < BwCount; i++)
            //    {
            //        bBiaoweiBz[i] = true;
            //    }
            //}
            //else
            //{
            //    string[] strSelectBw = CLDC_DeviceDriver.Setting.ConfigHelper.specialInfor.selectBiaowei.Split(',');
            //    foreach (string st in strSelectBw)
            //    {
            //        int bw = Convert.ToInt32(st) - 1;
            //        bBiaoweiBz[bw] = true;
            //    }
            //}
            //重新发送一次隔离表位，只做选择配置做功耗的表位 20140924
            MeterBasicInfo mbi = new MeterBasicInfo();
            bool[] YJMeter = new bool[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                mbi = Helper.MeterDataHelper.Instance.Meter(i);
                YJMeter[i] = mbi.YaoJianYn & bBiaoweiBz[i];
            }

          
                    Helper.EquipHelper.Instance.SetLoadRelayControl(YJMeter, 0);
               
        //    Helper.EquipHelper.Instance.SetMeterOnOff(YJMeter);

            Thread.Sleep(2000);
            //end

            //if (Cus_LossBoardType.LossBoard == CLDC_DeviceDriver.Setting.ConfigHelper.specialInfor.BoardReadLoss)
            //{
            Verify_PowerD();
            //}
            //else
            //{
            //    Verify_ErrPlate();
            //}

            MeterBasicInfo curMeter;
            MeterPower curResult;
            string[] arrStrResultKey = new string[BwCount];
            string strKey = ItemKey;
            if (Stop)
            {
                return;
            }
            for (int k = 0; k < BwCount; k++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (curMeter.YaoJianYn && bBiaoweiBz[k])
                {
                    if (curMeter.MeterPowers == null)
                    {
                        curMeter.MeterPowers = new Dictionary<string, MeterPower>();
                    }
                    //挂接结论
                    if (!curMeter.MeterPowers.ContainsKey(strKey))
                    {
                        curResult = new MeterPower();
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = "功耗测试";
                        curMeter.MeterPowers.Add(strKey, curResult);
                    }
                    else
                    {
                        curResult = curMeter.MeterPowers[strKey];
                    }

                    //上报数据
                    
                    
                }
                arrStrResultKey[k] = ItemKey;

                
            }
         //   GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_POWER_CONSUM_DATA, arrStrResultKey);
        }
        /// <summary>
        /// 误差板功耗
        /// </summary>
        private void Verify_ErrPlate()
        {
            if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            {
                return;
            }
            if (Stop)
            {
                Helper.LogHelper.Instance.Loger.Debug("外部停止，退出检定");
                return;
            }
            MessageController.Instance.AddMessage("控制源输出" + GlobalUnit.U + "V," + GlobalUnit.Ib + "A...");
            if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.正向有功, "1.0", true, false))
            {
                Check.Require(false, "控制源输出失败");
            }
            int sleepTime = 5;
            while (sleepTime > 1)
            {
                if (Stop)
                {
                    break;
                }
                sleepTime -= 1;
                MessageController.Instance.AddMessage("等待源稳定" + sleepTime + "秒...");
                Thread.Sleep(1000);
            }
            if (Stop)
            {
                return;
            }
            sleepTime = 10;
            while (sleepTime > 1)
            {
                if (Stop)
                {
                    break;
                }
                sleepTime -= 1;
                MessageController.Instance.AddMessage("正在测试功耗,等待" + sleepTime + "秒...");
                Thread.Sleep(1000);
            }
            if (Stop)
            {
                return;
            }
            MessageController.Instance.AddMessage("正在进行功耗测试...");
            bool[] YJMeter = new bool[BwCount];
            CLDC_DataCore.Struct.stGHPram[] tagGH = null;
            MeterBasicInfo curMeter;
            
            for (int i = 0; i < BwCount; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                YJMeter[i] = curMeter.YaoJianYn & bBiaoweiBz[i];
            }
            if (Array.IndexOf(YJMeter, true) != -1)
            {
                MessageController.Instance.AddMessage("正在读取功耗测试数据...");
                sleepTime = 3;
                while (sleepTime > 1)
                {
                    if (Stop)
                    {
                        break;
                    }
                    sleepTime -= 1;
                    MessageController.Instance.AddMessage("正在读取功耗测试数据,等待" + sleepTime + "秒...");
                    Thread.Sleep(1000);
                }
                if (Stop)
                {
                    return;
                }
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.ReadErrPltGHPram(YJMeter, out tagGH);
            }
            if (Stop)
            {
                return;
            }
            if (tagGH == null)
            {
                MessageController.Instance.AddMessage("读取功耗测试数据失败！");
                return;
            }
            int count = tagGH.Length;
            CLDC_DataCore.Struct.StPower tagPower = new CLDC_DataCore.Struct.StPower();
            tagPower = CLDC_VerifyAdapter.Helper.EquipHelper.Instance.ReadPowerInfo();
            if (Stop)
            {
                return;
            }
            bool isDan = GlobalUnit.IsDan;
            MessageController.Instance.AddMessage("正在处理功耗数据...");
            MeterPower curResult;
            string strKey = ItemKey;
            string strFormat = "F4";
            for (int intBw = 0; intBw < this.BwCount; intBw++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(intBw);
                if (curMeter.MeterPowers == null)
                {
                    curMeter.MeterPowers = new Dictionary<string, MeterPower>();
                }
                if (curMeter.YaoJianYn && bBiaoweiBz[intBw])//zxr 
                {
                    //挂接数据
                    if (!curMeter.MeterPowers.ContainsKey(strKey))
                    {
                        curResult = new MeterPower();
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = "功耗测试";
                        curResult.Md_chrValue = Variable.CTG_HeGe;
                        curMeter.MeterPowers.Add(strKey, curResult);
                    }
                    else
                    {
                        curResult = curMeter.MeterPowers[strKey];
                        if (string.IsNullOrEmpty(curResult.Md_chrValue))
                        {
                            curResult.Md_chrValue = Variable.CTG_HeGe;
                        }
                    }
                }
                else
                {
                    continue;//不检表位继续下一表位
                }

                curResult.AVR_VOT_CIR_P_LIMIT = U_P_Limit.ToString();
                curResult.AVR_VOT_CIR_S_LIMIT = U_S_Limit.ToString();
                curResult.AVR_CUR_CIR_S_LIMIT = I_S_Limit.ToString();

                if (isDan == true)
                {
                    curResult.AVR_CUR_CIR_A_VOT = tagGH[intBw].BU_Ib_or_L1_U.ToString();
                    curResult.AVR_CUR_CIR_A_CUR = tagPower.Ia.ToString();
                    curResult.AVR_CUR_CIR_B_VOT = "";
                    curResult.AVR_CUR_CIR_B_CUR = "";
                    curResult.AVR_CUR_CIR_C_VOT = "";
                    curResult.AVR_CUR_CIR_C_CUR = "";
                    float Ia_S = (tagGH[intBw].BU_Ib_or_L1_U * tagPower.Ia / 1000F);
                    curResult.Md_Ia_ReactiveS = Ia_S.ToString(strFormat);
                    curResult.Md_Ib_ReactiveS = "---";
                    curResult.Md_Ic_ReactiveS = "---";

                    curResult.AVR_VOT_CIR_A_VOT = tagPower.Ua.ToString();
                    curResult.AVR_VOT_CIR_A_CUR = tagGH[intBw].AU_Ia_or_I.ToString();
                    curResult.AVR_VOT_CIR_A_ANGLE = tagGH[intBw].AU_Phia_or_Phi.ToString();
                    curResult.AVR_VOT_CIR_B_VOT = "";
                    curResult.AVR_VOT_CIR_B_CUR = "";
                    curResult.AVR_VOT_CIR_B_ANGLE = "";
                    curResult.AVR_VOT_CIR_C_VOT = "";
                    curResult.AVR_VOT_CIR_C_CUR = "";
                    curResult.AVR_VOT_CIR_C_ANGLE = "";
                    float Ua_P = (float)(tagGH[intBw].AU_Ia_or_I * tagPower.Ua * Math.Cos(tagGH[intBw].AU_Phia_or_Phi / 180F * Math.PI) / 1000F);
                    curResult.Md_Ua_ReactiveP = Ua_P.ToString(strFormat);
                    float Ua_S = (tagGH[intBw].AU_Ia_or_I * tagPower.Ua / 1000F);
                    curResult.Md_Ua_ReactiveS = Ua_S.ToString(strFormat);
                    curResult.Md_Ub_ReactiveP = "---";
                    curResult.Md_Ub_ReactiveS = "---";
                    curResult.Md_Uc_ReactiveP = "---";
                    curResult.Md_Uc_ReactiveS = "---";

                    
                    if (Math.Abs(Ia_S) > I_S_Limit || Math.Abs(Ua_P) > U_P_Limit || Math.Abs(Ua_S) > U_S_Limit)
                    {
                        curResult.Md_chrValue = Variable.CTG_BuHeGe;
                    }
                    else
                    {
                        curResult.Md_chrValue = Variable.CTG_HeGe;
                    }

                }
                else
                {
                    curResult.AVR_CUR_CIR_A_VOT = tagGH[intBw].AI_Ua.ToString();
                    curResult.AVR_CUR_CIR_A_CUR = tagPower.Ia.ToString();
                    float Ia_S = (tagGH[intBw].AI_Ua * tagPower.Ia / 1000F);
                    curResult.Md_Ia_ReactiveS = Ia_S.ToString(strFormat);

                    curResult.AVR_CUR_CIR_B_VOT = tagGH[intBw].BI_Ub.ToString();
                    curResult.AVR_CUR_CIR_B_CUR = tagPower.Ib.ToString();
                    float Ib_S = (tagGH[intBw].BI_Ub * tagPower.Ib / 1000F);
                    curResult.Md_Ib_ReactiveS = Ib_S.ToString(strFormat);

                    curResult.AVR_CUR_CIR_C_VOT = tagGH[intBw].CI_Uc.ToString();
                    curResult.AVR_CUR_CIR_C_CUR = tagPower.Ic.ToString();
                    float Ic_S = (tagGH[intBw].CI_Uc * tagPower.Ic / 1000F);
                    curResult.Md_Ic_ReactiveS = Ic_S.ToString(strFormat);

                    curResult.AVR_VOT_CIR_A_VOT = tagPower.Ua.ToString();
                    curResult.AVR_VOT_CIR_A_CUR = tagGH[intBw].AU_Ia_or_I.ToString();
                    curResult.AVR_VOT_CIR_A_ANGLE = tagGH[intBw].AU_Phia_or_Phi.ToString();
                    float Ua_P = (float)(tagGH[intBw].AU_Ia_or_I * tagPower.Ua * Math.Cos(tagGH[intBw].AU_Phia_or_Phi / 180F * Math.PI) / 1000F);
                    curResult.Md_Ua_ReactiveP = Ua_P.ToString(strFormat);
                    float Ua_S = (tagGH[intBw].AU_Ia_or_I * tagPower.Ua / 1000F);
                    curResult.Md_Ua_ReactiveS = Ua_S.ToString(strFormat);

                    curResult.AVR_VOT_CIR_B_VOT = tagPower.Ub.ToString();
                    curResult.AVR_VOT_CIR_B_CUR = tagGH[intBw].BU_Ib_or_L1_U.ToString();
                    curResult.AVR_VOT_CIR_B_ANGLE = tagGH[intBw].AU_Phia_or_Phi.ToString();
                    float Ub_P = (float)(tagGH[intBw].BU_Ib_or_L1_U * tagPower.Ub * Math.Cos(tagGH[intBw].AU_Phia_or_Phi / 180F * Math.PI) / 1000F);
                    curResult.Md_Ub_ReactiveP = Ub_P.ToString(strFormat);
                    float Ub_S = (tagGH[intBw].BU_Ib_or_L1_U * tagPower.Ub / 1000F);
                    curResult.Md_Ub_ReactiveS = Ub_S.ToString(strFormat);

                    curResult.AVR_VOT_CIR_C_VOT = tagPower.Uc.ToString();
                    curResult.AVR_VOT_CIR_C_CUR = tagGH[intBw].CU_Ic_or_L2_U.ToString();
                    curResult.AVR_VOT_CIR_C_ANGLE = tagGH[intBw].AU_Phia_or_Phi.ToString();
                    float Uc_P = (float)(tagGH[intBw].CU_Ic_or_L2_U * tagPower.Uc * Math.Cos(tagGH[intBw].AU_Phia_or_Phi / 180F * Math.PI) / 1000F);
                    curResult.Md_Uc_ReactiveP = Uc_P.ToString(strFormat);
                    float Uc_S = (tagGH[intBw].CU_Ic_or_L2_U * tagPower.Uc / 1000F);
                    curResult.Md_Uc_ReactiveS = Uc_S.ToString(strFormat);

                    if (Math.Abs(Ia_S) > I_S_Limit || Math.Abs(Ib_S) > I_S_Limit || Math.Abs(Ic_S) > I_S_Limit ||
                        Math.Abs(Ua_P) > U_P_Limit || Math.Abs(Ua_S) > U_S_Limit ||
                        Math.Abs(Ub_P) > U_P_Limit || Math.Abs(Ub_S) > U_S_Limit ||
                        Math.Abs(Uc_P) > U_P_Limit || Math.Abs(Uc_S) > U_S_Limit)
                    {
                        curResult.Md_chrValue = Variable.CTG_BuHeGe;
                    }
                    else
                    {
                        curResult.Md_chrValue = Variable.CTG_HeGe;
                    }
                }

            }
        }
        /// <summary>
        /// 功耗板
        /// </summary>
        private void Verify_PowerD()
        {

            #region  --------变量声明--------

            //float _EffectiveU = 0F;
            //float _EffectiveI = 0F;
            //float _ReactiveP = 0F;
            //float _ReactiveQ = 0F;
            //float _ActiveP = 0F;
            float[] flt_PD = new float[4];
            float _EffectiveU = 0F;
            float _EffectiveI = 0F;
            float _ReactiveP = 0F;
            float _ReactiveQ = 0F;
            //float _ActiveP = 0F;
            int[] int_U = new int[3] { 1, 3, 5 };
            int[] int_I = new int[3] { 2, 4, 6 };
            bool bln_Result = true;

            MeterBasicInfo curMeter;
            MeterPower curResult;
            #endregion

            if (GlobalUnit.GetConfig(Variable.CTC_DESKTYPE, "") == "单相台")
            {
                int_U = new int[1] { 1 };
                int_I = new int[1] { 2 };
            }
            StPowerConsume _PowerCon = (StPowerConsume)CurPlan;
            

            MessageController.Instance.AddMessage("正在设置电压功耗测试参数");

            #region ---------------- 测试电压功耗 ------------------
            if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功))
            {
                Check.Require(false, "控制源输出失败");
            }
            MessageController.Instance.AddMessage("等待源稳定3秒...");
            Thread.Sleep(3000);
            MessageController.Instance.AddMessage("正在进行电压功耗测试...");
            Thread.Sleep(3000);
            string strKey = ItemKey;
            //读取电压功耗数据
            while (true)
            {
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
                if (Stop || m_CheckOver)
                {
                    break;
                }
                Thread.Sleep(1000);
                for (int intBw = 1; intBw <= this.BwCount; intBw++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(intBw - 1);
                    if (curMeter.MeterPowers == null)
                    {
                        curMeter.MeterPowers = new Dictionary<string, MeterPower>();
                    }
                    if (curMeter.YaoJianYn && bBiaoweiBz[intBw-1])//zxr
                    {
                        //挂接数据
                        if (!curMeter.MeterPowers.ContainsKey(strKey))
                        {
                            curResult = new MeterPower();
                            curResult.Md_PrjID = strKey;
                            curResult.Md_PrjName = "功耗测试" + ItemKey;
                            curResult.Md_chrValue = Variable.CTG_HeGe;
                            curMeter.MeterPowers.Add(strKey, curResult);
                        }
                        else
                        {
                            curResult = curMeter.MeterPowers[strKey];
                            if (string.IsNullOrEmpty(curResult.Md_chrValue))
                            {
                                curResult.Md_chrValue = Variable.CTG_HeGe;
                            }
                        }
                    }
                    else
                    {
                        continue;//不检表位继续下一表位
                    }
                    MessageController.Instance.AddMessage("正在进行" + intBw + "表位电压功耗测试...");
                //    Helper.EquipHelper.Instance.ReadStdInfo();
                    //读取电压功耗
                    bln_Result = true;
                    for (int i = 0; i < int_U.Length; i++)
                    {
                        Thread.Sleep(800);
                        // (int XType, ref float flt_U, ref float flt_I, ref float flt_P, ref float flt_Q, int TxID)
                        //Adapter.ComAdpater.ReadGHData(int_U[i], ref _EffectiveU, ref _EffectiveI, ref _ReactiveP,ref _ReactiveQ, 0);
                        if (!Helper.EquipHelper.Instance.ReadPowerDissipation(intBw, (byte)int_U[i], out flt_PD))
                        {
                            MessageController.Instance.AddMessage("读取" + intBw + "表位电压功耗失败！");
                        }
                        _EffectiveU = flt_PD[0];
                        _EffectiveI = flt_PD[1];
                        _ReactiveP = flt_PD[2];
                        _ReactiveQ = flt_PD[3];
                        switch (i)
                        {
                            case 0:
                                {
                                    curResult.Md_Ua_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                    curResult.Md_Ua_ReactiveP = _ReactiveP.ToString("F4");
                                }
                                break;
                            case 1:
                                {
                                    curResult.Md_Ub_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                    curResult.Md_Ub_ReactiveP = _ReactiveP.ToString("F4");
                                }
                                break;
                            case 2:
                                {
                                    curResult.Md_Uc_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                    curResult.Md_Uc_ReactiveP = _ReactiveP.ToString("F4");
                                }
                                break;
                            default:
                                break;
                        }


                        if ((_EffectiveU * _EffectiveI) > U_S_Limit || Math.Abs(_ReactiveP) > U_P_Limit)
                        {
                            bln_Result = false;

                        }
                        if (bln_Result)
                        {

                        }
                        else
                        {
                            curResult.Md_chrValue = Variable.CTG_BuHeGe;
                        }
                        if (i == int_U.Length - 1)
                        {
                            m_CheckOver = true;
                        }
                    }
                }
            }
            #endregion
            Helper.EquipHelper.Instance.PowerOff();
            Thread.Sleep(2000);
         //   Helper.EquipHelper.Instance.ReadStdInfo();
            MessageController.Instance.AddMessage("正在设置电流功耗测试参数");
            #region  --------------  测试电流功耗  ------------------
            if (!Helper.EquipHelper.Instance.PowerOn(0, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.正向有功, "1.0", true, false))
            {
                Check.Require(false, "控制源输出失败");
            }
            MessageController.Instance.AddMessage("等待源稳定5秒...");
            Thread.Sleep(5000);
         //   Helper.EquipHelper.Instance.ReadStdInfo();
            m_CheckOver = false;
            MessageController.Instance.AddMessage("正在进行电流功耗测试...");
            Thread.Sleep(5000);
            while (true)
            {
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
                if (Stop || m_CheckOver)
                {
                    break;
                }
                Thread.Sleep(1000);
                for (int intBw = 1; intBw <= this.BwCount; intBw++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(intBw - 1);
                    if (curMeter.MeterPowers == null)
                    {
                        curMeter.MeterPowers = new Dictionary<string, MeterPower>();
                    }
                    if (curMeter.YaoJianYn && bBiaoweiBz[intBw - 1])
                    {
                        //挂接数据
                        if (!curMeter.MeterPowers.ContainsKey(strKey))
                        {
                            curResult = new MeterPower();
                            curResult.Md_PrjID = strKey;
                            curResult.Md_PrjName = "功耗测试" + ItemKey;
                            curMeter.MeterPowers.Add(strKey, curResult);
                        }
                        else
                        {
                            curResult = curMeter.MeterPowers[strKey];
                            if (string.IsNullOrEmpty(curResult.Md_chrValue))
                            {
                                curResult.Md_chrValue = Variable.CTG_HeGe;
                            }
                        }
                    }
                    else
                    {
                        continue;//不检表位继续下一表位
                    }

                    MessageController.Instance.AddMessage("正在进行" + intBw + "表位电流功耗测试...");
                //    Helper.EquipHelper.Instance.ReadStdInfo();
                    //读取电流功耗
                    bln_Result = true;
                    for (int i = 0; i < int_I.Length; i++)
                    {
                        Thread.Sleep(800);
                        //Adapter.ComAdpater.ReadGHData(int_I[i], ref _EffectiveU, ref _EffectiveI, ref _ReactiveP,ref _ReactiveQ, 0);
                        if (!Helper.EquipHelper.Instance.ReadPowerDissipation(intBw, (byte)int_I[i], out flt_PD))
                        {
                            MessageController.Instance.AddMessage("读取" + intBw + "表位电流功耗失败！");
                        }
                        _EffectiveU = flt_PD[0];
                        _EffectiveI = flt_PD[1];
                        _ReactiveP = flt_PD[2];
                        _ReactiveQ = flt_PD[3];

                        switch (i)
                        {
                            case 0:
                                {
                                    curResult.Md_Ia_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                }
                                break;
                            case 1:
                                {
                                    curResult.Md_Ib_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                }
                                break;
                            case 2:
                                {
                                    curResult.Md_Ic_ReactiveS = (_EffectiveU * _EffectiveI).ToString("F4");
                                }
                                break;
                            default:
                                break;
                        }
                        if ((_EffectiveU * _EffectiveI) > I_S_Limit)
                        {
                            bln_Result = false;
                        }
                        if (bln_Result)
                        {

                        }
                        else
                        {
                            curResult.Md_chrValue = Variable.CTG_BuHeGe;
                        }
                        if (i == int_I.Length - 1)
                        {
                            m_CheckOver = true;
                        }
                    }
                }
            }
            #endregion;
        }

        #endregion
    }
}
