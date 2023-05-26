﻿using System.Windows;
using Mesurement.UiLayer.ViewModel.CodeTree;
using System.Windows.Controls;
using System.Collections.Generic;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewCodeTree
    {
        public ViewCodeTree()
        {
            InitializeComponent();
            Name = "编码配置";
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
            DockStyle.IsFloating = true;

            gridRoot.DataContext = CodeTreeViewModel.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            #region 获取按钮和数据
            if (button == null)
            {
                return;
            }
            CodeTreeNode codeNode = button.DataContext as CodeTreeNode;
            if (codeNode == null)
            {
                return;
            }
            #endregion
            string buttonName = button.Name;
            switch (buttonName)
            {
                case "buttonSave":
                    codeNode.SaveCode();
                    LogManager.AddMessage("编码信息已保存", EnumLogSource.用户操作日志, EnumLevel.Tip);
                    break;
                case "buttonAdd":
                    codeNode.AddCode();
                    break;
                case "buttonDelete":
                    if (MessageBox.Show("确认要删除选中的编码及所有子节点吗?", "删除节点", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    {
                        codeNode.DeleteCode();
                    }
                    break;
            }
        }

        private void Button_Click_Search(object sender, RoutedEventArgs e)
        {
            CodeTreeViewModel.Instance.SearchNodes();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Event_NodeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement elementTemp = e.OriginalSource as FrameworkElement;
            if(elementTemp ==null)
            {
                return;
            }
            CodeTreeNode nodeTemp = elementTemp.DataContext as CodeTreeNode;
            if (nodeTemp != null)
            {
                List<CodeTreeNode> nodesList = new List<CodeTreeNode>();
                #region 获取链
                nodesList.Add(nodeTemp);
                CodeTreeNode nodeParentTemp = nodeTemp.Parent;
                while (nodeParentTemp != null && nodeParentTemp.CODE_LEVEL >= 1)
                {
                    if (nodeParentTemp != null)
                    {
                        nodesList.Add(nodeParentTemp);
                    }
                    nodeParentTemp = nodeParentTemp.Parent;
                    if (nodeParentTemp == null)
                    {
                        break;
                    }
                }
                #endregion
                TreeViewItem treeItem = treeViewCode.ItemContainerGenerator.ContainerFromItem(nodesList[nodesList.Count - 1]) as TreeViewItem;
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

        public override void Dispose()
        {
            //重新加载编码字典
            //最简单野蛮的方法
            CodeTreeViewModel.Instance.InitializeTree();
            base.Dispose();
        }
    }
}
