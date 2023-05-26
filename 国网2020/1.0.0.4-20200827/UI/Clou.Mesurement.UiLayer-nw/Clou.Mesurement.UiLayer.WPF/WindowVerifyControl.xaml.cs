using Mesurement.UiLayer.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Mesurement.UiLayer.WPF
{
    /// <summary>
    /// WindowVerifyControl.xaml 的交互逻辑
    /// </summary>
    public partial class WindowVerifyControl : Window
    {
        public WindowVerifyControl()
        {
            InitializeComponent();
            DataContext = EquipmentData.Controller;
            ResizeMode = ResizeMode.NoResize;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Width - Width - 80;
            Top = 33;
            Topmost = true;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
