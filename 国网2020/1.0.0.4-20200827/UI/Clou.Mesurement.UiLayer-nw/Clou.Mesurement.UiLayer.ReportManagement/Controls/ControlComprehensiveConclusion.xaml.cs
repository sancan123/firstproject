using Mesurement.UiLayer.DataManager.Mark.ViewModel;
using Mesurement.UiLayer.DataManager.ViewModel.Mark;
using Mesurement.UiLayer.ViewModel.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// ControlComprehensiveConclusion.xaml 的交互逻辑
    /// </summary>
    public partial class ControlComprehensiveConclusion : UserControl
    {
        public ControlComprehensiveConclusion()
        {
            InitializeComponent();
            treeScheme.ItemsSource = FullTree.Instance.Children;
            comboBoxFormat.ItemsSource = Enum.GetValues(typeof(EnumFormat));
        }



        /// 方案视图
        /// <summary>
        /// 方案视图
        /// </summary>
        private ComprehensiveConclusionBookmarkMaker viewModel
        {
            get { return DataContext as ComprehensiveConclusionBookmarkMaker; }
        }

        private void ControlEnumComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.LoadCurrentKey();
        }

        private void treeScheme_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel currentNode = treeScheme.SelectedItem as SchemaNodeViewModel;
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
            //if (temp == 0)
            //{
            //    scrollviewer.Visibility = Visibility.Collapsed;
            //    textBlockPara.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    scrollviewer.Visibility = Visibility.Visible;
            //    textBlockPara.Visibility = Visibility.Collapsed;
            //}
            viewModel.LoadCurrentKey();
            viewModel.LoadResultNames();
        }
    }
}
