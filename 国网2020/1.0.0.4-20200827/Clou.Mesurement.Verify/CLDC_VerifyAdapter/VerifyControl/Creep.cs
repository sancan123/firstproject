
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DeviceDriver;
////using ClInterface;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 潜动试验
    /// 只记录当前方向结论
    /// </summary>
    class Creep : VerifyBase
    {
        float creepI = 9999999F;
        //检测系统参照规程
        float[] arrCreepTimes = new float[0];
        bool[] CheckOver = new bool[0];
        public Creep(object plan) : base(plan) { }

        #region-------基类方法重写 --------
        /// <summary>
        /// 总结论主键
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.潜动试验);
            }
        }

        /// <summary>
        /// 重写数据主键
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}{2}"                                          //Key:参见数据结构设计附2
                        , ResultKey
                        , ((int)PowerFangXiang).ToString()
                       , (Convert.ToInt32(curPlan.FloatxU * 100)).ToString("D3"));//, curPlan.FloatxU.ToString()
            }
        }
        private StPlan_QianDong curPlan = new StPlan_QianDong();
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //这里要解析检定参数
            //功率方向|是否默认合格|潜动电压|潜动电流|自动计算潜动时间|潜动时间
            string[] arrayTemp = VerifyPara.Split('|');
            if (arrayTemp.Length != 6)
            {
                return false;
            }
            Cus_PowerFangXiang fangxiangTemp = Cus_PowerFangXiang.正向有功;
            fangxiangTemp = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), arrayTemp[0]);
            curPlan.PowerFangXiang = fangxiangTemp;
            float floatTemp = 1.15F;
            if (float.TryParse(arrayTemp[2].Replace("%", ""), out floatTemp))
            {
                curPlan.FloatxU = floatTemp / 100F;
            }
            else
            {
                curPlan.FloatxU = 1.15F;
            }
            curPlan.FloatxIb = 0;
            switch (arrayTemp[3])
            {
                case "1/4启动电流":
                    curPlan.FloatxIb = 0.25F;
                    break;
                case "1/5启动电流":
                    curPlan.FloatxIb = 0.2F;
                    break;
            }
            if (arrayTemp[4] == "是")
            {
                curPlan.xTime = 0;
            }
            else
            {
                float fTemp = 0;
                if (float.TryParse(arrayTemp[5], out fTemp))
                {
                    curPlan.xTime = fTemp;
                }
                else
                {
                    curPlan.xTime = 0;
                }
            }
            curPlan.DefaultValue = arrayTemp[1] == "是" ? 1 : 0;
            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "功率方向", "试验电压", "标准试验时间", "试验电流", "开始时间", "结束时间", "实际运行时间", "脉冲数", "结论" };

            return true;
        }
        /// <summary>
        /// 挂默认数据,项目主键：ResultKey+PowerFangXiand
        /// </summary>
        protected override void DefaultItemData()
        {
            //结论数据
            string[] arrStrResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];

            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
                /*挂接结论数据*/
                MeterQdQid _MeterQdQid = new MeterQdQid();
                if (_Meter.MeterQdQids.ContainsKey(ItemKey))
                {
                    _Meter.MeterQdQids.Remove(ItemKey);
                }
                _Meter.MeterQdQids.Add(ItemKey, _MeterQdQid);
                _MeterQdQid.Mqd_chrDL = _tagQianDong.FloatIb.ToString();// _StartI.ToString("F4");           //潜动电流
                _MeterQdQid.AVR_VOLTAGE = curPlan.FloatxU.ToString();
                _MeterQdQid._intMyId = _Meter._intMyId;                //关联表ID
                _MeterQdQid.Mqd_chrProjectNo = ItemKey;                             //项目ID
                _MeterQdQid.Mqd_chrProjectName = _tagQianDong.ToString();          //项目名称
                _MeterQdQid.Mqd_chrJdfx = (Convert.ToInt32(curPlan.PowerFangXiang)).ToString();
                _MeterQdQid.Mqd_chrJL = Variable.CTG_HeGe;      //默认合格
                _MeterQdQid.AVR_STANDARD_TIME = arrCreepTimes[i].ToString();
                _MeterQdQid.AVR_ACTUAL_TIME = arrCreepTimes[i].ToString();
                _MeterQdQid.Mqd_chrTime = arrCreepTimes[i].ToString();   //标准时间
                arrStrResultKey[i] = ItemKey;
                objResultValue[i] = _MeterQdQid;
            }

            base.DefaultItemData();
        }
        #endregion

        #region ----------计算潜动时间及电流InitVerifyPara----------
        /// <summary>
        /// 初始化潜动参数
        /// </summary>
        /// <returns></returns>
        private float InitVerifyPara()
        {
            string[] arrayErrorPara = VerifyPara.Split('|');
          
            if (arrayErrorPara[4] == "否")
            {
                curPlan.xTime = float.Parse(arrayErrorPara[5]);
                curPlan.CheckTime = float.Parse(arrayErrorPara[5]);

            }
            float xU = curPlan.FloatxU;
            arrCreepTimes = new float[BwCount];
            bool bHasStartItem = false;             //是否有相同功率方向的启动项目
            float startXIB = 0F;                    //是否自定义起动电流
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!_Meter.YaoJianYn) continue;
                StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
                if (_tagQianDong.CheckTime == 0 && _tagQianDong.FloatIb == 0)
                {
                    bHasStartItem = isThisPFHasStartItem(ref startXIB);
                    if (bHasStartItem && startXIB > 0F)
                    {
                        _tagQianDong.CheckTimeAndIb(_Meter.GuiChengName,
                                                   GlobalUnit.Clfs,
                                                   GlobalUnit.U,
                                                   _Meter.Mb_chrIb,
                                                   startXIB,
                                                   _Meter.Mb_chrBdj,
                                                   _Meter.Mb_chrBcs,
                                                   _Meter.Mb_BlnZnq,
                                                   _Meter.Mb_BlnHgq);

                    }
                    else
                    {
                        _tagQianDong.CheckTimeAndIb(_Meter.GuiChengName,
                                                   GlobalUnit.Clfs,
                                                   GlobalUnit.U,
                                                   _Meter.Mb_chrIb,
                                                   _Meter.Mb_chrBdj,
                                                   _Meter.Mb_chrBcs,
                                                   _Meter.Mb_BlnZnq,
                                                   _Meter.Mb_BlnHgq);
                    }
                }
                arrCreepTimes[i] = (float)Math.Round(_tagQianDong.CheckTime*60, 2);
                if (_tagQianDong.FloatIb < creepI)
                    creepI = _tagQianDong.FloatIb;

            }
            //选择一个最大潜动时间
            float[] arrCreepTimeClone = (float[])arrCreepTimes.Clone();
            CLDC_DataCore.Function.Number.PopDesc(ref arrCreepTimeClone, false);
            MessageController.Instance.AddMessage("初始化检定数据完成");
            if (GlobalUnit.IsDemo)
                return 1F;
            else
                return arrCreepTimeClone[0];
        }
        /// <summary>
        /// 是否当前潜动功率方向下有启动项目
        /// </summary>
        /// <returns></returns>
        private bool isThisPFHasStartItem(ref float xIB)
        {
            StPlan_QianDong _tagQianDong = (StPlan_QianDong)curPlan;
            //object o = null;
            StPlan_QiDong _tagQiDong;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.CheckPlan.Count; i++)
            {
                // o = getPlan(CLDC_DataCore.Const.GlobalUnit.FirstYaoJianMeter, i);
                if (GlobalUnit.g_CUS.DnbData.CheckPlan[i] is StPlan_QiDong)
                {
                    _tagQiDong = (StPlan_QiDong)GlobalUnit.g_CUS.DnbData.CheckPlan[i];
                    //只能寻找到第一个符合要求的启动项目
                    if (_tagQianDong.PowerFangXiang == _tagQiDong.PowerFangXiang)
                    {
                        xIB = _tagQiDong.FloatxIb;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region ----------开始检定----------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
            string[] NoUse = new string[0];
            int[] PulseNum = new int[0];
            long _PastTime = 0;
            GlobalUnit.g_CurTestType = 3;
            float TotalTime = InitVerifyPara();
            base.Verify();
            CheckOver = new bool[BwCount];
            m_StartTime = DateTime.Now;

            if (Stop) return;
            //计算最大潜动时间
            float _MaxCreepTime = TotalTime /60;
            //上报检定参数
            object[] arrObjResultValue = new object[BwCount];
            //默认合格情况不作检定
            if (curPlan.DefaultValue == 1)
            {
                int totalTime = 3000;
                while (totalTime > 0)
                {
                    MessageController.Instance.AddMessage("方案设置默认合格,等待" + (totalTime / 1000) + "秒");
                    if (Stop) break;
                    Thread.Sleep(1000);
                    totalTime -= 1000;
                }
                for (int Num = 0; Num < BwCount; Num++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn)
                    {
                        continue;
                    }

                    ResultDictionary["试验电流"][Num] = creepI.ToString("F2");
                    ResultDictionary["开始时间"][Num] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["功率方向"][Num] = curPlan.PowerFangXiang.ToString();
                    ResultDictionary["试验电压"][Num] = (curPlan.FloatxU * GlobalUnit.U).ToString("F4");
                    ResultDictionary["实际运行时间"][Num] = (VerifyPassTime / 60.0).ToString("F4") + "分";
                    ResultDictionary["结论"][Num] = Variable.CTG_HeGe;
                }
                ConvertTestResult("标准试验时间", arrCreepTimes, 2);

                UploadTestResult("试验电压");
                UploadTestResult("试验电流");
                UploadTestResult("开始时间");
                UploadTestResult("功率方向");
                UploadTestResult("标准试验时间");
                UploadTestResult("实际运行时间");
                UploadTestResult("结论");
            }
            else
            {
                if (!GlobalUnit.IsDemo)
                {
                    #region
                    //PulseChannelDetection pulseDetec = new PulseChannelDetection(null);
                    //pulseDetec.ParentControl = this;
                    ////对标
                    //if (!pulseDetec.DuiSheBiao(curPlan.PowerFangXiang, CLDC_DataCore.Const.GlobalUnit.Ib * 0.1F, 1))
                    //{
                    //    Helper.LogHelper.Instance.Loger.Debug("对色标失败，退出检定");
                    //    return;
                    //}
                    if (Stop) return;
                    //初始化设置
                    int[] creepTimes = new int[BwCount];
                    for (int bw = 0; bw < BwCount; bw++)
                    {
                        creepTimes[bw] = (int)(arrCreepTimes[bw] / 60F);
                    }

                  

                    if (Stop) return;
                    //控制源输出
                    if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * curPlan.FloatxU, creepI, (int)Cus_PowerYuanJian.H, (int)curPlan.PowerFangXiang, "1.0", IsYouGong, false))
                    {
                        return;
                    }
                    if (GlobalUnit.clfs == Cus_Clfs.单相)
                    {
                        if (curPlan.PowerFangXiang == Cus_PowerFangXiang.正向无功 || curPlan.PowerFangXiang == Cus_PowerFangXiang.反向无功)
                        {
                            MeterProtocolAdapter.Instance.SetPulseCom(3);
                        }
                    }
                    if (!Helper.EquipHelper.Instance.InitPara_Creep(curPlan.PowerFangXiang, creepI, IsYouGong, creepTimes))
                    {

                        MessageController.Instance.AddMessage("初始化潜动参数失败，退出检定");
                        return;
                    }
                    #endregion
                    //
                    //Helper.EquipHelper.Instance.InitPara_InitTimeAccuracy();
                    //
                }
                m_StartTime = DateTime.Now;

                #region 上报试验参数
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["试验电流"][i] = creepI.ToString("F2");
                        ResultDictionary["开始时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["功率方向"][i] = curPlan.PowerFangXiang.ToString();
                        ResultDictionary["试验电压"][i] = (curPlan.FloatxU * GlobalUnit.U).ToString("F4");
                        arrCreepTimes[i] = arrCreepTimes[i] / 60F;
                    }
                }
                ConvertTestResult("标准试验时间", arrCreepTimes, 2);
                UploadTestResult("试验电压");
                UploadTestResult("试验电流");
                UploadTestResult("开始时间");
                UploadTestResult("功率方向");
                UploadTestResult("标准试验时间");
                #endregion

                Stop = false;
                //int rjs = 1;
                MessageController.Instance.AddMessage("检定中...");
                while (true)
                {
                    _PastTime = VerifyPassTime;

                    //每一秒刷新一次数据
                    Thread.Sleep(1000);
                    if (Stop)
                    {
                        Helper.LogHelper.Instance.Loger.Debug("外部停止，退出检定");
                        Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false,7);
                        m_CheckOver = true;
                        return;
                    }
                    if (!GlobalUnit.IsDemo)
                    {
                        ReadAndDealData(_PastTime);
                    }
                    else
                    {
                        m_CheckOver = false;
                    }

                    float PastTime = _PastTime / 60F; //转化为分发送到UI 
                    GlobalUnit.g_CUS.DnbData.NowMinute = PastTime;
                    MessageController.Instance.AddMessage("潜动时间" + (TotalTime/60).ToString("F2") + "分，已经经过" + PastTime.ToString("F2") + "分");
                    if ((PastTime > _MaxCreepTime) || m_CheckOver)
                    {
                        m_CheckOver = true;
                        Helper.LogHelper.Instance.Loger.Debug("潜动时间已到，退出检定");
                        MessageController.Instance.AddMessage("潜动时间已到，退出检定");
                        break;
                    }
                    //if (PastTime > 1 && rjs<2)
                    //{
                    //    rjs++;
                    //    Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false);
                    //}
                }
            }

            if (!GlobalUnit.IsDemo && !Stop && !m_CheckOver)
            {
                //完了再读一次，以防万一
                ReadAndDealData((long)_MaxCreepTime);
            }

            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    string resultTemp = ResultDictionary["结论"][i];
                    ResultDictionary["结论"][i] = resultTemp == "不合格" ? "不合格" : "合格";
                    if (resultTemp == "合格")
                    {
                        ResultDictionary["结束时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            UploadTestResult("实际运行时间");
            Thread.Sleep(1000);
            UploadTestResult("结束时间");
            Thread.Sleep(1000);
            UploadTestResult("结论");
            Thread.Sleep(1000);
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false,3);
            m_CheckOver = true;
        }

        /// <summary>
        /// 读取并处理数据[演示版无效]
        /// </summary>
        /// <param name="verifyTimes"></param>
        private void ReadAndDealData(long verifyTimes)
        {
           // stError[] arrTagError = Helper.EquipHelper.Instance.ReadWcb(true);
            if (Stop)
            {
                return;
            }
            //当所有表位均为不合格时,检定完毕
            m_CheckOver = true;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    CheckOver[i] = true;
                    continue;
                }
                int num = 0;
                string data = string.Empty;
                string time = string.Empty;

                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(i + 1, 3, out num, out data, out time);
                int intTemp = 0;
              //  int.TryParse(arrTagError[i].szError, out intTemp);
                //分析数据
                //如果脉冲数大于0,不合格
                if (verifyTimes < (arrCreepTimes[i]*60F) && CheckOver[i] == false)
                {
                    ResultDictionary["脉冲数"][i] = data;
                    if (data == "" || data == null) data = "0";
                    if (data != null && data != "" )
                    {
                        if (float.Parse(data) > 0)
                        {
                            ResultDictionary["实际运行时间"][i] = (verifyTimes / 60.0).ToString("F4") + "分";
                            ResultDictionary["结束时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                            CheckOver[i] = true;
                        }
                        else
                        {
                            ResultDictionary["实际运行时间"][i] = (((float)verifyTimes) / 60.0).ToString("F4") + "分";
                            CheckOver[i] = false;

                        }
                    }
                }
                if (!CheckOver[i])
                {
                    m_CheckOver = false;
                }
                if (Stop) break;

            }
            UploadTestResult("实际运行时间");
            Thread.Sleep(1000);
            UploadTestResult("结束时间");
            Thread.Sleep(1000);
            UploadTestResult("脉冲数");
            Thread.Sleep(1000);
        }
        #endregion
    }
}
