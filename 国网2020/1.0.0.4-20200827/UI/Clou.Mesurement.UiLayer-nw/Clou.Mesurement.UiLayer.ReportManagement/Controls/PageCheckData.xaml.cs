using Mesurement.UiLayer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using Mesurement.UiLayer.DAL.DataBaseView;
using System.Collections.Generic;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.DataManager.ViewModel;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// 检定数据界面
    /// </summary>
    public partial class PageCheckData
    {
        private AllMeterResult meterResults = new AllMeterResult(null);
        public PageCheckData()
        {
            InitializeComponent();
            Name = "审核存盘";
            datagridMeters.SelectedIndex = 0;
            gridMeterResult.DataContext = meterResults;
            LoadMeterInfo();
            LoadEquipInfo();
        }

        public void LoadCheckData(IEnumerable<DynamicViewModel> meters)
        {
            TaskManager.AddDataBaseAction(() =>
            {
                meterResults.LoadMeters(meters);
            });
        }

        /// <summary>
        /// 所有表的结论
        /// </summary>
        private AllMeterResult viewModel
        {
            get { return Resources["AllMeterResult"] as AllMeterResult; }
        }
        /// <summary>
        /// 加载结论对应的表格
        /// </summary>
        private void LoadMeterDataGrids(OneMeterResult meterResult)
        {
            if (meterResult == null)
            {
                return;
            }
            resultContainer.Children.Clear();
            for (int i = 0; i < meterResult.Categories.Count; i++)
            {
                DataGrid dataGrid = new DataGrid()
                {
                    Margin = new Thickness(3),
                    HeadersVisibility = DataGridHeadersVisibility.All,
                    IsReadOnly = true,
                    Style = Application.Current.Resources["dataGridStyleMeterDetailResult"] as Style,
                };
                dataGrid.ItemsSource = meterResult.Categories[i].ResultUnits;
                for (int j = 0; j < meterResult.Categories[i].Names.Count; j++)
                {
                    string columnName = meterResult.Categories[i].Names[j];
                    if (columnName == "要检" || columnName == "项目名" || columnName == "项目号")
                    {
                        continue;
                    }
                    DataGridTextColumn column = new DataGridTextColumn()
                    {
                        Header = columnName,
                        Binding = new Binding(columnName),
                        Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                    };
                    dataGrid.Columns.Add(column);
                }
                resultContainer.Children.Add(dataGrid);
            }
        }
        /// <summary>
        /// 选中表发生变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OneMeterResult meterResult = datagridMeters.SelectedItem as OneMeterResult;
            if(meterResult==null)
            {
                return;
            }
            LoadMeterDataGrids(meterResult);
           string equipNo = meterResult.MeterInfo.GetProperty("AVR_DEVICE_ID") as string;
            if(!string.IsNullOrEmpty(equipNo))
            {
                stackPanelEquipInfo.DataContext = Equipments.Instance.FindEquipInfo(equipNo);
            }
        }
        /// <summary>
        /// 加载表基本信息
        /// </summary>
        private void LoadMeterInfo()
        {
            //42是参数录入对应的列
            Dictionary<string, string> dictionaryColumn = ResultViewHelper.GetPkDisplayDictionary("42");
            foreach (string fieldName in dictionaryColumn.Keys)
            {
                if (fieldName == "CHR_CHECKED")
                {
                    continue;
                }
                Grid gridTemp = new Grid()
                {
                    Margin = new Thickness(2),
                };
                while (gridTemp.ColumnDefinitions.Count < 2)
                {
                    gridTemp.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        });
                }
                gridTemp.ColumnDefinitions[0].Width = new GridLength(70);
                TextBlock textBlockName = new TextBlock()
                {
                    Text = dictionaryColumn[fieldName],
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0)
                };
                TextBlock textBlockValue = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0)
                };
                textBlockValue.SetBinding(TextBlock.TextProperty, new Binding(string.Format("MeterInfo.{0}", fieldName)));
                Grid.SetColumn(textBlockValue, 1);
                gridTemp.Children.Add(textBlockName);
                gridTemp.Children.Add(textBlockValue);
                stackPanelMeterInfo.Children.Add(gridTemp);
            }
        }
        /// <summary>
        /// 加载台体信息
        /// </summary>
        private void LoadEquipInfo()
        {
            List<string> nameList = Equipments.Instance.GetNames();
            
            for (int i = 0; i < nameList.Count; i++)
            {
                //bool isReadOnly = false;
                string nameTemp = nameList[i];
                if (nameTemp == "台体序号" || nameTemp == "台体类型" || nameTemp == "表位数量")
                {
                    continue;
                    //isReadOnly = true;
                }
                Grid gridTemp = new Grid()
                {
                    Margin = new Thickness(2),
                };
                while (gridTemp.ColumnDefinitions.Count < 2)
                {
                    gridTemp.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            Width = new GridLength(1, GridUnitType.Star)
                        });
                }
                gridTemp.ColumnDefinitions[0].Width = new GridLength(90);
                TextBlock textBlockName = new TextBlock()
                {
                    Text = nameTemp,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(3, 0, 3, 0),
                    ToolTip=nameTemp
                };
                TextBox textBoxValue = new TextBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment=HorizontalAlignment.Stretch,
                    Margin = new Thickness(3, 0, 3, 0),
                    TextWrapping=TextWrapping.WrapWithOverflow
                    //IsReadOnly =isReadOnly
                    
                };
                textBoxValue.SetBinding(TextBox.TextProperty, new Binding(nameTemp));
                Grid.SetColumn(textBoxValue, 1);
                gridTemp.Children.Add(textBlockName);
                gridTemp.Children.Add(textBoxValue);
                stackPanelEquipInfo.Children.Add(gridTemp);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DynamicViewModel equipInfo = stackPanelEquipInfo.DataContext as DynamicViewModel;
            if(equipInfo!=null)
            {
                Equipments.Instance.SaveEquipInfo(equipInfo);
            }
        }
    }
}
