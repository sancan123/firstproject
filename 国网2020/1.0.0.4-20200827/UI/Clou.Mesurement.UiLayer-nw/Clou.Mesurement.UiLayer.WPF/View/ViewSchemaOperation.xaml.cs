using Mesurement.UiLayer.WPF.Schema;
using System.Windows;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewSchemaOperation
    {
        /// 方案操作视图
        /// <summary>
        /// 方案操作视图
        /// </summary>
        /// <param name="operationType"></param>
        public ViewSchemaOperation(string operationType)
        {
            InitializeComponent();
            switch (operationType)
            {
                case "新建方案":
                    Content  = new ViewAddSchema(); 
                    break;
                case "复制方案":
                    Content = new ViewCopySchema();
                    break;
                case "重命名方案":
                    Content = new ViewRenameSchema();
                    break;
                case "删除方案":
                    Content = new ViewDeleteSchema();
                    break;
            }
            Name = operationType;
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = new Size(700, 600);
        }
    }
}
