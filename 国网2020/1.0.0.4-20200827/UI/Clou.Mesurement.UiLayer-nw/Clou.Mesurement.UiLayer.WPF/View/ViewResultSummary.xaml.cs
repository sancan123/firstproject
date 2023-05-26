using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using DevComponents.WpfDock;
using Mesurement.UiLayer.WPF.Converter;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.ViewModel.WcfService;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ControlSchemeSummary.xaml 的交互逻辑
    /// </summary>
    public partial class ViewResultSummary
    {
        public ViewResultSummary()
        {
            InitializeComponent();
            Name = "结论总览";
            InitializeColumns();
            comboBoxSchema.DataContext = EquipmentData.SchemaModels;
            //treeScheme.DataContext = EquipmentData.CheckResults;
            treeSchema1.DataContext = EquipmentData.CheckResults;
            EquipmentData.CheckResults.PropertyChanged += CheckResults_PropertyChanged;
            DockStyle.Position = eDockSide.Tab;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            textBlockCheckPara.DataContext = EquipmentData.Controller;
            Binding bindingRefresh = new Binding("IsChecking");
            bindingRefresh.Source = EquipmentData.Controller;
            bindingRefresh.Converter = new NotBoolConverter();
            buttonRefresh.SetBinding(IsEnabledProperty, bindingRefresh);
            comboBoxSchema.SetBinding(IsHitTestVisibleProperty, bindingRefresh);
            treeSchema1.Loaded += treeScheme_Loaded;
        }

        void treeScheme_Loaded(object sender, RoutedEventArgs e)
        {
            if (EquipmentData.CheckResults.CheckNodeCurrent == null)
            {
                return;
            }
            if (EquipmentData.CheckResults.CheckNodeCurrent.Level == 1)
            {
                TreeViewItem treeItem = treeSchema1.ItemContainerGenerator.ContainerFromItem(EquipmentData.CheckResults.CheckNodeCurrent.Parent) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
        }
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
                checkBoxTemp.PreviewMouseLeftButtonDown += CheckBoxTemp_PreviewMouseLeftButtonDown;
                Binding binding = new Binding("CHR_CHECKED");
                binding.Source = EquipmentData.MeterGroupInfo.Meters[i];
                binding.Converter = new BoolBitConverter();
                checkBoxTemp.SetBinding(ToggleButton.IsCheckedProperty, binding);
                GridViewColumn column = new GridViewColumn
                {
                    Header = checkBoxTemp,
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

        private void CheckBoxTemp_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CheckBox temp = sender as CheckBox;
            if (temp.IsChecked.HasValue)
            {
                DynamicViewModel modelTemp = temp.GetBindingExpression(CheckBox.IsCheckedProperty).DataItem as DynamicViewModel;
                if (modelTemp != null)
                {
                    object objTemp = modelTemp.GetProperty("CHR_CHECKED");
                    if (objTemp.ToString() == "1")
                    {
                        modelTemp.SetProperty("CHR_CHECKED", "0");
                    }
                    else
                    {
                        modelTemp.SetProperty("CHR_CHECKED", "1");
                    }
                    TaskManager.AddWcfAction(() =>
                    {
                        WcfHelper.Instance.UpdateCheckFlag();
                    });
                    EquipmentData.CheckResults.RefreshYaojian();
                }
            }
            e.Handled = true;
        }

        private void CheckResults_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CheckNodeCurrent")
            {
                return;
            }
            Dispatcher.Invoke(new Action(() =>
            {
                CheckNodeViewModel nodeSelected = treeSchema1.SelectedItem as CheckNodeViewModel;
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
                    TreeViewItem treeItem = treeSchema1.ItemContainerGenerator.ContainerFromItem(nodeList[nodeList.Count - 1]) as TreeViewItem;
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

        public override void Dispose()
        {
            treeSchema1.Loaded -= treeScheme_Loaded;
            BindingOperations.ClearAllBindings(this);
            comboBoxSchema.DataContext = null;
            treeSchema1.DataContext = null;
            treeViewDecorator.Target = null;
            EquipmentData.CheckResults.PropertyChanged -= CheckResults_PropertyChanged;
            treeSchema1.SelectedItemChanged -= treeSchema1_SelectedItemChanged;
            base.Dispose();
        }

        private void treeSchema1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                object obj = treeSchema1.SelectedItem;
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

        /// <summary>
        /// 更新当前方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            EquipmentData.SchemaModels.RefreshCurrrentSchema();
        }

        private void Click_SchemaExpand(object sender, RoutedEventArgs e)
        {
            MenuItem menuTemp = e.OriginalSource as MenuItem;
            if (menuTemp != null)
            {
                for (int i = 0; i < EquipmentData.CheckResults.Categories.Count; i++)
                {
                    SetNodeExpanded(EquipmentData.CheckResults.Categories[i], menuTemp.Name == "menuExpand");
                }
            }
        }
        private void SetNodeExpanded(CheckNodeViewModel nodeTemp,bool isExpanded)
        {
            if (nodeTemp != null)
            {
                for (int i = 0; i < nodeTemp.Children.Count; i++)
                {
                    
                    nodeTemp.IsExpanded = isExpanded;
                    for (int j = 0; j < nodeTemp.Children.Count; j++)
                    {
                        
                        SetNodeExpanded(nodeTemp.Children[i], isExpanded);
                        
                    }
                }
            }
        }
    }
}
