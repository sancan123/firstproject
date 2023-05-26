using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置本机常数
    /// </summary>
    internal class CL3115_RequestSetStdMeterConstPacket : CL3115SendPacket
    {
        /// <summary>
        /// 本机常数，4字节，低字节先传
        /// </summary>
        private int stdMeterConst;

        public CL3115_RequestSetStdMeterConstPacket(int meterconst)
            : base()
        {
            stdMeterConst = meterconst;
        }

        /// <summary>
        /// 设置本机常数
        /// </summary>
        /// <param name="meterconst">本机常数</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterConstPacket(int meterconst,bool needReplay)
            : base(needReplay)
        {
            stdMeterConst = meterconst;
        }

        /*
         * 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x04);
            buf.Put(0x01);
            buf.PutInt_S(stdMeterConst);
            return buf.ToByteArray();
        }
    }
}
