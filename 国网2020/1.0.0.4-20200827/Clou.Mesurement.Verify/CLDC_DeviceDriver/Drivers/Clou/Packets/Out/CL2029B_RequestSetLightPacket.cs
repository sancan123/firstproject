using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 2029B设置警示灯请求包
    /// </summary>
    internal class CL2029B_RequestSetLightPacket : CL2029BSendPacket
    {
        public bool IsLink = true;
        /// <summary>
        /// 警示灯类型
        /// </summary>
        private int iLightType = 0;

        public CL2029B_RequestSetLightPacket()
            : base(false)
        {}

        public void SetPara(int iType)
        {
            this.iLightType = iType;
        }
        /*
         * 81 42 PCID 0B A3 02 01 01 0x xx CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x01);
            buf.Put(0x01);
            buf.Put(0x01);            
            buf.Put(Convert.ToByte(iLightType));
            
            return buf.ToByteArray();
        }
    }
}
