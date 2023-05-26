namespace CLDC_DeviceDriver.Drivers
{
    /// <summary>
    /// 台体 工作流状态
    /// </summary>
    public enum WorkFlow
    {
        None,
        Unknow,
        预热,
        启动,
        潜动,
        对色标,
        基本误差,
        走字,
        需量周期误差,
        多功能,
        耐压

    }
}
