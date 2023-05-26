using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{


    /// <summary>
    ///   带一个参数类型包
    ///   参数:byte 1
    ///   表号:byte 1
    ///   控制码:byte 1
    /// </summary>
    internal class CL188_RequestSetOneParaUShortPacket : Cl188SendPacket
    {
        /// <summary>
        /// 数据域
        /// </summary>
        public ushort Data = 0;
        /// <summary>
        /// 表位号，如果不需要请保持为-1
        /// </summary>
        public byte Pos = 0xFE;
        /// <summary>
        /// 控制码
        /// </summary>
        public byte CmdCode = 0x00;
        public override string GetPacketName()
        {
            return "CL188_RequestSetOneParaBytePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(CmdCode);
            if (Pos != 0xFe)
                buf.Put(Pos);
            buf.PutUShort(Data);
            return buf.ToByteArray();
        }
    }
}
