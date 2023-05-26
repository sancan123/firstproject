using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// 摆放电表的控件
    /// </summary>
    public class MeterContainer : ListBox
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MeterContainer()
        {
            // 在此点下面插入创建对象所需的代码。
        }

        /// 台体有多少列
        /// <summary>
        /// 台体有多少列
        /// </summary>
        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(MeterContainer), new PropertyMetadata(10));
    }
}