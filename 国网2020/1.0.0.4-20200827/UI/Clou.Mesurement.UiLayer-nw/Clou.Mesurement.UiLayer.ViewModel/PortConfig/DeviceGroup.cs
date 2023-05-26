using Mesurement.UiLayer.ViewModel.Model;

namespace Mesurement.UiLayer.ViewModel.PortConfig
{
    public class DeviceGroup : ViewModelBase
    {
        private bool isExist;
        /// <summary>
        /// 端口是否存在
        /// </summary>
        public bool IsExist
        {
            get { return isExist; }
            set { SetPropertyValue(value, ref isExist, "IsExist"); }
        }
        private string name;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string paraNo;
        /// <summary>
        /// 编号
        /// </summary>
        public string ParaNo
        {
            get { return paraNo; }
            set { SetPropertyValue(value, ref paraNo, "ParaNo"); }
        }
        private AsyncObservableCollection<DeviceItem> deviceItems = new AsyncObservableCollection<DeviceItem>();
        /// <summary>
        /// 设备列表
        /// </summary>
        public AsyncObservableCollection<DeviceItem> DeviceItems
        {
            get { return deviceItems; }
            set { SetPropertyValue(value, ref deviceItems, "DeviceItems"); }
        }
    }
}
