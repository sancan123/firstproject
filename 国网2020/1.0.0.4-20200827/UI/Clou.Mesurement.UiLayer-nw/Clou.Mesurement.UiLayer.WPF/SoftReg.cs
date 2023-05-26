using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Security.Cryptography;
using System.IO;

namespace Mesurement.UiLayer.WPF
{
    class SoftReg
    {
        ///<summary>
        /// 获取硬盘卷标号
        ///</summary>
        ///<returns></returns>
        public string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc = new ManagementClass("win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        ///<summary>
        /// 获取CPU序列号
        ///</summary>
        ///<returns></returns>
        public string GetCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuCollection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
            }
            return strCpu;
        }

        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <returns></returns>
        public string GetHostName()
        {
            string strName = System.Net.Dns.GetHostName();
            string key = "BLUE2016";

            strName = MD5Encrypt(strName, key);

            return strName;
        }

        public string MD5Encrypt(string pToEncrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        ///<summary>
        /// 生成机器码
        ///</summary>
        ///<returns></returns>
        public string GetMNum()
        {
            string strMNum = GetCpu().PadLeft(8, '0').Substring(0, 8);   //cpu序列号
            strMNum += GetDiskVolumeSerialNumber().PadLeft(8, '0').Substring(0, 8); //硬盘序列号
            string strHostName = GetHostName();
            strMNum += strHostName.Substring(strHostName.Length - 8, 8);    //计算机名称（MD5加密后）
            return strMNum;
        }

        public int[] intCode = new int[127];    //存储密钥
        public char[] charCode = new char[25];  //存储ASCII码
        public int[] intNumber = new int[25];   //存储ASCII码值

        //初始化密钥
        public void SetIntCode()
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }

        ///<summary>
        /// 生成注册码 --地市级
        ///</summary>
        ///<returns></returns>
        public string GetRNumBy1(string strMNum)
        {
            SetIntCode();

            for (int i = 1; i < charCode.Length; i++)   //存储机器码
            {
                charCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)  //改变ASCII码值
            {
                intNumber[j] = Convert.ToInt32(charCode[j]) + intCode[Convert.ToInt32(charCode[j])];
            }
            string strAsciiName = "";   //注册码
            for (int k = 1; k < intNumber.Length; k++)  //生成注册码
            {

                if ((intNumber[k] >= 48 && intNumber[k] <= 57) || (intNumber[k] >= 65 && intNumber[k]
                    <= 90) || (intNumber[k] >= 97 && intNumber[k] <= 122))  //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[k]).ToString();
                }
                else if (intNumber[k] > 122)  //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }

        /// <summary>
        ///  生成注册码 --省级
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string GetRNumBy2(string strMNum)
        {
            string key = "CLOU2016";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }

        /// <summary>
        /// 生成注册码 --南网科研院
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string GetRNumBy3(string strMNum)
        {
            string key = "CLOU2017";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }

        ///<summary>
        /// 生成注册码 --地市级
        ///</summary>
        ///<returns></returns>
        public string GetRNumBy1()
        {
            string strMNum = GetMNum();
            SetIntCode();

            for (int i = 1; i < charCode.Length; i++)   //存储机器码
            {
                charCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)  //改变ASCII码值
            {
                intNumber[j] = Convert.ToInt32(charCode[j]) + intCode[Convert.ToInt32(charCode[j])];
            }
            string strAsciiName = "";   //注册码
            for (int k = 1; k < intNumber.Length; k++)  //生成注册码
            {

                if ((intNumber[k] >= 48 && intNumber[k] <= 57) || (intNumber[k] >= 65 && intNumber[k]
                    <= 90) || (intNumber[k] >= 97 && intNumber[k] <= 122))  //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[k]).ToString();
                }
                else if (intNumber[k] > 122)  //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }

        /// <summary>
        ///  生成注册码 --省级
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string GetRNumBy2()
        {
            string strMNum = GetMNum();
            string key = "CLOU2016";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }

        /// <summary>
        /// 生成注册码 --南网科研院
        /// </summary>
        /// <param name="strMNum"></param>
        /// <returns></returns>
        public string GetRNumBy3()
        {
            string strMNum = GetMNum();
            string key = "CLOU2017";
            string strRNum = MD5Encrypt(strMNum, key);
            if (strRNum.Length > 24)
            {
                strRNum = strRNum.Substring(strRNum.Length - 24, 24);
            }
            return strRNum;
        }

        ///<summary>
        /// 生成注册码
        ///</summary>
        ///<returns></returns>
        public string GetRNum()
        {
            SetIntCode();
            string strMNum = GetMNum();
            for (int i = 1; i < charCode.Length; i++)   //存储机器码
            {
                charCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)  //改变ASCII码值
            {
                intNumber[j] = Convert.ToInt32(charCode[j]) + intCode[Convert.ToInt32(charCode[j])];
            }
            string strAsciiName = "";   //注册码
            for (int k = 1; k < intNumber.Length; k++)  //生成注册码
            {

                if ((intNumber[k] >= 48 && intNumber[k] <= 57) || (intNumber[k] >= 65 && intNumber[k]
                    <= 90) || (intNumber[k] >= 97 && intNumber[k] <= 122))  //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[k]).ToString();
                }
                else if (intNumber[k] > 122)  //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }

    }
}
