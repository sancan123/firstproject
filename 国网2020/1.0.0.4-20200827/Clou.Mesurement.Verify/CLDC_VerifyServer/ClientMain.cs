using System;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter;
using log4net;
using CLDC_VerifyAdapter.Helper;
using CLDC_DataCore.Function;

namespace CLDC_VerifyServer
{
    public class ClientMain
    {
        #region----------变量声明----------

        /// <summary>
        /// 台体号
        /// </summary>
        private int m_intTaiID;
        
        /// <summary>
        /// 写日志线程
        /// </summary>
        private Thread thWriteLog = null;
        
        /// <summary>
        /// 检定消息线程
        /// </summary>
        private Thread thVerifyMsg = null;
        /// <summary>
        /// 检定数据线程
        /// </summary>
        private Thread thVerifyData = null;

        /// <summary>
        /// 上传实时数据线程
        /// </summary>
        private Thread thUpdateRealTimeData = null;

        /// <summary>
        /// 读取标准信息线程
        /// </summary>
        private Thread thReadStdInfo = null;
        
        /// <summary>
        /// 默认表位数
        /// </summary>
        private int m_BwCount = 6;
        /// <summary>
        /// 消息锁
        /// </summary>
        private bool LockMsg = false;
        /// <summary>
        /// 数据锁
        /// </summary>
        private bool LockData = false;
        /// <summary>
        /// 是否是DEMO
        /// </summary>
        private bool isDemo;
        
        /// <summary>
        /// 当前是否有未响应的操作
        /// </summary>
        private bool isProcessing = false;
        #endregion

