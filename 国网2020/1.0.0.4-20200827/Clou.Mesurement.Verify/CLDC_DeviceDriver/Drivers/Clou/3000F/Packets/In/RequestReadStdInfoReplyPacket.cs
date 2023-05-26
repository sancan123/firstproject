using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.In
{
    /// <summary>
    /// 读取标准数据回复包
    /// </summary>
    class RequestReadStdInfoReplyPacket:ClouRecvPacket_CLT11 
    {
        #region ----------电压/电流参数----------
        private float m_Ua = 0F;
        public float Ua
        {
            get { return m_Ua; }
            set { m_Ua = value; }
        }
        private float m_Ub = 0F;

        public float Ub
        {
            get { return m_Ub; }
            set { m_Ub = value; }
        }
        private float m_Uc = 0F;

        public float Uc
        {
            get { return m_Uc; }
            set { m_Uc = value; }
        }
        private float m_Ia = 0F;

        public float Ia
        {
            get { return m_Ia; }
            set { m_Ia = value; }
        }
        private float m_Ib = 0F;

        public float Ib
        {
            get { return m_Ib; }
            set { m_Ib = value; }
        }
        private float m_Ic = 0F;

        public float Ic
        {
            get { return m_Ic; }
            set { m_Ic = value; }
        }
        #endregion

        private float m_Feq = 0F;
        /// <summary>
        /// 频率
        /// </summary>
        public float Feq
        {
            get { return m_Feq; }
            set { m_Feq = value; }
        }

        private float m_PPhi = 0F;
        /// <summary>
        /// 交流表功率相角
        /// </summary>
        public float PPhi
        {
            get { return m_PPhi; }
            set { m_PPhi = value; }
        }
        #region --------功率因数--------
        private float m_CosH = 0F;

        public float CosH
        {
            get { return m_CosH; }
            set { m_CosH = value; }
        }
        private float m_CosA = 0F;

        public float CosA
        {
            get { return m_CosA; }
            set { m_CosA = value; }
        }
        private float m_CosB = 0F;

        public float CosB
        {
            get { return m_CosB; }
            set { m_CosB = value; }
        }
        private float m_CosC = 0F;

        public float CosC
        {
            get { return m_CosC; }
            set { m_CosC = value; }
        }

        #endregion

        #region --------交流表角度---------
        private float m_PhiA = 0F;

        public float PhiA
        {
            get { return m_PhiA; }
            set { m_PhiA = value; }
        }
        private float m_PhiB = 0F;

        public float PhiB
        {
            get { return m_PhiB; }
            set { m_PhiB = value; }
        }
        private float m_PhiC = 0F;

        public float PhiC
        {
            get { return m_PhiC; }
            set { m_PhiC = value; }
        }

        #endregion

        #region --------基波相位---------
        private float m_BasicPhiUa = 0F;

        public float BasicPhiUa
        {
            get { return m_BasicPhiUa; }
            set { m_BasicPhiUa = value; }
        }
        private float m_BasicPhiUb = 0F;

        public float BasicPhiUb
        {
            get { return m_BasicPhiUb; }
            set { m_BasicPhiUb = value; }
        }
        private float m_BasicPhiUc = 0F;

        public float BasicPhiUc
        {
            get { return m_BasicPhiUc; }
            set { m_BasicPhiUc = value; }
        }
        private float m_BasicPhiIa = 0F;

        public float BasicPhiIa
        {
            get { return m_BasicPhiIa; }
            set { m_BasicPhiIa = value; }
        }
        private float m_BasicPhiIb = 0F;

        public float BasicPhiIb
        {
            get { return m_BasicPhiIb; }
            set { m_BasicPhiIb = value; }
        }
        private float m_BasicPhiIc = 0F;

        public float BasicPhiIc
        {
            get { return m_BasicPhiIc; }
            set { m_BasicPhiIc = value; }
        }

        #endregion

        #region --------总有功率---------
        private float m_PTotal = 0F;

        public float PTotal
        {
            get { return m_PTotal; }
            set { m_PTotal = value; }
        }
        private float m_PATotal = 0F;

        public float PATotal
        {
            get { return m_PATotal; }
            set { m_PATotal = value; }
        }
        private float m_PBTotal = 0F;

        public float PBTotal
        {
            get { return m_PBTotal; }
            set { m_PBTotal = value; }
        }
        private float m_PCTotal = 0F;

        public float PCTotal
        {
            get { return m_PCTotal; }
            set { m_PCTotal = value; }
        }
        #endregion
        #region --------总无功功率-------
        private float m_QTotal = 0F;

        public float QTotal
        {
            get { return m_QTotal; }
            set { m_QTotal = value; }
        }
        private float m_QATotal = 0F;

        public float QATotal
        {
            get { return m_QATotal; }
            set { m_QATotal = value; }
        }
        private float m_QBTotal = 0F;

        public float QBTotal
        {
            get { return m_QBTotal; }
            set { m_QBTotal = value; }
        }
        private float m_QCTotal = 0F;

        public float QCTotal
        {
            get { return m_QCTotal; }
            set { m_QCTotal = value; }
        }

        #endregion
        #region --------总视在功率-------
        private float m_STotal = 0F;

        public float STotal
        {
            get { return m_STotal; }
            set { m_STotal = value; }
        }
        private float m_SATotal = 0F;

        public float SATotal
        {
            get { return m_SATotal; }
            set { m_SATotal = value; }
        }
        private float m_SBTotal = 0F;

        public float SBTotal
        {
            get { return m_SBTotal; }
            set { m_SBTotal = value; }
        }
        private float m_SCTotal = 0F;

        public float SCTotal
        {
            get { return m_SCTotal; }
            set { m_SCTotal = value; }
        }

        #endregion


        protected override void ParseBody(byte[] data)
        {
            if (data[0] == 0x50)
            {
                ByteBuffer buf = new ByteBuffer();
                buf.GetInt();
                int pos = buf.Position;
                Uc = buf.GetIntE1();
                Ub = buf.GetIntE1();
                Ua = buf.GetIntE1();
                Ic = buf.GetIntE1();
                Ib = buf.GetIntE1();
                Ia = buf.GetIntE1();
                //电压断言
                if ((buf.Position - pos) != 30)
                    throw new Exception("pos error");
                pos = buf.Position;
                Feq = buf.GetInt_S() / 10000F;
                buf.Get();// 去掉0x08
                PPhi = buf.GetInt_S() / 1000F;
                buf.Get();//去掉0x3f 
                if ((buf.Position - pos) != 10)
                    throw new Exception("pos error");
                BasicPhiUc = buf.GetInt_S() / 1000F;
                BasicPhiUb = buf.GetInt_S() / 1000F;
                BasicPhiUa = buf.GetInt_S() / 1000F;
                BasicPhiIc = buf.GetInt_S() / 1000F;
                BasicPhiIb = buf.GetInt_S() / 1000F;
                BasicPhiIa = buf.GetInt_S() / 1000F;
                buf.Get();//0xff 
                PhiC = buf.GetInt_S() / 1000F;
                PhiB = buf.GetInt_S() / 1000F;
                PhiA = buf.GetInt_S() / 1000F;
                CosC = buf.GetInt_S() / 10000F;
                CosB = buf.GetInt_S() / 10000F;
                CosA = buf.GetInt_S() / 10000F;
                CosH = buf.GetInt_S() / 10000F;
                buf.Get();//0xff 
                PCTotal = buf.GetIntE1();
                PBTotal = buf.GetIntE1();
                PATotal = buf.GetIntE1();
                PTotal = buf.GetIntE1();
                QCTotal = buf.GetIntE1();
                QBTotal = buf.GetIntE1();
                QATotal = buf.GetIntE1();
                QTotal = buf.GetIntE1();
                buf.Get();// ox0f
                SCTotal = buf.GetIntE1();
                SBTotal = buf.GetIntE1();
                SATotal = buf.GetIntE1();
                STotal = buf.GetIntE1();

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
            return "RequestReadStdInfoReplyPacket";
        }
    }
}
