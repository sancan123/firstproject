using System;
using System.Collections.Generic;
using CLDC_DataCore.DataBase;
using System.Xml;
using System.Windows.Forms;
using System.Net;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemConfigure
    {
        /// <summary>
        /// 系统设置信息配置
        /// </summary>
        private Dictionary<string, Struct.StSystemInfo> _SystemMode;

        /// <summary>
        /// 
        /// </summary>
        public SystemConfigure()
        {
            _SystemMode = new Dictionary<string, Struct.StSystemInfo>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~SystemConfigure()
        {
            _SystemMode = null;
        }
        /// <summary>
        /// 初始化系统配置信息
        /// </summary>
        public void Load()
        {

        }
        /// <summary>
        /// 读取系统配置项目值
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns>系统项目配置值</returns>
        public string getValue(string Tkey)
        {
            if (_SystemMode.Count == 0)
                return "";
            if (_SystemMode.ContainsKey(Tkey))
                return _SystemMode[Tkey].Value;
            else
                return "";
        }
        /// <summary>
        /// 获取系统配置项目结构体
        /// </summary>
        /// <param name="Tkey">系统项目ID</param>
        /// <returns></returns>
        public Struct.StSystemInfo getItem(string Tkey)
        {
            if (_SystemMode.Count == 0)
                return new Struct.StSystemInfo();
            if (_SystemMode.ContainsKey(Tkey))
                return _SystemMode[Tkey];
            else
                return new Struct.StSystemInfo();
        }

        /// <summary>
        /// 添加系统配置项目
        /// </summary>
        /// <param name="Tkey">系统项目名称</param>
        /// <param name="Item">系统项目配置值</param>
        public void Add(string Tkey, Struct.StSystemInfo Item)
        {
            if (_SystemMode.ContainsKey(Tkey))
            {
                _SystemMode.Remove(Tkey);
                _SystemMode.Add(Tkey, Item);
            }
            else
                _SystemMode.Add(Tkey, Item);
            return;
        }

        /// <summary>
        /// 修改键值
        /// </summary>
        /// <param name="Tkey">关键字</param>
        /// <param name="TValue">修改的值</param>
        public bool EditValue(string Tkey, string TValue)
        {
            if (_SystemMode.ContainsKey(Tkey))
            {
                Struct.StSystemInfo _Item = _SystemMode[Tkey];
                _Item.Value = TValue;
                _SystemMode.Remove(Tkey);
                _SystemMode.Add(Tkey, _Item);
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// 系统配置项目个数
        /// </summary>
        public int Count
        {
            get
            {
                return _SystemMode.Count;
            }
        }

        /// <summary>
        /// 获取关键字列表
        /// </summary>
        /// <returns></returns>
        public List<string> getKeyNames()
        {
            List<string> _Keys = new List<string>();
            foreach (string _name in _SystemMode.Keys)
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
            _SystemMode.Clear();
        }



    }
}
