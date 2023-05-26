using Mesurement.UiLayer.WPF.Skin;
using DevComponents.WpfDock;
using System;
using System.Windows;

namespace Mesurement.UiLayer.WPF.Model
{
    public class DockWindowDisposable : DockWindow
    {
        /// 在创建窗体时对此控件赋值
        /// <summary>
        /// 在创建窗体时对此控件赋值
        /// </summary>
        public DockControlDisposable CurrentControl
        {
            get { return Content as DockControlDisposable; }
        }
        /// 关闭窗体时调用控件注销方法
        /// <summary>
        /// 关闭窗体时调用控件注销方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(System.Windows.RoutedEventArgs e)
        {
            if (CurrentControl != null)
            {
                CurrentControl.Dispose();
            }
            Content = null;
            MainViewModel.Instance.WindowsAll.Remove(this);
            DependencyObject obj = LogicalTreeHelper.GetParent(this);
            if (obj is DockWindowGroup)
            {
                ((DockWindowGroup)obj).Items.Remove(this);
                if (((DockWindowGroup)obj).Items.Count > 0)
                {
                    ((DockWindow)((DockWindowGroup)obj).Items[0]).IsSelected = true;
                }
            }
            base.OnClosed(e);
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        protected override void OnDockParentChanged(EventArgs e)
        {
            IsSelected = true;
            base.OnDockParentChanged(e);
            SkinManager.ChangeWindowSkin(CurrentControl);
        }
        protected override void OnTabVisibilityChanged(RoutedEventArgs e)
        {
            base.OnTabVisibilityChanged(e);
            SkinManager.ChangeWindowSkin(CurrentControl);
        }
    }
}
