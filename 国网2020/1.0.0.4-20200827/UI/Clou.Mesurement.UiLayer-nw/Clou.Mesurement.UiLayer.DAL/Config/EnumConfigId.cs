
namespace Mesurement.UiLayer.DAL.Config
{
    /// 配置编号枚举
    /// <summary>
    /// 配置编号枚举
    /// </summary>
    public enum EnumConfigId
    {
        未知编号配置 = 0,
        #region 仪表编号
        南网设备统一接口 = 1022,
        南网读写卡器统一接口 = 1023,

        RS485=1020,
        #endregion

        #region 台体基本信息
        台体基本信息 = 2001,
        #endregion

        #region 软件配置
        运行环境 = 03001,
        出厂编号配置=03002,
        标准表常数=03003,
        标准表固定常数值=03004,
        通讯方式=03005,
      
        #endregion

        #region 检定相关的服务
        服务地址配置 =05001,
        #endregion

        #region 加密机
        加密机配置=07001,
        #endregion

        #region 检定控制
        设备特殊配置=08001,

        #endregion

        #region 营销接口
        平台数据库配置 =09001,
        WebService配置=09002,
        检定结论服务地址=09003
        #endregion
    }
}
