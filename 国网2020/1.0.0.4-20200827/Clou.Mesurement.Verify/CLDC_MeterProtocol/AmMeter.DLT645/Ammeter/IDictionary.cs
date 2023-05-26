/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: IDictionary.cs
 * 文件功能描述: 数据字典接口
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter
{
    /// <summary>
    /// 数据字典接口
    /// </summary>
    public interface IDictionary
    {
        /// <summary>
        /// 根据标识码搜索数据项
        /// </summary>
        /// <param name="strKey">标识码(DI3-DI2-DI1-DI0)</param>
        /// <returns>如果搜索失败，则返回空列表</returns>
        List<DataInfo> Search(string strKey);
    }

    /// <summary>
    /// 数据项信息
    /// </summary>
    public class DataInfo 
    {
        private int _DataSort;
        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataSort 
        {
            get 
            {
                return _DataSort;
            }
            set 
            {
                _DataSort = value;
            }
        }

        private bool _IsCanRead;
        /// <summary>
        /// 是否可读
        /// </summary>
        public bool IsCanRead 
        {
            get 
            {
                return _IsCanRead;
            }
            set 
            {
                _IsCanRead = value;
            }
        }

        private bool _IsCanWrite;
        /// <summary>
        /// 是否可写
        /// </summary>
        public bool IsCanWrite 
        {
            get 
            { 
                return _IsCanWrite; 
            }
            set 
            { 
                _IsCanWrite = value; 
            }
        }

        private List<StDataField> _DataItems;
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<StDataField> DataItems 
        {
            get 
            {
                return _DataItems;
            }
            set 
            {
                _DataItems = value;
            }
        }

        #region ---- 数据字段信息 ----
        public struct StDataField
        {
            /// <summary>
            /// 数据长度
            /// </summary>
            public int AryLen;
            /// <summary>
            /// 数据信息
            /// </summary>
            public byte[] AryData;
            /// <summary>
            /// 放大倍数
            /// </summary>
            public int Multiple;
        }
        #endregion ---- 数据字段信息 ----

        public DataInfo() 
        {
            _DataSort = 1;
            _IsCanRead = true;
            _IsCanWrite = true;
            _DataItems = new List<StDataField>();
        }
        public DataInfo(int sort)
        {
            _DataSort = sort;
            _IsCanRead = true;
            _IsCanWrite = true;
            _DataItems = new List<StDataField>();
        }

        /// <summary>
        /// 数据大小
        /// </summary>
        public int ArySize
        {
            get
            {
                int s4Len = 0;
                if (DataItems != null)
                {
                    for (int i = 0; i < DataItems.Count; i++)
                    {
                        s4Len += DataItems[i].AryLen;
                    }
                }
                return s4Len;
            }
        }
        /// <summary>
        /// 数据内容
        /// </summary>
        public byte[] AryData
        {
            get
            {
                int s4Cursor = 0; //游标
                byte[] bytData = new byte[ArySize];
                for (int i = 0; i < _DataItems.Count; i++)
                {
                    Array.Copy(_DataItems[i].AryData, 0, bytData, s4Cursor, _DataItems[i].AryLen);
                    s4Cursor++;
                }
                return bytData;
            }
            set 
            {
                int s4Cursor = 0; //游标
                for (int i = 0; i < _DataItems.Count; i++)
                {
                    Array.Copy(value, s4Cursor, _DataItems[i].AryData, 0, _DataItems[i].AryLen);
                    s4Cursor++;
                }
            }
        }

        /// <summary>
        /// 初始化数据项
        /// </summary>
        /// <param name="dataSort">数据类别</param>
        /// <param name="isCanRead">是否可读</param>
        /// <param name="isCanWrite">是否可写</param>
        public void InitParam(int dataSort, bool isCanRead, bool isCanWrite) 
        {
            _DataSort = dataSort;
            _IsCanRead = isCanRead;
            _IsCanWrite = isCanWrite;
        }

        /// <summary>
        /// 添加数据字段
        /// </summary>
        /// <param name="aryLen">字段长度</param>
        /// <param name="multiple">放大倍数</param>
        /// <returns>成功返回索引号，失败返回-1</returns>
        public int AddDataField(int aryLen, int multiple) 
        {
            if (aryLen > 0)
            {
                StDataField new_Field = new StDataField();
                new_Field.AryLen = aryLen;
                new_Field.AryData = new byte[aryLen];
                new_Field.Multiple = multiple;
                _DataItems.Add(new_Field);
                return _DataItems.Count - 1;
            }
            return -1;
        }
    }
}
