using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 
    /// </summary>
    class Geny_RequestVoltageZBPacket : GenySendPacket
    {
        public double XS
        {
            get;
            set;
        }

        public byte Times
        {
            get;
            set;
        }

        public Geny_RequestVoltageZBPacket(byte deviceID, double xs, byte times)
            : base(deviceID, 0x22)
        {
            this.XS = xs;
            this.Times = times;
        }

        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>();

            buf.AddRange(DataFormart.Formart(this.XS, 3, false));
            buf.Add((this.Times));

            return buf.ToArray();
        }
    }
}
