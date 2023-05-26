using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.SystemModel.Item; //字典
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using System.Data;
using CLDC_VerifyAdapter.VerifyService;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 误差一致性检定控制器
    /// </summary>
    class ErrorAccord : VerifyBase
    {
        #region ----------变量声明----------

        /// <summary>
        /// 每一个误差点取几次误差参与计算
        /// </summary>
        private int m_WCTimesPerPoint;
        /// <summary>
        /// 标准偏差点取几次误差参与计算
        /// </summary>
        private int m_WindageTimesPerPoint;
        /// <summary>
        /// 每一个误差点最多读取多少次误差
        /// </summary>
        private int m_WCMaxTimes;
        /// <summary>
        /// 每一个误差点最多读多少秒
        /// </summary>
        private int m_WCMaxSeconds;
        /// <summary>
        /// 跳差判定标准
        /// </summary>
        private float m_WCJump;

        /// <summary>
        ///标准脉冲分频系数
        /// </summary>
        private float m_DriverF;

        #endregion

        #region ----------构造函数----------
        public ErrorAccord(object plan)
            : base(plan)
        {
            //检测检定参数是否正确
            // m_VerifyDemo.BWCount = BwCount;
            ResultNames = new string[] { "误差1","误差2","平均值","化整值","样品均值","差值","结论" };
        }
        #endregion

        #region 基类方法重写

        /// <summary>
        /// 重写方案
        /// </summary>
        private new StPlan_WcPoint CurPlan
        {
            get
            {
                return (StPlan_WcPoint)base.CurPlan;
            }
            set
            {
                base.CurPlan = value;
            }
        }

        /// <summary>
        /// 总结论ID
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                if (CurPlan.Pc == 1)
                    return string.Format("{0}", (int)Cus_MeterResultPrjID.标准偏差);
                else
                    return string.Format("{0}", (int)Cus_MeterResultPrjID.基本误差试验);
            }
        }
        /// <summary>
        /// 基本误差Key值:PrjID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return VerifyProcess.Instance.CurrentKey;
            }
        }

        #endregion

        /// <summary>
        /// 基本误差和标准偏差误差检定
        /// </summary>
        public override void Verify()
        {

            #region ----------变量声明----------
            GlobalUnit.g_CurTestType = 0;
            float meterLevel = 2;                                                           //表等级变量 
            MeterBasicInfo meterInfo;                                                       //电表数据
            StPlan_WcPoint curPoint = CurPlan;                                      //方案转换
            base.Verify();
            PowerOn();
            string[] verPlan = VerifyPara.Split('|');
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("开始预热，请等待" + TIME + "秒");
                if (Stop) return;
              

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * TIME);

            }//调用基类的检定方法，执行默认操作
            string[] arrCurWCValue = new string[BwCount];                                   //当前误差
            int[] arrCurWCTimes = new int[BwCount];                                         //误差次数
            string dataValueKey = ItemKey;                                                  //检定数据节点名称:P_检定ID
            int tableHeader = 2;// curPoint.Pc == 0 ? m_WCTimesPerPoint : m_WindageTimesPerPoint;  //每个点合格误差次数
            DataTable errorTable = new DataTable();                                         //误差值虚表
            StPlan_WcPoint[] arrPlanList = new StPlan_WcPoint[BwCount];                             //保存表方案
            int[] arrPulseLap = new int[BwCount];                                           //检定圈数
            //DateTime thisPointStartTime = DateTime.Now;                                     //记录检定开始时间
            int[] arrVerifyTimes = new int[BwCount];                                        //有效误差次数,当前表位已经做了多少次有效误差
            int[] arrCurrentWcNum = new int[BwCount];                                       //当前累计检定次数,是指误差板从启动开始到目前共产生了多少次误差
            int[] arrMeterWcNum = new int[BwCount];                                         //表位取误差次数
            bool[] arrCheckOver = new bool[0];                                              //表位完成记录
            //上报检定参数
            string[] arrStrResultKey = new string[BwCount];
            object[] arrObjResultValue = new object[BwCount];
            MeterError tmpError = null;
            MeterError curError = null;
            string[] tmpErrorConc = new string[BwCount];
            #endregion

            Helper.LogHelper.Instance.WriteInfo("初始化基本误差检定参数...");
            //初始化参数,带200MS延时
            InitVerifyPara(tableHeader, ref arrPlanList, ref arrPulseLap, errorTable);
            #region 上传误差参数
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
            //    {
            //        //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
            //        ResultDictionary["功率元件"][i] = arrPlanList[i].PowerYuanJian.ToString();
            //        ResultDictionary["功率方向"][i] = arrPlanList[i].PowerFangXiang.ToString();
            //        ResultDictionary["电流倍数"][i] = arrPlanList[i].PowerDianLiu;
            //        ResultDictionary["功率因数"][i] = arrPlanList[i].PowerYinSu;
            //        ResultDictionary["误差下限"][i] = arrPlanList[i].ErrorXiaXian.ToString();
            //        ResultDictionary["误差上限"][i] = arrPlanList[i].ErrorShangXian.ToString();
            //        ResultDictionary["误差圈数"][i] = arrPlanList[i].LapCount.ToString();
            //    }
            //}
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "功率元件", ResultDictionary["功率元件"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "功率方向", ResultDictionary["功率方向"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电流倍数", ResultDictionary["电流倍数"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "功率因数", ResultDictionary["功率因数"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差下限", ResultDictionary["误差下限"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差上限", ResultDictionary["误差上限"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差圈数", ResultDictionary["误差圈数"]);
            #endregion
            /*当前是否已经停止校验*/
            if (Stop) return;
            
            int maxWCnum = tableHeader;                         //最大误差次数
            meterInfo = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);
            Helper.LogHelper.Instance.WriteInfo("初始化设备参数...");
            //初始化设备
            bool resultInitEquip = InitEquipment(curPoint, float.Parse(meterInfo.Mb_chrHz), arrPulseLap);
            if (!resultInitEquip)
            {
                MessageController.Instance.AddMessage("初始化基本误差设备参数失败", 6, 2);
                //CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState = Cus_CheckStaute.错误;
                //Stop = true;
                //return;
            }
            if (Stop)
            {
                return;
            }
      //      Helper.VerifyDemoHelper.Instance.Reset();
            m_StartTime = DateTime.Now;                                             //记录下检定开始时间
            arrCheckOver = new bool[BwCount];                                       //是否已经完成本次检定
            int[] PulseClone = (int[])arrPulseLap.Clone();
            Number.PopDesc(ref PulseClone, true);                      //排序
            int sleepTime = PulseClone[0] * GetOneErrorTime();   //计算一个脉冲大概需要的时间 毫秒
            m_WCMaxSeconds = (int)(sleepTime * 13 * maxWCnum / 1000F + 10);
            if (sleepTime > 10000)
            {
                sleepTime = 5000;
            }       //避免等待时间过长，5秒钟读取一次误差
            Helper.LogHelper.Instance.WriteInfo("开始检定");
            //开始检定
            Common.Memset(ref arrCurrentWcNum, -1);
            Common.Memset(ref tmpErrorConc, Variable.CTG_DEFAULTRESULT);
            while (!m_CheckOver)
            {
                #region
                //MessageController.Instance.AddMessage("正在检定...");
                if (Stop) break;

                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
                if (base.VerifyPassTime > m_WCMaxSeconds &&
                   (GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.调表) != Cus_CheckStaute.调表)
                {
                    Helper.LogHelper.Instance.WriteWarm("当前点检定已经超过最大检定时间" + m_WCMaxSeconds + "秒！", null);
                    MessageController.Instance.AddMessage("当前点检定已经超过最大检定时间" + m_WCMaxSeconds + "秒！");
                    m_CheckOver = true;
                    break;
                }
                if ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.调表) == Cus_CheckStaute.调表)
                {
                    arrCheckOver = new bool[BwCount];
                }
                string[] arrLastWcValue = arrCurWCValue;
                int[] arrLastWcTimes = arrCurrentWcNum;

                arrCurWCValue = new string[BwCount];               //重新初始化本次误差
                arrCurrentWcNum = new int[BwCount];           //
                if (!GlobalUnit.IsDemo)
                    ShowWaitMessage("预计下一个误差将在{0}秒后计算完毕，请等待", sleepTime);
                else
                    ShowWaitMessage("预计下一个误差将在{0}秒后计算完毕，请等待", 3000);
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
                Helper.LogHelper.Instance.WriteInfo("开始读取当前检定点误差数据");
                if (!ReadData(ref arrCurWCValue, ref arrCurrentWcNum, arrPlanList[GlobalUnit.FirstYaoJianMeter].ErrorShangXian))
                {
                    continue;
                }

                if (Stop) break;
                m_CheckOver = true;
                //记录每块表的数据
                //arrStrResultKey = new string[BwCount];
                arrObjResultValue = new object[BwCount];
                //处理每一次检定数据
                Helper.LogHelper.Instance.WriteInfo("开始处理检定数据");
                List<string> listNotEnough = new List<string>();
                List<string> listOver = new List<string>();
                List<string> listNotPass = new List<string>();
                for (int i = 0; i < BwCount; i++)
                {
                    #region
                    if (Stop) break;
                    meterInfo = Helper.MeterDataHelper.Instance.Meter(i);      //表基本信息
                    if (!meterInfo.YaoJianYn)
                        arrCheckOver[i] = true;//不检表处理

                    if (!arrCheckOver[i] && arrCurrentWcNum[i] > 0)//
                    {
                        #region ----------数据合法性检测----------
                        /*
                        处理超过255次的情况
                        */
                        if (arrMeterWcNum[i] > 0 && arrCurrentWcNum[i] < arrMeterWcNum[i])
                        {
                            while (arrMeterWcNum[i] > arrCurrentWcNum[i])
                            {
                                arrCurrentWcNum[i] += 255;
                            }
                        }

                        //误差次数处理
                        if (arrMeterWcNum[i] < arrCurrentWcNum[i])
                        {
                            arrMeterWcNum[i] = arrCurrentWcNum[i];
                            arrVerifyTimes[i]++;  //这个才是真正的误差处理次数
                        }
                        else
                        {
                            //检测其它表位有没有出误差，给出相应的提示
                            int[] arr_Copy = (int[])arrVerifyTimes.Clone();
                            float[] arr_OtherWcnum = ConvertArray.ConvertInt2Float(arr_Copy);
                            Number.PopDesc(ref arr_OtherWcnum, false);
                            if (arr_OtherWcnum[0] > maxWCnum * 2 && arrVerifyTimes[i] == 0)
                            {
                                MessageController.Instance.AddMessage(String.Format("表位{0}没有检测到误差,请检查接线", i + 1), 6, 2);
                                ////ThreadManage.Sleep(3000); //停止3秒，让用户看提示消息
                            }
                            //误差次数没有增加，则此次误差板数据没有更新
                            if (arrVerifyTimes[i] < maxWCnum)
                                m_CheckOver = false;
                            continue;
                        }
                        if (arrCurrentWcNum[i] == 0 || arrCurrentWcNum[i] == 255)
                        {
                            m_CheckOver = false;
                            continue;            //如果本表位没有出误差，换下一表
                        }
                        #endregion

                        curPoint = arrPlanList[i];                              //当前检定方案
                        tmpError = GetMeterErrorData(meterInfo, curPoint);      //获取当前节点数据
                        meterLevel = MeterLevel(meterInfo);                   //当前表的等级
                        if (arrVerifyTimes[i] > tableHeader)
                        {
                            //推箱子,最后一次误差排列在最前面
                            for (int dtPos = tableHeader - 1; dtPos > 0; dtPos--)
                            {
                                errorTable.Rows[i][dtPos] = errorTable.Rows[i][dtPos - 1];
                            }
                            errorTable.Rows[i][0] = arrCurWCValue[i];     //最后一次误差始终放在第一位
                        }
                        else
                        {
                            errorTable.Rows[i][arrVerifyTimes[i] - 1] = arrCurWCValue[i];
                        }
                        /*计算误差*/
                        float[] tpmWc = ConvertArray.ConvertObj2Float(errorTable.Rows[i].ItemArray);  //Datable行到数组的转换
                        curError = CalculateMeterError(curPoint, meterLevel, tpmWc);
                        //tmpError.Me_chrWcJl = curError.Me_chrWcJl;
                        tmpErrorConc[i] = curError.Me_chrWcJl;
                        tmpError.Me_chrWcMore = curError.Me_chrWc;
                        //跳差检测
                        if (arrVerifyTimes[i] > 1)
                        {
                            string _PreWc = errorTable.Rows[i][1].ToString();
                            if (CheckJumpError(_PreWc, arrCurWCValue[i], meterLevel, m_WCJump))
                            {
                                arrCheckOver[i] = false;
                                //tmpError.Me_chrWcJl = Variable.CTG_BuHeGe;
                                tmpErrorConc[i] = Variable.CTG_BuHeGe;
                                if (arrVerifyTimes[i] > m_WCMaxTimes)
                                    arrCheckOver[i] = true;
                                else
                                {
                                    Helper.LogHelper.Instance.WriteInfo("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
                                    MessageController.Instance.AddMessage("检测到" + string.Format("{0}", i + 1) + "跳差，重新取误差进行计算");
                                    arrVerifyTimes[i] = 1;     //复位误差计算次数到
                                    m_CheckOver = false;
                                }
                            }
                        }
                        arrStrResultKey[i] = dataValueKey;
                        arrObjResultValue[i] = tmpError;
                        /*
                         * 检测是否所有表都合格
                         * 如果检定次数已经超过每个点检定的误差次数。则检测此点是否已经合格。如果不合格则
                         * 先检测当前误差次数是否已经达到每个点的最大误差差数或当前点检定时间是否已经超过
                         * 预定最大时间。是：则认为此表不合格。否：则清理掉当前点数据重检。
                         * _VerifyTimes[i] > _MaxWCnum 此时会把第一次误差数据清除掉。
                         */
                        if (arrVerifyTimes[i] >= maxWCnum)
                        {
                            //if (tmpError.Me_chrWcJl != Variable.CTG_HeGe && !arrCheckOver[i])
                            if (tmpErrorConc[i] != Variable.CTG_HeGe && !arrCheckOver[i])
                            {
                                if (arrVerifyTimes[i] > m_WCMaxTimes)
                                {
                                    arrCheckOver[i] = true;
                                    listOver.Add((i + 1).ToString());
                                }
                            }
                            else
                                arrCheckOver[i] = true;
                        }
                        else
                        {
                            arrCheckOver[i] = false;
                            listNotEnough.Add((i + 1).ToString());
                            //m_CheckOver = false;
                        }
                    }

                    if (i == BwCount - 1)
                    {
                        for (int j = 0; j < BwCount; j++)
                        {
                            if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                            if (!arrCheckOver[j])
                            {
                                listNotPass.Add((j + 1).ToString());
                                m_CheckOver = false;
                                break;
                            }
                            else
                            {
                                //int i6 = 123;
                            }
                        }
                    }
                    #endregion
                }

                //if (listNotPass.Count > 0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("第{0}块表还没有通过", string.Join(",", listNotPass.ToArray())));
                //}
                //if (listNotEnough.Count > 0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("第{0}表位还没有达到检定次数", string.Join(",", listNotEnough.ToArray())));
                //}
                //if (listOver.Count>0)
                //{
                //    MessageController.Instance.AddMessage(string.Format("第{0}表位超过最大检定次数", string.Join(",", listOver.ToArray())));
                //}

                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo meterTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i];
                    if (!meterTemp.YaoJianYn) continue;
                    if ((!meterTemp.MeterErrors.ContainsKey(ItemKey)) || string.IsNullOrEmpty(meterTemp.MeterErrors[ItemKey].Me_chrWcMore))
                    {
                        if (meterTemp.YaoJianYn == true)
                        {
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        }
                        continue;
                    }
                    string[] arrayTemp = GlobalUnit.g_CUS.DnbData.MeterGroup[i].MeterErrors[ItemKey].Me_chrWcMore.Split('|');
                    if (arrayTemp.Length > 3)
                    {
                        ResultDictionary["误差1"][i] = arrayTemp[0];
                        ResultDictionary["误差2"][i] = arrayTemp[1];
                        ResultDictionary["平均值"][i] = arrayTemp[2];
                        ResultDictionary["化整值"][i] = arrayTemp[3];
                    }
                    //ResultDictionary["结论"][i] = tmpErrorConc[i]; //curError.Me_chrWcJl;
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差1", ResultDictionary["误差1"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差2", ResultDictionary["误差2"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "平均值", ResultDictionary["平均值"]);
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "化整值", ResultDictionary["化整值"]);


                //如果是调表状态则不检测是否检定完毕
                //if ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.调表) == Cus_CheckStaute.调表)
                //{
                //    m_CheckOver = false;
                //}
                #endregion while结束
            }

            for (int i = 0; i < BwCount; i++)
            {
                #region
                if (Stop) break;
                meterInfo = Helper.MeterDataHelper.Instance.Meter(i);      //表基本信息
                if (!meterInfo.YaoJianYn)
                {
                    arrCheckOver[i] = true;//不检表处理
                }

                MeterError _EndError = GetMeterErrorData(meterInfo, curPoint);      //获取当前节点数据
                _EndError.Me_chrWcJl = tmpErrorConc[i];

                #endregion
            }

            float[] arrAvgErr = new float[BwCount]; 

            //处理结论
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");

            float AvgErr_YP =GetArryAvgErr(ResultDictionary["平均值"]);

            float Wcx = GetWcx(curPoint);


            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    string AvgErr1 = ResultDictionary["平均值"][i];
                    float VarietyErr = 100;

                    if (!string.IsNullOrEmpty(AvgErr1))
                    {
                        VarietyErr = Math.Abs(float.Parse(AvgErr1) - AvgErr_YP);
                        ResultDictionary["差值"][i] = VarietyErr.ToString();
                    }


                    if (VarietyErr <= Wcx)
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    }
                    ResultDictionary["样品均值"][i] = AvgErr_YP.ToString();
                }
            }
            //通知界面
            UploadTestResult("样品均值");
            UploadTestResult("差值");
            UploadTestResult("结论");


            //处理当前方向结论
            //ControlResult();
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "平均值", ResultDictionary["平均值"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "化整值", ResultDictionary["化整值"]);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            errorTable = null;
            MessageController.Instance.AddMessage("停止误差板!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 0);

        }

        #region ------------方法---------------

        #region 计算，检查
        /// <summary>
        /// 获取不平衡与平衡误差的±限值
        /// </summary>
        /// <returns></returns>
        private float GetErrLimit()
        {
            float value = 0f;
            string _Dj = GlobalUnit.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrBdj;
            float[] DJ = GetDJ(_Dj);//有功在数组0，无功在数组1
            if (CurPlan.PowerFangXiang == Cus_PowerFangXiang.正向无功 || CurPlan.PowerFangXiang == Cus_PowerFangXiang.反向无功)
            {
                if (DJ[1] == 2)
                    value = 3f;
                else if (DJ[1] == 3)
                    value = 4f;

            }
            else if (CurPlan.PowerFangXiang == Cus_PowerFangXiang.正向有功 || CurPlan.PowerFangXiang == Cus_PowerFangXiang.反向有功)
            {
                if (DJ[0] == 0.2f)
                    value = 0.3f;
                else if (DJ[0] == 0.5f)
                    value = 0.6f;
                else if (DJ[0] == 1f)
                    value = 2;
                else if (DJ[0] == 2f)
                    value = 3f;
            }
            return value;
        }
        /// <summary>
        /// 转化电能表等级类型（将字符串转化成浮点型）
        /// </summary>
        /// <param name="DJ"></param>
        /// <returns></returns>
        private float[] GetDJ(string DJ)
        {
            float[] value = new float[] { 0f, 0f };
            DJ = DJ.Replace("s", "");
            DJ = DJ.Replace("S", "");
            if (!DJ.Contains("("))
            {
                float.TryParse(DJ, out value[0]);
            }
            else
            {
                string temp = DJ;

                temp = temp.Substring(0, DJ.IndexOf("("));

                float.TryParse(temp, out value[0]);
                temp = DJ;
                temp = temp.Substring(DJ.IndexOf("(") + 1);
                temp = temp.Replace(")", "");
                float.TryParse(temp, out value[1]);
            }
            return value;
        }
        /// <summary>
        /// 计算出一个误差需要的时间ms
        /// </summary>
        ///<remarks>
        ///如果存在多种常数的电能表，则以最先出脉冲的电能表为准
        ///</remarks>
        /// <returns>出一个误差需要时间估算值,单位ms</returns>
        private int GetOneErrorTime()
        {
            MeterBasicInfo firstMeter = Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter);
            if (firstMeter == null) return 1000;//默认按一秒处理
            //计算当前负载功率
            float current = Number.GetCurrentByIb(CurPlan.PowerDianLiu, firstMeter.Mb_chrIb,firstMeter.Mb_BlnHgq);
            float currentPower = CalculatePower(GlobalUnit.U, current, GlobalUnit.Clfs, CurPlan.PowerYuanJian, CurPlan.PowerYinSu, IsYouGong);
            //计算一度大需要的时间,单位分钟
            float needTime = 1000F / currentPower * 60F;
            return OnePulseNeedTime(IsYouGong, needTime);
        }

        /// <summary>
        /// 获取当前数据节点，如果不存在则创建一个节点
        /// </summary>
        /// <param name="meterInfo">表基本信息</param>
        /// <param name="curPoint">当前方案</param>
        /// <returns>数据节点</returns>
        private MeterError GetMeterErrorData(MeterBasicInfo meterInfo, StPlan_WcPoint curPoint)
        {
            MeterError tmpError;
            if (meterInfo.MeterErrors.ContainsKey(ItemKey))
            {
                tmpError = meterInfo.MeterErrors[ItemKey];
            }
            else
            {
                tmpError = new MeterError();
                tmpError.Me_chrGlys = curPoint.PowerYinSu;      //功率因素
                switch (curPoint.PowerFangXiang)
                {
                    case Cus_PowerFangXiang.正向有功:
                        tmpError.Me_Glfx = "正向有功";
                        break;
                    case Cus_PowerFangXiang.反向有功:
                        tmpError.Me_Glfx = "反向有功";
                        break;
                    case Cus_PowerFangXiang.正向无功:
                        tmpError.Me_Glfx = "正向无功";
                        break;
                    case Cus_PowerFangXiang.反向无功:
                        tmpError.Me_Glfx = "反向无功";
                        break;
                    case Cus_PowerFangXiang.第一象限无功:
                        tmpError.Me_Glfx = "第一象限无功";
                        break;
                    case Cus_PowerFangXiang.第二象限无功:
                        tmpError.Me_Glfx = "第二象限无功";
                        break;
                    case Cus_PowerFangXiang.第三象限无功:
                        tmpError.Me_Glfx = "第三象限无功";
                        break;
                    case Cus_PowerFangXiang.第四象限无功:
                        tmpError.Me_Glfx = "第四象限无功";
                        break;
                }
                switch (curPoint.PowerYuanJian)
                {
                    case Cus_PowerYuanJian.A:
                        tmpError.Me_intYj = 2;
                        break;
                    case Cus_PowerYuanJian.B:
                        tmpError.Me_intYj = 3;
                        break;
                    case Cus_PowerYuanJian.C:
                        tmpError.Me_intYj = 4;
                        break;
                    case Cus_PowerYuanJian.H:
                        tmpError.Me_intYj = 1;
                        break;
                    default:
                        tmpError.Me_intYj = 1;
                        break;
                }
                tmpError._intMyId = meterInfo._intMyId;  //唯一编号
                //tmpError.Me_PL = meterInfo.Mb_chrHz;         //频率
                tmpError.Me_chrProjectNo = curPoint.PrjID;          //项目编号
                //tmpError.Me_PrjName = curPoint.PrjName;      //项目名称
                //tmpError.Me_Qs = curPoint.LapCount;          //圈数
                tmpError.Me_WcLimit = String.Format("{0}|{1}" //误差上下限
                    , Math.Round(curPoint.ErrorShangXian * GlobalUnit.g_CUS.DnbData.WcxUpPercent, 2),
                    curPoint.ErrorXiaXian * GlobalUnit.g_CUS.DnbData.WcxDownPercent);
                tmpError.Me_dblxIb = curPoint.PowerDianLiu;     //电流倍数
                //tmpError.Me_xU = CLDC_DataCore.Const.GlobalUnit.U.ToString();           //电压
                tmpError.Me_chrWcJl = Variable.CTG_DEFAULTRESULT;

                meterInfo.MeterErrors.Add(ItemKey, tmpError);
            }
            return tmpError;
        }
        /// <summary>
        /// 计算误差值
        /// </summary>
        /// <param name="curPoint">当前节点方案</param>
        /// <param name="meterLevel">表等级</param>
        /// <param name="wcData">误差数据</param>
        /// <returns>误差结构</returns>
        private MeterError CalculateMeterError(StPlan_WcPoint curPoint, float meterLevel, float[] wcData)
        {
            CLDC_DataCore.WuChaDeal.WuChaContext m_WuChaContext;
            CLDC_DataCore.Struct.StWuChaDeal wuChaPara = new CLDC_DataCore.Struct.StWuChaDeal();
            wuChaPara.MaxError = curPoint.ErrorShangXian;// * GlobalUnit.g_CUS.DnbData.WcxUpPercent;
            wuChaPara.MinError = curPoint.ErrorXiaXian;// * GlobalUnit.g_CUS.DnbData.WcxDownPercent;
            wuChaPara.MeterLevel = meterLevel;

            if (curPoint.Pc == 1)
                m_WuChaContext = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.标准偏差, wuChaPara);
            else
                m_WuChaContext = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.基本误差, wuChaPara);

            //WuChaPara = _WuChaPara;
            return (MeterError)m_WuChaContext.GetResult(wcData);
        }

        /// <summary>
        /// 获取一组数据的平均值
        /// </summary>
        /// <param name="arryErr"></param>
        /// <returns></returns>
        private float GetArryAvgErr(string[] arryErr)
        {
            float RevArryAvgErr = 0;
            int k = 0;

            for (int i = 0; i < arryErr.Length; i++)
            {
                if (string.IsNullOrEmpty(arryErr[i])) continue;
                RevArryAvgErr += float.Parse(arryErr[i]);
                k++;
            }
            RevArryAvgErr = RevArryAvgErr / k;
            return RevArryAvgErr;
        }

        /// <summary>
        /// 按电流、功率因数算出误差限
        /// </summary>
        /// <param name="curPoint"></param>
        /// <returns></returns>
        private float GetWcx(StPlan_WcPoint curPoint)
        {
            float Wcx = 100;
            if (curPoint.PowerDianLiu == "Ib" && curPoint.PowerYinSu == "1.0")
            {
                Wcx = 0.3F;
            }
            else if (curPoint.PowerDianLiu == "Ib" && curPoint.PowerYinSu == "0.5L")
            {
                Wcx = 0.3F;
            }
            else if (curPoint.PowerDianLiu == "Itr" && curPoint.PowerYinSu == "1.0")
            {
                Wcx = 0.4F;
            }
            return Wcx;
        }
        

        #endregion

        #region ----------参数初始化InitVerifyPara----------
        /// <summary>
        /// 初始化检定参数，包括初始化虚拟表单，初始化方案参数，初始化脉冲个数
        /// </summary>
        /// <param name="tableHeader">表头数量</param>
        /// <param name="planList">方案列表</param>
        /// <param name="Pulselap">检定圈数</param>
        /// <param name="DT">虚表</param>
        private void InitVerifyPara(int tableHeader, ref StPlan_WcPoint[] planList, ref int[] Pulselap, DataTable DT)
        {
            //上报数据参数
            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            planList = new StPlan_WcPoint[BwCount];
            Pulselap = new int[BwCount];
            MessageController.Instance.AddMessage("开始初始化检定参数...");
            //初始化虚表头
            for (int i = 0; i < tableHeader; i++)
            {
                DT.Columns.Add("WC" + i.ToString());
            }
            //填充空数据
            MeterBasicInfo _MeterInfo = null;
            string[] arrCurTypeBw = new string[0];
            Helper.MeterDataHelper.Instance.Init();
            for (int iType = 0; iType < Helper.MeterDataHelper.Instance.TypeCount; iType++)
            {
                //从电能表数据管理器中取每一种规格型号的电能表
                arrCurTypeBw = Helper.MeterDataHelper.Instance.MeterType(iType);
                int curFirstiType = 0;//当前类型的第一块索引
                for (int i = 0; i < arrCurTypeBw.Length; i++)
                {
                    if (!Number.IsIntNumber(arrCurTypeBw[i]))
                        continue;
                    //取当前要检的表号
                    int curMeterNumber = int.Parse(arrCurTypeBw[i]);
                    //得到当前表的基本信息
                    _MeterInfo = Helper.MeterDataHelper.Instance.Meter(curMeterNumber);
                    if (_MeterInfo.MeterErrors.ContainsKey(ItemKey))
                        _MeterInfo.MeterErrors.Remove(ItemKey);
                    strResultKey[curMeterNumber] = ItemKey;
                    if (_MeterInfo.YaoJianYn)
                    {
                        planList[curMeterNumber] = CurPlan;
                        planList[curMeterNumber].SetLapCount(Helper.MeterDataHelper.Instance.MeterConstMin(),
                                         _MeterInfo.Mb_chrBcs,
                                         _MeterInfo.Mb_chrIb,
                                         GlobalUnit.g_CUS.DnbData.CzIb,
                                         GlobalUnit.g_CUS.DnbData.CzQs
                                         );
                        planList[curMeterNumber].SetWcx(GlobalUnit.g_CUS.DnbData.CzWcLimit,
                            _MeterInfo.GuiChengName,
                             _MeterInfo.Mb_chrBdj,
                            _MeterInfo.Mb_BlnHgq);
                        planList[curMeterNumber].ErrorShangXian *= GlobalUnit.g_CUS.DnbData.WcxUpPercent;
                        planList[curMeterNumber].ErrorXiaXian *= GlobalUnit.g_CUS.DnbData.WcxDownPercent;

                        Pulselap[curMeterNumber] = planList[curMeterNumber].LapCount;
                        curFirstiType = curMeterNumber;
                    }
                    else
                    {
                        //不检定表设置为第一块要检定表圈数。便于发放统一检定参数。提高检定效率
                        Pulselap[curMeterNumber] = planList[curFirstiType].LapCount;
                    }

                }
            }
            //重新填充不检的表位
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)             //这个地方创建虚表行，多少表位创建多少行！！
            {
                DT.Rows.Add(new string[(tableHeader - 1)]);
                //如果有不检的表则直接填充为第一块要检表的圈数
                if (Pulselap[i] == 0)
                    Pulselap[i] = Pulselap[GlobalUnit.FirstYaoJianMeter];
            }

            MessageController.Instance.AddMessage("初始化检定参数完毕! ");
        }
        #endregion

        #region 初始化设备参数：正式版本有效[基本误差]:InitEquipment
        /// <summary>
        /// 初始化设备参数,计算每一块表需要检定的圈数
        /// </summary>
        /// <param name="_curPoint"></param>
        /// <param name="PL"></param>
        /// <param name="_Pulselap"></param>
        /// <returns></returns>
        private bool InitEquipment(StPlan_WcPoint _curPoint, float PL, int[] _Pulselap)
        {
            if (GlobalUnit.IsDemo) return true;
            //MessageController.Instance.AddMessage("设置表开关!", false);
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());
            

            MessageController.Instance.AddMessage("开始控制源输出!");
            float _xIb = Number.GetCurrentByIb(_curPoint.PowerDianLiu, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            bool result = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb, (int)_curPoint.PowerYuanJian, (int)_curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
            if (!result)
            {
                bool isSuccess = false;
                for (int i = 1; i <= 3; i++)
                {
                    System.Threading.Thread.Sleep(i * 1000);
                    string msg = "正控制源输出失败,重复" + i.ToString() + "次";
                    MessageController.Instance.AddMessage(msg);
                    msg = DateTime.Now.ToString() + "==========>" + msg;
                    ErrorLog.Write(new Exception(msg));
                    isSuccess = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb, (int)_curPoint.PowerYuanJian, (int)_curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
                    if (isSuccess)
                    {
                        break;
                    }
                }
                if (!isSuccess)
                {
                    MessageController.Instance.AddMessage("控制源输出失败", 6, 2);
                    //return false;
                }
            }
            if (Stop) return true;
            MessageController.Instance.AddMessage("控制源输出成功");
            MessageController.Instance.AddMessage("开始初始化基本误差检定参数!");
            int[] meterconst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
            //System.Threading.Thread.Sleep(base.m_WaitTime_PowerOn);
            result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang,meterconst, _Pulselap);
            MessageController.Instance.AddMessage("初始化误差检定参数" + GetResultString(result));
            if (!result)
            {
                result = Helper.EquipHelper.Instance.InitPara_BasicError(_curPoint.PowerFangXiang,meterconst, _Pulselap);
                if (!result)
                {
                    MessageController.Instance.AddMessage("初始化误差检定参数失败", 6, 2);
                    //return false;
                }
            }
            //System.Threading.Thread.Sleep(2000);
            return result;
        }
        #endregion

        #region ----------加载/检测检定参数是否正确:CheckPara
        /// <summary>
        /// 加载/检测检定参数是否正确
        /// </summary>
        protected override bool CheckPara()
        {
            //误差试验类型|功率方向|功率元件|功率因数|电流倍数|添加谐波|逆相序|误差试验圈数|上限|下限
            //基本误差|正向有功|H|1.0|Imax|否|否
            string[] arrayErrorPara = VerifyPara.Split('|');
         
            StPlan_WcPoint st_Wc = new StPlan_WcPoint();
            st_Wc.PrjID = "111010700";
            st_Wc.Dif_Err_Flag = 0;
            GlobalUnit.g_CUS.DnbData.SetWcxPercent(1, 1);
            //st_Wc.ErrorShangXian = 1;//如果赋值则不再根据规程计算
            //st_Wc.ErrorXiaXian = -1;S
            st_Wc.IsCheck = true;
            st_Wc.LapCount = 2;
            GlobalUnit.g_CUS.DnbData.SetCzQsIb(2, "Ib");
            st_Wc.nCheckOrder = 1;
            st_Wc.Pc = 0;
            st_Wc.PointId = 1;
            st_Wc.PowerDianLiu = arrayErrorPara[1];
            st_Wc.PowerFangXiang = Cus_PowerFangXiang.正向有功;
            st_Wc.PowerYinSu = arrayErrorPara[2];
            st_Wc.PowerYuanJian = Cus_PowerYuanJian.H;
            
            st_Wc.PrjName = "";
            st_Wc.XiangXu = 0;
            st_Wc.XieBo = 0;
            CurPlan = st_Wc;

            //每一个误差点取几次误差参与计算
            m_WCTimesPerPoint = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_BASICERROR, 2);
            // 标准偏差点取几次误差参与计算
            m_WindageTimesPerPoint = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_WINDAGE, 5);
            if (m_WindageTimesPerPoint % 5 != 0 || m_WindageTimesPerPoint < 5)
            {
                string strinfo = string.Format("检测到当前设置值为:{0},根据规程要求，标准偏差应该为5的整数倍。系统自动控制调整为5\r\n如果此对话框再次出现请到系统菜单中修改标准偏差次数为5的整数倍", m_WindageTimesPerPoint);
                m_WindageTimesPerPoint = 5;
                MessageController.Instance.AddMessage(strinfo, 6, 2);
            }
            /*每一个误差点最多读取多少次误差
         如果最多次数小于计算次数时则修改最多次数
        */
            m_WCMaxTimes = GlobalUnit.GetConfig(Variable.CTC_WC_MAXTIMES, 2);
            if (m_WCMaxTimes < m_WCTimesPerPoint)
                m_WCMaxTimes = m_WCTimesPerPoint;
            m_WCMaxSeconds = GlobalUnit.GetConfig(Variable.CTC_WC_MAXSECONDS, 10);
            m_WCJump = GlobalUnit.GetConfig(Variable.CTC_WC_JUMP, 1F);
            //标准脉冲分频系数
            m_DriverF = GlobalUnit.GetConfig(Variable.CTC_DRIVERF, 1F);
            return true;
        }
        #endregion

        #region 当前检定方向结论
        /// <summary>
        /// 处理当前方向结论
        /// </summary>
        protected void ControlResult()
        {
            if (Stop) return;
            Cus_MeterResultPrjID curItemID;

            StPlan_WcPoint curPoint = (StPlan_WcPoint)CurPlan;
            if (curPoint.Pc == 1)
            {
                curItemID = Cus_MeterResultPrjID.标准偏差;
            }
            else
            {
                curItemID = Cus_MeterResultPrjID.基本误差试验;
            }
            //当前检定方向总结论节点名称
            string curItemKey = ((int)curItemID).ToString("D3") + ((int)PowerFangXiang).ToString();
            for (int bw = 0; bw < BwCount; bw++)
            {

                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(bw);
                //开始处理分方向结论
                if (curMeter.MeterResults.ContainsKey(curItemKey))
                    curMeter.MeterResults.Remove(curItemKey);
                if (!curMeter.YaoJianYn) continue;
                //标准偏差结论处理
                if (curItemID == Cus_MeterResultPrjID.标准偏差)
                {
                    ControlPCResult(curMeter);
                }
                else
                {
                    //基本误差结论处理
                    MeterResult curResult = new MeterResult();
                    curResult.Mr_chrRstId = curItemKey;
                    curResult.Mr_chrRstName = curItemID.ToString() + PowerFangXiang.ToString() + "结论";
                    curResult.Mr_chrRstValue = Variable.CTG_HeGe;
                    curMeter.MeterResults.Add(curItemKey, curResult);
                    //分方向结论处理完毕
                    if (curMeter.MeterErrors.ContainsKey(ItemKey))
                    {

                        if (curMeter.MeterErrors[ItemKey].Me_chrWcJl == Variable.CTG_BuHeGe)
                            curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                        else
                        {
                            //检测当前方向下的其它点是否合格
                            if (!isTheSamePowerPDHeGe(curMeter))
                                curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                        }
                    }
                    else
                    {
                        //GlobalUnit.ForceVerifyStop = true;
                        //Stop = true;
                        //Check.Require(false, "当前误差点没有读取到误差，请通过以下几种方法来解决：\r\n1、检查表脉冲接线是否正确\r\n2、检查脉冲通道共阴共阳是否正确\r\n3、如果是小电流点，系统设置中最大处理时间值设置大一些");
                        //ThreadManage.Sleep(1000);
                        return;
                    }
                }
            }
        }

        //处理最大偏差
        private void ControlPCResult(MeterBasicInfo curMeter)
        {

            //标准偏差处理
            #region -----处理最大偏差:一块表一个最大偏差-----
            string strKey = String.Format("{0}", ((int)CLDC_Comm.Enum.Cus_MeterResultPrjID.最大偏差).ToString());
            //strKey += ((int)_curPoint.PowerFangXiang).ToString();       //加功率方向
            MeterResult _MaxWindage = null;
            if (!curMeter.MeterErrors.ContainsKey(ItemKey))
            {
                Stop = true;
                //GlobalUnit.ForceVerifyStop = true;
                //MessageController.Instance.AddMessage("当前误差点没有读取到误差，请通过以下几种方法来解决：\r\n1、检查表脉冲接线是否正确\r\n2、检查脉冲通道共阴共阳是否正确\r\n3、如果是小电流点，系统设置中最大处理时间值设置大一些", 6, 2);
                //ThreadManage.Sleep(1000);
                return;
                //throw new Exception("当前检定点中没有发现误差数据");
            }

            string[] _w = curMeter.MeterErrors[ItemKey].Me_chrWcMore.Split('|');
            if (!curMeter.MeterResults.ContainsKey(strKey))
            {
                _MaxWindage = new MeterResult();
                _MaxWindage.Mr_chrRstId = strKey;
                _MaxWindage.Mr_chrRstName = CLDC_Comm.Enum.Cus_MeterResultPrjID.最大偏差.ToString();
                _MaxWindage.Mr_chrRstValue = _w[_w.Length - 1];       //最大偏差时这儿保存的是偏差值
                curMeter.MeterResults.Add(strKey, _MaxWindage);
            }
            else
            {
                _MaxWindage = curMeter.MeterResults[strKey];
                //如果本次比上次大，则替换
                float _LastWindage = float.Parse(_MaxWindage.Mr_chrRstValue);
                float _thisWindage = float.Parse(_w[_w.Length - 1]);
                if (_thisWindage > _LastWindage)
                {
                    /* 
                     这儿为什么为不用_thisWindage呢？
                     因为_thisWindage是被parse过的，会影响后面的小数位
                     */
                    _MaxWindage.Mr_chrRstValue = _w[_w.Length - 1];
                }
            }

            #endregion

        }

        /// <summary>
        /// 是否相同方向下的所有当前检定项目都合格
        /// </summary>
        /// <param name="curMeter">当前表数据</param>
        /// <returns></returns>
        private bool isTheSamePowerPDHeGe(MeterBasicInfo curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.MeterErrors.Keys)
            {
                //当前功率方向
                MeterError _Item = curMeter.MeterErrors[strKey];

                if (_Item.Me_chrProjectNo.Substring(0, 1).Trim() == "1")            //如果是误差
                {
                    Cus_PowerFangXiang thisPointFX = (Cus_PowerFangXiang)(int.Parse(_Item.Me_chrProjectNo.Substring(1, 1)));
                    //CheckPoint curResultItem = (CheckPoint)CurPlan;
                    if (PowerFangXiang == thisPointFX)
                    {
                        if (_Item.Me_chrWcJl == Variable.CTG_BuHeGe)
                        {
                            isAllItemOk = false;
                            break;
                        }
                    }
                }
            }
            return isAllItemOk;
        }
        #endregion
        #endregion
    }
}
