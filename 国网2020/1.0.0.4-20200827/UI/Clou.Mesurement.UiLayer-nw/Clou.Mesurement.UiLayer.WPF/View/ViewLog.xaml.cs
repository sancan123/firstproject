using System.Windows;
using System.Windows.Data;
using Mesurement.UiLayer.ViewModel.Log;
using DevComponents.WpfDock;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewLog.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLog
    {
        public ViewLog()//string sourceString)
        {
            //EnumLogSource logSource = EnumLogSource.用户操作日志;
            //Enum.TryParse(sourceString, out logSource);
            InitializeComponent();
            Name = "运行日志";
            //Name = logSource.ToString();
            //switch (logSource)
            //{
            //    case EnumLogSource.用户操作日志:
            //        dataGrid.ItemsSource = LogViewModel.Instance.LogsUserOperation;
            //        break;
            //    case EnumLogSource.数据库存取日志:
            //        dataGrid.ItemsSource = LogViewModel.Instance.LogsDatabase;
            //        break;
            //    case EnumLogSource.检定业务日志:
                    dataGrid.ItemsSource = LogViewModel.Instance.LogsCheckLogic;
            //        break;
            //    case EnumLogSource.设备操作日志:
            //        dataGrid.ItemsSource = LogViewModel.Instance.LogsDevice;
            //        break;
            //}
            DockStyle.Position = eDockSide.Bottom;
           LogViewModel.LogCollection logCollection = dataGrid.ItemsSource as LogViewModel.LogCollection;
           if (logCollection != null)
           {
               logCollection.CollectionChanged += logCollection_CollectionChanged;
           }
        }

        void logCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LogViewModel.LogCollection logCollection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            if (logCollection != null && logCollection.Count > 0)
            {
                dataGrid.ScrollIntoView(logCollection[logCollection.Count - 1]);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            LogViewModel.LogCollection collection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            if (collection != null)
            {
                collection.Clear();
            }
        }

        public override void Dispose()
        {
            LogViewModel.LogCollection logCollection = dataGrid.ItemsSource as LogViewModel.LogCollection;
            if (logCollection != null)
            {
                logCollection.CollectionChanged -= logCollection_CollectionChanged;
            }
            //清除绑定
            BindingOperations.ClearAllBindings(this);
            dataGrid.ItemsSource = null;
            menuItemClearLog.Click -= MenuItem_Click;
            base.Dispose();
        }
    }
}
