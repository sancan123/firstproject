using System.Collections.Generic;
using CLDC_DataCore.DataBase;
using System.Xml;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{

    public class TestSetting
    {
        /// <summary>
        /// 实验参数
        /// </summary>
        private Dictionary<string, Struct.StSystemInfo> _TestSetting;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TestSetting()
        {
            _TestSetting = new Dictionary<string, Struct.StSystemInfo>();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TestSetting()
        {
            _TestSetting = null;
        }

    }
}
