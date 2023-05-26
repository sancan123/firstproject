using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置电能指示
    /// </summary>
    internal class CL3115_RequestSetStdMeterUsE1typePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterUsE1typePacket()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(CLDC_Comm.Enum.Cus_PowerFangXiang glfx)
        {            
            if (glfx == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功 || glfx == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功)
                _SetData = 0x00;
            else
                _SetData = 0x40;                    
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put(0x11);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
    }
}
