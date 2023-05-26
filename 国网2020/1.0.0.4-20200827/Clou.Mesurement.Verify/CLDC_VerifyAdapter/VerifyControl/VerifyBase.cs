
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Function;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 检验控制基类
    /// </summary>
    public abstract class VerifyBase
    {
        #region -------变量声明----------

        /// <summary>
        /// 台体编号
        /// </summary>
        private int m_intTaiID;
        /// <summary>
        /// 检定开始时间,用于检定计时
        /// </summary>
        protected DateTime m_StartTime;

        /// <summary>
        /// 是否已经完成本项目检定
        /// 只有m_Stop=true且m_CheckOver=true时，检定停止操作才算真正完成
        /// </summary>
        protected bool m_CheckOver = false;

        /// <summary>
        /// 是否停止,停止操作一般是在外部因素作用下状态被更改。内部检定一般不会使用此标志。
        /// </summary>
        private bool m_Stop = false;


        /// <summary>
        /// 经互感器二次变比
        /// </summary>
        private float m_HGQ2DL = 5F;

        /// <summary>
        /// 设置脉冲通道后等待时间
        /// </summary>
        protected int m_WaitTime_SelectPulseChannel;
        /// <summary>
        /// 设置检定参数后等待时间
        /// </summary>
        protected int m_WaitTime_SetTaskPara;

        /// <summary>
        /// 普通升源后等待的时间
        /// </summary>
        protected int m_WaitTime_PowerOn;

        /// <summary>
        /// 当前误差一致性类型
        /// </summary>
        private int m_ErrAccordType;

        /// <summary>
        /// 启动检定后等待时间
        /// </summary>
        protected int m_WaitTime_StartTask;
        /// <summary>
        /// 设置188G后等待时间
        /// </summary>
        //protected int m_WaitTime_Set188G;
        /// <summary>
        /// 捕捉到错误时提示信息等待时间
        /// </summary>
        protected int m_WaitTime_ExceptionMsg = 300;

        /// <summary>
        /// 不合格原因数组
        /// </summary>
        protected string[] reasonS = new string[Adapter.Instance.BwCount];
        #endregion

        #region --------构造函数---------
        /// <summary>
        /// 构造函数
        /// <param name="plan">方案</param>
        /// </summary>
        public VerifyBase(object plan)
        {
            //保存当前方案内容
            //CurPlan = plan;
            VerifyPara = plan as string;
        }
        #endregion

        #region -----------检定控制------------
        /// <summary>
        /// 执行检定操作入口
        /// </summary>
        public void DoVerify()
        {
            GlobalUnit.ManualResult = new string[BwCount];
            GlobalUnit.ManualShuju = new string[BwCount];
            //
            //参数检查
            if (!CheckPara())
            {
                //Helper.LogHelper.Instance.WriteWarm("方案内容不符合要求:" + CurPlan.ToString(), null);
                MessageController.Instance.AddMessage(string.Format("解析方案参数出错,参数内容:{0}",VerifyPara),6,2);
                return;
            }
            //初始化方案参数
            //InitPlanData();
            //停止标志
            m_Stop = false;
            //完成标志
            m_CheckOver = false;
            //检定当前进度置0
            GlobalUnit.g_CUS.DnbData.NowMinute = 0F;
            MessageController.Instance.AddMessage(string.Format("开始检定:{0}",VerifyProcess.Instance.CurrentName),6,90);
            try
            {
                //记录开始检定时间
                m_StartTime = DateTime.Now;
                //调用检测方法，本方法需要由派生类实现
                Verify();
                //Helper.EquipHelper.Instance.PowerOff();
            }
            catch (Exception ex)
            {
                //TODO:检定中的异常捕捉，分类
                Helper.LogHelper.Instance.Loger.Error(ex.Message, ex);
                GlobalUnit.Logger.Error("执行检定出错:" + this.GetType().FullName, ex);
              //  MessageController.Instance.AddMessage(m_intTaiID.ToString() + "号装置检定过程出错:"+ex.Message,6,2);
            }
            finally
            {
                //完成后清理工作
                Helper.LogHelper.Instance.WriteInfo("当前项目检定完毕，还原检定标识");
                m_Stop = true;
                m_CheckOver = true;
                Helper.LogHelper.Instance.WriteInfo("当前项目检定完毕，退出检定器");
            }
        }

        /// <summary>
        /// 倒计时
        /// <paramref name="seconds">倒计时时长（秒）</paramref>
        /// <paramref name="msg">显示功能的信息，正在进行{0}        5/20秒</paramref>
        /// </summary>
        public void Countdown(float seconds, string msg)
        {
            DateTime startT = DateTime.Now;
            TimeSpan ts = DateTime.Now.Subtract(startT);
            while (ts.TotalSeconds < seconds && !Stop)
            {
                GlobalUnit.g_CUS.DnbData.NowMinute = (float)ts.TotalMinutes;

                string des = string.Format("正在进行{0}        {1}/{2}秒", msg, (int)(seconds - ts.TotalSeconds), seconds);
                MessageController.Instance.AddMessage(des);

                Thread.Sleep(100);
                ts = DateTime.Now.Subtract(startT);
            }
            GlobalUnit.g_CUS.DnbData.NowMinute = (float)ts.TotalMinutes;
        }
        /// <summary>
        /// 开始检定[需要派生类重写]
        /// 方案及ID合法性检测
        /// 文字参数检测[有特殊需要时，派生类需要重写CheckPara()方法]
        /// 清理节点数据[已经默认处理项目结论，项目具体数据需要派生类自己重载处理]
        /// 清空消息队列，设置检定状态,设置检定灯状态
        /// </summary>
        public virtual void Verify()
        {
            GlobalUnit.IsJdb = false;
            GlobalUnit.IsXieBo = false;
            GlobalUnit.IsJxb = false;
            if (GlobalUnit.IsCL3112)
            {
                Helper.EquipHelper.Instance.FuncMstate(0xFE);
            }
            Helper.EquipHelper.Instance.SetErrCalcType(0);
            //if (GlobalUnit.ENCRYPTION_MACHINE_TYPE.Contains("南网"))
            {
                if (Stop) return;
            //    Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
                if (Stop) return;
            //    Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter,0);
            }
            //add by wzs on 20200421
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试时间", setSameStrArryValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
   
            return;
        }
        /// <summary>
        /// 无参数升源:按当前方案参数升源
        /// </summary>
        protected virtual bool PowerOn()
        {
            bool isYouGong = true;//(CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.正向有功 || CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.反向有功);
            float xIb = 0F;
            //float.TryParse( CurPlan.OutPramerter.xIb.Replace("Ib", ""),out xIb); 
            //电流倍数与功率因素方案内容与文档内容不符合，默认按无电流，1.0输出

            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U
                                                      , xIb * GlobalUnit.Ib
                                                      , (int)Cus_PowerYuanJian.H
                                                      , (int)Cus_PowerFangXiang.正向有功
                                                      , "1.0"
                                                      , isYouGong
                                                      , false);

            if (ret)
            {
                MessageController.Instance.AddMessage("正在等待多功能操作电能表正常运行时间" + 5 + "秒。");
                Thread.Sleep(5 * 1000);
            }

            return ret;
        }

        /// <summary>
        /// 参数合法性检测[由具体检定器实现]
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckPara()
        {
            return true;
        }

        /// <summary>
        /// 清理当前节点数据,默认清理当前检定类型结论数据
        /// 包括总结论和当前方向的结论
        /// </summary>
        protected virtual void ClearItemData()
        {
            //MessageController.Instance.AddMessage("开始清理当前检定结论数据");
            //清理总结论
            Helper.MeterDataHelper.Instance.ClearResultData(ResultKey, Cus_MeterDataType.检定结论);
            //清理分方向结论
            String powerDirectResult = string.Format("{0}{1}", ResultKey, (int)PowerFangXiang);
            Helper.MeterDataHelper.Instance.ClearResultData(powerDirectResult, Cus_MeterDataType.检定结论);
            //具体数据由检定器自己处理
        }

        /// <summary>
        /// 清理当前节点数据,默认清理当前检定类型结论数据
        /// 包括总结论和当前方向的结论
        /// </summary>
        protected virtual void ClearItemData(string strKey)
        {
            MessageController.Instance.AddMessage("开始清理当前检定结论数据");
            //清理总结论
            Helper.MeterDataHelper.Instance.ClearResultData(strKey, Cus_MeterDataType.检定结论);
            //清理分方向结论
            String powerDirectResult = string.Format("{0}{1}", strKey, (int)PowerFangXiang);
            Helper.MeterDataHelper.Instance.ClearResultData(powerDirectResult, Cus_MeterDataType.检定结论);
            //具体数据由检定器自己处理
        }

        /// <summary>
        /// 挂接默认数据[需要由派生类重写]
        /// </summary>
        protected virtual void DefaultItemData()
        {
            return;
        }

        /// <summary>
        /// 获取或设置停止检定状态
        /// </summary>
        public bool Stop
        {
            set
            {
                m_Stop = value;
            }
            get { return m_Stop; }
        }
        #endregion

        #region ------------属性-------------
        /// <summary>
        /// 互感器二次变比
        /// </summary>
        public float HGQ2DL
        {
            set { m_HGQ2DL = value; }

        }

        /// <summary>
        /// 当前检定器是否已经完成检定
        /// </summary>
        public bool IsRunOver
        {
            get { return m_CheckOver; }
        }

        /// <summary>
        /// 获取当前检定是有功还是无功
        /// </summary>
        protected bool IsYouGong
        {
            get
            {
                bool _IsP = false;
                if (PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功
                    || PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功 || PowerFangXiang == Cus_PowerFangXiang.组合有功)
                    _IsP = true;
                return _IsP;
            }
        }
        /// <summary>
        /// 功率方向[正向或是反向]
        /// </summary>
        protected string FangXiangStr
        {
            get
            {
                string _IsZ = string.Empty;
                if (PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.反向无功 ||
                    PowerFangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功)
                    _IsZ = "-";
                return _IsZ;

            }
        }
        /// <summary>
        /// 当前检定功率方向
        /// </summary>
        protected Cus_PowerFangXiang PowerFangXiang
        {
            get;
            set;
        }


        /// <summary>
        /// 返回表位数量
        /// </summary>
        protected int BwCount
        {
            get
            {

                return Adapter.Instance.BwCount;
            }
        }
        /// <summary>
        /// 误差类型
        /// </summary>
        protected int ErrAccordType
        {
            set
            {
                m_ErrAccordType = value;
            }
            get { return m_ErrAccordType; }
        }

        /// <summary>
        /// 当前检定方案内容
        /// </summary>
        public object CurPlan
        {
            get;
            set;
        }
        private object m_CurPlan = null;


        /// <summary>
        /// 项目Key,如:P_1 表示项目和第一个项目
        /// 表示当前检定项目的数据节点
        /// </summary> 
        protected abstract string ItemKey { get; }

        /// <summary>
        /// 项目结论节点,为项目总结论结点值
        /// </summary>
        protected abstract string ResultKey { get; }

        /// <summary>
        /// 检定到当前计时，单位：秒
        /// </summary>
        public long VerifyPassTime
        {
            get
            {
                return (long)((TimeSpan)(DateTime.Now - m_StartTime)).TotalSeconds;
            }
        }
        #endregion

        #region -------------方法------------

        #region-----------计算指定负载下的标准功率----------
        /// <summary>
        /// 计算指定负载下的标准功率.(合元功率)
        /// </summary>
        /// <param name="U">负载电压</param>
        /// <param name="I">负载电流</param>
        /// <param name="Clfs">测量方式</param>
        /// <returns>标准功率</returns>
        protected float CalculatePower(float U, float I, Cus_Clfs Clfs)
        {
            float p = U * I;

            if (Clfs == Cus_Clfs.三相四线)
            {
                p *= 3F;
                return p;
            }
            else if (Clfs == Cus_Clfs.单相)
            {
                return p;
            }
            else
            {
                p *= 1.732F;
                return p;
            }
        }
        /// <summary>
        /// 计算指定负载下的标准功率.(W)
        /// </summary>
        /// <param name="U">负载电压</param>
        /// <param name="I">负载电流</param>
        /// <param name="Clfs">测量方式</param>
        /// <param name="Yj">元件H，ABC</param>
        /// <param name="Glys">功率因数，0.5L</param>
        /// <param name="isP">true 有功，false 无功</param>
        /// <returns>标准功率</returns>
        protected float CalculatePower(float U, float I, Cus_Clfs Clfs, Cus_PowerYuanJian Yj, string Glys, bool isP)
        {
            float flt_GlysP = 1;
            float flt_GlysQ = 0;
            if (isP)
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysP);
                flt_GlysQ = (float)Math.Sqrt(1 - Math.Pow(flt_GlysP, 2));
            }
            else
            {
                float.TryParse(Glys.Replace("C", "").Replace("L", "").ToString(), out flt_GlysQ);
                flt_GlysP = (float)Math.Sqrt(1 - Math.Pow(flt_GlysQ, 2));
            }
            float p = U * I * flt_GlysP;
            float q = U * I * flt_GlysQ;
            if (Cus_PowerYuanJian.H == Yj)
            {
                if (Clfs == Cus_Clfs.三相四线)
                {
                    p *= 3F;
                    q *= 3F;
                }
                else if (Clfs == Cus_Clfs.单相)
                {

                }
                else
                {
                    p *= 1.732F;
                    q *= 1.732F;
                }
            }
            return isP ? p : q;
        }
        #endregion

        #region ----------计算当前负载下本批表中脉冲常数最小的表跑一个脉冲需要的时间----------
        /// <summary>
        /// 计算出一个误差需要的时间ms
        /// </summary>
        ///<remarks>
        ///如果存在多种常数的电能表，则以最先出脉冲的电能表为准
        ///</remarks>
        /// <returns>出一个误差需要时间估算值,单位ms</returns>
        protected int GetOneErrorTime(string PowerDianLiu, Cus_PowerYuanJian PowerYuanJian, string PowerYinSu, bool IsP)
        {
            MeterBasicInfo firstMeter = Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter);
            if (firstMeter == null) return 1000;//默认按一秒处理
            //计算当前负载功率
            float current = Number.GetCurrentByIb(PowerDianLiu, firstMeter.Mb_chrIb,firstMeter.Mb_BlnHgq);
            float currentPower = CalculatePower(GlobalUnit.U, current, GlobalUnit.Clfs, PowerYuanJian, PowerYinSu, IsP);
            //计算一度大需要的时间,单位分钟
            float needTime = 1000F / currentPower * 60F;
            return OnePulseNeedTime(IsYouGong, needTime);
        }
        /// <summary>
        /// 计算当前负载下本批表中脉冲常数最小的表跑一个脉冲需要的时间（ms）
        /// </summary>
        /// <param name="bYouGong">有功/无功</param>
        /// <param name="OneKWHTime">一度电需要的时间(分)</param>
        /// <returns>以毫秒为单位</returns>
        protected int OnePulseNeedTime(bool bYouGong, float OneKWHTime)
        {
            float _MinConst = 999999999;
            int _OnePulseTime = 99;
            int[] arrConst = Helper.MeterDataHelper.Instance.MeterConst(bYouGong);
            for (int i = 0; i < arrConst.Length; i++)
            {

                if (arrConst[i] < _MinConst)
                    _MinConst = arrConst[i];

            }
            if (_MinConst == 999999999) return 1;
            _OnePulseTime = (int)Math.Ceiling((OneKWHTime * 60 / _MinConst * 1000));
            return _OnePulseTime;
        }
        #endregion
        /// <summary>
        /// 检测是否跳差
        /// </summary>
        /// <param name="lastError">前一误差</param>
        /// <param name="curError">当前误差</param>
        /// <param name="meterLevel">表等级</param>
        /// <param name="m_WCJump">跳差系数</param>
        /// <returns>T:跳差;F:不跳差</returns>
        protected bool CheckJumpError(string lastError, string curError, float meterLevel, float m_WCJump)
        {
            bool result = false;
            if (Number.IsNumeric(lastError) && Number.IsNumeric(curError))
            {
                float _Jump = float.Parse(curError) - float.Parse(lastError);
                if (Math.Abs(_Jump) > meterLevel * m_WCJump)
                {
                    result = true;
                }
            }
            return result;
        }
        /// <summary>
        /// 获取指定电能表当前功率方向下表等级
        /// </summary>
        /// <returns></returns>
        protected float MeterLevel(MeterBasicInfo meter)
        {
            string[] _DJ = Number.getDj(meter.Mb_chrBdj);
            return float.Parse(_DJ[IsYouGong ? 0 : 1]);                   //当前表的等级
        }
        #endregion

        #region ----------读取功能数据---------
        /// <summary>
        /// 读取检定数据,数据类型为当前检定数据
        /// </summary>
        /// <param name="arrData">功能数组，每表位对应</param>
        /// <param name="arrWcCount">误差次数数组，每表位对应</param>
        /// <param name="DemoWcLimit">当前误差限，用于DEMO时生成演示数据</param>
        /// <returns>是否读取成功</returns>
        protected bool ReadData(ref string[] arrData, ref int[] arrWcCount, float DemoWcLimit)
        {
            if (!GlobalUnit.IsDemo)
            {
                MessageController.Instance.AddMessage("正在读取检定数据...");
                CLDC_DeviceDriver.stError[] arrError = Helper.EquipHelper.Instance.ReadWcb(true);
                MessageController.Instance.AddMessage("读取检定数据完成，等待分析数据");
                arrData = new string[BwCount];
                arrWcCount = new int[BwCount];
                for (int i = 0; i < arrError.Length && i < BwCount; i++)
                {
                    arrData[i] = arrError[i].szError;
                    arrWcCount[i] = arrError[i].Index;
                }
            }
            else
            {
           //     arrData = Helper.VerifyDemoHelper.Instance.BasicError2(DemoWcLimit, ref arrWcCount);
            }
            /*
                对取到的误差次数进行排序，如果最后一个次数还是为0的话则认识全部都没有出误差
                */
            int[] currentWcNumCopy = (int[])arrWcCount.Clone();
            Array.Sort(currentWcNumCopy);
            if (currentWcNumCopy == null
                || currentWcNumCopy.Length == 0
                || currentWcNumCopy[currentWcNumCopy.Length - 1] == 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region ----------错误处理----------
        /// <summary>
        /// 统一错误处理
        /// </summary>
        /// <param name="ex">错误对象</param>
        /// <returns>false[主要用于处理函数中的返回]</returns>
        protected bool CatchException(Exception ex)
        {
            //只有当前状态为非停止状态下才提示。
            //Console.WriteLine("Stop:" + Stop.ToString() + "====ForceVerifyStop:" + Comm.GlobalUnit.ForceVerifyStop.ToString());
            MessageController.Instance.AddMessage("检定过程中出现错误:"+ex.Message,6,2);

            if (!Stop)
            {
                //GlobalUnit.ForceVerifyStop = true;
                Stop = true;
                Thread.Sleep(m_WaitTime_ExceptionMsg);
            }
            return false;
        }
        #endregion

        
        #region 初始化方案参数
        /// <summary>
        /// 初始化方案内容，主要处理方案中的功率源参数
        /// 如:当前的功率方向，是否是有功等
        /// </summary>
        private void InitPlanData()
        {
            if (CurPlan is StPlan_Dgn)
            {
                StPlan_Dgn tagDgn = (StPlan_Dgn)CurPlan;
                PowerFangXiang = tagDgn.OutPramerter.GLFX;
            }
        }
        #endregion

        #region 格式化等待
        /// <summary>
        /// 格式化等待信息输出
        /// </summary>
        /// <param name="formatString">显示的内容,必须且只能包括一个{0}占位符</param>
        /// <param name="maxWaitTime">最大等等时间，单位MS</param>
        protected void ShowWaitMessage(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
            }
        }



        protected void ShowWaitMessageForSelfHeating(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 格式化等待信息输出
        /// </summary>
        /// <param name="formatString">显示的内容,必须且只能包括一个{0}占位符</param>
        /// <param name="maxWaitTime">最大等等时间，单位MS</param>
        protected bool ShowWaitTimeMessage(string formatString, int maxWaitTime)
        {
            int maxtime = maxWaitTime;
            string message = string.Empty;
            while (maxtime > 0)
            {
                message = string.Format(formatString, (maxtime / 1000) + 1);
                MessageController.Instance.AddMessage(message);
                Thread.Sleep(3000);
                maxtime -= 3000;
                if (maxtime < 1000)
                {
                    return true;
                }
                if (Stop) break;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    return true;
                }
            }
            return true;
        }
        #endregion
        /// <summary>
        /// 根据操作结果获取结果描述
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected string GetResultString(bool result)
        {
            return result ? "成功" : "失败";
        }

        #region 新加的


        /// <summary>
        /// 误差计算组件
        /// </summary>
        protected CLDC_DataCore.WuChaDeal.WuChaContext m_WuChaContext;
        /// 结束检定,调用通知
        /// <summary>
        /// 结束检定,调用通知
        /// </summary>
        public virtual void FinishVerify()
        {
            //等待1秒,等待数据上传完毕
            MessageController.Instance.NotifyVerifyFinished();
        }
        public string VerifyKey { get; set; }
        public string VerifyPara { get; set; }

        /// <summary>
        /// 结论字典
        /// </summary>
        private Dictionary<string, string[]> resultDictionary = new Dictionary<string, string[]>();
        /// <summary>
        /// 结论字典
        /// </summary>
        protected Dictionary<string, string[]> ResultDictionary
        {
            get { return resultDictionary; }
            set { resultDictionary = value; }
        }

        /// <summary>
        /// 结论的所有列名称
        /// </summary>
        /// <param name="arrayResultName"></param>
        protected string[] ResultNames
        {
            set
            {
                if (value != null)
                {
                    ResultDictionary.Clear();
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (!resultDictionary.ContainsKey(value[i]))
                        {
                            resultDictionary.Add(value[i], new string[BwCount]);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 上传检定结论
        /// </summary>
        /// <param name="resultName"></param>
        public void UploadTestResult(string resultName)
        {
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, resultName, ResultDictionary[resultName]);
        }
        /// <summary>
        /// 转换检定结论
        /// </summary>
        /// <param name="resultName">结论项名称</param>
        /// <param name="arrayResult">结论</param>
        public void ConvertTestResult(string resultName, bool[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i] ? "合格" : "不合格";
                    }
                }
            }
            UploadTestResult(resultName);
        }

        /// <summary>
        /// 转换检定结论
        /// </summary>
        /// <param name="resultName">结论项名称</param>
        /// <param name="arrayResult">结论</param>
        public void ConvertTestResult(string resultName, int[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString();
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// 转换检定结论
        /// </summary>
        /// <param name="resultName">结论项名称</param>
        /// <param name="arrayResult">结论</param>
        public void ConvertTestResult(string resultName, string[] arrayResult)
        {
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i];
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// 转换检定结论
        /// </summary>
        /// <param name="resultName">结论项名称</param>
        /// <param name="dotNumber">小数点后数量</param>
        /// <param name="arrayResult">结论</param>
        public void ConvertTestResult(string resultName, float[] arrayResult,int dotNumber=2)
        {
            string formatTemp="0";
            if(dotNumber>0)
            {
                formatTemp="0.".PadRight(2+dotNumber,'0');
            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString(formatTemp) ;
                    }
                }
            }
            UploadTestResult(resultName);
        }
        /// <summary>
        /// 转换检定结论
        /// </summary>
        /// <param name="resultName">结论项名称</param>
        /// <param name="dotNumber">小数点后数量</param>
        /// <param name="arrayResult">结论</param>
        public void ConvertTestResult(string resultName, double[] arrayResult, int dotNumber = 2)
        {
            string formatTemp = "0";
            if (dotNumber > 0)
            {
                formatTemp = "0.".PadRight(2 + dotNumber, '0');
            }
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (arrayResult.Length > i)
                    {
                        ResultDictionary[resultName][i] = arrayResult[i].ToString(formatTemp);
                    }
                }
            }
            UploadTestResult(resultName);
        }
        #endregion



        /// <summary>
        /// 切换到远程模式下的准备工作
        /// </summary>
        /// <param name="iFlag"></param>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        public void ChangRemotePreparatoryWork(out int[] iFlag, out string[] strRand1, out string[] strRand2, out string[] strEsamNo)
        {
            iFlag = new int[BwCount];
            strRand1 = new string[BwCount];
            strRand2 = new string[BwCount];
            strEsamNo = new string[BwCount];

            string[] strRand1Tmp = new string[BwCount];
            string[] strRand2Tmp = new string[BwCount];
            string[] strEsamNoTmp = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strRevCode = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strERand1 = new string[BwCount];
            string[] strERand2 = new string[BwCount];


            //准备

            #region 先红外认证后才能进行读取的一系列操作

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
            string[] MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(MyStatus[i])) continue;
                iSelectBwCount++;
                if (MyStatus[i].EndsWith("1FFFF"))
                {
                    rstTmp[i] = true;
                    iFlag[i] = 1;
                }
            }

            if (Stop) return;
            int[] iFlagTmp = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);
            

            //if (Stop) return;
            //MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            //bool[] result = MeterProtocolAdapter.Instance.SouthFindCard(1);



            //// 1.密钥恢复
            //if (Array.IndexOf(rstTmp, true) > -1)
            //{
            //    if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
            //    {
            //        for (int i = 0; i < BwCount; i++)
            //        {
            //            if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
            //            {
            //                if (Stop) return;
            //                if (rstTmp[i])
            //                {
            //                    MessageController.Instance.AddMessage("正在第" + (i + 1) + "表位密钥恢复,请稍候....");
            //                    bool blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "00", 17, strRand2Tmp[i], strEsamNoTmp[i]);
            //                    iFlagTmp[i] = 0;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Stop) return;
            //        MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
            //        bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2Tmp, strEsamNoTmp);
            //        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //        Common.Memset(ref iFlagTmp, 0);
            //    }
            //}



            //if (Stop) return;
            //Common.Memset(ref strRevCode, "DF010001002D0001");
            //MessageController.Instance.AddMessage("正在读取费控模式状态字");
            //result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlagTmp, strRand1Tmp, strRevCode, out FkStatus, out strOutMac1);

            //iSelectBwCount = 0;
            //Common.Memset(ref rstTmp, false);
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //    iSelectBwCount++;
            //    if (FkStatus[i] != "01")
            //    {
            //        rstTmp[i] = true;
            //    }
            //}

            //if (Array.IndexOf(rstTmp, true) > -1)
            //{
            //    Common.Memset(ref strData, "01" + "00000000" + "00000000");

            //    if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
            //    {
            //        for (int i = 0; i < BwCount; i++)
            //        {
            //            if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
            //            if (Stop) return;
            //            if (rstTmp[i])
            //            {
            //                MessageController.Instance.AddMessage("正在对第" + (i + 1) + "块表下发模式切换命令切换到远程模式,请稍候....");
            //                bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlagTmp[i], strRand2Tmp[i], strData[i]);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (Stop) return;
            //        MessageController.Instance.AddMessage("正在下发模式切换命令切换到远程模式,请稍候....");
            //        result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlagTmp, strRand2Tmp, strData);
            //        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            //    }
            //}

            iFlag = iFlagTmp;
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
        }

        /// <summary>
        /// 切换到本地模式下的准备工作
        /// </summary>
        /// <param name="iFlag"></param>
        /// <param name="strRand1"></param>
        /// <param name="strRand2"></param>
        /// <param name="strEsamNo"></param>
        public void ChangLocalPreparatoryWork(out int[] iFlag, out string[] strRand1, out string[] strRand2, out string[] strEsamNo)
        {
            iFlag = new int[BwCount];
            strRand1 = new string[BwCount];
            strRand2 = new string[BwCount];
            strEsamNo = new string[BwCount];

            string[] strRand1Tmp = new string[BwCount];
            string[] strRand2Tmp = new string[BwCount];
            string[] strEsamNoTmp = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            int iSelectBwCount = 0;
            string[] strRevCode = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strData = new string[BwCount];


            //准备

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            if (Stop) return;
            int[] iFlagTmp = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1Tmp, out strRand2Tmp, out strEsamNoTmp);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            bool[] result = MeterProtocolAdapter.Instance.SouthFindCard(1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在检查密钥状态,请稍候....");
            string[] MyStatus = MeterProtocolAdapter.Instance.ReadData("04000508", 4);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(MyStatus[i])) continue;
                iSelectBwCount++;
                if (MyStatus[i].EndsWith("1FFFF"))
                {
                    rstTmp[i] = true;
                }
            }

            // 1.密钥恢复
            if (Array.IndexOf(rstTmp, true) > -1)
            {
                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (Stop) return;
                            if (rstTmp[i])
                            {
                                MessageController.Instance.AddMessage("正在第" + (i + 1) + "表位密钥恢复,请稍候....");
                                bool blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2(i, "00", 17, strRand2Tmp[i], strEsamNoTmp[i]);
                                iFlagTmp[i] = 0;
                            }
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在密钥恢复,请稍候....");
                    bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2Tmp, strEsamNoTmp);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlagTmp, 0);
                }
            }



            if (Stop) return;
            Common.Memset(ref strRevCode, "DF010001002D0001");
            MessageController.Instance.AddMessage("正在读取费控模式状态字");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlagTmp, strRand1Tmp, strRevCode, out FkStatus, out strOutMac1);

            iSelectBwCount = 0;
            Common.Memset(ref rstTmp, false);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                iSelectBwCount++;
                if (FkStatus[i] != "00")
                {
                    rstTmp[i] = true;
                }
            }

            if (Array.IndexOf(rstTmp, true) > -1)
            {
                Common.Memset(ref strData, "00" + "00000000" + "00000000");

                if (Common.GetResultCount(rstTmp, iSelectBwCount / 4))
                {
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (Stop) return;
                        if (rstTmp[i])
                        {
                            MessageController.Instance.AddMessage("正在对第" + (i + 1) + "块表下发模式切换命令切换到本地模式,请稍候....");
                            bool blnResult = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(i, iFlagTmp[i], strRand2Tmp[i], strData[i]);
                        }
                    }
                }
                else
                {
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发模式切换命令切换到本地模式,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlagTmp, strRand2Tmp, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                }
            }

            iFlag = iFlagTmp;
            strRand1 = strRand1Tmp;
            strRand2 = strRand2Tmp;
            strEsamNo = strEsamNoTmp;
        }

        /// <summary>
        /// 读取表地址和表号
        /// </summary>
        public void ReadMeterAddrAndMeterNo()
        {
            if (GlobalUnit.IsDemo) return;

          //  if (!GlobalUnit.ReadMeterAddressAndNo) return;

            MeterBasicInfo FirstMeter = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);

          //  if (FirstMeter.DgnProtocol.ClassName != "CDLT6452007")
          //      return;




            GlobalUnit.g_MsgControl.OutMessage("正在进行【读取表地址】操作...");

            string[] address = MeterProtocolAdapter.Instance.ReadAddress();


            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter.Mb_chrAddr = address[i];

            }
            Adapter.Instance.UpdateMeterProtocol();
            GlobalUnit.g_MsgControl.OutMessage("正在进行【读取表号】操作...");

            string[] meterno = MeterProtocolAdapter.Instance.ReadData("04000402", 6);

            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter._Mb_MeterNo = meterno[i];
            }
            GlobalUnit.g_MsgControl.OutMessage();

          //  GlobalUnit.ReadMeterAddressAndNo = false;
        }



        #region 载波测试
        public void SwitchCarrierOr485(Cus_CommunType communType)
        {
            if (GlobalUnit.IsDemo) return;
            if (Stop) return ;   
            Helper.EquipHelper.Instance.PowerOff();
            Thread.Sleep(5000);
            GlobalUnit.g_IsOnPowerU = false;
            if (communType == Cus_CommunType.通讯485)
            {
                GlobalUnit.g_MsgControl.OutMessage("控制普通供电继电器闭合", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(3);
                Thread.Sleep(300);
                //if (!GlobalUnit.g_IsOnPowerU)
                //    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, Cus_PowerFangXiang.正向有功);//只输出电压
                GlobalUnit.g_CommunType = Cus_CommunType.通讯485;

            }
            else if (communType == Cus_CommunType.通讯载波)
            {
                GlobalUnit.g_MsgControl.OutMessage("控制载波供电继电器闭合", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(2);
                Thread.Sleep(300);
                if (!GlobalUnit.g_IsOnPowerU)
                    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);//只输出电压

                GlobalUnit.g_MsgControl.OutMessage("等待升源稳定...", false);

                Thread.Sleep(5000);

                GlobalUnit.g_MsgControl.OutMessage("正在进行载波试验检定...", false);

                if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierType.IndexOf("2041") != -1)
                {
                    //【初始化控制器】
                    GlobalUnit.g_MsgControl.OutMessage("初始化控制器...", false);
                    Dictionary<string, int> _2041ID = new Dictionary<string, int>();
                    for (int iBw = 0; iBw < BwCount; iBw++)
                    {

                        //【获取指定表位电表信息】
                        MeterBasicInfo curMeterTmp = Helper.MeterDataHelper.Instance.Meter(iBw);

                        //【判断是否要检】
                        if (!curMeterTmp.YaoJianYn)
                        {
                            continue;
                        }
                        if (!_2041ID.ContainsKey(curMeterTmp.AVR_CARR_PROTC_NAME))
                        {
                            _2041ID.Add(curMeterTmp.AVR_CARR_PROTC_NAME, iBw);
                        }
                    }
                   
                        Helper.EquipHelper.Instance.Init2041();
                        Thread.Sleep(1000);
                                        GlobalUnit.g_MsgControl.OutMessage("添加从节点...", false);
                    AddCarrierNodes();
                    int Overtime = 0;
                    if (Stop) return;  
                    if (GlobalUnit.CarrierInfo.CarrierName == "中电华瑞" || GlobalUnit.CarrierInfo.CarrierName == "中电华瑞2016") //如果是宽带，等待组网时间
                    {
                    //    for (int k = 0; k < 300; k++)
                    //    {
                    //        if (Stop) return;
                    //        Overtime += 1;
                     //       Thread.Sleep(1000);
                    //        GlobalUnit.g_MsgControl.OutMessage("等待载波组网300秒,已经过去" + Overtime + "秒...", false);
                     //   }
                        ShowWaitMessage("等待载波组网300秒,剩下{0}秒...", 300000);
                    }
                    if (Stop) return;  
                    if (GlobalUnit.CarrierInfo.CarrierName != "中电华瑞2016")
                    {
                       // foreach (int item in _2041ID.Values)
                     //   {
                            Helper.EquipHelper.Instance.PauseRouter();
                            Thread.Sleep(1000);
                       // }
                    }
                    else
                    {
                        Helper.EquipHelper.Instance.StarCarrier();
                    }
                    Thread.Sleep(500);
                }
                GlobalUnit.g_CommunType = Cus_CommunType.通讯载波;
            }
            else if (communType == Cus_CommunType.通讯无线)
            {
                GlobalUnit.g_MsgControl.OutMessage("控制无线供电继电器闭合", false);
                Helper.EquipHelper.Instance.SetPowerSupplyType(2);
                Thread.Sleep(300);
                if (!GlobalUnit.g_IsOnPowerU)
                    Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);//只输出电压

                GlobalUnit.g_MsgControl.OutMessage("等待升源稳定...", false);

                Thread.Sleep(5000);

                GlobalUnit.g_MsgControl.OutMessage("正在进行无线试验检定...", false);


                if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierType.IndexOf("2041") != -1)
                {
                    //【初始化控制器】
                    GlobalUnit.g_MsgControl.OutMessage("初始化控制器...", false);
                    Dictionary<string, int> _2041ID = new Dictionary<string, int>();
                    for (int iBw = 0; iBw < BwCount; iBw++)
                    {

                        //【获取指定表位电表信息】
                        MeterBasicInfo curMeterTmp = Helper.MeterDataHelper.Instance.Meter(iBw);

                        //【判断是否要检】
                        if (!curMeterTmp.YaoJianYn)
                        {
                            continue;
                        }
                        if (!_2041ID.ContainsKey(curMeterTmp.AVR_CARR_PROTC_NAME))
                        {
                            _2041ID.Add(curMeterTmp.AVR_CARR_PROTC_NAME, iBw);
                        }
                    }
                   
                        Helper.EquipHelper.Instance.Init2041();
                        Thread.Sleep(1000);
                    

                    GlobalUnit.g_MsgControl.OutMessage("添加从节点...", false);
                    AddCarrierNodes();
                  
                        Helper.EquipHelper.Instance.PauseRouter();
                        Thread.Sleep(500);
                    
                    Thread.Sleep(500);
                }
                if (Stop) return;  
                int _MaxStartTime = 180;
                m_StartTime = DateTime.Now;
                while (true)
                {
                    //每一秒刷新一次数据
                    long _PastTime = VerifyPassTime;
                    System.Threading.Thread.Sleep(1000);

                    float pastMinute = _PastTime / 60F;
                    CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                    string strDes = string.Format("等待组网需要", PowerFangXiang) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                    CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(strDes, true);

                    if ((_PastTime >= _MaxStartTime) || Stop)
                    {
                        CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                        break;
                    }

                    if (Stop) return;
                }
                if (Stop) return;  
                GlobalUnit.g_CommunType = Cus_CommunType.通讯无线;
            }
        }

        /// <summary>
        /// 添加载波从节点S
        /// </summary>
        private void AddCarrierNodes()
        {
            MeterBasicInfo curMeter;
            string strAddress;
            for (int iBw = 0; iBw < BwCount; iBw++)
            {
                //【强制停止】
                if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    break;
                }

                //【获取指定表位电表信息】
                curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);

                //【判断是否要检】
                if (!curMeter.YaoJianYn)
                {
                    continue;
                }
                strAddress = curMeter.Mb_chrAddr;
                if (strAddress == "")
                {
                    continue;
                }
                Helper.EquipHelper.Instance.AddCarrierNode(1, strAddress.PadLeft(12, '0'));

            }
        }







        #endregion


        public string[] setSameStrArryValue(string s)
        {
            string[] strs = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    strs[i] = s;
                }
            }
            return strs;


        }

    }
}
