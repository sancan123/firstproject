using System.Collections.Generic;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 
    /// </summary>
    public class csDgnDic
    {
        /// <summary>
        /// 多功能配置字典
        /// </summary>
        private Dictionary<string, Struct.StDgnConfig> _DgnConfig;
        /// <summary>
        /// 构造函数
        /// </summary>
        public csDgnDic()
        {
            _DgnConfig = new Dictionary<string, Struct.StDgnConfig>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~csDgnDic()
        {
            _DgnConfig = null;
        }

 
        /// <summary>
        /// 添加和修改多功能配置信息
        /// </summary>
        /// <param name="DgnInfo">多功能配置信息结构体</param>
        public void Add(Struct.StDgnConfig DgnInfo)
        {
            if (_DgnConfig.ContainsKey(DgnInfo.DgnPrjID))
            {
                this.Remove(DgnInfo.DgnPrjID);
            }
            _DgnConfig.Add(DgnInfo.DgnPrjID, DgnInfo);
            return;
        }
        /// <summary>
        /// 移除一个多功能配置项目
        /// </summary>
        /// <param name="PrjID"></param>
        public void Remove(string PrjID)
        {
            if (!_DgnConfig.ContainsKey(PrjID))
                return;
            _DgnConfig.Remove(PrjID);
            return;
        }
        /// <summary>
        /// 获取多功能项目信息列表
        /// </summary>
        /// <returns></returns>
        public List<Struct.StDgnConfig> getDgnPrj()
        {
            List<Struct.StDgnConfig> _Dgn = new List<Struct.StDgnConfig>();
            foreach (string _ID in _DgnConfig.Keys)
            {
                _Dgn.Add(_DgnConfig[_ID]);
            }
            return _Dgn;
        }
        /// <summary>
        /// 获取一个项目信息
        /// </summary>
        /// <param name="PrjID"></param>
        /// <returns></returns>
        public Struct.StDgnConfig getDgnPrj(string PrjID)
        {
            if (!_DgnConfig.ContainsKey(PrjID))
                return new Struct.StDgnConfig();
            return _DgnConfig[PrjID];
        }

    }
}
