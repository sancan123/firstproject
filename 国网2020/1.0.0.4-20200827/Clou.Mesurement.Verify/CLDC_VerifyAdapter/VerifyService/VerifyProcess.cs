using CLDC_Comm.BaseClass;
using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Helper;
using System;
using System.Threading;

namespace CLDC_VerifyAdapter.VerifyService
{
    /// 检定线程控制器
    /// <summary>
    /// 检定线程控制器
    /// </summary>
    public class VerifyProcess : SingletonBase<VerifyProcess>
    {
        /// <summary>
        /// 检定心跳定时器,检定过程中会发出通知
        /// </summary>
        private System.Timers.Timer timer = new System.Timers.Timer();
        /// 构造函数,开始线程
        /// <summary>
        /// 构造函数,开始线程
        /// </summary>
        public VerifyProcess()
        {
            //20秒检定心跳
            timer = new System.Timers.Timer(10000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            threadVerify = new Thread(() => ThreadProcess());
            threadVerify.Start();
        }
        /// <summary>
        /// 20秒的检定心跳
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //只发正在检定,不发停止检定
            if (IsBusy)
            {
                MessageController.Instance.NotifyIsChecking("1");
            }
            else
            {
                //MessageController.Instance.NotifyIsChecking("0");
            }
        }
        #region 私有成员
        /// 检定器工厂
        /// <summary>
        /// 检定器工厂
        /// </summary>
        private VerifyFactory factory = new VerifyFactory();
        /// 当前的检定项
        /// <summary>
        /// 当前的检定项
        /// </summary>
        private VerifyBase verifyControl = null;
        /// 检定完毕以后的电源控制
        /// <summary>
        /// 检定完毕以后的电源控制
        /// </summary>
        private string powerOperation = "0";
        /// 等待句柄
        /// <summary>
        /// 等待句柄
        /// </summary>
        private AutoResetEvent waitHandle = new AutoResetEvent(true);
        private Thread threadVerify = null;
        /// 执行检定线程
        /// <summary>
        /// 执行检定线程
        /// </summary>
        private void ThreadProcess()
        {
            while (true)
            {
                if (exitFlag)
                {
                    break;
                }
                try
                {
                    if (verifyControl != null)
                    {
                        GlobalUnit.g_CUS.DnbData.CheckState = Cus_CheckStaute.检定;
                        //PreVerify();
                        IsBusy = true;
                        verifyControl.DoVerify();
                        AfterVerify();
                        verifyControl.FinishVerify();
                    }
                }
                catch (Exception e)
                {
                    MessageController.Instance.AddMessage("检定异常" + e.Message, 6, 92);
                    AfterVerify();
                    verifyControl.FinishVerify();
                }
                IsBusy = false;
                waitHandle.WaitOne();
            }
            threadVerify.Abort();
        }
        /// 检定完毕以后的动作,主要包括是否关源
        /// <summary>
        /// 检定完毕以后的动作,主要包括是否关源
        /// </summary>
        private void AfterVerify()
        {
            switch (powerOperation)
            {
                case "0":
                    Helper.EquipHelper.Instance.Stop();                                     //停止设备控制器
                    MeterProtocolAdapter.Instance.Stop();
                    break;
                case "1":
                    Helper.EquipHelper.Instance.Stop();                                     //停止设备控制器
                    MeterProtocolAdapter.Instance.Stop();
                    EquipHelper.Instance.PowerOff();
                    break;
                default:
                    Helper.EquipHelper.Instance.Stop();                                     //停止设备控制器
                    MeterProtocolAdapter.Instance.Stop();
                    EquipHelper.Instance.PowerOff();
                    break;
            }

            string[] EbNo = new string[GlobalUnit.g_CUS.DnbData._Bws];
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                EbNo[i] = "--" + (i + 1).ToString().PadLeft(2, '0') + "--";
            }
            MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join("|", EbNo));
        }
        #endregion

        #region 公开成员
        /// 当前检定点编号
        /// <summary>
        /// 当前检定点编号
        /// </summary>
        public string CurrentKey { get; private set; }
        public string CurrentName { get; private set; }
        /// 检定线程控制器状态
        /// <summary>
        /// 检定线程控制器状态
        /// </summary>
        public bool IsBusy { get; private set; }
        /// <summary>
        /// 开始检定
        /// </summary>
        /// <param name="itemKey">检定点编号</param>
        /// <param name="verifyPara">检定参数</param>
        /// <param name="powerOption">检定完毕以后的电源控制</param>
        /// <returns>操作结果</returns>
        public bool StartVerify(string itemName, string itemKey, string className, string verifyPara, string powerOption)
        {
            //powerOperation = powerOption;
            powerOperation = "1";
            verifyControl = factory.GetVerifyControl(className, verifyPara);
            if (verifyControl != null)
            {
                CurrentKey = itemKey;
                CurrentName = itemName;
                waitHandle.Set();
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("检定服务未实现检定器:{0} 的功能", CurrentName), 6, 2);
            }
            return verifyControl != null;
        }
        /// 停止检定
        /// <summary>
        /// 停止检定
        /// </summary>
        public void StopVerify(string itemKey)
        {
            powerOperation = "1";
            if (verifyControl != null)
            {
                if (IsBusy)
                {
                    EquipHelper.Instance.PowerOff();
                    verifyControl.Stop = true;
                  //  GlobalUnit.IsStop = true;
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
                }
                else
                {
                    MessageController.Instance.NotifyVerifyFinished();
                }
            }
            else
            {
                Helper.EquipHelper.Instance.PowerOff();
                MessageController.Instance.NotifyVerifyFinished();
            }
        }

        private bool exitFlag = false;
        /// <summary>
        /// 退出时调用
        /// </summary>
        public void Exit()
        {
            timer.Stop();
            timer.Dispose();
            exitFlag = true;
            waitHandle.Set();
        }
        #endregion
    }
}
