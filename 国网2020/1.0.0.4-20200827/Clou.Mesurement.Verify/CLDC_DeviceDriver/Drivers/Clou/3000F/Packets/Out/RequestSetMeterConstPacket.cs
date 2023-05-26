using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置表常数
    /// </summary>
    class RequestSetMeterConstPacket : ClouSendPacket_CLT11
    {
        private byte[] constNum;
        /// <summary>
        /// 表常数
        /// </summary>
        public byte[] ConstNum
        {
            get { return constNum; }
            set { constNum = value; }
        }

        /// <summary>
        /// 起始位置
        /// </summary>
        private int StartPos = 0;

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa6);
            buf.Put(0x00);
            buf.Put(0x1b);
            buf.PutInt_S(StartPos);
            buf.PutInt_S(constNum.Length * 4);

            for (int i = 0; i < constNum.Length; i++)
            {
                buf.PutInt_S(constNum[i]);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetMeterConstPacket";
        }
    }
}
