using System;
using CLDC_DataCore;
using System.Data;
using CLDC_Comm;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Function;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using System.Collections.Generic;
using System.Threading;

namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 电能示值的组合误差
    /// </summary>
    class IndicateCombinateError: VerifyBase
    {
        public IndicateCombinateError(object plan)
            : base(plan)
               
        {
            ResultNames = new string[] { "试验前费率电量", "试验前总电量", "试验后费率电量", "试验后总电量","误差", "结论" };
          
        
        
        }
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

        public override void Verify()
        {
            float[] flt_DLFlq =new  float[BwCount];
            float[] flt_DLY = new float[BwCount];
            float[] flt_DLFlh = new float[BwCount];
            float[] flt_DLYh = new float[BwCount];
            Stop = false;
            int _MaxTestTime = 60;
            string stroutmessage = "";
            bool bResult = PowerOn();
            Check.Require(bResult, "控制源输出失败");
            base.Verify();

            string curSD = string.Empty;
            string[] arrPara = VerifyPara.Split('|');
            string[] arrSD = arrPara[0].Split(',');
            _MaxTestTime = int.Parse(arrPara[1]) * _MaxTestTime;
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
            string[] arrStrResultKey = new string[BwCount];
            string[] arrSumKey = new string[BwCount];//当前分项上报key
            string[] arrCurKey = new string[BwCount];//当前子项的上报key
            string totalKey = ResultKey;// CurPlan.DgnPrjID;//((int)Cus_DgnItem.费率时段示值误差).ToString("D3");//总结论节点主键
            PowerFangXiang = Cus_PowerFangXiang.正向有功; //getCurPowerFangXiang();//当前检定功率方向
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Thread.Sleep(1000);
            //对每一个时段进行试验
            for (int k = 0; k < arrSD.Length; k++)
            {
                curSD = arrSD[k];
                string[] para = curSD.Split('(');
                if (para.Length != 2)
                    continue;
                MessageController.Instance.AddMessage("正在做" + curSD );

                string sdTime = DateTime.Parse(para[0]).ToString("yyMMddHHmmss");
                //Comm.MessageController.Instance.AddMessage("开始" + sdTime, false);
                //System.Windows.Forms.MessageBox.Show("请确认打开电表编程键", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                //设置时间
                Thread.Sleep(1000);
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = sdTime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += sdTime.Substring(6, 6);
                    strShowData[i] = sdTime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                //result = MeterProtocolAdapter.Instance.WriteDateTime(sdTime);
                bResult = true;
                for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
                {
                    if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                    {
                        if (!result[i])
                            bResult = false;
                    }
                }
                Thread.Sleep(3000);
                //费率
                Cus_FeiLv curTri = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), para[1].Replace(")", ""));
                /*
                    09/25/2009 12-12-45  By Niaoked
                    内容说明：
                    注意电量读取组件的位置，处理事件冲突
                */


              
                MeterBasicInfo curMeter;
                stRatePeriodData[] tagRatePeriodData = new stRatePeriodData[BwCount];


                MessageController.Instance.AddMessage("试验中...");

                string strFX = "00";
                string StrDNZ = "00000000";
                switch (PowerFangXiang)
                {
                    case Cus_PowerFangXiang.正向有功:
                        strFX = "01";
                        StrDNZ = "00010000";
                        break;
                    case Cus_PowerFangXiang.反向有功:
                        strFX = "02";
                        StrDNZ = "00020000";
                            break;
                    case Cus_PowerFangXiang.正向无功:
                        strFX = "03";
                        StrDNZ = "00030000";
                        break;
                    case Cus_PowerFangXiang.反向无功:
                        strFX = "04";
                        StrDNZ = "00040000";
                        break;
                }
                string StrFL = "";

                switch (curTri)
                {
                    case Cus_FeiLv.尖:
                        StrFL = "01";
                        break;
                    case Cus_FeiLv.峰:
                        StrFL = "02";
                        break;
                    case Cus_FeiLv.平:
                        StrFL = "03";
                        break;
                    case Cus_FeiLv.谷:
                        StrFL = "04";
                        break;
                }

                string StrFLDN = "00" + strFX + StrFL + "00";
                if (Stop)
                    break;
                MessageController.Instance.AddMessage("读取试验前电量");
                flt_DLFlq = MeterProtocolAdapter.Instance.ReadData(StrFLDN, 4, 2);
                flt_DLY = MeterProtocolAdapter.Instance.ReadData(StrDNZ, 4, 2);
               
                Thread.Sleep(1000);
                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //把方案费率编号转换到被检表费率顺序号
                  
                    if (!curMeter.YaoJianYn) continue;
                 
                 
                    #region----------总检定结果----------
                  
                 
                    #endregion



                    ResultDictionary["试验前费率电量"][iNum] = flt_DLFlq[iNum].ToString() + "(" + curTri.ToString() + ")";
                    ResultDictionary["试验前总电量"][iNum] = flt_DLY[iNum].ToString() + "(总)";
                         //读取电量完毕，刷新一次数据


                }
                UploadTestResult("试验前费率电量");
                UploadTestResult("试验前总电量");
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, "1", true, false) == false)
                {
                    MessageController.Instance.AddMessage("控制源输出失败");
                    return;
                }
                Thread.Sleep(300);
                DateTime startTime = DateTime.Now;

                m_CheckOver = false;
                while (true)
                {
                    Thread.Sleep(5000);
                    if (Stop) return;
                    int _PastTime = DateTimes.DateDiff(startTime);
                    //处理跨天
                    if (_PastTime < 0) _PastTime += 43200;      //如果当前PC为12小时制
                    if (_PastTime < 0) _PastTime += 43200;      //24小时制
                    if (_PastTime < 0)
                    {
                        //处理人为修改时间
                        //跨度超过24小时，肯定是系统日期被修改
                        MessageController.Instance.AddMessage("系统时间被人为修改超过24小时，检定停止");
                        Stop = true;
                        return;
                    }


                    if (_PastTime >= _MaxTestTime)
                    {
                        m_CheckOver = true;
                    }
                    GlobalUnit.g_CUS.DnbData.NowMinute = _PastTime / 60F;
                    stroutmessage = string.Format("设置当前费率运行：{0}分，已经走字：{1}分", _MaxTestTime / 60, Math.Round(GlobalUnit.g_CUS.DnbData.NowMinute, 2));

                    MessageController.Instance.AddMessage(stroutmessage);
                    if (m_CheckOver)
                    {
                        break;
                    }
                }
                if (Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)Cus_PowerYuanJian.H, (int)Cus_PowerFangXiang.正向有功, "1", true, false) == false)
                {
                    MessageController.Instance.AddMessage("控制源输出失败");
                    return;
                }
                Thread.Sleep(5000);
                MessageController.Instance.AddMessage("读取试验后电量");
                flt_DLFlh = MeterProtocolAdapter.Instance.ReadData(StrFLDN, 4, 2);
                flt_DLYh = MeterProtocolAdapter.Instance.ReadData(StrDNZ, 4, 2);

                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //把方案费率编号转换到被检表费率顺序号
                   
                    if (!curMeter.YaoJianYn) continue;



                    ResultDictionary["试验后费率电量"][iNum] = flt_DLFlh[iNum].ToString() + "(" + curTri.ToString() + ")";
                    ResultDictionary["试验后总电量"][iNum] = flt_DLYh[iNum].ToString() + "(总)";

                    float fErr = -999;
                    if (flt_DLFlh[iNum] == -1 || flt_DLYh[iNum] == -1 || flt_DLY[iNum] == -1 || flt_DLYh[iNum] == -1)
                    {
                        fErr = -999;
                    }
                    else if ((flt_DLFlh[iNum] - flt_DLFlq[iNum]) == 0)
                    {
                        fErr = 1;
                    }
                    else
                    {
                        fErr = (flt_DLFlh[iNum] - flt_DLFlq[iNum]) - (flt_DLYh[iNum] - flt_DLY[iNum]);
                    }

                 
                    ResultDictionary["误差"][iNum] =  Math.Abs(fErr).ToString("0.0000") ;
                    ResultDictionary["结论"][iNum] = "合格";
                    if (Math.Abs(fErr) > 0.01)
                        ResultDictionary["结论"][iNum] = "不合格";
                    //读取电量完毕，刷新一次数据


                }
                //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrCurKey);
            }
            UploadTestResult("试验后费率电量");
            UploadTestResult("试验后总电量");
            UploadTestResult("误差");
            UploadTestResult("结论");
            //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
            MessageController.Instance.AddMessage("正在写入当前时间");
            MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
        }
      
        /// <summary>
        /// 检定参数校验
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {

            //
            // Check.Require(PrjPara.Length > 0, "检定方案中没有设置被检表时段，请到检定方案中设置");
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



        #region 费率电能示值误差结构体
        struct stRatePeriodData
        {
            public int BW;              //表位号            
            public string FL;           //费率
            public float fStartDLPeriod;       //分费率起始电量
            public float fEndDLPeriod;         //分费率结束电量
            public float fStartDLSum;          //总起始电量
            public float fEndDLSum;            //总结束
            public override string ToString()
            {
                string str = string.Format("{0}|{1}|{2}|{3}|{4}|{5}", fStartDLPeriod, fEndDLPeriod, fStartDLSum, fEndDLSum, Error(), FL);

                return str;
            }
            /// <summary>
            /// 费率电量示值误差
            /// </summary>
            /// <returns></returns>
            public string Error()
            {
                float fError;
                float fErrorPeriod;
                float fErrorSum;

                try
                {
                    fErrorPeriod = fEndDLPeriod - fStartDLPeriod;
                    fErrorSum = fEndDLSum - fStartDLSum;
                    //add by zxr in then nanchang 2014-08-26 处理差值为0的情况。
                    if (fErrorSum == 0.0f || fErrorPeriod == 0.0f)
                    {
                        fError = 0.000f;
                    }
                    else
                    {
                        fError = (fErrorPeriod - fErrorSum) / fErrorSum;
                    }
                }
                catch
                {
                    return "";
                }
                return fError.ToString("0.000");
            }
        }
        /// <summary>
        /// 费率名称转化到费率编号
        /// </summary>
        /// <param name="FeiLvName"></param>
        /// <param name="bwIndex"></param>
        /// <returns>被检表的第几个费率</returns>
        protected int SwitchFeiLvNameToOrder(string FeiLvName, int bwIndex)
        {
            if (FeiLvName == "总") return 0;
            if (FeiLvName.Length > 1) return 0;
            string[] arrSourceFL = new string[] { "总", "尖", "峰", "平", "谷" };
            int sourceOrder = 0;
            for (int j = 0; j < arrSourceFL.Length; j++)
            {
                sourceOrder = j;
                if (arrSourceFL[j].Equals(FeiLvName))
                {
                    break;
                }
            }
            //  sourceOrder++;
            return sourceOrder;

            /*
           int returnOrder = 0;
            string meterFeiLvOrder = Helper.MeterDataHelper.Instance.Meter(bwIndex).DgnProtocol.TariffOrderType;
            for (int k = 0; k < meterFeiLvOrder.Length; k++)
            {
                returnOrder = k;
                if (meterFeiLvOrder.Substring(k, 1) == sourceOrder.ToString())
                {
                    break;
                }
            }
            return returnOrder;
        */

        }
        #endregion
    }
}
