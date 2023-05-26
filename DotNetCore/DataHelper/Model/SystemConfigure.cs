using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public class SystemConfigure : SettingBase
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemConfigure()
            : base(App.AppPath + @"\System\system.ini")
        {
        }

        #region 基本设置
        /// <summary>
        /// 代理模式
        /// </summary>
        [Category("基本设置"), DisplayName("代理模式"), Description("代理模式，正向代理或者反向代理"), DefaultValue(0)]
        public Agency AgencyModel { get; set; }

        /// <summary>
        /// 发送目录
        /// </summary>
        [Category("基本设置"), DisplayName("发送目录"), Description("隔离装置发送端文件目录"), DefaultValue("")]
        public string SendDirectory { get; set; }

        /// <summary>
        /// 接收目录 
        /// </summary>
        [Category("基本设置"), DisplayName("接收目录"), Description("隔离装置接收端文件目录"), DefaultValue("")]
        public string ReceiveDirectory { get; set; }

        /// <summary>
        /// 搜索间隔 
        /// </summary>
        [Category("基本设置"), DisplayName("搜索间隔"), Description("搜索接收文件间隔，单位毫秒（ms）"), DefaultValue(10)]
        public int SearchInterval { get; set; }

        /// <summary>
        /// 等待搜索时间 
        /// </summary>
        [Category("基本设置"), DisplayName("等待搜索时间"), Description("等待搜索接收文件的时间，超时判定未收到不再继续搜索。单位秒（s）"), DefaultValue(180)]
        public int WaitingForSearchTime { get; set; }

        /// <summary>
        /// 最大显示消息数量 
        /// </summary>
        [Category("基本设置"), DisplayName("最大显示消息数量"), Description("网格中最多显示的消息数量，超过设定值则清除第一条"), DefaultValue(10)]
        public int MaxMsgCount { get; set; }

        /// <summary>
        /// 是否打印日志
        /// </summary>
        [Category("基本设置"), DisplayName("是否打印日志"), Description("是否打印日志"), DefaultValue(false)]
        public bool PrintLog { get; set; }

        /// <summary>
        /// 是否备份文件
        /// </summary>
        [Category("基本设置"), DisplayName("是否备份文件"), Description("是否备份文件"), DefaultValue(false)]
        public bool BackupFile { get; set; }

        /// <summary>
        /// 定期清理日志间隔 
        /// </summary>
        [Category("基本设置"), DisplayName("定期清理日志间隔"), Description("定期清理日志的间隔, 单位（天）"), DefaultValue(10)]
        public int DeleteLogInterval { get; set; }
        #endregion

        #region 代理服务
        /// <summary>
        /// 是否开启代理服务
        /// </summary>
        [Category("代理服务"), DisplayName("是否开启代理服务"), Description("True:正式运行；False：测试运行。测试运行时正向不开网站，反向不调用平台接口，请先配置好代理服务网站信息再开启服务！"), DefaultValue(false)]
        public bool IsOpenProxy { get; set; }

        /// <summary>
        /// 代理服务IP地址
        /// </summary>
        [Category("代理服务"), DisplayName("代理服务IP地址"), Description("代理服务器的IP地址，本机可以不填，或者填127.0.0.1"), DefaultValue("127.0.0.1")]
        public string HostIPOfWebSite { get; set; }

        /// <summary>
        /// 代理服务端口号
        /// </summary>
        [Category("代理服务"), DisplayName("代理服务端口号"), Description("代理服务端口号"), DefaultValue("12313")]
        public string PortNumOfWebSite { get; set; }

        /// <summary>
        /// 代理服务网站名称 
        /// </summary>
        [Category("代理服务"), DisplayName("代理服务网站名称"), Description("代理服务网站名称"), DefaultValue("ForwardProxy")]
        public string CommentOfWebSite { get; set; }

        /// <summary>
        /// 代理服务网站应用程序池 
        /// </summary>
        [Category("代理服务"), DisplayName("代理服务网站应用程序池"), Description("应用程序池，请设置和网站名称一致，以免多个网站使用默认连接池发生错乱"), DefaultValue("ForwardProxy")]
        public string AppPoolIdOfWebSite { get; set; }
        #endregion
    }
}
