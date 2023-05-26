using Mesurement.UiLayer.ViewModel.Model;

namespace Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// 检定结论总览视图
    /// <summary>
    /// 检定结论总览视图
    /// </summary>
    public class SummaryResultViewModel : ViewModelBase
    {
        private string checkPointName;

        public string CheckPointName
        {
            get { return checkPointName; }
            set { SetPropertyValue(value, ref checkPointName, "CheckPointName"); }
        }
        

        private DynamicViewModel result = new DynamicViewModel(0);
        /// 检定结论总览
        /// <summary>
        /// 检定结论总览
        /// </summary>
        public DynamicViewModel Result
        {
            get { return result; }
            set { SetPropertyValue(value, ref result, "Result"); }
        }

        private AsyncObservableCollection<SummaryResultViewModel> children = new AsyncObservableCollection<SummaryResultViewModel>();
        /// 分层次的检定点列表
        /// <summary>
        /// 分层次的检定点列表
        /// </summary>
        public AsyncObservableCollection<SummaryResultViewModel> Children
        {
            get { return children; }
            set { children = value; }
        }
        /// <summary>
        /// 父元素
        /// </summary>
        public SummaryResultViewModel Parent { get; set; }
    }
}
