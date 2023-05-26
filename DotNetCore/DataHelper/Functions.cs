using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataHelper
{
    public class Functions
    {
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="inifile"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void IniWrite(string filePath, string section, string key, string value)
        {
            if (System.IO.File.Exists(filePath) == false)
                System.IO.File.Create(filePath).Close();

            Win32Api.WritePrivateProfileString(section, key, value, filePath);
        }

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public string IniRead(string filePath, string section, string key, string defValue)
        {
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Close();

            byte[] Buffer = new byte[65535];
            int bufLen = Win32Api.GetPrivateProfileString(section, key, defValue, Buffer, Buffer.GetUpperBound(0), filePath);
            //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
            string s = Encoding.GetEncoding(0).GetString(Buffer);
            s = s.Substring(0, bufLen);
            s = s.Replace("\0", "");
            return s.Trim();

        }

        /// <summary>
        /// 读取当前目录配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public string IniReadEx(string filePath, string section, string key, string defValue)
        {
            string path = string.Format("{0}/{1}", Directory.GetCurrentDirectory(), filePath);
            return IniRead(path, section, key, defValue);
        }

        /// <summary>
        /// 写入当前目录配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void IniWriteEx(string filePath, string section, string key, string value)
        {
            string path = string.Format("{0}/{1}", Directory.GetCurrentDirectory(), filePath);
            IniWrite(path, section, key, value);
        }



        /// <summary>
        /// 将字节进行二进制反转，主要用于645与698特征字转换
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        public byte BitRever(byte chr)
        {
            string bs = Convert.ToString(chr, 2).PadLeft(8, '0');
            string s = "";
            for (int i = 0; i < 8; i++)
            {
                s += bs[7 - i];
            }
            return Convert.ToByte(s, 2);
        }

        /// <summary>
        /// 将字节进行二进制反转，只要用于645与698特征字转换
        /// </summary>
        /// <param name="chr">转换的值</param>
        /// <param name="totalWidth">二进制长度</param>
        /// <returns></returns>
        public int BitRever(int chr, int totalWidth)
        {
            string bs = Convert.ToString(chr, 2).PadLeft(totalWidth, '0');
            string s = "";
            for (int i = 0; i < totalWidth; i++)
            {
                s += bs[totalWidth - 1 - i];
            }
            return Convert.ToInt32(s, 2);
        }
    }
}
