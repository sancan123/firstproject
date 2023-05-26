using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取工作状态回复包
    /// </summary>
    class RequestReadStatusReplyPacket : ClouRecvPacket_CLT11
    {
        /// <summary>
        /// 源状态
        /// </summary>
        public byte PowerStatus { get; set; }
        /// <summary>
        /// 表状态
        /// </summary>
        public byte MeterStatus { get; set; }
        /// <summary>
        /// GPS状态
        /// </summary>
        public byte GpsStatus { get; set; }
        /// <summary>
        /// 188状态
        /// </summary>
        public byte CL188Status { get; set; }
        /// <summary>
        /// 2036状态
        /// </summary>
        public byte CL2036Status { get; set; }
        /// <summary>
        /// 等电位状态
        /// </summary>
        public byte DengDianWeiStatus { get; set; }
        /// <summary>
        /// 高压源
        /// </summary>
        public byte PowerHeightStatus { get; set; }
        /// <summary>
        /// 前次试验类型
        /// </summary>
        public byte LastTaskType { get; set; }


        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50 && data.Length > 11)
            {
                if (data[1] == 0x00 && data[2] == 0x04 && data[3] == 0x02)
                {
                    PowerStatus = data[4];
                    MeterStatus = data[5];
                    GpsStatus = data[6];
                    CL188Status = data[7];
                    CL2036Status = data[8];
                    DengDianWeiStatus = data[9];
                    PowerHeightStatus = data[10];
                    LastTaskType = data[11];
                    ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
                }
                else
                {
                    ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
                }
            }
            else
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.FrameError;
            }
        }

        public override string GetPacketName()
        {
            return "RequestReadStatusReplyPacket";
        }
    }
}
