using Clou.Mesurement.UiLayer.ViewModel.Model;

namespace Clou.Mesurement.UiLayer.ViewModel.Code
{
    /// 配置信息节点
    /// <summary>
    /// 配置信息节点
    /// </summary>
    public class CodeTreeNode:ViewModelBase
    {
        public int ID { get; set; }
        public int Level { get; set; }
        private string codeName;

        public string CodeName
        {
            get { return codeName; }
            set
            { 
                SetPropertyValue(value, ref codeName, "CodeName");
                ChangeFlag = true;
            }
        }

        private string codeDisplay;

        public string CodeDisplay
        {
            get { return codeDisplay; }
            set
            {
                SetPropertyValue(value, ref codeDisplay, "CodeDisplay");
                ChangeFlag = true;
            }
        }

        private string codeCategory;

        public string CodeCategory
        {
            get { return codeCategory; }
            set
            {
                SetPropertyValue(value, ref codeCategory, "CodeCategory");
                ChangeFlag = true;
            }
        }

        private bool changeFlag;

        public bool ChangeFlag
        {
            get { return changeFlag; }
            set { SetPropertyValue(value, ref changeFlag, "ChangeFlag"); }
        }


        private AsyncObservableCollection<CodeTreeNode> children = new AsyncObservableCollection<CodeTreeNode>();

        public AsyncObservableCollection<CodeTreeNode> Children
        {
            get { return children; }
            set { children = value; }
        }
    }
}
