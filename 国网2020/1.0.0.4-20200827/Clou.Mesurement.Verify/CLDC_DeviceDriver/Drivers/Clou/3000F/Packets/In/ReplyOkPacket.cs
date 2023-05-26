using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 回复数据包
    /// </summary>
    class ReplyOkPacket:ClouRecvPacket_CLT11
    {
        protected override void ParseBody(byte[] data)
        {
            if (data.Length == 1)
            {
                if (data[0] == 0x30)
                {
                    ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
                }
                else if (data[0] == 0x33)
                {
                    ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
                }
                else
                {
                    ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.NOCOMMAND;
                }
            }
            else
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.FrameError;
            }
        }

        public override string GetPacketName()
        {
            return "ReplyOkPacket";
        }
    }
}
