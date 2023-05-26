using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 请求启动标准表指令包
    /// 返回0x4b成功
    /// </summary>
    internal class CL3115_RequestStartTaskPacket : CL3115SendPacket
    {
        /// <summary>
        /// 控制类型 0，停止；1，开始计算电能误差；2，开始计算电能走字
        /// </summary>
        /// <param name="iType"></param>
        public CL3115_RequestStartTaskPacket(int iType)
            : base()
        {
            this.iControlType = iType;
        }
        /// <summary>
        /// 控制类型 0，停止；1，开始计算电能误差；2，开始计算电能走字
        /// </summary>
        private int iControlType;
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x10);
            buf.Put(Convert.ToByte(iControlType));
            return buf.ToByteArray();
        }
    }
}
