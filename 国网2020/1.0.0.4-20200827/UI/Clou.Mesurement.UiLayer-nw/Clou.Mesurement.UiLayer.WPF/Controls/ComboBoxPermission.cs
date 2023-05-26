using Mesurement.UiLayer.ViewModel.CodeTree;
using System;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// 用户权限下拉框
    /// </summary>
    public class ComboBoxPermission :ComboBox
    {
        public ComboBoxPermission() : base()
        {
            ItemsSource = Enum.GetValues(typeof(EnumPermission));
        }
    }
}
