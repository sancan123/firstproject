using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6452007.In
{
    class RequestReadDataBlockReplayPacket : RequestReadDataReplayPacket
    {
        /// <summary>
        /// 读取到的数据
        /// </summary>
        public new string[] ReadData { get; private set; }
        protected override bool ParseBody(byte[] buf)
        {
            List<string> lstData = new List<string>();
            bool hasSpliter = true;
            ByteBuffer body = new ByteBuffer(buf);
            body.GetByteArray(4);//标识符
            if (((buf.Length - 4) % Len) == 0)
                hasSpliter = false;
            while (body.Position < buf.Length)
            {
                byte[] tmpData = body.GetByteArray(Len);
                lstData.Add(BitConverter.ToString(tmpData).Replace("-", ""));
                if (hasSpliter) body.Get();
            }
            string[] arrRet = lstData.ToArray();
            Array.Reverse(arrRet);
            ReadData = arrRet;
            return true;
        }
    }
}
