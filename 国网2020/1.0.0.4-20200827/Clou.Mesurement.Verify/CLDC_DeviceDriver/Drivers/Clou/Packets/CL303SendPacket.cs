using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    abstract class CL303SendPacket : ClouSendPacket_NotCltTwo
    {
        public CL303SendPacket()
            : base(true, 0x20)
        {

        }
    }
}
