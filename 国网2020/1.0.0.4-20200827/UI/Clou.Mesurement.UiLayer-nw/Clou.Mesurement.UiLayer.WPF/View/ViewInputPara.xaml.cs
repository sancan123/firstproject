using System;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using Mesurement.UiLayer.ViewModel.InputPara;
using System.Windows.Controls;
using Mesurement.UiLayer.WPF.Controls;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.WPF.Converter;
using Mesurement.UiLayer.WPF.UiGeneral;
using System.Windows.Input;
using Mesurement.UiLayer.WPF.Model;
using Mesurement.UiLayer.ViewModel.CodeTree;
using Mesurement.UiLayer.DAL;
using System.Collections.Generic;
using Mesurement.UiLayer.DAL.Config;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewInputPara
    {
        public ViewInputPara()
        {
            InitializeComponent();
            Name = "参数录入";
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            for (int i = 0; i < viewModel.ParasModel.AllUnits.Count; i++)
            {
                if (viewModel.ParasModel.AllUnits[i].IsSame && viewModel.ParasModel.AllUnits[i].IsNecessary)
                {
                    AddBasicPara(viewModel.ParasModel.AllUnits[i]);
                }
            }
            comboBoxSchema.DataContext = EquipmentData.SchemaModels;
            GenerateColumns();
        }
        private MeterInputParaViewModel viewModel
        {
            get { return Resources["MeterInputParaViewModel"] as MeterInputParaViewModel; }
        }
        private void AddBasicPara(InputParaUnit paraUnit)
        {
            StackPanel stackPanel = new StackPanel()
            {
                Margin = new Thickness(5, 3, 5, 3),
                Orientation = Orientation.Horizontal,
            };
            TextBlock textBlock = new TextBlock
            {
                Text = paraUnit.DisplayName,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 50
            };
            ControlEnumComboBox comboBox = new ControlEnumComboBox()
            {
                EnumName = paraUnit.CodeType,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Style = Application.Current.Resources["StyleComboBox"] as Style,
                Width = 80,
                Tag = paraUnit.FieldName,
            };
            comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding(string.Format("FirstMeter.{0}", paraUnit.FieldName)) { Mode = BindingMode.TwoWay });
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(comboBox);
            wrapPanelParas.Children.Add(stackPanel);
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string fieldName = comboBox.Tag as string;
                if (!string.IsNullOrEmpty(fieldName))
                {
                    for (int i = 0; i < viewModel.Meters.Count; i++)
                    {
                        viewModel.Meters[i].SetProperty(fieldName, comboBox.SelectedItem);
                    }
                }
            }
        }



        public bool IsAllSelected
        {
            get { return (bool)GetValue(IsAllSelectedProperty); }
            set { SetValue(IsAllSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAllSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAllSelectedProperty =
            DependencyProperty.Register("IsAllSelected", typeof(bool), typeof(ViewInputPara), new PropertyMetadata(false));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "IsAllSelected")
            {
                for (int i = 0; i < viewModel.Meters.Count; i++)
                {
                    viewModel.Meters[i].SetProperty("CHR_CHECKED", IsAllSelected ? "1" : "0");
                }
            }
            base.OnPropertyChanged(e);
        }


        private void GenerateColumns()
        {
            #region 要检
            CheckBox checkbox = new CheckBox
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = "表要检标记,全选"
            };
            Binding binding = new Binding("IsAllSelected")
            {
                Source = this
            };
            checkbox.SetBinding(CheckBox.IsCheckedProperty, binding);
            Binding cellBinding = new Binding("CHR_CHECKED");
            cellBinding.Mode = BindingMode.TwoWay;
            cellBinding.Converter = new BoolBitConverter();
            DataGridColumn columnYaojian = Resources["KeyYaojianColumn"] as DataGridColumn;
            columnYaojian.Header = checkbox;
            dataGridMeters.Columns.Add(columnYaojian);
            #endregion
            for (int i = 0; i < viewModel.ParasModel.AllUnits.Count; i++)
            {
                InputParaUnit paraUnit = viewModel.ParasModel.AllUnits[i];
                if (paraUnit.IsDisplayMember && (!paraUnit.IsSame) && (paraUnit.FieldName != "CHR_CHECKED"))
                {
                    DataGridColumn column;
                    column = ControlFactory.CreateColumn(paraUnit.DisplayName, paraUnit.CodeType, paraUnit.FieldName, true);

                    if (paraUnit.FieldName == "AVR_BAR_CODE")
                    {
                        column.Width = 170;
                    }
                    else if (paraUnit.FieldName == "AVR_ADDRESS")
                    {
                        //column.IsReadOnly = true;
                        column.Width = 100;
                    }
                    else if (paraUnit.FieldName == "LNG_BENCH_POINT_NO")
                    {
                        column.IsReadOnly = true;
                        column.Width = 40;
                    }
                    else
                    {
                        column.MinWidth = 40;
                        column.Width = new DataGridLength(1, DataGridLengthUnitType.Auto);
                    }

                    dataGridMeters.Columns.Add(column);
                }
            }
        }

        private void dataGridMeters_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {


            if (!(checkBoxQuickInput.IsChecked.HasValue ))
            {
                return;
            }

            #region 校验单元格
            string columnHeader = e.Column.Header as string;
            InputParaUnit paraUnit = viewModel.ParasModel.AllUnits.FirstOrDefault(item => item.DisplayName == columnHeader);
            if (paraUnit == null)
            {
                return;
            }
            string fieldTemp = paraUnit.FieldName;

            DynamicViewModel meterCurrent = e.Row.DataContext as DynamicViewModel;
            if (meterCurrent == null)
            {
                return;
            }
            int indexTemp = viewModel.Meters.IndexOf(meterCurrent);
            #endregion

            object cellValue = "";
            ComboBox currentElement1 = e.EditingElement as ComboBox;
            if (currentElement1 != null)
            {
                cellValue = currentElement1.SelectedItem;
            }
            TextBox currentElement2 = e.EditingElement as TextBox;
            if (currentElement2 != null)
            {
                cellValue = currentElement2.Text;
            }

            if (cellValue == null)
            {
                return;
            }



            if (fieldTemp == "AVR_ADDRESS")
            {
                if (!string.IsNullOrEmpty(cellValue as string))
                {
                    string tempaddress = cellValue.ToString();
                    string tempbluetooth = "C0" + Convert.ToInt64(tempaddress).ToString("X2").PadLeft(10, '0');
                    viewModel.Meters[indexTemp].SetProperty("AVR_OTHER_2", tempbluetooth);
                }
                return;
            }



            if (!(checkBoxQuickInput.IsChecked.HasValue && checkBoxQuickInput.IsChecked.Value))
            {
                return;
            }
          

            //if (cellValue.Equals(meterCurrent.GetProperty(fieldTemp)))
            //{
            //    return;
            //}

            if (fieldTemp == "AVR_BAR_CODE")
            {
                if (!string.IsNullOrEmpty(cellValue as string))
                {
                    string tempBarCode = cellValue.ToString();
                    int assertNoStartIndex=ConfigHelper.Instance.AssertNoStartIndex-1;
                    int assertNoLength=ConfigHelper.Instance.AssertNoLength;
                    if (ConfigHelper.Instance.IsAssertNoFromBarCode&& assertNoStartIndex>=0)
                    {
                        if (tempBarCode.Length >= assertNoStartIndex + assertNoLength)
                        { 
                            string assertNo=string.Format("{0}{1}{2}",ConfigHelper.Instance.AssertNoStartStr,tempBarCode.Substring(assertNoStartIndex,assertNoLength),ConfigHelper.Instance.AssertNoEndStr);
                            viewModel.Meters[indexTemp].SetProperty("AVR_MADE_NO", assertNo);
                        }
                    }
                }
                return;
            }

            string CertificateIndex = "";
            for (int i = indexTemp; i < viewModel.Meters.Count; i++)
            {
                if (fieldTemp == "AVR_CERTIFICATE_NO")
                {
                    CertificateIndex = (i + 1).ToString().PadLeft(3, '0');
                    viewModel.Meters[i].SetProperty(fieldTemp, cellValue + CertificateIndex);
                }
                else
                {
                    viewModel.Meters[i].SetProperty(fieldTemp, cellValue);
                }
            }
          
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox boxYaojian = sender as CheckBox;
            if (boxYaojian != null)
            {
                DynamicViewModel modelTemp = boxYaojian.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    string yaojianTemp = modelTemp.GetProperty("CHR_CHECKED") as string;
                    if (yaojianTemp == "1")
                    {
                        modelTemp.SetProperty("CHR_CHECKED", "0");
                    }
                    else
                    {
                        modelTemp.SetProperty("CHR_CHECKED", "1");
                    }
                }
            }
        }

        private void Event_AddNew(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = e.OriginalSource as ComboBox;
            if (comboBox == null || comboBox.SelectedItem as string != "添加新项")
            {
                return;
            }
            #region 添加新的编码
            BindingExpression expression = comboBox.GetBindingExpression(ComboBox.SelectedItemProperty);
            if (expression == null)
            {
                return;
            }
            //解析编码路径
            string strPath = expression.ParentBinding.Path.Path;
            //如果没有创建新的值,就恢复原来的值
            string oldValue = (expression.DataItem as DynamicViewModel).GetProperty(strPath) as string;
            InputParaUnit unitTemp = viewModel.ParasModel.AllUnits.FirstOrDefault(item => item.FieldName == strPath);
            if (unitTemp != null)
            {
                //获取节点
                CodeTreeNode nodeTemp = CodeTreeViewModel.Instance.GetCodeByEnName(unitTemp.CodeType, 2);
                if (nodeTemp != null)
                {
                    WindowAddNewCode windowTemp = new WindowAddNewCode(nodeTemp);
                    bool? boolTemp = windowTemp.ShowDialog();
                    if (boolTemp.HasValue && boolTemp.Value)
                    {
                        DataGridCell cellTemp = Utils.FindVisualParent<DataGridCell>(comboBox);
                        if (cellTemp != null)
                        {
                            DataGridComboBoxColumn columnTemp = cellTemp.Column as DataGridComboBoxColumn;
                            if (columnTemp !=null && nodeTemp.Children.Count > 0)
                            {
                                Dictionary<string, string> dictionary = CodeDictionary.GetLayer2(nodeTemp.CODE_TYPE);
                                List<string> dataSource=dictionary.Keys.ToList();
                                dataSource.Add("添加新项");
                                columnTemp.ItemsSource = dataSource;
                                comboBox.SelectedItem = nodeTemp.Children[nodeTemp.Children.Count - 1].CODE_NAME;
                                return;
                            }
                        }
                    }
                }
            }
            #endregion
            comboBox.SelectedItem = oldValue;
        }
    }
}
