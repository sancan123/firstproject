using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using Comm.Const;
using Comm.Enum;
//using ClInterface;
using Comm.Model.DnbModel.DnbInfo;
using Comm.Model.DgnProtocol;
using Comm.BaseClass;

namespace VerifyAdapter.Helper
{
    /// <summary>
    /// RS485控制单元
    /// </summary>
    public class Rs485Helper : SingletonBase<Rs485Helper>
    {
        #region ------------变量声明------------
        /// <summary>
        /// 电能表操作接口
        /// </summary>
        private MeterProtocol.IMeterProtocol IMeterController = null;
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// 停止标志
        /// </summary>
        private bool isStop = false;

        /// <summary>
        /// 已经成功的数量
        /// </summary>
        private int successCount = 0;

        /// <summary>
        /// 发送指令后最大等等时间
        /// </summary>
        private int maxWaitReturnTime = 45;
        /// <summary>
        /// 数据返回标志,用于保存每一块表的数据返回状态
        /// </summary>
        private bool[] arrReturnFlag = new bool[0];

        /// <summary>
        /// 完成标志,用于标志每一块表的数据是否返回完成
        /// </summary>
        private bool[] arrOverFlag = new bool[0];

        /// <summary>
        /// BOOL型返回值
        /// </summary>
        private bool[] arrReturnValue_Bool = new bool[0];

        /// <summary>
        /// 字符串型返回值
        /// </summary>
        private string[] arrReturnValue_String = new string[0];
        /// <summary>
        /// 数组类型的返回值
        /// </summary>
        private Dictionary<int, string[]> dicReturnValue_StringArray = new Dictionary<int, string[]>();

        /// <summary>
        /// 浮点数数组类型返回
        /// </summary>
        private Dictionary<int, float[]> dicReturnValue_FloatArray = new Dictionary<int, float[]>();
        #endregion

        public Rs485Helper()
        {
            maxWaitReturnTime = GlobalUnit.GetConfig(Variable.CTC_DGN_MAXWAITDATABACKTIME, 45);
        }

        #region -----------------属性-----------------
        /// <summary>
        /// RS485是否已经成功初始化
        /// </summary>
        private bool isInitOk = false;

        private int bwCount = 1;
        /// <summary>
        /// 表位数量
        /// </summary>
        public int BwCount
        {
            get { return bwCount; }
            set { bwCount = value; }
        }

        private int retryTimes = 3;
        /// <summary>
        /// 读取重试次数
        /// </summary>
        public int RetryTimes
        {
            get { return retryTimes; }
            set { retryTimes = value; }
        }
        #endregion


        /// <summary>
        /// 停止当前操作
        /// </summary>
        public void Stop()
        {
            isStop = true;                                              //运行标志更新为停止
            while (isRunning)                                           //等待当前操作运行完毕
            {
                System.Threading.Thread.Sleep(30);
            }
        }

        /// <summary>
        /// 获取指定表位的结论
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool GetResult(int pos)
        {
            if (pos > -1 && pos < BwCount)
            { 
                return arrReturnValue_Bool[pos]; 
            }
            return false;
        }

        /// <summary>
        /// 复位
        /// </summary>
        private void Reset()
        {
            arrOverFlag = new bool[BwCount];                                 //完成标志
            arrReturnFlag = new bool[BwCount];                              //返回标识
            arrReturnValue_Bool = new bool[BwCount];                        //返回结论
            arrReturnValue_String = new string[BwCount];                     //返回字符串
            dicReturnValue_StringArray = new Dictionary<int, string[]>();   //返回数据
            dicReturnValue_FloatArray = new Dictionary<int, float[]>();
            successCount = 0;
        }

