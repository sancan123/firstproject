using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取 电流 电压 变比
    /// </summary>
    class Geny_RequestSourceReadPTCTPacket : GenySendPacket
    {

        public Geny_RequestSourceReadPTCTPacket()
        { }

        protected override byte[] GetBody()
        {
            throw new NotImplementedException();
        }
    }
}
