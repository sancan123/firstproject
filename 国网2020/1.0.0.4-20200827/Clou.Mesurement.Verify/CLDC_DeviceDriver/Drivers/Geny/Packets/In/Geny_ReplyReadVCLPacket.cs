using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{
    /// <summary>
    /// 读取 电压，电流档位返回值
    /// </summary>
    class Geny_ReplyReadVCLPacket : GenyRecvPacket
    {

        /// <summary>
        /// 电流
        /// </summary>
        public double CurrentLevel
        {
            get;
            set;
        }

        /// <summary>
        /// 电压
        /// </summary>
        public double VoltageLevel
        {
            get;
            set;
        }

        protected override void ParseData(string s)
        {
            this.CurrentLevel = (s[0] << 24) + (s[1] << 16) + (s[2] << 8) + s[3];
            this.VoltageLevel = (s[4] << 24) + (s[5] << 16) + (s[6] << 8) + s[7];

            this.CurrentLevel /= 100;
            this.VoltageLevel /= 100;
            this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
        }
    }
}
