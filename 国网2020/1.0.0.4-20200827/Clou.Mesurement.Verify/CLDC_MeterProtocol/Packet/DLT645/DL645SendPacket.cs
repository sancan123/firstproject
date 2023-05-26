using System;
using System.Collections.Generic;
using System.Text;
using SocketModule.Packet;

namespace MeterProtocol.Packet.DLT645
{
    /// <summary>
    /// 645协议发送包
    /// </summary>
     class DL645SendPacket : SendPacket
    {
        const byte packetHeader = 0x68;
        const byte packetFooter = 0x16;
        private string meterAddress = string.Empty;

        public DL645SendPacket()
        {
            Data = new List<byte[]>();
            IsNeedReturn = true;
        }

        /// <summary>
        /// 前导FE数量
        /// </summary>
        public int FECount { get; set; }
        /// <summary>
        /// 电表地址
        /// </summary>
        public string Address
        {
            get { return meterAddress; }
            set
            {
                meterAddress = value;
                meterAddress = meterAddress.PadLeft(12, '0');
                meterAddress = meterAddress.Substring(0, 12);
            }
        }
        /// <summary>
        /// 控制码
        /// </summary>
        public byte CmdCode { get; set; }
        /// <summary>
        /// 数据标识
        /// </summary>
        //public byte[] DataMark { get; set; }
        /// <summary>
        /// 数据域,每一段添加一次
        /// </summary>
        public List<byte[]> Data { get; set; }
        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            //填充FE
            for (int i = 0; i < FECount; i++)
                buf.Put(0xFE);
            //包头
            buf.Put(packetHeader);
            //地址

            byte[] addr = Util.Functions.String2BCDCode(Address);
            Array.Reverse(addr);//反转
            buf.Put(addr);
            //第二个68
            buf.Put(packetHeader);
            //控制码
            buf.Put(CmdCode);
            //数据长度
            ByteBuffer databuf = new ByteBuffer();
            foreach (byte[] arrdata in Data)
            {
                //每一段加33H并反转
                byte[] newData = Util.Functions.Add33H(arrdata);
                Array.Reverse(newData);
                databuf.Put(newData);
            }
            buf.Put((byte)databuf.Length);
            buf.Put(databuf.ToByteArray());
            //校验码\
            byte[] data = buf.ToByteArray();
            byte chkCode = Util.Functions.GetChkSum(data, FECount, data.Length - FECount);
            buf.Put(chkCode);
            buf.Put(packetFooter);
            return buf.ToByteArray();
        }


    }
}
