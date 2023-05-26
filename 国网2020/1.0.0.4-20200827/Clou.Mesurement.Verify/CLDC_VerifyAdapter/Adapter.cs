
using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_DataCore.Model.DgnProtocol;
using CLDC_Comm.BaseClass;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 检定控制器
    /// 根据当前检定项目创建对应检定器，启动检定线程完成检定
    /// </summary>
    public class Adapter : SingletonBase<Adapter>
    {
        #region ----------模块变量声明----------
        /// <summary>
        /// 装置号
        /// </summary>
        private int m_intTaiID;
        /// <summary>
        /// 运行标志,用于确定当前检定线程是否被外部要求停止
        /// </summary>
        /// <remarks>
        /// 当检定开始时，本标志会被拉高，当外部要求当前检定停止时，本标志被拉低.
        /// </remarks>
        private bool isRunning = false;

        /// <summary>
        /// 运行标志,用来确定当前检定操作是不是真的已经完成退出
        /// </summary>
        /// <remarks>
        /// 当检定线程开始时，本标志被拉低，检定线程退出后本标志被拉高
        /// 可以通过本标志检测上一次检定是否真的已经完全完成.
        /// </remarks>
        private bool RunFlag = false;
        #endregion

        #region ------------检定控制:Verify()------------
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <returns>启动检定是否成功</returns>
        Thread newThread;
        public bool Verify()
        {
            //m_intTaiID = int.Parse(GlobalUnit.g_SystemConfig.SystemMode.getValue(Variable.CTC_DESKNO));

            //以下进入检定
            int startPos = CurItem;
            int endPos = CurItem + 1;
            Helper.LogHelper.Instance.Loger.Debug("开始进入VerifyAdapter.Adapter");
            lock (this)
            {
                if (RunFlag)
                {
                    Helper.LogHelper.Instance.Loger.Debug("第一次检定还没有完成，不能重复提交检定请求");
                    return false;                                                  //防止多次重复调用
                    //RunFlag = true;
                }
            }
            //更新一下电能表数据
            Helper.MeterDataHelper.Instance.Init();
            //更新多功能协议
            UpdateMeterProtocol();
            //设置全局标识
            
            //检测是否是连续检定
            bool isConsecutive = ((GlobalUnit.g_CUS.DnbData.CheckState & Cus_CheckStaute.单步检定) != Cus_CheckStaute.单步检定);
            if (isConsecutive)
            {
                endPos = GlobalUnit.g_CUS.DnbData.CheckPlan.Count;
            }


            //lsx--检定线程
            if (newThread != null)
                if (newThread.IsAlive)
                {
                    newThread.Abort();
                }
            newThread = new Thread(new ParameterizedThreadStart(callBackVerifyThread));
            int[] intObjest = new int[] { startPos, endPos };
            object B = intObjest;
            newThread.Start(B);
            //  ThreadPool.QueueUserWorkItem(new WaitCallback(callBackVerifyThread), new int[] { startPos, endPos }); 



            return true;
        }

        /// <summary>
        /// 更新电能表协议信息
        /// </summary>
        public void UpdateMeterProtocol()
        {
            DgnProtocolInfo[] protocols = Helper.MeterDataHelper.Instance.GetAllProtocols();
            string[] meterAddress = Helper.MeterDataHelper.Instance.GetMeterAddress();
            string[] meterAddress_MAC = Helper.MeterDataHelper.Instance.GetMeterAddress_MAC();
            MeterProtocolAdapter.Instance.Initialize(protocols, meterAddress, meterAddress_MAC);
        }

        /// <summary>
        /// 检定线程控制
        /// </summary>
        /// <param name="patas">线程参数，分别为开始ID和结束ID</param>
        private void callBackVerifyThread(object paras)//
        {


            isRunning = true;
            int[] args = (int[])paras;
            Helper.LogHelper.Instance.Loger.Debug("进入检定线程，设置当前运行标志isRunning为True");

            string[] EbNo = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                EbNo[i] = "--" + (i + 1).ToString().PadLeft(2, '0') + "--";
            }

            for (int currentItemID = args[0]; currentItemID < args[1]; currentItemID++)
            {
                CurItem = currentItemID;
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                {
                    isRunning = false;
                    GlobalUnit.g_CUS.DnbData.CheckState = Cus_CheckStaute.检定;
                }
                if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.错误)
                {
                    StopVerify();
                }
                if (!isRunning) break;                                                  //中途是否已经停止
                GlobalUnit.g_CheckTimeCurrentStart = DateTime.Now;
                
                doVerify();
                //启动项目检定
                GlobalUnit.g_CheckTimeCurrentEnd = DateTime.Now;
                
                //if (GlobalUnit.MeterResult)
                //    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetEquipmentThreeColor(20, 1);
                //else
                //    CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetEquipmentThreeColor(18, 1);

                if (!isRunning) break;                                                  //中途是否已经停止
                Thread.Sleep(500);//连续跳转的间隔TODO:配置
            }
            Console.WriteLine("Adapter已完成检定");
            GlobalUnit.g_CUS.DnbData.CheckState = Cus_CheckStaute.停止检定;

            if (isRunning)                                                              //检测是正常完毕还是强制停止
            {
                //if (GlobalUnit.GetConfig(Variable.CTC_ISCONTROL, Variable.CTG_CONTROLMODEL_BECONTROL) == Variable.CTG_CONTROLMODEL_BECONTROL)
                //{
                //    MessageController.Instance.AddMessage(m_intTaiID.ToString() + "号装置当前项目检定完毕!");
                //}
                //else
                //{
                //    MessageController.Instance.AddMessage(m_intTaiID.ToString() + "号装置当前项目检定完毕!");
                //}
            }
            else
            {
                Helper.LogHelper.Instance.WriteDebug("当前检定项目被手动中止!");
                MessageController.Instance.AddMessage(m_intTaiID.ToString() + "号装置当前检定项目被手动中止");
            }
            isRunning = false;                                                          //恢复数据标识
            RunFlag = false;                                                            //恢复运行标识,以允许下一次操作进入
            //关源
            Helper.EquipHelper.Instance.PowerOff();
            //Helper.EquipHelper.Instance.WriteControlCmdData(false);
            Helper.EquipHelper.Instance.Stop();
            //CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetEquipmentThreeColor(19, 1);
            Thread.Sleep(300);
            //刷新一下界面
            MessageController.Instance.AddMessage("检定完成。");
            Helper.LogHelper.Instance.WriteDebug("检定线程已经完全退出");
            GlobalUnit.TestThreadIsRunning = false;
        }

        /// <summary>
        /// 开始检定[从指定项目开始]
        /// 2009-1-9 修改
        /// 此检定入口不再支持外部调用，如果需要指定检定ID，请直接设置CurItem后直接调用Verify
        /// </summary>
        private void doVerify()
        {
            GlobalUnit.UpdateActiveID(CurItem);        //更新全局项目ID
            //更新全局项目ID
            MessageController.Instance.AddMessage("正在跳转检定点到" + CurPlan.ToString());
            Helper.LogHelper.Instance.WriteInfo("跳转检定点到" + CurPlan.ToString());
            VerifyFactory vFactory = new VerifyFactory(CurPlan);                        //初始化检定控制器
            m_curControl = vFactory.GetVerifyControl();                                 //获取检定器实例
            if (m_curControl == null)
            {
                if (CurPlan.ToString() != "外观检查")
                    MessageController.Instance.AddMessage("创建检定控制器失败，如果当前是多功能项目请检测是否为电能表指定了通讯协议");
                return;
            }
            m_curControl.DoVerify();
            Helper.LogHelper.Instance.Loger.Debug("当前检定完成:" + m_curControl.ToString());
            MessageController.Instance.AddMessage("检定完成");
        }

        /// <summary>
        /// 停止检定
        /// </summary>
        /// <remarks>
        /// 停止检定流程：
        /// 第一步：设置检定控制器运行标志为False以终止当前方案循环
        /// 第二步：设置当前检定控制器运行标志为Flase以终止当前检定逻辑
        /// 第三步：设置设备控制单元运行状态为False以终止设置控制操作
        /// </remarks>
        public void StopVerify()
        {
            isRunning = false;                                                      //先停止检定控制器循环
            if (m_curControl != null)
            {
                //停止检定控制器
                Helper.LogHelper.Instance.WriteDebug("开始停止设备控制");
                Helper.EquipHelper.Instance.Stop();                                     //停止设备控制器
                MeterProtocolAdapter.Instance.Stop();
                Helper.LogHelper.Instance.WriteDebug("开始等待检定方法完成");
                Helper.LogHelper.Instance.WriteDebug("开始停止当前检定项目:" + CurItem.ToString());
                m_curControl.Stop = true;
                Helper.EquipHelper.Instance.PowerOff();
                int wiringMode = 0;
                if (GlobalUnit.clfs == Cus_Clfs.单相)
                {
                    wiringMode = 5;
                }
                else if (GlobalUnit.clfs == Cus_Clfs.三相四线)
                {
                    wiringMode = 0;
                }
                else
                {
                    wiringMode = 1;
                }
                CLDC_DeviceDriver.DeviceControl.Instance.Dev_DeviceControl[0].SetWiringMode(0, wiringMode);


                if (GlobalUnit.IsCL3112)
                {
                    Helper.EquipHelper.Instance.FuncMstate( 0xFE);
                }
                while (RunFlag && !m_curControl.IsRunOver)                                                         //等待上一次操作完成    
                {
                    Helper.EquipHelper.Instance.Stop();                                     //停止设备控制器
                    Thread.Sleep(1000);
                    MeterProtocolAdapter.Instance.Stop();
                    Helper.LogHelper.Instance.WriteDebug("检定方法还没有完成");
                    Thread.Sleep(1000);
                }
            }
            Helper.LogHelper.Instance.WriteDebug("停止检定成功");
        }
        #endregion

        #region ------------属     性------------
        private int m_BwCount;
        /// <summary>
        /// 台体表位数量
        /// </summary>
        public int BwCount
        {
            get { return m_BwCount; }
            set
            {
                m_BwCount = value;
                MeterProtocolAdapter.Instance.SetBwCount(value);
                //Helper.EquipHelper.Instance.Initialize(value);
                //Helper.VerifyDemoHelper.Instance.BWCount = value;
            }
        }

        private int m_CurItem = 0;
        /// <summary>
        /// 当前检定项目编号
        /// </summary>
        public int CurItem
        {
            get { return m_CurItem; }
            set { m_CurItem = value; }
        }

        /// <summary>
        /// 返回当前检定方案
        /// </summary>
        public object CurPlan
        {
            get
            {
                if (CurItem >= GlobalUnit.g_CUS.DnbData.CheckPlan.Count)
                    return null;
                if (CurItem < 0) return null;
                return GlobalUnit.g_CUS.DnbData.CheckPlan[CurItem];
            }
        }

        private VerifyBase m_curControl = null;
        /// <summary>
        /// 当前控制器,用于停止检定时控制
        /// </summary>
        public VerifyBase CurControl
        {
            get { return m_curControl; }
        }

        #endregion

        #region --------公共方法----------
        /// <summary>
        /// 设置当前操作完成[用于走字输入电量等]
        /// </summary>
        /// <returns></returns>
        public void CurActionOver()
        {
            IWorkDone iWorkDone = m_curControl as IWorkDone;
            if (iWorkDone != null)
                iWorkDone.WorkDone();
        }
        /// <summary>
        /// 获取配置窗体
        /// </summary>
        public void ShowConfigForm()
        {
        }
        /// <summary>
        /// 显示RS485配置信息
        /// </summary>
        public void ShowRs485Configs()
        {
            //CLDC_MeterProtocol.Settings.frmDgnSetting.Instance.ShowD();

        }

        #endregion
    }
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////