using Mesurement.UiLayer.ViewModel.Log;
using System.Windows;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewFrameLog
    {
        public ViewFrameLog()
        {
            InitializeComponent();
            Name = "报文记录";
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
        }
        private FrameLogViewModel viewModel
        {
            get
            {
                return Resources["FrameLogViewModel"] as FrameLogViewModel;
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
