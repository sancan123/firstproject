using System;
using CLDC_Comm.BaseClass;
using log4net;

namespace CLDC_VerifyAdapter.Helper
{
    /// <summary>
    /// 日志信息助手
    /// </summary>
    public class LogHelper:SingletonBase<LogHelper>
    {

        private ILog loger = null;
        /// <summary>
        /// 日志接口
        /// </summary>
        public ILog Loger
        {
            set { loger = value; }
            get { return loger; }
        }

        /// <summary>
        /// 输出运行时消息 
        /// </summary>
        /// <param name="message">消息内容</param>
        internal  void WriteInfo(object message)
        {
            if (Loger == null) return;
          
            //Console.SetCursorPosition(1, 1);
            Loger.Info(message);
        }

        /// <summary>
        /// 输出警告消息
        /// </summary>
        /// <param name="message"></param>
        internal  void WriteWarm(object message, Exception ex)
        {
            if (Loger == null) return;
            Loger.Warn(message, ex);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="ex">异常</param>
        internal  void WriteError(object message, Exception ex)
        {
            if (Loger == null) return;
            Loger.Error(message, ex);
        }

        /// <summary>
        /// 调试输出
        /// </summary>
        /// <param name="message"></param>
        internal  void WriteDebug(object message)
        {
            if (Loger == null) return;
            Loger.Debug(message);
        }
    }
}
