
using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_VerifyAdapter.MulitThread;

namespace CLDC_VerifyAdapter.Multicore
{
    /// <summary>
    /// 剩余电量递减准确度
    /// </summary>
    class LeftEnergyAccuracy : VerifyBase 
    {
        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        public LeftEnergyAccuracy(object plan)
            : base(plan)
        {
        }

        //开始电量
        float[] arrayStartEnergy = new float[Adapter.Instance.BwCount];
        //结束电量
        float[] arrayEndEnergy = new float[Adapter.Instance.BwCount];
        //价格
        float[] arrayPrice = new float[Adapter.Instance.BwCount];
        //总结论
        bool[] arrayResult = new bool[Adapter.Instance.BwCount];
        //结束时剩余金额
        float[] arrayLeftMoneyEnd = new float[Adapter.Instance.BwCount];
        //开始时剩余金额
        float[] arrayLeftMoneyStart = new float[Adapter.Instance.BwCount];
        //误差值
        float[] arrayError = new float[Adapter.Instance.BwCount];
        string[] arrayDisableReasion = new string[Adapter.Instance.BwCount];
        /// <summary>
        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            RefreshResult(false);
            #region 初始化
            base.Verify();
            if (Stop) return;                   //假如当前停止检定，则退出
            MeterBasicInfo curMeter;
         //   string keyitem = ((int)CLDC_Comm.Enum.Cus_CostControlItem.剩余电量递减准确度).ToString().PadLeft(3, '0');
            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMeter.YaoJianYn)
                {
                    //初始化结论为合格
                    arrayResult[i] = true;
                 //   strResultKey[i] = ItemKey;
                }
            }
            if (!PowerOn())
            {
                //GlobalUnit.g_MsgControl.OutMessage("升电压失败! ");
                //return;
            }
            #endregion
            try
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                //获取所有表的表地址
                ReadMeterAddrAndMeterNo();

                RefreshResult(false);

                #region //切换费率,以免检定时发生费率切换
                bool[] resultChangePeriod = InitialPeriod(out arrayPrice);

                for (int i = 0; i < BwCount; i++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (curMeter.YaoJianYn)
                    {
                        if (!resultChangePeriod[i])
                        {
                            arrayResult[i] = false;
                            arrayDisableReasion[i] = "切换表费率时段失败!!!";
                        }
                    }
                }
                #endregion

                #region 读取开始时数据
                if (Stop)
                {
                    return;
                }
                GlobalUnit.g_MsgControl.OutMessage("读取开始时的剩余金额...", false);
                arrayLeftMoneyStart = MeterProtocolAdapter.Instance.ReadData("00900200", 4, 2);
                GlobalUnit.g_MsgControl.OutMessage("读取开始时的总电量...", false);
                arrayStartEnergy = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
                RefreshResult(false);
                #endregion

                #region 计算并等待:2度电
                MeterBasicInfo firstMeter = Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter);
                float currentImax = CLDC_DataCore.Function.Number.GetCurrentByIb("Imax", firstMeter.Mb_chrIb, firstMeter.Mb_BlnHgq);
                float currentPower = base.CalculatePower(CLDC_DataCore.Const.GlobalUnit.U, currentImax, CLDC_DataCore.Const.GlobalUnit.Clfs, Cus_PowerYuanJian.H, "1.0", true);
                float totalTime = ((float)3600 * 1000 * 2) / currentPower;

                if (Stop)
                {
                    return;
                }
                Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U
                                                          , currentImax
                                                          , (int)Cus_PowerYuanJian.H
                                                          , (int)Cus_PowerFangXiang.正向有功
                                                          , "1", true, false);
                DateTime timeStart = DateTime.Now;
                DateTime timeEnd = timeStart.AddSeconds(totalTime);

                while (DateTime.Now < timeEnd)
                {
                    if (Stop)
                    {
                        return;
                    }
                    System.Threading.Thread.Sleep(5000);
                    int secondCount = (int)((timeEnd - DateTime.Now).TotalSeconds);
                    GlobalUnit.g_MsgControl.OutMessage(string.Format("等待电表走2度电,剩余时间:{0}秒", secondCount), false);
                }
                Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
                #endregion

                #region 读取结束时数据
                if (Stop)
                {
                    return;
                }
                GlobalUnit.g_MsgControl.OutMessage("读取结束时的剩余金额...", false);
                arrayLeftMoneyEnd = MeterProtocolAdapter.Instance.ReadData("00900200", 4, 2);
                GlobalUnit.g_MsgControl.OutMessage("读取结束时的总电量...", false);
                arrayEndEnergy = MeterProtocolAdapter.Instance.ReadEnergy((byte)0, (byte)0);
                #endregion

                #region 计算误差,判断结论
                float[] arrayMoneyUsed = new float[BwCount];
                float[] arrayEnergyMoney = new float[BwCount];
                for (int i = 0; i < BwCount; i++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (curMeter.YaoJianYn)
                    {
                        arrayMoneyUsed[i] = arrayLeftMoneyStart[i] - arrayLeftMoneyEnd[i];
                        arrayEnergyMoney[i] = (arrayEndEnergy[i] - arrayStartEnergy[i]) * arrayPrice[i];
                        arrayError[i] = (arrayMoneyUsed[i] - arrayEnergyMoney[i]);// / arrayEnergyMoney[i];
                    }
                }
                RefreshResult(true);
                #endregion

             
            }
            finally
            {
              

                DateTime readTime = DateTime.Now;
             //   if (GlobalUnit.g_SystemConfig.methodAndBasis.getValue(Variable.CTC_TM_GPSGETT) != "取电脑时间")
                {
                    GlobalUnit.g_MsgControl.OutMessage("开始读取GPS时间...", false);
                    readTime = Helper.EquipHelper.Instance.ReadGpsTime();  //读取GPS时间
                }
                GlobalUnit.g_MsgControl.OutMessage("开始恢复表时间......", false);
                bool[] result = MeterProtocolAdapter.Instance.WriteDateTime(readTime.ToString("yyMMddHHmmss"));
            }
        }

        /// <summary>
        /// 更新结论
        /// </summary>
        private void RefreshResult(bool flagFinished)
        {
            //string keyTemp = ((int)CLDC_Comm.Enum.Cus_CostControlItem.剩余电量递减准确度).ToString().PadLeft(3, '0');
            //for (int i = 0; i < BwCount; i++)
            //{
            //    MeterBasicInfo meterTemp = Helper.MeterDataHelper.Instance.Meter(i);
            //    if (meterTemp.YaoJianYn)
            //    {
            //        MeterFK meterFk = null;
            //        if (meterTemp.MeterCostControls.ContainsKey(keyTemp))
            //        {
            //            meterFk = meterTemp.MeterCostControls[keyTemp];
            //        }
            //        else
            //        {
            //            meterFk = new MeterFK()
            //            {
            //                Mcc_PrjName = CLDC_Comm.Enum.Cus_CostControlItem.剩余电量递减准确度.ToString(),
            //                Mfk_chrItemType = keyTemp,
            //                _intMyId = meterTemp._intMyId,
            //            };
            //            meterTemp.MeterCostControls.Add(keyTemp, meterFk);
            //        }
            //        //当前电价|开始剩余金额|开始总电量|结束剩余金额|结束总电量|误差
            //        meterFk.Mfk_chrData = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", GetFloatString(arrayPrice[i]), GetFloatString(arrayLeftMoneyStart[i]), GetFloatString(arrayStartEnergy[i]), GetFloatString(arrayLeftMoneyEnd[i]), GetFloatString(arrayEndEnergy[i]), GetFloatString(arrayError[i]));
            //        meterFk.AVR_DIS_REASON = arrayDisableReasion[i];
            //        if (flagFinished)
            //        {
            //            meterFk.Mfk_chrJL = Math.Abs(arrayError[i]) <= 0.01 ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            //            if (meterFk.Mfk_chrJL == Variable.CTG_HeGe)
            //            {
            //                meterFk.AVR_DIS_REASON = "";
            //            }
            //        }
            //    }
            //}
            //GlobalUnit.g_MsgControl.OutMessage();
        }

        private bool[] InitialPeriod(out float[] arrayPrice)
        {
            bool[] resultTemp = new bool[BwCount];
            arrayPrice = new float[BwCount];

            if (Stop)
            {
                return resultTemp;
            }
            GlobalUnit.g_MsgControl.OutMessage("开始读取电表第一个费率时段...", false);
            string[] strReadData = MeterProtocolAdapter.Instance.ReadData("04010001", 42);
            int firstIndex = Helper.MeterDataHelper.Instance.FirstYaoJianMeter;

            int indexTemp = 0;
            string s1 = "";
            while (indexTemp < 7)
            {

                s1 = strReadData[firstIndex].Substring(6 * indexTemp, 6);
                if (s1.EndsWith("03"))
                {
                    break;
                }
                indexTemp++;
            }
            //if (s1.EndsWith("01"))
            {

                string strTemp = s1.Substring(0, 2) + ":" + s1.Substring(2, 2);
                string sdTime = DateTime.Parse(strTemp).ToString("yyMMddHHmmss");
                if (Stop)
                {
                    return resultTemp;
                }

              //  Identity();

                GlobalUnit.g_MsgControl.OutMessage("开始设置电表时间为第一个费率时段...", false);

                resultTemp = MeterProtocolAdapter.Instance.WriteDateTime(sdTime);
                ShowWaitMessage("等待{0}秒至电表时间切换完成...", 15000);
                if (Stop)
                {
                    return resultTemp;
                }
                GlobalUnit.g_MsgControl.OutMessage("开始读取电表当前电价...", false);
           //     string priceDI = GlobalUnit.g_SystemConfig.methodAndBasis.getValue(Variable.CTC_TM_LEFTRIGHTPRICE);
            //    if (string.IsNullOrEmpty(priceDI) || priceDI.Length < 12)
               // {
             //      priceDI = "02800020,4,4";
            //    }
          //     string[] PDI = priceDI.Split(',');
           //     arrayPrice = MeterProtocolAdapter.Instance.ReadData(PDI[0], Convert.ToInt32(PDI[1]), Convert.ToInt32(PDI[2]));//"04050103"
            }

            return resultTemp;
        }
        private string GetFloatString(float valueTemp)
        {
            if (valueTemp == 0)
            {
                return "";
            }
            else
            {
                return valueTemp.ToString("0.00");
            }
        }
    }
}
