using Mesurement.UiLayer.ViewModel.Model;

namespace Mesurement.UiLayer.ViewModel.Config
{
    /// 配置信息节点
    /// <summary>
    /// 配置信息节点
    /// </summary>
    public class ConfigTreeNode:ViewModelBase
    {
        public int Level { get; set; }
        private string configNo;

        public string ConfigNo
        {
            get { return configNo; }
            set { SetPropertyValue(value, ref configNo, "ConfigNo"); }
        }
        
        private string configName;

        public string ConfigName
        {
            get { return configName; }
            set { SetPropertyValue(value, ref configName, "ConfigName"); }
        }

        private AsyncObservableCollection<ConfigTreeNode> children = new AsyncObservableCollection<ConfigTreeNode>();

        public AsyncObservableCollection<ConfigTreeNode> Children
        {
            get { return children; }
            set { SetPropertyValue(value, ref children, "Children"); }
        }
        
    }
}