        /// <summary>
        /// 复位指定表位
        /// </summary>
        /// <param name="pos">指定表位号</param>
        private void Reset(int pos)
        {
            if (pos < 0 || pos > arrOverFlag.Length - 1 || pos > arrReturnFlag.Length - 1) return;
            arrOverFlag[pos] = false;
            arrReturnFlag[pos] = false;
            arrReturnValue_Bool[pos] = false;
            arrReturnValue_String[pos] = string.Empty;
            if (dicReturnValue_StringArray.ContainsKey(pos))
                dicReturnValue_StringArray.Remove(pos);
            if (dicReturnValue_FloatArray.ContainsKey(pos))
                dicReturnValue_FloatArray.Remove(pos);
            //successCount--;
        }

        /// <summary>
        /// 是否所有结果已经返回
        /// </summary>
        /// <returns></returns>
        internal bool IsAllReturn()
        {
            return successCount == BwCount;
        }

        /// <summary>
        /// 需要重试的电表列表
        /// </summary>
        /// <returns></returns>
        private int[] GetNeedReturnList()
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < BwCount; i++)
            {
                if (arrReturnFlag[i])
                    ret.Add(i);
            }
            return ret.ToArray();
        }

        /// <summary>
        /// 发送数据后等待返回
        /// </summary>
        internal void WaitReturn()
        {
            DateTime dtStartTime = DateTime.Now;
            while (true)
            {
                if (IsAllReturn())            //全部返回
                    break;
                if (isStop)                 //停止标志
                    break;
                if (((TimeSpan)(DateTime.Now - dtStartTime)).TotalSeconds > maxWaitReturnTime)
                    break;                  //超时退出
            }
        }

        #region ----------------------初始化RS485组件----------------------
        /// <summary>
        /// 初始化多功能
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
           // Settings.DgnConfigManager.Instance.Load();
           // MeterProtocolAdapter.Instance.SetBwCount(); 
            return true;
        }

        /// <summary>
        /// 多功能事件注册
        /// </summary>
        private void RegisterEvent()
        {
            //事件响应
            //IMeterController.OnChangePassword += new DelegateChangePassword(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventBroadcaseTime += new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventChangeSetting += new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearDemand += new DelegateClearDemand(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEnergy += new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEventLog += new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventCommTest += new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventFreezeCmd += new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventReadAddress += new DelegateReadAddress(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadData += new DelegateReadData(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDateTime += new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDemand += new DelegateReadDemand(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadEnergy += new DelegateReadEnergy(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadPeriodTime += new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
            //IMeterController.OnEventSetPulseCom += new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteAddress += new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteData += new DelegateWriteData(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteDateTime += new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWritePeriodTime += new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            ////数据事件
            //IMeterController.OnEventRxFrame += new Dge_EventRxFrame(IMeterControler_OnEventRxFrame);
            //IMeterController.OnEventTxFrame += new Dge_EventTxFrame(IMeterControler_OnEventTxFrame);

        }

        /// <summary>
        /// 解除事件绑定
        /// </summary>
        public void RealseDgnEvent()
        {

            //IMeterController.OnChangePassword -= new DelegateChangePassword(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventBroadcaseTime -= new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventChangeSetting -= new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearDemand -= new DelegateClearDemand(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEnergy -= new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventClearEventLog -= new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventCommTest -= new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventFreezeCmd -= new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventReadAddress -= new DelegateReadAddress(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadData -= new DelegateReadData(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDateTime -= new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            //IMeterController.OnEventReadDemand -= new DelegateReadDemand(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadEnergy -= new DelegateReadEnergy(IMeterControler_OnEventReturnFloat);
            //IMeterController.OnEventReadPeriodTime -= new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
            //IMeterController.OnEventSetPulseCom -= new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteAddress -= new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteData -= new DelegateWriteData(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWriteDateTime -= new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
            //IMeterController.OnEventWritePeriodTime -= new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            ////数据事件
            //IMeterController.OnEventRxFrame -= new Dge_EventRxFrame(IMeterControler_OnEventRxFrame);
            //IMeterController.OnEventTxFrame -= new Dge_EventTxFrame(IMeterControler_OnEventTxFrame);

        }
        #endregion

        #region ----------多功能事件返回----------

        /// <summary>
        /// 只返回结果类型的事件
        /// </summary>
        /// <param name="int_Index">表位号[0-BW]</param>
        /// <param name="bln_Result">结论</param>
        private void IMeterControler_OnEventReturnResult(int int_Index, bool bln_Result)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //已经合格不再处理
            arrReturnValue_Bool[int_Index] = bln_Result;    //记录下返回值
            arrReturnFlag[int_Index] = true;                //更新返回标志
            arrOverFlag[int_Index] = true;                  //更新完成标志
            successCount++;                                 //更新完成计数器
        }

        /// <summary>
        ///  返回字符类型数据
        /// </summary>
        /// <param name="int_Index">表位号[0-BW]</param>
        /// <param name="bln_Result">结论</param>
        /// <param name="str_Data">返回数据</param>
        private void IMeterControler_OnEventReturnString(int int_Index, bool bln_Result, string str_Data)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //已经合格不再处理
            arrReturnValue_String[int_Index] = str_Data;    //记录下返回值
            arrReturnFlag[int_Index] = true;                //更新返回标志
            arrOverFlag[int_Index] = bln_Result;            //更新完成标志
            if (bln_Result) successCount++;                  //更新完成计数器
        }
        /// <summary>
        /// 字符串数组类型返回
        /// </summary>
        /// <param name="int_Index"></param>
        /// <param name="bln_Result"></param>
        /// <param name="str_Data"></param>
        private void IMeterControler_OnEventReturnStringA(int int_Index, bool bln_Result, string[] str_Data)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //已经合格不再处理
            if (dicReturnValue_StringArray.ContainsKey(int_Index))
                dicReturnValue_StringArray.Remove(int_Index);
            dicReturnValue_StringArray.Add(int_Index, str_Data);
            arrReturnFlag[int_Index] = true;                //更新返回标志
            arrOverFlag[int_Index] = bln_Result;            //更新完成标志
            if (bln_Result)
                successCount++;                             //更新完成计数器
        }

        /// <summary>
        ///  //读取电量
        /// </summary>
        /// <param name="int_Index">表位索引,0开始</param>
        /// <param name="bln_Result">读取电量结果</param>
        /// <param name="sng_Energy">电量数据</param>

        public void IMeterControler_OnEventReturnFloat(int int_Index, bool bln_Result, float[] sng_Energy)
        {
            if (isStop) return;
            if (arrOverFlag[int_Index]) return;             //已经合格不再处理
            if (dicReturnValue_FloatArray.ContainsKey(int_Index))
                dicReturnValue_FloatArray.Remove(int_Index);
            dicReturnValue_FloatArray.Add(int_Index, sng_Energy);
            arrReturnFlag[int_Index] = true;                //更新返回标志
            arrOverFlag[int_Index] = true;                  //更新完成标志
            if (bln_Result) successCount++;                                 //更新完成计数器
        }

        #endregion

        #region ------------通讯测试------------
        /// <summary>
        /// 通讯测试，所有表位有效
        /// </summary>
        /// <returns>操作是否成功</returns>
        public bool CommTest()
        {
            Reset();
            Helper.Rs485CallBackHelper.CallBack callBack = IMeterController.ComTest;
            return Helper.Rs485CallBackHelper.Instance.CallBackWithNoPara(callBack);  
        }

        /// <summary>
        /// 对指定表位进行通讯测试
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool CommTest(int pos)
        {
            Reset(pos);
            Helper.Rs485CallBackHelper.CallBackOnePara_Int callBack=null;//IMeterController.ComTest;
            return Helper.Rs485CallBackHelper.Instance.CallBackWithOnePara_Int(callBack, pos); 
        }
        #endregion

        #region 读表时间 ReadDateTime()
        /// <summary>
        /// 读表时间
        /// </summary>
        /// <returns></returns>
        public bool ReadDateTime(ref string[] arrReturnData)
        {
            Reset();

            for (int i = 0; i < RetryTimes; i++)
            {
               // if (IMeterController.ReadDateTime())
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;               //全部完成则退出
            }
            if (arrReturnData.Length != BwCount)
                Array.Resize(ref arrReturnData, BwCount);
            Array.Copy(arrReturnValue_String, arrReturnData, arrReturnValue_String.Length);
            return IsAllReturn();
        }

        /// <summary>
        /// 读取指定表位的日期时间
        /// </summary>
        /// <param name="pos">表位号</param>
        /// <param name="returnData">读取到的日期时间</param>
        /// <returns>读取是否成功</returns>
        public bool ReadDateTime(int pos, ref string returnData)
        {
            successCount = BwCount - 1;
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.ReadDateTime(pos))                                        //发送指令
                {
                    LogHelper.Instance.WriteDebug(string.Format("第{0}次发送读取指令失败", i));
                    continue;
                }
                //检测当前表位是否有返回
                WaitReturn();
                if (IsAllReturn()) break;
            }
            returnData = arrReturnValue_String[pos];
            return arrOverFlag[pos];
        }

        #endregion

        #region 表操作-写表时间:m_485Adpater.WriteDateTime(string str_DateTime)
        /// <summary>
        /// 设置被检表时间
        /// </summary>
        /// <param name="str_DateTime">要设置的时间[yyMMddHHmmss]</param>
        /// <returns>如果一块都没有写过返回False,反之返回True</returns>
        public bool WriteDateTime(string str_DateTime)
        {
            Comm.GlobalUnit.g_MsgControl.OutMessage("开始写表时间到" + str_DateTime, false);
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
               // if (!IMeterController.WriteDateTime(str_DateTime))
                {
                    continue;
                }
                WaitReturn();
                if (IsAllReturn()) break;
            }
            return GetNeedReturnList().Length == 0;
        }

        /// <summary>
        /// 设置指定表位的电表时间
        /// </summary>
        /// <param name="str_DateTime">要设置的电表时间</param>
        /// <param name="pos">是表表位</param>
        /// <returns>设置是否成功</returns>
        public bool WriteDateTime(string str_DateTime, int pos)
        {
            Reset(pos);
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.WriteDateTime(str_DateTime))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            return IsAllReturn();
        }
        #endregion

        #region 读取表电量 ReadEnergy(enmPDirectType pDirect, enmTariffType TariffType)

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="pDirect">功率方向:0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="TariffType">费率:0=总，1=峰，2=平，3=谷，4=尖,5=所有 </param>
        /// <returns>命令是否成功发送</returns>
        public bool ReadEnergy(byte pDirect, byte TariffType, ref Dictionary<int, float[]> dicReturnValue)
        {
            Reset();
            Comm.GlobalUnit.g_MsgControl.OutMessage("开始读取" + pDirect.ToString() + TariffType.ToString() + "电量");
            for (int i = 0; i < RetryTimes; i++)
            {
               // if (!IMeterController.ReadEnergy(pDirect, TariffType))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            dicReturnValue = dicReturnValue_FloatArray;
            return IsAllReturn();
        }

        #endregion

        #region 读取需量ReadDemand(enmPDirectType PDirect, enmTariffType TariffType)

        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="PDirect">功率方向</param>
        /// <param name="TariffType">费率</param>
        /// <returns>操作是否成功</returns>

        public bool ReadDemand(Cus_PowerFangXiang PDirect, Cus_FeiLv TariffType, ref Dictionary<int, float[]> dicReturnValue)
        {
            // if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
                //正式版操作
               // if (!IMeterController.ReadDemand((enmPDirectType)PDirect, (enmTariffType)TariffType))
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }
            dicReturnValue = dicReturnValue_FloatArray;
            return IsAllReturn();
        }

        #endregion

        #region----------清空需量----------
        /// <summary>
        /// 清空需量
        /// </summary>
        /// <returns></returns>
        public bool ClearDemand()
        {
            // Check.Require(CommAdpter.m_IMeterControler, "电能表通讯控制器", Check.NotNull);
            //OpenPramgramLock();
            //int noPassCount = 0;
            //所有状态复位
            Reset();
            // CallBack callback_All = new CallBack(CommAdpter.m_IMeterControler.ClearDemand);
            // CallBack_Retry callback_one = new CallBack_Retry(CommAdpter.m_IMeterControler.ClearDemand);
            //return DoTestCallBack_NoPara(callback_All, callback_one);

            //#region ----------原来不用的代码
            for (int i = 0; i < RetryTimes; i++)
            {
                //if (!IMeterController.ClearDemand())
                    continue;
                WaitReturn();
                if (IsAllReturn()) break;
            }

            return IsAllReturn();
            //if (!isAllReturn())
            //    return false;
            //noPassCount = NoPassCount();
            //if (noPassCount == 0)                           //第一次群发是否都通过
            //    return true;
            //else
            //{
            //    //只有全部通过，只重试没有通过的表
            //    ResetNoPassMeterStatus();                   //重置没有成功表的返回状态
            //    int reTryMeter = 0;
            //    int[] reTryMeterArr = new int[lstNoPassMeter.Count];
            //    lstNoPassMeter.Values.CopyTo(reTryMeterArr, 0);
            //    for (int n = 0; n < reTryMeterArr.Length; n++)
            //    {
            //        reTryMeter = reTryMeterArr[n];
            //        for (int i = 0; i < ReTryTimes; i++)
            //        {
            //            if (CommAdpter.m_IMeterControler.ClearDemand(reTryMeter))
            //            {
            //                //最多发送三次，有一次成功就退出
            //                break;
            //            }
            //        }
            //    }
            //    if (!isAllReturn())
            //        return false;
            //    noPassCount = NoPassCount();
            //}
            //return noPassCount == 0;
            //#endregion
        }
        #endregion

        #region----------清空电量----------
        /// <summary>
        /// 清空电量
        /// </summary>
        /// <returns></returns>
        public bool ClearEnergy()
        {
            int noPassCount = 0;
            //if (CommAdpter.m_IMeterControler == null) return false;
            //OpenPramgramLock();
            Reset();
            for (int i = 0; i < RetryTimes; i++)
            {
              //  if (!IMeterController.ClearEnergy())
                    continue;
                WaitReturn();
                if (IsAllReturn())
                    break;
            }
            return IsAllReturn();
        }
        #endregion

        #region ----------读取表数据,自定义标识----------
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">长度</param>
        /// <param name="int_Dot">小数位,小数位为0代表没有小数位</param>
        /// <returns></returns>
        public bool ReadData(string str_ID, int int_Len, int int_Dot, ref string[] returnValue)
        {
            // if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            //readDataID = str_ID;
            //readDataLen = int_Len;
            //readDataDot = int_Dot;           //-1为区分是否有小数点读取方式
            for (int i = 0; i < RetryTimes; i++)
            {
                if (int_Dot > 0)
                {
                   // if (!IMeterController.ReadData(str_ID, int_Len, int_Dot))
                    {
                        continue;
                    }
                }
                else
                {
                   // if (!IMeterController.ReadData(str_ID, int_Len))
                    {
                        continue;
                    }
                }
                WaitReturn();
                if (IsAllReturn()) break;
            }
            if (returnValue.Length != BwCount)
                Array.Resize(ref returnValue, BwCount);
            Array.Copy(arrReturnValue_String, returnValue, arrReturnValue_String.Length);
            return IsAllReturn();
        }
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">长度</param>
        /// <returns></returns>
        //public bool ReadData(string str_ID, int int_Len)
        //{
        //    bool Result = false;
        //    for (int i = 0; i < ReTryTimes; i++)
        //    {
        // Result = CommAdpter.m_IMeterControler.ReadData(str_ID, int_Len);
        //       if (Result)
        //          break;
        // }
        //return Result;
        //}
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识</param>
        /// <param name="int_Len">长度</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns></returns>
        //public bool ReadDataBlock(string str_ID, int int_Len, int int_Dot,)
        //{
        //    //if (CommAdpter.m_IMeterControler == null) return false;
        //    Reset();
        //    //if (!CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len, int_Dot))
        //    {
        //        Comm.GlobalUnit.g_MsgControl.OutMessage("发送读取表数据命令失败!", false);
        //        return false;
        //    }
        //    return isAllReturn();
        //}
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID"> 标识</param>
        /// <param name="int_Len">长度</param>
        /// <returns></returns>
        //public bool ReadDataBlock(string str_ID, int int_Len)
        //{
        //    //  if (CommAdpter.m_IMeterControler == null) return false;
        //    Reset();
        //    for (int i = 0; i < ReTryTimes; i++)
        //    {
        //        //  if (CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len))
        //        {
        //            break;  //发送三次。有一次成功就BREAK    
        //        }
        //    }
        //    return isAllReturn();
        //}
        /// <summary>
        /// 多功能下行数据显示
        /// </summary>
        /// <param name="str_Frame"></param>
        private void IMeterControler_OnEventRxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("==>" + str_Frame);
        }
        /// <summary>
        /// 多功能上行数据显示
        /// </summary>
        /// <param name="str_Frame"></param>
        private void IMeterControler_OnEventTxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("<==" + str_Frame);
        }

        #endregion

        #region 设置表通讯协议m_485Adpater.setMeterPara

        /// <summary>
        /// 单独设置一块表多功能参数。用于协议配置
        /// </summary>
        /// <param name="protocol">电表协议</param>
        /// <param name="addr">表地址</param>
        /// <param name="BwH">表位号</param>
        /// <returns></returns>
        public bool SetMeterPara(Comm.Model.DgnProtocol.DgnProtocolInfo protocol, string addr, int BwH)
        {
            //ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
            //meterDgnInfo[BwH] = new CAmMeterInfo();
            //IMeterController.Selected[BwH] = true;                                  //要检标志
            //if (!SetProtocol(protocol, meterDgnInfo[BwH], addr))
            //{
            //    return false;
            //}
            ////后面跟其它信息
            //IMeterController.AmMeterInfo = meterDgnInfo;
            return true;
        }

        /// <summary>
        /// 设置多功能参数，用于检定时调用
        /// </summary>
        //public bool SetMeterPara()
        //{
        //    ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
        //    string meterAddress = string.Empty;
        //    for (int k = 0; k < BwCount; k++)
        //    {
        //        MeterBasicInfo curMeter = GlobalUnit.Meter(k);
        //        meterDgnInfo[k] = new CAmMeterInfo();
        //        IMeterController.Selected[k] = curMeter.YaoJianYn;
        //        if (curMeter.YaoJianYn)
        //        {
        //            meterAddress = curMeter.Mb_chrAddr.Length > 0 ? curMeter.Mb_chrAddr : curMeter.Mb_ChrCcbh;
        //            if (!curMeter.DgnProtocol.Loading) curMeter.DgnProtocol.Load();
        //            if (!curMeter.DgnProtocol.Loading)
        //            {
        //                Helper.LogHelper.Instance.WriteError(string.Format("第{0}表位电能表多功能协议初始化失败", k + 1), null);
        //                return false;
        //            }
        //            if (!SetProtocol(curMeter.DgnProtocol, meterDgnInfo[k], meterAddress))
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //    IMeterController.AmMeterInfo = meterDgnInfo;
        //    return true;
        //}

        /// <summary>
        /// 设备协议内容
        /// </summary>
        //private bool SetProtocol(DgnProtocolInfo protocol, ClInterface.CAmMeterInfo meterDgnInfo, string meterAddr)
        //{

        //    meterDgnInfo.DllFile = protocol.DllFile;     //动态
        //    meterDgnInfo.ClassName = protocol.ClassName;
        //    meterDgnInfo.Address = meterAddr;
        //    meterDgnInfo.Setting = protocol.Setting;
        //    meterDgnInfo.UserID = protocol.UserID;
        //    meterDgnInfo.VerifyPasswordType = protocol.VerifyPasswordType;
        //    meterDgnInfo.WritePassword = protocol.WritePassword;
        //    meterDgnInfo.WritePswClass = protocol.WriteClass;
        //    meterDgnInfo.ClearDemandPassword = protocol.ClearDemandPassword;
        //    meterDgnInfo.ClearDemandPswClass = protocol.ClearDemandClass;
        //    meterDgnInfo.ClearEnergyPassword = protocol.ClearDLPassword;
        //    meterDgnInfo.ClearEnergyPswClass = protocol.ClearDLClass;
        //    meterDgnInfo.DataFieldPassword = protocol.DataFieldPassword;
        //    meterDgnInfo.BlockAddAA = protocol.BlockAddAA;
        //    meterDgnInfo.TariffOrderType = protocol.TariffOrderType;
        //    meterDgnInfo.DateTimeFormat = protocol.DateTimeFormat;
        //    meterDgnInfo.SundayIndex = protocol.SundayIndex;
        //    meterDgnInfo.ConfigFile = protocol.ConfigFile;
        //    meterDgnInfo.ComTestType = GetType(protocol.DgnPras, "001", 1);
        //    // meterDgnInfo[k].BroadcastTimeType = 1;
        //    meterDgnInfo.ReadEnergyType = GetType(protocol.DgnPras, "006", 1);
        //    meterDgnInfo.ReadDemandType = GetType(protocol.DgnPras, "005", 1);
        //    meterDgnInfo.ReadDateTimeType = GetType(protocol.DgnPras, "003", 1);
        //    //meterDgnInfo[k].ReadAddressType = 1;
        //    meterDgnInfo.ReadPeriodTimeType = GetType(protocol.DgnPras, "007", 1);
        //    // meterDgnInfo[k].ReadDataType = 1;
        //    // meterDgnInfo[k].WriteAddressType = 1;
        //    meterDgnInfo.WriteDateTimeType = GetType(protocol.DgnPras, "002", 1);
        //    // meterDgnInfo[k].WritePeriodTimeType = 1;
        //    // meterDgnInfo[k].WriteDataType = 1;
        //    meterDgnInfo.ClearDemandType = GetType(protocol.DgnPras, "004", 1);
        //    meterDgnInfo.ClearEnergyType = GetType(protocol.DgnPras, "008", 1);
        //    //meterDgnInfo[k].ClearEventLogType = 1;
        //    // meterDgnInfo[k].SetPulseComType = 1;
        //    // meterDgnInfo[k].FreezeCmdType = 1;
        //    //meterDgnInfo[k].ChangeSettingType = 1;
        //    //meterDgnInfo[k].ChangePasswordType = 1;
        //    meterDgnInfo.FECount = protocol.FECount;
        //    //后面跟其它信息
        //    return true;
        //}




        /// <summary>
        /// 获取多功能协议配置参数
        /// </summary>
        /// <param name="DgnPram">多功能参数列表</param>
        /// <param name="PramKey">要取值的KEY</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        private int GetType(Dictionary<string, string> DgnPram, string PramKey, int DefaultValue)
        {
            if (!DgnPram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = DgnPram[PramKey].Split('|');
            if (Arr_Pram.Length == 2)
            {
                return int.Parse(Arr_Pram[0]);
            }
            else
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// 分解析多功能参数
        /// </summary>
        /// <param name="Bwh"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] GetType(int Bwh, string PramKey)
        {
            if (!GlobalUnit.Meter(Bwh).DgnProtocol.Loading)
            {
                return new string[] { };
            }
            Dictionary<string, string> DgnPram = GlobalUnit.Meter(Bwh).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return new string[] { };
            }
            return DgnPram[PramKey].Split('|');
        }

        #endregion

        #region -------------------辅助方法-------------------
        /// <summary>
        /// 检测前置条件
        /// </summary>
        /// <returns></returns>
        private bool CheckCondition()
        {
            if (IMeterController == null || !isInitOk)
            {
                LogHelper.Instance.WriteError("RS485组件调用前必须先初始化", null);
                return false;
            }
            return true;
        }
        #endregion

    }
}
