// ArrowEnds.cs by Charles Petzold, December 2007
using System;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// 箭头结束位置的枚举
    /// <summary>
    /// 箭头结束位置的枚举
    /// </summary>
    [Flags]
    public enum ArrowEnds
    {
        /// 没有箭头
        /// <summary>
        /// 没有箭头
        /// </summary>
        None = 0,
        /// 在开始的点
        /// <summary>
        /// 在开始的点
        /// </summary>
        Start = 1,
        /// 在开始的点
        /// <summary>
        /// 在开始的点
        /// </summary>
        Begin = 1,
        /// 在结束点
        /// <summary>
        /// 在结束点
        /// </summary>
        End = 2,
        /// 两头都有箭头
        /// <summary>
        /// 两头都有箭头
        /// </summary>
        Both = 3
    }
}
