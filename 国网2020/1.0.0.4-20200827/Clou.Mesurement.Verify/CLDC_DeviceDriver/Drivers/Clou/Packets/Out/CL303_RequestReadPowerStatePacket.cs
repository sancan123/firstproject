using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    class CL303_RequestReadPowerStatePacket:CL303SendPacket
    {
        protected override byte[] GetBody()
        {
            return new byte[1] { 0x42};
        }
    }
}
