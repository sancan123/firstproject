using System.Collections.Generic;
using CLDC_DataCore.Struct;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{

    public class csDataFlag
    {
        #region--------------˽�б���-----------------
        private Dictionary<string, StDataFlagInfo> m_Dic_DataFlagInfo;
        #endregion------------------------------------

        #region--------------���캯��-----------------
        /// <summary>
        /// ���캯��
        /// </summary>
        public csDataFlag()
        {
            m_Dic_DataFlagInfo = new Dictionary<string, StDataFlagInfo>();
        }

        /// <summary>
        /// �������� 
        /// </summary>
        ~csDataFlag()
        {
            m_Dic_DataFlagInfo = null;
        }

        #endregion------------------------------------

        #region--------------��������-----------------


        /// <summary>
        /// �洢�ز�����ģ�����ݵ�XML�ĵ�
        /// </summary>
        public void Save()
        {
        }

        /// <summary>
        /// ����һ�����ݱ�ʶ
        /// </summary>
        /// <param name="p_sci_DataFlagInfo">���ݱ�ʶ�ṹ��</param>
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
            this.Save();        //������ϱ���XML�ĵ�

        }

        /// <summary>
        /// ������ݱ�ʶ�Ƿ����
        /// </summary>
        /// <param name="p_str_DataFlagName">���ݱ�ʶ��</param>
        /// <returns></returns>
        public bool FindDataFlagInfo(string p_str_DataFlagName)
        {
            return m_Dic_DataFlagInfo.ContainsKey(p_str_DataFlagName);
        }

        /// <summary>
        /// �Ƴ�һ�����ݱ�ʶ
        /// </summary>
        /// <param name="p_str_DataFlagName">���ݱ�ʶ��</param>
        public void Remove(string p_str_DataFlagName)
        {
            if (!m_Dic_DataFlagInfo.ContainsKey(p_str_DataFlagName))
                return;
            m_Dic_DataFlagInfo.Remove(p_str_DataFlagName);
            this.Save();
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="p_str_DataFlagName">����������</param>
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
        /// ��ȡ������
        /// </summary>
        /// <param name="p_str_DataFlagName">����������</param>
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
        /// ��ȡ�������ݱ�ʶ�б�
        /// </summary>
        /// <returns>����List</returns>
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
        /// ��ȡ���ݱ�ʶ�����б�
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
