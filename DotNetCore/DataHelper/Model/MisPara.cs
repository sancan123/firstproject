using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public class MisPara
    {
        #region 属性
        /// <summary>
        /// 数据库IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 数据库端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 数据库登陆用户
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 数据库登陆密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// WebServer路径
        /// </summary>
        public string WebServiceURL { get; set; }

        /// <summary>
        /// 获取当前联接字符串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return string.Format("Data Source=(DESCRIPTION =(ADDRESS =(PROTOCOL = TCP)(HOST = {0})(PORT = {1}))(CONNECT_DATA = (SERVICE_NAME ={2})));User ID={3};Password={4};Persist Security Info=True",
                        Ip, Port, DataSource, UserId, Password);
            }
        }
        #endregion
    }
}
