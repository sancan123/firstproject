using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.ErrorLimit;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewErrorLimit.xaml
    /// </summary>
    public partial class ViewErrorLimit
    {
        public ViewErrorLimit()
        {
            InitializeComponent();
            Name = "误差限配置";
            InitializeDataGrid();
            List<Controls.ControlEnumComboBox> comboBoxes = Model.Utils.FindChildren<Controls.ControlEnumComboBox>(stackPanel);
            for (int i = 0; i < comboBoxes.Count; i++)
            {
                comboBoxes[i].SelectedIndex = 0;
            }
            stackPanel.AddHandler(Selector.SelectionChangedEvent, new SelectionChangedEventHandler(StackPanel_SelectionChanged));
            viewModel.WcLimitNames.PropertyChanged += WcLimitNames_PropertyChanged;
        }

        private void WcLimitNames_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if(viewModel.WcLimitNames.SelectedItem==null)
                {
                    if(viewModel.WcLimitNames.ItemsSource.Count>0)
                    {
                        viewModel.WcLimitNames.SelectedItem = viewModel.WcLimitNames.ItemsSource[0];
                    }
                }
                else
                {
                if (viewModel.WcLimitNames.SelectedItem.ID == "1")
                {
                    viewModel.EnableEdit = false;
                }
                else
                {
                    viewModel.EnableEdit = true;
                } }
                viewModel.LoadErrorLimit();
            }
        }

        private Dictionary<string, string> columnNameDictionary = new Dictionary<string, string>();

        private void InitializeDataGrid()
        {
            Dictionary<string, string> currentDictionary = CodeDictionary.GetLayer2("CurrentTimes");
            foreach (KeyValuePair<string, string> pair in currentDictionary)
            {
                string propertyName = pair.Key.Replace('.', '_').Replace('(', '_').Replace(')', '_');
                columnNameDictionary.Add(propertyName, pair.Key);
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = pair.Key,
                    Binding = new Binding(propertyName + ".LimitValue"),
                    EditingElementStyle = Application.Current.Resources["StyleEditTextBox"] as Style,
                    Width = new DataGridLength(75)
                };
                #region 前景颜色设置
                Style styleTextBlock = new Style(typeof(TextBlock));
                DataTrigger triggerChanged = new DataTrigger()
                {
                    Binding = new Binding(propertyName + ".ChangeFlag"),
                    Value = true,
                };
                Setter setterChanged = new Setter() { Property = TextBlock.ForegroundProperty, Value = new SolidColorBrush(Colors.Red) };
                triggerChanged.Setters.Add(setterChanged);
                styleTextBlock.Triggers.Add(triggerChanged);
                DataTrigger triggerNoValue = new DataTrigger()
                {
                    Binding = new Binding(propertyName + ".FlagNoValue"),
                    Value = true,
                };
                Setter setterNoValue = new Setter() { Property = TextBlock.ForegroundProperty, Value = new SolidColorBrush(Colors.Gray) };
                triggerNoValue.Setters.Add(setterNoValue);
                styleTextBlock.Triggers.Add(triggerNoValue);
                column.ElementStyle = styleTextBlock;
                #endregion
                dataGrid.Columns.Add(column);
            }
        }

        private ErrorLimitViewModel viewModel
        {
            get { return Resources["ErrorLimitViewModel"] as ErrorLimitViewModel; }
        }

        private void StackPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.LoadErrorLimit();
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            #region 解析绑定路径
            TextBox textBoxTemp = e.EditingElement as TextBox;
            if (textBoxTemp == null)
            {
                return;
            }
            DataGridTextColumn column = e.Column as DataGridTextColumn;
            Binding bindingColumn = column.Binding as Binding;
            string pathTemp = bindingColumn.Path.Path;
            string[] arrayTemp = pathTemp.Split('.');
            if (arrayTemp.Length != 2)
            {
                return;
            }
            string currentString = arrayTemp[0];
            DynamicViewModel viewModelTemp = e.Row.DataContext as DynamicViewModel;
            if (viewModelTemp == null)
            {
                return;
            }
            ErrorLimitCell cellTemp = viewModelTemp.GetProperty(currentString) as ErrorLimitCell;
            if (cellTemp == null)
            {
                return;
            }
            cellTemp.LimitValue = textBoxTemp.Text;
            #endregion
            if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
            {
                if (radioButtonCurrent.IsChecked.HasValue && radioButtonCurrent.IsChecked.Value)
                {
                    //修改该电流下的所有误差限
                    for (int i = 0; i < viewModel.ValuesLimit.Count; i++)
                    {
                        ErrorLimitCell cellTemp1 = viewModel.ValuesLimit[i].GetProperty(currentString) as ErrorLimitCell;
                        if (cellTemp1 != null)
                        {
                            cellTemp1.LimitValue = cellTemp.LimitValue;
                        }
                    }
                }
                else
                {
                    //修改相同功率因数下的所有误差限
                    List<string> currentList = viewModelTemp.GetAllProperyName();
                    foreach (string currentTemp in currentList)
                    {
                        ErrorLimitCell cellTemp1 = viewModelTemp.GetProperty(currentTemp) as ErrorLimitCell;
                        if (cellTemp1 != null)
                        {
                            cellTemp1.LimitValue = cellTemp.LimitValue;
                        }
                    }
                }
            }
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            viewModel.SaveLimitValues();
        }

        private void Click_Delete_Limit(object sender, RoutedEventArgs e)
        {
            ContextMenu menuTemp = Resources["menuDeleteLimitName"] as ContextMenu;
            if (menuTemp != null)
            {
                FrameworkElement elementTemp = ContextMenuService.GetPlacementTarget(menuTemp) as FrameworkElement;
                if (elementTemp != null)
                {
                    LimitNameModel modelTemp = elementTemp.DataContext as LimitNameModel;
                    if (modelTemp != null)
                    {
                        viewModel.DeleteLimitName(modelTemp);
                    }
                }
            }
        }

        private void editBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ContextMenu menuTemp = Resources["menuDeleteLimitName"] as ContextMenu;
            FrameworkElement elementTemp = sender as FrameworkElement;
            if (elementTemp != null)
            {
                LimitNameModel modelTemp = elementTemp.DataContext as LimitNameModel;
                if (modelTemp != null && modelTemp.ID == "1")
                {
                    menuTemp.IsEnabled = false;
                }
                else
                {
                    menuTemp.IsEnabled = true;
                }
            }
        }
    }
}
