using System;
using CLDC_Comm.BaseClass;
using log4net;

namespace CLDC_VerifyAdapter.Helper
{
    /// <summary>
    /// ��־��Ϣ����
    /// </summary>
    public class LogHelper:SingletonBase<LogHelper>
    {

        private ILog loger = null;
        /// <summary>
        /// ��־�ӿ�
        /// </summary>
        public ILog Loger
        {
            set { loger = value; }
            get { return loger; }
        }

        /// <summary>
        /// �������ʱ��Ϣ 
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        internal  void WriteInfo(object message)
        {
            if (Loger == null) return;
          
            //Console.SetCursorPosition(1, 1);
            Loger.Info(message);
        }

        /// <summary>
        /// ���������Ϣ
        /// </summary>
        /// <param name="message"></param>
        internal  void WriteWarm(object message, Exception ex)
        {
            if (Loger == null) return;
            Loger.Warn(message, ex);
        }

        /// <summary>
        /// ������־
        /// </summary>
        /// <param name="message">��Ϣ����</param>
        /// <param name="ex">�쳣</param>
        internal  void WriteError(object message, Exception ex)
        {
            if (Loger == null) return;
            Loger.Error(message, ex);
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="message"></param>
        internal  void WriteDebug(object message)
        {
            if (Loger == null) return;
            Loger.Debug(message);
        }
    }
}
