using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public static class App
    {
        static App()
        {
            Funs = new Functions();
            AppPath = Directory.GetCurrentDirectory();
            BasicSetting = new SystemConfigure();
            BasicSetting.Load();
        }

        /// <summary>
        /// 通用函数库
        /// </summary>
        public static Functions Funs { get; set; }

        /// <summary>
        /// 当前执行文件目录
        /// </summary>
        public static string AppPath { get; set; }

        /// <summary>
        /// 当前应用的基本设置
        /// </summary>
        public static SystemConfigure BasicSetting { get; set; }

        /// <summary>
        ///消息处理 
        /// </summary>
        public static MessageControl Message { get; set; }
    }
}
