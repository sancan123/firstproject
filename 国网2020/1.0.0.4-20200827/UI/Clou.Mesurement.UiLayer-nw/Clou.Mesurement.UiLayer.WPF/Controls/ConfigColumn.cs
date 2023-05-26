using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Mesurement.UiLayer.ViewModel.CodeTree;

namespace Mesurement.UiLayer.WPF.Controls
{

    public class ConfigColumn : DataGridComboBoxColumn
    {
        public ConfigColumn()
        {
            CodeTreeNode nodeTemp = CodeTreeViewModel.Instance.GetCodeByEnName("ConfigSource", 1);

            var names = from nodeChild in nodeTemp.Children select nodeChild.CODE_NAME;
            List<string> listTemp = new List<string> { ""};
            listTemp.AddRange(names);
            ItemsSource = listTemp;
        }
    }
}
