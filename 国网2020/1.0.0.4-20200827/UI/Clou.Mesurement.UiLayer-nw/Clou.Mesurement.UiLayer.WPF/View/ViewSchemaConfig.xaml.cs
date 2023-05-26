using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.ViewModel.Schema.Error;
using Mesurement.UiLayer.WPF.Model;
using System.Collections.Generic;
using Mesurement.UiLayer.DAL;
using System.Windows.Data;
using System.Xml;
using System.IO;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewSchema.xaml 的交互逻辑
    /// </summary>
    public partial class ViewSchemaConfig
    {
        private DynamicViewModel currentSchema
        {
            get { return comboBoxSchemas.SelectedItem as DynamicViewModel; }
        }
        public ViewSchemaConfig()
        {
            InitializeComponent();
            Name = "编辑方案";
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = new Size(SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
            treeFramework.ItemsSource = FullTree.Instance.Children;
            gridSchemas.DataContext = EquipmentData.SchemaModels;
            comboBoxSchemas.SelectionChanged += ComboBoxSchemas_SelectionChanged;

            if (EquipmentData.SchemaModels.SelectedSchema != null)
            {
                comboBoxSchemas.SelectedItem = EquipmentData.SchemaModels.SelectedSchema;
                viewModel.LoadSchema((int)EquipmentData.SchemaModels.SelectedSchema.GetProperty("ID"));
            }

            controlError.PointsChanged += controlEror_PointsChanged;
            controlError.AllPoints.PropertyChanged += AllPoints_PropertyChanged;

            checkBoxErrorView.Checked += CheckBoxErrorView_Checked;
            checkBoxErrorView.Unchecked += CheckBoxErrorView_Checked;
        }

        private void AllPoints_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (viewModel.SelectedNode != null && (viewModel.SelectedNode.ParaNo == "02001"))
            {
                ViewModel.Model.AsyncObservableCollection<DynamicViewModel> viewModelsTemp = viewModel.ParaValuesView;
                foreach (DynamicViewModel modelTemp in viewModelsTemp)
                {
                    if (e.PropertyName == "LapCountIb")     //相对于Ib圈数
                    {
                        modelTemp.SetProperty("误差圈数(Ib)", controlError.AllPoints.LapCountIb);
                    }
                    else if (e.PropertyName == "GuichengMulti")      //规程误差限倍数
                    {
                        modelTemp.SetProperty("误差限倍数(%)", controlError.AllPoints.GuichengMulti);
                    }
                }
            }
        }

        void controlEror_PointsChanged(object sender, System.EventArgs e)
        {
            ErrorModel model = sender as ErrorModel;
            if (model is ErrorModel)
            {
                viewModel.UpdateErrorPoint(model);
            }
        }
        private SchemaViewModel viewModel
        {
            get { return Resources["SchemaViewModel"] as SchemaViewModel; }
        }

        #region 用户事件
        private void ButtonParaInfo_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            viewModel.ParaInfo.CommandFactoryMethod(button.Name);
        }

        public override void Dispose()
        {
            controlError.PointsChanged -= controlEror_PointsChanged;
            checkBoxErrorView.Checked -= CheckBoxErrorView_Checked;
            checkBoxErrorView.Unchecked -= CheckBoxErrorView_Checked;
            comboBoxSchemas.SelectionChanged -= ComboBoxSchemas_SelectionChanged;
            dataGridGeneral.DataContext = null;
            dataGridGeneral.Columns.Clear();
            dataGridGeneral.ItemsSource = null;
            base.Dispose();
        }

        private void Button_Click_RemoveNode(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                SchemaNodeViewModel node = button.DataContext as SchemaNodeViewModel;
                if (node.Level == 1)
                {
                    viewModel.Children.Remove(node);
                }
                else
                {
                    node.ParentNode.Children.Remove(node);
                }
                viewModel.RefreshPointCount();
            }
        }

        private void treeSchema_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (currentNode == null)
            {
                return;
            }
            if (currentNode.Children.Count == 0)
            {
                //切换检定点时保存(不保存到数据库)
                if (viewModel.SelectedNode != null)
                {
                    viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                }

                viewModel.SelectedNode = currentNode;
                viewModel.ParaNo = currentNode.ParaNo;
            }
            //02001:基本误差  修改20210802--ZRB
            //if (viewModel.ParaNo == "02001" )
            //{
            //    checkBoxErrorView.IsChecked = true;
            //    checkBoxErrorView.Visibility = Visibility.Visible;
            //    gridGeneral.Visibility = Visibility.Collapsed;
            //    scrollViewError.Visibility = Visibility.Visible;
            //    controlError.AllPoints.Load(viewModel.SelectedNode.ParaValuesCurrent);
            //}
            //else
            //{
                checkBoxErrorView.IsChecked = true;
                checkBoxErrorView.Visibility = Visibility.Collapsed;
                gridGeneral.Visibility = Visibility.Visible;
                scrollViewError.Visibility = Visibility.Collapsed;
            //}
        }

        private void Button_Click_RemoveItem(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
                if (modelTemp != null)
                {
                    viewModel.ParaValuesView.Remove(modelTemp);
                    if (viewModel.ParaValuesView.Count == 0)
                    {
                        if (viewModel.SelectedNode.Level == 1)
                        {
                            viewModel.Children.Remove(viewModel.SelectedNode);
                        }
                        else
                        {
                            viewModel.SelectedNode.ParentNode.Children.Remove(viewModel.SelectedNode);
                        }
                        viewModel.RefreshPointCount();
                        return;
                    }
                    else
                    {
                        viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
                        viewModel.RefreshPointCount();
                    }
                }
            }
        }

        private void Button_Click_AddItem(object sender, RoutedEventArgs e)
        {
            viewModel.AddNewParaValue();
            viewModel.RefreshPointCount();
        }

        private void ButtonClick_AddNode(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button == null) return;
            if (button.Name != "buttonAdd") return;
            SchemaNodeViewModel nodeTemp = button.DataContext as SchemaNodeViewModel;
            if (nodeTemp == null) return;
            List<SchemaNodeViewModel> nodeList = new List<SchemaNodeViewModel>();
            if (nodeTemp.IsTerminal)
            {
                nodeList.Add(nodeTemp);
            }
            else
            {
                nodeList = nodeTemp.GetTerminalNodes();
            }
            List<string> namesList = new List<string>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                string noTemp = nodeList[i].ParaNo;
                if (viewModel.ExistNode(noTemp))
                {
                    namesList.Add(nodeList[i].Name);
                    continue;
                }
                SchemaNodeViewModel nodeNew = viewModel.AddParaNode(noTemp);
                if (i == nodeList.Count - 1)
                {
                    SelectNode(nodeNew);
                }
            }
            if (namesList.Count > 0)
            {
                LogManager.AddMessage(string.Format("检定点:{0}已存在,将不会重复添加!", string.Join(",", namesList)), EnumLogSource.用户操作日志, EnumLevel.Tip);
            }
        }

        private void SelectNode(SchemaNodeViewModel nodeTemp)
        {
            if (nodeTemp != null)
            {
                List<SchemaNodeViewModel> nodesList = new List<SchemaNodeViewModel>();
                #region 获取链
                nodesList.Add(nodeTemp);
                SchemaNodeViewModel nodeParentTemp = nodeTemp.ParentNode;
                while (nodeParentTemp != null && nodeParentTemp.Level >= 1)
                {
                    if (nodeParentTemp != null)
                    {
                        nodesList.Add(nodeParentTemp);
                    }
                    nodeParentTemp = nodeParentTemp.ParentNode;
                    if (nodeParentTemp == null)
                    {
                        break;
                    }
                }
                #endregion

                nodesList = OrderByListChildren(nodesList);

                TreeViewItem treeItem = treeSchema.ItemContainerGenerator.ContainerFromItem(nodesList[nodesList.Count - 1]) as TreeViewItem;
                if (treeItem == null)
                {
                    return;
                }
                else
                {
                    treeItem.IsExpanded = true;
                    treeItem.IsSelected = true;
                    treeItem.BringIntoView();
                }
                for (int i = nodesList.Count - 2; i >= 0; i--)
                {
                    treeItem = treeItem.ItemContainerGenerator.ContainerFromItem(nodesList[i]) as TreeViewItem;
                    if (treeItem == null)
                    {
                        return;
                    }
                    else
                    {
                        treeItem.IsExpanded = true;
                        treeItem.IsSelected = true;
                        treeItem.BringIntoView();
                    }
                }
            }
        }

        /// <summary>
        /// 方案排序
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public List<SchemaNodeViewModel> OrderByListChildren(List<SchemaNodeViewModel> ViewModel)
        {
            List<SchemaNodeViewModel> ViewModelList = new List<SchemaNodeViewModel>();
            SchemaNodeViewModel ViewModeTmp = new SchemaNodeViewModel();


            //if (ViewModel == null) return ViewModelList;

            for (int k = 0; k < ViewModel.Count; k++)
            {
                for (int i = ViewModel[k].Children.Count; i > 0; i--)
                {
                    for (int j = 0; j < i - 1; j++)
                    {
                        if (int.Parse(ViewModel[k].Children[j].ParaNo) > int.Parse(ViewModel[k].Children[j + 1].ParaNo))
                        {
                            ViewModeTmp = ViewModel[k].Children[j];
                            ViewModel[k].Children[j] = ViewModel[k].Children[j + 1];
                            ViewModel[k].Children[j + 1] = ViewModeTmp;
                        }
                        if (ViewModel[k].Children[j].Children.Count > 0)
                        {
                            for (int p = 0; p < ViewModel[k].Children[j].Children.Count-1; p++)
                            {
                                if (int.Parse(ViewModel[k].Children[j].Children[p].ParaNo) > int.Parse(ViewModel[k].Children[j].Children[p + 1].ParaNo))
                                {
                                    ViewModeTmp = ViewModel[k].Children[j].Children[p];
                                    ViewModel[k].Children[j].Children[p] = ViewModel[k].Children[j].Children[p + 1];
                                    ViewModel[k].Children[j].Children[p + 1] = ViewModeTmp;
                                }
                            }
                        }

                    }
                }
            }

            return ViewModelList = ViewModel;

        }

        private void buttonClick_Save(object sender, RoutedEventArgs e)
        {
            viewModel.SaveParaValue();
        }

        private void buttonClick_SortDefault(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确定要默认排序吗?", "默认排序", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                viewModel.SortDefault();
            }
        }

        private void Event_NodeMove(object sender, DragEventArgs e)
        {
            SchemaNodeViewModel nodeSource = e.Data.GetData(typeof(SchemaNodeViewModel)) as SchemaNodeViewModel;
            if (nodeSource == null)
            { return; }
            Point pos = e.GetPosition(treeSchema);
            HitTestResult result = VisualTreeHelper.HitTest(treeSchema, pos);
            if (result == null)
                return;

            TreeViewItem selectedItem = Utils.FindVisualParent<TreeViewItem>(result.VisualHit);
            if (selectedItem == null)
            {
                return;
            }
            SchemaNodeViewModel nodeDest = selectedItem.DataContext as SchemaNodeViewModel;
            if (nodeDest == null)
            {
                return;
            }
            if (nodeDest == nodeSource)
            {
                return;
            }
            viewModel.MoveNode(nodeSource, nodeDest);
        }

        private void Button_Click_ItemUp(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
            if (modelTemp == null)
            {
                return;
            }
            int index = viewModel.ParaValuesView.IndexOf(modelTemp);
            if (index > 0)
            {
                viewModel.ParaValuesView.Move(index, index - 1);
            }
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
        }

        private void Button_Click_ItemDown(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            DynamicViewModel modelTemp = button.DataContext as DynamicViewModel;
            if (modelTemp == null)
            {
                return;
            }
            int index = viewModel.ParaValuesView.IndexOf(modelTemp);
            if (index < viewModel.ParaValuesView.Count - 1)
            {
                viewModel.ParaValuesView.Move(index, index + 1);
            }
            viewModel.SelectedNode.ParaValuesCurrent = viewModel.ParaValuesConvertBack();
        }

        private void CheckBoxErrorView_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel.ParaNo != "02001")
            {
                return;
            }
            if (checkBoxErrorView.IsChecked.HasValue && checkBoxErrorView.IsChecked.Value)
            {
                gridGeneral.Visibility = Visibility.Collapsed;
                scrollViewError.Visibility = Visibility.Visible;
            }
            else
            {
                gridGeneral.Visibility = Visibility.Visible;
                scrollViewError.Visibility = Visibility.Collapsed;
            }
        }

        private void ComboBoxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentSchema != null)
            {
                viewModel.LoadSchema((int)currentSchema.GetProperty("ID"));
            }
        }
        #endregion

        private void Click_SchemaOperation(object sender, RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null)
            {
                try
                {
                    switch (button.Name)
                    {
                        case "buttonNew":
                            MainViewModel.Instance.CommandFactoryMethod("新建方案|ViewSchemaOperation|新建方案");
                            break;
                        case "buttonDelete":
                            MainViewModel.Instance.CommandFactoryMethod("删除方案|ViewSchemaOperation|删除方案");
                            break;
                        case "buttonRename":
                            MainViewModel.Instance.CommandFactoryMethod("重命名方案|ViewSchemaOperation|重命名方案");
                            break;
                        case "buttonCopy":
                            MainViewModel.Instance.CommandFactoryMethod("复制方案|ViewSchemaOperation|复制方案");
                            break;
                    }
                }
                catch
                { }
            }
        }
        private XmlNode nodeDataFlags = null;
        /// <summary>
        /// 通信协议检查加载数据标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridGeneral_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (viewModel.ParaInfo != null && viewModel.ParaInfo.ParaNo == "14002")
            {
                DataGridComboBoxColumn column = e.Column as DataGridComboBoxColumn;
                if (column == null)
                {
                    return;
                }
                Binding bindingColumn = column.SelectedItemBinding as Binding;
                if (bindingColumn != null)
                {
                    string pathTemp = bindingColumn.Path.Path;
                    if (pathTemp == "数据项名称")
                    {
                        BindingExpression expressionTemp = e.EditingElement.GetBindingExpression(ComboBox.SelectedItemProperty);
                        DynamicViewModel modelTemp = expressionTemp.DataItem as DynamicViewModel;
                        //加载数据标识内容
                        if (modelTemp != null)
                        {
                            if (nodeDataFlags == null)
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.Load(string.Format(@"{0}\xml\DataFlagDict.xml", Directory.GetCurrentDirectory()));
                                nodeDataFlags = doc.DocumentElement;
                            }
                            foreach (XmlNode nodeTemp in nodeDataFlags.ChildNodes)
                            {
                                try
                                {
                                    ComboBox comboBox = e.EditingElement as ComboBox;
                                    if (nodeTemp.Attributes["DataFlagName"].Value == comboBox.SelectedItem.ToString())
                                    {
                                        modelTemp.SetProperty("标识编码", nodeTemp.Attributes["DataFlag"].Value);
                                        modelTemp.SetProperty("长度", nodeTemp.Attributes["DataLength"].Value);
                                        modelTemp.SetProperty("小数位", nodeTemp.Attributes["DataSmallNumber"].Value);
                                        modelTemp.SetProperty("数据格式", nodeTemp.Attributes["DataFormat"].Value);
                                        modelTemp.SetProperty("写入数据示例", nodeTemp.Attributes["Default"].Value);
                                        return;
                                    }
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
            }
        }
    }
}
