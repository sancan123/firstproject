using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Mesurement.UiLayer.WPF.Controls.Light
{
    /// 有颜色的灯
    /// <summary>
    /// 有颜色的灯
    /// </summary>
    public partial class ColorLight : UserControl
    {
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// <summary>
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// </summary>
        public ColorLight()
        {
            InitializeComponent();
        }


        public Color LightColor
        {
            get { return (Color)GetValue(LightColorProperty); }
            set { SetValue(LightColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LightColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LightColorProperty =
            DependencyProperty.Register("LightColor", typeof(Color), typeof(ColorLight), new PropertyMetadata(Colors.Green));

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Shine")
            {
                if (Shine)
                {
                    brush.Color = LightColor;
                }
                else
                {
                    Storyboard storyBoard = Resources["storyBoard"] as Storyboard;
                    if (storyBoard != null)
                    {
                        storyBoard.Stop();
                    }

                    brush.Color = Colors.LightGray;
                }
            }
            if (e.Property.Name == "Flash")
            {
                if (Flash && Shine)
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

        /// 点亮灯
        /// <summary>
        /// 点亮灯
        /// </summary>
        public bool Shine
        {
            get { return (bool)GetValue(ShineProperty); }
            set
            {
                SetValue(ShineProperty, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShineProperty =
            DependencyProperty.Register("Shine", typeof(bool), typeof(ColorLight), new PropertyMetadata(false));



        public bool Flash
        {
            get { return (bool)GetValue(FlashProperty); }
            set { SetValue(FlashProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Flash.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlashProperty =
            DependencyProperty.Register("Flash", typeof(bool), typeof(ColorLight), new PropertyMetadata(false));
    }
}
