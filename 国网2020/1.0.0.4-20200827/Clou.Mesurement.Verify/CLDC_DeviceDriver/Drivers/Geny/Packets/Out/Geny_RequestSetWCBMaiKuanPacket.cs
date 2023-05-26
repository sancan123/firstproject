using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    class Geny_RequestSetWCBMaiKuanPacket:Geny_RequestMaiFuPacket
    {
        public Geny_RequestSetWCBMaiKuanPacket(byte deviceID, byte channels, CLDC_Comm.Enum.Cus_GyGyType pluseModel)
            : base(deviceID,channels,pluseModel)
        {
            CmdCode = 0x20;
        }
    }
}
