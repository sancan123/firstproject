using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V90.Packets.Out
{
    /// <summary>
    /// 设置检定点参数
    /// </summary>
    class RequestSetTestPointPacket : Base
    {
        private Comm.Enum.Cus_Clfs m_clfs;
        private float m_sng_Ub;
        private float m_sng_Ib;
        private float m_sng_IMax;
        private float m_sng_xUb_A;
        private float m_sng_xUb_B;
        private float m_sng_xUb_C;
        private float m_sng_xIb_A;
        private float m_sng_xIb_B;
        private float m_sng_xIb_C;
        private Comm.Enum.Cus_PowerYuanJiang m_element;
        private float m_sng_UaPhi;
        private float m_sng_UbPhi;
        private float m_sng_UcPhi;
        private float m_sng_IaPhi;
        private float m_sng_IbPhi;
        private float m_sng_IcPhi;
        private float m_sng_Freq;
        private bool m_bln_IsNxx;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="clfs"></param>
        /// <param name="sng_Ub"></param>
        /// <param name="sng_Ib"></param>
        /// <param name="sng_IMax"></param>
        /// <param name="sng_xUb_A"></param>
        /// <param name="sng_xUb_B"></param>
        /// <param name="sng_xUb_C"></param>
        /// <param name="sng_xIb_A"></param>
        /// <param name="sng_xIb_B"></param>
        /// <param name="sng_xIb_C"></param>
        /// <param name="element"></param>
        /// <param name="sng_UaPhi"></param>
        /// <param name="sng_UbPhi"></param>
        /// <param name="sng_UcPhi"></param>
        /// <param name="sng_IaPhi"></param>
        /// <param name="sng_IbPhi"></param>
        /// <param name="sng_IcPhi"></param>
        /// <param name="sng_Freq"></param>
        /// <param name="bln_IsNxx"></param>
        public void SetPara(Comm.Enum.Cus_Clfs clfs, float sng_Ub, float sng_Ib, float sng_IMax, float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A, float sng_xIb_B, float sng_xIb_C, Comm.Enum.Cus_PowerYuanJiang element, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi, float sng_IcPhi, float sng_Freq, bool bln_IsNxx)
        {
            m_clfs = clfs;
            m_sng_Ub = sng_Ub;
            m_sng_Ib = sng_Ib;
            m_sng_IMax = sng_IMax;
            m_sng_xUb_A = sng_xUb_A;
            m_sng_xUb_B = sng_xUb_B;
            m_sng_xUb_C = sng_xUb_C;
            m_sng_xIb_A = sng_xIb_A;
            m_sng_xIb_B = sng_xIb_B;
            m_sng_xIb_C = sng_xIb_C;
            m_element = element;
            m_sng_UaPhi = sng_UaPhi;
            m_sng_UbPhi = sng_UbPhi;
            m_sng_UcPhi = sng_UcPhi;
            m_sng_IaPhi = sng_IaPhi;
            m_sng_IbPhi = sng_IbPhi;
            m_sng_IcPhi = sng_IcPhi;
            m_sng_Freq = sng_Freq;
            m_bln_IsNxx = bln_IsNxx;
        }

        protected override void PutBody()
        {
        }
    }
}
