﻿using System;

namespace CLDC_DataCore.Struct
{
    [Serializable()]
    public class StSRFrame
    {
        /// <summary>
        /// 帧
        /// </summary>
        public byte[] Frame;
        /// <summary>
        /// 帧解析
        /// </summary>
        public string FrameMeaning;
        /// <summary>
        /// 帧时间
        /// </summary>
        public DateTime FrameTime;
        /// <summary>
        /// 获取帧字符串
        /// </summary>
        public string getStrFrame
        {
            get
            {
                if (null == Frame)
                {
                    return "";
                }

                return BitConverter.ToString(Frame);
            }
        }
        /// <summary>
        /// 以英文逗号间隔的串：'帧','帧解析','帧时间'
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("'{0}','{1}','{2}'", getStrFrame, FrameMeaning, FrameTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
