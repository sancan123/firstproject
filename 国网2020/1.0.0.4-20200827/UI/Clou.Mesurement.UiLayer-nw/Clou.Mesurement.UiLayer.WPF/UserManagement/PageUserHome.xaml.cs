using Mesurement.UiLayer.ViewModel.User;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.UserManagement
{
    /// <summary>
    /// PageUserHome.xaml 的交互逻辑
    /// </summary>
    public partial class PageUserHome : IDisposable
    {
        public PageUserHome()
        {
            InitializeComponent();
            DataContext = UserViewModel.Instance;
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if(button==null)
            {
                return ;
            }
            switch (button.Name)
            {
                case "buttonChangeUser":
                    UserViewModel.Instance.CurrentUser = null;
                    UserViewModel.Instance.Step = 0;
                    break;
                case "buttonChangePassword":
                    UserViewModel.Instance.Step = 2;
                    break;
                case "buttonAddUser":
                    UserViewModel.Instance.Step = 4;
                    break;
                case "buttonDeleteUser":
                    UserViewModel.Instance.Step = 3;
                    break;
            }
        }

        public void Dispose()
        {
            buttonChangeUser.Click -= StackPanel_Click;
            buttonChangePassword.Click -= StackPanel_Click;
            buttonAddUser.Click -= StackPanel_Click;
            buttonDeleteUser.Click -= StackPanel_Click;
        }
    }
}
