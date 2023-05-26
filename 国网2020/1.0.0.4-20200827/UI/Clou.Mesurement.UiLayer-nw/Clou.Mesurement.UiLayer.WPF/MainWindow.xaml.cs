using System.Windows;
using DevComponents.WpfDock;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.WPF.Model;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Media;
using System.ComponentModel;
using Mesurement.UiLayer.ViewModel.Time;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mesurement.UiLayer.WPF.Skin;
using System.Windows.Threading;
using Mesurement.UiLayer.Utility;

namespace Mesurement.UiLayer.WPF
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow
    {
        private WindowVerifyControl verifyWindow = new WindowVerifyControl();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.Instance;
            MainViewModel.Instance.WindowsAll.CollectionChanged += WindowsAll_CollectionChanged;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            verifyWindow.Show();
            verifyWindow.Owner = this;
            if (Properties.Settings.Default.ShowTip)
            {
                WindowError.Instance.Owner = this;
            }
            EquipmentData.NavigateCurrentUi();
            ThemeItem itemTemp = SkinViewModel.Instance.Themes.FirstOrDefault(item => item.Name == Properties.Settings.Default.FacadeName);
            if (itemTemp != null)
            {
                itemTemp.Load();
            }

            DispatcherTimer timerTemp = new DispatcherTimer();
            timerTemp.Interval = new System.TimeSpan(0, 0, 2);
            timerTemp.Tick += (obj, eventArg) =>
            {
                if (itemTemp != null)
                {
                    itemTemp.Load();
                }
                timerTemp.Stop();
            };
            timerTemp.Start();
        }

        /// 新添窗体时
        /// <summary>
        /// 新添窗体时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WindowsAll_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList list = e.NewItems;
            if (list == null)
                return;
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                DockWindowDisposable dockWindow = list[i] as DockWindowDisposable;
                if (dockWindow != null && dockWindow.CurrentControl != null)
                {
                    AddDockWindow(dockWindow);
                }
            }
        }

        private void AddDockWindow(DockWindowDisposable dockWindow)
        {
            eDockSide dockSide = dockWindow.CurrentControl.DockStyle.Position;
            if (dockSide == eDockSide.Tab)
            {
                AppDock.DockWindow(dockWindow, dockSide);
            }
            else
            {
                SplitPanel splitPanel = GetSplitPanel(dockSide);
                if (splitPanel == null)
                {
                    if (dockSide == eDockSide.Left || dockSide == eDockSide.Right)
                    {
                        AppDock.DockWindow(dockWindow, dockSide, true, eEventActionSource.Code, false);
                    }
                    else
                    {
                        AppDock.DockWindow(dockWindow, dockSide);
                    }
                    #region 添加新的面板并设置面板大小
                    splitPanel = GetSplitPanel(dockSide);
                    if (splitPanel != null)
                    {
                        switch (dockSide)
                        {
                            case eDockSide.Left:
                            case eDockSide.Right:
                                DockSite.SetDockSize(splitPanel, 285);
                                break;
                            case eDockSide.Bottom:
                                DockSite.SetDockSize(splitPanel, 175);
                                break;
                            case eDockSide.Top:
                                DockSite.SetDockSize(splitPanel, 46);
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 将窗体添加到现有的面板
                    if (splitPanel.Children.Count > 0)
                    {
                        DockWindowGroup windowGroup = splitPanel.Children[0] as DockWindowGroup;
                        if (windowGroup != null)
                        {
                            windowGroup.Items.Add(dockWindow);
                            if (dockWindow.Name == "标准表信息")
                            {
                                AppDock.DockWindow(dockWindow, windowGroup, eDockSide.Left);
                                windowGroup.SetValue(SplitPanel.RelativeSizeProperty, new Size(346, 100));
                            }
                            else if (dockWindow.Name == "串口服务器")
                            {
                                if (splitPanel.Children.Count > 1)
                                {
                                    windowGroup = splitPanel.Children[splitPanel.Children.Count-1] as DockWindowGroup;
                                }
                                AppDock.DockWindow(dockWindow, windowGroup, eDockSide.Right);
                                windowGroup.SetValue(SplitPanel.RelativeSizeProperty, new Size(160, 100));
                            }
                            splitPanel.Visibility = Visibility.Visible;
                            windowGroup.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        DockWindowGroup windowGroup = new DockWindowGroup();
                        windowGroup.Items.Add(dockWindow);
                        splitPanel.Children.Add(windowGroup);
                    }
                    #endregion
                }
            }
            if (dockWindow.CurrentControl.DockStyle.IsFloating)
            {
                AppDock.FloatWindow(dockWindow, true);
                DependencyObject objTemp = dockWindow.Parent;
                while (objTemp != null)
                {
                    if (objTemp is FloatingWindow)
                    {
                        FloatingWindow floatingWindow = (FloatingWindow)objTemp;
                        floatingWindow.ResizeMode = dockWindow.CurrentControl.DockStyle.ResizeMode;
                        break;
                    }
                    else
                    {
                        objTemp = VisualTreeHelper.GetParent(objTemp);
                    }
                }
            }
            else
            {
                dockWindow.IsSelected = true;
            }
        }

        private SplitPanel GetSplitPanel(eDockSide dockSide)
        {
            SplitPanel splitPanel = null;
            string dockString = dockSide.ToString();
            for (int i = 0; i < AppDock.SplitPanels.Count; i++)
            {
                object obj = AppDock.SplitPanels[i].GetValue(DockSite.DockProperty);
                if (dockString == obj.ToString())
                {
                    splitPanel = AppDock.SplitPanels[i];
                    break;
                }
            }
            return splitPanel;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            MainViewModel.Instance.WindowsAll.CollectionChanged -= WindowsAll_CollectionChanged;
            base.OnClosed(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            if (EquipmentData.Controller.IsChecking)
            {
                if (MessageBox.Show("确认要退出检定吗?", "退出程序", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (MessageBox.Show("程序正在检定,确认要退出吗?", "退出程序", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (MessageBox.Show("确认要退出检定吗?", "退出程序", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            EquipmentData.DeviceManager.UnLink();
            VerifyClientController.CloseVerifyClient();
            verifyWindow.Close();
            base.OnClosing(e);
        }
        #region 状态栏数据
        private bool initialFlag = false;
        protected override void OnActivated(System.EventArgs e)
        {
            if (!initialFlag)
            {
                BindingStatusBar();
                initialFlag = true;
            }
            base.OnActivated(e);
        }
        private void BindingStatusBar()
        {
            textBlockSchemaName.DataContext = EquipmentData.SchemaModels;
            textBlockCheckName.DataContext = EquipmentData.Controller;
            textBlockCheckIndex.DataContext = EquipmentData.Controller;
            textBlockTestMode.DataContext = EquipmentData.Controller;
            lightCheckStatus.DataContext = EquipmentData.Controller;
            textBlockCheckStatus.DataContext = EquipmentData.Controller;
            textEquipmentNo.DataContext = EquipmentData.Equipment;
            textEquipmentType.DataContext = EquipmentData.Equipment;
            stackPanelChecker.DataContext = EquipmentData.LastCheckInfo;

            imageEqupmentStatus.DataContext = EquipmentData.DeviceManager;

            stackPanelTime.DataContext = TimeMonitor.Instance;
        }
        #endregion
    }
}