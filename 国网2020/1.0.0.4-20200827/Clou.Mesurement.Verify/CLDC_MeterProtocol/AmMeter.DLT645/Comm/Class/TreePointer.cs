
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

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Class
{
    /// <summary>
    /// 树-父指针表示法
    /// </summary>
    public class TreePointer
    {
        private int[] _KeyCounts; //各级节点总数
        private IList<StNodeInfo> _Nodes; //节点信息列表

        #region ---- 节点信息 ----
        struct StNodeInfo
        {
            /// <summary>
            /// 节点数据
            /// </summary>
            public byte Data;
            /// <summary>
            /// 父节点索引
            /// </summary>
            public int Parent;
            /// <summary>
            /// 数据项索引
            /// </summary>
            public int DataItem;
            /// <summary>
            /// 子节点个数
            /// </summary>
            public int ChdCount;

            /// <summary>
            /// 创建枝节点
            /// </summary>
            /// <param name="data">数据域</param>
            /// <param name="parent">父节点索引</param>
            public StNodeInfo(byte data, int parent)
            {
                this.Data = data;
                this.Parent = parent;
                this.DataItem = -1;
                this.ChdCount = 0;
            }
            /// <summary>
            /// 创建叶节点
            /// </summary>
            /// <param name="data">数据域</param>
            /// <param name="parent">父节点索引</param>
            /// <param name="dataItem">数据项索引</param>
            public StNodeInfo(byte data, int parent, int dataItem)
            {
                this.Data = data;
                this.Parent = parent;
                this.DataItem = dataItem;
                this.ChdCount = 0;
            }
        }
        #endregion ---- 节点信息 ----

        public TreePointer()
        {
            _Nodes = new List<StNodeInfo>();
            _KeyCounts = new int[5];
            //添加根节点
            _KeyCounts[0] = 1;
            _Nodes.Add(new StNodeInfo(0xFF, -1));
        }

        /*
         * 节点路径：从根节点到叶节点数据.
         */

        /// <summary>
        /// 添加树的节点
        /// </summary>
        /// <param name="byts_Key">节点路径</param>
        /// <param name="s4_Data">待搜索的数据</param>
        public void AddData(byte[] byts_Key, int s4_Data)
        {
            int keyIndex = 0; //节点索引
            int prtIndex = 0; //父节点索引
            int suitIndex = 0; //插入点索引
            int chdStartIndex = 0; //子节点起始索引
            int limit = 0;
            StNodeInfo new_Node;
            for (int s4_Grade = 1; s4_Grade < _KeyCounts.Length; s4_Grade++)
            {
                prtIndex = keyIndex;
                chdStartIndex = GetChildStartIndex(prtIndex, s4_Grade - 1);
                keyIndex = Search(byts_Key[s4_Grade - 1], chdStartIndex, _Nodes[prtIndex].ChdCount, ref suitIndex);
                if (keyIndex >= 0) //节点已经存在
                    continue;
                keyIndex = suitIndex;
                if (s4_Grade < _KeyCounts.Length - 1)
                    new_Node = new StNodeInfo(byts_Key[s4_Grade - 1], prtIndex);
                else //叶节点
                    new_Node = new StNodeInfo(byts_Key[s4_Grade - 1], prtIndex, s4_Data);
                if (suitIndex > _Nodes.Count - 1)
                    _Nodes.Add(new_Node);
                else
                    _Nodes.Insert(keyIndex, new_Node);
                _KeyCounts[s4_Grade] += 1;
                //更改子节点个数
                ModifyChildCount(prtIndex);
                //更改父节点索引值
                if (s4_Grade == _KeyCounts.Length - 1) //叶节点，不做处理
                    continue;
                limit = GetChildStartIndex(keyIndex, s4_Grade);
                while (limit < _Nodes.Count)
                {
                    ModifyParentIndex(limit);
                    limit++;
                }
            }
        }

        /// <summary>
        /// 根据节点路径获取数据列表
        /// </summary>
        /// <param name="byts_Key">节点路径</param>
        /// <returns>如果叶节点数据为0xFF，则返回全部叶节点数据</returns>
        public IList<int> FindData(byte[] byts_Key)
        {
            IList<int> lst_Items = new List<int>();
            int keyIndex = 0; //节点索引
            int prtIndex = 0; //父节点索引
            int suitIndex = 0; //插入点索引
            int chdStartIndex = 0; //子节点起始索引
            bool isMass = false;
            for (int s4_Grade = 1; s4_Grade < _KeyCounts.Length; s4_Grade++)
            {
                prtIndex = keyIndex;
                chdStartIndex = GetChildStartIndex(prtIndex, s4_Grade - 1);
                if (byts_Key[s4_Grade - 1] == 0xFF)
                {
                    isMass = true;
                    for (int j = 0; j < _Nodes[keyIndex].ChdCount; j++)
                    {
                        byte[] temp_Keys = new byte[4];
                        Array.Copy(byts_Key, 0, temp_Keys, 0, 4);
                        temp_Keys[s4_Grade - 1] = _Nodes[chdStartIndex + j].Data;
                        IList<int> temp_Items = FindData(temp_Keys); //递归调用
                        for (int k = 0; k < temp_Items.Count; k++)
                            lst_Items.Add(temp_Items[k]);
                    }
                }
                else
                {
                    if (isMass) //数据块
                        break;
                    keyIndex = Search(byts_Key[s4_Grade - 1], chdStartIndex, _Nodes[prtIndex].ChdCount, ref suitIndex); //?
                    if (keyIndex < 0) //节点不存在
                        break;
                    if (s4_Grade == _KeyCounts.Length - 1) //叶节点
                        lst_Items.Add(_Nodes[keyIndex].DataItem);
                }
            }
            return lst_Items;
        }

        #region ---- 辅助函数 ----
        /// <summary>
        /// 修改子节点个数
        /// </summary>
        /// <param name="s4_Index">节点索引</param>
        private void ModifyChildCount(int s4_Index)
        {
            StNodeInfo temp_Node = _Nodes[s4_Index];
            temp_Node.ChdCount++;
            _Nodes[s4_Index] = temp_Node;
        }
        /// <summary>
        /// 修改父节点索引值
        /// </summary>
        /// <param name="s4_Index">节点索引</param>
        private void ModifyParentIndex(int s4_Index)
        {
            StNodeInfo temp_Node = _Nodes[s4_Index];
            temp_Node.Parent++;
            _Nodes[s4_Index] = temp_Node;
        }
        /// <summary>
        /// 获取大于该等级的节点总数
        /// </summary>
        /// <param name="s4_Grade">节点等级</param>
        /// <returns>节点总数</returns>
        private int GetGradeLimit(int s4_Grade)
        {
            int limit = 0;
            int i = 0;
            while (i <= s4_Grade)
            {
                limit += _KeyCounts[i];
                i++;
            }
            return limit;
        }
        /// <summary>
        /// 获取子节点的起始索引
        /// </summary>
        /// <param name="s4_Index">节点索引</param>
        /// <param name="s4_Grade">节点等级</param>
        /// <returns>起始索引</returns>
        private int GetChildStartIndex(int s4_Index, int s4_Grade)
        {
            int limit = GetGradeLimit(s4_Grade);
            int cursor = GetGradeLimit(s4_Grade - 1);
            while (cursor < s4_Index)
            {
                limit += _Nodes[cursor].ChdCount;
                cursor++;
            }
            return limit;
        }
        /// <summary>
        /// 在指定范围内搜索数据
        /// </summary>
        /// <param name="s4_Data">节点数据</param>
        /// <param name="s4_StartIndex">起始位置</param>
        /// <param name="s4_RangeLen">范围长度</param>
        /// <param name="s4_SuitIdx">恰当索引【当搜索失败时使用】</param>
        /// <returns>如果搜索失败，则返回-1</returns>
        private int Search(int s4_Data, int s4_StartIndex, int s4_RangeLen, ref int s4_SuitIdx)
        {
            int high = s4_RangeLen + s4_StartIndex - 1;
            int low = s4_StartIndex;
            int mid = 0;
            int trend = 0; //0-向右 1-向左
            while (low <= high)
            {
                mid = (high + low) / 2;
                if (s4_Data > _Nodes[mid].Data) //右搜索区间
                {
                    low = mid + 1;
                    trend = 0;
                }
                else if (s4_Data < _Nodes[mid].Data) //左搜索区间
                {
                    high = mid - 1;
                    trend = 1;
                }
                else
                    return mid;
            }
            if (trend == 0)
                s4_SuitIdx = low;
            else
                s4_SuitIdx = s4_StartIndex;
            return -1;
        }
        #endregion ---- 辅助函数 ----
    }//Class
}
