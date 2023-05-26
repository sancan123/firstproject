﻿using Mesurement.UiLayer.WPF.Model;
using Mesurement.UiLayer.WPF.UiGeneral;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Mesurement.UiLayer.WPF.Controls
{
    public class ListBoxDragDrop : ListBox
    {
        public ListBoxDragDrop()
        {
            AllowDrop = true;
        }
        private AdornerLayer mAdornerLayer;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (treeItemTemp == null)
                {
                    mAdornerLayer = null;
                    return;
                }

                DragDropAdorner adorner = new DragDropAdorner(treeItemTemp);
                mAdornerLayer = AdornerLayer.GetAdornerLayer(this); // Window class do not have AdornerLayer
                mAdornerLayer.Add(adorner);
                // Window class do not have AdornerLayer

                DragDrop.DoDragDrop(this, element.DataContext, DragDropEffects.Move);
                if (mAdornerLayer == null)
                {
                    return;
                }
                mAdornerLayer.Remove(adorner);
                mAdornerLayer = null;
            }
            base.OnMouseMove(e);
        }
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (mAdornerLayer == null)
            {
                return;
            }
            mAdornerLayer.Update();

            Win32.POINT point = new Win32.POINT();
            if (Win32.GetCursorPos(ref point))
            {
                Point pos = PointFromScreen(new Point(point.X, point.Y));
                HitTestResult result = VisualTreeHelper.HitTest(this, pos);
                if (result != null)
                {
                    ListBoxItem treeViewItem = Utils.FindVisualParent<ListBoxItem>(result.VisualHit); // Find your actual visual you want to drag
                    if (treeViewItem != null)
                    {
                        treeViewItem.IsSelected = true;
                    }
                }
            }

            base.OnQueryContinueDrag(e);
        }
        private ListBoxItem treeItemTemp = null;
        /// <summary>
        /// 鼠标左键选中要拖动的项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(this);
            HitTestResult result = VisualTreeHelper.HitTest(this, pos);
            if (result == null)
            {
                return;
            }
            treeItemTemp = Utils.FindVisualParent<ListBoxItem>(result.VisualHit); // Find your actual visual you want to drag
            base.OnPreviewMouseLeftButtonDown(e);
        }
        /// <summary>
        /// 鼠标弹起时释放要拖动的项
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            treeItemTemp = null;
            base.OnMouseLeftButtonUp(e);
        }
    }
}
