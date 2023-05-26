namespace Mesurement.UiLayer.ViewModel.PortConfig
{
    /// <summary>
    /// Rs485端口列表
    /// </summary>
    public class RS485Item : DeviceItem
    {
        public RS485Item()
        {
            ComParam = "2400,e,8,1";
        }

        private string name="RS485";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string paraNo = "01020";
        /// <summary>
        /// 编码
        /// </summary>
        public string ParaNo
        {
            get { return paraNo; }
            set { SetPropertyValue(value, ref paraNo, "ParaNo"); }
        }

        private string portCount = "24";
        /// <summary>
        /// 端口数量
        /// </summary>
        public string PortCount
        {
            get { return portCount; }
            set { SetPropertyValue(value, ref portCount, "PortCount"); }
        }
        private string intervalValue = "1";
        /// <summary>
        /// 端口间隔
        /// </summary>
        public string IntervalValue
        {
            get { return intervalValue; }
            set { SetPropertyValue(value, ref intervalValue, "IntervalValue"); }
        }
        private string startPort = "1";

        public string StartPort
        {
            get { return startPort; }
            set { SetPropertyValue(value, ref startPort, "StartPort"); }
        }

        //private string dllType = "DotNet平台开发";

        //public string DllType
        //{
        //    get { return dllType; }
        //    set { SetPropertyValue(value, ref dllType, "DllType"); }
        //}

        public override string ToString()
        {
            string stringTemp = string.Format("{6}|{0}|{1}|{2}|{7}|{3}|{4}|{5}|{8}", PortCount, StartPort, IntervalValue, Server.Address, MaxTimePerFrame, MaxTimePerByte, Name, ComParam, DllType);
            return stringTemp;
        }
    }
}
