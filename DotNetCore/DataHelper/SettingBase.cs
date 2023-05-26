using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataHelper
{
    /// <summary>
    /// 设置基类
    /// </summary>
    public abstract class SettingBase
    {
        private string _filePath;
        /// <summary>
        /// 设置基类
        /// </summary>
        /// <param name="filePath">配置文件路径</param>
        public SettingBase(string filePath)
        {
            _filePath = filePath;
        }
        /// <summary>
        /// 保存设置数据
        /// </summary>
        public void Save()
        {
            Type t = GetType();
            PropertyInfo[] propertys = t.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                CategoryAttribute[] ca = (CategoryAttribute[])p.GetCustomAttributes(typeof(CategoryAttribute), true);

                App.Funs.IniWrite(_filePath, ca[0].Category, p.Name, p.GetValue(this, null).ToString());
            }

        }

        /// <summary>
        /// 加载设置数据
        /// </summary>
        public void Load()
        {
            Type t = GetType();
            PropertyInfo[] propertys = t.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                DefaultValueAttribute[] dv = (DefaultValueAttribute[])p.GetCustomAttributes(typeof(DefaultValueAttribute), true);
                CategoryAttribute[] ca = (CategoryAttribute[])p.GetCustomAttributes(typeof(CategoryAttribute), true);

                try
                {
                    string val = "";
                    if (p.PropertyType.BaseType == typeof(Enum))
                        val = App.Funs.IniRead(_filePath, ca[0].Category, p.Name, Enum.GetName(p.PropertyType, dv[0].Value));
                    else
                        val = App.Funs.IniRead(_filePath, ca[0].Category, p.Name, dv[0].Value.ToString());

                    if (p.PropertyType == typeof(bool))
                    {
                        p.SetValue(this, (val.ToLower() == "true" ? true : false), null);
                    }
                    else if (p.PropertyType == typeof(Int32))
                    {
                        p.SetValue(this, Convert.ToInt32(val), null);
                    }
                    else if (p.PropertyType.BaseType == typeof(Enum))
                    {
                        p.SetValue(this, Enum.Parse(p.PropertyType, val), null);
                    }
                    else if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(this, Convert.ToSingle(val), null);
                    }
                    else
                    {
                        p.SetValue(this, val, null);
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
