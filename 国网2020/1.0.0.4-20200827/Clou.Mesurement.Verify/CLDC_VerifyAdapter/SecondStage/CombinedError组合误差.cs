using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using System.Threading;

namespace CLDC_VerifyAdapter.SecondStage
{
    class CombinedError : VerifyBase
    {
        public CombinedError(object plan)
            : base(plan)
        {



        }
        protected override bool CheckPara()
        {

            ResultNames = new string[] { "试验前费率电量", "试验前总电量", "试验后费率电量", "试验后总电量", "误差", "结论", "不合格原因" };
            return true;
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
            Dictionary<int, float[]> dicEnergy = new Dictionary<int, float[]>();
            Stop = false;
            int _MaxTestTime = 60;
            string stroutmessage = "";
            bool bResult = PowerOn();

            base.Verify();

            string curSD = string.Empty;
            string[] arrPara = VerifyPara.Split('|');
            string[] arrSD = arrPara[0].Split(',');
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

            PowerFangXiang = Cus_PowerFangXiang.正向有功; //getCurPowerFangXiang();//当前检定功率方向
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            //对每一个时段进行试验
            for (int k = 0; k < arrSD.Length; k++)
            {
                curSD = arrSD[k];
                string[] para = curSD.Split('(');
                if (para.Length != 2)
                    continue;
                MessageController.Instance.AddMessage("正在做" + curSD);

                string sdTime = DateTime.Parse(para[0]).ToString("yyMMddHHmmss");


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


                if (Stop)
                    break;

                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((int)Cus_PowerFangXiang.正向有功, 0);

                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //把方案费率编号转换到被检表费率顺序号
                    int feilvOrder = SwitchFeiLvNameToOrder(para[1].Replace(")", ""), iNum);
                    if (!curMeter.YaoJianYn) continue;
                    if (dicEnergy.ContainsKey(iNum) == false) continue;

                    #region----------总检定结果----------


                    #endregion


                    tagRatePeriodData[iNum].BW = iNum + 1;
                    tagRatePeriodData[iNum].fStartDLPeriod = dicEnergy[iNum][Convert.ToInt32(curTri)];
                    tagRatePeriodData[iNum].fStartDLSum = dicEnergy[iNum][0];
                    tagRatePeriodData[iNum].FL = curTri.ToString();
                    ResultDictionary["试验前费率电量"][iNum] = ResultDictionary["试验前费率电量"][iNum] + "|" + tagRatePeriodData[iNum].fStartDLPeriod + "(" + curTri.ToString() + ")";
                    ResultDictionary["试验前总电量"][iNum] = ResultDictionary["试验前总电量"][iNum] + "|" + tagRatePeriodData[iNum].fStartDLSum + "(总)";
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
                    Thread.Sleep(1000);
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

                dicEnergy = MeterProtocolAdapter.Instance.ReadEnergys((int)Cus_PowerFangXiang.正向有功, 0);

                for (int iNum = 0; iNum < BwCount; iNum++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(iNum);
                    //把方案费率编号转换到被检表费率顺序号
                    int feilvOrder = SwitchFeiLvNameToOrder(para[1].Replace(")", ""), iNum);
                    if (!curMeter.YaoJianYn) continue;
                    if (dicEnergy.ContainsKey(iNum) == false) continue;

                    tagRatePeriodData[iNum].fEndDLPeriod = dicEnergy[iNum][Convert.ToInt32(curTri)];
                    tagRatePeriodData[iNum].fEndDLSum = dicEnergy[iNum][0];
                    tagRatePeriodData[iNum].FL = curTri.ToString();
                    ResultDictionary["试验后费率电量"][iNum] = ResultDictionary["试验后费率电量"][iNum] + "|" + tagRatePeriodData[iNum].fEndDLPeriod + "(" + curTri.ToString() + ")";
                    ResultDictionary["试验后总电量"][iNum] = ResultDictionary["试验后总电量"][iNum] + "|" + tagRatePeriodData[iNum].fEndDLSum + "(总)";
                    ResultDictionary["结论"][iNum] = "合格";
                    if (dicEnergy[iNum][0] <= 0)
                    {
                        ResultDictionary["结论"][iNum] = "不合格";
                        ResultDictionary["误差"][iNum] = "-999";
                        ResultDictionary["不合格原因"][iNum] = "读取不了电量";

                    }
                    else
                    {
                        float fErr = Convert.ToSingle(tagRatePeriodData[iNum].Error());
                        ResultDictionary["误差"][iNum] = Math.Abs(fErr).ToString();

                        if (Math.Abs(fErr) > 0.01)
                        {


                            ResultDictionary["结论"][iNum] = "不合格";
                            ResultDictionary["不合格原因"][iNum] = "误差超差";
                        }
                    }



                    //读取电量完毕，刷新一次数据


                }
                //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrCurKey);
            }
            UploadTestResult("试验后费率电量");
            UploadTestResult("试验后总电量");
            UploadTestResult("误差");
            UploadTestResult("结论");
            UploadTestResult("不合格原因");
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            string sdTime1 = DateTime.Now.ToString("yyMMddHHmmss");


            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = sdTime1.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += sdTime1.Substring(6, 6);
                strShowData[i] = sdTime1;
                strData[i] = strCode[i] + strSetData[i];
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
            //  GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);

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
        #endregion
    }
}

