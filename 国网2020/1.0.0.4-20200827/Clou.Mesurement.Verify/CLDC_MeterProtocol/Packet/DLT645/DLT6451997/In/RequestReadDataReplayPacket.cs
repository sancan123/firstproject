using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.In
{
    class RequestReadDataReplayPacket : DL645RecvPacket
    {
        public string ReadData { get; set; }
        public int Len { get; set; }
        protected override bool ParseBody(byte[] buf)
        {
            ByteBuffer body = new ByteBuffer(buf);
            body.GetUShort();
            byte[] data = body.GetByteArray(Len) ;
            Array.Reverse(data);
            string tmpValue = BitConverter.ToString(data);
            tmpValue = tmpValue.Replace("-", "");
            ReadData = tmpValue;
            return true;
        }
    }
}
