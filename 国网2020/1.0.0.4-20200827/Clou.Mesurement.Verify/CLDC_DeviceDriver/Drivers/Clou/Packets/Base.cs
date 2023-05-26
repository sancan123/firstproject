using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 数据包基础类,默认支持CLT11协议
    /// </summary>
    internal abstract class Base : PackBase.Packet
    {
        #region --------变量声明--------
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
        #endregion

        public Base(bool breturn)
            : base(breturn)
        { }
        public Base() { }


        /// <summary>
        /// 是否是结论回复包
        /// </summary>
        public bool IsResultPacket
        {
            get { return m_IsResultPacket; }
            set { m_IsResultPacket = value; }
        }
        /// <summary>
        /// 获取包名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetPacketName()
        {
            return "Unknown packet";
        }

        protected override bool CheckRecvFrame()
        {
            if (LstPacketFrame.Count < 6) return false;
            //if (LstPacketFrame[0] == m_PacketHead && LstPacketFrame[1] == m_MyID && LstPacketFrame[2] == m_ToID)
            {
                //计算验证码
                byte[] bytData = LstPacketFrame.ToArray();
                byte[] bytData2 = new byte[bytData.Length - 1];
                Array.Copy(bytData, 0, bytData2, 0, bytData.Length - 1);
                int chkCode = GetChkSum(bytData2);
                return (chkCode == bytData[bytData.Length - 1]);
            }
            return false;
        }

        public override byte[] CreateSendFrame()
        {
            ByteBuffer buf = new ByteBuffer();
            putHeader(buf);                         //填充包头
            //计算包体长度
            ByteBuffer bodyBuf = new ByteBuffer();
            PutBody(bodyBuf);                       //包体
            byte[] body = bodyBuf.ToByteArray();
            byte packetLen = (byte)(body.Length + m_PacketHeaderLen+1);
            buf.Put(packetLen);//填充包长度
            buf.Put(body);     //填充包体 
            encodePacket(buf);                      //数据加密
            LstPacketFrame.Clear();
            //InsertBytes(buf.ToByteArray()); 
            //Console.WriteLine("Send:" + BitConverter.ToString(LstPacketFrame.ToArray()));
            //return LstPacketFrame.ToArray();
            return buf.ToByteArray();
        }
        /// <summary>
        /// 组织包头
        /// </summary>
        protected virtual void putHeader(ByteBuffer buf)
        {
            buf.Put(m_PacketHead);
            buf.Put(m_MyID);
            buf.Put(m_ToID);
            //buf.Put(0);
           // InsertByte(m_PacketHead);
           // InsertByte(m_ToID);
           // InsertByte(m_MyID);
           // InsertByte(0);      //0占位
        }
        /// <summary>
        /// 组织包体指令。不包括包头,从命令码开始，不用计算验证码
        /// </summary>
        protected virtual void PutBody(ByteBuffer buf)
        {

        }
        /// <summary>
        /// 数据加密
        /// </summary>
        protected virtual void encodePacket(ByteBuffer buf)
        {
            byte checkcode = GetChkSum(buf.ToByteArray());
            buf.Put(checkcode);
        }
        /// <summary>
        /// 计算检验码
        /// </summary>
        /// <param name="bytData"></param>
        /// <returns></returns>
        protected byte GetChkSum(byte[] bytData)
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
