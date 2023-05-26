
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace Mesurement.UiLayer.DataManager
{
    /// <summary>
    /// Interaction logic for WindowTemplatePath.xaml
    /// </summary>
    public partial class WindowTemplatePath
    {
        public WindowTemplatePath()
        {
            InitializeComponent();
            textBoxPath.Text = Properties.Settings.Default.ReportPath;
        }

        private void Click_Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Click_Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory() + @"\ReportTemplate";
            fileDialog.Filter = "word文件|*.doc;*.docx";
            bool? result = fileDialog.ShowDialog(this);
            if (result.HasValue && result.Value)
            {
                Properties.Settings.Default.ReportPath = fileDialog.FileName;
                textBoxPath.Text = Properties.Settings.Default.ReportPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
