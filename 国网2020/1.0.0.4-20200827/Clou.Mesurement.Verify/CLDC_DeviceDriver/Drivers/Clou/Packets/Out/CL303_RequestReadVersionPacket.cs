using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取CL303源版本号
    /// 发送字母V 的ASC码
    /// </summary>
    internal class CL303_RequestReadVersionPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x56);
            return buf.ToByteArray();
        }
    }
}
