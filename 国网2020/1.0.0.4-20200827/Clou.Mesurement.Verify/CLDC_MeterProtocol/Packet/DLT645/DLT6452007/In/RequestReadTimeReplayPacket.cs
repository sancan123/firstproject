using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6452007.In
{
    class RequestReadTimeReplayPacket:DL645RecvPacket
    {
        public string TimeString { get; private set; }
        protected override bool ParseBody(byte[] buf)
        {
            ByteBuffer body = new ByteBuffer(buf);
            body.GetUShort();
            byte[] arrTime = body.GetByteArray(3);
            Array.Reverse(arrTime);
            TimeString = BitConverter.ToString(arrTime).Replace("-", "");
            return true;
        }
    }
}
