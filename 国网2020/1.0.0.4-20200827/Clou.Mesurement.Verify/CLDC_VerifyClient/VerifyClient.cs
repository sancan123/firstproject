using System;
using CLDC_Interfaces;
using CLDC_Interfaces.WcfDynamicProxy;
using CLDC_DataCore;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.LogModel;

namespace CLDC_VerifyClient
{
    public class VerifyClient : SingletonBase<VerifyClient>, IVerifyMessage
    {
        private DynamicProxy client = null;

        private int connectionID = 0;

        public bool Status { get; set; }

        public VerifyClient()
        {
            DynamicProxyFactory dpf = new DynamicProxyFactory(Properties.Settings.Default.ViewIP);
            client = dpf.CreateProxy("IVerifyMessage");
            if (client != null)
            {
                Status = true;
                MessageController.Instance.EventNewMessage += Instance_EventNewMessage;
            }
        }

        void Instance_EventNewMessage(object sender, EventArgs e)
        {
            if (sender is MessageUnit)
            {
                MessageUnit message = (MessageUnit)sender;
                #region 各种类型的消息
                if (message.MessageType == EnumMonitorType.Default)
                {
                    OutMessage(message.Source, message.Level, message.Text);
                }
                else if (message.MessageType == EnumMonitorType.VerifyFinished)
                {
                    //调用检定完成
                    if (VerifyFinished())
                    {
                        Console.WriteLine("检定结束调用成功!!");
                    }
                    else
                    {
                        Console.WriteLine("检定结束调用失败!!");
                    }
                }
                else if(message.MessageType==EnumMonitorType.CheckStatus) //通知正在检定
                {
                    NotifyIsChecking(message.Text);
                }
                else
                {
                    OutMonitorInfo((int)(message.MessageType), message.Text);
                }
                #endregion
            }
            else if (sender is VerifyData)
            {
                VerifyData verifyData = (VerifyData)sender;
                OutVerifyData(verifyData.ItemKey, verifyData.ResultName, verifyData.ResultArray);
            }
            else if (sender is LogFrameInfo)
            {
                OutFrame((LogFrameInfo)sender);
            }
        }

        #region IVerifyMessage 成员
        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="messageSourse"></param>
        /// <param name="messageType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public void RequestId()
        {
            try
            {
                object rst = client.CallMethod("RequestId");

                if (rst is int)
                {
                    connectionID = (int)rst;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// 日志服务
        /// </summary>
        /// <param name="messageSourse">信息数据源:6:检定业务日志,7:设备操作日志,8:被检表报文</param>
        /// <param name="messageType">0:正常信息,1:告警信息,2:异常信息</param>
        /// <param name="message">信息文本</param>
        /// <returns></returns>
        public bool OutMessage(int messageSourse, int messageType, string message)
        {
            try
            {
                object rst = client.CallMethod("OutMessage", connectionID, messageSourse, messageType, message);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool OutVerifyData(string itemKey, string dataName, string[] dataValue)
        {
            try
            {
                object rst = client.CallMethod("OutVerifyData", connectionID, itemKey, dataName, dataValue);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool OutOneMeterData(string itemKey, string dataName, int meterIndex, string dataValue)
        {
            try
            {
                object rst = client.CallMethod("OutOneMeterData", connectionID, itemKey, dataName, meterIndex, dataValue);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 外发监视数据
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="monitorType">监视类型：enum_MonitorType</param>
        /// <param name="formatData"></param>
        /// <returns></returns>
        public bool OutMonitorInfo(int monitorType, string formatData)
        {
            try
            {
                object rst = client.CallMethod("OutMonitorInfo", connectionID, monitorType, formatData);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// 结束当前检定
        /// <summary>
        /// 结束当前检定
        /// </summary>
        /// <returns></returns>
        public bool VerifyFinished()
        {
            try
            {
                object rst = client.CallMethod("VerifyFinished", connectionID);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 通知正在检定
        /// </summary>
        /// <returns></returns>
        public bool NotifyIsChecking(string checkState)
        {
            try
            {
                object rst = client.CallMethod("NotifyIsChecking", connectionID,checkState);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool OutFrame(LogFrameInfo frameInfo)
        {
            try
            {
                string frameString = string.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}", frameInfo.strPortNo, frameInfo.strEquipName, frameInfo.strItemName, frameInfo.strMessage, frameInfo.sendFrm.getStrFrame, frameInfo.sendFrm.FrameMeaning, frameInfo.sendFrm.FrameTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), frameInfo.ResvFrm.getStrFrame, frameInfo.ResvFrm.FrameMeaning, frameInfo.ResvFrm.FrameTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), frameInfo.strOther);
                object rst = client.CallMethod("OutFrame", connectionID, frameString);

                if (rst is bool)
                {
                    return (bool)rst;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion
    }
}
