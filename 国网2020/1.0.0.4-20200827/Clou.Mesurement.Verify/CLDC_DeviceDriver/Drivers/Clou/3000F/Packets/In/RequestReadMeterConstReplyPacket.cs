using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读表常数回复包
    /// </summary>
    class RequestReadMeterConstReplyPacket:ClouRecvPacket_CLT11 
    {
        private ulong meterConst;
        /// <summary>
        /// 表常数
        /// </summary>
        public ulong MeterConst
        {
            get { return meterConst; }
            set { meterConst = value; }
        }

        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                this.meterConst = BitConverter.ToUInt64(data, 4);
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
            return "RequestReadMeterConstReplyPacket";
        }
    }
}
