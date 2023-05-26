using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public class MessageControl
    {
        
        public delegate void OnShowMsg(bool isSend, string fileName);
        /// <summary>
        /// 消息委托，当有消息到达时触发
        /// </summary>
        public OnShowMsg ShowMsg;
    }
}
