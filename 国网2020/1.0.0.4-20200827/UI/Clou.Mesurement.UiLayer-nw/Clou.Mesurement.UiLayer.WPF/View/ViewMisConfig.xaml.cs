using Mesurement.UiLayer.ViewModel.Schema;
using System.Reflection;
using System.Windows;
using Mesurement.UiLayer.ViewModel.Mis;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewAbout.xaml
    /// </summary>
    public partial class ViewMisConfig
    {
        /// <summary>
        /// 检定结论上传到MIS平台数据库
        /// </summary>
        public ViewMisConfig()
        {
            InitializeComponent();
            Name = "上传配置";
            treeSchema.ItemsSource = FullTree.Instance.Children;
        }

        public MisConfigViewModel viewModel
        {
            get { return Resources["MisConfigViewModel"] as MisConfigViewModel; }
        }
        

        private void treeSchema_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel nodeTemp = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (nodeTemp != null && nodeTemp.IsTerminal)
            {
                viewModel.ParaNo = nodeTemp.ParaNo;
            }
        }
    }
}