        #region ------------系统初始化-ClientMain-----------
        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public void Initialize(int TaiID, int BwCount, bool Demo)
        {
            #region 加载系统配置
            Console.WriteLine("加载系统配置...");
            if (GlobalUnit.g_SystemConfig == null)
             {
                GlobalUnit.g_SystemConfig = new CLDC_DataCore.SystemModel.SystemInfo();
                GlobalUnit.g_SystemConfig.Load();
            }
            //获取运行时的配置值

            m_BwCount = BwCount;// GlobalUnit.GetConfig(Variable.CTC_BWCOUNT, 24);             //台体表位数
            isDemo = Demo;// GlobalUnit.IsDemo;                                             //是否是演示版本
            m_intTaiID = TaiID;// GlobalUnit.GetConfig(Variable.CTC_DESKNO, 1);
            
            #endregion

            #region 初始化队列组件
            Console.WriteLine("初始化队列组件...");
            GlobalUnit.Log = new CLDC_DataCore.Function.RunLog();       //日志队列
            //GlobalUnit.FrameLog = new CLDC_DataCore.Function.RunLogFrame();//帧日志队列
            GlobalUnit.g_DataControl = new VerifyMsgControl();                      //数据队列
            GlobalUnit.g_MsgControl = new VerifyMsgControl();                       //消息队列            
            GlobalUnit.g_485DataControl = new VerifyMsgControl();                   //485数据队列
            GlobalUnit.g_RealTimeDataControl = new RealTimeMsgControl();            //实时数据队列

            GlobalUnit.g_MsgControl.SleepTime = 5;
            #endregion

            #region 加载检定数据
            Console.WriteLine("加载检定数据...");
            GlobalUnit.g_CUS = new CLDC_DataCore.CusModel(m_BwCount, m_intTaiID);
            GlobalUnit.g_CUS.Load();
            
            //加载特殊配置参数
            //LoadCache();
            #endregion

            #region 初始化检定组件----------
            //为检定组件指定日志输出组件
            Console.WriteLine("初始化检定组件...");
             Adapter.Instance.BwCount = m_BwCount;                                           //检定控制器
            LogHelper.Instance.Loger = Logger;                         //日志组件
            MeterDataHelper.Instance.Init();                           //初始化表数据

            #endregion

            #region 启动线程
            Console.WriteLine("系统核心组件初始化完成...");
            
            ///*日志线程k*/
            thWriteLog = new Thread(new ThreadStart(GlobalUnit.Log.DoWork));
            thWriteLog.IsBackground = true;
            thWriteLog.Name = "WriteLog";
            thWriteLog.Start();
            //帧日志线程
            //thFrameLog = new Thread(new ThreadStart(RunLogFrame.Instance.DoWork));
            //thFrameLog.IsBackground = true;
            //thFrameLog.Name = "FrameLog";
            ////GlobalUnit.FrameLog.DeleteAll();//TODO:先不删除
            //thFrameLog.Start();//TODO:这里先不启动,目前测试在这里启动，要放到调试控制中,界面上控制

            //消息队列
            GlobalUnit.g_MsgControl.IsMsg = true;
            GlobalUnit.g_MsgControl.ShowMsg += new VerifyMsgControl.OnShowMsg(ShowMsg);
            GlobalUnit.g_MsgControl.SleepTime = 2;
            //数据队列
            GlobalUnit.g_DataControl.IsMsg = false;
            GlobalUnit.g_DataControl.ShowMsg += new VerifyMsgControl.OnShowMsg(ShowData);
            GlobalUnit.g_DataControl.SleepTime = 10;
            
            ///*上传实时消息线程*/
            thUpdateRealTimeData = new Thread(new ThreadStart(GlobalUnit.g_RealTimeDataControl.DoWork));
            thUpdateRealTimeData.IsBackground = true;
            thUpdateRealTimeData.Name = "thUpdateRealTimeData";
            thUpdateRealTimeData.Start();
            ///*检定消息线程*/
            thVerifyMsg = new Thread(new ThreadStart(GlobalUnit.g_MsgControl.DoWork));
            thVerifyMsg.IsBackground = true;
            thVerifyMsg.Name = "thVerifyMsg";
            thVerifyMsg.Start();
            ///*检定数据线程*/
            thVerifyData = new Thread(new ThreadStart(GlobalUnit.g_DataControl.DoWork));
            thVerifyData.IsBackground = true;
            thVerifyData.Name = "VerifyData";
            thVerifyData.Start();
            //485数据线程
            Thread thShowData = new Thread(GlobalUnit.g_485DataControl.DoWork);
            thShowData.IsBackground = true;
            thShowData.Name = "Show485Data";
            thShowData.Start();
            
            /*标准信息读取线程*/
            thReadStdInfo = new Thread(readStdInfo);
            thReadStdInfo.IsBackground = true;
            thReadStdInfo.Name = "ReadStdInfo";
            //thReadStdInfo.Start();    //取消标准信息读取线程
            #endregion

            //CLDC_DataCore.Function.ThreadCallBack.Call(new CLDC_DataCore.Function.CallBack(thConnectEquip), 6000);
            ThreadCallBack.Call(new CallBack(StartReadStdThread), 12000);
        }

        private void StartReadStdThread()
        {
            if (thReadStdInfo.IsAlive == false)
            {
                thReadStdInfo.Start();    //标准信息读取线程
            }
        }

        private void thConnectEquip()
        {
            ConnectEquip(true);
            
        }

        #endregion

        #region -----------属       性----------

        private ILog logger = null;
        /// <summary>
        /// 系统日志组件
        /// </summary>
        public ILog Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        #endregion

        #region----------更新客户端电能表总模型----------
        private object objLockModifyDNBModule = new object();
        private void UpdateMeterGroup(CLDC_DataCore.Model.DnbModel.DnbGroupInfo MeterData)
        {
            //UpdateMeterGroup(MeterData, false);
            //GlobalUnit.g_MsgControl.OutMessage();
        }

        private void UpdateMeterGroup(CLDC_DataCore.Model.DnbModel.DnbGroupInfo MeterData, bool isSendToServer)
        {
            //lock (objLockModifyDNBModule)
            //{
            //    GlobalUnit.g_CUS.DnbData = MeterData;
                
            //}
        }


