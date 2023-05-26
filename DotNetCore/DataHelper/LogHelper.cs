using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataHelper
{
    //日志管理类
    public class LogHelper
    {
        public static string LogPath = App.AppPath + @"\Log";
        /// <summary>
        /// 报错日志
        /// </summary>
        /// <param name="msg">报错信息</param>
        public static void WriteErrorLog(string msg)
        {
            try
            {
                if (!App.BasicSetting.PrintLog) return;
                string ErrorLogPath = LogPath + "\\ErrorLog";
                if (!Directory.Exists(ErrorLogPath))
                    Directory.CreateDirectory(ErrorLogPath);
                DirectoryInfo thisDirectory = new DirectoryInfo(ErrorLogPath);
                foreach (DirectoryInfo directory in thisDirectory.GetDirectories())
                {
                    if (DateTime.Now.Subtract(directory.LastWriteTime).Days > App.BasicSetting.DeleteLogInterval)
                        System.IO.Directory.Delete(directory.FullName, true);
                }


                string file = string.Format(@"{0}\{1}.txt", ErrorLogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                string text = string.Format(@"{0}:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg) + "\r\n\r\n";

                File.AppendAllText(file, text);
            }
            catch
            { }
        }


        /// <summary>
        /// 运行日志
        /// </summary>
        /// <param name="msg">运行操作消息</param>
        public static void WriteRunLog(string msg)
        {
            try
            {
                if (!App.BasicSetting.PrintLog) return;
                string ErrorLogPath = LogPath + "\\RunLog";
                if (!Directory.Exists(ErrorLogPath))
                    Directory.CreateDirectory(ErrorLogPath);
                DirectoryInfo thisDirectory = new DirectoryInfo(ErrorLogPath);
                foreach (DirectoryInfo directory in thisDirectory.GetDirectories())
                {
                    if (DateTime.Now.Subtract(directory.LastWriteTime).Days > App.BasicSetting.DeleteLogInterval)
                        System.IO.Directory.Delete(directory.FullName, true);
                }


                string file = string.Format(@"{0}\{1}.txt", ErrorLogPath, DateTime.Now.ToString("yyyy-MM-dd"));
                string text = string.Format(@"{0}:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg) + "\r\n\r\n";

                File.AppendAllText(file, text);
            }
            catch
            { }
        }     
    }
}
