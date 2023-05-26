using System.Drawing;

namespace CLDC_DeviceDriver.Setting
{
    /// <summary>
    /// 节点相关信息
    /// </summary>
    public abstract class ItemInfor
    {
        /// <summary>
        /// 伪主键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 主键值
        /// </summary>
        public string KeyValue { get; set; }
        private string _Name = "";
        /// <summary>
        /// 项的名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value.Trim('\0'); }
        }
        /// <summary>
        /// 基本参数
        /// </summary>
        public string SettingBase { get; set; }
        /// <summary>
        /// 扩展参数
        /// </summary>
        public string SettingExpand { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 节点颜色
        /// </summary>
        public Color ColorItem { get; set; }
        /// <summary>
        /// 是否存在
        /// </summary>
        public bool Exist { get; set; }

        /// <summary>
        /// 保存信息
        /// </summary>
        //public abstract void SaveItem(string filePath);

        /// <summary>
        /// 从文件更新节点信息
        /// </summary>
        //public abstract void RefreshItem(string filePath);

        /// <summary>
        /// 从字符串获取颜色
        /// </summary>
        /// <param name="stringColor">颜色的字符串</param>
        /// <returns>颜色</returns>
        protected Color GetColor(string stringColor)
        {
            try
            {  
                Color color = Color.FromName(stringColor);
                if (color.GetBrightness() == 0)
                {
                    color = ColorTranslator.FromHtml("#"+stringColor);
                }
                return color;
            }
            catch
            {
                return Color.White;
            }
        }

        /// <summary>
        /// 从字符串获取bool量
        /// </summary>
        /// <param name="stringBool"></param>
        /// <returns></returns>
        protected bool GetBool(string stringBool)
        {
            bool result;
            bool.TryParse(stringBool, out result);
            return result;
        }

        /// <summary>
        /// 获取单项的内容
        /// </summary>
        /// <param name="stringSection">组名称</param>
        /// <param name="nodeName">项名称</param>
        /// <returns>获取的值</returns>
        protected string ReadSingle(string filePath, string stringSection, string nodeName)
        {
            return CLDC_DataCore.Function.File.ReadInIString(CLDC_DataCore.Function.File.GetPhyPath(filePath), stringSection, nodeName, "");
        }

        /// <summary>
        /// 写入单项
        /// </summary>
        /// <param name="stringSection">组名称</param>
        /// <param name="nodeName">项名称</param>
        protected void WriteSingle(string filePath,string stringSection, string nodeName,string stringValue)
        {
            CLDC_DataCore.Function.File.WriteInIString(CLDC_DataCore.Function.File.GetPhyPath(filePath), stringSection, nodeName, stringValue);
        }
    }
}