        #endregion

        #region----------框架操作指令----------
        //停止检定器
        private void Frame_StopAdpater()
        {
            //GlobalUnit.g_MsgControl.OutMessage(m_intTaiID.ToString() + "号装置停止检定", false, Cus_MessageType.语音消息);
            //stopAdpater();                          //停止掉检定器
        }
        private void Frame_ReadPara()
        {
            //GlobalUnit.g_MsgControl.OutMessage(m_intTaiID.ToString() + "号装置读取参数", false, Cus_MessageType.语音消息);
            //CLDC_Comm.ThreadPool.QueueUserWorkItem(new WaitCallback(Do_ReadPara));
                                     
        }
        
        //启动检定器
        private void Frame_StartAdpater(int ItemID)
        {
            //GlobalUnit.g_MsgControl.OutMessage(m_intTaiID.ToString() + "号装置开始检定", false, Cus_MessageType.语音消息);
            //startAdpater(ItemID);
        }
        #endregion

        

        #region----------操作函数:启动/停止检定器----------
        /// <summary>
        /// 停止检定器
        /// </summary>
        /// <returns>停止检定器是否成功</returns>
        private bool stopAdpater()
        {
            //WriteInfo("开始停止检定控制组件...");
            //Adapter.Instance.StopVerify();
            
            ////修改本地数据标识
            //GlobalUnit.g_CUS.DnbData.CheckState = CLDC_Comm.Enum.Cus_CheckStaute.停止检定;
            
            //WriteInfo("停止检定控制器完成");
            return true;
        }

        /// <summary>
        /// 启动检定器
        /// </summary>
        /// <param name="ItemID">开始检定指定项目</param>
        /// <returns>开始检定启动是否成功</returns>
        private bool startAdpater(int ItemID)
        {
            bool ret = false;            
            //重复跳点检测
            if (isProcessing)
            {
                WriteInfo("上一次跳点操作还没有完成，当前切换检定点操作无效。");
                GlobalUnit.Logger.Debug("上一次操作没有完成，已终止当前请求");
                GlobalUnit.g_MsgControl.OutMessage("上一次操作还没有完成，请等待上一次操作完成后再试", false, CLDC_Comm.Enum.Cus_MessageType.提示消息);
                //return false;
            }
            isProcessing = true;
            //if (m_VerifyAdpater != null)
            //{
            CLDC_Comm.Enum.Cus_CheckStaute preState = GlobalUnit.g_CUS.DnbData.CheckState;         //记录当前的检定状态操作,因为StopVerify会改变检定状态 
            Adapter.Instance.StopVerify();
            Adapter.Instance.CurItem = ItemID;
            GlobalUnit.g_CUS.DnbData.CheckState = preState;                         //状态还原
            ret = Adapter.Instance.Verify();
            //}
            isProcessing = false;
            return ret;
        }

        #endregion


        #region ConnectServer() 连接到服务器

