using Mesurement.UiLayer.Utility.Log;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// <summary>
    /// Interaction logic for WindowEditEnum.xaml
    /// </summary>
    public partial class WindowEditEnum
    {
        public WindowEditEnum(string enumCode)
        {
            InitializeComponent();
            codeType = enumCode;
            dictionary = DAL.CodeDictionary.GetLayer2(enumCode);
            List<EnumUnitTemp> listTemp = new List<EnumUnitTemp>();
            for (int i = 0; i < dictionary.Count; i++)
            {
                listTemp.Add(new EnumUnitTemp { textValue = dictionary.Keys.ElementAt(i), textName = dictionary.Values.ElementAt(i) });
            }
            listBox.ItemsSource = listTemp;
        }
        private string codeType = "";
        Dictionary<string, string> dictionary = null;

        public class EnumUnitTemp
        {
            public string textValue { get; set; }
            public string textName { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                switch (button.Content.ToString())
                {
                    case "确认":
                        AddNewValue();
                        break;
                    case "取消":
                        Close();
                        break;
                }
            }
        }

        private void AddNewValue()
        {
            string textValue = textNewValue.Text;
            string textName = textNewName.Text;
            if (string.IsNullOrEmpty(textValue) || string.IsNullOrEmpty(textName))
            {
                LogManager.AddMessage("值不能为空!", EnumLogSource.用户操作日志, EnumLevel.Warning);
                return;
            }
            if(dictionary.ContainsKey(textValue) || dictionary.ContainsValue(textName))
            {
                LogManager.AddMessage("值已经存在!!", EnumLogSource.用户操作日志, EnumLevel.Warning);
                return;
            }
            //DynamicModel model = new DynamicModel();
            //model.SetProperty("ENUM_NAME", textName);
            //model.SetProperty("ENUM_VALUE", textValue);
            //model.SetProperty("ENUM_TYPE", codeType);
            //DALManager.ApplicationDbDal.Insert(EnumAppDbTable.CODE_DETAIL.ToString(), model);
            //DialogResult = true;
            Close();
        }
    }
}
