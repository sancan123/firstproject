using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置电压/电流档位请求包
    /// </summary>
    internal class CL311_RequestSetUScalePacket : Cl311SendPacket
    {
        private float m_ua;
        private float m_ub;
        private float m_uc;
        /// <summary>
        /// 为TRUE时为设置电压
        /// </summary>
        public bool IsU = false;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="ua">A相电压档位</param>
        /// <param name="ub">B相电压档位</param>
        /// <param name="uc">C相电压档位</param>
        public void SetPara(float a, float b, float c, bool needConvert)
        {
            if (needConvert)
            {
                if (IsU)
                {
                    m_ua = GetUScaleIndex(a);
                    m_ub = GetUScaleIndex(b);
                    m_uc = GetUScaleIndex(c);
                }
                else
                {
                    m_ua = GetIScaleIndex(a);
                    m_ub = GetIScaleIndex(b);
                    m_uc = GetIScaleIndex(c);

                }
            }
            else
            {
                m_ua = a;
                m_ub = b;
                m_uc = c;
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetUScalePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x41);
            buf.Put((byte)m_ua);
            buf.Put((byte)m_ub);
            buf.Put((byte)m_uc);
            return buf.ToByteArray();
        }

        private int GetUScaleIndex(Single sng_U)
        {
            //0=1000V  1=600V 2=380V 3=220V 4=100V  5=60V 6=30V  15=自动档

            if (sng_U > 1000)           //超过1000V 则自动档
                return 15;
            else if (1000 >= sng_U && sng_U > 600)          // 1000V 档  1000---600V
                return 0;
            else if (600 >= sng_U && sng_U > 380)           // 600V 档  600---380V
                return 1;
            else if (380 >= sng_U && sng_U > 220 * 1.2)           // 380V 档  380---220V
                return 2;
            else if (220 * 1.2 >= sng_U && sng_U > 100 * 1.2)           // 220V 档  220---100V
                return 3;
            else if (100 * 1.2 >= sng_U && sng_U > 60 * 1.2)            // 100V 档  100---60V
                return 4;
            else if (60 * 1.2 >= sng_U && sng_U > 30 * 1.2)             // 60V 档  100---60V
                return 5;
            else if (30 * 1.2 >= sng_U)                           // 30V 档  100---60V
                return 6;
            else
                return 15;
        }
        private int GetIScaleIndex(Single sng_I)
        {
            //0=100A  1=50A  2=25A  3=10A 4=5A  5=2.5A  6=1A  7=0.5A 8=0.25A  9=0.1A  10=0.05A  11=0.025A  15=自动档
            if (sng_I > 120)                        //超过100A档，为自动档
                return 15;
            else if (120 >= sng_I && sng_I > 60)    //100A档范围内 120%   120---60
                return 0;
            else if (60 >= sng_I && sng_I > 30)     //50A档范围内 120%   60---30
                return 1;
            else if (30 >= sng_I && sng_I > 12)     //25A档范围内 120%   30---12
                return 2;
            else if (12 >= sng_I && sng_I > 6)      //10A档范围内 120%   12---6
                return 3;
            else if (6 >= sng_I && sng_I > 3)       //5A档范围内 120%   6---3
                return 4;
            else if (3 >= sng_I && sng_I > 1.2)       //2.5A档范围内 120%   3---1.2
                return 5;
            else if (1.2 >= sng_I && sng_I > 0.6)       //1A档范围内 120%   1.2---0.6
                return 6;
            else if (0.6 >= sng_I && sng_I > 0.3)       //0.5A档范围内 120%   0.6---0.3
                return 7;
            else if (0.3 >= sng_I && sng_I > 0.12)       //0.25A档范围内 120%   0.3---0.12
                return 8;
            else if (0.12 >= sng_I && sng_I > 0.06)       //0.1A档范围内 120%   0.12---0.06
                return 9;
            else if (0.06 >= sng_I && sng_I > 0.03)       //0.05A档范围内 120%   0.06---0.03
                return 10;
            else if (0.03 >= sng_I)                    //0.025A档范围内 120%   0.03---0
                return 11;
            else
                return 15;
        }
    }
}
