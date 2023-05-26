using Mesurement.UiLayer.ViewModel.Menu;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewMenuConfig
    {
        public ViewMenuConfig()
        {
            InitializeComponent();
            //InitializeColumns();
            Name = "目录配置";
        }
        private MenuViewModel viewModel
        {
            get { return Resources["MenuViewModel"] as MenuViewModel; }
        }
    }
}
