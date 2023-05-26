namespace Mesurement.UiLayer.ViewModel.WcfService
{
    /// 监视数据类型
    /// <summary>
    /// 监视数据类型
    /// </summary>
    public enum EnumMonitorType
    {
        Default=0,
        /// 进度条消息
        /// <summary>
        /// 进度条消息
        /// </summary>
        ProgressBar=2,
        /// 标准表信息
        /// <summary>
        /// 标准表信息
        /// </summary>
        MeterStandard=3,
        /// 误差板消息
        /// <summary>
        /// 误差板消息
        /// </summary>
        ErrorBoard=4,
        /// 检定消息
        /// <summary>
        /// 检定消息
        /// </summary>
        CheckMessage=5,
        /// 表上下位状态
        /// <summary>
        /// 表上下位状态
        /// </summary>
        PressStatus=6,
        /// 翻转点击状态
        /// <summary>
        /// 翻转点击状态
        /// </summary>
        ReverseStatus=7,
        /// 表位隔离状态
        /// <summary>
        /// 表位隔离状态
        /// </summary>
        IgnoreStatus=8,
        /// 检定项目编号
        /// <summary>
        /// 检定项目编号
        /// </summary>
        CurrentCheckID=9,
        /// 更换检定方案
        /// <summary>
        /// 更换检定方案
        /// </summary>
        ChangeScheme=10,
        /// 更换表信息
        /// <summary>
        /// 更换表信息
        /// </summary>
        ChangeMeter=11,
        /// 台体版本消息
        /// <summary>
        /// 台体版本消息
        /// </summary>
        EquipmentVersion=12,
        /// 台体检定状态
        /// <summary>
        /// 台体检定状态
        /// </summary>
        CheckStatus=13,
        /// 耐压状态
        /// <summary>
        /// 耐压状态
        /// </summary>
        Insulation = 14,
        /// <summary>
        /// 帧数据消息
        /// </summary>
        Frame=16
    }
}
