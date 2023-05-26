﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using Mesurement.UiLayer.DAL;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Mesurement.UiLayer.Utility.Log;


namespace Mesurement.UiLayer.ViewModel
{
    /// 动态属性类
    /// <summary>
    /// 动态属性类
    /// </summary>
    public class DynamicViewModel : DynamicObject, INotifyPropertyChanged, IDisposable
    {

        private int index;
        /// 序号
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                if (value != index)
                {
                    index = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Index"));
                    }
                }
            }
        }

        ///// 存储在后台的数据源
        ///// <summary>
        ///// 存储在后台的数据源
        ///// </summary>
        //private DynamicModel dataSource = new DynamicModel();
        /// 存储在后台的数据源
        /// <summary>
        /// 存储在后台的数据源
        /// </summary>
        private ObservableCollection<PairUnit> dataSource = new ObservableCollection<PairUnit>();
        public DynamicViewModel(int index)
        {
            Index = index;
        }
        /// 初始化动态属性,不要有相同的名字
        /// <summary>
        /// 初始化动态属性,不要有相同的名字
        /// </summary>
        /// <param name="listPropertyName"></param>
        /// <param name="index"></param>
        public DynamicViewModel(List<string> listPropertyName, int index)
        {
            Index = index;
            for (int i = 0; i < listPropertyName.Count; i++)
            {
                SetProperty(listPropertyName[i], null);
            }
        }
        public DynamicViewModel(DynamicModel model, int index)
        {
            Index = index;
            dataSource = new ObservableCollection<PairUnit>();
            List<string> propertyNames = model.GetAllProperyName();
            for (int i = 0; i < propertyNames.Count; i++)
            {
                dataSource.Add(new PairUnit
                {
                    Key = propertyNames[i],
                    Value = model.GetProperty(propertyNames[i])
                });
            }
        }
        /// 重写Set方法
        /// <summary>
        /// 重写Set方法
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder.Name == "写入内容")
            {
                bool isNuber = true;
                string strValue = value.ToString();
                for (int i = 0; i < strValue.Length; i++)
                {
                    if (!Char.IsNumber(strValue, i))
                    {
                        isNuber = false;
                        break;
                    }
                }
                if(!isNuber)
                System.Windows.MessageBox.Show("输入格式有误，请输入数字。");
                
            }

            SetProperty(binder.Name, value);
            if (value == null || !value.Equals(GetProperty(binder.Name)))
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(binder.Name));
                }
            }
            return true;
        }
        public void SetProperty(string propertyName, object value)
        {
            var unit = dataSource.ToList().Find(item => item.Key == propertyName);
            if (unit == null)
            {
                dataSource.Add(new PairUnit { Key = propertyName, Value = value });
            }
            else
            {
                unit.Value = value;
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public object GetProperty(string propertyName)
        {
            try
            {
                var unit = dataSource.ToList().FirstOrDefault(item => item.Key == propertyName);
                if (unit == null)
                {
                    return null;
                }
                else
                {
                    return unit.Value;
                }
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("获取数据异常,这个异常往往是线程不同步引起的:{0}", e.Message), EnumLogSource.用户操作日志);
                return null;
            }
        }
        /// 重写get方法
        /// <summary>
        /// 重写get方法
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetProperty(binder.Name);
            return true;
        }
        /// 获取所有属性
        /// <summary>
        /// 获取所有属性
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllProperyName()
        {
            List<string> list = new List<string>();
            var temp = from item in dataSource select item.Key;
            if (temp != null)
            {
                list = temp.ToList();
            }
            return list;
        }
        /// 获取视图的数据源
        /// <summary>
        /// 获取视图的数据源
        /// </summary>
        public DynamicModel GetDataSource()
        {
            DynamicModel model = new DynamicModel();
            for (int i = 0; i < dataSource.Count; i++)
            {
                model.SetProperty(dataSource[i].Key, dataSource[i].Value);
            }
            return model;
        }
        public void RemoveProperty(string propertyName)
        {
            var units = dataSource.Where(item => item.Key == propertyName);
            if (units != null)
            {
                dataSource.Remove(units.ElementAt(0));
            }
        }
        #region
        /// 属性变化事件
        /// <summary>
        /// 属性变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public void Dispose()
        {
            if (PropertyChanged != null)
            {
                Delegate[] ds = PropertyChanged.GetInvocationList();
                foreach (Delegate d in PropertyChanged.GetInvocationList())
                {
                    PropertyChangedEventHandler pd = d as PropertyChangedEventHandler;
                    if (pd != null)
                    {
                        PropertyChanged -= pd;
                    }
                }
            }
            GC.SuppressFinalize(this);
        }
    }
    /// 一对值
    /// <summary>
    /// 一对值
    /// </summary>
    public class PairUnit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// 属性更改事件
        /// <summary>
        /// 属性更改事件
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private string key;
        /// 关键字
        /// <summary>
        /// 关键字
        /// </summary>
        public string Key
        {
            get { return key; }
            set { key = value; OnPropertyChanged("Key"); }
        }
        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("Value"); }
        }

    }
}
