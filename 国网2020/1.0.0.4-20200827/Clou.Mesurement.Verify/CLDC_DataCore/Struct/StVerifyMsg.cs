using System;

namespace CLDC_DataCore.Struct
{

    public struct StVerifyMsg
    {
        /// <summary>
        /// 消息发送者
        /// </summary>
        public object objSender;
        /// <summary>
        /// 消息参数
        /// </summary>
        public EventArgs objEventArgs;

        /// <summary>
        /// 数据参数
        /// </summary>
        public CLDC_Comm.SerializationBytes cmdData; 
    }
}
