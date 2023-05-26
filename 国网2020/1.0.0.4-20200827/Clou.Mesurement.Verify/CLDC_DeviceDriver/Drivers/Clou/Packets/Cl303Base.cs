using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// CL303数据包基类
    /// </summary>
    internal class Cl303Base : Base
    {
        public Cl303Base() : base() { }
        public Cl303Base(bool bReturn)
            : base(bReturn)
        {
        }
        /// <summary>
        /// 帧验证
        /// </summary>
        /// <returns></returns>
        protected override bool CheckRecvFrame()
        {
            if (LstPacketFrame.Count < 6) return false;
            if (LstPacketFrame[0] == m_PacketHead && LstPacketFrame[1] == m_ToID)
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
        /// <summary>
        /// 填充包头
        /// </summary>
        /// <param name="buf"></param>
        protected override void putHeader(ByteBuffer buf)
        {
            m_ToID = 0x20;
            buf.Put(m_PacketHead);
            buf.Put(m_ToID);
        }
        /// <summary>
        /// 组织发送帧
        /// </summary>
        /// <returns></returns>
        public override byte[] CreateSendFrame()
        {
            ByteBuffer buf = new ByteBuffer();
            putHeader(buf);                         //填充包头
            //计算包体长度
            ByteBuffer bodyBuf = new ByteBuffer();
            PutBody(bodyBuf);                       //包体
            byte[] body = bodyBuf.ToByteArray();
            byte packetLen = (byte)(body.Length + m_PacketHeaderLen);
            packetLen += 1;
            buf.PutUShort(packetLen);//填充包长度
            buf.Put(body);     //填充包体 
            encodePacket(buf);                      //数据加密
            LstPacketFrame.Clear();
            //InsertBytes(buf.ToByteArray()); 
            //Console.WriteLine("Send:" + BitConverter.ToString(LstPacketFrame.ToArray()));
            //return LstPacketFrame.ToArray();
            return buf.ToByteArray();
        }
        /// <summary>
        /// 将数字转换成10字节数据
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected byte[] get10bitData(Single Data)
        {
            string strdata = Data.ToString("D10");
            strdata = strdata.Substring(0, 10);
            byte[] d = ASCIIEncoding.ASCII.GetBytes(strdata);
            d[9] = 0x48;
            return d;
        }
    }
}
