using CLDC_Comm.BaseClass;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.LogModel;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CLDC_DataCore
{
    public class MessageController : SingletonBase<MessageController>
    {
        public MessageController()
        {
            ThreadPool.QueueUserWorkItem((obj) => ExecuteProcess());
        }
        private AutoResetEvent waitHandle = new AutoResetEvent(false);
        /// <summary>
        /// 检定消息队列
        /// </summary>
        private Queue<MessageUnit> actionQueue = new Queue<MessageUnit>();
        /// <summary>
        /// 检定结论队列
        /// </summary>
        private Queue<VerifyData> verifyDataQueue = new Queue<VerifyData>();
        /// <summary>
        /// 帧数据包队列
        /// </summary>
        private Queue<LogFrameInfo> frameQueue = new Queue<LogFrameInfo>();
        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="text">信息文本</param>
        /// <param name="source">信息数据源:6:检定业务日志,7:设备操作日志,8:被检表报文</param>
        /// <param name="level">0:正常信息,1:告警信息,2:异常信息,90:语音信息,91:语音信息告警,92:语音信息错误</param>
        public void AddMessage(string text, int source = 6, int level = 0)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(new MessageUnit { Source = source, Level = level, Text = text });
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 添加设备数据及状态消息
        /// </summary>
        /// <param name="monitorType"></param>
        /// <param name="text"></param>
        public void AddMonitorMessage(EnumMonitorType monitorType, string text)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(new MessageUnit { MessageType = monitorType, Text = text });
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 通知检定项完成
        /// </summary>
        public void NotifyVerifyFinished()
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(new MessageUnit { MessageType = EnumMonitorType.VerifyFinished, Text = "" });
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 通知主界面程序正在检定
        /// </summary>
        public void NotifyIsChecking(string checkStatus)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(new MessageUnit
                {
                    MessageType = EnumMonitorType.CheckStatus,
                    Text = checkStatus
                });
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 上传检点结论
        /// </summary>
        /// <param name="itemKey"></param>
        /// <param name="resultName"></param>
        /// <param name="resultArray"></param>
        public void UploadCheckResult(string itemKey, string resultName, string[] resultArray)
        {
            Thread.Sleep(500);
            lock (verifyDataQueue)
            {
                verifyDataQueue.Enqueue(new VerifyData(itemKey, resultName, resultArray));
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 通知数据帧
        /// </summary>
        /// <param name="framLog"></param>
        public void AddFrameLog(LogFrameInfo frameLog)
        {
            lock (frameQueue)
            {
                frameQueue.Enqueue(frameLog);
            }
            waitHandle.Set();
        }
        /// <summary>
        /// 在此处等待或执行队列中的动作
        /// </summary>
        private void ExecuteProcess()
        {
            while (true)
            {
                try
                {
                    while (actionQueue.Count > 0 || verifyDataQueue.Count > 0 || frameQueue.Count > 0)
                    {
                        if (verifyDataQueue.Count > 0)
                        {
                            VerifyData verifyData = verifyDataQueue.Dequeue();
                            if (EventNewMessage != null)
                            {
                                EventNewMessage(verifyData, null);
                            }
                        }
                        if (actionQueue.Count > 0)
                        {
                            MessageUnit message = actionQueue.Dequeue();
                            if (EventNewMessage != null)
                            {
                                EventNewMessage(message, null);
                            }
                        }
                        if (frameQueue.Count > 0)
                        {
                            LogFrameInfo frameInfo = frameQueue.Dequeue();
                            if (EventNewMessage != null)
                            {
                                EventNewMessage(frameInfo, null);
                            }
                        }
                    }
                }
                catch
                { }
                waitHandle.Reset();
                waitHandle.WaitOne();
            }
        }
        public event EventHandler EventNewMessage;
    }
    /// <summary>
    /// 消息
    /// </summary>
    public class MessageUnit
    {
        /// <summary>
        /// 信息类型 0:默认值,日志信息   其它见枚举值
        /// </summary>
        public EnumMonitorType MessageType { get; set; }
        /// <summary>
        /// 6:检定业务日志,7:设备操作日志,8:被检表报文
        /// </summary>
        public int Source { get; set; }
        /// <summary>
        /// 0:正常信息,1:告警信息,2:故障信息
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 消息包含的文本
        /// </summary>
        public string Text { get; set; }
    }
    /// 检定数据
    /// <summary>
    /// 检定数据
    /// </summary>
    public class VerifyData
    {
        public VerifyData(string itemKey, string resultName, string[] resultArray)
        {
            ItemKey = itemKey;
            ResultName = resultName;
            ResultArray = resultArray;
        }
        /// <summary>
        /// 检定点编号
        /// </summary>
        public string ItemKey { get; private set; }
        /// <summary>
        /// 结论名称
        /// </summary>
        public string ResultName { get; private set; }
        /// <summary>
        /// 检定结论
        /// </summary>
        public string[] ResultArray { get; set; }
    }
}
