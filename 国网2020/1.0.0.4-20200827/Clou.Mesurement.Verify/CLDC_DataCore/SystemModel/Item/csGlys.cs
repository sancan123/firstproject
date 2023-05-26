using System.Collections.Generic;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;


namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 功率因素字典
    /// </summary>
    public class csGlys
    {
        /// <summary>
        /// 功率因素字典
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _GlysDic;
        
        /// <summary>
        /// 功率因素ID对照字典
        /// </summary>
        private Dictionary<string, string> _GlysTable;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public csGlys()
        {
            _GlysDic = new Dictionary<string, Dictionary<string, string>>();
            _GlysTable = new Dictionary<string, string>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~csGlys()
        {
            _GlysDic = null;
            _GlysTable = null;
        }
        /// <summary>
        /// 获取功率因素ID值
        /// </summary>
        /// <param name="Glys">功率因素名称</param>
        /// <returns></returns>
        public string getGlysID(string Glys)
        {
            foreach (string Key in _GlysTable.Keys)
            {
                if (Key.ToLower() == Glys.ToLower())
                {
                    return _GlysTable[Key];
                }

            }
            return "";
        }


    }


}
