using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 请求关源指令
    /// 返回:CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetPowerOffPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x45);
            return buf.ToByteArray();
        }
    }
}
