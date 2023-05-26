using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置电压跌落试验请求包0x36
    /// 返回包:CLNormalRequestResultReplayPacket
    /// </summary>
    internal class CL303_RequestSetVoltageFallOffPacket : CL303SendPacket
    {
        /// <summary>
        /// 试验方式
        /// </summary>
        public byte VerifyType = 0;

        public override string GetPacketName()
        {
            return "CL303_RequestSetVoltageFallOffPacket";
        }

        public CL303_RequestSetVoltageFallOffPacket()
        {
            this.ToID = 0x20;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="upara">电压参数</param>
        /// <param name="phipara"></param>
        public void SetPara(byte verifyType)
        {
            VerifyType = verifyType;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x36);
            buf.Put(VerifyType);
            return buf.ToByteArray();
        }

        //protected override byte FrameLengthByteCount
        //{
        //    get { throw new NotImplementedException(); }
        //}
    }
}
