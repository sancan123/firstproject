using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.Utility.DogPackage
{
    /// <summary>
    /// 加密狗相关时间
    /// </summary>
    public class DogEventArgs :EventArgs
    {
        public DogEventArgs(bool flagExit,string errorString)
        {
            ErrorString = errorString;
            FlagExit = flagExit;
        }
        /// <summary>
        /// 错误字符串
        /// </summary>
        public string ErrorString { get; private set; }
        /// <summary>
        /// true:彻底找不到加密狗,要退出程序了,false:给用户提示当前没有找到加密狗
        /// </summary>
        public bool FlagExit { get; private set; }
    }
}
