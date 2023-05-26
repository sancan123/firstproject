
using System;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 启动/起动试验检定器
    /// 起动试验结论数据直接记录在_MeterQdQid中
    /// </summary>
    class Starts : VerifyBase
    {

        bool[] CheckOver = new bool[0];
        /// <summary>
        /// 每一块表需要的起动时间
        /// </summary>
        float[] arrStartTimes = new float[0];
        /// <summary>
        /// 每一块表需要的起动电流
        /// </summary>
        float[] arrStartCurrents = new float[0];
        /// <summary>
        /// 起动读取的第一个时间
        /// </summary>
        float[] StartTimeBefore = new float[0];
        /// <summary>
        /// 起动读取的第二个时间
        /// </summary>
        float[] StartTimeAffter = new float[0];
        /// <summary>
        /// 最终起动电流
        /// </summary>
        float startCurrent = 0F;

        public Starts(object plan)
            : base(plan)
        {
        }
        /// <summary>
        /// 如果有参数要重写CheckPara()
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //这里要解析检定参数
            //功率方向|是否默认合格|自动计算试验电流|试验电流|自动计算试验时间|试验时间(分钟)
            string[] arrayTemp = VerifyPara.Split('|');
            if (arrayTemp.Length != 7)
            {
                return false;
            }
            Cus_PowerFangXiang fangxiangTemp = Cus_PowerFangXiang.正向有功;
            fangxiangTemp = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), arrayTemp[1]);
            curPlan.PowerFangXiang = fangxiangTemp;
            if (arrayTemp[3] == "是")
            {
                curPlan.FloatxIb = 0;
            }
            else
            {
                curPlan.FloatxIb = float.Parse(arrayTemp[4]);
            }
            if (arrayTemp[5] == "是")
            {
                curPlan.xTime = 0;
            }
            else
            {
                curPlan.xTime = float.Parse(arrayTemp[6]);
            }
            curPlan.DefaultValue = arrayTemp[2] == "是" ? 1 : 0;
            //确定检定项包含哪些详细数据,由需求决定
            ResultNames = new string[] { "测试时间", "功率方向", "试验电压", "标准试验时间(一个脉冲)", "试验电流", "开始时间", "结束时间", "实际运行时间", "脉冲数", "误差1", "误差2", "结论", "不合格原因" };

            return true;
        }
        /// <summary>
        /// 项目总结论主键
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)CLDC_Comm.Enum.Cus_MeterResultPrjID.起动试验);
            }
        }

        private StPlan_QiDong curPlan = new StPlan_QiDong();

        /// <summary>
        /// 项目主键值,起动试验只记录当前功率方向结论。不记录总结论
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}"
                , ResultKey
                , ((int)PowerFangXiang).ToString());
            }
        }

        /// <summary>
        /// 清理检定数据,计算检定时间和起动电流
        /// </summary>
        protected override void DefaultItemData()
        {
            //结论数据更新
            string[] arrStrResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);

                /*
                 Rem:
                 * 不检定的表也要把节点挂上，不然后面再选择要检会出错。
                 */
                /*添加结论节点*/
                MeterQdQid _MeterQdQid = null;
                if (_Meter.MeterQdQids.ContainsKey(ItemKey))
                {
                    _Meter.MeterQdQids.Remove(ItemKey);                             //加个保险
                }
                _MeterQdQid = new MeterQdQid();
                _Meter.MeterQdQids.Add(ItemKey, _MeterQdQid);
                _MeterQdQid.Mqd_chrDL = curPlan.FloatxIb.ToString();                 //起动电流
                _MeterQdQid._intMyId = _Meter._intMyId;                        //表唯一标识号
                _MeterQdQid.Mqd_chrProjectNo = ItemKey;                                     //项目ID
                _MeterQdQid.Mqd_chrProjectName = curPlan.ToString();                       //项目名称
                _MeterQdQid.Mqd_chrJdfx = (Convert.ToInt32(curPlan.PowerFangXiang)).ToString();
                _MeterQdQid.Mqd_chrJL = Variable.CTG_DEFAULTRESULT;            //默认
                _MeterQdQid.AVR_STANDARD_TIME = arrStartTimes[i].ToString();
                _MeterQdQid.AVR_ACTUAL_TIME = (arrStartTimes[i] - 4).ToString();
                _MeterQdQid.Mqd_chrTime = (arrStartTimes[i] - 4).ToString();                 //默认时间为标准计算时间  
                arrStrResultKey[i] = ItemKey;
                objResultValue[i] = _MeterQdQid;
                //if (Stop) return;
            }

            //刷新UI
            base.DefaultItemData();
        }

        /// <summary>
        /// 初始化检定参数
        /// </summary>
        /// <returns>起动时间</returns>
        private float InitVerifyPara()
        {
            string[] arrayErrorPara = VerifyPara.Split('|');
            if (arrayErrorPara[3] == "否")
            {
                //  curPlan.FloatxIb = 1F;
                curPlan.FloatxIb = float.Parse(arrayErrorPara[4]);
            }
            if (arrayErrorPara[5] == "否")
            {
                curPlan.xTime = float.Parse(arrayErrorPara[6]);
                curPlan.CheckTime = float.Parse(arrayErrorPara[6]);

            }


            //检测系统参照规程
            //计算每一块表的起动时间
            int[] _MeterConst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
            arrStartTimes = new float[BwCount];
            arrStartCurrents = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                //计算起动电流
                MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (_Meter == null || !_Meter.YaoJianYn)
                {
                    continue;
                }

                bool bFind = false;

                for (int j = i - 1; j >= 0; j--)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                    if (_Meter.Mb_chrBcs == Helper.MeterDataHelper.Instance.Meter(j).Mb_chrBcs && _Meter.Mb_chrBdj == Helper.MeterDataHelper.Instance.Meter(j).Mb_chrBdj)
                    {
                        arrStartTimes[i] = arrStartTimes[j];
                        arrStartCurrents[i] = arrStartCurrents[j];
                        bFind = true;
                        break;
                    }
                    if (Stop) return 0F;
                }

                if (!bFind)
                {
                    StPlan_QiDong _tagQiDong = (StPlan_QiDong)curPlan;
                
                    _tagQiDong.CheckTimeAndIb(_Meter.GuiChengName,
                                              GlobalUnit.Clfs,
                                              GlobalUnit.U,
                                              _Meter.Mb_chrIb,
                                              _Meter.Mb_chrBdj,
                                              _Meter.Mb_chrBcs,
                                              _Meter.Mb_BlnZnq,
                                              _Meter.Mb_BlnHgq);
                    arrStartTimes[i] = (float)Math.Round(_tagQiDong.CheckTime, 2);
                    arrStartCurrents[i] = _tagQiDong.FloatIb;
                    /*
                    如果同一批平存在不同起动电流，则起动电流取最大值
                    */
                    if (_tagQiDong.FloatIb > startCurrent)
                    {
                        startCurrent = _tagQiDong.FloatIb;
                    }
                }
            }

            float[] arrStartTimeClone = (float[])arrStartTimes.Clone();
            CLDC_DataCore.Function.Number.PopDesc(ref arrStartTimeClone, false);                        //选择一个最大起动时间
            MessageController.Instance.AddMessage("清理检定数据完毕");
            if (GlobalUnit.IsDemo)
                return 1F;
            else
                return arrStartTimeClone[0];
        }

        #region ----------开始检定---------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
           
            string[] PulseTime = new string[BwCount];                   //记录开始起动时间
            int[] PulseCount = new int[BwCount];                        //脉冲计数
            CheckOver = new bool[BwCount];
            float TotalTime = InitVerifyPara();                         //初始参数
            float _MaxStartTime = TotalTime * 60F;                      //计算最大起动时间
            StartTimeAffter = new float[BwCount];
            StartTimeBefore = new float[BwCount];
            base.Verify();
            string[] verPlan = VerifyPara.Split('|');
            PowerOn();
            if (verPlan[0] != "0")
            {
                int TIME = int.Parse(verPlan[0]) * 60;
                MessageController.Instance.AddMessage("开始预热，请等待" + TIME + "秒");
                if (Stop) return;
               

                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * TIME);

            }//记录当前检定ID
            GlobalUnit.g_CurTestType = 3;
            m_StartTime = DateTime.Now;

            float[] TestTime = new float[BwCount];
            float[] TimesD = new float[BwCount];
            //默认合格情况不作检定

            //上报检定参数
            //string dataValueKey = ItemKey;                                                  //检定数据节点名称:P_检定ID
            //string[] arrStrResultKey = new string[BwCount];
            //object[] arrObjResultValue = new object[BwCount];
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
                    if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn) continue;
                    ResultDictionary["结论"][Num] = Variable.CTG_HeGe;
                    ResultDictionary["试验电流"][Num] = startCurrent.ToString("F2");
                    ResultDictionary["开始时间"][Num] = m_StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    ResultDictionary["功率方向"][Num] = curPlan.PowerFangXiang.ToString();
                    ResultDictionary["试验电压"][Num] = GlobalUnit.U.ToString("F2");
                    ResultDictionary["实际运行时间"][Num] = (VerifyPassTime / 60.0).ToString("F4") + "分";
                    ResultDictionary["结束时间"][Num] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    
                }
                ConvertTestResult("标准试验时间(一个脉冲)", arrStartTimes, 2);

                UploadTestResult("结束时间");
                Thread.Sleep(1000);
                UploadTestResult("试验电压");
                Thread.Sleep(1000);
                UploadTestResult("试验电流");
                Thread.Sleep(1000);
                UploadTestResult("开始时间");
                Thread.Sleep(1000);
                UploadTestResult("功率方向");
                Thread.Sleep(1000);
                UploadTestResult("标准试验时间(一个脉冲)");
                Thread.Sleep(1000);
                UploadTestResult("实际运行时间");
                Thread.Sleep(1000);
                UploadTestResult("结论");
            }
            else
            {
                #region -----------开始启动-----------
                //第一步，对色标
                if (!GlobalUnit.IsDemo)
                {
                    //对色标
                    //PulseChannelDetection pulseDetect = new PulseChannelDetection(null);
                    //pulseDetect.ParentControl = this;
                    //if (!pulseDetect.DuiSheBiao(curPlan.PowerFangXiang, CLDC_DataCore.Const.GlobalUnit.Ib * 0.1F, 1))
                    //{
                    //    return;
                    //}
                    if (Stop)
                    {
                        return;
                    }
                    //设置功能参数
                    int[] startTimes = new int[BwCount];
                    for (int bw = 0; bw < BwCount; bw++)
                    {
                        startTimes[bw] = (int)(arrStartTimes[bw] * 60F);
                    }
                    //输出启动电压电流
                    if (!Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, startCurrent, (int)Cus_PowerYuanJian.H, (int)curPlan.PowerFangXiang, FangXiangStr + "1.0", IsYouGong, false))
                        return;
                  int[] meterconst = Helper.MeterDataHelper.Instance.MeterConst(IsYouGong);
                  if (GlobalUnit.clfs == Cus_Clfs.单相)
                  {
                      if (curPlan.PowerFangXiang == Cus_PowerFangXiang.正向无功 || curPlan.PowerFangXiang == Cus_PowerFangXiang.反向无功)
                      {
                          MeterProtocolAdapter.Instance.SetPulseCom(3);
                      }
                  }


                    if (!Helper.EquipHelper.Instance.InitPara_Start(curPlan.PowerFangXiang, startCurrent, IsYouGong, startTimes,meterconst))
                    {
                        Helper.EquipHelper.Instance.InitPara_Start(curPlan.PowerFangXiang, startCurrent, IsYouGong, startTimes,meterconst);
                        //MessageController.Instance.AddMessage("初始化启动设备参数失败!",false , Cus_MessageType.提示消息); 
                        //return;
                    }
                    if (Stop) return;
                }

                m_CheckOver = false;
                MessageController.Instance.AddMessage("检定中...");
                m_StartTime = DateTime.Now;

                #region 上报试验参数
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        ResultDictionary["试验电流"][i] = startCurrent.ToString("F2");
                        ResultDictionary["开始时间"][i] = m_StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                        ResultDictionary["功率方向"][i] = curPlan.PowerFangXiang.ToString();
                        ResultDictionary["试验电压"][i] = GlobalUnit.U.ToString("F2");
                        TestTime[i] = (float)(arrStartTimes[i] * 4.5 / 100);
                        TimesD[i] = arrStartTimes[i] / 60F;
                    }
                }

                ConvertTestResult("标准试验时间(一个脉冲)", TimesD, 2);

                UploadTestResult("试验电压");
                Thread.Sleep(1000);
                UploadTestResult("试验电流");
                Thread.Sleep(1000);
                UploadTestResult("开始时间");
                Thread.Sleep(1000);
                UploadTestResult("功率方向");
                Thread.Sleep(1000);
                UploadTestResult("标准试验时间(一个脉冲)");
                #endregion

                while (true)
                {

                    /*减少硬件负担，前半段时间读取频率为1次，后30%段以5秒/次的频率读取，后20%以一秒一次的频率*/
                    //每一秒刷新一次数据
                    long _PastTime = base.VerifyPassTime;
                    Thread.Sleep(1000);
                    m_CheckOver = true;
                    if (!GlobalUnit.IsDemo)
                    {
                        ReadAndDealData(_PastTime);
                    }
                    else
                    {
                        m_CheckOver = false;
                    }

                    if (Stop)
                    {
                        return;
                    }

                    float pastMinute = _PastTime / 60F;
                    GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = "启(起)动时间" + (TotalTime * 4.5 / 60).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";
                    if (Helper.MeterDataHelper.Instance.TypeCount > 1)
                    {
                        strDes += ",由于是多种表混检，大常数表可能提前出脉冲";
                    }
                    MessageController.Instance.AddMessage(strDes);

                    if (Stop || m_CheckOver)
                    {
                        GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }
                }

                #endregion

            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    ResultDictionary["结论"][i] = ResultDictionary["结论"][i] == "合格" ? "合格" : "不合格";
                    if (ResultDictionary["结论"][i] == "不合格")
                    {
                        ResultDictionary["结束时间"][i] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            UploadTestResult("结束时间");
            Thread.Sleep(1000);
            UploadTestResult("结论");

            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false, 3);
            MessageController.Instance.AddMessage("当前试验项目完毕");
        }

        /// <summary>
        /// 读取并处理检定数据
        /// </summary>
        private void ReadAndDealData(long verifyTime)
        {

              //  CLDC_DeviceDriver.stError[] arrTagData = Helper.EquipHelper.Instance.ReadWcb(true);
            m_CheckOver = true;
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    CheckOver[k] = true;
                    continue;
                }
                int num = 0;
                string data = string.Empty;
                string time = string.Empty;

                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(k + 1, 3, out num, out data, out time);
                CLDC_DeviceDriver.stError arrTagData = Helper.EquipHelper.Instance.ReadWc(k + 1);
                if (verifyTime <= arrStartTimes[k] * 4.5 && CheckOver[k] == false)
                {
                    ResultDictionary["脉冲数"][k] = data;
                    if (data != null && time != null && data != "" && time != "")
                    {
                        if (data == "2")
                        {
                            StartTimeBefore[k] = float.Parse(time);
                            ResultDictionary["误差1"][k] = arrTagData.szError;
                        }
                        else if (data == "3")
                        {
                            StartTimeAffter[k] = float.Parse(time);
                            ResultDictionary["误差2"][k] = arrTagData.szError;
                            CheckOver[k] = true;
                        }


                        if (StartTimeBefore[k] != 0 && StartTimeAffter[k] != 0 && arrStartTimes[k] * 1.5 > StartTimeBefore[k] && arrStartTimes[k] * 1.5 > StartTimeAffter[k])
                        {
                            ResultDictionary["结论"][k] = Variable.CTG_HeGe;
                            ResultDictionary["结束时间"][k] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            ResultDictionary["实际运行时间"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "分";
                        }
                        else
                        {
                            ResultDictionary["结论"][k] = "";
                            ResultDictionary["实际运行时间"][k] = (((float)verifyTime) / 60.0).ToString("F4") + "分";
                            CheckOver[k] = false;
                        }
                    }

                    if (!CheckOver[k])
                    {
                        m_CheckOver = false;
                    }
                }
                if (Stop) break;
            }

            UploadTestResult("结束时间");
            Thread.Sleep(1000);
            UploadTestResult("实际运行时间");
            Thread.Sleep(1000);
            UploadTestResult("脉冲数");
            Thread.Sleep(1000);
            UploadTestResult("误差1");
            Thread.Sleep(1000);
            UploadTestResult("误差2");
            Thread.Sleep(1000);
        }

        #endregion




    }
}
