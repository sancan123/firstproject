using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6452007.In
{
    class RequestReadDateReplyPacket : DL645RecvPacket
    {
        public string DateString { get; set; }
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
