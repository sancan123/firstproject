using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.SocketModule.Packet;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 485数据请求包
    /// </summary>
    internal class Rs485RequestPacket : SendPacket
    {
        public byte[] pata;



        public Rs485RequestPacket(bool needReturn)
        {
            IsNeedReturn = needReturn;
        }

        public override byte[] GetPacketData()
        {
            return pata;
        }

    }
}
