﻿using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL3115.Device
{
    /// <summary>
    /// 3115 标准表接收基类
    /// </summary>
    internal class CL3115RecvPacket : ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 标准表发送包基类
    /// </summary>
    internal class CL3115SendPacket : ClouSendPacket_CLT11
    {
        public CL3115SendPacket()
            : base()
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        public CL3115SendPacket(bool needReplay)
            : base(needReplay)
        {
            ToID = 0x30;
            MyID = 0x05;
        }

        public override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
