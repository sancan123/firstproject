using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 光电头状态选择
    /// 通讯选择： 
    /// 0x00表示选择一对一模式485通讯（默认模式）；
    /// 0X01表示选择奇数表位485通讯；
    /// 0X02表示选择偶数表位485通讯；
    /// 0x03表示选择一对一模式红外通讯；
    /// 0X04表示选择奇数表位红外通讯；
    /// 0X05表示选择偶数表位红外通讯；
    /// 0X06表示选择切换到485总线（电科院协议用）。
    /// </summary>
    internal class CL188L_RequestSelectLightStatusPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xAC;

        /// <summary>
        /// 0x00表示选择一对一模式485通讯（默认模式）；0X01表示选择奇数表位485通讯；0X02表示选择偶数表位485通讯；
        /// 0x03表示选择一对一模式红外通讯；0X04表示选择奇数表位红外通讯；0X05表示选择偶数表位红外通讯；0X06表示选择切换到485总线（电科院协议用）。
        /// </summary>
        private Cus_LightSelect selectType;

        public CL188L_RequestSelectLightStatusPacket(bool[] bwstatus)
            :base(false)
        {
            BwStatus = bwstatus;
            this.selectType = Cus_LightSelect.一对一模式485通讯;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="bwstatus">电表状态</param>
        /// <param name="selecttype">通讯选择</param>
        public void SetPara(bool[] bwstatus, Cus_LightSelect selecttype)
        {
            BwStatus = bwstatus;
            this.selectType = selecttype;
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectLightStatusPacket";
        }

        /*
         * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 通讯选择（1Byte）。
        */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put(Data1);
            buf.Put(0x0C);
            if (ChannelByte == null)
                return ChannelByte;
            else
                buf.Put(ChannelByte);
            buf.Put(Convert.ToByte((int)selectType));
            return buf.ToByteArray();
        }
    }
}
