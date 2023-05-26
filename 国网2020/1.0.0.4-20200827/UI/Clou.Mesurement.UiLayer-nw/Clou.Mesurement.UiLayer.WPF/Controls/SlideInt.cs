using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    public class SlideInt:Slider
    {


        public int ValueInt
        {
            get { return (int)GetValue(ValueIntProperty); }
            set { SetValue(ValueIntProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueInt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueIntProperty =
            DependencyProperty.Register("ValueInt", typeof(int), typeof(SlideInt), new PropertyMetadata(0));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Value")
            {
                ValueInt = (int)Value;
            }
            base.OnPropertyChanged(e);
        }
    }
}
