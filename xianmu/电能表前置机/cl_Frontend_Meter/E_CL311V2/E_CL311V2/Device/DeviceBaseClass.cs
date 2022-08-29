using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL311V2.Device
{

    /// <summary>
    /// 311 标准表接收基类
    /// </summary>
    internal class Cl311RecvPacket : ClouRecvPacket_NotCltOne
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class Cl311SendPacket : ClouSendPacket_NotCltOne
    {
        public Cl311SendPacket()
            : base(true, 0x16)
        {
        }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
