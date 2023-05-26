using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 命令 50H
    /// 帧格式：帧头+50H+误差路数
    /// </summary>
    internal class CL188_RequestReadStatusPacket : Cl188SendPacket
    {
        protected override byte[] GetBody()
        {
            return new byte[2] { 0x50, Pos };
        }
    }
}
