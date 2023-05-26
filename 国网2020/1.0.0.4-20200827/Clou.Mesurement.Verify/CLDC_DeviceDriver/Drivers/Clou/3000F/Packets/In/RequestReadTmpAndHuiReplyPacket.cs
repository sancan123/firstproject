using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读温度湿度回复包
    /// </summary>
    class RequestReadTmpAndHuiReplyPacket:ClouRecvPacket_CLT11
    {
        #region 变量声明
        private int iTemperature;
        /// <summary>
        /// 温度
        /// </summary>
        public int ITemperature
        {
            get { return iTemperature; }
            set { iTemperature = value; }
        }
        private int iHumidity;
        /// <summary>
        /// 湿度
        /// </summary>
        public int IHumidity
        {
            get { return iHumidity; }
            set { iHumidity = value; }
        }

        #endregion
        
        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                ITemperature = BitConverter.ToInt16(data, 4);
                IHumidity = BitConverter.ToUInt16(data, 6);

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
            return "RequestReadTmpAndHuiReplyPacket";
        }
    }
}
