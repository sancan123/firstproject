using DevComponents.WpfDock;
using Mesurement.UiLayer.ViewModel;
using System.Windows;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewStd
    {
        public ViewStd()
        {
            InitializeComponent();
            Name = "标准表信息";
            DockStyle.Position = eDockSide.Bottom;
            DataContext = EquipmentData.StdInfo;
            if (EquipmentData.Equipment.EquipmentType == "三相台")
            {
                DockStyle.FloatingSize = new Size(500, 400);
            }
            else
            {
                DockStyle.FloatingSize = new Size(500, 250);
            }
        }
    }
}
