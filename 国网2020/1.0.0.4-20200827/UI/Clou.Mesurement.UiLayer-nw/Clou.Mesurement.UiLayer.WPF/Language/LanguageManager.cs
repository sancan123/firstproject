using System;
using System.Collections.Generic;
using System.Windows;
using Mesurement.UiLayer.DAL;
namespace Mesurement.UiLayer.WPF.Language
{
    public class LanguageManager
    {
        static LanguageManager()
        {
            CurrentLanguage = "汉语";
        }
        private static Dictionary<string, string> dictionaryLanguage = new Dictionary<string, string>();
        /// <summary>
        /// 当前语言
        /// </summary>
        public static string CurrentLanguage { get; set; }
        /// <summary>
        /// 加载语言包
        /// </summary>
        /// <param name="languageName"></param>
        public static void LoadLanguage(string languageName)
        {
            string fileName = CodeDictionary.GetValueLayer2("LanguageList", languageName);
            ResourceDictionary resouce = new ResourceDictionary();
            resouce.Source = new Uri(string.Format(@"{0}\Language\{1}.xaml", System.IO.Directory.GetCurrentDirectory(), fileName), UriKind.Absolute);
            foreach(string key in resouce.Keys)
            {
                if (Application.Current.Resources.Contains(key))
                {
                    Application.Current.Resources[key] = resouce[key];
                }
                else
                {
                    Application.Current.Resources.Add(key, resouce[key]);
                }
            }
        }
    }
}
