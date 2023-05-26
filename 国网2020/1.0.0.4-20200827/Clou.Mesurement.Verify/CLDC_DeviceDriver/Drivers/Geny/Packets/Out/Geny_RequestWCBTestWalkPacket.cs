using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 走字误差 
    /// </summary>
    class Geny_RequestWCBTestWalkPacket : Geny_RequestBasicErrorPacket
    {

        public byte ReverseMark
        {
            get;
            set;
        }

        public byte NoError
        {
            get;
            set;
        }

        public Geny_RequestWCBTestWalkPacket(byte meterIndex, ushort circleCount, int standPredcitValue, byte signalChannel, CLDC_Comm.Enum.Cus_GyGyType pluseType, double errorUpLimit, double errorDownLimit, byte reverseMark, byte noError)
            : base(meterIndex, circleCount, standPredcitValue, signalChannel, pluseType, errorUpLimit, errorDownLimit)
        {
            this.CmdCode = 0x2b;
            this.ReverseMark = reverseMark;
            this.NoError = noError;
        }

        protected override byte[] GetBody()
        {
            List<byte> buf = new List<byte>();

            buf.AddRange(DataFormart.Formart((uint)this.CircleCount, false));
            buf.AddRange(DataFormart.Formart((double)(Math.Round(this.StandardPredcitValue * 1.0 / this.CircleCount, 0)), 0, false));
            buf.Add((byte)this.SignalChannel);
            buf.Add((byte)this.PulseType);
            buf.AddRange(DataFormart.Formart(this.ErrorUpLimit * 10000, 0, false));
            buf.AddRange(DataFormart.Formart(this.ErrorDownLimit * 10000, 0, false));
            buf.Add(this.ReverseMark);
            buf.Add(this.NoError);
            return buf.ToArray();
        }
    }
}
