using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 读取 ptct 返回包
    /// </summary>
    class Geny_ReplyReadPTCTPacket : GenyRecvPacket
    {

        public int PTValue
        {
            get;
            set;
        }

        public int CTValue
        {
            get;
            set;
        }

        /// <summary>
        /// 台体的 编号
        /// </summary>
        public int TN
        {
            get;
            set;
        }

        protected override void ParseData(string s)
        {
            byte[] buf = Encoding.ASCII.GetBytes(s);

            if (buf.Length < 6)
            {
                this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
                return;
            }

            this.PTValue = buf[0] * 256 + buf[1];
            this.CTValue = buf[2] * 256 + buf[3];
            this.TN = buf[4] * 256 + buf[5];
            this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
        }
    }
}
