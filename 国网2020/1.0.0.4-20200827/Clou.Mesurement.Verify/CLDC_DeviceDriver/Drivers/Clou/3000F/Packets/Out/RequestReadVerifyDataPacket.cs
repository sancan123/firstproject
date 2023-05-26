using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 读取功能数据1请求包
    /// </summary>
    class RequestReadVerifyDataPacket:ClouSendPacket_CLT11
    {
        /// <summary>
        /// 设置当前请求的功能数据次数，最大保存5次缓存数据
        /// </summary>
        public byte DataDesc = 1;
        /// <summary>
        /// 序号下标
        /// </summary>
        public int StartPos = 0;
        /// <summary>
        /// 读取数量
        /// </summary>
        public byte ReadCount = 6;

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            ReadCount = (byte)(ReadCount * 5);

            buf.Put(0xa5);
            buf.Put(0);

            buf.Put((byte)(0x20 + DataDesc));           //功能数据次数
            buf.PutUShort_S((ushort)StartPos);
            buf.Put(ReadCount);     

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestReadVerifyDataPacket";
        }
    }
}
