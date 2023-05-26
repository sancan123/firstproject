using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Mesurement.UiLayer.ViewModel.Const
{
   public class OperateFile
    {
        [DllImport("kernel32")]
        private extern static int WritePrivateProfileStringA(string segName, string keyName, string sValue, string fileName);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        /// <summary>
        /// 写配置文件数据
        /// </summary>
        /// <param name="sSection"></param>
        /// <param name="sKey"></param>
        /// <param name="sValue"></param>
        /// <param name="IniPath"></param>
        public static void WriteIni(string sSection, string sKey, string sValue, string IniPath)
        {
            WritePrivateProfileStringA(sSection, sKey, sValue, IniPath);
        }

        #region 读取INI文件string ReadInIString(string inifile, string Section, string Ident, string Default)
        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="inifile"></param>
        /// <param name="Section"></param>
        /// <param name="Ident"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        public static string ReadInIString(string inifile, string Section, string Ident, string Default)
        {
            try
            {
                inifile = GetPhyPath(inifile);
                if (System.IO.File.Exists(inifile) == false)
                {
                    System.IO.File.Create(inifile).Close();
                }

                Byte[] Buffer = new Byte[65535];
                int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), inifile);
                //必须设定0（系统默认的代码页）的编码方式，否则无法支持中文
                string s = Encoding.GetEncoding(0).GetString(Buffer);
                s = s.Substring(0, bufLen);
                return s.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据相对路径获取文件、文件夹绝对路径
        /// </summary>
        /// <param name="FileName">相对路径</param>   
        /// <returns></returns>
        public static string GetPhyPath(string FileName)
        {
            FileName = FileName.Replace('/', '\\');             //规范路径写法
            if (FileName.IndexOf(':') != -1) return FileName;   //已经是绝对路径了
            if (FileName.Length > 0 && FileName[0] == '\\') FileName = FileName.Substring(1);
            return string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), FileName);
        }


        #endregion

        
    }
}
