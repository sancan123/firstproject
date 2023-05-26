using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 置标准表界面
    /// 由于谐波数据和波形数据仅在对应界面下获取，读取谐波数据和波形数据前必须将界面切到对应界面
    /// 界面设置命令在界面切换过程中享有最高优先级，因此为不影响上位机和使用人员的正常操作
    /// 在不需读取谐波数据和波形数据后，将界面设置为清除上位机设置。
    /// </summary>
    internal class CL3115_RequestSetStdMeterScreenPacket : CL3115SendPacket
    {
        /// <summary>
        /// 标准表界面指示
        /// </summary>
        public Cus_StdMeterScreen meterScreen;

        /// <summary>
        /// 设置标准表界面
        /// </summary>
        /// <param name="meterscreen">标准表界面指示</param>
        public CL3115_RequestSetStdMeterScreenPacket(Cus_StdMeterScreen meterscreen)
            : base()
        {
            meterScreen = meterscreen;
        }


        /*
         * 81 30 PCID 0a a3 00 10 80 ucARM_Menu CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put((byte)meterScreen);
            return buf.ToByteArray();
        }
    }
}
