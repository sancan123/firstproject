using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V90.Packets.In
{
    internal class RequestResultReplayPacket : V90.Packets.Base
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public enum ReplayCode
        {
            /// <summary>
            /// 响应命令，表示“OK”
            /// </summary>
            Ok = 0x30,
            /// <summary>
            /// 响应命令，表示出错
            /// </summary>
            Error = 0x33,
            /// <summary>
            /// 响应命令，表示系统估计还要忙若干mS
            /// </summary>
            Busy = 0x35,
            /// <summary>
            /// 响应命令，禁止改写数据或存储器内容
            /// </summary>
            CanWrite = 0x36
        }
        public RequestResultReplayPacket()
        {
            IsResultPacket = true;
        }
        /// <summary>
        /// 回复结果
        /// </summary>
        public ReplayCode Result
        {
            get { return (ReplayCode)base.LstPacketFrame[4]; }
        }


    }
}
