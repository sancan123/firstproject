using System.Collections.Generic;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 系统字典
    /// </summary>
    public class csDictionary
    {
        /// <summary>
        /// 字典集合
        /// </summary>
        private Dictionary<string, List<string>> _ZiDian;
        /// <summary>
        /// 构造函数，初始化字典集合
        /// </summary>
        public csDictionary()
        {
            _ZiDian = new Dictionary<string, List<string>>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~csDictionary()
        {
            _ZiDian = null;
        }

        /// <summary>
        /// 增加一个字典属性值
        /// </summary>
        /// <param name="ZiDianName">字典名称</param>
        /// <param name="sValue">属性值</param>
        public void Add(string ZiDianName, string sValue)
        {
            if (!_ZiDian.ContainsKey(ZiDianName))
                return;
            if (_ZiDian[ZiDianName].Contains(sValue))
                return;
            _ZiDian[ZiDianName].Add(sValue);

        }

        /// <summary>
        /// 移除一个属性值
        /// </summary>
        /// <param name="ZiDianName">字典名称</param>
        /// <param name="RemoveValue">需要移除的属性值</param>
        public void Remove(string ZiDianName, string RemoveValue)
        {
            if (!_ZiDian.ContainsKey(ZiDianName) || !_ZiDian[ZiDianName].Contains(RemoveValue))
                return;
            _ZiDian[ZiDianName].Remove(RemoveValue);
        }


        /// <summary>
        /// 移除一个字典的所有属性值
        /// </summary>
        /// <param name="ZiDianName">字典名称</param>
        public void Remove(string ZiDianName)
        {
            if (!_ZiDian.ContainsKey(ZiDianName))
                return;
            _ZiDian[ZiDianName].Clear();
            return;
        }



        /// <summary>
        /// 获取字典名称
        /// </summary>
        /// <returns>返回字典名称集合</returns>
        public List<string> getZiDianName()
        {
            List<string> _Name = new List<string>();

            foreach (string _k in _ZiDian.Keys)
            {
                _Name.Add(_k);
            }
            return _Name;
        }

        /// <summary>
        /// 获取字典对应值集合
        /// </summary>
        /// <param name="ZiDianName">字典名称</param>
        /// <returns></returns>
        public List<string> getValues(string ZiDianName)
        {
            if (!_ZiDian.ContainsKey(ZiDianName))
                return new List<string>();
            return _ZiDian[ZiDianName];
        }
    }
}
