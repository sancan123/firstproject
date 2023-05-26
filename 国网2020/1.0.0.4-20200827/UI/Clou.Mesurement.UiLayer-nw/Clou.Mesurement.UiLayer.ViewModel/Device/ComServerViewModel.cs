namespace Mesurement.UiLayer.ViewModel.Device
{
    /// 串口服务器视图模型
    /// <summary>
    /// 串口服务器视图模型
    /// </summary>
    public class ComServerViewModel : ViewModelBase
    {
        private int index;
        /// <summary>
        /// 串口服务器序号,从0开始
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                SetPropertyValue(value, ref index, "Index");
                OnPropertyChanged("Name");
            }
        }
        public string Name
        {
            get { return ServerAddress; }
        }

        private string serverAddress;
        /// <summary>
        /// 对应于串口服务器的地址
        /// </summary>
        public string ServerAddress
        {
            get { return serverAddress; }
            set
            {
                SetPropertyValue(value, ref serverAddress, "ServerAddress");
                OnPropertyChanged("Name");
            }
        }
        /// 设备构造函数
        /// <summary>
        /// 设备构造函数
        /// </summary>
        /// <param name="configString"></param>
        public ComServerViewModel()
        {
            Units.Clear();
            for (int i = 0; i < 64; i++)
            {
                Units.Add(new PortUnit
                {
                    Index = i + 1,
                });
            }
        }

        private Model.AsyncObservableCollection<PortUnit> units = new Model.AsyncObservableCollection<PortUnit>();
        /// 串口服务器端口列表
        /// <summary>
        /// 串口服务器端口列表
        /// </summary>
        public Model.AsyncObservableCollection<PortUnit> Units
        {
            get { return units; }
            set { SetPropertyValue(value, ref units, "Units"); }
        }
    }

    /// 端口单元
    /// <summary>
    /// 端口单元
    /// </summary>
    public class PortUnit : ViewModelBase
    {
        public bool IsUsed
        { get { return !string.IsNullOrEmpty(Name); } }

        private string name;
        /// <summary>
        /// 设备名
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                SetPropertyValue(value, ref name, "Name");
                OnPropertyChanged("DisplayName");
                OnPropertyChanged("IsUsed");
            }
        }

        private int index;
        /// 对应于2018的端口号
        /// <summary>
        /// 对应于2018的端口号
        /// </summary>
        public int Index
        {
            get { return index; }
            set { SetPropertyValue(value, ref index, "Index"); }
        }
        private bool newSend;
        /// 发出新数据
        /// <summary>
        /// 发出新数据
        /// </summary>
        public bool NewSend
        {
            get { return newSend; }
            set
            {
                SetPropertyValue(value, ref newSend, "NewSend");
            }
        }

        private bool newReceived;
        /// 收到新数据
        /// <summary>
        /// 收到新数据
        /// </summary>
        public bool NewReceived
        {
            get { return newReceived; }
            set
            {
                SetPropertyValue(value, ref newReceived, "NewReceived");
            }
        }
        /// 显示名称
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return "";
                }
                else
                {
                    return string.Format("{0}-{1}", Name, DeviceIndex + 1);
                }
            }
        }
        private int deviceIndex;
        /// 设备序号
        /// <summary>
        /// 设备序号
        /// </summary>
        public int DeviceIndex
        {
            get { return deviceIndex; }
            set { SetPropertyValue(value, ref deviceIndex, "DeviceIndex"); }
        }

    }
}