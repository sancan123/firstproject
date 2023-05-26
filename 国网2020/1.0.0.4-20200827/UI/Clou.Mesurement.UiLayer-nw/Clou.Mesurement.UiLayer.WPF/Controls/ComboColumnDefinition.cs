using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Clou.Mesurement.UiLayer.DAL;
using System.Windows.Controls;
using System.Windows.Data;

namespace Clou.Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// 下拉选项为编码的对应列表
    /// </summary>
    public class DataGridColumnCode : DataGridComboBoxColumn
    {
        /// <summary>
        /// 允许添加
        /// </summary>
        public bool AllowAdd
        {
            get { return (bool)GetValue(AllowAddProperty); }
            set { SetValue(AllowAddProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowAdd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowAddProperty =
            DependencyProperty.Register("AllowAdd", typeof(bool), typeof(DataGridColumnCode), new PropertyMetadata(false));

        public string EnumType
        {
            get { return (string)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnumType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(string), typeof(DataGridColumnCode), new PropertyMetadata(""));

        public void RefreshDropDownList()
        {
            LoadDropDownList();
        }

        private void LoadDropDownList()
        {
            if (string.IsNullOrEmpty(EnumType))
            {
                return;
            }
            Dictionary<string,string> dictionary = CodeDictionary.GetLayer2(EnumType);
            object[] array = new object[dictionary.Count];
            for (int i = 0; i < dictionary.Count; i++)
            {
                array[i] = dictionary.Keys.ElementAt(i);
            }

            if (array.Length > 0)
            {
                SelectorEditorSettings settings = new SelectorEditorSettings(array);
                settings.IsInCellEditingEnabled = false;
                EditorSettings = settings;
                if(menu.Items.Count<=0)
                {
                    menu.Items.Add("添加新的值");
                    ContextMenu = menu;
                    menu.AddHandler(System.Windows.Controls.MenuItem.ClickEvent,new RoutedEventHandler(MenuItem_Click));
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            WindowEditEnum windowEditEnum = new WindowEditEnum(EnumType);
            bool? dialogResult = windowEditEnum.ShowDialog();
            if(dialogResult.HasValue && dialogResult.Value)
            {
                LoadDropDownList();
            }
        }

        public override BindingBase SelectedItemBinding
        {
            get
            {
                return base.SelectedItemBinding;
            }

            set
            {
                base.SelectedItemBinding = value;
            }
        }
        {
            LoadDropDownList();
            base.OnInitialized(e);
        }
    }
}
