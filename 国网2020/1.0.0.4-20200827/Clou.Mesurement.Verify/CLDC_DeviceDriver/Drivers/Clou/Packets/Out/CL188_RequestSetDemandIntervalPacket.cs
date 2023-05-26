using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置脉冲间隔时间及脉冲个数请求包
    /// 0x41
    /// </summary>
    internal class CL188_RequestSetDemandIntervalPacket : Cl188SendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte Pos = 0;

        private int PulseSpaceTime = 0;

        private ushort PulseCount = 0;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位</param>
        /// <param name="psapce">脉冲间隔时间</param>
        /// <param name="pcount">脉冲个数</param>
        public void SetPara(byte pos, int psapce, ushort pcount)
        {
            Pos = pos;
            PulseSpaceTime = psapce;
            PulseCount = pcount;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetDemandIntervalPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x41);
            buf.Put(Pos);
            buf.PutInt(PulseSpaceTime);
            buf.PutUShort(PulseCount);
            return buf.ToByteArray();
        }
    }
}
