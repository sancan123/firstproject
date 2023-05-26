using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// 非CLT11协议基类
    /// 比较CLT11协议，本协议帧头少了一个发信节点ID
    /// </summary>
    internal class BaseNoClt : Base
    {

        public BaseNoClt() : base() {
            m_PacketHeaderLen = 3;
        }
        public BaseNoClt(bool bReturn)
            : base(bReturn)
        {
            m_PacketHeaderLen = 3;
        }

        protected override bool CheckRecvFrame()
        {
            if (LstPacketFrame.Count < 6) return false;
            //if (LstPacketFrame[0] == m_PacketHead  && LstPacketFrame[1] == m_ToID)
            //{
            //计算验证码
            byte[] bytData = LstPacketFrame.ToArray();
            byte[] bytData2 = new byte[bytData.Length - 1];
            Array.Copy(bytData, 0, bytData2, 0, bytData.Length - 1);
            int chkCode = GetChkSum(bytData2);
            return (chkCode == bytData[bytData.Length - 1]);
            //}
            //return false;
        }

        protected override void putHeader(ByteBuffer buf)
        {
            buf.Put(m_PacketHead);
            buf.Put(m_ToID);
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
