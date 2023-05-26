using System.Windows;
using System.Windows.Controls;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Schema;
using Mesurement.UiLayer.ViewModel.Schema.Error;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewSchema.xaml 的交互逻辑
    /// </summary>
    public partial class ViewSchemaItemConfig
    {
        public ViewSchemaItemConfig()
        {
            InitializeComponent();
            Name = "检定项配置";
            DockStyle.IsFloating = true;

            treeSchema.ItemsSource = FullTree.Instance.Children;

            for (int i=0;i<viewModelSchemas.Schemas.Count;i++)
            {
                if ((int)viewModelSchemas.Schemas[i].GetProperty("ID") == EquipmentData.Schema.SchemaId)
                {
                    viewModelSchemas.SelectedSchema = viewModelSchemas.Schemas[i];
                    break;
                }
            }
        }

        private void ComboBoxSchemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(viewModelSchemas.SelectedSchema!=null)
            {
                viewModel.LoadSchema((int)viewModelSchemas.SelectedSchema.GetProperty("ID"));
            }
        }

        void controlEror_PointsChanged(object sender, System.EventArgs e)
        {
            ErrorModel model = sender as ErrorModel;
            if (model is ErrorModel)
            {
                viewModel.UpdateErrorPoint(model);
            }
        }
        private SchemaOperationViewModel viewModelSchemas
        {
            get { return Resources["SchemasViewModel"] as SchemaOperationViewModel; }
        }
        private SchemaViewModel viewModel
        {
            get { return Resources["SchemaViewModel"] as SchemaViewModel; }
        }

        private void ButtonParaInfo_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;
            if (button.DataContext is CheckParaViewModel)
            {
                viewModel.ParaInfo.CheckParaCurrent = button.DataContext as CheckParaViewModel;
            }
            viewModel.ParaInfo.CommandFactoryMethod(button.Name);
            listBoxParaConfig.Items.Refresh();
        }
        public override void Dispose()
        {
            treeSchema.SelectedItemChanged -= AdvTree_ActiveItemChanged;
            base.Dispose();
        }

        private void AdvTree_ActiveItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SchemaNodeViewModel currentNode = treeSchema.SelectedItem as SchemaNodeViewModel;
            if (currentNode == null)
            {
                return;
            }
            viewModel.SchemaId = currentNode.SchemaId;
            if (currentNode.Children.Count == 0)
            {
                viewModel.ParaNo = currentNode.ParaNo;
            }
        }
    }
}
