using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL303
{
    #region  303
    /// <summary>
    /// 303 控源设备 接收包基类
    /// </summary>
    abstract class CL303RecvPacket : ClouRecvPacket_NotCltTwo
    {
    }
    /// <summary>
    /// 
    /// </summary>
    abstract class CL303SendPacket : ClouSendPacket_NotCltTwo
    {
        public CL303SendPacket()
            : base(true, 0x20)
        {

        }

        /// <summary>
        /// 返回数据的最大字节数（byte）
        /// </summary>
        /// <returns></returns>
        public override int MaxByte()
        {
            return 12;
        }
    }
    #endregion 


}
