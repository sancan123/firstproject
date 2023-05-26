using Mesurement.UiLayer.ViewModel.Log;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLogBox
    {
        public ViewLogBox()
        {
            InitializeComponent();
            Name = "日志记录";
            DockStyle.IsFloating = true;
        }
        private LogSearchViewModel viewModel
        {
            get
            {
                return Resources["LogSearchViewModel"] as LogSearchViewModel;
            }
        }
        public override void Dispose()
        {
            viewModel.Dispose();
            dataGrid.Columns.Clear();
            Resources.Clear();
            base.Dispose();
        }
    }
}
