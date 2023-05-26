using Mesurement.UiLayer.DataManager.ViewModel.Mark;
using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// Interaction logic for ControlEquipmentInfoItem.xaml
    /// </summary>
    public partial class ControlDeviceInfoItem : UserControl
    {
        public ControlDeviceInfoItem()
        {
            InitializeComponent();
            Array arrayTemp = Enum.GetValues(typeof(EnumFormat));
            comboBoxFormat.ItemsSource = arrayTemp;
            comboBoxFormat.SelectedItem = EnumFormat.无;
            comboBoxFormat1.ItemsSource = arrayTemp;
            comboBoxFormat1.SelectedItem = EnumFormat.无;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
    }
}
