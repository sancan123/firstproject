using DevComponents.WpfDock;
using Mesurement.UiLayer.ViewModel;
using System.Windows;
using Mesurement.UiLayer.ViewModel.FrameLog;
using System;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Mesurement.UiLayer.WPF.Model;
using System.Collections.Generic;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewStd.xaml 的交互逻辑
    /// </summary>
    public partial class ViewLiveMeterFrame
    {
        public ViewLiveMeterFrame()
        {
            InitializeComponent();
            Name = "电能表报文记录";
            DockStyle.Position = eDockSide.Bottom;
            gridFrames.DataContext = EquipmentData.CheckResults;
            LiveMeterFrame.EventNewFrame += LiveMeterFrame_EventNewFrame;
        }

        void LiveMeterFrame_EventNewFrame(object sender, System.EventArgs e)
        {
            Dispatcher.Invoke(new Action(()=>
                {
                    ViewModel.Model.AsyncObservableCollection<DynamicViewModel> frames = EquipmentData.CheckResults.CheckNodeCurrent.LiveFrames.DisplayModels;
                    if (frames.Count > 0)
                    {
                        dataGrid.ScrollIntoView(frames[frames.Count - 1]);
                    }
                }));
        }
        #region 复制报文内容
        /// <summary>
        /// 复制报文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string copyText = GetCopyText();
            if (!string.IsNullOrEmpty(copyText))
            {
                Clipboard.SetDataObject(copyText);
            }
        }
        /// <summary>
        /// 获取复制文本
        /// </summary>
        /// <returns></returns>
        private string GetCopyText()
        {
            StringBuilder sbCopy = new StringBuilder();
            var cells = dataGrid.SelectedCells;
            if (cells == null) { return null; }
            foreach (DataGridCellInfo cell in cells)
            {
                FrameworkElement contentTemp = cell.Column.GetCellContent(cell.Item);
                if (contentTemp != null)
                {
                    List<TextBlock> textBlockList = Utils.FindChildren<TextBlock>(contentTemp);
                    if (textBlockList != null)
                    {
                        foreach (TextBlock tb in textBlockList)
                        {
                            sbCopy.AppendLine(tb.Text);
                        }
                    }
                }
            }
            return sbCopy.ToString();
        }
        #endregion
    }
}
