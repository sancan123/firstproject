using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Log;
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

namespace Mesurement.UiLayer.WPF
{
    /// <summary>
    /// Interaction logic for WindowError.xaml
    /// </summary>
    public partial class WindowError : Window
    {
        private static WindowError instance = null;

        public static WindowError Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WindowError();
                }
                return instance;
            }
        }

        public WindowError()
        {
            InitializeComponent();
            Visibility = Visibility.Collapsed;
            LogViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            Topmost = true;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TipMessage")
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    try
                    {
                        Visibility = Visibility.Visible;
                        textBlock.Text = LogViewModel.Instance.TipMessage;
                        Show();
                    }
                    catch
                    { }
                }));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UiInterface.ChangeUi("日志记录", "ViewLogBox");
            Visibility = Visibility.Collapsed;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
