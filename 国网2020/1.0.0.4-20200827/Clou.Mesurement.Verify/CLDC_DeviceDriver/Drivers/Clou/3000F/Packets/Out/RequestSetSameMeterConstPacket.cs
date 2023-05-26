using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置统一表常数
    /// </summary>
    class RequestSetSameMeterConstPacket : ClouSendPacket_CLT11
    {
        private int constNum;
        /// <summary>
        /// 表常数
        /// </summary>
        public int ConstNum
        {
            get { return constNum; }
            set { constNum = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x08);
            buf.PutInt_S(constNum*100);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetSameMeterConstPacket";
        }
    }
}
