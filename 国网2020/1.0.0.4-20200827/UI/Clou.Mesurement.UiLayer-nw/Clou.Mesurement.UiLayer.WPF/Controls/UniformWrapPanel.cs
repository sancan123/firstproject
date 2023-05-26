using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// 具有指定列的WrapPanel
    /// </summary>
    public class UniformWrapPanel : WrapPanel
    {
        /// 有多少列
        /// <summary>
        /// 有多少列
        /// </summary>
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        /// 列数发生变化时自动设置item的宽度
        /// <summary>
        /// 列数发生变化时自动设置item的宽度
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "ColumnCount")
            {
                ItemWidth = RenderSize.Width / ColumnCount;
            }
            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(UniformWrapPanel), new PropertyMetadata(10));

        /// 重写排列控件方法
        /// <summary>
        /// 重写排列控件方法
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if(finalSize.Width>SystemParameters.WorkArea.Width || finalSize.Height>SystemParameters.WorkArea.Height)
            {
                return new Size(1000, 600);
            }
            ItemWidth = finalSize.Width / ColumnCount;
            return base.ArrangeOverride(finalSize);
        }
    }
}