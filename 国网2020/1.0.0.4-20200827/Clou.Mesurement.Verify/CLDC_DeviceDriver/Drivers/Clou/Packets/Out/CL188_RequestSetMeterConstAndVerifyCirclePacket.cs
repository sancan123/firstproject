using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置被检表脉冲常数及校验圈数请求包
    /// </summary>
    internal class CL188_RequestSetMeterConstAndVerifyCirclePacket : Cl188SendPacket
    {

        private byte m_Pos = 0;
        private bool m_isSameMeterConst = true;
        private bool m_isSameCircle = true;

        private int[] m_arrMeterConst = new int[0];
        private int[] m_arrCircle = new int[0];
        private int m_MeterConst = 0;
        private int m_Circle = 0;

        public CL188_RequestSetMeterConstAndVerifyCirclePacket()
            : base(false)
        { }

        /// <summary>
        /// 统一设置被检表常数及检验圈数
        /// </summary>
        /// <param name="Pos">表位</param>
        /// <param name="meterconst">表常数</param>
        /// <param name="circle">检定圈数</param>
        public void SetPara( int meterconst, int circle)
        {
            m_isSameMeterConst = true;
            m_isSameCircle = true;
            //m_MeterConst = meterconst*100;
            m_MeterConst = meterconst;
            m_Circle = circle;
        }

        /// <summary>
        /// 设置被检表常数及检验圈数
        /// </summary>
        /// <param name="meterconst">表常数</param>
        /// <param name="circle">检定圈数</param>
        public void SetPara(byte Pos,int[] meterconst, int[] circle)
        {
            m_isSameMeterConst = false;
            m_isSameCircle = false;
            m_Pos = Pos;
            m_arrMeterConst = meterconst;
            m_arrCircle = circle;
            if (meterconst.Length != circle.Length)
                throw new Exception("meterconst 和 circle 数组长度不一致");
            //m_isSameMeterConst = isSameArray(meterconst);
            //m_isSameCircle = isSameArray(circle);
            if (m_isSameCircle && m_isSameMeterConst)
            {
                //m_MeterConst = meterconst[0]*100;
                m_MeterConst = meterconst[0];
                m_Circle = circle[0];
            }
        }

        public override string GetPacketName()
        {
            return "CL188_RequestSetMeterConstAndVerifyCirclePacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (m_isSameMeterConst && m_isSameCircle)
            {
                buf.Put(0x33);
                buf.PutInt(m_MeterConst);
                buf.PutInt(m_Circle);
            }
            else
            {
                buf.Put(0x32);
                buf.Put(m_Pos);
                for (int i = 0; i < m_arrMeterConst.Length; i++)
                {
                    buf.PutInt(m_arrMeterConst[i]);
                    //buf.PutInt(m_arrMeterConst[i]*100);
                    buf.PutInt(m_arrCircle[i]);
                }
            }
            return buf.ToByteArray();
        }
    }
}
