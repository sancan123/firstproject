using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80_2036.Packets
{
    internal class Base : PackBase.Packet 
    {
        protected override bool CheckRecvFrame()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override byte[] CreateSendFrame()
        {
            throw new Exception("The method or operation is not implemented.");
        }


        public override bool ParsePacket(byte[] byData)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
