using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Mesurement.UiLayer.ViewModel.Model;
using System.Reflection;
using Mesurement.UiLayer.Utility.Log;
namespace Mesurement.UiLayer.ViewModel
{
    /// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and clean up of resources such as event instanceHandlers. This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged
    {
        #region Constructor / Fields 

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ViewModelBase()
        {
            LocalCommand.CommandAction = (obj) => CommandFactoryMethod(obj as string);
        }
        /// <summary>
        /// Construct with display name.
        /// </summary>
        protected ViewModelBase(string displayName) 
        {
        }

        #endregion // Constructor
        

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
       internal protected virtual void OnPropertyChanged(string propertyName)
        {
            //VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        /// <summary>
        /// Generic method to set a property value with equality 
        /// checking and raising the property changed event.
        /// </summary>
        internal protected bool SetPropertyValue<T>(T value, ref T field, string propertyName)
        {
            if ((value != null && !value.Equals(field)) || (value == null && field != null))
            {
                field = value;
                if (propertyName != null)
                {
                    OnPropertyChanged(propertyName);
                }
                return true;
            }
            return false;
        }

        
        #endregion // INotifyPropertyChanged Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event instanceHandlers.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (PropertyChanged != null)
            {
                Delegate[] ds = PropertyChanged.GetInvocationList();
                foreach (Delegate d in ds)
                {
                    PropertyChangedEventHandler pd = d as PropertyChangedEventHandler;
                    if (pd != null)
                    {
                        PropertyChanged -= pd;
                    }
                }
            }
            LocalCommand.CommandAction = null;
            LocalCommand = null;
        }

        #region 命令相关
        private BasicCommand localCommand;
        /// 控件命令
        /// <summary>
        /// 控件命令
        /// </summary>
        public BasicCommand LocalCommand
        {
            get
            {
                if (localCommand == null)
                {
                    localCommand = new BasicCommand();
                }
                return localCommand;
            }
            set { localCommand = value; }
        }
        /// <summary>
        /// 命令工厂方法
        /// </summary>
        /// <param name="methodName"></param>
        public virtual void CommandFactoryMethod(string methodName)
        {
            try
            {
                //将方法添加到数据库进程去处理
                MethodInfo method = GetType().GetMethod(methodName);
                method.Invoke(this, new object[] { });
            }
            catch (Exception e)
            {
                LogManager.AddMessage(string.Format("调用方法:{0} 出错:{1}", methodName, e.Message), EnumLogSource.用户操作日志, EnumLevel.Error, e);
            }
        }
        #endregion

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        ~ViewModelBase()
        {
        }
        #endif
    }
}
