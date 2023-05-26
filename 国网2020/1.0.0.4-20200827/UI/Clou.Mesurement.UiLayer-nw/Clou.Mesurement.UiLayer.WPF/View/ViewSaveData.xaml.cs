using Mesurement.UiLayer.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Mesurement.UiLayer.WPF.Converter;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using Mesurement.UiLayer.DAL.DataBaseView;
using System.Collections.Generic;
using Mesurement.UiLayer.ViewModel.User;
using System;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// 审核存盘界面
    /// </summary>
    public partial class ViewSaveData
    {
        public ViewSaveData()
        {
            InitializeComponent();
            Name = "审核存盘";
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            LoadColumns();
            textBoxTemperature.Text = Properties.Settings.Default.Temperature;
            textBoxHumidy.Text = Properties.Settings.Default.Humidy;
            textBoxValidate.Text = Properties.Settings.Default.ValidateTime.ToString();
            treeSchema1.DataContext = EquipmentData.CheckResults;
            datagridMeters.SelectedIndex = 0;
            LoadMeterInfo();
            LoadUsers();
        }

        /// <summary>
        /// 所有表的结论
        /// </summary>
        private AllMeterResult viewModel
        {
            get { return Resources["AllMeterResult"] as AllMeterResult; }
        }
        /// <summary>
        /// 结论总览加载表信息列
        /// </summary>
        private void LoadColumns()
        {
            GridViewColumnCollection columns = Application.Current.Resources["ColumnCollectionSave"] as GridViewColumnCollection;
            while (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                GridViewColumn column = new GridViewColumn
                {
                    Header = string.Format("表位{0}", i + 1),
                    //DisplayMemberBinding = new Binding(string.Format("ResultSummary.表位{0}.ResultValue", i + 1)),
                    Width = 58,
                };
                #region 动态模板
                DataTemplate dataTemplateTemp = new DataTemplate();
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock), "textBlock");
                //上下文
                Binding bindingDataContext = new Binding(string.Format("ResultSummary.表位{0}", i + 1));
                factory.SetBinding(TextBlock.DataContextProperty, bindingDataContext);
                //文本
                Binding bindingText = new Binding("ResultValue");
                factory.SetBinding(TextBlock.TextProperty, bindingText);
                dataTemplateTemp.VisualTree = factory;
                Binding bindingColor = new Binding("Result");
                bindingColor.Converter = new ResultColorConverter();
                factory.SetBinding(TextBlock.ForegroundProperty, bindingColor);
                column.CellTemplate = dataTemplateTemp;
                #endregion
                columns.Add(column);
            }
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
            LoadMeterDataGrids(meterResult);
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
        /// 加载用户
        /// </summary>
        private void LoadUsers()
        {
            List<string> userNames = UserViewModel.Instance.GetList("");
            comboBoxAuditor.ItemsSource = userNames;
            comboBoxAuditor.SelectedItem = EquipmentData.LastCheckInfo.AuditPerson;

            comboBoxBoss.ItemsSource = userNames;

            comboBoxTester.ItemsSource = userNames;
            comboBoxTester.SelectedItem = EquipmentData.LastCheckInfo.TestPerson;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //温度,湿度,批准人,检验员,核验员,有效期,检定日期
            string[] arrayField = new string[]
                {
                    "AVR_TEMPERATURE",
                    "AVR_HUMIDITY",
                    "AVR_SUPERVISOR",
                    "AVR_TEST_PERSON",
                    "AVR_AUDIT_PERSON",
                    "DTM_VALID_DATE",
                     "DTM_TEST_DATE"
                };
            Properties.Settings.Default.Temperature = textBoxTemperature.Text;
            Properties.Settings.Default.Humidy = textBoxHumidy.Text;
            int intTemp = Properties.Settings.Default.ValidateTime;
            if(int.TryParse(textBoxValidate.Text, out intTemp))
            {
                Properties.Settings.Default.ValidateTime = intTemp;
            }
            Properties.Settings.Default.Save();
            //有效期
            string stringValidDate = DateTime.Now.AddYears(intTemp).ToString("yyyy-MM-dd");
            string strTestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string[] arrayValue = new string[]
                {
                    textBoxTemperature.Text,
                    textBoxHumidy.Text,
                    comboBoxBoss.SelectedItem as string,
                    comboBoxTester.Text,
                    comboBoxAuditor.SelectedItem as string,
                    stringValidDate,
                    strTestDate,
                };
            bool[] yaojianTemp = EquipmentData.MeterGroupInfo.YaoJian;
            List<string> sqlList = new List<string>();
            for (int i = 0; i < EquipmentData.MeterGroupInfo.Meters.Count; i++)
            {
                if (yaojianTemp[i])
                {
                    for (int j = 0; j < arrayField.Length; j++)
                    {
                        EquipmentData.MeterGroupInfo.Meters[i].SetProperty(arrayField[j], arrayValue[j]);
                    }
                }
            }

            viewModel.SaveAllInfo();
        }
    }
}
