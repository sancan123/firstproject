using Mesurement.UiLayer.ViewModel.Model;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel.Config
{
    /// 一组配置数据
    /// <summary>
    /// 一组配置数据
    /// </summary>
    public class ConfigGroup:ViewModelBase
    {
        public int ID { get; set; }
        //private string configName;
        ///// <summary>
        ///// 配置名称
        ///// </summary>
        //public string ConfigName
        //{
        //    get { return configName; }
        //    set 
        //    {
        //        if (value != configName)
        //        {
        //            ChangeFlag = true;
        //        }
        //        SetPropertyValue(value, ref configName, "ConfigName");
        //    }
        //}

        public string StringValue
        {
            get 
            {
                var values = from item in Units select item.ConfigValue;
                return string.Join("|",values);
            }
        }

        private AsyncObservableCollection<ConfigUnit> units = new AsyncObservableCollection<ConfigUnit>();
        /// <summary>
        /// 数据源
        /// </summary>
        public AsyncObservableCollection<ConfigUnit> Units
        {
            get { return units; }
            set { units = value; }
        }
        private bool changeFlag;

        public bool ChangeFlag
        {
            get { return changeFlag; }
            set 
            { 
                SetPropertyValue(value, ref changeFlag, "ChangeFlag");
                OnPropertyChanged("StringValue");
            }
        }
        
    }
}
