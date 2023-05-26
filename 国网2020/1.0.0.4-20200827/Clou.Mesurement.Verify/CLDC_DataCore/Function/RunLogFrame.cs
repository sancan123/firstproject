using System;
using System.Collections.Generic;
using CLDC_DataCore.Model.LogModel;
using System.Data.OleDb;
using CLDC_Comm.BaseClass;
using System.Threading;

namespace CLDC_DataCore.Function
{
    /// <summary>
    /// 写帧日志队列
    /// 数据库连接对象，调试模式一直处于连接状态，停止检定或停止调试，释放连接
    /// 1、默认日志库，Access
    /// 2、自定义数据库
    /// </summary>
    public class RunLogFrame : SingletonBase<RunLogFrame>
    {
        /// <summary>
        /// 
        /// </summary>
        public int SleepTime = 1;//ms
        /// <summary>
        /// 
        /// </summary>
        public int CmdTimeOut = 5000;//ms

        private string m_strLogPath;
        const string CONST_ACCESS = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Jet OLEDB:DataBase Password=;Data Source=";
        private OleDbConnection _Con;
        private object objLock = new object();
        private Queue<LogFrameInfo> lstRunLog = new Queue<LogFrameInfo>();

        //用于等待消息来临
        private AutoResetEvent waitHandle = new AutoResetEvent(true);

        /// <summary>
        /// 日志入口
        /// </summary>
        /// <param name="LogMsg">帧数据内容</param>
        public void WriteFrameLog(LogFrameInfo LogMsg)
        {

            WriteFrameLog(null, "", LogMsg);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="FunName"></param>
        /// <param name="LogMsg"></param>
        public void WriteFrameLog(object sender, string FunName, LogFrameInfo LogMsg)
        {

            lock (objLock)
            {

                System.Diagnostics.Debug.Assert(LogMsg != null);

                lstRunLog.Enqueue(LogMsg);

                waitHandle.Set();
            }

        }

    }
}
