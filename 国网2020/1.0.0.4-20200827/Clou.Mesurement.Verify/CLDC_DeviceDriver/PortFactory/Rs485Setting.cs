using CLDC_DeviceDriver.Drivers.Clou;

namespace CLDC_DeviceDriver.PortFactory
{
    /// 485端口配置
    /// <summary>
    /// 485端口配置
    /// </summary>
    public class Rs485Setting
    {
        /// 获取485端口配置
        /// <summary>
        /// 获取485端口配置
        /// </summary>
        /// <returns></returns>
        public static DeviceParaUnit[] GetRs485Ports()
        {
            return DeviceFactory.Instance.GetRs485Params();
        }
        /// 获取读卡器端口配置
        /// <summary>
        /// 获取读卡器端口配置
        /// </summary>
        /// <returns></returns>
        public static DeviceParaUnit[] GetRsCardPorts()
        {
            return DeviceFactory.Instance.GetRsCardParams();
        }
    }
}
