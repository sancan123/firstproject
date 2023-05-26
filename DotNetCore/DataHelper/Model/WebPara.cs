using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    [Serializable()]
    public class WebPara
    {
        /// <summary>
        /// webserviceURL
        /// </summary>
        public string webserviceURL { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string interfaceName { get; set; }
        /// <summary>
        /// 输入参数
        /// </summary>
        public string inputPara { get; set; }
        /// <summary>
        /// 返回参数
        /// </summary>
        public string returnPara { get; set; }
    }
}
