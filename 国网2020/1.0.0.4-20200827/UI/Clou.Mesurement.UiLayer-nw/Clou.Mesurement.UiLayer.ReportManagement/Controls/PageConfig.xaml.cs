using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// Interaction logic for PageConfig.xaml
    /// </summary>
    public partial class PageConfig : Page
    {
        public PageConfig()
        {
            InitializeComponent();
            checkboxPreview.IsChecked = Properties.Settings.Default.IsPreview;
           
        }

        private void Click_SaveConfig(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsPreview = checkboxPreview.IsChecked.HasValue && checkboxPreview.IsChecked.Value;
            Properties.Settings.Default.Save();
        }
    }
}
