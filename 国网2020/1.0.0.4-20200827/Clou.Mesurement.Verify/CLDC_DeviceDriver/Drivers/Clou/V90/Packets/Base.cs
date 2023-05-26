using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V90.Packets
{
    internal class Base : PackBase.Packet
    {
        /// <summary>
        /// 包头
        /// </summary>
        protected byte m_PacketHead = 0x81;
        /// <summary>
        /// 发信节点
        /// </summary>
        protected byte m_MyID = 0x80;
        /// <summary>
        /// 受信节点
        /// </summary>
        protected byte m_ToID = 0x10;
        /// <summary>
        /// 包头长度
        /// </summary>
        protected byte m_PacketHeaderLen = 4;
        /// <summary>
        /// 是否是结果回复包
        /// </summary>
        private bool m_IsResultPacket = false;
        /// <summary>
        /// 是否是初始化数据包
        /// </summary>
        protected bool isSettingPacket = false;

        public Base(bool breturn)
            : base(breturn)
        { }
        public Base()
        { }
        /// <summary>
        /// 是否是结论回复包
        /// </summary>
        public bool IsResultPacket
        {
            get { return m_IsResultPacket; }
            set { m_IsResultPacket = value; }
        }

        protected override bool CheckRecvFrame()
        {
            if (LstPacketFrame.Count < 6) return false;
            if (LstPacketFrame[0] == m_PacketHead && LstPacketFrame[1] == m_MyID && LstPacketFrame[2] == m_ToID)
            {
                //计算验证码
                byte[] bytData = LstPacketFrame.ToArray();
                byte[] bytData2 = new byte[bytData.Length - 1];
                Array.Copy(bytData, 0, bytData2, 0, bytData.Length - 1);
                int chkCode=GetChkSum(bytData2);
                return (chkCode == bytData[bytData.Length - 1]);
            }
            return false;
        }

        public override byte[] CreateSendFrame()
        {
            if (!isSettingPacket)
            {
                //组织包头
                putHeader();
                //组织包体
                PutBody();
                //加密包体
                encodePacket();
            }
            else
                PutBody();
            Console.WriteLine("Send:"+ BitConverter.ToString(LstPacketFrame.ToArray())); 
            return LstPacketFrame.ToArray();
        }
        /// <summary>
        /// 组织包头
        /// </summary>
        private void putHeader()
        {
            InsertByte(m_PacketHead);
            InsertByte(m_ToID);
            InsertByte(m_MyID);
            InsertByte(0);      //0占位
        }
        /// <summary>
        /// 组织包体指令。不包括包头,从命令码开始，不用计算验证码
        /// </summary>
        protected virtual void PutBody()
        {

        }
        /// <summary>
        /// 数据加密
        /// </summary>
        private void encodePacket()
        {
            byte[] bytData = LstPacketFrame.ToArray();
            byte packetLen = (byte)(bytData.Length + 1);
            EditByte(packetLen, m_PacketHeaderLen - 1); //加入数据长度
            bytData = bytData = LstPacketFrame.ToArray();
            byte chkSum = GetChkSum(bytData);
            Position = packetLen - 1;
            InsertByte(chkSum);
        }
        /// <summary>
        /// 计算检验码
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        private byte GetChkSum(byte[] bytData)
        {
            byte bytChkSum = 0;
            for (int int_Inc = 1; int_Inc < bytData.Length; int_Inc++)
            {
                bytChkSum ^= bytData[int_Inc];
            }
            return bytChkSum;
        }
    }
}
