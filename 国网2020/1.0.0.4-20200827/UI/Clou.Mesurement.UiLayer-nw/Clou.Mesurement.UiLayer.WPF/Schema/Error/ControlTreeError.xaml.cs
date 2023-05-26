using Mesurement.UiLayer.ViewModel.Schema.Error;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Schema.Error
{
    /// <summary>
    /// ControlTreeError.xaml 的交互逻辑
    /// </summary>
    public partial class ControlTreeError
    {
        public ControlTreeError()
        {
            InitializeComponent();
            DataContext = AllPoints;
            AllPoints.PointsChanged += AllPoints_PointsChanged;
        }

        void AllPoints_PointsChanged(object sender, EventArgs e)
        {
            if (PointsChanged != null)
            {
                PointsChanged(sender, e);
            }
        }


        public AllErrorModel AllPoints
        {
            get { return (AllErrorModel)GetValue(AllPointsProperty); }
            set { SetValue(AllPointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllPoints.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllPointsProperty =
            DependencyProperty.Register("AllPoints", typeof(AllErrorModel), typeof(ControlTreeError), new PropertyMetadata(new AllErrorModel()));

        public event EventHandler PointsChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (button.DataContext is ErrorCategory)
                {
                    ErrorCategory categoryToRemove = button.DataContext as ErrorCategory;
                    while (categoryToRemove.ErrorPoints.Count > 0)
                    {
                        ErrorModel errorPoint = categoryToRemove.ErrorPoints[0];
                        errorPoint.FlagRemove = true;
                        if (PointsChanged != null)
                        {
                            PointsChanged(errorPoint, null);
                        }
                        categoryToRemove.ErrorPoints.Remove(errorPoint);
                    }
                    AllPoints.Categories.Remove(button.DataContext as ErrorCategory);
                }
            }
        }
    }
}
