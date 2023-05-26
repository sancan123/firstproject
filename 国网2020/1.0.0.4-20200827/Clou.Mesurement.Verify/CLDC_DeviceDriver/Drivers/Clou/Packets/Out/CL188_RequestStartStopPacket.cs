using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 控制误差板启动/停止/继续
    /// 48-4A指令
    /// </summary>
    internal class CL188_RequestStartStopPacket : Cl188SendPacket
    {

        public CL188_RequestStartStopPacket():base(false)
        {
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public enum ControlType
        {
            启动当前功能 = 0x48,
            停止当前功能 = 0x49,
            继续当前功能 = 0x4A,
        }

        /// <summary>
        /// 误差板序号
        /// </summary>
        public byte Pos = 0;
        /// <summary>
        /// 操作类型
        /// </summary>
        public ControlType ControlTypes = ControlType.启动当前功能;

        public override string GetPacketName()
        {
            return "CL188_RequestStartStopPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put((byte)ControlTypes);
            buf.Put(Pos);
            return buf.ToByteArray();
        }
    }
}
