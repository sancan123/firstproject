using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.SocketModule.Packet;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{
    class Geny_ReplyStdMeterPacket : GenyRecvPacket
    {
        /// <summary>
        /// 已重写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool ParsePacket(byte[] value)
        {
            if (value.Length < 10)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }

            if (value[0] != 0x02 || value[1] != 0xAA || value[9] != 0xAA)
            {
                this.ReciveResult = RecvResult.FrameError;
                return false;
            }
            this.CmdCode = 0xAA;
            this.SendID = 0x02;

            if (value.Length == 10)
            {
                this.resultData = Encoding.ASCII.GetString(value, 2, 7);
            }
            else
            {
                this.resultData = Encoding.ASCII.GetString(value, 10, value.Length - 10);
            }
            this.ParseData(this.resultData);
            return true;
        }

        /// <summary>
        /// 已重写，
        /// 无任何操作
        /// </summary>
        /// <param name="s"></param>
        protected override void ParseData(string s)
        {
            this.ReciveResult = RecvResult.OK; 
        }
    }
}
