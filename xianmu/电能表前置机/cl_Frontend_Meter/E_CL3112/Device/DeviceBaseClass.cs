using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL3112.Device
{
    #region CL3112
    /// <summary>
    /// 3112标准表接收基类
    /// </summary>
    internal class CL3112RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 3112发送标准表基类
    /// </summary>
    internal class CL3112SendPacket : ClouSendPacket_CLT11
    {
        public CL3112SendPacket()
            : base()
        {
            ToID = 0x01;
            MyID = 0x05;
        }

        public CL3112SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x01;
            MyID = 0x05;
        }
        public override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }

    #endregion CL3112
}
