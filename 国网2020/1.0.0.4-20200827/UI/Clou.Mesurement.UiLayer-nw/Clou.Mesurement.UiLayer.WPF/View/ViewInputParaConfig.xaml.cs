using System;
using Mesurement.UiLayer.ViewModel.InputPara;
using Mesurement.UiLayer.ViewModel.CodeTree;
using System.Collections.Generic;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewInputParaConfig
    {
        public ViewInputParaConfig()
        {
            InitializeComponent();
            Name = "参数录入配置";
            DockStyle.IsFloating = true;
            LoadDropDownList();
        }
        private void LoadDropDownList()
        {
            columnValueType.ItemsSource = Enum.GetValues(typeof(InputParaUnit.EnumValueType));
            List<string> listTemp = new List<string>() { ""};
            foreach (CodeTreeNode node in CodeTreeViewModel.Instance.CodeNodes)
            {
                foreach (CodeTreeNode nodeChild in node.Children)
                {
                    if (nodeChild.CODE_PARENT == "CheckParamSource" || nodeChild.CODE_PARENT == "ConfigSource")
                    {
                        listTemp.Add(nodeChild.CODE_NAME);
                    }
                }
            }
            columnCodeType.ItemsSource = listTemp;
        }
    }
}
