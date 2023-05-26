using Mesurement.UiLayer.ViewModel;
using System.Windows.Data;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewErrorBoard.xaml 的交互逻辑
    /// </summary>
    public partial class ViewErrorBoard
    {
        public ViewErrorBoard()
        {
            InitializeComponent();
            Name = "误差板数据";
            meterContainer.DataContext = EquipmentData.DeviceManager;
            DockStyle.Position = DevComponents.WpfDock.eDockSide.Top;
            DockStyle.FloatingSize = new System.Windows.Size(1200, 100);
        }

        public override void Dispose()
        {
            //清除绑定
            BindingOperations.ClearAllBindings(this);
            meterContainer.DataContext = null;
            base.Dispose();
        }
    }
}
