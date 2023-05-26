
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;
using System.Data;
using CLDC_DataCore.Struct;
using CLDC_DataCore.WuChaDeal;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Multi
{
    //zhengrubin-20190920
    /// <summary>
    /// 日计时误差检定
    /// 检定方法:取10次误差计算平均值和化整值，与方案误差限进行比较
    /// </summary>
    class Dgn_ClockError : DgnBase
    {
        int maxErrorTimes = 10;               //最大误差次数
        const int maxVerifySeconds = 120;                       //最大做130秒

        DataTable errorTable = new DataTable();

        private int[] _VerifyTimes = new int[0];          //有效误差次数

        private int[] _CurrentWcNum = new int[0];         //当前累计检定次数
        private int[] _MeterWcNum = new int[0];            //表位取误差次数

        public Dgn_ClockError(object plan)
            : base(plan)
        {
            _VerifyTimes = new int[BwCount];          //有效误差次数
            _CurrentWcNum = new int[BwCount];         //当前累计检定次数
            _MeterWcNum = new int[BwCount];            //表位取误差次数
        }

        /// <summary>
        /// 检定逻辑
        /// </summary>
        public override void Verify()
        {
            //初始化结论
            ResultNames = new string[] { "误差1", "误差2", "误差3", "误差4", "误差5", "误差6", "误差7", "误差8", "误差9", "误差10", "平均值", "化整值", "结论", "不合格原因" };
            maxErrorTimes = GetWcCount();
            base.Verify();
            MeterBasicInfo _MeterInfo;
            /*开始校验*/
            string[] _CurWC = new string[BwCount];
            bool[] result = new bool[BwCount];
            int[] _CurWCTimes = new int[BwCount];
            //项目键值
            //string _Key = Plan.DgnPrjID;
            MessageController.Instance.AddMessage("开始初始化检定参数...");
            //初始化误差计算表
            InitErrorTable();
            /*当前是否已经停止校验*/

            if (Stop)
            {
                return;
            }
            m_CheckOver = false;                                  //还原检定标识

            if (!InitEquipment())
            {
                return;
            }
            //
            m_StartTime = DateTime.Now;                          //记录下开始时间
            MessageController.Instance.AddMessage("正在检定......");
            int ReadWCNum = 0;
            while (true)
            {
                Thread.Sleep(GlobalUnit.g_ThreadWaitTime);
                if (m_CheckOver)
                {
                    for (int i = 0; i < BwCount; i++)
                    {

                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (string.IsNullOrEmpty(ResultDictionary["平均值"][i]))
                            {
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["不合格原因"][i] = "不出误差";
                            }
                               
                        }

                    }

                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "平均值", ResultDictionary["平均值"]);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "化整值", ResultDictionary["化整值"]);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
                    break;
                }

                if (Stop)
                {
                    MessageController.Instance.AddMessage("检定停止，操作完毕");
                    break;
                }
                //开始读取误差板
                _CurWC = new string[BwCount];               //重新初始化本次误差
                _CurrentWcNum = new int[BwCount];           //重新初始化读取误差次数
                string[] arrLastWcValue = _CurWC;
                int[] arrLastWcTimes = _CurrentWcNum;
                //Thread.Sleep(8500);                
                ShowWaitMessage("预计下一个日计时误差将在{0}秒后计算完毕，请等待", GetWcTestCount() * 100);
                if (!ReadData(ref _CurWC, ref _CurrentWcNum, 1))
                {
                    if (Stop)
                    {
                        break;
                    }
                    continue;
                }

                #region 检定结束判断
                //当读取的误差次数大于最大读取次数时，如果还有没读到的数据，则继续读
                if (ReadWCNum >= maxErrorTimes +5)
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        m_CheckOver = true;
                        //误差点数量
                        for (int dtPos = maxErrorTimes - 1; dtPos > 0; dtPos--)
                        {
                            //如果读了十次仍未读到数据，则不管
                          
                            if (errorTable.Rows[i][dtPos].ToString() == "")
                            {
                                m_CheckOver = true;
                                break;
                            }
                        }
                        if (!m_CheckOver)
                        {
                            break;
                        }
                    }
                    //如果检查完了，则退出
                    if (m_CheckOver)
                    {
                        continue;
                    }
                }
                #endregion
                ReadWCNum++;
                MessageController.Instance.AddMessage("第" + ReadWCNum.ToString() + "次读取日计时误差完成，开始分析数据...");
                //开始分析数据
                // m_CheckOver = true;
                bool[] _CheckOver = new bool[BwCount];          //是否已经完成本次检定
                //记录每块表的数据
                string[] strResultKey = new string[BwCount];
                object[] objResultValue = new object[BwCount];
                //m_CheckOver = true;

                for (int i = 0; i < BwCount; i++)
                {
                    #region 解析每一块表的结论
                    if (Stop)
                    {
                        MessageController.Instance.AddMessage("检定停止，操作完毕");
                        break;
                    }

                    _MeterInfo = Helper.MeterDataHelper.Instance.Meter(i);                                              //表基本信息

                    #region ----------数据合法性检测----------
                    if (!_MeterInfo.YaoJianYn)
                    {
                        continue;
                    }
                    /*
                    处理超过255次的情况
                    */
                    if (_MeterWcNum[i] > 0 && _CurrentWcNum[i] < _MeterWcNum[i])
                    {
                        while (_MeterWcNum[i] > _CurrentWcNum[i])
                        {
                            _CurrentWcNum[i] += 255;
                        }
                    }
                    if (_MeterWcNum[i] < _CurrentWcNum[i])
                    {
                        _MeterWcNum[i] = _CurrentWcNum[i];
                        _VerifyTimes[i]++;
                    }
                    else
                    {
                        if (_VerifyTimes[i] < maxErrorTimes)
                            m_CheckOver = false;
                        continue;
                    }
                    if (_CurrentWcNum[i] == 0 || _CurrentWcNum[i] == 255)
                    {
                        m_CheckOver = false;
                        continue;            //如果本表位没有出误差，换下一表
                    }
                    #endregion

                    if (_VerifyTimes[i] > maxErrorTimes)
                    {
                        //推箱子,最后一次误差排列在最前面
                        for (int dtPos = maxErrorTimes - 1; dtPos > 0; dtPos--)
                        {
                            errorTable.Rows[i][dtPos] = errorTable.Rows[i][dtPos - 1];
                        }
                        errorTable.Rows[i][0] = _CurWC[i];     //最后一次误差始终放在第一位
                    }
                    else
                    {
                        errorTable.Rows[i][_VerifyTimes[i] - 1] = _CurWC[i];
                    }

                    #region 获取结论
                    /*设置误差参数*/
                    float wcLimit = 1.0F;
                    wcLimit = GetTxt_Wcx();
                    wcLimit *= 0.5F;
                    string[] _DJ = Number.getDj(_MeterInfo.Mb_chrBdj);
                    float _MeterLevel = float.Parse(_DJ[IsYouGong ? 0 : 1]);                   //当前表的等级
                    StWuChaDeal _WuChaPara = new StWuChaDeal();
                    _WuChaPara.MaxError = wcLimit;
                    _WuChaPara.MinError = -wcLimit;
                    _WuChaPara.MeterLevel = _MeterLevel;
                    WuChaContext vContext = new WuChaContext(WuChaType.多功能_日计时误差, _WuChaPara);
                    //Datable行到数组的转换
                    float[] _wc = ConvertArray.ConvertObj2Float(errorTable.Rows[i].ItemArray);
                    MeterDgn _tagResult = (MeterDgn)vContext.GetResult(_wc);
                    ResultDictionary["结论"][i] = _tagResult.Md_chrValue;
                    #endregion
                    #region 获取详细数据

                    //平均值
                    //string[] arrResult = _tagResult.Data.Split('|');    //分割结果
                    float fSum = 0.0f;
                    string[] strWc = new string[10];
                    string strAver;
                    string strHz;
                    for (int iNum = 0; iNum < _wc.Length; iNum++)
                    {
                        if (_wc[iNum] == Variable.WUCHA_INVIADE)
                        {
                            strWc[iNum] = "";
                            ResultDictionary[string.Format("误差{0}", iNum + 1)][i] = "";
                           // ResultDictionary["结论"][i] = "不合格";
                           

                        }
                        else
                        {
                            strWc[iNum] = _wc[iNum].ToString("F5");
                            fSum = fSum + Convert.ToSingle(strWc[iNum]);
                            ResultDictionary[string.Format("误差{0}", iNum + 1)][i] = _wc[iNum].ToString("F5");
                        
                        }
                    }
                    fSum = fSum / _wc.Length;
                    strAver = fSum.ToString("0.00");
                    strHz = fSum.ToString("0.0");
                  
                        ResultDictionary["平均值"][i] = fSum.ToString("0.00");
                        ResultDictionary["化整值"][i] = fSum.ToString("0.0");
                        if (Math.Abs(fSum) > _MeterLevel)
                        {
                            ResultDictionary["结论"][i] = "不合格";
                             ResultDictionary["不合格原因"][i] = "误差超差";
                        }

                    #endregion
                    #endregion
                }
                #region 上传结论
                int countTemp = 5;
                if (maxErrorTimes > 5)
                {
                    countTemp = 10;
                }
                for (int i = 0; i < countTemp; i++)
                {
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差" + (i + 1).ToString(), ResultDictionary[string.Format("误差{0}", i + 1)]);
                }
                #endregion
                Thread.Sleep(100);
            }

          MessageController.Instance.AddMessage("停止误差板!");
         Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(false,2);
            Thread.Sleep(200);
        }

        /// <summary>
        /// 初始化误差表
        /// </summary>
        private void InitErrorTable()
        {
            for (int i = 0; i < maxErrorTimes; i++)
            {
                errorTable.Columns.Add("RJSWC" + i.ToString());
            }
            //填充空数据
            for (int i = 0; i < BwCount; i++)
            {
                int j = maxErrorTimes - 1;
                errorTable.Rows.Add(new string[j]);
            }
            MessageController.Instance.AddMessage("初始化检定参数完毕! ");
        }

        /// <summary>
        /// 获取检定圈数
        /// </summary>
        /// <returns></returns>
        private int GetWcTestCount()
        {
            int Count;
            string[] str = VerifyPara.Split('|');
            if (int.TryParse(str[2], out Count))
            {
                return Count;
            }
            return 60;
        }

        /// <summary>
        /// 获取日计时误差限控制比例：
        /// </summary>
        /// <returns></returns>
        private float GetTxt_Wcx()
        {
            float Count;
            string[] str = VerifyPara.Split('|');

            if (float.TryParse(str[0], out Count))
            {
                return Count;
            }
            return 1.0F;
        }
        /// <summary>
        /// 获取误差次数
        /// </summary>
        /// <returns></returns>
        private int GetWcCount()
        {
            int Count;
            string[] str = VerifyPara.Split('|');
            if (int.TryParse(str[1], out Count))
            {
                return Count;
            }
            return 10;
        }
        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            string strBcs = "";
            if (GlobalUnit.IsDemo) return true;
            try
            {

                if (!PowerOn())
                {
                    throw new Exception("控制源输出失败！");
                }
                //电表选择时钟通道
                MeterProtocolAdapter.Instance.SetPulseCom(0);
                float[] values = new float[this.BwCount];
                float[] bcs = new float[this.BwCount];   //表常数
                int[] quans = new int[this.BwCount];        //圈数
                for (int i = 0; i < values.Length; i++)
                {
                    if (GlobalUnit.Meter(i).DgnProtocol != null && GlobalUnit.Meter(i).DgnProtocol.Loading)
                    {
                        values[i] = GlobalUnit.Meter(i).DgnProtocol.ClockPL;
                    }
                    if (GlobalUnit.Meter(i).Mb_chrBcs != "")
                    {
                        strBcs = GlobalUnit.Meter(i).Mb_chrBcs.ToString();
                        if (strBcs.IndexOf('(') != -1)
                        {
                            strBcs = strBcs.Substring(0, strBcs.IndexOf('('));
                        }
                        bcs[i] = float.Parse(strBcs);
                        quans[i] = GetWcTestCount();
                    }
                }
                return Helper.EquipHelper.Instance.InitPara_InitTimeAccuracy(values, bcs, quans);
            }
            catch (System.Exception e)
            {
                return CatchException(e);
            }
        }

      
        protected override bool CheckPara()
        {
            return true;
        }
    }
}