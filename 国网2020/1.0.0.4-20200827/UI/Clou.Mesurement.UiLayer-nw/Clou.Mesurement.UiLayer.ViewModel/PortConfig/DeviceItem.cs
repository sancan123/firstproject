namespace Mesurement.UiLayer.ViewModel.PortConfig
{
    /// <summary>
    /// 设备端口类
    /// </summary>
    public class DeviceItem : ViewModelBase
    {
        private string id = "0";
        /// <summary>
        /// 存储在数据库中的编号
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private ServerItem server;
        /// <summary>
        /// 服务器名
        /// </summary>
        public ServerItem Server
        {
            get { return server; }
            set { SetPropertyValue(value, ref server, "Server"); }
        }
        private string portNum = "1";

        public string PortNum
        {
            get { return portNum; }
            set { SetPropertyValue(value, ref portNum, "PortNum"); }
        }

        private string comParam = "38400,n,8,1";

        public string ComParam
        {
            get { return comParam; }
            set { SetPropertyValue(value, ref comParam, "ComParam"); }
        }

        private string maxTimePerByte = "10";
        /// <summary>
        /// 字节最大时间间隔(ms)
        /// </summary>
        public string MaxTimePerByte
        {
            get { return maxTimePerByte; }
            set { SetPropertyValue(value, ref maxTimePerByte, "MaxTimePerByte"); }
        }
        private string maxTimePerFrame = "3000";
        /// <summary>
        /// 帧最大时间间隔(ms)
        /// </summary>
        public string MaxTimePerFrame
        {
            get { return maxTimePerFrame; }
            set { SetPropertyValue(value, ref maxTimePerFrame, "MaxTimePerFrame"); }
        }

        private bool flagChanged;
        /// <summary>
        /// 修改标记
        /// </summary>
        public bool FlagChanged
        {
            get { return flagChanged; }
            set { SetPropertyValue(value, ref flagChanged, "FlagChanged"); }
        }

        private string deviceName ="aa";

        /// <summary>
        /// 设备驱动名
        /// </summary>
        public string DeviceName
        {
            get { return deviceName; }
            set { SetPropertyValue(value, ref deviceName, "DeviceName"); }
        }


        private string className = "aa";

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName
        {
            get { return className; }
            set { SetPropertyValue(value, ref className, "ClassName"); }
        }

        private string dllType;

        /// <summary>
        /// 动态库类型
        /// </summary>
        public string DllType
        {
            get { return dllType; }
            set { SetPropertyValue(value, ref dllType, "DllType"); }
        }


        protected internal override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != "FlagChanged")
            {
                FlagChanged = true;
            }
        }

    }
}
