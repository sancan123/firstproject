
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
    /// 帧数据缓存
    /// </summary>
    public class FrameDeposit
    {
        private List<StFrameData> _FrameDatas; //数据缓存区
        private int _MaxSize; //缓存区大小
        private object _SyncLock = new object(); //锁

        struct StFrameData
        {
            private string _DataCode;
            /// <summary>
            /// 数据标识
            /// </summary>
            public string DataCode 
            {
                get { return _DataCode; }
            }

            private byte[] _FrameData;
            /// <summary>
            /// 帧数据
            /// </summary>
            public byte[] FrameData 
            {
                get { return _FrameData; }
            }

            public StFrameData(string dataCode, byte[] frameData) 
            {
                this._DataCode = dataCode;
                this._FrameData = frameData;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public FrameDeposit() 
        {
            _FrameDatas = new List<StFrameData>();
            _MaxSize = 10;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        public FrameDeposit(int size) 
        {
            _FrameDatas = new List<StFrameData>();
            _MaxSize = (size > 0) ? size : 10;
        }

        #region ---- 公共属性 ----
        /// <summary>
        /// 缓存区大小
        /// </summary>
        public int Size 
        {
            get 
            {
                return _MaxSize;
            }
            set
            {
                _MaxSize = (value > 0) ? value : 10;
            }
        }
        #endregion ---- 公共属性 ----

        /// <summary>
        /// 获取帧数据
        /// </summary>
        /// <param name="index">索引号</param>
        /// <returns>帧数据</returns>
        public byte[] this[int index] 
        {
            get
            {
                byte[] bfData = new byte[_FrameDatas[index].FrameData.Length]; //备份
                Array.Copy(_FrameDatas[index].FrameData, 0, bfData, 0, bfData.Length);
                return bfData;
            }
        }

        /// <summary>
        /// 查找数据标识
        /// </summary>
        /// <param name="dataCode">数据标识</param>
        /// <returns>成功返回索引号，失败返回-1</returns>
        public int FindCode(string dataCode)
        {
            for (int i = 0; i < _FrameDatas.Count; i++)
            {
                if (_FrameDatas[i].DataCode == dataCode)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 添加帧数据信息
        /// </summary>
        /// <param name="dataCode">数据标识</param>
        /// <param name="frameData">帧数据</param>
        /// <returns>成功返回索引号，失败返回-1</returns>
        public int AddFrameData(string dataCode, byte[] frameData)
        {
            try
            {
                System.Threading.Monitor.Enter(_SyncLock);
                if (_FrameDatas.Count == _MaxSize)
                    _FrameDatas.RemoveAt(_FrameDatas.Count - 1);
                byte[] bfData = new byte[frameData.Length];
                Array.Copy(frameData, 0, bfData, 0, bfData.Length); //备份
                StFrameData newData = new StFrameData(dataCode, bfData);
                _FrameDatas.Add(newData);
                return _FrameDatas.Count - 1;
            }
            catch { return -1; }
            finally
            {
                System.Threading.Monitor.Exit(_SyncLock);
            }
        }
    }
}
