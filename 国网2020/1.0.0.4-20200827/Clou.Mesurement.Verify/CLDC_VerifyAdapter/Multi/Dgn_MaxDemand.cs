
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;


namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 多功能_最大需量试验
    /// </summary>
    class Dgn_MaxDemand : DgnBase
    {

        public Dgn_MaxDemand(object plan)
            : base(plan)
        {
        }

        /// <summary>
        /// 电流倍数
        /// </summary>
        public float XIB { get; set; }


        /// <summary>
        /// 检定
        /// </summary>
        public override void Verify()
        {
            base.Verify();    //
            bool bDoThisPowerFangXiang = false; //是否要做当前方向
            int demandPeriod = 15;               //需量周期
            int slipTimes = 1;                 //滑差时间
            int slipPage = 3;                   //滑差次数
            int.TryParse(PrjPara[0], out demandPeriod);
            int.TryParse(PrjPara[1], out slipTimes);
            int.TryParse(PrjPara[2], out slipPage);
            int maxMinute = demandPeriod + slipTimes * slipPage;
            int maxTime = maxMinute * 60;
            string[] arrStrResultKey = new string[BwCount];

            //清理甩的方向数据
            //ClearAllItemData();
            for (int pq = 0; pq < 4; pq++)
            {
                if (Stop || m_CheckOver) break;

                bDoThisPowerFangXiang = (PrjPara[4].Substring(pq, 1) == "1");
                if (!bDoThisPowerFangXiang) continue;

                Cus_PowerFangXiang CurFangXiang = (Cus_PowerFangXiang)(pq + 1);

                bool bPowerOn = PowerOn();              //先按方案升源，把表点亮后清需量
                Check.Require(bPowerOn, "控制源输出失败！");
                ShowWirteMeterWwaring();
                //第一步：清空需量
                bool[] clearDemand = MeterProtocolAdapter.Instance.ClearDemand();

                MessageController.Instance.AddMessage("开始做最大需量");
                //先把源关掉
                base.PowerFangXiang = (Cus_PowerFangXiang)(pq + 1);
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * XIB, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, FangXiangStr + "1.0", IsYouGong, false) == false)
                {
                    Stop = true;
                    MessageController.Instance.AddMessage("开始做最大需量");

                    return;
                }
                //同时做需量周期脉冲
                if (PrjPara[3] == "1" && pq == 0)
                {
                    
                    //电能表脉冲切换到需量周期
                    MeterProtocolAdapter.Instance.SetPulseCom(1);
                    MeterProtocolAdapter.Instance.SetPulseCom(1);
                    Helper.EquipHelper.Instance.InitPara_InitDemandPeriod(demandPeriod, slipPage);
                    if (!Helper.EquipHelper.Instance.InitPara_InitDemandPeriod(demandPeriod, slipPage))
                    {
                        return;
                    }
                }
                m_StartTime = DateTime.Now;
                int PastMinute = 0;
                float[] meterXL = new float[BwCount];    //被检表最大需量
                float standMeterP = 0;
                string strcurKey = string.Format("{0}{1}1", ItemKey, (int)PowerFangXiang);
                float[] arrDemandValue = new float[0];
                //标准表功率
                while (true)
                {
                    if (Stop || m_CheckOver) return;
                    Thread.Sleep(2000);
                    GlobalUnit.g_MsgControl.ClearCache();

                    int pastTime = (int)VerifyPassTime;
                    int tempMinute = (int)(VerifyPassTime / 55);
                    if (VerifyPassTime >= maxTime)
                    {
                        MessageController.Instance.AddMessage("检定时间达到方案预定时间，检定完成");
                        break;
                    }
                    else
                    {
                        pastTime *= 100;
                        pastTime /= 60;
                        float curPorcess = pastTime / 100F;
                        GlobalUnit.g_CUS.DnbData.NowMinute = curPorcess;
                        MessageController.Instance.AddMessage(string.Format("{0}{1}检定需要{2}分，已经进行{3}分)", PowerFangXiang.ToString(), CurPlan.ToString(), maxMinute, curPorcess));
                    }
                    float curStandMeterP = GlobalUnit.g_StrandMeterP[0];
                    //有功/无功功率
                    if (!IsYouGong)
                    {
                        curStandMeterP = GlobalUnit.g_StrandMeterP[1];
                    }
                    standMeterP = Math.Abs(curStandMeterP);
                    //每一次分钟读取一次需量数据
                    if (tempMinute > PastMinute)
                    {
                        PastMinute = tempMinute;
                        byte curPD = (byte)((byte)CurFangXiang - 1);
                        arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand(curPD, 0x00);
                        for (int i = 0; i < BwCount; i++)
                        {
                            if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                            {
                                float readXl = arrDemandValue[i] * 1000;
                                if (meterXL[i] < readXl)
                                    meterXL[i] = readXl;    //记录下需量
                                if (Helper.MeterDataHelper.Instance.Meter(i).MeterDgns.ContainsKey(strcurKey))
                                {
                                    MeterDgn dataPQ = Helper.MeterDataHelper.Instance.Meter(i).MeterDgns[strcurKey];
                                    dataPQ.Md_chrValue = string.Format("{0}|{1}|---", standMeterP, meterXL[i]);
                                }
                            }
                            
                        }
                    }
                    //如果要做需量周期误差，则需要读取误差板
                    //if (PrjPara[3] == "1" && pq == 0)
                    //{ 
                        //

                    // bool ret=   ReadData(
                    //}
                    
                }
                //再读取一次需量
                arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(CurFangXiang - 1), 0x00);
                bool bResult = true;
                for (int j = 0; j < BwCount; j++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn)
                    {
                        if (arrDemandValue[j] == 0)
                        {
                            bResult = false;
                            break;
                        }
                    }
                }
                if(!bResult)
                    arrDemandValue = MeterProtocolAdapter.Instance.ReadDemand((byte)(CurFangXiang - 1), 0x00);
                //PowerOn();
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        float readXl = arrDemandValue[i] * 1000;
                        if (meterXL[i] < readXl)
                            meterXL[i] = readXl;    //记录下需量
                        if (Helper.MeterDataHelper.Instance.Meter(i).MeterDgns.ContainsKey(strcurKey))
                        {
                            MeterDgn dataPQ = Helper.MeterDataHelper.Instance.Meter(i).MeterDgns[strcurKey];
                            dataPQ.Md_chrValue = string.Format("{0}|{1}|---", standMeterP, meterXL[i]);
                        }

                    }
                    arrStrResultKey[i] = ItemKey;
                    
                }
                
                if (PrjPara[3] == "1" && pq == 0)
                {
                    //处理需量周期误差
                    ControlXLZQError();
                }
                //计算误差 (被检表最大需量-标准表最大功率)/标准表最大功率
                string strKey = string.Empty;                  //项目结论
                string strItemKey;              //项目数据    
                Cus_DgnItem curItem;
                if (XIB == 0.1F)
                {
                    curItem = Cus_DgnItem.最大需量01Ib;
                }
                else if (XIB == 1F)
                {
                    curItem = Cus_DgnItem.最大需量10Ib;
                }
                else
                {
                    curItem = Cus_DgnItem.最大需量Imax;
                }
                strKey = string.Format("{0}{1}", ItemKey, (int)PowerFangXiang);         //结论
                strItemKey = string.Format("{0}1", strKey); //数据
                for (int k = 0; k < BwCount; k++)
                {
                    MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(k);

                    MeterDgn curResult;         //数据
                    MeterDgn curItemResult;     //结论
                    MeterDgn curPDResult;
                    if (!curMeter.YaoJianYn) continue;

                    //挂数据

                    #region 查询当前方向总结论节点
                    if (!curMeter.MeterDgns.ContainsKey(strKey))
                    {
                        curResult = new MeterDgn();
                        curMeter.MeterDgns.Add(strKey, curResult);
                        curResult.Md_PrjID = strKey;
                        curResult.Md_PrjName = curItem.ToString();
                        //curMeter.MeterDgns.Add(strKey, curResult);
                    }
                    else
                    {
                        curResult = curMeter.MeterDgns[strKey];
                    }
                    #endregion

                    #region 查询当前方向数据结点
                    if (!curMeter.MeterDgns.ContainsKey(strItemKey))
                    {
                        curItemResult = new MeterDgn();
                        curMeter.MeterDgns.Add(strItemKey, curResult);
                        curItemResult.Md_PrjID = strItemKey;
                        curItemResult.Md_PrjName = curItem.ToString();
                        //curMeter.MeterDgns.Add(strKey, curItemResult);
                    }
                    else
                    {
                        curItemResult = curMeter.MeterDgns[strItemKey];
                    }
                    #endregion

                    #region 查询当前项目总结论节点
                    if (!curMeter.MeterDgns.ContainsKey(ItemKey))
                    {
                        curPDResult = new MeterDgn();
                        curMeter.MeterDgns.Add(ItemKey, curPDResult);
                        curPDResult.Md_PrjID = ItemKey;
                        curPDResult.Md_PrjName = curItem.ToString();
                        //curMeter.MeterDgns.Add(strKey, curResult);
                    }
                    else
                    {
                        curPDResult = curMeter.MeterDgns[ItemKey];
                    }
                    #endregion


                    float curLevel = Number.SplitKF(Helper.MeterDataHelper.Instance.Meter(k).Mb_chrBdj, IsYouGong);
                    float errorLevel = GetErrorLevel(curLevel);

                    CLDC_DataCore.Struct.StWuChaDeal _WuChaPara = new CLDC_DataCore.Struct.StWuChaDeal();
                    _WuChaPara.IsBiaoZunBiao = false;
                    _WuChaPara.MeterLevel = curLevel;

                    _WuChaPara.MaxError = errorLevel;
                    _WuChaPara.MinError = -errorLevel;

                    m_WuChaContext = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.多功能_需量误差, _WuChaPara);
                    m_WuChaContext.OtherData = standMeterP.ToString();  //通过OtherData传入当前负载点标准功率
                    MeterDgn result = (MeterDgn)m_WuChaContext.GetResult(meterXL[k]);
                    curResult.Md_chrValue = m_WuChaContext.OtherData;
                    //OtherData保存结论
                    curItemResult.Md_chrValue = result.Md_chrValue;
                    //总结论
                    if (curResult.Md_chrValue == Variable.CTG_BuHeGe)
                    {
                        //当前方向如果不合格，直接标记
                        curPDResult.Md_chrValue = curResult.Md_chrValue;
                    }
                    else
                    {
                        //查询其它几个方向是否合格
                        string strTmpKey = string.Empty;
                        bool tmpResult = true;
                        for (int pr = 1; pr < 5; pr++)
                        {
                            strTmpKey = string.Format("{0}{1}", ItemKey, pr);
                            if (curMeter.MeterDgns.ContainsKey(strTmpKey))
                            {
                                tmpResult = curMeter.MeterDgns[strTmpKey].Md_chrValue ==
                                    Variable.CTG_HeGe;
                                if (!tmpResult)
                                    break;
                            }
                        }
                        curPDResult.Md_chrValue = CLDC_DataCore.Function.Common.ConverResult(tmpResult);
                    }
                    
                }
            }
            
         //   GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }
        /// <summary>
        /// 根据不同检定项目检测参数是否合法
        /// </summary>
        /// <returns>T/F</returns>
        protected override bool CheckPara()
        {
            string[] para = PrjPara;
            Check.Require(para.Length == 5, "最大需量方案参数个数不正确");
            bool isNumericPara =
                Number.IsNumeric(para[0]) &&
                Number.IsNumeric(para[1]) &&
                Number.IsNumeric(para[2]);
            Check.Require(isNumericPara, "最大需量方案值应该都为数字");
            return true;
        }

        /// <summary>
        /// 清理数据
        /// </summary>
        protected override void ClearItemData()
        {
            ClearAllItemData();
            //string strKey = string.Empty;
            //MeterBasicInfo curMeter = new MeterBasicInfo();
            //string powerFangXiangKey = ((int)base.PowerFangXiang).ToString();
            //ClearItemData_One(powerFangXiangKey);
        }
        /// <summary>
        /// 清理所有方向的数据
        /// </summary>
        private void ClearAllItemData()
        {

            for (int i = 1; i < 5; i++)
            {
                ClearItemData_One(i.ToString());
                
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// 清理一个方向的检定数据
        /// </summary>
        /// <param name="strFangXiang"></param>
        private void ClearItemData_One(string strFangXiang)
        {
            string strKey = string.Empty;
            MeterBasicInfo curMeter = new MeterBasicInfo();
            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                //清理分方向结论
                strKey = string.Format("{0}{1}", ItemKey, strFangXiang);
                if (curMeter.MeterDgns.ContainsKey(strKey))
                    curMeter.MeterDgns.Remove(strKey);
                //清理分方向数据
                strKey += "1";
                if (curMeter.MeterDgns.ContainsKey(strKey))
                    curMeter.MeterDgns.Remove(strKey);
                //清理周期误差
                strKey = ItemKey + "02";
                if (curMeter.MeterDgns.ContainsKey(strKey))
                    curMeter.MeterDgns.Remove(strKey);

                

            }
            
            
            Thread.Sleep(100);
        }
        /// <summary>
        /// 挂默认数据
        /// </summary>
        protected override void DefaultItemData()
        {
            string strKey = string.Empty;
            MeterBasicInfo curMeter = new MeterBasicInfo();
            MeterDgn resultPQ = null;       //分方向结论
            MeterDgn dataPQ = null;         //分方向数据
            //MeterDgn dataZQ = null;         //周期误差
            //for (int i = 1; i < 5; i++)
            //{
            string powerFangXiangKey = ((int)base.PowerFangXiang).ToString();

            for (int j = 0; j < BwCount; j++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                //清理分方向结论
                strKey = string.Format("{0}{1}", ItemKey, powerFangXiangKey);
                if (!curMeter.MeterDgns.ContainsKey(strKey))
                {
                    resultPQ = new MeterDgn();
                    resultPQ.Md_PrjID = strKey;
                    resultPQ.Md_PrjName = PowerFangXiang.ToString();
                    curMeter.MeterDgns.Add(strKey, resultPQ);
                }
                //清理分方向数据
                strKey += "1";
                if (!curMeter.MeterDgns.ContainsKey(strKey))
                {
                    dataPQ = new MeterDgn();
                    dataPQ.Md_PrjID = strKey;
                    dataPQ.Md_PrjName = PowerFangXiang.ToString() + "数据";
                    dataPQ.Md_chrValue = "60秒后出数据|60秒后出数据|60秒后出数据";  //挂默认数据
                    curMeter.MeterDgns.Add(strKey, dataPQ);
                }

                //清理周期误差
                //strKey = string.Format("{0}02", ItemKey);
                //if (!curMeter.MeterDgns.ContainsKey(strKey))
                //{
                //    dataZQ = new MeterDgn();
                //    dataZQ.Md_PrjID = strKey;
                //    dataZQ.Md_PrjName =  "周期误差";
                //    dataZQ.Md_chrValue = "||";
                //}
                //}
            }
            base.DefaultItemData();
        }

        /// <summary>
        /// 处理需量周期误差
        /// </summary>
        private void ControlXLZQError()
        {
            string[] str1 = new string[0];
            int[] iNumber = new int[0];
            bool Result = ReadData(ref str1, ref iNumber, 1F);

            if (!Result)
            {
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(2000);
                    if (ReadData(ref str1, ref iNumber, 1F))
                    {
                        Result = true;
                        break;
                    }
                }
            }
            if (!Result)
            {
                MessageController.Instance.AddMessage("读取需量周期误差失败！", 6, 2);
                Thread.Sleep(200);
            }
            //处理误差
            MessageController.Instance.AddMessage("开始处理需量周期误差");
            MeterBasicInfo curMeter = null;
            string strKey = string.Format("{0}02", ItemKey);
            MeterDgn dataZQ = null;
            bool result = false;
            float wc = 0F;
            for (int k = 0; k < BwCount; k++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(k);
                if (!curMeter.YaoJianYn)
                    continue;
                if (!curMeter.MeterDgns.ContainsKey(strKey))
                {
                    dataZQ = new MeterDgn();
                    dataZQ.Md_PrjID = strKey;
                    dataZQ.Md_PrjName = CurPlan.ToString() + "周期误差";
                    curMeter.MeterDgns.Add(strKey, dataZQ);
                }
                else
                {
                    dataZQ = curMeter.MeterDgns[strKey];
                }
                if (str1[k] != null && str1[k] != "")
                {
                    wc = float.Parse(str1[k]);
                    result = (Math.Abs(wc) < 0.6F);   //参见V80 TestMaxXL
                }
                else
                {
                    result = false;
                }
                
                dataZQ.Md_chrValue = string.Format("{0}|{1}|{2}", PrjPara[0], str1[k], CLDC_DataCore.Function.Common.ConverResult(result));
            }
            MessageController.Instance.AddMessage("处理需量周期误差完毕");
        }

        /// <summary>
        /// 根据南网标准计算需量示值误差限
        /// </summary>
        /// <param name="meterLevel">电能表等级</param>
        /// <returns></returns>
        private float GetErrorLevel(float meterLevel)
        {
            //海南计算公式:表等级+0.05*额定功率/实际功率
            //标准功率
            float strandPower = CalculatePower(GlobalUnit.U, GlobalUnit.Ib, GlobalUnit.Clfs);
            //负载功率
            float current = GlobalUnit.Imax;
            if (XIB == 0.1F) current = GlobalUnit.Ib * XIB;
            if (XIB == 1F) current = GlobalUnit.Ib;
            float currentPower = CalculatePower(GlobalUnit.U, current, GlobalUnit.Clfs);
            return meterLevel + 0.05F * strandPower / currentPower;
        }
    }
}
