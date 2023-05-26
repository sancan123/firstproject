using System.Windows;
using System.Linq;
using System.Windows.Controls;
using Mesurement.UiLayer.ViewModel.CheckInfo;
using System.Windows.Data;
using Mesurement.UiLayer.ViewModel.Schema;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// DataBaseDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class DataBaseDisplay
    {
        /// 数据库显示
        /// <summary>
        /// 数据库显示
        /// </summary>
        public DataBaseDisplay()
        {
            InitializeComponent();
            Name = "结论配置";
            treeSchema.DataContext = FullTree.Instance;
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = SystemParameters.WorkArea.Size;
        }

        private DataBaseDisplayViewModel viewModel
        {
            get
            {
                try
                {
                    return Resources["dataDisplayViewModel"] as DataBaseDisplayViewModel;
                }
                catch
                {
                    return null;
                }
            }
        }
        private void tableNameChanged(object sender, SelectionChangedEventArgs e)
        {
            columnFieldPk.ItemsSource= viewModel.FieldNames;
            columnFieldFk.ItemsSource= viewModel.FieldNames;
        }

        private void ClickItemMove(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (button.Name.ToLower().Contains("fk"))
                {
                    viewModel.FKField = button.DataContext as FieldModelView;
                }
                else
                {
                    viewModel.PKField = button.DataContext as FieldModelView;
                }
                viewModel.CommandFactoryMethod(button.Name);
            }
        }

        public override void Dispose()
        {
            BindingOperations.ClearAllBindings(this);
            Resources.Clear(); listboxViewId.SelectionChanged -= ViewItemChanged;
            comboBoxTable.SelectionChanged -= tableNameChanged;
            base.Dispose();
        }

        private void ViewItemChanged(object sender, SelectionChangedEventArgs e)
        {
            viewModel.LoadFieldView();
        }

        private void AdvTree_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel nodeTemp = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (nodeTemp != null && nodeTemp.IsTerminal)
            {
                viewModel.ParaNo = nodeTemp.ParaNo;
                viewModel.ViewIds.SelectedUnit = viewModel.ViewIds.ViewUnits.FirstOrDefault(item => item.ViewId == nodeTemp.ViewNo);
                if (viewModel.ViewIds.SelectedUnit != null)
                {
                    FrameworkElement itemTemp = listboxViewId.ItemContainerGenerator.ContainerFromItem(viewModel.ViewIds.SelectedUnit) as FrameworkElement;
                    if (itemTemp != null)
                    {
                        itemTemp.BringIntoView();
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveFieldView();
            SchemaNodeViewModel nodeTemp = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (nodeTemp != null && nodeTemp.IsTerminal && viewModel.ViewIds.SelectedUnit != null)
            {
                nodeTemp.ViewNo = viewModel.ViewIds.SelectedUnit.ViewId;
            }
        }
    }
}
