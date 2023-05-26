using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 标准表 通信包基类
    /// 标准表的帧结构与其它帧结构不一样
    /// </summary>
    internal abstract class Geny_RequestStdMeterPacket : GenySendPacket
    {
        /// <summary>
        /// 标准表类型
        /// </summary>
        public string StdMeterType
        {
            get;
            set;
        }

        /// <summary>
        /// 该类不需要，其它信息
        /// </summary>
        public Geny_RequestStdMeterPacket(string stdMeterType)
        {
            this.StdMeterType = stdMeterType;
        }

        /// <summary>
        /// 已重写，
        /// </summary>
        /// <returns></returns>
        public override byte[] GetPacketData()
        {
            List<byte> buf = new List<byte>(10);

            buf.Add(0x02);
            buf.Add(0xAA);

            buf.AddRange(this.GetBody());
            buf.Add(0xAA);
            if (buf.Count < 10)
            {
                throw new Exception(this.GetType().FullName + "GetPacketData");
            }
            return buf.ToArray();
        }
    }
}
