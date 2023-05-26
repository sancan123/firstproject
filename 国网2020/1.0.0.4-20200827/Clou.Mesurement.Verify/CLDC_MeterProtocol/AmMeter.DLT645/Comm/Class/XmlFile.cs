
/* ----------------------------------------------------------------
 * Copyright (C) 2010 科陆电子电测事业部
 * 文件名：
 * 文件功能描述：
 * 创建标识：
 * 修改标识：
 * 修改描述：
 * ---------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Class
{
    /// <summary>
    /// Xml文件操作类
    /// </summary>
    public class XmlFile
    {
        private string _FilePath; //文件路径
        private XmlDocument _XmlDoc; //Xml文档

        public XmlFile(string filePath)
        {
            this._FilePath = filePath;
            this._XmlDoc = new XmlDocument();
        }

        #region ---- 公共属性 ----
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; }
        }
        #endregion

        #region ---- 辅助函数 ----



        #endregion

        /// <summary>
        /// 加载文件
        /// </summary>
        public void Load()
        {
            try
            {
                _XmlDoc.Load(GetPhyPath(this._FilePath));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region ---- 功能函数 ----
        /// <summary>
        /// 获取子节点个数
        /// </summary>
        /// <param name="str_XPath">节点表达式</param>
        /// <param name="str_ChildName">子节点名称</param>
        /// <returns>子节点个数</returns>
        public int GetChildCount(string str_XPath, string str_ChildName)
        {
            try
            {
                int s4_ChildCount = 0;
                XmlNode tagNode = _XmlDoc.SelectSingleNode(str_XPath);
                for (int i = 0; i < tagNode.ChildNodes.Count; i++)
                {
                    if (tagNode.ChildNodes[i].Name.Equals(str_ChildName))
                        s4_ChildCount++;
                }
                return s4_ChildCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取节点属性值
        /// </summary>
        /// <param name="str_XPath">节点表达式</param>
        /// <param name="str_AttributeName">属性名称</param>
        /// <returns>属性值</returns>
        public string GetAttributeValue(string str_XPath, string str_AttributeName)
        {
            try
            {
                XmlNode tagNode = _XmlDoc.SelectSingleNode(str_XPath);
                return ((XmlElement)tagNode).GetAttribute(str_AttributeName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取节点文本值
        /// </summary>
        /// <param name="str_XPath">节点表达式</param>
        /// <returns>文本值</returns>
        public string GetNodeText(string str_XPath)
        {
            try
            {
                XmlNode tagNode = _XmlDoc.SelectSingleNode(str_XPath);
                return tagNode.InnerText;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion ---- 功能函数 ----

        /// <summary>
        /// 根据相对路径获取文件、文件夹绝对路径
        /// </summary>
        /// <param name="filePath">相对路径</param>
        /// <returns>绝对路径</returns>
        private string GetPhyPath(string filePath)
        {
            filePath = filePath.Replace('/', '\\');
            if (filePath.IndexOf(':') != -1)
                return filePath; //已是绝对路径
            if (filePath.Length > 0 && filePath[0] == '\\')
                filePath = filePath.Substring(1);
            return AppDomain.CurrentDomain.BaseDirectory + String.Intern("\\") + filePath;
        }
    }
}
