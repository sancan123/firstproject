using System;
using System.Collections.Generic;

namespace CLDC_DataCore.Function
{
    /// <summary>
    /// 事件日志类
    /// 确保每个应用程序只有一个线程调用.DoWork方法
    /// </summary>
    public class RunLog
    {
        private string m_strLogPath;
        private object objLock = new object();
        /// <summary>
        /// 
        /// </summary>
        public int SleepTime = 1;
        private struct stLog
        {
            public string LogPath;
            public string Sender;
            public string LogMsg;

            public override string ToString()
            {
                string strReturn = string.Format("\r\n{0}\r\n{1}\r\n描述:{2}"
                                                , DateTime.Now.ToString()
                                                , Sender
                                                , LogMsg);
                return strReturn;
            }
        }

        private List<stLog> lstRunLog = new List<stLog>();

        /// <summary>
        /// 
        /// </summary>
        public RunLog()
        {
            m_strLogPath = File.GetPhyPath("\\Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".Log");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPath"></param>
        public RunLog(string strPath)
        {
            if (!System.IO.File.Exists(strPath))
                File.Create(strPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="FunName"></param>
        /// <param name="LogMsg"></param>
        public void WriteLog(object sender, string FunName, string LogMsg)
        {
            WriteLog(String.Empty, sender, FunName, LogMsg);
        }
        /// <summary>
        /// 加入一条日志记录
        /// </summary>
        /// <param name="LogFilePath">自定义保存路径</param>
        /// <param name="sender">发生者</param>
        /// <param name="FunName">过程名</param>
        /// <param name="LogMsg">要记录的内容</param>
        public void WriteLog(string LogFilePath, object sender, string FunName, string LogMsg)
        {
            string FilePath = String.Empty;
            stLog _tagLog = new stLog();
            lock (objLock)
            {
                if (!Function.Common.IsEmpty(LogFilePath))
                    FilePath = File.GetPhyPath(LogFilePath);
                else
                    FilePath = m_strLogPath;
                if (!System.IO.File.Exists(FilePath))
                {
                    System.IO.FileStream _FS = File.Create(FilePath);
                    if (_FS != null)
                        _FS.Close();
                }

                System.Diagnostics.Debug.Assert(FilePath != null);

                if (sender == null) sender = "null";
                _tagLog.Sender = sender.ToString() + "." + FunName;
                _tagLog.LogPath = FilePath;
                _tagLog.LogMsg = LogMsg;
                lstRunLog.Add(_tagLog);
            }

        }
        /// <summary>
        /// 线程函数,必须保证只有一个线程写日志
        /// </summary>
        public void DoWork()
        {
            while (true)
            {
                if (Const.GlobalUnit.ApplicationIsOver)
                    break;
                if (lstRunLog.Count > 0)
                {
                    try
                    {
                        stLog _tagLog = lstRunLog[0];
                        if (_tagLog.LogPath != null)
                        {
                            System.IO.File.AppendAllText(_tagLog.LogPath, _tagLog.ToString());
                            lstRunLog.RemoveAt(0);
                        }
                    }
                    catch (Exception EX)
                    {
                        string logPath = "/Log/Thread/LogThread-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                        WriteLog(logPath, this, "DoWork", EX.Message + "\r\n" + EX.StackTrace);
                    }
                }
                System.Threading.Thread.Sleep(SleepTime);
            }
        }

    }
}
