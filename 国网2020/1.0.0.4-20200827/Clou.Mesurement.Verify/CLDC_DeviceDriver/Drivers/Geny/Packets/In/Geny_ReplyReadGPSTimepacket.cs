using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;
using CLDC_Comm.SocketModule.Packet;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 读取gps 时间返回包
    /// </summary>
    class Geny_ReplyReadGPSTimepacket : GenyRecvPacket
    {
        static byte[] sendByte = Geny_RequestReadGPSTimepacket.buf.ToArray();

        /// <summary>
        /// 读取的GPS时间
        /// </summary>
        public DateTime GPSTime
        {
            get;
            set;
        }

        /// <summary>
        /// 是否直接使用时基源，
        /// 是否直接使用，两者发回的数据结构不一样
        /// </summary>
        public bool IsUseTimeBase
        {
            get;
            set;
        }

        public Geny_ReplyReadGPSTimepacket(bool isUseTimeBase)
        {
            this.IsUseTimeBase = isUseTimeBase;
        }

        private bool ParsePacketGPS(byte[] value)
        {
            string data = Encoding.ASCII.GetString(value);
            int startIndex = data.IndexOf("GPSGA", StringComparison.OrdinalIgnoreCase);

            if (startIndex < 0)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }
            string[] strs = data.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (strs.Length < 2)
                return false;
            string dateTime = strs[1];
            foreach (char c in dateTime)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            int hh = int.Parse(dateTime.Substring(0, 2)) + 8;
            int mm = int.Parse(dateTime.Substring(2, 2));
            int ss = int.Parse(dateTime.Substring(4, 2));
            DateTime dt = DateTime.Now;
            this.GPSTime = new DateTime(dt.Year, dt.Month, dt.Day, hh, mm, ss);
            return true;
        }

        private bool ParsePacketTimeBase(byte[] value)
        {
            //检查头是否与返回的数据相同
            if (value.Length < 10 + 7)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }

            int i = 0;
            for (i = 0; i < sendByte.Length; i++)
            {
                if (sendByte[i] != value[i])
                {
                    break;
                }
            }
            if (i < sendByte.Length)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }

            int yy = value[11] * 100 + value[10];
            int mm = value[12];
            int dd = value[13];

            int hh = (value[14] % 16) * 10 + value[14] / 16;
            int min = (value[15] % 16) * 10 + value[15] / 16;
            int ss = (value[16] % 16) * 10 + value[16] / 16;

            GPSTime = new DateTime(yy, mm, dd, hh, min, ss, DateTimeKind.Local);
            this.ReciveResult = RecvResult.OK;
            return true;
        }

        public override bool ParsePacket(byte[] value)
        {
            if (IsUseTimeBase)
                return ParsePacketTimeBase(value);
            else
                return ParsePacketGPS(value);
        }
        protected override void ParseData(string s)
        {
        }
    }
}
