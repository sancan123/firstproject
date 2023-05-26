﻿using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Mesurement.UiLayer.WPF.Skin
{
    public class SkinViewModel : ViewModelBase
    {
        private static SkinViewModel instance = null;
        public static SkinViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SkinViewModel();
                }
                return instance;
            }
        }
        /// <summary>
        /// 加载皮肤资源
        /// </summary>
        public SkinViewModel()
        {
            //默认主题颜色
            //themes.Add(new ThemeItem());
            string[] fileNames = Directory.GetFiles(string.Format(@"{0}\Skin", Directory.GetCurrentDirectory()));
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i].EndsWith(".xaml"))
                {
                    int indexTemp = fileNames[i].LastIndexOf('\\');
                    if (indexTemp >= 0)
                    {
                        string nameTemp = fileNames[i].Substring(indexTemp + 1);
                        nameTemp = nameTemp.Replace(".xaml", "");
                        try
                        {
                            themes.Add(new ThemeItem(nameTemp));
                        }
                        catch (Exception e)
                        {
                            LogManager.AddMessage(string.Format("加载外观样式文件:{0}失败:{1}", fileNames[i], e.Message), EnumLogSource.用户操作日志, EnumLevel.Warning);
                        }
                    }
                }
            }
        }
        private ObservableCollection<ThemeItem> themes = new ObservableCollection<ThemeItem>();

        public ObservableCollection<ThemeItem> Themes
        {
            get { return themes; }
            set { SetPropertyValue(value, ref themes, "Themes"); }
        }
    }
}
