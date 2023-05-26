using System.Collections.Generic;
using CLDC_DataCore.DataBase;
using System.Xml;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 功能描述：实验方法与依据
    /// 作    者：lsx 
    /// 编写日期：2014-02-12
    /// 修改记录：
    ///         修改日期		     修改人	            修改内容
    ///
    /// </summary>
    public class MethodAndBasis
    {
        /// <summary>
        /// 实验方法与依据
        /// </summary>
        private Dictionary<string, Struct.StSystemInfo> _MethodAndBasis;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MethodAndBasis()
        {
            _MethodAndBasis = new Dictionary<string, Struct.StSystemInfo>();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~MethodAndBasis()
        {
            _MethodAndBasis = null;
        }



        /// <summary>
        /// 获取关键字列表
        /// </summary>
        /// <returns></returns>
        public List<string> getKeyNames()
        {
            List<string> _Keys = new List<string>();
            foreach (string _name in _MethodAndBasis.Keys)
            {
                _Keys.Add(_name);
            }
            return _Keys;
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            _MethodAndBasis.Clear();
        }

        /// <summary>
        /// 获取实验方法与依据的结构体
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns></returns>
        public Struct.StSystemInfo getItem(string Tkey)
        {
            if (_MethodAndBasis.Count == 0)
                return new Struct.StSystemInfo();
            if (_MethodAndBasis.ContainsKey(Tkey))
                return _MethodAndBasis[Tkey];
            else
                return new Struct.StSystemInfo();
        }

        /// <summary>
        /// 实验方法与依据的个数
        /// </summary>
        public int Count
        {
            get
            {
                return _MethodAndBasis.Count;
            }
        }

        /// <summary>
        /// 添加实验方法与依据的项目
        /// </summary>
        /// <param name="Tkey">实验方法与依据 项目名称</param>
        /// <param name="Item">实验方法与依据 配置值</param>
        public void Add(string Tkey, Struct.StSystemInfo Item)
        {
            if (_MethodAndBasis.ContainsKey(Tkey))
            {
                _MethodAndBasis.Remove(Tkey);
                _MethodAndBasis.Add(Tkey, Item);
            }
            else
                _MethodAndBasis.Add(Tkey, Item);
            return;
        }
    }
}
