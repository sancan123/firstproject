using Mesurement.UiLayer.ViewModel.User;
using System;
using System.ComponentModel;
using System.Windows;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// WindowUser.xaml 的交互逻辑
    /// </summary>
    public partial class ViewUser : IDisposable
    {
        public ViewUser()
        {
            InitializeComponent();
            Name = "用户管理";
            DataContext = UserViewModel.Instance;
            frameUser.Navigating += frameUser_Navigating;
            UserViewModel.Instance.PropertyChanged += Instance_PropertyChanged;
            if (UserViewModel.Instance.Step > 1)
            {
                buttonBack.Visibility = Visibility.Visible;
            }
            else
            {
                buttonBack.Visibility = Visibility.Collapsed;
            }
            UserViewModel.Instance.Step = 0;
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = new Size(700, 600);
        }

        void frameUser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            IDisposable page = frameUser.Content as IDisposable;
            if (page != null)
            {
                page.Dispose();
            }
        }

        private void Instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Step")
            {
                NavigateToCurrrentPage();
                if (UserViewModel.Instance.Step > 1)
                {
                    buttonBack.Visibility = Visibility.Visible;
                }
                else
                {
                    buttonBack.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void NavigateToCurrrentPage()
        {
            switch (UserViewModel.Instance.Step)
            {
                case 0:
                    frameUser.Source = new Uri("/Mesurement.UiLayer.WPF;component/UserManagement/PageUserLogin.xaml", UriKind.Relative);
                    break;
                case 1:
                    frameUser.Source = new Uri("/Mesurement.UiLayer.WPF;component/UserManagement/PageUserHome.xaml", UriKind.Relative);
                    break;
                case 2:
                    frameUser.Source = new Uri("/Mesurement.UiLayer.WPF;component/UserManagement/PageChangePassword.xaml", UriKind.Relative);
                    break;
                case 3:
                    frameUser.Source = new Uri("/Mesurement.UiLayer.WPF;component/UserManagement/PageDeleteUser.xaml", UriKind.Relative);
                    break;
                case 4:
                    frameUser.Source = new Uri("/Mesurement.UiLayer.WPF;component/UserManagement/PageAddUser.xaml", UriKind.Relative);
                    break;
            }
        }

        public override void Dispose()
        {
            frameUser.Navigating -= frameUser_Navigating;
            UserViewModel.Instance.PropertyChanged -= Instance_PropertyChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (UserViewModel.Instance.Step > 1)
            {
                UserViewModel.Instance.Step = 1;
            }
        }
    }
}
