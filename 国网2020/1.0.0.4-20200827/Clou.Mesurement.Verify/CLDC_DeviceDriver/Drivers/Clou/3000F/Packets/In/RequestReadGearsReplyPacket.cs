using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取档位回复包
    /// </summary>
    class RequestReadGearsReplyPacket:ClouRecvPacket_CLT11
    {
        #region 变量声明
        private byte gearUc;
        /// <summary>
        /// 交流表C相电压
        /// </summary>
        public byte GearUc
        {
            get { return gearUc; }
            set { gearUc = value; }
        }
        private byte gearUb;
        /// <summary>
        /// 交流表B相电压
        /// </summary>        
        public byte GearUb
        {
            get { return gearUb; }
            set { gearUb = value; }
        }
        private byte gearUa;
        /// <summary>
        /// 交流表A相电压
        /// </summary>
        public byte GearUa
        {
            get { return gearUa; }
            set { gearUa = value; }
        }
        private byte gearIc;
        /// <summary>
        /// 交流表C相电流
        /// </summary>
        public byte GearIc
        {
            get { return gearIc; }
            set { gearIc = value; }
        }
        private byte gearIb;
        /// <summary>
        /// 交流表B相电流
        /// </summary>        
        public byte GearIb
        {
            get { return gearIb ; }
            set { gearIb = value; }
        }

        private byte gearIa;
        /// <summary>
        /// 交流表A相电流
        /// </summary>
        public byte GearIa
        {
            get { return gearIa; }
            set { gearIa = value; }
        }

        #endregion

        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                GearUc = data[4];
                GearUb = data[5];
                GearUa = data[6];
                GearIc = data[7];
                GearIb = data[8];
                GearIa = data[9];

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
            return "RequestReadGearsReplyPacket";
        }
    }
}
