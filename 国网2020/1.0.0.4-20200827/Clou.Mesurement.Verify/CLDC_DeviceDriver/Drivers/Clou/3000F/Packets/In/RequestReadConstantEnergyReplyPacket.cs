using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取走字累计电量回复包
    /// </summary>
    class RequestReadConstantEnergyReplyPacket:ClouRecvPacket_CLT11
    {
        private int constantEnergy;
        /// <summary>
        /// 走字累计电量
        /// </summary>
        public int ConstantEnergy
        {
            get { return constantEnergy; }
            set { constantEnergy = value; }
        }

        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                ConstantEnergy = BitConverter.ToInt32(data, 4)*1000;
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
            }
            else if (data[0] == 0x33)
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
            }
            else
            {
                ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.Unknow;
            }
        }

        public override string GetPacketName()
        {
            return "RequestReadConstantEnergyReplyPacket";
        }
    }
}
