using System.Windows.Controls;
using Mesurement.UiLayer.ViewModel.Schema;
using System.Windows;
using Mesurement.UiLayer.DataManager.Mark.ViewModel;
using System;
using Mesurement.UiLayer.DataManager.ViewModel.Mark;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// Interaction logic for ControlCheckItem.xaml
    /// </summary>
    public partial class ControlResultItem : UserControl
    {
        /// 检定结论项控件
        /// <summary>
        /// 检定结论项控件
        /// </summary>
        public ControlResultItem()
        {
            InitializeComponent();
            treeSchema.ItemsSource = FullTree.Instance.Children;
            comboBoxFormat.ItemsSource = Enum.GetValues(typeof(EnumFormat));
        }
        /// 方案视图
        /// <summary>
        /// 方案视图
        /// </summary>
        private ResultBookmarkMaker viewModel
        {
            get { return DataContext as ResultBookmarkMaker; }
        }

        private void ControlEnumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.LoadCurrentKey();
        }

        private void treeSchema_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (currentNode == null)
            {
                return;
            }
            viewModel.CategoryNo = currentNode.ParaNo;
            if (currentNode.Children.Count == 0)
            {
                viewModel.Schema.ParaNo = currentNode.ParaNo;
            }
            int temp = 0;
            for (int i = 0; i < viewModel.Schema.ParaInfo.CheckParas.Count; i++)
            {
                if (viewModel.Schema.ParaInfo.CheckParas[i].IsKeyMember)
                {
                    temp = temp + 1;
                    viewModel.Schema.ParaInfo.CheckParas[i].CodeValue = viewModel.Schema.ParaInfo.CheckParas[i].DefaultValue;
                }
            }
            if (temp == 0)
            {
                scrollviewer.Visibility = Visibility.Collapsed;
                textBlockPara.Visibility = Visibility.Visible;
            }
            else
            {
                scrollviewer.Visibility = Visibility.Visible;
                textBlockPara.Visibility = Visibility.Collapsed;
            }
            viewModel.LoadCurrentKey();
            viewModel.LoadResultNames();
        }
    }
}
