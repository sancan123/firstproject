using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewRegister.xaml 的交互逻辑
    /// </summary>
    public partial class ViewRegister : Window
    {
        private static ViewRegister instance = null;
        public static ViewRegister Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ViewRegister();
                }
                return instance;
            }
        }

        public ViewRegister()
        {
            InitializeComponent();
            this.txt_MNum.Text = softReg.GetMNum();
        }
        SoftReg softReg = new SoftReg();
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strReg = softReg.GetRNumBy3(this.txt_MNum.Text);
                if (txt_RNum.Text == strReg)
                {
                    MessageBox.Show("注册成功！", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("wxf").CreateSubKey("wxf.INI3").CreateSubKey(txt_RNum.Text);
                    retkey.SetValue("UserName", "Rsoft");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("注册码错误！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txt_RNum.SelectAll();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       
    }
}
