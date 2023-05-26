using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读GPS时间回复包
    /// </summary>
    class RequestReadGpsTimeReplyPacket:ClouRecvPacket_CLT11
    {
        #region 变量声明
        private int iYear;
        /// <summary>
        /// 年
        /// </summary>
        public int IYear
        {
            get { return iYear; }
            set { iYear = value; }
        }
        private int iMonth;
        /// <summary>
        /// 月
        /// </summary>
        public int IMonth
        {
            get { return iMonth; }
            set { iMonth = value; }
        }
        private int iDay;
        /// <summary>
        /// 日
        /// </summary>
        public int IDay
        {
            get { return iDay; }
            set { iDay = value; }
        }
        private int iHour;
        /// <summary>
        /// 时
        /// </summary>
        public int IHour
        {
            get { return iHour; }
            set { iHour = value; }
        }
        private int iMinute;
        /// <summary>
        /// 分
        /// </summary>
        public int IMinute
        {
            get { return iMinute; }
            set { iMinute = value; }
        }
        private int iSecond;
        /// <summary>
        /// 秒
        /// </summary>
        public int ISecond
        {
            get { return iSecond; }
            set { iSecond = value; }
        }

        #endregion
        
        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                ISecond = data[4];
                IMonth=data[5];
                IHour=data[6];
                IDay = data[7];
                IMonth=data[8];
                IYear = BitConverter.ToUInt16(data, 9);

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
            return "RequestReadGpsTimeReplyPacket";
        }
    }
}
