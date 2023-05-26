using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    [Serializable()]
    public class UploadModel : DataModel
    {
        /// <summary>
        /// 执行语句字典
        /// </summary>
        public Dictionary<string, List<string>> DataDic { get; set; }
        /// <summary>
        /// 返回结果字典
        /// </summary>
        public Dictionary<string, string> RlstDic { get; set; }
        /// <summary>
        /// webservice接口字典
        /// </summary>
        public Dictionary<string, List<WebPara>> WebDic { get; set; }        

    }
}
