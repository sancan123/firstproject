using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace Mesurement.UiLayer.Utility.Log
{
    /// <summary>
    /// 日志管理器
    /// </summary>
    public class LogManager
    {
        static LogManager()
        {
            //日志处理线程
            new Task(new Action(LogProcess)).Start();
        }
        /// <summary>
        /// 进程里面的等待句柄
        /// 如果显示完毕以后设置为false
        /// </summary>
        private static AutoResetEvent waitHandle = new AutoResetEvent(false);
        /// <summary>
        /// 日志队列
        /// </summary>
        private static Queue<LogModel> queueMessage = new Queue<LogModel>();
        /// <summary>
        /// 添加到日志队列
        /// </summary>
        /// <param name="message">概要信息</param>
        /// <param name="source">数据源</param>
        /// <param name="level">日志等级</param>
        /// <param name="e">异常内容</param>
        public static void AddMessage(string message,EnumLogSource source = EnumLogSource.用户操作日志 ,EnumLevel level =EnumLevel.Information, Exception e=null)
        {
            queueMessage.Enqueue(new LogModel(message, source, level, e));

            //这里要判断任务的状态
            waitHandle.Set();
            if (level ==  EnumLevel.Error || level == EnumLevel.ErrorSpeech )
            {
                WriteErrLog(message);
            }
        }
        /// <summary>
        /// 日志线程要执行的动作
        /// </summary>
        private static void LogProcess()
        {
            while (true)
            {
                while (queueMessage.Count > 0)
                {
                    LogModel logClass = queueMessage.Dequeue();
                    //执行事件处理函数
                    ExecuteException(logClass);
                }
                waitHandle.Reset();
                waitHandle.WaitOne();
            }
        }
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="logClass"></param>
        private static void ExecuteException(LogModel logClass)
        {
            #region 触发事件
            if (LogMessageArrived != null)
            {
                LogMessageArrived(logClass, null);
            }
            #endregion
        }
        public static event EventHandler LogMessageArrived;

        /// <summary>
        /// 记录运行错误
        /// </summary>
        /// <param name="strMessage"></param>
        public static void WriteErrLog(string strMessage)
        {

            string LogPath = string.Format(@"Log\RunErrLog\{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
            LogPath = Directory.GetCurrentDirectory() + "\\" + LogPath;
            FileStream fs = Create(LogPath);
            if (fs == null)
            {
                return;
            }
            fs.Close();
            fs.Dispose();

            System.IO.File.AppendAllText(LogPath, strMessage + "\r\n\r\n");

        }


        /// <summary>
        /// 创建文件、如果目录不存在则自动创建、路径既可以是绝对路径也可以是相对路径
        /// 返回文件数据流，如果创建失败在返回null、如果文件存在则打开它
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static FileStream Create(string FileName)
        {
            
            string folder = FileName.Substring(0, FileName.LastIndexOf('\\') + 1);

            string tmpFolder = folder.Substring(0, FileName.IndexOf('\\')); //磁盘跟目录
            //逐层创建文件夹
            try
            {
                while (tmpFolder != folder)
                {
                    tmpFolder = folder.Substring(0, FileName.IndexOf('\\', tmpFolder.Length) + 1);
                    if (!System.IO.Directory.Exists(tmpFolder))
                        System.IO.Directory.CreateDirectory(tmpFolder);
                }
            }
            catch { return null; }

            if (System.IO.File.Exists(FileName))
            {
                return System.IO.File.Open(FileName, FileMode.Open, FileAccess.ReadWrite);
                //return null;
            }
            else
            {
                try
                {
                    return System.IO.File.Create(FileName);
                }
                catch { return null; }
            }
        }
    }
}
