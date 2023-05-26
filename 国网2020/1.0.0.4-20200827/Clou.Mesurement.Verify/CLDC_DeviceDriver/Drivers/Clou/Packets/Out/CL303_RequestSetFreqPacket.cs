using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置频率指令
    /// ox33
    /// 返回：CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetFreqPacket : CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x33);
            return buf.ToByteArray();
        }
    }
}
