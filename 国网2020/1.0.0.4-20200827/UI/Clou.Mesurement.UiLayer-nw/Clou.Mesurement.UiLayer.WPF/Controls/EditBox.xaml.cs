using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for EditBox.xaml
    /// </summary>
    public partial class EditBox : UserControl
    {
        public EditBox()
        {
            InitializeComponent();
            gridRoot.DataContext = this;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(EditBox), new PropertyMetadata(""));

        private void textBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            textBlock.Visibility = Visibility.Collapsed;
            textBox.Visibility = Visibility.Visible;
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            textBlock.Visibility = Visibility.Visible;
            textBox.Visibility = Visibility.Collapsed;
        }
    }
}
