using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// ox30指令升源
    /// </summary>
    internal class CL303_RequestSetPowerOnPacket0x30 : CL303SendPacket
    {
        /// <summary>
        /// 电压参数
        /// </summary>
        public struct UPara
        {
            public float Ua;
            public float Ub;
            public float Uc;
            public float Ia;
            public float Ib;
            public float Ic;
        }
        private UPara m_Upara;
        public struct PhiPara
        {
            public float PhiUa;
            public float PhiUb;
            public float PhiUc;
            public float PhiIa;
            public float PhiIb;
            public float PhiIc;
        }
        private PhiPara m_Phipara;
        private byte m_xwkz = 63;
        private byte m_xiebo = 0;
        private byte m_freq = 50;
        
        /// <summary>
        /// 谐波开关设置
        /// </summary>
        public bool OpenXieBo { set { m_xwkz = value ? (byte)0xFF : (byte)0; } }
        /// <summary>
        /// 设置频率,默认值50hz
        /// </summary>
        public byte Freq { set { m_freq = value; } get { return m_freq; } }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="upara">电压参数</param>
        /// <param name="phipara"></param>
        public void SetPara(byte clfs,UPara upara, PhiPara phipara)
        {
            m_Upara = upara;
            m_Phipara = phipara;
            if (clfs == 7)
                m_xwkz &= 9;
            else if (clfs == 6)
                m_xwkz &= 45;
        }

        public override string GetPacketName()
        {
            return "CL303_RequestSetPowerOnPacket0x30";
        }

        public CL303_RequestSetPowerOnPacket0x30()
        {
            this.ToID = 0x20;
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x30);
            buf.Put(m_xwkz);                //相位控制
            buf.Put(m_xiebo);               //谐波开关
            buf.Put(get10bitData(Freq));    //频率
            //UA
            buf.Put(GetUScale(m_Upara.Ua));       //
            buf.Put(get10bitData(m_Upara.Ua));    //
            buf.Put(get10bitData(m_Phipara.PhiUa)); //
            //Ub
            buf.Put(GetUScale(m_Upara.Ub));       //
            buf.Put(get10bitData(m_Upara.Ub));    //
            buf.Put(get10bitData(m_Phipara.PhiUb)); //

            //Uc
            buf.Put(GetUScale(m_Upara.Uc));       //
            buf.Put(get10bitData(m_Upara.Uc));    //
            buf.Put(get10bitData(m_Phipara.PhiUc)); //

            //Ia
            buf.Put(GetUScale(m_Upara.Ia));       //
            buf.Put(get10bitData(m_Upara.Ia));    //
            buf.Put(get10bitData(m_Phipara.PhiIa)); //
            //Ib
            buf.Put(GetUScale(m_Upara.Ib));       //
            buf.Put(get10bitData(m_Upara.Ib));    //
            buf.Put(get10bitData(m_Phipara.PhiIb)); //
            //Ic
            buf.Put(GetUScale(m_Upara.Ic));       //
            buf.Put(get10bitData(m_Upara.Ic));    //
            buf.Put(get10bitData(m_Phipara.PhiIc)); //
            if (buf.ToByteArray().Length != 139)
                throw new Exception(GetPacketName()+"数据包长度不对");

            return buf.ToByteArray();
        }
        private byte GetIScale(Single sngI)
        {
            if (sngI <= 0.012) return 80;            //"50";
            else if (sngI <= 0.03) return 81;        //"51";
            else if (sngI <= 0.06) return 82;        // "52";
            else if (sngI <= 0.12) return 83;       // "53";
            else if (sngI <= 0.3) return 84;        //"54";
            else if (sngI <= 0.6) return 85;        //"55";
            else if (sngI <= 1.2) return 86;        //"56";
            else if (sngI <= 3) return 87;          // "57";
            else if (sngI <= 6) return 88;          //"58";
            else if (sngI <= 12) return 89;         //"59";
            else if (sngI <= 30) return 90;         //"5a";
            else if (sngI <= 60) return 91;         //"5b";
            else if (sngI <= 120) return 92;        // "5c";
            else return 92;// "5c";
        }

        private byte GetUScale(Single sngU)
        {
            if (sngU <= 57 * 1.2) return 64;//"40";
            else if (sngU <= 120) return 65;// "41";
            else if (sngU <= 264) return 66;// "42";
            else if (sngU <= 480) return 67;//"43";
            else if (sngU <= 900) return 68;//"44";
            else return 66;// "42";
        }
    }
}
