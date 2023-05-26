using System.Collections.Generic;
using CLDC_DataCore.Struct;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{

    public class csDataFlag
    {
        #region--------------私有变量-----------------
        private Dictionary<string, StDataFlagInfo> m_Dic_DataFlagInfo;
        #endregion------------------------------------

        #region--------------构造函数-----------------
        /// <summary>
        /// 构造函数
        /// </summary>
        public csDataFlag()
        {
            m_Dic_DataFlagInfo = new Dictionary<string, StDataFlagInfo>();
        }

        /// <summary>
        /// 析构函数 
        /// </summary>
        ~csDataFlag()
        {
            m_Dic_DataFlagInfo = null;
        }

        #endregion------------------------------------

        #region--------------公共函数-----------------


        /// <summary>
        /// 存储载波方案模型数据到XML文档
        /// </summary>
        public void Save()
        {
        }

        /// <summary>
        /// 新增一个数据标识
        /// </summary>
        /// <param name="p_sci_DataFlagInfo">数据标识结构体</param>
        public void Add(StDataFlagInfo p_sci_DataFlagInfo)
        {
            if (p_sci_DataFlagInfo.DataFlagName == "")
            {
                return;
            }
            if (m_Dic_DataFlagInfo.ContainsKey(p_sci_DataFlagInfo.DataFlagName))
            {
                m_Dic_DataFlagInfo[p_sci_DataFlagInfo.DataFlagName] = p_sci_DataFlagInfo;

            }
            else
            {
                m_Dic_DataFlagInfo.Add(p_sci_DataFlagInfo.DataFlagName, p_sci_DataFlagInfo);
            }
            this.Save();        //新增完毕保存XML文档

        }

        /// <summary>
        /// 检测数据标识是否存在
        /// </summary>
        /// <param name="p_str_DataFlagName">数据标识名</param>
        /// <returns></returns>
        public bool FindDataFlagInfo(string p_str_DataFlagName)
        {
            return m_Dic_DataFlagInfo.ContainsKey(p_str_DataFlagName);
        }

        /// <summary>
        /// 移除一个数据标识
        /// </summary>
        /// <param name="p_str_DataFlagName">数据标识名</param>
        public void Remove(string p_str_DataFlagName)
        {
            if (!m_Dic_DataFlagInfo.ContainsKey(p_str_DataFlagName))
                return;
            m_Dic_DataFlagInfo.Remove(p_str_DataFlagName);
            this.Save();
        }

        /// <summary>
        /// 获取数据项
        /// </summary>
        /// <param name="p_str_DataFlagName">数据项名称</param>
        /// <returns></returns>
        public int GetDataFlagNo(string p_str_DataFlagName)
        {
            StDataFlagInfo DataFlagInfo = new StDataFlagInfo();
            int iNo = 0;
            foreach (string _name in m_Dic_DataFlagInfo.Keys)
            {
                
                DataFlagInfo = m_Dic_DataFlagInfo[_name];
                if (DataFlagInfo.DataFlagName == p_str_DataFlagName)
                {
                    
                    break;
                }
                iNo++;
            }
            return iNo;
        }

        /// <summary>
        /// 获取数据项
        /// </summary>
        /// <param name="p_str_DataFlagName">数据项名称</param>
        /// <returns></returns>
        public StDataFlagInfo GetDataFlagInfo(string p_str_DataFlagName)
        {
            StDataFlagInfo DataFlagInfo = new StDataFlagInfo();
            foreach (string _name in m_Dic_DataFlagInfo.Keys)
            {
                DataFlagInfo = m_Dic_DataFlagInfo[_name];
                if (DataFlagInfo.DataFlagName == p_str_DataFlagName)
                    break;
            }
            return DataFlagInfo;
        }
        /// <summary>
        /// 获取所有数据标识列表
        /// </summary>
        /// <returns>返回List</returns>
        public List<StDataFlagInfo> GetDataFlagList()
        {
            List<StDataFlagInfo> lst_stDataFlagInfo = new List<StDataFlagInfo>();
            foreach (string _name in m_Dic_DataFlagInfo.Keys)
            {
                StDataFlagInfo stc_tmp = m_Dic_DataFlagInfo[_name];
                lst_stDataFlagInfo.Add(stc_tmp);
            }
            return lst_stDataFlagInfo;
        }
        /// <summary>
        /// 获取数据标识名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetDataFlagNameList()
        {
            List<string> lst_stDataFlagInfo = new List<string>();
            foreach (string _name in m_Dic_DataFlagInfo.Keys)
            {
                StDataFlagInfo stc_tmp = m_Dic_DataFlagInfo[_name];
                lst_stDataFlagInfo.Add(stc_tmp.DataFlagName);
            }
            return lst_stDataFlagInfo;
        }
        #endregion------------------------------------
    }
}
