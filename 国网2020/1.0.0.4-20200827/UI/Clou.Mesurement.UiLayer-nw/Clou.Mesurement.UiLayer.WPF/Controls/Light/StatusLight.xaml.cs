using System.Windows.Controls;
using System.Windows.Media;

namespace Mesurement.UiLayer.WPF.Controls.Light
{
    /// 状态指示灯
    /// 共三种状态，灭，正常，故障
    /// <summary>
    /// 状态指示灯
    /// 共三种状态，灭，正常，故障
    /// </summary>
    public partial class StatusLight : UserControl
    {
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// <summary>
        /// xaml里面添加了一个storyboard，通过shine属性的变化来控制亮和灭
        /// </summary>
        public StatusLight()
        {
            InitializeComponent();
        }
        private bool? status = null;
        /// 灯的状态，null为灭；false为红；true为绿
        /// <summary>
        /// 灯的状态，null为灭；false为红；true为绿
        /// </summary>
        public bool? Status
        {
            get { return status; }
            set 
            {
                status=value;
                if (status == null)
                {
                    ellipse.Fill = new SolidColorBrush(Color.FromRgb(0xDC, 0xDC, 0xDC));
                }
                else if (status == true)
                {
                    ellipse.Fill = new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    ellipse.Fill = new SolidColorBrush(Colors.Red);
                }
            }
        }
    }
}
