using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取版本号回复包
    /// </summary>
    class RequestReadVersionReplyPacket:ClouRecvPacket_CLT11
    {
        /// <summary>
        /// 设备型号
        /// </summary>
        private string EquipmenType=null;
        /// <summary>
        /// 版本号
        /// </summary>
        private string VersionNumber=null;


        protected override void ParseBody(byte[] data)
        {
            if(data[0]==0x39)
            {
                ByteBuffer buf = new ByteBuffer(data);
                buf.Get();
                byte[] ver = buf.ToByteArray();

                string version = System.Text.Encoding.ASCII.GetString(ver);

                EquipmenType = version.Substring(8, 9);
                VersionNumber = version.Substring(17, 5);

                ReciveResult=CLDC_Comm.SocketModule.Packet.RecvResult.OK ;
            }
            else if (data[0]==0x33)
            {
                ReciveResult=CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
            }
            else
            {
                ReciveResult=CLDC_Comm.SocketModule.Packet.RecvResult.NOCOMMAND;
            }
        }

        public override string GetPacketName()
        {
            return "RequestReadVersionReplyPacket";
        }
    }
}
