using System;
using System.Collections.Generic;
using System.Text;

namespace MeterProtocol.Packet.DLT645.DLT6451997.Out
{
    /// <summary>
    /// 读取日期时间【块读】
    /// </summary>
    class RequestReadDateTimePacket : DL645SendPacket
    {
        public int ReadType
        {
            set
            {
                byte[] data = new byte[2];
                data[0] = 0xC0;
                if (value == 3)
                    data[1] = 0x12;
                else
                    data[1] = 0x1f;
                Data.Add(data);
            }
        }
    }
}
