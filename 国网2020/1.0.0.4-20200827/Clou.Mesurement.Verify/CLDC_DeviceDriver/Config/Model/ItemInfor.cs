using System.Collections.Generic;

namespace CLDC_DeviceDriver.Config.Model
{
    /// <summary>
    /// 节点相关信息
    /// </summary>
    public class ItemInfor
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 节点内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 节点描述
        /// </summary>
        public string Description { get; set; }

        private List<string> dataSource = new List<string>();
        /// <summary>
        /// 用于ComboBox显示时的数据源
        /// </summary>
        public List<string> DataSource
        {
            get
            {
                if (dataSource == null)
                {
                    dataSource = new List<string>();
                }
                return dataSource;
            }
            internal set { dataSource = value; }
        }
    }
}
