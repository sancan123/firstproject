using System.Windows;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewAbout.xaml
    /// </summary>
    public partial class ViewClientConfig
    {
        public ViewClientConfig()
        {
            InitializeComponent();
            DockStyle.IsFloating = true;
            DockStyle.FloatingSize = new Size(500, 350);
            DockStyle.ResizeMode = ResizeMode.NoResize;
            Name = "软件版本";
        }
    }
}