        //读取标准表信息
        private void readStdInfo()
        {
            //return;
            
            if (GlobalUnit.IsDemo) return;
            int time = 2000;
            //try
            //{
            //    time = int.Parse(GlobalUnit.GetConfig(Variable.CTC_STDMETERREADTIME, "2000"));
            //}
            //catch
            //{
            //}
            while (true)
            {
                if (GlobalUnit.ApplicationIsOver)
                    break;
                bool temp = EquipHelper.Instance.isConnected;
                if (!isDemo && GlobalUnit.EnableReadStd && temp)
                {
                    CLDC_DataCore.Struct.StPower tagPower = EquipHelper.Instance.ReadPowerInfo();
                    //更新监视器
                    UpdatePowerInfo(tagPower);
                    
                }
                Thread.Sleep(time);
            }
        }
        /// <summary>
        /// 标准表更新信息,
        /// </summary>
        /// <param name="strInfo"></param>
        private void UpdatePowerInfo(CLDC_DataCore.Struct.StPower tagPower)
        {
            //记录全局数据
            GlobalUnit.g_StrandMeterU[0] = tagPower.Ua;
            GlobalUnit.g_StrandMeterU[1] = tagPower.Ub;
            GlobalUnit.g_StrandMeterU[2] = tagPower.Uc;
            GlobalUnit.g_StrandMeterI[0] = tagPower.Ia;
            GlobalUnit.g_StrandMeterI[1] = tagPower.Ib;
            GlobalUnit.g_StrandMeterI[2] = tagPower.Ic;

            GlobalUnit.g_StrandMeterP[0] = tagPower.P; //有功
            GlobalUnit.g_StrandMeterP[1] = tagPower.Q; //无功
            GlobalUnit.g_StrandMeterP[2] = tagPower.S; //视在


            GlobalUnit.g_StrandMeterPFY[0] = tagPower.Pa;
            GlobalUnit.g_StrandMeterPFY[1] = tagPower.Pb;
            GlobalUnit.g_StrandMeterPFY[2] = tagPower.Pc;


            GlobalUnit.g_StrandMeterQFY[0] = tagPower.Qa;
            GlobalUnit.g_StrandMeterQFY[1] = tagPower.Qb;
            GlobalUnit.g_StrandMeterQFY[2] = tagPower.Qc;
            int taiid = 0;
            //try
            //{
            //    taiid = int.Parse(GlobalUnit.GetConfig(Variable.CTC_DESKNO, "1"));
            //}
            //catch
            //{
            //}
            //显示到客户端
            try
            {
                string powerText = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}||{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}|{23}|{24}|{25}|{26}||||{27}||{28}",
                tagPower.Ua, tagPower.Ub, tagPower.Uc,
                tagPower.Ia, tagPower.Ib, tagPower.Ic,
                tagPower.Phi_Ua, tagPower.Phi_Ub, tagPower.Phi_Uc,
                tagPower.Phi_Ia, tagPower.Phi_Ib, tagPower.Phi_Ic,
                tagPower.Phi_Ia, tagPower.Phi_Ib, tagPower.Phi_Ic,
                tagPower.Pa,tagPower.Pb,tagPower.Pc, 
                tagPower.P,
                tagPower.Qa,tagPower.Qb,tagPower.Qc,
                tagPower.Q,
                tagPower.Sa,tagPower.Sb,tagPower.Sc,
                tagPower.S, 
                tagPower.COS, tagPower.Freq);
                MessageController.Instance.AddMonitorMessage(EnumMonitorType.MeterStandard,powerText);
            }
            catch (Exception ex)
            {
                ErrorLog.Write(ex);
            }
        }
        #endregion


        /// <summary>
        /// 联机线程
        /// </summary>
        private void thConnectServer()
        {
            //int LoopCount = 1;
            //while (true)
            //{
            //    if (GlobalUnit.ApplicationIsOver)
            //        break;
            //    //保存总模型
            //    if (++LoopCount % 2 == 0)
            //    {
            //        GlobalUnit.g_CUS.DnbData.Save();
            //        LoopCount %= 100000;
            //    }


            //    Thread.Sleep(2000);
            //}
        }

        #region ----------辅助函数----------
        private void LoadCache()
        {
            //GlobalUnit.g_SystemConfig.Load();
            //添加每个误差点检定次数
            //GlobalUnit.g_CUS.DnbData.WcCheckNumic = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_BASICERROR, 2);
            //GlobalUnit.g_CUS.DnbData.PcCheckNumic = GlobalUnit.GetConfig(Variable.CTC_WC_TIMES_WINDAGE, 5);

            //GlobalUnit.g_CUS.DnbData.CheckState = CLDC_Comm.Enum.Cus_CheckStaute.停止检定;
        }
        //联机/脱机控制
        private void ConnectEquip(bool bLink)
        {
            
            if (GlobalUnit.g_CUS.DnbData.CheckState != CLDC_Comm.Enum.Cus_CheckStaute.停止检定)
            {
                GlobalUnit.g_MsgControl.OutMessage("当前正在检定中，不能够联机/脱机.如需操作，请先停止检定", false, Cus_MessageType.提示消息);
                return;
            }
            if (bLink)
            {
                bool connectStatus = CLDC_VerifyAdapter.Helper.EquipHelper.Instance.Link();
                
                if (connectStatus)
                {
                    GlobalUnit.EquipConnectedState = Cus_NetState.Connected;
                    GlobalUnit.g_MsgControl.OutMessage("联机成功!", false);
                }
                else
                {
                    //GlobalUnit.g_MsgControl.OutMessage("联机失败", false);
                }
            }
            else
            {
                if (CLDC_VerifyAdapter.Helper.EquipHelper.Instance.UnLink())
                {
                    GlobalUnit.EquipConnectedState = Cus_NetState.DisConnected;
                    GlobalUnit.g_MsgControl.OutMessage("脱机成功!", false);
                }
                else
                {
                    GlobalUnit.g_MsgControl.OutMessage("脱机失败", false);
                }
            }

            GlobalUnit.g_CUS.DnbData.CheckState = CLDC_Comm.Enum.Cus_CheckStaute.停止检定;
        }
        /// <summary>
        /// 升源或是关源
        /// </summary>
        /// <param name="bOn"></param>
        private void SetPower(bool bOn)
        {
            //if (bOn)
            //{
            //    try
            //    {
            //        CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            //    }
            //    catch (Exception e)
            //    {
            //        GlobalUnit.g_MsgControl.OutMessage(e.Message, false, Cus_MessageType.提示消息);
            //    }
            //}
            //else
            //{
            //    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOff();
            //}
            //}
        }

        #endregion

        #region ---------检定器消息/数据处理----------
        /// <summary>
        /// 检定消息处理
        /// </summary>
        /// <param name="sourceAdpater">消息发出者</param>
        /// <param name="MessageArgs">消息参数</param>
        private void ShowMsg(object sourceAdpater, object MessageArgs)
        {
            if (LockMsg)//如果消息锁打开，则不再处理消息
            {
                GlobalUnit.g_MsgControl.ClearCache();
                GlobalUnit.Log.WriteLog(this, "ShowMsg", "消息锁已经打开，不再处理消息");
                return;
            }
        }

        /// <summary>
        /// 检定数据处理
        /// </summary>
        /// <param name="sourceAdpater">消息发出者</param>
        /// <param name="VerifyDataArgs">消息参数</param>
        private void ShowData(object sourceAdpater, object VerifyDataArgs)
        {
            if (LockData)
            {
                GlobalUnit.g_DataControl.ClearCache();
                GlobalUnit.Log.WriteLog(this, "ShowData", "消息锁已经打开，不再处理消息");
                return;
            }

            
        }
        
        /// <summary>
        /// 多功能协议配置
        /// </summary>
        private void ProtocolSetup()
        {
            if (GlobalUnit.g_CUS.DnbData.CheckState != Cus_CheckStaute.停止检定)
            {
                GlobalUnit.g_MsgControl.OutMessage("由于配置协议需要脱机后重新联机，请先停止检定后再操作",
                    false, Cus_MessageType.提示消息);
                return;
            }
            if (CLDC_VerifyAdapter.Helper.EquipHelper.Instance.UnLink())
            {
            }
            else
            {
                GlobalUnit.g_MsgControl.OutMessage("断开当前台体硬件连接失败，请稍候重试", false, Cus_MessageType.提示消息);
            }
        }
        #endregion

        #region -----------系统运行日志-----------
        /// <summary>
        /// 输出运行时消息 
        /// </summary>
        /// <param name="message">消息内容</param>
        private void WriteInfo(object message)
        {
            if (Logger == null) return;
            Logger.Info(message);
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        private void WriteWarm(object message)
        {
            if (Logger == null) return;
            Logger.Warn(message);
        }
        #endregion
    }
}
