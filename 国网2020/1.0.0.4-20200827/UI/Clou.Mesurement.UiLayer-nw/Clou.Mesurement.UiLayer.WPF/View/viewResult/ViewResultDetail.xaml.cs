using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.WPF.Model;
using System.Windows;
using System.Windows.Data;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewResultDetail.xaml 的交互逻辑
    /// </summary>
    public partial class ViewResultDetail : DockControlDisposable
    {
        /// <summary>
        /// 显示检定时的详细结论
        /// </summary>
        public ViewResultDetail()
        {
            InitializeComponent();
            Name = "详细结论";
            DataContext = EquipmentData.CheckResults;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            EquipmentData.CheckResults.PropertyChanged += CheckResults_PropertyChanged;
            treeScheme.Loaded += treeScheme_Loaded;
        }

        void treeScheme_Loaded(object sender, RoutedEventArgs e)
        {
            if (EquipmentData.CheckResults.CheckNodeCurrent.Level == 1)
            {
                TreeViewItem treeItem = treeScheme.ItemContainerGenerator.ContainerFromItem(EquipmentData.CheckResults.CheckNodeCurrent.Parent) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
            ReloadColumn();
        }

        void CheckResults_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FlagLoadColumn" && !EquipmentData.CheckResults.FlagLoadColumn)
            {
                ReloadColumn();
            }
            if (e.PropertyName == "CheckNodeCurrent")
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    CheckNodeViewModel nodeSelected = treeScheme.SelectedItem as CheckNodeViewModel;
                    if (nodeSelected == EquipmentData.CheckResults.CheckNodeCurrent)
                    {
                        return;
                    }
                    if (EquipmentData.CheckResults.CheckNodeCurrent != null)
                    {
                        List<CheckNodeViewModel> nodeList = new List<CheckNodeViewModel>() { EquipmentData.CheckResults.CheckNodeCurrent };
                        CheckNodeViewModel nodeParent = EquipmentData.CheckResults.CheckNodeCurrent.Parent;
                        while (nodeParent != null)
                        {
                            nodeList.Add(nodeParent);
                            nodeParent = nodeParent.Parent;
                        }
                        TreeViewItem treeItem = treeScheme.ItemContainerGenerator.ContainerFromItem(nodeList[nodeList.Count - 1]) as TreeViewItem;
                        if (treeItem == null)
                        {
                            return;
                        }
                        else
                        {
                            treeItem.IsExpanded = true;
                        }
                        for (int i = nodeList.Count - 2; i >= 0; i--)
                        {
                            treeItem = treeItem.ItemContainerGenerator.ContainerFromItem(nodeList[i]) as TreeViewItem;
                            if (treeItem == null)
                            {
                                break;
                            }
                            else
                            {
                                treeItem.IsExpanded = true;
                            }
                        }
                        if (treeItem != null)
                        {
                            treeItem.IsSelected = true;
                            treeItem.BringIntoView();
                        }
                    }
                }));
            }
        }

        /// 加载检定点列
        /// <summary>
        /// 加载检定点列
        /// </summary>
        public void ReloadColumn()
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                    {
                        while (dataGridCheck.Columns.Count > 1)
                        {
                            BindingOperations.ClearAllBindings(dataGridCheck.Columns[1]);
                            dataGridCheck.Columns.Remove(dataGridCheck.Columns[1]);
                        }
                        List<string> columnNames = EquipmentData.CheckResults.CheckNodeCurrent.CheckResults[0].GetAllProperyName();
                        double widthTemp = dataGridCheck.ActualWidth;
                        double columnWidth = 100;
                        if (columnNames.Count > 1)
                        {
                            columnWidth = (widthTemp - 100) / (columnNames.Count-1);
                        }
                        if (columnWidth < 70)
                        {
                            columnWidth = 70;
                        }
                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            if (columnNames[i] == "要检" || columnNames[i]=="项目名")
                            {
                                continue;
                            }
                            DataGridTextColumn column = new DataGridTextColumn
                            {
                                Header = columnNames[i],
                                Binding = new Binding(columnNames[i]),
                                Width = new DataGridLength(columnWidth),
                                MinWidth = 50
                            };
                            dataGridCheck.Columns.Add(column);
                        };
                    }));
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(string.Format("控件加载异常:{0}", ex.Message), EnumLogSource.用户操作日志, EnumLevel.Warning, ex);
            }
        }

        public sealed override void Dispose()
        {
            //清除绑定
            dataGridCheck.Columns.Clear();
            decorator1.Target = null;
            treeScheme.DataContext = null;
            treeScheme.SelectedItemChanged -= treeScheme_SelectionChanged;
            EquipmentData.CheckResults.PropertyChanged -= CheckResults_PropertyChanged;
            DataContext = null;
            base.Dispose();
        }

        private void treeScheme_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                object obj = treeScheme.SelectedItem;
                if (EquipmentData.Controller.IsChecking)
                {
                    if (obj is CheckNodeViewModel)
                    {
                        if (((CheckNodeViewModel)obj).CheckResults.Count > 0)
                        {
                            EquipmentData.CheckResults.CheckNodeCurrent = (CheckNodeViewModel)obj;
                        }
                    }
                }
                else
                {
                    if (obj is CheckNodeViewModel)
                    {
                        if (((CheckNodeViewModel)obj).CheckResults.Count > 0)
                        {
                            EquipmentData.Controller.Index = EquipmentData.CheckResults.ResultCollection.IndexOf((CheckNodeViewModel)obj);
                        }
                    }
                }
            }));
        }
        #region 总结tab add by wzs on 20191030
        private void InitializeColumns()
        {
            GridViewColumnCollection columns = Application.Current.Resources["ColumnCollection"] as GridViewColumnCollection;
            while (columns.Count > 2)
            {
                columns.RemoveAt(2);
            }
            for (int i = 0; i < EquipmentData.Equipment.MeterCount; i++)
            {
                CheckBox checkBoxTemp = new CheckBox
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = string.Format("{0}表位", i + 1),
                };
              //  checkBoxTemp.PreviewMouseLeftButtonDown += CheckBoxTemp_PreviewMouseLeftButtonDown;
                Binding binding = new Binding("CHR_CHECKED");
                binding.Source = EquipmentData.MeterGroupInfo.Meters[i];
             //   binding.Converter = new BoolBitConverter();
              //  checkBoxTemp.SetBinding(ToggleButton.IsCheckedProperty, binding);
                GridViewColumn column = new GridViewColumn
                {
                    Header = checkBoxTemp,
                   
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
              //  bindingColor.Converter = new ResultColorConverter();
                factory.SetBinding(TextBlock.ForegroundProperty, bindingColor);
                column.CellTemplate = dataTemplateTemp;
                #endregion
                columns.Add(column);
            }
        }


        /// <summary>
        /// 更新当前方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            EquipmentData.SchemaModels.RefreshCurrrentSchema();
        }


        #endregion
    }
}
