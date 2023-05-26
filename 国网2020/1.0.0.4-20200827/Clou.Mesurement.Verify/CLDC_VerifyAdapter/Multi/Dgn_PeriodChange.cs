
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;



namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 时段投切试验
    /// 【注意】显示界面中显示的电量值挂接在读取电量主键中
    /// 新增加读取误差板时段投切脉冲功能，电量或是脉冲二者满足之一则合格
    /// </summary>
    class Dgn_PeriodChange : DgnBase
    {
        /// <summary>
        /// 获取自动时段投切状态
        /// true: 自动时段投切 false: 手动
        /// </summary>
        /// <returns></returns>
        private bool GetAutoPeriodChangeStatus()
        {
            if (PrjPara == null)
            {
                return false;
            }
            string[] arrSD = PrjPara;
            string str = arrSD[arrSD.Length - 1];
            if (str == "0")
                return false;
            if (str == "1")
                return true;
            return false;
        }
        /// <summary>
        /// 获取时段投切数据
        /// </summary>
        /// <returns></returns>
        private string[] GetPeriodChangePara()
        {
            string[] Para = new string[1];
            Para[0] = "";
            if (PrjPara == null)
            {
                return Para;
            }
            if (PrjPara.Length < 2)
            {
                return Para;
            }
            int sdLength = sdLength = PrjPara.Length - 1; ;
            if (PrjPara[PrjPara.Length - 1].Length == 4)
            {
                sdLength = PrjPara.Length - 2;
            }

            string[] str = new string[sdLength];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = PrjPara[i];
            }
            return str;
        }
        public Dgn_PeriodChange(object plan) : base(plan) { }
        public override void Verify()
        {
            Stop = false;
            bool bResult = PowerOn();//Adapter.ComAdpater.PowerOnOnlyU();
            Check.Require(bResult, "控制源输出失败");
            base.Verify();

            string[] arrStrResultKey = new string[BwCount];
            string[] arrSumKey = new string[BwCount];//当前分项上报key
            string[] arrCurKey = new string[BwCount];//当前子项的上报key
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] strMeterTime = new string[BwCount];
            string[] strShowData = new string[BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string curSD = string.Empty;

            string[] arrSD = GetPeriodChangePara();
            //Adapter.ComAdpater.SetTestPoint(CLDC_DataCore.Const.GlobalUnit.U, 0, "1.0",Cus_PowerYuanJian.H, true);
            //PowerOn(true, PowerFangXiang);
            if (Stop)
            {
                return;
            }
            string totalKey = CurPlan.DgnPrjID;//((int)Cus_DgnItem.时段投切).ToString("D3");//总结论节点主键
            PowerFangXiang = getCurPowerFangXiang();//当前检定功率方向

            ShowWirteMeterWwaring();
            //对每一个时段进行试验
            for (int k = 0; k < arrSD.Length; k++)
            {
                if (Stop)
                {
                    return;
                }
                curSD = arrSD[k];
                string[] para = curSD.Split('(');
                if (para.Length != 2)
                    continue;
                MessageController.Instance.AddMessage("正在做" + curSD + CurPlan.ToString());

                string sdTime = DateTime.Parse(para[0]).ToString("yyMMddHHmmss");
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = sdTime.Substring(0,6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += sdTime.Substring(6, 6);
                    strShowData[i] = sdTime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

                //设置时间
              //  bool[] result = MeterProtocolAdapter.Instance.WriteDateTime(sdTime);
                bResult = true;
                for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
                {
                    if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                    {
                        if (!bln_Rst[i])
                        {
                           
                                bResult = false;
                            
                        }
                    }
                }
                if (!bResult)
                //if (!Helper.Rs485Helper.Instance.WriteDateTime(sdTime))
                {
                    //GlobalUnit.ForceVerifyStop = true;
                    MessageController.Instance.AddMessage("有电能表写表时间失败!请检查多功能协议配置、表接线或是表编程开关是否已经打开。\r\n当前检定中止", 6, 2);
                    //return;
                }
                //费率
                Cus_FeiLv curTri = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), para[1].Replace(")", ""));
                //enmTariffType curTri = (enmTariffType)Enum.Parse(typeof(enmTariffType), para[1].Replace(")", ""));
                /*
                	09/25/2009 12-12-45  By Niaoked
                	内容说明：
                	注意电量读取组件的位置，处理事件冲突
                */
                //Dgn_ReadEnerfy readEnergyControl = new Dgn_ReadEnerfy(null);
                
                string curItemKey = totalKey + ((int)(k + 1)).ToString("D2");//当前项目节点主键
                
                MeterBasicInfo curMeter;
                stSDTQData[] tagTQData = new stSDTQData[BwCount];
                //ClearItemData(curItemKey);
                
                MessageController.Instance.AddMessage("试验中...");

                //if (!bPoserOn)
                m_StartTime = DateTime.Now;
                //DateTime startTQTime = DateTime.Now;

                // Reset();
                //string strReadEnergyPara = "11111|00000000";
                //char[] chrReadEnergy = strReadEnergyPara.ToCharArray();
                //MessageController.Instance.AddMessage(DefaultPD.ToString());
                //chrReadEnergy[6 + (int)DefaultPD] = '1';
                //strReadEnergyPara = new string(chrReadEnergy);
                //string[] arrReadEnergyPara = strReadEnergyPara.Split('|');
                if (Stop)
                {
                    return;
                }
                Helper.MeterDataHelper.Instance.ClearDgnData(curItemKey, false);
                //m_DataManage.ClearDgnData(curItemKey, false);
                m_CheckOver = false;
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, _xIb * GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, "1", true, false) == false)//
                {
                    MessageController.Instance.AddMessage("控制源输出失败");
                    return;
                }
                if (Stop)
                {
                    return;
                }
                MessageController.Instance.AddMessage("正在做" + curSD + CurPlan.ToString());
                while (true)
                {
                    Thread.Sleep(100);
                    if (m_CheckOver)
                        break;
                    if (Stop)
                        break;
                    // if (!ReadEnergy(DefaultPD, curTri))
                    //     continue;

                    float[] energyValue = MeterProtocolAdapter.Instance.ReadEnergy((byte)((int)PowerFangXiang - 1), (byte)curTri);


                    //m_CheckOver = true;
                    // Reset();
                    for (int iNum = 0; iNum < BwCount; iNum++)
                    {
                        curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                        //把方案费率编号转换到被检表费率顺序号
                        int feilvOrder = SwitchFeiLvNameToOrder(para[1].Replace(")", ""), iNum);
                        if (!curMeter.YaoJianYn) continue;
                        if (tagTQData[iNum].tqTime != null && tagTQData[iNum].tqTime.Length > 10)
                            continue;

                        MeterDgn _Result;

                        #region----------总检定结果----------
                        MeterDgn _ResultZhong;
                        if (!curMeter.MeterDgns.ContainsKey(totalKey))
                        {
                            _ResultZhong = new MeterDgn();
                            curMeter.MeterDgns.Add(totalKey, _ResultZhong);
                            _ResultZhong.Md_chrValue = Variable.CTG_HeGe;
                            _ResultZhong.Md_PrjID = totalKey;
                            _ResultZhong.Md_PrjName = CurPlan.DgnPrjName;
                        }
                        else
                        {
                            _ResultZhong = curMeter.MeterDgns[totalKey];
                        }
                        #endregion

                        //if (!dicEnergy.ContainsKey(iNum))
                        if (energyValue[iNum] == -1) //-1表示没有读到
                            continue;
                        //if (! CurReturnDataA.ContainsKey(iNum.ToString()))
                        //{
                        //    continue;
                        //}



                        //float[] CurDL = null;// dicEnergy[iNum];

                        //是否已经记录了起码
                        if (!tagTQData[iNum].ReadDL)
                        {
                            if (!curMeter.MeterDgns.ContainsKey(curItemKey))
                            {
                                _Result = new MeterDgn();
                                _Result.Md_PrjID = curItemKey;
                                curMeter.MeterDgns.Add(curItemKey, _Result);
                            }
                            else
                            {
                                _Result = curMeter.MeterDgns[curItemKey];
                            }
                            tagTQData[iNum].BW = iNum + 1;
                            tagTQData[iNum].StartDL = energyValue[iNum];
                            tagTQData[iNum].ReadDL = true;
                            tagTQData[iNum].FL = curTri.ToString();
                            tagTQData[iNum].standTime = para[0];
                            tagTQData[iNum].tqTime = "";
                            tagTQData[iNum].startTime = CLDC_DataCore.Function.DateTimes.FormatStringToDateTime(sdTime);
                            _Result.Md_chrValue = tagTQData[iNum].ToString();
                            //读取电量完毕，刷新一次数据
                            
                            continue;
                        }

                        tagTQData[iNum].curDL = energyValue[iNum]; ;
                        if (GlobalUnit.IsDemo) tagTQData[iNum].curDL += 0.1F;
                        if (!tagTQData[iNum].CheckOver)
                        {

                            if (tagTQData[iNum].curDL - tagTQData[iNum].StartDL > 0)
                            {
                                tagTQData[iNum].CheckOver = true;
                                //int pastSeconds = Comm.Function.DateTimes.DateDiff(startTQTime);
                                DateTime endTime = tagTQData[iNum].startTime.AddSeconds(VerifyPassTime);
                                tagTQData[iNum].tqTime = endTime.ToString("HH:mm:ss");
                                _Result = curMeter.MeterDgns[curItemKey];
                                _Result.Md_PrjName = CurPlan.DgnPrjName;
                                _Result.Md_chrValue = tagTQData[iNum].ToString();
                                //_ResultZhong.Md_chrValue = Variable.CTG_BuHeGe;
                                //外发数据通知
                                
                            }
                        }

                        //检测 是否所有表都合格
                        m_CheckOver = true;
                        for (int intBW = 0; intBW < BwCount; intBW++)
                        {
                            if (!Helper.MeterDataHelper.Instance.Meter(intBW).YaoJianYn)
                                continue;
                            if (!tagTQData[intBW].CheckOver)
                            {
                                m_CheckOver = false;
                                break;
                            }
                        }
                        //超时检测
                        if (VerifyPassTime > 300)
                        {
                            if (!tagTQData[iNum].CheckOver)
                                _ResultZhong.Md_chrValue = Variable.CTG_BuHeGe;
                            m_CheckOver = true;
                        }
                        arrStrResultKey[k] = ItemKey;
                        arrCurKey[k] = curItemKey;
                        
                    }
                    Thread.Sleep(300);
                }
           //     GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrCurKey);
                Thread.Sleep(900);
            }

            //恢复时间
            MessageController.Instance.AddMessage("正在恢复电能表时间...");
            string RsdTime = DateTime.Now.ToString("yyMMddHHmmss");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = RsdTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += RsdTime.Substring(6, 6);
                strShowData[i] = RsdTime;
                strData[i] = strCode[i] + strSetData[i];
            }
         MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            

            //GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }

        private Cus_PowerFangXiang getCurPowerFangXiang()
        {
            Cus_PowerFangXiang _tmp = PowerFangXiang;
            if (int.Parse(CurPlan.DgnPrjID) == (int)Cus_DgnItem.反向有功时段投切)
            {
                _tmp = Cus_PowerFangXiang.反向有功;
            }
            else if (int.Parse(CurPlan.DgnPrjID) == (int)Cus_DgnItem.正向无功时段投切)
            {
                _tmp = Cus_PowerFangXiang.正向无功;
            }
            else if (int.Parse(CurPlan.DgnPrjID) == (int)Cus_DgnItem.反向无功时段投切)
            {
                _tmp = Cus_PowerFangXiang.反向无功;
            }
            return _tmp;
        }
        /// <summary>
        /// 检定参数校验
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            //
            Check.Require(PrjPara.Length > 0, "检定方案中没有设置被检表时段，请到检定方案中设置");
            //读取一下时段
