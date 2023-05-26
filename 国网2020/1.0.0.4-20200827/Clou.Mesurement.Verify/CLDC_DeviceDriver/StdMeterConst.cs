using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver
{
    /// <summary>
    /// 标准表常数查表
    /// 查表原则:
    /// 如果在非临界电压电流区域直接按常数表查询返回
    /// 如果在临界电压或临界电流区域，首先查询本次电压和上次电压是否相同，如果相同则返回上
    /// 次查询结果，如果不同则返回0，提示客户端需要发送指令给标准表读取标准表常数
    /// </summary>
    internal class StdMeterConst
    {
        /// <summary>
        /// 第一维为电流，第二维为电压
        /// </summary>
        /// 
        private static Dictionary<string, int> dicStdConstSheet = new Dictionary<string, int>();
        private static int[] arrU = new int[5] { 60, 100, 220, 380, 1000 };
        private static int[] arrI = new int[5] { 1, 5, 10, 50, 100 };
        public static float m_LastSearchU = 0F;        //上一次查询的电压
        public static float m_LastSearchI = 0F;        //上一次查询的电流
        public static int m_StdMeterConst = 0;         //上一次查询标准表常数
        static StdMeterConst()
        {
            int[] consts = new int[25]{
            (int)1.2*100000000,(int)2.4*10000000,(int)1.2*10000000,(int)2.4*1000000,(int)1.2*1000000,
            6*10000000,(int)1.2*10000000,6*1000000,(int)1.2*1000000,6*100000,
                3*10000000,6*1000000,3*1000000,6*100000,3*100000,
                (int)1.5*10000000,3*1000000,(int)1.5*1000000,3*100000,(int)1.5*100000,
                6*1000000,(int)1.2*1000000,6*100000,(int)1.2*100000,6*10000
            };
            string strKey = "";
            for (int i = 0; i < arrU.Length; i++)
            {
                for (int j = 0; j < arrI.Length; j++)
                {
                    strKey = string.Format("{0}{1}", arrU[i], arrI[j]);
                    dicStdConstSheet.Add(strKey, consts[i * 5 + j]);
                }
            }
        }
        /// <summary>
        /// 查表
        /// </summary>
        /// <param name="u"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static int SearchStdMeterConst(float sngU, float sngI)
        {
            bool bFound = false;
            int meterconst = 0;
            //查询是否是临界点
            if (IsCriticalValue(sngU, sngI))
            {
                //如果是临界点，则查询本次是否和上一次的电压电流一样
                if (sngU == m_LastSearchU && sngI == m_LastSearchI && m_StdMeterConst!=0)
                    meterconst = m_StdMeterConst;
                return meterconst;
            }
            for (int i = 0; i < arrU.Length; i++)
            {
                if (sngU < arrU[i] * 1.2F)
                {
                    for (int j = 0; j < arrI.Length; j++)
                    {
                        if (sngI < arrI[j] * 1.2F)
                        {
                            string strKey = string.Format("{0}{1}", arrU[i], arrI[j]);
                            if (dicStdConstSheet.ContainsKey(strKey))
                            {
                                meterconst = dicStdConstSheet[strKey]; 
                                bFound = true;
                                break;
                            }
                            else
                            {
                                throw new Exception("the key is not found"); 
                            }
                        }
                    }
                }
                if (bFound) break;
            }
            return meterconst;
        }

        public static bool IsCriticalValue(float sngU, float sngI)
        {
            //首先检测电压是否临界
            bool isCritical = false;
            float tmp = 0;
            for (int i = 0; i < arrU.Length; i++)
            {
                 tmp=arrU[i] * 1.2F;
                if (sngU == tmp)
                {
                    isCritical = true;
                    break;
                }
            }
            if (isCritical) return true;
            //检测电流是否临界
            for (int i = 0; i < arrI.Length; i++)
            {
                tmp=arrI[i] * 1.2F;
                if (sngI * 1000000F == tmp * 1000000F)
                {
                    isCritical = true;
                    break;
                }
            }
            return isCritical;
        }
    }
}
