namespace Mesurement.UiLayer.ViewModel.Log
{
    /// <summary>
    /// 查询条件模块
    /// </summary>
    public class SearchModule:ViewModelBase
    {
        /// <summary>
        /// 单个条件条件
        /// </summary>
        public class SearchCondition : ViewModelBase
        {
            private bool isSelected;
            /// <summary>
            /// 选中条件
            /// </summary>
            public bool IsSelected
            {
                get { return isSelected; }
                set { SetPropertyValue(value, ref isSelected, "IsSelected"); }
            }
            private string textCondition;
            /// <summary>
            /// 条件内容
            /// </summary>
            public string TextCondition
            {
                get { return textCondition; }
                set { SetPropertyValue(value, ref textCondition, "TextCondition"); }
            }
        }
        private bool isSelected;
        /// <summary>
        /// 模块选中
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetPropertyValue(value, ref isSelected, "IsSelected"); }
        }
        private string header;
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Header
        {
            get { return header; }
            set { SetPropertyValue(value, ref header, "Header"); }
        }

        private Model.AsyncObservableCollection<SearchCondition> itemsSource=new Model.AsyncObservableCollection<SearchCondition>();
        /// <summary>
        /// 条件集合
        /// </summary>
        public Model.AsyncObservableCollection<SearchCondition> ItemsSource
        {
            get { return itemsSource; }
            set { SetPropertyValue(value, ref itemsSource, "ItemsSource"); }
        }
    }
}
