using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// ArrowLineWithText.xaml 的交互逻辑
    /// </summary>
    public partial class ArrowLineWithText : UserControl
    {
        public ArrowLineWithText()
        {
            InitializeComponent();
            canvasTop.DataContext = this;
            RefreshArrow();
            Binding binding = new Binding("BrushArrow");
            binding.Source = this;
            textBlock.SetBinding(TextBlock.ForegroundProperty, binding);
        }

        /// 幅值
        /// <summary>
        /// 幅值
        /// </summary>
        public float Length
        {
            get 
            {
                return (float)GetValue(LengthProperty);
            }
            set { SetValue(LengthProperty, value); }
        }



        public float LenthRating
        {
            get { return (float)GetValue(LenthRatingProperty); }
            set { SetValue(LenthRatingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LenthRating.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LenthRatingProperty =
            DependencyProperty.Register("LenthRating", typeof(float), typeof(ArrowLineWithText), new PropertyMetadata(100F));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(float), typeof(ArrowLineWithText), new PropertyMetadata(0F));

        /// 相位
        /// <summary>
        /// 相位
        /// </summary>
        public float Phase
        {
            get { return (float)GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register("Phase", typeof(float), typeof(ArrowLineWithText), new PropertyMetadata(0F));

        /// 向量名
        /// <summary>
        /// 向量名
        /// </summary>
        public string NameVector
        {
            get { return (string)GetValue(NameVectorProperty); }
            set { SetValue(NameVectorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NameVectorProperty =
            DependencyProperty.Register("NameVector", typeof(string), typeof(ArrowLineWithText), new PropertyMetadata(string.Empty));

        /// 箭头的颜色
        /// <summary>
        /// 箭头的颜色
        /// </summary>
        public Brush BrushArrow
        {
            get { return (Brush)GetValue(BrushArrowProperty); }
            set { SetValue(BrushArrowProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty BrushArrowProperty =
            DependencyProperty.Register("BrushArrow", typeof(Brush), typeof(ArrowLineWithText), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// 更新箭头的显示
        /// <summary>
        /// 更新箭头的显示
        /// </summary>
        private void RefreshArrow()
        {
            #region 更新箭头显示
            arrowLine.X1 = 0;
            arrowLine.Y1 = 0;
            //额定电流长度为总长度的1/1.2
            double arrowX = Length * Math.Cos((Phase - 270) * Math.PI / 180) * 120 / LenthRating;
            arrowLine.X2 = arrowX;
            double arrowY = Length * Math.Sin((Phase - 90) * Math.PI / 180) * 120 / LenthRating;
            arrowLine.Y2 = arrowY;
            #endregion
            #region 更新文本位置
            if (Length < 0.1)
            {
                textBlock.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
            double textWidth = 16;
            double textHeight = 15;
            if (arrowX > 0)
            {
                Canvas.SetLeft(textBlock, arrowX + 3);
            }
            else
            {
                Canvas.SetLeft(textBlock, arrowX - textWidth - 3);
            }
            if (arrowY > 0)
            {
                Canvas.SetTop(textBlock, arrowY + 3);
            }
            else
            {
                Canvas.SetTop(textBlock, arrowY - textHeight - 3);
            }
            string unitString = "";
            if (NameVector.ToLower().Contains("u"))
            {
                unitString = "V";
            }
            else if (NameVector.ToLower().Contains("i"))
            { 
                unitString = "A";
            }
            textBlock.Text = string.Format("{0:F2}{1}", Length,unitString);
            #endregion
        }

        /// 当属性更改时设置箭头
        /// <summary>
        /// 当属性更改时设置箭头
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "LenthRating" || e.Property.Name == "Length" || e.Property.Name == "Phase")
            {
                RefreshArrow();
            }
            base.OnPropertyChanged(e);
        }
    }
}
