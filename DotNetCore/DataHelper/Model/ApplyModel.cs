using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    [Serializable()]
    public class ApplyModel : DataModel
    {
        /// <summary>
        /// web接口参数
        /// </summary>
        public WebPara webPara { get; set; }

    }
}
