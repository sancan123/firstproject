using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DataHelper
{
    [Serializable()]
    public class DownModel : DataModel
    {
        /// <summary>
        /// 执行语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 执行语句
        /// </summary>
        public List<string> SqlList { get; set; }



        /// <summary>
        /// 返回结果
        /// </summary>
        public List<Dictionary<string, object>> dateTableToList { get; set; }
    }
}
