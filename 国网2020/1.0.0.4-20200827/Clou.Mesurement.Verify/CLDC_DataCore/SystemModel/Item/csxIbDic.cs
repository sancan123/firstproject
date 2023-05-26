using System.Collections.Generic;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 
    /// </summary>
    public class csxIbDic
    {
        /// <summary>
        /// 电流倍数字典集合
        /// </summary>
        private Dictionary<string, string> _xIbDic;
        /// <summary>
        /// 构造函数
        /// </summary>
        public csxIbDic()
        {
            _xIbDic = new Dictionary<string, string>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~csxIbDic()
        {
            _xIbDic = null;
        }


        /// <summary>
        /// 添加新的负载点
        /// <param name="xIbName">负载点名称</param>
        /// </summary>
        public bool Add(string xIbName)
        {
            if (_xIbDic.ContainsKey(xIbName))
                return false;
            string _ID = "";
            for (int _I = 1; _I <= 99; _I++)            //总共99个预留点，
            { 
                _ID=_I.ToString("d2");
                if (!_xIbDic.ContainsValue(_ID))
                    break;
            }
            _xIbDic.Add(xIbName, _ID);
            return true;
        }

        /// <summary>
        /// 移除一个电流负载点
        /// </summary>
        /// <param name="xIbName">电流倍数名称Iamx,1.0Ib,3.0Ib</param>
        public void Remove(string xIbName)
        {
            if (!_xIbDic.ContainsKey(xIbName) || int.Parse(_xIbDic[xIbName])<15)
                return;
            _xIbDic.Remove(xIbName);
            return;
        }

        /// <summary>
        /// 获取电流倍数ID值
        /// </summary>
        /// <param name="xIbName">电流倍数</param>
        /// <returns></returns>
        public string getxIbID(string xIbName)
        {
            foreach (string Key in _xIbDic.Keys)
            {
                if (Key.ToLower() == xIbName.ToLower())
                {
                    return _xIbDic[Key];
                }
            }

            return "";
        }

        /// <summary>
        /// 获取电流倍数名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> getxIb()
        {
            List<string> _xIbs = new List<string>();
            foreach (string _Name in _xIbDic.Keys)
            {
                _xIbs.Add(_Name);
            }
            return _xIbs;
        }

    }
}
