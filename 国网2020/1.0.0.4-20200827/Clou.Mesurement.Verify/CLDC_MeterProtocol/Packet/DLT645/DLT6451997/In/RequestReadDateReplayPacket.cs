using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.In
{
    class RequestReadDateReplayPacket : DL645RecvPacket
    {
        public string DateString { get; private set; }
        protected override bool ParseBody(byte[] buf)
        {
            ByteBuffer body = new ByteBuffer(buf);
            body.GetByteArray(3);
            byte[] arrdate = body.GetByteArray(3);
            Array.Reverse(arrdate);
            DateString = BitConverter.ToString(arrdate).Replace("-", "");
            return true;
        }
    }
}
