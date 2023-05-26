using System;
using System.Windows;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Log;
using System.Threading;
using Mesurement.UiLayer.DAL.Config;
using System.Windows.Threading;
using Mesurement.UiLayer.ViewModel.Time;
using Mesurement.UiLayer.ViewModel.CodeTree;
using Mesurement.UiLayer.Utility.DogPackage;
using System.Security.Principal;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private DispatcherTimer timer = new DispatcherTimer();
        /// 程序启动前要执行的动作
        /// <summary>
        /// 程序启动前要执行的动作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeDog();
            UiInterface.UiDispatcher = SynchronizationContext.Current;
            CodeTreeViewModel.Instance.InitializeTree();
            timer.Interval = new TimeSpan(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
            #region 初始化日志
            LogViewModel.Instance.Initialize();
            LogManager.LogMessageArrived += (sender, args) =>
            {
                if (sender is LogModel)
                {
                    LogViewModel.Instance.AddLogModel(sender as LogModel);
                }
            };
            #endregion
            ConfigHelper.Instance.LoadAllConfig();
            EquipmentData.LastCheckInfo.LoadLastCheckInfo();
            base.OnStartup(e);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeMonitor.Instance.Timer_Elapsed();
        }

        /// 界面初始化标记
        /// <summary>
        /// 界面初始化标记
        /// </summary>
        private bool flagInitialize = false;
        /// 程序可见后要执行的动作
        /// <summary>
        /// 程序可见后要执行的动作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnActivated(EventArgs e)
        {
            if (!flagInitialize)
            {
                UiInterface.UiDispatcher = SynchronizationContext.Current;
                flagInitialize = true;
            }
            base.OnActivated(e);
        }

        private bool flagUseDog = false;
        private void InitializeDog()
        {
            if (!flagUseDog)
            {
                return;
            }
            string errorString = "";
            if (!DogHelper.Instance.Run(out errorString))
            {
                MessageBox.Show(errorString, "找不到加密狗!");
                Current.Shutdown();
            }
            DogHelper.Instance.EventLostDog += Instance_EventLostDog;
        }

        private void Instance_EventLostDog(object sender, EventArgs e)
        {
            DogEventArgs args = e as DogEventArgs;
            if (args != null)
            {
                if (!args.FlagExit)
                {
                    LogManager.AddMessage(args.ErrorString, EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                }
                else
                {
                    LogManager.AddMessage(args.ErrorString, EnumLogSource.用户操作日志, EnumLevel.Error);
                    Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (Current.MainWindow != null)
                        {
                            MessageBox.Show("找不到加密狗,程序将退出运行!", "找不到加密狗");
                            Current.Shutdown();
                        }
                    }));
                }
            }
        }
    }
}
