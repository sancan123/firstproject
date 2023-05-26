using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2029B切换继电器请求包
    /// </summary>
    internal class CL2029B_RequestSetSwitchPacket : CL2029BSendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 继电器类型
        /// </summary>
        private bool bSwitchType = false;

        public CL2029B_RequestSetSwitchPacket()
            : base(false)
        {}

        public void SetPara(bool bType)
        {
            this.bSwitchType = bType;
        }
        /*
         * 81 42 PCID 0B A3 02 01 01 0x xx CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);
            if (bSwitchType)
                buf.Put(0x03);
            else
                buf.Put(0x00);
            
            return buf.ToByteArray();
        }
    }
}