#if OLD
            //对比时段是否和方案一致
            string[] strP = new string[0];
            string[] arrScheme = new string[0];
            string strTime = string.Empty;     //费率时间
            string strDesc = string.Empty;     //费率号
            string strTmp = string.Empty;      //
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    continue;
                if (!Control485.curReturnStringA.ContainsKey(i.ToString()))
                {
                    MessageController.Instance.AddMessage(string.Format("第[{0}]表位读取时段失败!", i + 1), false);
                    continue;
                }
                strP = Control485.curReturnStringA[i.ToString()];
                //对比
                for (int j = 0; j < strP.Length; j++)
                {
                    if (j > PrjPara.Length - 1) break;
                    arrScheme = PrjPara[j].Split('(');
                    Check.Require(arrScheme.Length == 2, "方案中时段格式错误");
                    //时间检测
                    strTime = strP[j].Substring(0, 4); //取时间
                    strDesc = strP[j].Substring(4, 2); //取序号
                    strTmp = arrScheme[0].Replace(":", "");    //替换掉方案中的":"
                    Check.Require(strTmp.Equals(strTime), string.Format("方案第{0}个费率时段与被检表{1}不一致", j + 1, i + 1));
                    //序号检测
                    strTmp = arrScheme[1].Replace(")", "");
                    strTmp = SwitchFeiLvNameToOrder(strTmp, i).ToString("00");
                    Check.Require(Comm.Function.Number.IsIntNumber(strTmp), "费率号必须为数字");
                    Check.Require(strTmp.Equals(strDesc), string.Format("方案第{0}个费率号与被检表{1}不一致,被检表时间为：{2}", j + 1, i + 1, strP));
                }


            }
#endif
            return true;
        }



        #region 时段投切结构体
        struct stSDTQData
        {
            public int BW;              //表位号
            public string standTime;    //标准时间
            public string tqTime;       //投切时间
            public string FL;           //费率
            public float StartDL;       //开始电量
            public float curDL;         //当前电量
            public bool ReadDL;         //是否已经读起码
            public DateTime startTime;  //开始试验时间
            public bool CheckOver;      //是否已经试验完毕
            public override string ToString()
            {
                string str = string.Format("{0}|{1}|{2}|{3}", standTime, tqTime, Error(), FL);

                return str;
            }
            /// <summary>
            /// 时段投切误差
            /// </summary>
            /// <returns></returns>
            public string Error()
            {
                int pastTime = 0;
                try
                {
                    if (tqTime == "")
                        tqTime = standTime;
                    pastTime = CLDC_DataCore.Function.DateTimes.DateDiff(DateTime.Parse(standTime), DateTime.Parse(tqTime));

                }
                catch
                {
                    return "00:00";
                }
                return CLDC_DataCore.Function.DateTimes.TimeSerial(pastTime).ToString();
            }
        }
        #endregion
    }
}
