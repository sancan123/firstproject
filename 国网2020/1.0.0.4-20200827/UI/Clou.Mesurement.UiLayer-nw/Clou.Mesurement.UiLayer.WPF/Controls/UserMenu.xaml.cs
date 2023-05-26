using Mesurement.UiLayer.ViewModel.Menu;
using Mesurement.UiLayer.WPF.Skin;
using Mesurement.UiLayer.WPF.UiGeneral;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Shapes = System.Windows.Shapes;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// UserMenu.xaml 的交互逻辑
    /// </summary>
    public partial class UserMenu
    {
        public UserMenu()
        {
            InitializeComponent();
            //textVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            textBlockSoftWareName.Text = Application.Current.Resources["软件名称"].ToString();
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != "")
                {
                    textBlockSoftWareName.Text = titleAttribute.Title;
                }
            }
            LoadMenu();
            MouseLeftButtonDown += UserMenu_MouseLeftButtonDown;
            listBoxSkin.ItemsSource = SkinViewModel.Instance.Themes;
           listBoxSkin.SelectedIndex = 1;//设置白色
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image imageTemp = e.OriginalSource as Image;
            if (imageTemp != null)
            {
                switch (imageTemp.Name)
                {
                    case "imageMin":
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        break;
                    case "imageMax":
                        if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Normal;
                            imageMax.Source = new BitmapImage(new Uri(@"../images/Maximum.ico", UriKind.RelativeOrAbsolute));
                        }
                        else
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Maximized;
                            imageMax.Source = new BitmapImage(new Uri(@"../images/Normal.ico", UriKind.RelativeOrAbsolute));
                        }
                        break;
                    case "imageClose":
                        try
                        {
                            Application.Current.MainWindow.Close();
                        }
                        catch
                        {
                            Application.Current.Shutdown();
                        }
                        break;
                }
            }
            if (e.OriginalSource is TextBlock)
            {
                TextBlock textBlock = e.OriginalSource as TextBlock;
                if (textBlock.Name == "TextDataManager")
                {
                    StartDataManage();
                }
            }
        }

        private void UserMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                    imageMax.Source = new BitmapImage(new Uri(@"../images/Maximum.ico", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                    imageMax.Source = new BitmapImage(new Uri(@"../images/Normal.ico", UriKind.RelativeOrAbsolute));
                }
            }
        }
        private void LoadMenu()
        {
            MenuViewModel menuViewModel = new MenuViewModel();
            for (int i = 0; i < menuViewModel.Menus.Count; i++)
            {
                Button button = ControlFactory.CreateButton(menuViewModel.Menus[i], true);
                if (button != null)
                {
                    stackPannelMenu.Children.Add(button);
                }
            }
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is Shapes.Path)
            {
                ThemeItem itemTemp = ((Shapes.Path)sender).DataContext as ThemeItem;
                if(itemTemp!=null)
                {
                    itemTemp.Load();
                    Properties.Settings.Default.FacadeName = itemTemp.Name;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void StartDataManage()
        {
            Process[] processes = Process.GetProcesses();
            Process processTemp = processes.FirstOrDefault(item => item.ProcessName == "Mesurement.UiLayer.DataManager");
            if (processTemp == null)
            {
                try
                {
                    Process.Start(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Mesurement.UiLayer.DataManager.exe"));
                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("启动数据管理程序失败:{0}",e.Message), EnumLogSource.用户操作日志, EnumLevel.Error);
                }
            }
            else
            {
                return;
            }
        }
    }
}
