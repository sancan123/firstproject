
using System;
using System.Collections.Generic;
using System.Threading;

namespace CLDC_DataCore
{
    public class RealTimeMsgControl
    {
        #region ----------公共成员----------
        /// <summary>
        /// 消息事件委托，
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="E">消息参数</param>
        public delegate void OnUpdateRealTimeMsg(object sender, object E);
        /// <summary>
        /// 消息委托，当有消息到达时触发
        /// </summary>
        public OnUpdateRealTimeMsg UpdateRealTimeMsg;

        /// <summary>
        /// 消息轮询间隔，此越小处理消息速度越快
        /// </summary>
        public int SleepTime = 10;

        
        /// <summary>
        /// 队列最大成员数量。多余部分刚删除掉
        /// </summary>
        public int MaxItem = 50;
        #endregion

        #region ----------私有成员----------
        /// <summary>
        /// 队列对象
        /// </summary>
        private Queue<Struct.StRealTimeMsg> lstMsg = new Queue<Struct.StRealTimeMsg>();
        /// <summary>
        /// 线程读取锁
        /// </summary>
        private object objLock = new object();
        /// <summary>
        /// 线程写锁
        /// </summary>
        private object objAddLock = new object();
        #endregion


        /// <summary>
        /// 清除当前所有没处理的消息
        /// </summary>
        public void ClearCache()
        {
            lstMsg.Clear();
        }

        /// <summary>
        /// 取当前消息队列中的消息数量
        /// </summary>
        public int Count
        {
            get
            {
                return lstMsg.Count;
            }
        }

        #region ---------消息队列添加---------

        /// <summary>
        /// 添加消息/数据到队列中
        /// </summary>
        /// <param name="sender">消息发出者</param>
        /// <param name="e">消息参数</param>
        public void AddMsg(object sender, object e)
        {
            //if (GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
            //    return;
            try
            {
                Struct.StRealTimeMsg _Msg = new Struct.StRealTimeMsg();
                _Msg.objSender = sender;
                //移除已经过期的消息
                while (lstMsg.Count > MaxItem)
                {
                    Struct.StRealTimeMsg m = lstMsg.Dequeue();                    
                }

                _Msg.cmdData = (CLDC_Comm.SerializationBytes)e;
                
                lstMsg.Enqueue(_Msg);

            }
            catch (Exception ex)
            {
                if (!(ex is ThreadAbortException))
                {
                    string logPath = "/Log/Thread/MsgThread-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    Const.GlobalUnit.Log.WriteLog(logPath, this, "AddMsg", ex.Message + "\r\n" + ex.StackTrace);
                }
#if DEBUG
                throw ex;
#endif
            }

        }

        #endregion

        #region----------消息/数据队列处理-DoWork()---------
        /// <summary>
        /// 消息处理线程，确保只有一个线程调用
        /// </summary>
        
        public void DoWork()
        {
            if (Const.GlobalUnit.IsDemo) return;
            while (true)
            {
                if (Const.GlobalUnit.ApplicationIsOver)
                {
                    break;
                }
                if (lstMsg.Count > 0)
                {
                    try
                    {
                        Struct.StRealTimeMsg _Msg = lstMsg.Dequeue();

                        if (UpdateRealTimeMsg != null)
                        {                            
                            //数据队列处理
                            UpdateRealTimeMsg(_Msg.objSender, _Msg.cmdData);                            
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        e.ToString();
                        //消息队列为空时的意外处理.
                    }
                    catch (Exception ex)
                    {
                        string logPath = "/Log/Thread/MsgThread-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                        Const.GlobalUnit.Log.WriteLog(logPath, this, "DoWork", ex.Message + "\r\n" + ex.StackTrace);
                    }
                }
                Thread.Sleep(SleepTime);
            }
           
            string logThreadPath = "/Log/Thread/MsgThreadInfo.log";
            Const.GlobalUnit.Log.WriteLog(logThreadPath, this, "DoWork", Thread.CurrentThread.Name + "退出");

        }
        #endregion

    }
}
