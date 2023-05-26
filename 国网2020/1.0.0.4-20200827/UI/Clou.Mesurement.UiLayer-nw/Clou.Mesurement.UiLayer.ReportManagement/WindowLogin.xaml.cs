using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.DataManager.ViewModel;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.CodeTree;
using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.ViewModel.User;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Mesurement.UiLayer.DataManager
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin
    {
        public WindowLogin()
        {
            InitializeComponent();
            UiInterface.UiDispatcher = SynchronizationContext.Current;
            textBlockSoftwareName.Text = AssemblyProduct;
            textBlockVersion.Text = "V " + AssemblyVersion;
            textblockCopyright.Text = AssemblyCopyright;
            textboxUserName.Text = Properties.Settings.Default.LastUserName;
            //textboxPassword.Password = Properties.Settings.Default.DefaultPassword;
        }
        #region 软件基本信息
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        #endregion

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Click_Login(object sender, RoutedEventArgs e)
        {
            if (UserViewModel.Instance.Login(textboxUserName.Text, textboxPassword.Password))
            {
                #region 运行主程序
                Properties.Settings.Default.LastUserName = textboxUserName.Text;
                Properties.Settings.Default.Save();
                buttonLogin.IsEnabled = false;
                MessageDisplay.Instance.Initialize();
                new Thread(() =>
                {
                    CodeTreeViewModel.Instance.InitializeTree();
                    FullTree.Instance.Initialize();
                    ResultViewHelper.Initialize();
                    Equipments.Instance.Initialize();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MainWindow mainWindow = new MainWindow();
                        Application.Current.MainWindow = mainWindow;
                        mainWindow.Show();
                        Close();
                    }));
                }).Start();
                #endregion
            }
            else
            {
                textBlockError.Visibility = Visibility.Visible;
                new Thread(() =>
                {
                    Thread.Sleep(3000);
                    Dispatcher.Invoke(new Action(() =>
                        {
                            textBlockError.Visibility = Visibility.Collapsed;
                        }));
                }).Start();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Click_About(object sender, RoutedEventArgs e)
        {
            WindowAbout windowAbout = new WindowAbout();
            windowAbout.ShowDialog();
        }
        ///下面这个方法是dso打开文件时需要的一个参数，代表office文件类型
        /// <summary>
        /// 根据后缀名得到打开方式
        /// </summary>
        /// <param name="_sExten"></param>
        /// <returns></returns>
        private string LoadOpenFileType(string _sExten)
        {
            try
            {
                string sOpenType = "";
                switch (_sExten.ToLower())
                {
                    case "xls":
                        sOpenType = "Excel.Sheet";
                        break;
                    case "doc":
                    case "docx":
                        sOpenType = "Word.Document";
                        break;
                    case "ppt":
                        sOpenType = "PowerPoint.Show";
                        break;
                    case "vsd":
                        sOpenType = "Visio.Drawing";
                        break;
                    default:
                        sOpenType = "Word.Document";
                        break;
                }
                return sOpenType;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
