using Clou.Mesurement.UiLayer.Utility.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace Clou.Mesurement.UiLayer.ViewModel.Model
{
    /// 适用于多线程的ObservableCollection
    /// ObservableCollection不能多线程操作的原因是OnpropertyChanged和OnCollectionChanged是在本线程执行的操作
    /// 通过SynchronizationContext.Post方法来将多线程执行的更改通知汇聚到一起
    /// 通过重写OnCollectionChanged来实现
    /// <summary>
    /// 适用于多线程的ObservableCollection
    /// ObservableCollection不能多线程操作的原因是OnpropertyChanged和OnCollectionChanged是在本线程执行的操作
    /// 通过SynchronizationContext.Post方法来将多线程执行的更改通知汇聚到一起
    /// 通过重写OnCollectionChanged来实现
    /// </summary>
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private object lockObj = new object();
        public AsyncObservableCollection()
        { }
        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        { }
        /// 重写集合更改事件
        /// <summary>
        /// 重写集合更改事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            //如果发生在同一线程,直接执行基类方法;如果不在同一线程通过Synchronization来调用基类方法
            if (SynchronizationContext.Current == EquipmentData.UiDispatcher)
            {
                base.OnCollectionChanged(e);
            }
            else
            {
                EquipmentData.UiDispatcher.Post(obj =>
                    {
                        try
                        {
                            base.OnCollectionChanged(e);
                        }
                        catch (Exception ex)
                        {
                            LogManager.AddMessage(string.Format("列表数据操作失败:{0}", ex.Message), EnumLogSource.用户操作日志, EnumLevel.Warning, ex);
                        }
                    }, e);
            }
        }

        protected override void ClearItems()
        {
            if (SynchronizationContext.Current == EquipmentData.UiDispatcher)
            {
                lock (lockObj)
                {
                    base.ClearItems();
                }
            }
            else
            {
                EquipmentData.UiDispatcher.Post(obj =>
                {
                    lock (lockObj)
                    {
                        base.ClearItems();
                    }
                }, null);
            }
        }
        protected override void InsertItem(int index, T item)
        {
            if (SynchronizationContext.Current == EquipmentData.UiDispatcher)
            {
                //if (index <= Count && index >= 0)
                //{
                lock (lockObj)
                {
                    base.InsertItem(index, item);
                }
                //}
            }
            else
            {
                EquipmentData.UiDispatcher.Post(obj =>
                {
                    try
                    {
                        lock (lockObj)
                        {
                            //if (index <= Count && index >= 0)
                            //{
                            base.InsertItem(index, item);
                            //}
                        }
                    }
                    catch
                    { }
                }, null);
            }
        }
        protected override void RemoveItem(int index)
        {
            if (SynchronizationContext.Current == EquipmentData.UiDispatcher)
            {
                lock (lockObj)
                {
                    base.RemoveItem(index);
                }
            }
            else
            {
                EquipmentData.UiDispatcher.Post(obj =>
                {
                    try
                    {
                        lock (lockObj)
                        {
                            if (Count > index)
                            {
                                base.RemoveItem(index);
                            }
                        }
                    }
                    catch
                    {
                    }
                }, null);
            }
        }

        /// 重写属性更改事件
        /// <summary>
        /// 重写属性更改事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                //如果发生在同一线程,直接执行基类方法;如果不在同一线程通过Synchronization来调用基类方法
                if (SynchronizationContext.Current == EquipmentData.UiDispatcher)
                {
                    base.OnPropertyChanged(e);
                }
                else
                {
                    EquipmentData.UiDispatcher.Post(obj => base.OnPropertyChanged(obj as PropertyChangedEventArgs), e);
                }
            }
            catch (Exception ex)
            {
                LogManager.AddMessage(string.Format("列表数据操作失败:{0}", ex.Message), EnumLogSource.用户操作日志, EnumLevel.Warning, ex);
            }
        }
        /// 内部排序方法
        /// <summary>
        /// 内部排序方法
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            var sortedItems = Items.OrderBy(keySelector);
            for (int i = 0; i < sortedItems.Count(); i++)
            {
                var item = sortedItems.ElementAt(i);
                Move(IndexOf(item), i);
            }
        }
    }
}