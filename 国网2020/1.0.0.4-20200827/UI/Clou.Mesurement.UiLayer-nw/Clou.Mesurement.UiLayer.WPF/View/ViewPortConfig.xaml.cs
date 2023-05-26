using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.PortConfig;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// 端口配置页面
    /// </summary>
    public partial class ViewPortConfig
    {
        public ViewPortConfig()
        {
            InitializeComponent();
            Name = "端口配置";
            //LoadRelayColumns();
            columnQuickPort.ItemsSource = CodeDictionary.GetLayer2("QuickPort").Keys;
        }

        private PortConfigViewModel viewModel
        {
            get
            {
                return Resources["PortConfigViewModel"] as PortConfigViewModel;
            }
        }

        private void Click_Delete(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null && button.Name== "buttonDelete")
            {
                DeviceItem deviceItem = button.DataContext as DeviceItem;
                if (deviceItem != null)
                {
                    viewModel.DeleteItem(deviceItem);
                }
            }
        }

        private void Click_Add(object sender, RoutedEventArgs e)
        {
            Button buttonTemp = sender as Button;
            if (buttonTemp != null)
            {
                DeviceGroup groupTemp = buttonTemp.DataContext as DeviceGroup;
                if (groupTemp != null)
                {
                    groupTemp.DeviceItems.Add(new DeviceItem()
                    {
                        Server = viewModel.Servers[0],
                        FlagChanged = false
                    });
                }
            }
        }

        private void Click_AddRs485(object sender, System.Windows.RoutedEventArgs e)
        {
            RS485Item rs485 = new RS485Item();
            rs485.Server = viewModel.Servers[0];
            rs485.FlagChanged = true;
            viewModel.Rs485Group.Add(rs485);
        }

        private void Click_DeleteRs485(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                RS485Item rs485 = button.DataContext as RS485Item;
                viewModel.DeleteRs485(rs485);
            }
        }
        private void LoadRelayColumns()
        {
            //dataGridRelays.Columns.Clear();
            //for (int i = 0; i < 10; i++)
            //{
            //    string header = "继电器" + (i + 1).ToString();
            //    DataGridCheckBoxColumn column = new DataGridCheckBoxColumn()
            //    {
            //        Header = header,
            //        Binding = new Binding("RelayModel." + header) { Mode = BindingMode.TwoWay },
            //        EditingElementStyle=Resources["styleCheckBoxRelay"] as Style
            //    };
            //    dataGridRelays.Columns.Add(column);
            //}
            //DataGridColumn columnEdit = Resources["columnEdit"] as DataGridColumn;
            //dataGridRelays.Columns.Add(columnEdit);
        }
    }
}
