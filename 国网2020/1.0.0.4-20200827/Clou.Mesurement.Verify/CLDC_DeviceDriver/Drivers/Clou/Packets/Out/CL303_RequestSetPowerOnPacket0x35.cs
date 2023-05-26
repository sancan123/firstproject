using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 相同电流电压源输出指令
    /// 返回4B则成功
    /// </summary>
    internal class CL303_RequestSetPowerOnPacket0x35 : CL303SendPacket
    {
        private byte m_clfs = 0;
        private byte m_xwkz = 0;
        private bool m_xiebo = false;
        private float m_U = 0;
        private float m_I = 0;
        private bool m_isDuiSheBiao = false;
        private float m_phi = 0;
        private byte m_BuPingHeng = 0;//不平衡负载
        /// <summary>
        /// 设置频率，默认为50HZ
        /// </summary>
        public float Freq = 50;
        /// <summary>
        /// 电压百分比
        /// </summary>
        public float PercentOfU = 100F;
        /// <summary>
        /// 是否是潜动
        /// </summary>
        public bool IsCreeping = false;
        /// <summary>
        /// 元件
        /// </summary>
        public CLDC_Comm.Enum.Cus_PowerYuanJiang m_YuanJian = CLDC_Comm.Enum.Cus_PowerYuanJiang.H;

        /// <summary>
        /// 设置源控制参数
        /// </summary>
        /// <param name="clfs">
        /// 测量方式
        /// 0表示PT4       1表示QT4    2表示P32 
        /// 3表示Q32       4表示Q60    5表示Q90
        /// 6表示Q33       7表示P
        /// </param>
        /// <param name="xiebo"></param>
        /// <param name="U"></param>
        /// <param name="I"></param>
        /// <param name="duisebiao"></param>
        /// <param name="Phi"></param>
        public void SetPara(byte clfs, byte xwkz, bool xiebo,
                            float U, float I, bool duisebiao, float Phi)
        {
            m_clfs = clfs;
            m_isDuiSheBiao = duisebiao;
            m_xwkz = 63;
            m_xwkz = xwkz;
            m_xiebo = xiebo;
            m_U = U;
            m_I = I;
            m_phi = Phi;
            //计算不平衡负载
            updateClfs();       //更新测量方式字节 

        }

        /// <summary>
        /// 计算测量方式:
        /// </summary>
        private void updateClfs()
        {
            m_clfs = (byte)(m_isDuiSheBiao ? m_clfs ^ 128 : m_clfs & 127);  //对标标志
            m_clfs = (byte)(m_clfs & 191);                                  //缓降
            m_clfs = (byte)(m_clfs & 223);                                  //闭环
            //m_clfs = 0x00;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            /*
            接线方式          1Byte    	
            不平衡负载        1Byte    	
            波段开关          1 Byte   	
            谐波开关          1Byte    	
            电压档位          1 Byte	2008512 20090034
            电流档位          1 Byte	
            电压幅度          4 Byte	值×6553600
            电流幅度          4 Byte	值×6553600
            φ                4 Byte	值×6553600
            频率              4 Byte	值×6553600
            负载率            4 Byte	值×6553600

             */
            byte boduan = GetBoDuan();
            buf.Put(0x35);              //CMD    
            buf.Put(m_clfs);            //接线方式
            buf.Put(boduan);                 //只让加电流开关有效
            if (m_isDuiSheBiao)
                buf.Put(0x0f);
            else
                buf.Put(m_xwkz);            //相位开关 即ABC电压ABC电流，哪相输出哪相关闭
            buf.PutInt(0xFFFFFFFF);
            buf.PutUShort(0xFFFF);
            if (m_xiebo)
                buf.Put(0xFF);
            else
                buf.Put(0);
            buf.Put(GetUScale(m_U));
            if (m_isDuiSheBiao)
                buf.Put(86);
            else
                buf.Put(GetIScale(m_I));
            buf.PutInt((int)(m_U * 65536));
            buf.PutInt((int)(m_I * 65536 * 100));
            buf.PutInt((int)(m_phi * 65536));
            buf.PutInt((int)(Freq * 65536));
            buf.Put(0);
            buf.Put(100);
            buf.PutUShort(0);
            //81:20:00:26:35:01:18:3f:ff:ff:ff:ff:ff:ff:00:42:57:00:dc:00:00:00:96:00:00:00:5a:00:00:00:32:00:00:00:64:00:00:46
            //            35-00-18-3F-FF-FF-FF-FF-FF-FF-00-42-57-00-DC-00-00-00-96-00-00-00-5A-00-00-00-32-00-00-00-64-00-00
            return buf.ToByteArray();
        }
        /// <summary>
        /// 获取波段控制
        /// </summary>
        /// <returns></returns>
        private byte GetBoDuan()
        {
            byte Tb = 0;
            if (IsCreeping)
                Tb = 0x80;
            byte Tb2 = 0;
            if (PercentOfU == 0 || PercentOfU == 100)
                Tb2 = 0x10;
            else if (PercentOfU == 110)
                Tb2 = 0x20;
            else if (PercentOfU == 115)
                Tb2 = 0x30;
            else if (PercentOfU == 120)
                Tb2 = 0x40;
            Tb += Tb2;

            if (m_I > 0F)
                Tb += 8;
            Tb += (byte)(((byte)m_YuanJian) - 1);
            return Tb;
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
