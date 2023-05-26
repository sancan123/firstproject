using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 读取 gps 时间的包
    /// </summary>
    class Geny_RequestReadGPSTimepacket : GenySendPacket
    {
        internal static List<byte> buf = new List<byte>();

        static Geny_RequestReadGPSTimepacket()
        {
            buf.Add(0x03);
            buf.Add(0xAA);
            buf.Add(0x05);
            for (int i = 0; i < 7; i++)
            {
                buf.Add(0xAA);
            }
        }

        /// <summary>
        /// 是否使用时基源
        /// </summary>
        public bool IsUseTimeBase
        {
            get;
            set;
        }



        public Geny_RequestReadGPSTimepacket(bool isUseTimeBase)
        {
            this.IsUseTimeBase = IsUseTimeBase;
        }

        public override byte[] GetPacketData()
        {
            if (IsUseTimeBase == true)
            {
                return buf.ToArray();
            }
            else
            {
                return new byte[0];
            }
        }

        protected override byte[] GetBody()
        {
            throw new Exception();
        }
    }
}
