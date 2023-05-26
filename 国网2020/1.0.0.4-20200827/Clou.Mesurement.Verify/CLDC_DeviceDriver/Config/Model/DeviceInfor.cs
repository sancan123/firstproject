using System.Collections.Generic;

namespace CLDC_DeviceDriver.Config.Model
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceInfor
    {
        /// <summary>
        /// 设备关键字，用于索引
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 设备是否存在
        /// </summary>
        public bool Exist { get; set; }
        /// <summary>
        /// 设备参数
        /// </summary>
        public List<PortInfor> Ports
        {
            get
            {
                if (ports == null)
                {
                    ports = new List<PortInfor>();
                }
                return ports;
            }
            internal set
            {
                ports = value;
            }
        }
        private List<PortInfor> ports = new List<PortInfor>();
        /// <summary>
        /// 设备型号，CL3115、CL311V2、CL309
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// DLL文件名称，xxx.dll
        /// </summary>
        public string DLLName { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 操作与方法、参数对照表
        /// 第一个string：指定的操作名
        /// 第二个string：方法名,参数格式
        /// </summary>
        public Dictionary<string, string> MethodDic
        {
            get
            {
                if (null == _MethodDic)
                {
                    return new Dictionary<string, string>();
                }
                return _MethodDic;
            }
            set { _MethodDic = value; }
        }
        private Dictionary<string, string> _MethodDic;



    }
}
