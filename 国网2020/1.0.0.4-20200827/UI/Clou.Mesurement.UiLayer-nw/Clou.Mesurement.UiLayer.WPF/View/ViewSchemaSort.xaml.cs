using DevComponents.WpfDock;
using Mesurement.UiLayer.ViewModel;
using System.Windows;
using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.ViewModel.Model;
using System.Collections.Generic;
using System.Windows.Controls;
using Mesurement.UiLayer.WPF.Model;
using System.Windows.Media;
using Mesurement.UiLayer.Utility.Log;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewSchemaSort
    {
        public ViewSchemaSort()
        {
            InitializeComponent();
            Name = "方案排序";
            InitializeNodes();
            listBox.ItemsSource = nodes;
        }

        private void InitializeNodes()
        {
            foreach (SchemaNodeViewModel node in FullTree.Instance.Children)
            {
                List<SchemaNodeViewModel> nodesTerminal = node.GetTerminalNodes();
                foreach (var item in nodesTerminal)
                {
                    nodes.Add(item);
                }
            }
            nodes.Sort(item => item.SortNo);

        }
        private AsyncObservableCollection<SchemaNodeViewModel> nodes = new AsyncObservableCollection<SchemaNodeViewModel>();

        private void listBox_Drop(object sender, DragEventArgs e)
        {
            SchemaNodeViewModel nodeSource = e.Data.GetData(typeof(SchemaNodeViewModel)) as SchemaNodeViewModel;
            if (nodeSource == null)
            { return; }
            Point pos = e.GetPosition(listBox);
            HitTestResult result = VisualTreeHelper.HitTest(listBox, pos);
            if (result == null)
                return;

            ListBoxItem selectedItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit);
            if (selectedItem == null)
            {
                return;
            }
            SchemaNodeViewModel nodeDest = selectedItem.DataContext as SchemaNodeViewModel;
            if (nodeDest == null)
            {
                return;
            }
            if (nodeDest == nodeSource)
            {
                return;
            }
            int oldeIndex = nodes.IndexOf(nodeSource);
            int newIndex = nodes.IndexOf(nodeDest);
            nodes.Move(oldeIndex, newIndex);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].SortNo = (i + 1).ToString("D3");
            }
            List<string> Sqlst = new List<string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                Sqlst.Add(string.Format("update SCHEMA_PARA_FORMAT set DEFAULT_SORT_NO='{0}' where PARA_NO='{1}'", nodes[i].SortNo, nodes[i].ParaNo));
            }
            int updateCount = DAL.DALManager.ApplicationDbDal.ExecuteOperation(Sqlst);
            LogManager.AddMessage(updateCount + "条，更新成功", EnumLogSource.数据库存取日志, EnumLevel.Tip);
        }
    }
}
