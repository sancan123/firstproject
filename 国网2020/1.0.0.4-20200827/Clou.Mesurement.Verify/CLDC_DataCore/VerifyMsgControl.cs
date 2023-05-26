
using System;
using System.Collections.Generic;
using System.Threading;
using CLDC_Comm.MessageArgs;
using CLDC_Comm.Enum;

namespace CLDC_DataCore
{

    public class VerifyMsgControl
    {
        #region ----------公共成员----------
        /// <summary>
        /// 消息事件委托，
        /// </summary>
        /// <param name="sender">发生者</param>
        /// <param name="E">消息参数</param>
        public delegate void OnShowMsg(object sender, object E);
        /// <summary>
        /// 消息委托，当有消息到达时触发
        /// </summary>
        public OnShowMsg ShowMsg;

        /// <summary>
        /// 消息轮询间隔，此越小处理消息速度越快
        /// </summary>
        public int SleepTime = 5;

        /// <summary>
        /// 是否是消息队列，为TRUE时为消息队列，为FALSE时为数据队列
        /// </summary>
        public bool IsMsg = true;

        /// <summary>
        /// 队列最大成员数量。多余部分刚删除掉
        /// </summary>
        public int MaxItem = 50;
        #endregion

        #region ----------私有成员----------
        /// <summary>
        /// 队列对象
        /// </summary>
        private Queue<Struct.StVerifyMsg> lstMsg = new Queue<Struct.StVerifyMsg>();
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
                Struct.StVerifyMsg _Msg = new Struct.StVerifyMsg();
                _Msg.objSender = sender;
                //移除已经过期的消息
                while (lstMsg.Count > MaxItem)
                {
                    Struct.StVerifyMsg m = lstMsg.Dequeue();
                    //Console.WriteLine("move one message");
                }
                if (IsMsg)
                {
                    _Msg.objEventArgs = (EventArgs)e;
                    //进度消息不重复添加
                     if (e is CLDC_Comm.MessageArgs.EventMessageArgs)
                    {
                        //清空队列
                        if (((CLDC_Comm.MessageArgs.EventMessageArgs)e).MessageType == Cus_MessageType.清空消息队列)
                        {
                            ClearCache();
                            return;
                        }
                        else if (((CLDC_Comm.MessageArgs.EventMessageArgs)e).MessageType == Cus_MessageType.提示消息)
                        {
                            //线程消息过虑
                            if (((CLDC_Comm.MessageArgs.EventMessageArgs)e).Message.IndexOf("线程") != -1 ||
                                ((CLDC_Comm.MessageArgs.EventMessageArgs)e).Message.IndexOf("Thread was") != -1)
                            {
                                return;
                            }
                        }
                    }

                }
                else
                {
                    _Msg.cmdData = (CLDC_Comm.SerializationBytes)e;
                }
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
                        Struct.StVerifyMsg _Msg = lstMsg.Dequeue();

                        if (ShowMsg != null)
                        {
                            if (IsMsg)
                            {
                                //消息队列处理
                                ShowMsg(_Msg.objSender, _Msg.objEventArgs);
                            }
                            else
                            {
                                //数据队列处理
                                ShowMsg(_Msg.objSender, _Msg.cmdData);
                            }
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
#if DEBUG
                        //throw ex;
#endif
                    }
                }
                Thread.Sleep(SleepTime);
            }
           // Console.WriteLine("消息线程已经退出");
            string logThreadPath = "/Log/Thread/MsgThreadInfo.log";
            Const.GlobalUnit.Log.WriteLog(logThreadPath, this, "DoWork", Thread.CurrentThread.Name + "退出");

        }
        #endregion

        #region----------消息泵----------
        /// <summary>
        /// 外发消息:只刷新数据
        /// </summary>
        public void OutMessage()
        {
            OutMessage("null");
        }

        /// <summary>
        /// 外发检定消息[默认为运行时消息，需要刷新数据]
        /// </summary>
        /// <param name="strMessage"></param>
        public void OutMessage(string strMessage)
        {
            CLDC_Comm.MessageArgs.EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = CLDC_Comm.Enum.Cus_MessageType.运行时消息;
            _Message.Message = strMessage;
            
            OutMessage(_Message);
        }

        /// <summary>
        /// 外发检定消息[默认为运行时消息，可设置是否需要刷新数据]
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="RefreshData"></param>
        public void OutMessage(string strMessage, bool RefreshData)
        {
            CLDC_Comm.MessageArgs.EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = Cus_MessageType.运行时消息;
            _Message.Message = strMessage;
            _Message.RefreshData = RefreshData;
            OutMessage(_Message);

        }

        /// <summary>
        /// 外发检定消息[可设置是否刷新数据及消息类型]
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="RefreshData"></param>
        /// <param name="eType"></param>
        public void OutMessage(string strMessage, bool RefreshData, CLDC_Comm.Enum.Cus_MessageType eType)
        {
            CLDC_Comm.MessageArgs.EventMessageArgs _Message = new EventMessageArgs();
            _Message.MessageType = eType;
            _Message.Message = strMessage;
            _Message.RefreshData = RefreshData;
            OutMessage(_Message);
        }

        /// <summary>
        /// 外发检定消息
        /// </summary>
        /// <param name="MessageType"></param>
        public void OutMessage(CLDC_Comm.Enum.Cus_MessageType MessageType)
        {
            CLDC_Comm.MessageArgs.EventMessageArgs _E = new EventMessageArgs();
            _E.MessageType = MessageType;
            _E.RefreshData = false;
            OutMessage(_E);
        }

        /// <summary>
        /// 外发检定消息
        /// </summary>
        /// <param name="e"></param>
        public void OutMessage(CLDC_Comm.MessageArgs.EventMessageArgs e)
        {
            if (IsMsg)
            {
                AddMsg(this, e);
            }
        }

        
        #endregion
    }
}
