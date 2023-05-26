using Mesurement.UiLayer.DataManager.Converter;
using Mesurement.UiLayer.DataManager.ViewModel.Meters;
using Mesurement.UiLayer.ViewModel.InputPara;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using Mesurement.UiLayer.ViewModel;
using System.IO;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using Mesurement.UiLayer.Utility;
using System;
using Mesurement.UiLayer.DataManager.ViewModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// Interaction logic for ControlMeters.xaml
    /// </summary>
    public partial class PageMeters
    {
        public PageMeters()
        {
            InitializeComponent();
            LoadColumns();
            LoadTemplates();
            viewModel.SearchMeters();
            checkBoxPreview.IsChecked = Properties.Settings.Default.IsPreview;
            datagridMeter.ColumnReordered += datagridMeter_ColumnReordered;
        }

        void datagridMeter_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            var columnsTemp = datagridMeter.Columns.OrderBy(item => item.DisplayIndex);
            var columnNames = from item in columnsTemp select item.Header.ToString();
            Properties.Settings.Default.ColumnNames = string.Join(",", columnNames);
            Properties.Settings.Default.Save();
        }
        /// <summary>
        /// 表信息模型
        /// </summary>
        private MetersViewModel viewModel
        {
            get
            {
                return Resources["MetersViewModel"] as MetersViewModel;
            }
        }
        /// <summary>
        /// 加载列
        /// </summary>
        private void LoadColumns()
        {
            if (Properties.Settings.Default.ColumnNames == null)
            {
                Properties.Settings.Default.ColumnNames = "";
            }
            string[] columnNames = Properties.Settings.Default.ColumnNames.Split(',');
            if (columnNames.Length != MetersViewModel.ParasModel.AllUnits.Count)
            {
                columnNames = new string[MetersViewModel.ParasModel.AllUnits.Count];
                for (int i = 0; i < MetersViewModel.ParasModel.AllUnits.Count; i++)
                {
                    columnNames[i] = MetersViewModel.ParasModel.AllUnits[i].DisplayName;
                }

                Properties.Settings.Default.ColumnNames = string.Join(",", columnNames);
                Properties.Settings.Default.Save();
            }
            for (int i = 0; i < columnNames.Length; i++)
            {
                InputParaUnit paraUnit = MetersViewModel.ParasModel.AllUnits.FirstOrDefault(item=>item.DisplayName==columnNames[i]);
                #region 要检
                if (paraUnit.FieldName == "CHR_CHECKED")
                {
                    Binding cellBinding = new Binding("CHR_CHECKED");
                    cellBinding.Mode = BindingMode.TwoWay;
                    cellBinding.Converter = new BoolBitConverter();
                    DataGridColumn columnYaojian = new DataGridCheckBoxColumn
                    {
                        Header = paraUnit.DisplayName,
                        Binding = cellBinding,
                        IsReadOnly = true
                    };
                    datagridMeter.Columns.Add(columnYaojian);
                }
                #endregion
                else
                {
                    DataGridColumn column = new DataGridTextColumn
                    {
                        Header = paraUnit.DisplayName,
                        Binding = new Binding(paraUnit.FieldName),
                        IsReadOnly = true
                    };
                    datagridMeter.Columns.Add(column);
                }
            }
        }

        private ContextMenu menuTemp
        {
            get { return Resources["contextMenu"] as ContextMenu; }
        }

        private void datagridMeter_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region 寻找鼠标点到的单元格
            DataGridCell cellTemp = null;
            Point pointTemp = e.GetPosition(datagridMeter);
            HitTestResult hitResult = VisualTreeHelper.HitTest(datagridMeter, pointTemp);
            if (hitResult != null)
            {
                cellTemp = Utils.FindVisualParent<DataGridCell>(hitResult.VisualHit);
            }
            if (cellTemp == null)
            {
                //menuTemp.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                menuTemp.Visibility = Visibility.Visible;
            }
            #endregion
            DynamicViewModel modelTemp = cellTemp.DataContext as DynamicViewModel;
            string fieldName = "";
            #region 获取列对应的字段名称
            if (cellTemp.Column is DataGridTextColumn)
            {
                DataGridTextColumn columnTemp = cellTemp.Column as DataGridTextColumn;
                Binding bindingTemp = columnTemp.Binding as Binding;
                fieldName = bindingTemp.Path.Path;
            }
            else
            {
                if (cellTemp.Column is DataGridCheckBoxColumn)
                {
                    DataGridCheckBoxColumn columnTemp = cellTemp.Column as DataGridCheckBoxColumn;
                    Binding bindingTemp = columnTemp.Binding as Binding;
                    fieldName = bindingTemp.Path.Path;
                }
            }
            #endregion
            if (string.IsNullOrEmpty(fieldName))
            {
                //menuTemp.Visibility = Visibility.Collapsed;
                return;
            }
            object cellValue = modelTemp.GetProperty(fieldName);
            string temp = "";
            if (cellValue != null)
            {
                temp = cellValue.ToString();
            }
            viewModel.LoadFilterCollection(fieldName, temp);
        }

        private void ContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuTemp = e.OriginalSource as MenuItem;
            if (menuTemp == null)
            {
                return;
            }
            SearchMenuItem searchItem = menuTemp.DataContext as SearchMenuItem;
            if (searchItem.CompareExpression == EnumCompare.清空筛选条件)
            {
                viewModel.SearchList.Clear();
            }
            else if (searchItem.CompareExpression == EnumCompare.自定义筛选条件)
            {
                WindowSearchItem windowTemp = new WindowSearchItem();
                windowTemp.DataContext = searchItem.SearchItemChild;
                bool? boolTemp = windowTemp.ShowDialog();
                if (boolTemp.HasValue && boolTemp.Value)
                {
                    viewModel.SearchList.Add(searchItem.SearchItemChild.ToString());
                }
                else
                {
                    return;
                }
            }
            else
            {
                viewModel.SearchList.Add(searchItem.ToString());
            }
            viewModel.SearchMeters();
        }
        /// <summary>
        /// 查看检定数据页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_CheckData(object sender, RoutedEventArgs e)
        {
            MainWindow windowTemp = Application.Current.MainWindow as MainWindow;

            PageCheckData pageCheckData = windowTemp.Pages.FirstOrDefault(item => item is PageCheckData) as PageCheckData;
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            if (pageCheckData == null)
            {
                if (meters == null || meters.Count() > 0)
                {
                    pageCheckData = new PageCheckData();
                }
                else
                {
                    MessageBox.Show("请至少选择一块表!");
                    return;
                }
                windowTemp.Pages.Add(pageCheckData);
            }
            pageCheckData.LoadCheckData(meters);
            windowTemp.frameMain.Navigate(pageCheckData);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBoxTemp = sender as CheckBox;
            if (checkBoxTemp != null)
            {
                DynamicViewModel modelTemp = checkBoxTemp.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    if (checkBoxTemp.IsChecked.HasValue && checkBoxTemp.IsChecked.Value)
                    {
                        modelTemp.SetProperty("IsSelected", true);
                    }
                    else
                    {
                        modelTemp.SetProperty("IsSelected", false);
                    }
                }
            }
        }

        /// <summary>
        /// 加载报表模板列表
        /// </summary>
        private void LoadTemplates()
        {
            string[] fileNames = Directory.GetFiles(string.Format(@"{0}\ReportTemplate", Directory.GetCurrentDirectory()));
            System.Collections.Generic.List<string> listNames = new System.Collections.Generic.List<string>();
            foreach (string fileName in fileNames)
            {
                string[] arrayName = fileName.Split('\\');
                string nameTemp = arrayName[arrayName.Length - 1];
                if (nameTemp.EndsWith(".doc") || nameTemp.EndsWith(".docx") || nameTemp.EndsWith(".xls") || nameTemp.EndsWith(".xlsx"))
                {
                    listNames.Add(nameTemp);
                }
            }
            comboBoxTemplates.ItemsSource = listNames;
            comboBoxTemplates.SelectedItem = Properties.Settings.Default.ReportPath;
        }

        private void comboBoxTemplates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxTemplates.SelectedItem != null)
            {
                Properties.Settings.Default.ReportPath = comboBoxTemplates.SelectedItem.ToString();
                Properties.Settings.Default.Save();

                MainWindow windowTemp = Application.Current.MainWindow as MainWindow;
                if (windowTemp != null)
                {
                    PageInsertBookmark pageWordTemplate = windowTemp.Pages.FirstOrDefault(item => item is PageInsertBookmark) as PageInsertBookmark;
                    if (pageWordTemplate != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            pageWordTemplate.OpenTemplate();
                        }));
                    }
                }
            }
        }
        /// <summary>
        /// 转到报表模板编辑页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_WordTemplate(object sender, RoutedEventArgs e)
        {
            string fileName = string.Format(@"{0}\ReportTemplate\{1}", Directory.GetCurrentDirectory(), Properties.Settings.Default.ReportPath);
            MessageDisplay.Instance.Message = string.Format("正在打开报表模板,文件路径为:{0},请稍后...", fileName);
            MainWindow windowTemp = Application.Current.MainWindow as MainWindow;

            Page pageWordTemplate = windowTemp.Pages.FirstOrDefault(item => item is PageInsertBookmark);
            if (pageWordTemplate == null)
            {
                pageWordTemplate = new PageInsertBookmark();
                windowTemp.Pages.Add(pageWordTemplate);
            }
            windowTemp.frameMain.Navigate(pageWordTemplate);
        }

        private void Click_PrintReport(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string templateName = Properties.Settings.Default.ReportPath;
            buttonPrint.IsEnabled = false;
            TaskManager.AddUIAction(() =>
            {
                int i = 1;
                int countTemp = meters.Count();
                try
                {
                    foreach (DynamicViewModel meter in meters)
                    {
                        string barcode = meter.GetProperty("AVR_BAR_CODE") as string;
                        MessageDisplay.Instance.Message = string.Format("开始打印条码号为:{0}的电能表检定信息,第{1}块/共{2}块,请等待", barcode, i, meters.Count());
                        ReportHelper.PrintReport(meter);
                        i++;
                    }

                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = string.Format("打印检定报表时出错:{0}", ex.Message);
                }
                Dispatcher.Invoke(new Action(() => buttonPrint.IsEnabled = true));
            });
        }

        private void Click_Config(object sender, RoutedEventArgs e)
        {
            MainWindow windowTemp = Application.Current.MainWindow as MainWindow;
            if (windowTemp != null)
            {
                Page pageConfig = windowTemp.Pages.FirstOrDefault(item => item is PageConfig) as Page;
                if (pageConfig == null)
                {
                    pageConfig = new PageConfig();
                }
                windowTemp.frameMain.Navigate(pageConfig);
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsPreview = checkBoxPreview.IsChecked.HasValue && checkBoxPreview.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void datagridMeter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList listTemp = e.AddedItems;
            foreach(object obj in listTemp)
            {
                DynamicViewModel modelTemp = obj as DynamicViewModel;
                if (modelTemp != null)
                {
                    object boolTemp = modelTemp.GetProperty("IsSelected");
                    if (boolTemp is bool)
                    {
                        modelTemp.SetProperty("IsSelected", !((bool)boolTemp));
                    }
                }
            }
        }
        /// <summary>
        /// 查询表信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_SearchMeters(object sender, RoutedEventArgs e)
        {
            viewModel.SearchMeters1();
        }

        private void Click_PrintReportToOne(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string templateName = Properties.Settings.Default.ReportPath;
            buttonPrint.IsEnabled = false;
            TaskManager.AddUIAction(() =>
            {
                int i = 1;
                int countTemp = meters.Count();
                try
                {
                    MessageDisplay.Instance.Message = string.Format("开始打印电能表检定信息,请等待");
                    List<DynamicViewModel> meterlist = new List<DynamicViewModel>();
                    foreach (DynamicViewModel meter in meters)
                    {
                        meterlist.Add(meter);
                    }
                    ReportHelper.PrintReport(meterlist);
                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = string.Format("打印检定报表时出错:{0}", ex.Message);
                }
                Dispatcher.Invoke(new Action(() => buttonPrint.IsEnabled = true));
            });
        }

        /// <summary>
        /// 打印Excel结论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_PrintExcel(object sender, RoutedEventArgs e)
        {
            var meters = viewModel.Meters.Where(item => ((bool)item.GetProperty("IsSelected")));
            if (meters == null || meters.Count() == 0)
            {
                MessageBox.Show("请至少选择一块表!");
                return;
            }
            string templateName = Properties.Settings.Default.ReportPath;
            buttonPrint.IsEnabled = false;
            TaskManager.AddUIAction(() =>
            {
                int i = 1;
                int countTemp = meters.Count();
                try
                {
                    MessageDisplay.Instance.Message = string.Format("开始打印电能表检定信息,请等待");
                    List<DynamicViewModel> meterlist = new List<DynamicViewModel>();
                    foreach (DynamicViewModel meter in meters)
                    {
                        meterlist.Add(meter);
                    }
                    ReportHelper.PrintExelConclusion(meterlist);
                }
                catch (Exception ex)
                {
                    MessageDisplay.Instance.Message = string.Format("打印Excel结论时出错:{0}", ex.Message);
                }
                if (Properties.Settings.Default.IsPreview)
                {
                    string destPath = string.Format(@"{0}\Reports", Directory.GetCurrentDirectory());
                    string destFileName = string.Format(@"{0}\temp.xls", destPath);
                    Process.Start(destFileName);
                }
            });
        }

    }
}
