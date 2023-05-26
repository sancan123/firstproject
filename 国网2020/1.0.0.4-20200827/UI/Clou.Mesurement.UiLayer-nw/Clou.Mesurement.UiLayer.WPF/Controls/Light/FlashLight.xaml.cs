using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Mesurement.UiLayer.WPF.Controls.Light
{
    /// 有颜色的灯
    /// <summary>
    /// 有颜色的灯
    /// </summary>
    public partial class FlashLight : UserControl
    {
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// <summary>
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// </summary>
        public FlashLight()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Flash")
            {
                if (Flash)
                {
                    Storyboard storyBoard = Resources["storyBoard"] as Storyboard;
                    if (storyBoard != null)
                    {
                        storyBoard.Begin();
                    }
                }
                Flash = false;
            }
            base.OnPropertyChanged(e);
        }

        public bool Flash
        {
            get { return (bool)GetValue(FlashProperty); }
            set { SetValue(FlashProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Flash.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlashProperty =
            DependencyProperty.Register("Flash", typeof(bool), typeof(FlashLight), new PropertyMetadata(false));
    }
}
