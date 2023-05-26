using Mesurement.UiLayer.DataManager.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using Mesurement.UiLayer.DataManager.ViewModel;
using Mesurement.UiLayer.DataManager.Mark.ViewModel;
using System.Runtime.InteropServices;

namespace Mesurement.UiLayer.DataManager
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// 主窗体
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            PageMeters pageMeters = new PageMeters();
            Pages.Add(pageMeters);
            frameMain.Navigate(pageMeters);
            textBlockMessage.DataContext = MessageDisplay.Instance;
        }

        /// <summary>
        /// 窗体的拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private List<Page> pages=new List<Page>();
        /// <summary>
        /// 页面列表
        /// </summary>
        public List<Page> Pages { get { return pages; } set { pages = value; } }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                ReportHelper.WordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                KillExcel(ReportHelper.ExcelApp);
            }
            catch
            { }
            try
            {
                DocumentViewModel.WordApp.Quit(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                KillExcel(DocumentViewModel.ExcelApp);
            }
            catch
            { }
            base.OnClosed(e);
        }

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        /// <summary>
        /// 退出Excel
        /// </summary>
        private static void KillExcel(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            if (excelApp != null)
                excelApp.Quit();
            GC.Collect();  // 回收资源

            IntPtr t = new IntPtr(excelApp.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口 
            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }
    }
}
