// ***************************************************************
//  Control485   date: 09/15/2009
//  -------------------------------------------------------------
//  Description:
//  485操作控制类
//  -------------------------------------------------------------
//  Copyright (C) 2009 -CdClou All Rights Reserved
// ***************************************************************
// Modify Log:
// 09/15/2009 16-28-10    Created
// 09/23/2009 10-53       增加表写操作时弹出提示打开编程开关
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Comm;
using ClInterface;
using ClAmMeterController;
using Comm.Model.DnbModel.DnbInfo;
using Comm.Struct;
using Comm.Enum;
using Comm.Const;
using Comm.Model.DgnProtocol;

namespace VerifyAdapter.Multi
{

    #region ----------委托----------
    /// <summary>
    /// 无参数回调
    /// </summary>
    /// <returns></returns>
    public delegate bool CallBack();
    /// <summary>
    /// 无参数重试回调
    /// </summary>
    /// <returns></returns>
    public delegate bool CallBack_Retry(int Index);
    /// <summary>
    /// 一个参数回调
    /// </summary>
    /// <param name="strdata"></param>
    /// <returns></returns>
    public delegate bool CallBack_Para1(string strdata);
    /// <summary>
    /// 一个参数回调重试
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="strdata"></param>
    /// <returns></returns>
    public delegate bool CallBack_Para1_Retry(int Index, string strdata);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate bool CallBack_Paras(params object[] args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate bool CallBack_Params_Retry(int Index, params object[] args);
    #endregion

    /// <summary>
    /// 电能表485操作模块
    /// </summary>
    public class Control485
    {

#if OLD
        #region ---------公共声明-------------
        /// <summary>
        /// 当前操作是否已经完成
        /// </summary>
        public static bool CurActionOver = false;
        /// <summary>
        /// 每一表位操作状态
        /// </summary>
        public static bool[] CheckOK = null;
        /// <summary>
        /// 数组类型返回[Float],每表位对应一个Float数组
        /// </summary>
        public static Dictionary<string, float[]> CurReturnDataA;

        /// <summary>
        /// 字符串数据组类型返回,每表位对应一个String[]
        /// </summary>
        public static Dictionary<string, string[]> curReturnStringA;
        /// <summary>
        /// 字符串类型返回
        /// </summary>
        public static string[] CurReturnString = new string[0];
        /// <summary>
        /// 单精度类型返回
        /// </summary>
        public static float[] CurResurnFloat = new float[0];
        /// <summary>
        /// 重试次数
        /// </summary>
        public int ReTryTimes = 3;

        /// <summary>
        /// 没有通过的表
        /// </summary>
        public static Dictionary<string, int> lstNoPassMeter = new Dictionary<string, int>();
        /// <summary>
        /// 没有返回的表
        /// </summary>
        public static Dictionary<int, int> lstNotReturnMeter = new Dictionary<int, int>();
        
        #endregion

        #region ----------私有变量---------
        /// <summary>
        /// 停止标志
        /// </summary>
        private bool m_Stop = true;
        /// <summary>
        /// 当前总操作状态
        /// </summary>
        private bool CurActionOk = false;
        /// <summary>
        /// 当前操作功率方向
        /// </summary>
        protected Cus_PowerFangXiang CurPDirect;
        /// <summary>
        /// 当前操作费率
        /// </summary>
        protected Cus_FeiLv CurTariff;
        /// <summary>
        /// 事件锁
        /// </summary>

        private object objEventLock = new object();
        //读表数据参数
        /// <summary>
        /// 数据读取标识
        /// </summary>
        private string readDataID;
        /// <summary>
        /// 数据读取长度
        /// </summary>
        private int readDataLen;
        /// <summary>
        /// 数据读取小数点位数
        /// </summary>
        private int readDataDot;

        /// <summary>
        /// 发送操作命令后等待返回最大时间
        /// </summary>
        private int maxWaitDataBackTime;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Control485()
        {
            maxWaitDataBackTime = GlobalUnit.GetConfig(Variable.CTC_DGN_MAXWAITDATABACKTIME, 45);
        }

        #region ----------属性----------
        /// <summary>
        /// 停止标志
        /// </summary>
        public bool Stop
        {
            set
            {
                m_Stop = value;
            }
            get { return m_Stop; }
        }


        /// <summary>
        /// 台体挂表架表位数量[只读]
        /// </summary>
        private int BwCount
        {
            get { return Adapter.BwCount; }
        }

        /// <summary>
        /// 设备控制器[只读]
        /// </summary>
        //private EquipUnit CommAdpter
        //{
        //    get { return Adapter.ComAdpater; }
        //}
        #endregion

        #region ----------数据返回检测----------

        /// <summary>
        /// 检测当前操作是否所有返回,最大等待时间对应系统设置发送
        /// 操作指令后等待返回时间。中途停止或是操作超时将返回False
        /// </summary>
        /// <returns>全部返回/True 反之否 </returns>
        public bool isAllReturn()
        {
            //加入最大等待时间
            DateTime dateStartTime = DateTime.Now;
            while (!CurActionOver)
            {
                //完成检验
                if (lstNotReturnMeter.Count == 0)   //都已经返回。PASS
                    break;
                if (lstNoPassMeter.Count == 0)      //都已经通过 PASS
                    break;
                if (Stop)                           //停止校验 
                    return false;
                if (GlobalUnit.ForceVerifyStop)     //标志校验
                    return false;
                //时间校验
                if (Comm.Function.DateTimes.DateDiff(dateStartTime) >= maxWaitDataBackTime)
                    return false;
                Thread.Sleep(GlobalUnit.g_ThreadWaitTime);
            }
            return true;
        }

        /// <summary>
        /// 检测本轮检测没有通过表的只数
        /// </summary>
        /// <returns>没有通过测试的表的数量</returns>
        public int NoPassCount()
        {
            return lstNoPassMeter.Count;
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
        public bool setMeterPara(Comm.Model.DgnProtocol.DgnProtocolInfo protocol, string addr, int BwH)
        {
            //Check.Require(CommAdpter, "485控制模块", Check.NotNull);
            //Check.Require(CommAdpter.m_IMeterControler, "多功能控制模块", Check.NotNull);
            //CommAdpter.m_IMeterControler.SetAdapter = CommAdpter.m_IComAdpater;
            ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
            meterDgnInfo[BwH] = new CAmMeterInfo();
            //CommAdpter.m_IMeterControler.Selected[BwH] = true;

            if (!setProtocol(protocol, meterDgnInfo[BwH], addr))
            {
                return false;
            }
            //后面跟其它信息
           // CommAdpter.m_IMeterControler.AmMeterInfo = meterDgnInfo;
            return true;
        }

        /// <summary>
        /// 设置多功能参数
        /// </summary>
        public bool setMeterPara()
        {
            //Check.Require(CommAdpter, "485控制模块", Check.NotNull);
           // Check.Require(CommAdpter.m_IMeterControler, "多功能控制模块", Check.NotNull);
           // CommAdpter.m_IMeterControler.SetAdapter = CommAdpter.m_IComAdpater;
            ClInterface.CAmMeterInfo[] meterDgnInfo = new CAmMeterInfo[BwCount];
            for (int k = 0; k < BwCount; k++)
            {
                MeterBasicInfo curMeter = GlobalUnit.Meter(k);
                meterDgnInfo[k] = new CAmMeterInfo();
                //CommAdpter.m_IMeterControler.Selected[k] = curMeter.YaoJianYn;
                if (curMeter.YaoJianYn)
                {
                    if (!setProtocol(curMeter.DgnProtocol, meterDgnInfo[k], curMeter.Mb_chrAddr.Length > 0 ? curMeter.Mb_chrAddr : curMeter.Mb_ChrCcbh))
                    {
                        return false;
                    }
                }
            }
           // CommAdpter.m_IMeterControler.AmMeterInfo = meterDgnInfo;
            return true;
        }

        /// <summary>
        /// 设备协议内容
        /// </summary>
        private bool setProtocol(DgnProtocolInfo protocol, ClInterface.CAmMeterInfo meterDgnInfo, string meterAddr)
        {
            Check.Require(protocol, "电能表通讯协议", Check.NotNull);
             Check.Assert(protocol.Loading, string.Format("电能表通讯协议{0}加载失败",protocol.ProtocolName));
           Check.Require(protocol.DllFile.Length > 4, string.Format("错误的电能表通讯协议",protocol.ProtocolName));

            meterDgnInfo.DllFile = protocol.DllFile;     //动态
            meterDgnInfo.ClassName = protocol.ClassName;
            meterDgnInfo.Address = meterAddr;
            meterDgnInfo.Setting = protocol.Setting;
            meterDgnInfo.UserID = protocol.UserID;
            meterDgnInfo.VerifyPasswordType = protocol.VerifyPasswordType;
            meterDgnInfo.WritePassword = protocol.WritePassword;
            meterDgnInfo.WritePswClass = protocol.WriteClass;
            meterDgnInfo.ClearDemandPassword = protocol.ClearDemandPassword;
            meterDgnInfo.ClearDemandPswClass = protocol.ClearDemandClass;
            meterDgnInfo.ClearEnergyPassword = protocol.ClearDLPassword;
            meterDgnInfo.ClearEnergyPswClass = protocol.ClearDLClass;
            meterDgnInfo.DataFieldPassword = protocol.DataFieldPassword;
            meterDgnInfo.BlockAddAA = protocol.BlockAddAA;
            meterDgnInfo.TariffOrderType = protocol.TariffOrderType;
            meterDgnInfo.DateTimeFormat = protocol.DateTimeFormat;
            meterDgnInfo.SundayIndex = protocol.SundayIndex;
            meterDgnInfo.ConfigFile = protocol.ConfigFile;
            meterDgnInfo.ComTestType = this.getType(protocol.DgnPras, "001", 1);
            // meterDgnInfo[k].BroadcastTimeType = 1;
            meterDgnInfo.ReadEnergyType = this.getType(protocol.DgnPras, "006", 1);
            meterDgnInfo.ReadDemandType = this.getType(protocol.DgnPras, "005", 1);
            meterDgnInfo.ReadDateTimeType = this.getType(protocol.DgnPras, "003", 1);
            //meterDgnInfo[k].ReadAddressType = 1;
            meterDgnInfo.ReadPeriodTimeType = this.getType(protocol.DgnPras, "007", 1);
            // meterDgnInfo[k].ReadDataType = 1;
            // meterDgnInfo[k].WriteAddressType = 1;
            meterDgnInfo.WriteDateTimeType = this.getType(protocol.DgnPras, "002", 1);
            // meterDgnInfo[k].WritePeriodTimeType = 1;
            // meterDgnInfo[k].WriteDataType = 1;
            meterDgnInfo.ClearDemandType = this.getType(protocol.DgnPras, "004", 1);
            meterDgnInfo.ClearEnergyType = this.getType(protocol.DgnPras, "008", 1);
            //meterDgnInfo[k].ClearEventLogType = 1;
            // meterDgnInfo[k].SetPulseComType = 1;
            // meterDgnInfo[k].FreezeCmdType = 1;
            //meterDgnInfo[k].ChangeSettingType = 1;
            //meterDgnInfo[k].ChangePasswordType = 1;
            meterDgnInfo.FECount = protocol.FECount;
            //后面跟其它信息
            return true;
        }




        /// <summary>
        /// 获取多功能协议配置参数
        /// </summary>
        /// <param name="DgnPram">多功能参数列表</param>
        /// <param name="PramKey">要取值的KEY</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns></returns>
        private int getType(Dictionary<string, string> DgnPram, string PramKey, int DefaultValue)
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
        protected string[] getType(int Bwh, string PramKey)
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


        #region ----------初始化多功能参数----------
        /// <summary>
        /// 初始化多功能参数
        /// </summary>
        public void InitDgn()
        {
            //Check.Require(CommAdpter, "多功能组件", Check.NotNull);
            //if (!GlobalUnit.IsDemo)
            //    CommAdpter.m_IMeterControler = new CMultiController(BwCount);
            //else
            //{
            //    CommAdpter.m_IMeterControler = new Dgn_Demo();
           // }
          //  CommAdpter.m_IMeterControler.BWCount = BwCount;
            //事件响应
           // CommAdpter.m_IMeterControler.OnChangePassword += new DelegateChangePassword(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventBroadcaseTime += new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventChangeSetting += new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventClearDemand += new DelegateClearDemand(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventClearEnergy += new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventClearEventLog += new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventCommTest += new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventFreezeCmd += new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventReadAddress += new DelegateReadAddress(IMeterControler_OnEventReturnString);
           // CommAdpter.m_IMeterControler.OnEventReadData += new DelegateReadData(IMeterControler_OnEventReturnString);
            //CommAdpter.m_IMeterControler.OnEventReadDateTime += new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            //CommAdpter.m_IMeterControler.OnEventReadDemand += new DelegateReadDemand(IMeterControler_OnEventReadDemand);
           // CommAdpter.m_IMeterControler.OnEventReadEnergy += new DelegateReadEnergy(IMeterControler_OnEventReadEnergy);
           // CommAdpter.m_IMeterControler.OnEventReadPeriodTime += new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
           // CommAdpter.m_IMeterControler.OnEventSetPulseCom += new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventWriteAddress += new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventWriteData += new DelegateWriteData(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventWriteDateTime += new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
           // CommAdpter.m_IMeterControler.OnEventWritePeriodTime += new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            //数据事件
           // CommAdpter.m_IMeterControler.OnEventRxFrame += new Dge_EventRxFrame(m_IMeterControler_OnEventRxFrame);
           // CommAdpter.m_IMeterControler.OnEventTxFrame += new Dge_EventTxFrame(m_IMeterControler_OnEventTxFrame);
            //isInitOk = true;    //初始化成功标识

        }

        /// <summary>
        /// 解除事件绑定
        /// </summary>
        public void RealseDgnEvent()
        {
            /*
            CommAdpter.m_IMeterControler.OnChangePassword -= new DelegateChangePassword(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventBroadcaseTime -= new DelegateEventBroadcastTime(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventChangeSetting -= new DelegateChangeSetting(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventClearDemand -= new DelegateClearDemand(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventClearEnergy -= new DelegateClearEnergy(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventClearEventLog -= new DelegateClearEventLog(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventCommTest -= new DelegateEventCommTest(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventFreezeCmd -= new DelegateFreezeCmd(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventReadAddress -= new DelegateReadAddress(IMeterControler_OnEventReturnString);
            CommAdpter.m_IMeterControler.OnEventReadData -= new DelegateReadData(IMeterControler_OnEventReturnString);
            CommAdpter.m_IMeterControler.OnEventReadDateTime -= new DelegateReadDateTime(IMeterControler_OnEventReturnString);
            CommAdpter.m_IMeterControler.OnEventReadDemand -= new DelegateReadDemand(IMeterControler_OnEventReadDemand);
            CommAdpter.m_IMeterControler.OnEventReadEnergy -= new DelegateReadEnergy(IMeterControler_OnEventReadEnergy);
            CommAdpter.m_IMeterControler.OnEventReadPeriodTime -= new DelegateReadPeriodTime(IMeterControler_OnEventReturnStringA);
            CommAdpter.m_IMeterControler.OnEventSetPulseCom -= new DelegateSetPulseCom(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventWriteAddress -= new DelegateWriteAddress(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventWriteData -= new DelegateWriteData(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventWriteDateTime -= new DelegateWriteDateTime(IMeterControler_OnEventReturnResult);
            CommAdpter.m_IMeterControler.OnEventWritePeriodTime -= new DelegateWritePeriodTime(IMeterControler_OnEventReturnResult);
            //数据事件
            CommAdpter.m_IMeterControler.OnEventRxFrame -= new Dge_EventRxFrame(m_IMeterControler_OnEventRxFrame);
            CommAdpter.m_IMeterControler.OnEventTxFrame -= new Dge_EventTxFrame(m_IMeterControler_OnEventTxFrame);
            */
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
            if (GlobalUnit.ForceVerifyStop) return;
            if (CheckOK[int_Index]) return; //已经合格不再处理，记得到Rese
            if (Stop) return;
            lock (objEventLock)
            {
                if (GlobalUnit.ForceVerifyStop) return;
                if (Stop) return;
                CheckOK[int_Index] = bln_Result;
                isCheckOver(int_Index);
            }
        }

        /// <summary>
        ///  返回字符类型数据
        /// </summary>
        /// <param name="int_Index">表位号[0-BW]</param>
        /// <param name="bln_Result">结论</param>
        /// <param name="str_Data">返回数据</param>
        private void IMeterControler_OnEventReturnString(int int_Index, bool bln_Result, string str_Data)
        {
            if (Stop || GlobalUnit.ForceVerifyStop) return;
            if (CheckOK[int_Index]) return; //已经合格不再处理，记得到Rese
            lock (objEventLock)
            {
                if (Stop || GlobalUnit.ForceVerifyStop) return;
                CheckOK[int_Index] = bln_Result;
                if (bln_Result)
                {
                    CurReturnString[int_Index] = str_Data;
                }
                isCheckOver(int_Index);
            }
        }
        /// <summary>
        /// 字符串数组类型返回
        /// </summary>
        /// <param name="int_Index"></param>
        /// <param name="bln_Result"></param>
        /// <param name="str_Data"></param>
        private void IMeterControler_OnEventReturnStringA(int int_Index, bool bln_Result, string[] str_Data)
        {
            if (Stop || GlobalUnit.ForceVerifyStop) return;
            if (CheckOK[int_Index]) return; //已经合格不再处理，记得到Rese
            lock (objEventLock)
            {
                string[] strTmp = str_Data;
                if (Stop || GlobalUnit.ForceVerifyStop) return;
                CheckOK[int_Index] = bln_Result;
                if (bln_Result)
                {
                    curReturnStringA.Add(int_Index.ToString(), strTmp);
                }
                isCheckOver(int_Index);
            }
        }


        //读取时段
        //private void IMeterControler_OnEventReadPeriodTime(int int_Index, bool bln_Result, string[] str_PTime)
        //{
        //    if (Stop || GlobalUnit.ForceVerifyStop) return;
        //    lock (objEventLock)
        //    {
        //        if (Stop || GlobalUnit.ForceVerifyStop) return;
        //        CheckOK[int_Index] = bln_Result;
        //        isCheckOver(int_Index);
        //    }
        //}

        /// <summary>
        ///  //读取电量
        /// </summary>
        /// <param name="int_Index">表位索引,0开始</param>
        /// <param name="bln_Result">读取电量结果</param>
        /// <param name="sng_Energy">电量数据</param>

        public void IMeterControler_OnEventReadEnergy(int int_Index, bool bln_Result, float[] sng_Energy)
        {
            if (Stop || GlobalUnit.ForceVerifyStop) return;
            if (CheckOK[int_Index]) return; //已经合格不再处理，记得到Rese
            lock (objEventLock)
            {
                Console.WriteLine("表位"+int_Index+"已经返回"+bln_Result.ToString());
                float[] sngTmp = sng_Energy;
                if (Stop || GlobalUnit.ForceVerifyStop) return;
                CheckOK[int_Index] = bln_Result;
                if (CurReturnDataA.ContainsKey(int_Index.ToString()))
                    CurReturnDataA.Remove(int_Index.ToString());
                CurReturnDataA.Add(int_Index.ToString(), sngTmp);
                isCheckOver(int_Index);
            }
        }
        /// <summary>
        ///  //读取需量
        /// </summary>
        /// <param name="int_Index">表位索引</param>
        /// <param name="bln_Result">返回结果</param>
        /// <param name="sng_Demand">返回数据</param>

        private void IMeterControler_OnEventReadDemand(int int_Index, bool bln_Result, float[] sng_Demand)
        {
            if (Stop || GlobalUnit.ForceVerifyStop) return;
            if (CheckOK[int_Index]) return; //已经合格不再处理，记得到Rese
            lock (objEventLock)
            {
                float[] sngTmp = sng_Demand;
                if (Stop || GlobalUnit.ForceVerifyStop) return;
                CheckOK[int_Index] = bln_Result;
                if (bln_Result)
                {
                    //if (CurTariff == Cus_FeiLv.所有)
                    {
                        if (CurReturnDataA.ContainsKey(int_Index.ToString()))
                            CurReturnDataA.Remove(int_Index.ToString());
                        CurReturnDataA.Add(int_Index.ToString(), sngTmp);
                    }
                    else
                        CurResurnFloat[int_Index] = sngTmp[0];
                }
                isCheckOver(int_Index);
            }
        }

        #endregion

        /// <summary>
        /// 重新初始化控制变量
        /// </summary>
        int[] m_RetryTimes = new int[0];
        bool m_CheckOver;

        #region ----------检定状态控制----------

        /// <summary>
        /// 复位状态[所有表]
        /// </summary>
        public void Reset()
        {
            ResetStatus();          //重置状态
            for (int i = 0; i < BwCount; i++)
            {
                if (GlobalUnit.Meter(i).YaoJianYn)
                {
                    lstNoPassMeter.Add(i.ToString(), i);
                    lstNotReturnMeter.Add(i, i);
                }
            }
        }
        /// <summary>
        /// 单块表复位
        /// </summary>
        /// <param name="intBW">表位号，下标0</param>
        public void Reset(int intBW)
        {
            ResetStatus();
            lstNoPassMeter.Clear();
            lstNotReturnMeter.Clear();
            lstNoPassMeter.Add(intBW.ToString(), intBW);
            lstNotReturnMeter.Add(intBW, intBW);
        }

        /// <summary>
        /// 重设没有通过的表的状态
        /// 用于第二次检定没有通过的表。使用此方法时请不要再次调用ReSet()
        /// </summary>
        public void ResetNoPassMeterStatus()
        {
            lstNotReturnMeter = new Dictionary<int, int>();
            int curMeter = 0;
            foreach (string strKey in lstNoPassMeter.Keys)
            {
                curMeter = lstNoPassMeter[strKey];
                lstNotReturnMeter.Add(curMeter, curMeter);
            }
        }
        /// <summary>
        /// 整状态复元
        /// </summary>
        private void ResetStatus()
        {
            // Console.WriteLine(this.ToString());
            Stop = false;
            m_RetryTimes = new int[BwCount];
            CurActionOver = false;
            CurActionOk = true;
            m_CheckOver = false;
            CheckOK = new bool[BwCount];
            CurReturnString = new string[BwCount];
            CurReturnDataA = new Dictionary<string, float[]>();
            curReturnStringA = new Dictionary<string, string[]>();
            CurResurnFloat = new float[BwCount];
            lstNoPassMeter = new Dictionary<string, int>();             //没有通过的表
            lstNotReturnMeter = new Dictionary<int, int>();             //没有返回数据的表

        }
        #endregion



        #region 读表时间 ReadDateTime()
        /// <summary>
        /// 读表时间
        /// </summary>
        /// <returns></returns>
        public bool ReadDateTime()
        {
            //if (CommAdpter.m_IMeterControler == null) return false;
            int noPassCount = 0;
            Reset();
           // CallBack callBack = new CallBack(CommAdpter.m_IMeterControler.ReadDateTime);
           // CallBack_Retry callBack2 = new CallBack_Retry(CommAdpter.m_IMeterControler.ReadDateTime);
            //return DoTestCallBack_NoPara(callBack, callBack2);

            #region ----------原来不用的代码

            //for (int i = 0; i < ReTryTimes; i++)
            //{
            //    Reset();
            //    //CurReturnString = new string[BwCount];
            //    if (!CommAdpter.m_IMeterControler.ReadDateTime())
            //        continue;
            //    if (!isAllReturn())
            //        return false;
            //    noPassCount = NoPassCount();
            //    if (noPassCount == 0)
            //        break;
            //}
            //return true;
            #endregion
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
            return true;
           // Check.Require(CommAdpter.m_IMeterControler, "表操作模块", Check.NotNull);
            OpenPramgramLock();
            Comm.GlobalUnit.g_MsgControl.OutMessage("开始写表时间到" + str_DateTime, false);
            Reset();                                    //状态还原
           // CallBack_Para1 callBack = new CallBack_Para1(CommAdpter.m_IMeterControler.WriteDateTime);
          //  CallBack_Para1_Retry callBack2 = new CallBack_Para1_Retry(CommAdpter.m_IMeterControler.WriteDateTime);
            //return Adapter.Adpater485.DoTestCallBack_OnePara(str_DateTime, callBack, callBack2);

            #region ---以前的代码------
            //for (int i = 0; i < ReTryTimes; i++)        //最大发送三次，有一次成功就返回
            //{
            //    if (CommAdpter.m_IMeterControler.WriteDateTime(str_DateTime))
            //    {
            //        if (!isAllReturn())
            //            return false;
            //        noPassCount = NoPassCount();
            //        break;
            //    }
            //}
            ////检测还有没有表位没有通过。是否需要单独写时间
            //if (noPassCount > 0)
            //{
            //    int reTryMeter = 0;
            //    foreach (string strKey in lstNoPassMeter.Keys)
            //    {
            //        reTryMeter = lstNoPassMeter[strKey];
            //        for (int k = 0; k < ReTryTimes; k++)
            //        {
            //            if (CommAdpter.m_IMeterControler.WriteDateTime(reTryMeter, str_DateTime))
            //            {
            //                Thread.Sleep(200);  //这个值等测试后确定
            //                break;
            //            }
            //        }
            //    }
            //    isAllReturn();                  //再次检测是不是所有表都有返回
            //    noPassCount = NoPassCount();
            //    if (noPassCount > 0)
            //    {
            //        GlobalUnit.g_MsgControl.OutMessage(string.Format("表{0}写时间失败", getNoPassStr()), false, Cus_MessageType.提示消息);
            //        Thread.Sleep(200);
            //    }
            //}
            //return noPassCount == 0;
            #endregion
        }
        #endregion

        #region 读取表电量 ReadEnergy(enmPDirectType pDirect, enmTariffType TariffType)

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="pDirect">功率方向:0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="TariffType">费率:0=总，1=峰，2=平，3=谷，4=尖 </param>
        /// <returns>命令是否成功发送</returns>
        public bool ReadEnergy(Cus_PowerFangXiang pDirect, Cus_FeiLv TariffType)
        {
            //if (CommAdpter.m_IMeterControler == null) return false;
            int noPassCount = 0;
            Reset();
            Comm.GlobalUnit.g_MsgControl.OutMessage("开始读取" + pDirect.ToString() + "电量");
            //for (int i = 0; i < ReTryTimes; i++)
            //{
            //    //if (TariffType == Cus_FeiLv.所有)
            //    {
            //        //if (CommAdpter.m_IMeterControler.ReadEnergy(pDirect))
            //            break;
            //    }
            //    else
            //    {

            //        //if (CommAdpter.m_IMeterControler.ReadEnergy(pDirect, TariffType))
            //            break;
            //    }
            //}
            if (!isAllReturn())
                return false;
            noPassCount = NoPassCount();
            if (noPassCount == 0)
                return true;
            else
            {
                int reTrymeter = 0;
                int[] reTrymeterArr = new int[lstNoPassMeter.Count];
                lstNoPassMeter.Values.CopyTo(reTrymeterArr, 0);
                for (int n = 0; n < reTrymeterArr.Length; n++)
                {
                    reTrymeter = reTrymeterArr[n];
                    for (int i = 0; i < ReTryTimes; i++)
                    {
                       // if (CommAdpter.m_IMeterControler.ReadEnergy(pDirect, TariffType))
                            break;
                    }
                }
                if (!isAllReturn())
                    return false;
                noPassCount = NoPassCount();
            }
           // return noPassCount != Adapter.CurControl.Helper.MeterDataHelper.Instance.YaoJianMeterCount;
        }

        /// <summary>
        /// 读取电量[读取一个功率方向下的所有费率]
        /// </summary>
        /// <param name="PDirect">功率方向:0=P+ 1=P- 2=Q+ 3=Q-</param>
        /// <returns></returns>
        public bool ReadEnergy(Cus_PowerFangXiang PDirect)
        {
           // if (CommAdpter.m_IMeterControler == null) return false;
            return false;//ReadEnergy(PDirect, Cus_PowerFangXiang.正向有功);
        }

        #endregion

        #region 读取需量ReadDemand(enmPDirectType PDirect, enmTariffType TariffType)

        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="PDirect">功率方向</param>
        /// <param name="TariffType">费率</param>
        /// <returns>操作是否成功</returns>

        public bool ReadDemand(Cus_PowerFangXiang PDirect, Cus_FeiLv TariffType)
        {
           // if (CommAdpter.m_IMeterControler == null) return false;
            int noPassCount = 0;
            Reset();
            for (int i = 0; i < ReTryTimes; i++)
            {
                //正式版操作
               // if (CommAdpter.m_IMeterControler.ReadDemand(PDirect, TariffType))
                    break;
            }
            if (!isAllReturn())
                return false;
            noPassCount = NoPassCount();
            if (noPassCount == 0)
                return true;
            else
            {
                int reTrymeter = 0;
                int[] reTrymeterArr = new int[lstNoPassMeter.Count];
                lstNoPassMeter.Values.CopyTo(reTrymeterArr, 0);
                for (int n = 0; n < reTrymeterArr.Length; n++)
                {
                    reTrymeter = reTrymeterArr[n];
                    for (int i = 0; i < ReTryTimes; i++)
                    {
                       // if (CommAdpter.m_IMeterControler.ReadDemand(reTrymeter, PDirect, TariffType))
                            break;
                    }
                }
                if (!isAllReturn())
                    return false;
                noPassCount = NoPassCount();
            }
            return noPassCount == 0;
        }
        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="PDirect">功率方向</param>
        /// <returns>操作是否成功</returns>
        public bool ReadDemand(Cus_PowerFangXiang PDirect)
        {
            CurPDirect = PDirect;
            return false;// ReadDemand(PDirect, enmTariffType.所有);
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
            OpenPramgramLock();
            int noPassCount = 0;
            //所有状态复位
            Reset();
           // CallBack callback_All = new CallBack(CommAdpter.m_IMeterControler.ClearDemand);
           // CallBack_Retry callback_one = new CallBack_Retry(CommAdpter.m_IMeterControler.ClearDemand);
            return DoTestCallBack_NoPara(callback_All, callback_one);

            #region ----------原来不用的代码
            //for (int i = 0; i < ReTryTimes; i++)
            //{
            //    if (CommAdpter.m_IMeterControler.ClearDemand())
            //    {
            //        //最多发送三次，有一次成功就退出
            //        break;
            //    }
            //}
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
            #endregion
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
            OpenPramgramLock();
            for (int i = 0; i < ReTryTimes; i++)
            {
                Reset();
              //  if (!CommAdpter.m_IMeterControler.ClearEnergy())
                    continue;
                if (!isAllReturn())
                    return false;
                noPassCount = NoPassCount();
                if (noPassCount == 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region 读取表数据,自定义标识
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">长度</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns></returns>
        public bool ReadData(string str_ID, int int_Len, int int_Dot)
        {
           // if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            readDataID = str_ID;
            readDataLen = int_Len;
            readDataDot = int_Dot;           //-1为区分是否有小数点读取方式
            for (int i = 0; i < ReTryTimes; i++)
            {
              //  if (CommAdpter.m_IMeterControler.ReadData(str_ID, int_Len, int_Dot))
                {
                    Comm.GlobalUnit.g_MsgControl.OutMessage("发送读取表数据命令失败!", false);
                    break;
                }
            }
            return isAllReturn();
        }
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">长度</param>
        /// <returns></returns>
        public bool ReadData(string str_ID, int int_Len)
        {
            bool Result = false;
            for (int i = 0; i < ReTryTimes; i++)
            {
               // Result = CommAdpter.m_IMeterControler.ReadData(str_ID, int_Len);
                if (Result)
                    break;
            }
            return Result;
        }
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID">标识</param>
        /// <param name="int_Len">长度</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns></returns>
        public bool ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            //if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            //if (!CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len, int_Dot))
            {
                Comm.GlobalUnit.g_MsgControl.OutMessage("发送读取表数据命令失败!", false);
                return false;
            }
            return isAllReturn();
        }
        /// <summary>
        /// 读取表数据
        /// </summary>
        /// <param name="str_ID"> 标识</param>
        /// <param name="int_Len">长度</param>
        /// <returns></returns>
        public bool ReadDataBlock(string str_ID, int int_Len)
        {
          //  if (CommAdpter.m_IMeterControler == null) return false;
            Reset();
            for (int i = 0; i < ReTryTimes; i++)
            {
              //  if (CommAdpter.m_IMeterControler.ReadDataBlock(str_ID, int_Len))
                {
                    break;  //发送三次。有一次成功就BREAK    
                }
            }
            return isAllReturn();
        }
        /// <summary>
        /// 多功能下行数据显示
        /// </summary>
        /// <param name="str_Frame"></param>
        private void m_IMeterControler_OnEventTxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("==>" + str_Frame);
        }
        /// <summary>
        /// 多功能上行数据显示
        /// </summary>
        /// <param name="str_Frame"></param>
        private void m_IMeterControler_OnEventRxFrame(string str_Frame)
        {
            GlobalUnit.m_485DataControl.OutMessage("<==" + str_Frame);
        }

        #endregion


        #region ----------是否所有表位都已经返回:isCheckOver(int Index)-----------
        //检测是否所有表位都已经返回数据
        object objChangeResultLock = new object();
        private void isCheckOver(int Index)
        {
            if (GlobalUnit.ForceVerifyStop) return;
            lock (objChangeResultLock)
            {
                if (GlobalUnit.ForceVerifyStop) return;
                GlobalUnit.g_MsgControl.OutMessage(String.Format("表{0}返回{1}结果", Index + 1, CheckOK[Index] ? "成功" : "失败"), false);
                m_RetryTimes[Index]++;
                if (lstNotReturnMeter.ContainsKey(Index))
                {
                    lstNotReturnMeter.Remove(Index);
                    if (CheckOK[Index])
                    {
                        //如果当前表位已经成功，则从已经成功的列队中移除
                        if (lstNoPassMeter.ContainsKey(Index.ToString()))
                            lstNoPassMeter.Remove(Index.ToString());
                    }
                }
                if (lstNotReturnMeter.Count == 0)
                {
                    CurActionOver = true;
                }
            }
        }
        #endregion

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
            return sourceOrder;
        }

        /// <summary>
        /// 没有通过的表描述
        /// </summary>
        /// <returns>没有通过表位描述，如1,2,3</returns>
        public string getNoPassStr()
        {
            String strValue = String.Empty;
            foreach (String keys in lstNoPassMeter.Keys)
            {
                strValue += String.Format("{0},", lstNoPassMeter[keys] + 1);
            }
            strValue = strValue.Substring(0, strValue.Length - 1);
            return strValue;
        }

        #region ----------测试控制----------
        #region ----------编程开关控制---------
        private void OpenPramgramLock()
        {
            if (GlobalUnit.GetConfig(Variable.CTC_DGN_WRITEMETERALARM, "是") == "是")
            {
                System.Windows.Forms.MessageBox.Show("请打开被检表编程开关,操作完成后点击[确定按钮]");
            }
        }
        #endregion

        #region ----------不带参数回调----------


        // 
        /// <summary>
        /// 带一个参数测试方法回调
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="callBack_OnePara"></param>
        /// <returns></returns>
        public bool DoTestCallBack_NoPara(CallBack callBack, CallBack_Retry callBack_OnePara)
        {
            int noPassCount = 0;
            Reset();
            for (int i = 0; i < ReTryTimes; i++)
            {
                if (callBack())
                {
                    //最多发送三次，有一次成功就退出
                    break;
                }
            }
            if (!isAllReturn())
                return false;
            noPassCount = NoPassCount();
            if (noPassCount == 0)                           //第一次群发是否都通过
                return true;
            else
            {
                //只有全部通过，只重试没有通过的表
                ResetNoPassMeterStatus();                   //重置没有成功表的返回状态
                int reTryMeter = 0;
                int[] reTryMeterArr = new int[lstNoPassMeter.Count];
                lstNoPassMeter.Values.CopyTo(reTryMeterArr, 0);
                for (int n = 0; n < reTryMeterArr.Length; n++)
                {
                    reTryMeter = reTryMeterArr[n];
                    for (int i = 0; i < ReTryTimes; i++)
                    {
                        // if (CommAdpter.m_IMeterControler.ClearDemand(reTryMeter))
                        if (callBack_OnePara(reTryMeter))
                        {
                            //最多发送三次，有一次成功就退出
                            break;
                        }
                    }
                }
                if (!isAllReturn())
                    return false;
                noPassCount = NoPassCount();
            }
            return noPassCount == 0;
        }
        #endregion

        #region ----------带一个参数回调----------
        /// <summary>
        /// 带一个参数测试方法回调
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public bool DoTestCallBack_OnePara(string strPara,
            CallBack_Para1 callBack,
            CallBack_Para1_Retry callBack_OnePara)
        {
            int noPassCount = 0;
            Reset();
            for (int i = 0; i < ReTryTimes; i++)
            {
                if (callBack(strPara))
                {
                    //最多发送三次，有一次成功就退出
                    break;
                }
            }
            if (!isAllReturn())
                return false;
            noPassCount = NoPassCount();
            if (noPassCount == 0)                           //第一次群发是否都通过
                return true;
            else
            {
                //只有全部通过，只重试没有通过的表
                ResetNoPassMeterStatus();                   //重置没有成功表的返回状态
                int reTryMeter = 0;
                int[] reTryMeterArr = new int[lstNoPassMeter.Count];
                lstNoPassMeter.Values.CopyTo(reTryMeterArr, 0);
                for (int n = 0; n < reTryMeterArr.Length; n++)
                {
                    reTryMeter = reTryMeterArr[n];
                    for (int i = 0; i < ReTryTimes; i++)
                    {
                        //if (CommAdpter.m_IMeterControler.WriteDateTime()
                        if (callBack_OnePara(reTryMeter, strPara))
                        {
                            //最多发送三次，有一次成功就退出
                            break;
                        }
                    }
                }
                if (!isAllReturn())
                    return false;
                noPassCount = NoPassCount();
            }
            return noPassCount!=Helper.MeterDataHelper.Instance.YaoJianMeterCount ;
        }
        #endregion
        #endregion
#endif
    }
}
