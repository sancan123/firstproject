using Mesurement.UiLayer.DAL.Config;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.Const;

namespace Mesurement.UiLayer.ViewModel.Device
{
    /// 仪表设备数据基类
    /// <summary>
    /// 仪表设备数据基类
    /// </summary>
    public class DevicePortClass : ViewModelBase
    {
        private bool status;
        /// <summary>
        /// 联机状态
        /// </summary>
        public bool Status
        {
            get { return status; }
            set { SetPropertyValue(value, ref status, "Status"); }
        }

        /// 设备构造函数
        /// <summary>
        /// 设备构造函数
        /// </summary>
        /// <param name="configString"></param>
        public DevicePortClass(string configString)
        {
            string[] arrayTemp = configString.Split('|');
            string errorMessage = "";
            if (arrayTemp.Length >= 8)
            {
                Name = arrayTemp[0];
                string temp = arrayTemp[1];
                string[] arrayAddress = temp.Split('_');
                if (temp.ToLower() == "com")
                {
                    Address = "COM";
                }
                else
                {
                    if (arrayAddress.Length == 3)
                    {
                        Address = arrayAddress[0];
                        StartPort = arrayAddress[2];
                        RemotePort = arrayAddress[1];
                    }
                    else
                    {
                        errorMessage = string.Format("设备IP地址及端口配置错误:{0}", temp);
                    }
                }
                PortNo = arrayTemp[2];
                DriverName = arrayTemp[3];
                ClassName = arrayTemp[4];
                Baudrate = arrayTemp[5];
                MaxTimePerFrame = arrayTemp[6];
                MaxTimePerByte = arrayTemp[7];

                if (arrayTemp.Length == 9)
                {
                    DllType = arrayTemp[8];
                    if (Name == "南网设备统一接口")
                    {
                        GlobalUnit.DeviceDllType = dllType;
                    }
                    else if (Name == "南网读写卡器统一接口")
                    {
                        GlobalUnit.CardDllType = dllType;
                    }
                }
            }
            else
            {
                errorMessage = string.Format("设备端口配置信息长度错误:{0}", configString);
            }
            if (errorMessage != "")
            {
                LogManager.AddMessage(errorMessage, EnumLogSource.设备操作日志, EnumLevel.Error);
            }
        }

        private EnumConfigId deviceType;
        /// <summary>
        /// 设备对应的配置类型
        /// </summary>
        public EnumConfigId DeviceType
        {
            get
            {
                return deviceType;
            }
            set
            {
                SetPropertyValue(value, ref deviceType, "DeviceType");
            }
        }

        //设备名|数量|序号|IP或“COM”|起始端口|远程端口|端口号|驱动文件全名|完整类名|串口参数|帧数据最大间隔|bit延时
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
            }
        }

        private int count;
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return count; }
            set { SetPropertyValue(value, ref count, "Count"); }
        }

        private int index;
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get { return index; }
            set
            {
                SetPropertyValue(value, ref index, "Index");
                #region 设置2018端口
                if (!string.IsNullOrEmpty(Name))
                {
                    int temp = 0;
                    int.TryParse(PortNo, out temp);
                    ComServerViewModel server = ServersViewModel.Instance.Register(Address,RemotePort,StartPort);
                    if (temp > 0 &&server!=null && temp <= server.Units.Count)
                    {
                        server.Units[temp - 1].DeviceIndex = Index;
                        server.Units[temp - 1].Name = Name;
                    }
                }
                #endregion
            }
        }

        private string address;
        /// <summary>
        /// IP或“COM”
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            {
                SetPropertyValue(value, ref address, "Address");
            }
        }
        private string startPort;
        /// <summary>
        /// 起始端口
        /// </summary>
        public string StartPort
        {
            get { return startPort; }
            set { SetPropertyValue(value, ref startPort, "StartPort"); }
        }
        private string remotePort;

        public string RemotePort
        {
            get { return remotePort; }
            set { SetPropertyValue(value, ref remotePort, "RemotePort"); }
        }

        private string portNo;
        /// <summary>
        /// 端口号
        /// </summary>
        public string PortNo
        {
            get { return portNo; }
            set { SetPropertyValue(value, ref portNo, "PortNo"); }
        }

        private string driverName;
        /// <summary>
        /// 驱动文件全名
        /// </summary>
        public string DriverName
        {
            get { return driverName; }
            set { SetPropertyValue(value, ref driverName, "DriverName"); }
        }

        private string className;
        /// <summary>
        /// 完整类名
        /// </summary>
        public string ClassName
        {
            get { return className; }
            set { SetPropertyValue(value, ref className, "ClassName"); }
        }
        private string baudrate;
        /// 串口波特率
        /// <summary>
        /// 串口波特率
        /// </summary>
        public string Baudrate
        {
            get { return baudrate; }
            set { SetPropertyValue(value, ref baudrate, "Baudrate"); }
        }
        private string maxTimePerFrame;
        /// 帧间隔最大延时
        /// <summary>
        /// 帧间隔最大延时
        /// </summary>
        public string MaxTimePerFrame
        {
            get { return maxTimePerFrame; }
            set { SetPropertyValue(value, ref maxTimePerFrame, "MaxTimePerFrame"); }
        }
        private string maxTimePerByte;
        /// 字节最大间隔
        /// <summary>
        /// 字节最大间隔
        /// </summary>
        public string MaxTimePerByte
        {
            get { return maxTimePerByte; }
            set { SetPropertyValue(value, ref maxTimePerByte, "MaxTimePerByte"); }
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

        /// <summary>
        /// 设备参数
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", Name, Address, StartPort, RemotePort, PortNo, DriverName, ClassName, Baudrate, MaxTimePerFrame, MaxTimePerByte, DllType);
        }

        public bool Link()
        {
            return true;
        }
    }
}