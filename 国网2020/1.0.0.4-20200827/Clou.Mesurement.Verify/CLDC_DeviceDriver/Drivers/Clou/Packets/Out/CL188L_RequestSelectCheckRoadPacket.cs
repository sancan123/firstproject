using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 用于双回路检定时，选择其中的某一路作为电流的输出回路
    /// </summary>
    internal class CL188L_RequestSelectCheckRoadPacket :Cl188LSendPacket
    {
       /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0xAF;

        /// <summary>
        /// 电流的输出回路
        /// </summary>
        private Cus_BothIRoadType iRoad;

        /// <summary>
        /// 电压回路选择
        /// </summary>
        private Cus_BothVRoadType vRoad;

        public CL188L_RequestSelectCheckRoadPacket()
            :base(false)
        {}

        public CL188L_RequestSelectCheckRoadPacket(bool[] bwstatus)
            : base(false)
        {
            this.iRoad = Cus_BothIRoadType.第一个电流回路;
            this.vRoad = Cus_BothVRoadType.直接接入式;
        }

        /// <summary>
        /// 用于双回路检定时，选择其中的某一路作为电流的输出回路；0x00表示第一个电流回路，0x01表示第二个电流回路，系统默认为第一个电流回路。
        /// 选择电压回路时，0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入，系统默认为直接接入式电表电压回路。
        /// </summary>
        /// <param name="bwstatus"></param>
        /// <param name="iroad"></param>
        /// <param name="vroad"></param>
        public CL188L_RequestSelectCheckRoadPacket(bool[] bwstatus, Cus_BothIRoadType iroad, Cus_BothVRoadType vroad)
            : base(false)
        {
            this.iRoad = iroad;
            this.vRoad = vroad; 
        }

        public override string GetPacketName()
        {
            return "CL188L_RequestSelectCheckRoadPacket";
        }

        /// <summary>
        /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 电流回路路数（1Byte） + 电压回路路数（1Byte）。
        /// </summary>
        /// <returns></returns>
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
            buf.Put(Convert.ToByte((int)iRoad));
            buf.Put(Convert.ToByte((int)vRoad));
            return buf.ToByteArray();
        }
    }
}
