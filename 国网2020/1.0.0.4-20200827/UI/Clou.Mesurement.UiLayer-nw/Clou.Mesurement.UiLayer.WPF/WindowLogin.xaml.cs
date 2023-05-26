using Mesurement.UiLayer.DAL.Config;
using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.ViewModel.Time;
using Mesurement.UiLayer.ViewModel.User;
using Mesurement.UiLayer.ViewModel.WcfService;
using Mesurement.UiLayer.WPF.Language;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Mesurement.UiLayer.WPF.Model;

namespace Mesurement.UiLayer.WPF
{
    /// <summary>
    /// WindowLogin.xaml 的交互逻辑
    /// </summary>
    public partial class WindowLogin : Window
    {
        public WindowLogin()
        {
            InitializeComponent();
            LoadUsers();
            DataContext = EquipmentData.LastCheckInfo;
            comboBoxAuditor.Text = EquipmentData.LastCheckInfo.AuditPerson;
            comboBoxChecker.Text = EquipmentData.LastCheckInfo.TestPerson;

            timeBar.DataContext = TimeMonitor.Instance.LoginTime;
            timeBar.SetBinding(ProgressBar.ValueProperty, new Binding("PastTime") { Mode = BindingMode.OneWay });
            timeBar.SetBinding(ProgressBar.MaximumProperty, new Binding("TotalTime"));

            textBlockLogin.DataContext = EquipmentData.Equipment;
            textBlockLogin.SetBinding(TextBlock.TextProperty, new Binding("TextLogin"));

            
        }

        SoftReg softReg = new SoftReg();

        protected override void OnActivated(EventArgs e)
        {
            textVersion.Text = "V" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            comboBoxLanguage.SelectedValue = ConfigHelper.Instance.Language;
            base.OnActivated(e);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                switch (button.Name)
                {
                    case "buttonLogin":
                        if (this.CheckRegister())
                        {
                            if (CheckLogin())
                            {
                                if (!BackClienController.IsRunningBackUp)
                                {
                                    //BackClienController.CloseBackUpClient();
                                  //  BackClienController.RunBackUpClient();
                                }
                                gridContent.Visibility = Visibility.Collapsed;
                                gridLogIn.Visibility = Visibility.Visible;
                                TimeMonitor.Instance.LogIn();
                                InitializeCheckInfo();

                            }
                        }
                        else
                        {
                            View.ViewRegister register = new View.ViewRegister();
                            register.Topmost = true;
                            register.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                            register.ShowDialog();
                        }
                        break;
                    case "buttonQuit":
                        Close();
                        break;
                }
            }
        }



        private bool CheckLogin()
        {
            bool flag = true;
            if (comboBoxCurrent.SelectedItem == null)
            {
                comboBoxCurrent.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                comboBoxCurrent.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            if (comboBoxVoltage.SelectedItem == null)
            {
                comboBoxVoltage.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                comboBoxVoltage.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            string checker = comboBoxChecker.Text;
            string password1 = passwordBoxChecker.Password;
            string auditor = comboBoxAuditor.Text;
            string password2 = passwordBoxAuditor.Password;
            if (!UserViewModel.Instance.Login(auditor, password2))
            {
                passwordBoxAuditor.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                passwordBoxAuditor.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            if (!UserViewModel.Instance.Login(checker, password1))
            {
                passwordBoxChecker.BorderBrush = new SolidColorBrush(Colors.Red);
                flag = false;
            }
            else
            {
                passwordBoxChecker.BorderBrush = SystemColors.ControlDarkDarkBrush;
            }
            if (flag)
            {
                EquipmentData.LastCheckInfo.AuditPerson = auditor;
                EquipmentData.LastCheckInfo.ProtectedCurrent = comboBoxCurrent.SelectedItem as string;
                EquipmentData.LastCheckInfo.ProtectedVoltage = comboBoxVoltage.SelectedItem as string;
            }
            return flag;
        }
        private void InitializeCheckInfo()
        {
            new Thread(() =>
            {
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:配置信息加载完毕,开始加载方案信息...";
                FullTree.Instance.Initialize();
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:正在加载结论视图...";
                ResultViewHelper.Initialize();
                EquipmentData.Equipment.TextLogin = "软件登录中,请等待:结论视图加载完毕,开始初始化检定信息...";
                EquipmentData.Initialize();

                #region 初始化Wcf服务
                //开启界面端的wcf服务,做好接收检定模块消息的准备
                LogManager.AddMessage("开始下发参数并建立设备连接,请等待....", EnumLogSource.设备操作日志);
                //创建wcf客户端,连接到检定模块的wcf服务
                TaskManager.AddWcfAction(() =>
                {
                    //开启消息服务
                    WcfHelper.Instance.InitialMessageService(); 
                    //开机检定结论服务
                    WcfHelper.Instance.InitialLocalMisDataService();
                    //客户端启动时会自动执行初始化动作
                    if (ConfigHelper.Instance.ServiceAddressVerify.Contains("127.0.0.1") && !VerifyClientController.IsRunning)
                    {
                        //启动检定客户端
                        VerifyClientController.RunVerifyClient();
                        //等待3秒
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        WcfHelper.Instance.InitialControlClient();
                    }
                });
                #endregion

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    TimeMonitor.Instance.LoginTime.FinishItem();
                    Close();
                }));
            }).Start();
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxLanguage.SelectedItem != null)
            {
                ConfigHelper.Instance.Language = comboBoxLanguage.SelectedItem.ToString();
                LanguageManager.LoadLanguage(comboBoxLanguage.SelectedItem.ToString());
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            buttonLogin.Click -= Button_Click;
            buttonQuit.Click -= Button_Click;
            comboBoxAuditor.KeyUp -= comboBoxChecker_KeyUp;
            comboBoxChecker.KeyUp -= comboBoxChecker_KeyUp;
            comboBoxLanguage.SelectionChanged -= Language_SelectionChanged;

            base.OnClosed(e);
        }

        private void comboBoxChecker_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
            {
                return;
            }
            string inputText = comboBox.Text;
            comboBox.ItemsSource = UserViewModel.Instance.GetList(inputText);
        }
        /// <summary>
        /// 加载用户
        /// </summary>
        private void LoadUsers()
        {
            List<string> userNames = UserViewModel.Instance.GetList("");
            comboBoxChecker.ItemsSource = userNames;
            comboBoxChecker.SelectedItem = EquipmentData.LastCheckInfo.TestPerson;
            comboBoxAuditor.ItemsSource = userNames;
            comboBoxAuditor.SelectedItem = EquipmentData.LastCheckInfo.AuditPerson;


        }

        public bool CheckRegister()
        {
            
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("wxf").CreateSubKey("wxf.INI3");
            foreach (string strRNum in retkey.GetSubKeyNames())
            {
                if (strRNum == softReg.GetRNumBy3())
                {
                    return true;
                }
            }
            return false;
        }
    }
}