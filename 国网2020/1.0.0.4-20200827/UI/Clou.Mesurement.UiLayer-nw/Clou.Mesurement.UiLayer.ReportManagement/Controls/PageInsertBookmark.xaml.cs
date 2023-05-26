using Mesurement.UiLayer.DataManager.Mark.ViewModel;
using Mesurement.UiLayer.Utility;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.DataManager.Controls
{
    /// <summary>
    /// Interaction logic for ControlInsertBookmark.xaml
    /// </summary>
    public partial class PageInsertBookmark
    {
        public PageInsertBookmark()
        {
            InitializeComponent();
            TaskManager.AddUIAction(() =>
            {
                OpenTemplate();
            });
        }

        private DocumentViewModel viewModel
        {
            get { return Resources["DocumentViewModel"] as DocumentViewModel; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button is Button)
            {
                BookmarkModel model = button.DataContext as BookmarkModel;
                if (model != null)
                {
                    viewModel.DeleteBookmark(model);
                }
            }
        }

        private void AdvGrid_ActiveItemChanged(object sender, SelectionChangedEventArgs e)
        {
            BookmarkModel model = advGrid.SelectedItem as BookmarkModel;
            if (model != null)
            {
                string fileName = string.Format(@"{0}\ReportTemplate\{1}", Directory.GetCurrentDirectory(), Properties.Settings.Default.ReportPath);
                if (fileName.EndsWith(".doc") || fileName.EndsWith(".docx"))
                {
                viewModel.WordDocument.Application.Activate();
                viewModel.SelectBookmark(model);
                }
                else if (fileName.EndsWith(".xls") || fileName.EndsWith(".xlsx"))
                {
                    viewModel.ExcelWookbook.Activate();
                    viewModel.SelectBookmark(model);
                }
            }
        }
        public void OpenTemplate()
        {
            try
            {
                string fileName = string.Format(@"{0}\ReportTemplate\{1}", Directory.GetCurrentDirectory(), Properties.Settings.Default.ReportPath);
                if (File.Exists(fileName))
                {
                    viewModel.OpenTemplate();
                }
                else
                {
                    MessageBox.Show(string.Format("打开报表模板失败:文件不存在{0}", fileName), "打开报表模板失败");
                }
            }
            catch
            {
                MessageBox.Show("打开报表模板失败,请确认当前报表模板没有被其它软件打开", "打开报表模板失败");
            }
        }

        private void Click_Clearbookmarks(object sender, RoutedEventArgs e)
        {
            viewModel.ClearBookmarks();
        }

        private void Click_ReloadTemplate(object sender, RoutedEventArgs e)
        {
            TaskManager.AddUIAction(() =>
            {
                Document documentTemp = viewModel.WordDocument;
            });
        }
    }
}
