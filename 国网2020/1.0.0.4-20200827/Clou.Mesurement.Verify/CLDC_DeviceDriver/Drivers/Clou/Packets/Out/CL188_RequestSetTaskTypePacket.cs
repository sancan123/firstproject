using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置试验功能类型请求包
    /// </summary>
    internal class CL188_RequestSetTaskTypePacket : Cl188SendPacket
    {
        private TaskType tType = TaskType.电能误差;
        /// <summary>
        /// 试验类型
        /// </summary>
        public enum TaskType
        {
            电能误差 = 0,
            需量周期 = 1,
            时钟日误差 = 2,
            脉冲计数 = 3,
            电流开路 = 4,
            电流接触电阻 = 5,
            电压短路 = 6,
        }

        public CL188_RequestSetTaskTypePacket()
            : base(false)
        {

        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        public void SetPara(byte pos, TaskType type)
        {
            Pos = pos;
            tType = type;
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetTaskTypePacket";
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x47);
            buf.Put(Pos);
            buf.Put((byte)tType);
            return buf.ToByteArray();
        }
    }
}
