using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace Mesurement.UiLayer.WPF.UiGeneral
{
    public class PreviewMoveBehavior : Behavior<UIElement>
    {
        private Canvas canvas;

        private bool isDragging = false;

        private Point mouseOffset;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;

            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;

            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_MouseRightButtonUp;

            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;

            AssociatedObject.PreviewMouseMove -= AssociatedObject_MouseMove;

            AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_MouseRightButtonUp;

            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
        }

        void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomMeterContainer(1.2);
            }
            else if (e.Delta < 0)
            {
                ZoomMeterContainer(1 / 1.2);
            }
        }
        /// 缩放
        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="zoomParameter"></param>
        private void ZoomMeterContainer(double zoomParameter)
        {
            Point positionMouse = Mouse.GetPosition(AssociatedObject);
            AssociatedObject.SetValue(UserControl.WidthProperty, AssociatedObject.RenderSize.Width * zoomParameter);
            Canvas.SetTop(AssociatedObject, Canvas.GetTop(AssociatedObject) + positionMouse.Y * (1 - zoomParameter));
            Canvas.SetLeft(AssociatedObject, Canvas.GetLeft(AssociatedObject) + positionMouse.X * (1 - zoomParameter));
        }

        void AssociatedObject_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                AssociatedObject.ReleaseMouseCapture();

                isDragging = false;
            }
        }

        void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point point = e.GetPosition(canvas);

                AssociatedObject.SetValue(Canvas.TopProperty, point.Y - mouseOffset.Y);

                AssociatedObject.SetValue(Canvas.LeftProperty, point.X - mouseOffset.X);
            }
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                return;
            }

            if (canvas == null)
            {
                canvas = VisualTreeHelper.GetParent(AssociatedObject) as Canvas;
            }

            isDragging = true;

            mouseOffset = e.GetPosition(AssociatedObject);

            AssociatedObject.CaptureMouse();
        }
    }
}
