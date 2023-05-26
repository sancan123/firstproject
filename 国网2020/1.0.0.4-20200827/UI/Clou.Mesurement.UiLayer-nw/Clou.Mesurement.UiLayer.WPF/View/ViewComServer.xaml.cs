using DevComponents.WpfDock;
using Mesurement.UiLayer.ViewModel;
using System.Windows;
using Mesurement.UiLayer.ViewModel.Device;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewComServer
    {
        /// 串口服务器
        /// <summary>
        /// 串口服务器
        /// </summary>
        public ViewComServer()
        {
            InitializeComponent();
            Name = "串口服务器";
            DockStyle.Position = eDockSide.Bottom;
            DataContext = EquipmentData.StdInfo;
            DockStyle.FloatingSize = new Size(500,250);
            DockStyle.ResizeMode = ResizeMode.NoResize;
            controlServers.DataContext = ServersViewModel.Instance;
        }
    }
}
