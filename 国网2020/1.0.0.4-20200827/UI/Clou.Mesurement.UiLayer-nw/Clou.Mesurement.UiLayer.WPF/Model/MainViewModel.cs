using Mesurement.UiLayer.ViewModel;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Linq;
using Mesurement.UiLayer.Utility.Log;
using System.Windows;
using System;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.WPF.Model
{
    /// 主窗体信息视图
    /// <summary>
    /// 主窗体信息视图
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static MainViewModel instance;
        /// <summary>
        /// 窗体集合的单例
        /// </summary>
        public static MainViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainViewModel();
                }
                return instance;
            }
        }
        /// <summary>
        /// 获取界面原始线程
        /// </summary>
        private SynchronizationContext originalDispatcher = SynchronizationContext.Current;
        /// 初始化要显示的窗体
        /// <summary>
        /// 初始化要显示的窗体
        /// </summary>
        public MainViewModel()
        {
            //必须进入当前线程调用界面更新
            UiInterface.UiMessageArrived += (sender, e) =>
                {
                    if (SynchronizationContext.Current == originalDispatcher && originalDispatcher != null)
                    {
                        CommandFactoryMethod(sender as string);
                    }
                    else
                    {
                        originalDispatcher.Post(obj =>
                            {
                                CommandFactoryMethod(obj as string);
                            }, sender);
                    }
                };
            UiInterface.EventCloseWindow += (sender, e) =>
              {
                  string windowName = sender as string;
                  if (string.IsNullOrEmpty(windowName))
                  {
                      return;
                  }
                  for (int i = 0; i < WindowsAll.Count; i++)
                  {
                      DockWindowDisposable windowTemp = WindowsAll[i];
                      if (windowTemp.Name == windowName)
                      {
                          WindowsAll.Remove(windowTemp);
                          windowTemp.Close();
                          return;
                      }
                  }
              };
        }
        private ObservableCollection<DockWindowDisposable> windowsAll = new ObservableCollection<DockWindowDisposable>();
        /// 所有窗体列表
        /// <summary>
        /// 所有窗体列表
        /// </summary>
        public ObservableCollection<DockWindowDisposable> WindowsAll
        {
            get { return windowsAll; }
            set { windowsAll = value; }
        }

        public override void CommandFactoryMethod(string windowName)
        {
            if (windowName.Contains("数据备份"))
            {
                //if (!BackClienController.IsRunningBackUp)
                {
                    //BackClienController.CloseBackUpClient();
                //   BackClienController.RunBackUpClient();
                }
                return;
            }

            #region commandparameter里面的参数格式:页面名称|页面类的名称,参数,参数......
            if (string.IsNullOrEmpty(windowName))
            {
                return;
            }
            string[] windowNameArray = windowName.Split('|');
            if (windowNameArray.Length < 2)
            {
                return;
            }
            #endregion

            #region 首先遍历现在的窗体,如果与现有窗体,将窗体显示出来
            DockWindowDisposable windowNow = WindowsAll.ToList().Find(item => item.Name == windowNameArray[0]);
            if (windowNameArray[0] != "更多操作")
            {
                for (int i = 0; i < WindowsAll.Count; i++)
                {
                    DockWindowDisposable windowTemp = WindowsAll[i];
                    if (windowTemp.Name == "更多操作")
                    {
                        WindowsAll.Remove(windowTemp);
                        windowTemp.Close();
                        break;
                    }
                }
            }
            if (windowNow is DockWindowDisposable)
            {
                if (!windowNow.IsSelected && windowNameArray[1] != "ViewLog")
                {
                    LogManager.AddMessage(string.Format("切换到{0}窗口.", windowNameArray[0]));
                }
                windowNow.IsSelected = true;
                windowNow.IsAutoHide = false;
                return;
            }
            #endregion

            #region 如果窗体不存在则创建窗体
            string className = string.Format("Mesurement.UiLayer.WPF.View.{0}", windowNameArray[1]);
            Assembly assemblyCurrent = Assembly.Load("Mesurement.UiLayer.WPF");
            object obj = null;
            if (windowNameArray.Length == 2)
            {
                obj = assemblyCurrent.CreateInstance(className);
            }
            else
            {
                string[] paras = windowNameArray[2].Split(',');
                obj = assemblyCurrent.CreateInstance(className, true, BindingFlags.CreateInstance, null, paras, null, null);
            }
            if (obj is DockControlDisposable)
            {
                DockControlDisposable dockControl = (DockControlDisposable)obj;
                DockWindowDisposable windowNew = UIGeneralClass.CreateDockWindow(dockControl);
                WindowsAll.Add(windowNew);
            }
            else
            {
                LogManager.AddMessage(string.Format("创建{0}窗口操作失败.", windowNameArray[0]), EnumLogSource.用户操作日志, EnumLevel.Warning);
            }
            #endregion
        }


    }
}
