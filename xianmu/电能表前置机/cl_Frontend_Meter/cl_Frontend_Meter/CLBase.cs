using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace cl_Frontend_Meter
{

    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;

        /// <summary>
        /// 从System.DateTime转换。
        /// </summary>
        /// <param name="time">System.DateTime类型的时间。</param>
        public void FromDateTime(DateTime time)
        {
            wYear = (ushort)time.Year;
            wMonth = (ushort)time.Month;
            wDayOfWeek = (ushort)time.DayOfWeek;
            wDay = (ushort)time.Day;
            wHour = (ushort)time.Hour;
            wMinute = (ushort)time.Minute;
            wSecond = (ushort)time.Second;
            wMilliseconds = (ushort)time.Millisecond;
        }
        /// <summary>
        /// 转换为System.DateTime类型。
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
        }
        /// <summary>
        /// 静态方法。转换为System.DateTime类型。
        /// </summary>
        /// <param name="time">SYSTEMTIME类型的时间。</param>
        /// <returns></returns>
        public static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }
    }

    public class CLBase
    {
        public static bool Is_Thread = true;
        public static bool Is_Doing = true;
        public static int G_Bws = 15;
        public static bool G_ZB = true;
        public static bool[] G_YJ = new bool[16];
        public const int CST_CLOSE = 1;
        public const int CST_OPENZD = 2;
        public const int Cst_OPENBIAO = 3;

        [DllImport("winmm.dll")]
        public static extern int timeGetTime();

        [DllImport("kernel32.DLL")]
        private static extern int WritePrivateProfileStringA(string lpName, string lpKeyName, string lpString, string lpFileName);

        [DllImport("kernel32.DLL")]
        private static extern int GetPrivateProfileStringA(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.DLL")]
        public static extern bool SetLocalTime(ref SYSTEMTIME Time);


        public static void Delay(int Tms) //pyys=硬延时
        {
            int Tb; int Te;
            if (Tms < 20) Tms = 20;
            Tb = timeGetTime();
            while (true)
            {
                Application.DoEvents();
                Te = timeGetTime();
                if (Te < Tb) Tb = 0;
                if ((Te - Tb) > Tms) break;
            }

        }

        public static string g_GetINI(string sINIFile, string sSection, string sKey, string sDefault)
        {
            StringBuilder sTemp = new StringBuilder(256);   //As String * 256
            int nLength;

            nLength = GetPrivateProfileStringA(sSection, sKey, sDefault, sTemp, 255, sINIFile);
            return sTemp.ToString();
        }

        public static void g_WriteINI(string sINIFile, string sSection, string sKey, string sValue)
        {
            int N;
            string sTemp;
            sTemp = sValue;
            N = WritePrivateProfileStringA(sSection, sKey, sTemp, sINIFile);

        }

        /// <summary>
        /// WriteLog 将运行日志保存到文件//czx
        /// </summary>
        /// <param name="info"></param>
        public static void WriteLog(string s1, string info)
        {
            object obj = new object();
            lock (obj)
            {
                string ls_filename;
                string ls_line;

                TextWriter s;
                StringWriter strWriter;
                try
                {
                    //ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    ls_filename = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\" + s1 + ".log";
                    string FilePath = Path.GetDirectoryName(ls_filename);
                    if (!Directory.Exists(FilePath))
                        Directory.CreateDirectory(FilePath);
                    strWriter = new StringWriter();
                    s = new StreamWriter(ls_filename, true, System.Text.Encoding.Default);

                    ls_line =  info;
                    s.WriteLine(ls_line);
                    s.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 字符串转换成Byte数组
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static byte[] ConvertStringToBytes(string p_str_Context)
        {
            if (p_str_Context.Length < 1)
                return new byte[0];
            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];

            for (int i = 0; i < int_ByteCount; i++)
            {
                byt_Return[i] = Convert.ToByte(p_str_Context.Substring(i * 2, 2), 16);
            }

            return byt_Return;
        }

        /// <summary>
        /// Byte数组转换成字符串
        /// </summary>
        /// <param name="p_str_Context"></param>
        /// <returns></returns>
        public static string ConvertBytesToString(byte[] p_byte_Context)
        {
            string strFrame = "";
            try
            {
                if (p_byte_Context == null) return strFrame;
                for (int i = 0; i < p_byte_Context.Length; i++)
                {
                    strFrame += Convert.ToString(p_byte_Context[i], 16).PadLeft(2, '0');
                }
                return strFrame;
            }
            catch
            { return strFrame; }

        }

        public static byte getChkSum(byte[] aryData)
        {
            int Ti = 0;
            byte bytChk = 0;
            for (Ti = 0; Ti < aryData.Length - 2; Ti++)
            {
                bytChk = Convert.ToByte((bytChk + aryData[Ti]) % 256);
            }
            return bytChk;
        }

        public static byte[] ChangeTtoByte(string Tstr)
        {
            int i; int Tlen;
            Tlen = (int)(Tstr.Length / 2);
            byte[] Tcl = new byte[Tlen];
            for (i = 0; i < Tlen; i++)
                Tcl[i] = Convert.ToByte(Tstr.Substring(i * 2, 2), 16);

            return Tcl;
        }

        public static string getChkSum(string Pstr)        //获得校验码
        {
            int Ti;
            int Tlen;
            int Tall;
            Tall = 0;

            Tlen = (int)(Pstr.Length / 2) - 1;
            for (Ti = 0; Ti <= Tlen; Ti++)
                Tall = (Tall + Convert.ToByte(Pstr.Substring(Ti * 2, 2),16)) % 256;
            return string.Format("{0:X2}", Tall);
        }

        public static string revStr(string Pstr) // '反转字符串
        {
            int Ti;
            string Ts;
            int Tlen;
            Tlen = (int)(Pstr.Length / 2) - 1;
            Ts = "";
            for (Ti = Tlen; Ti >= 0; Ti--)
                Ts = Ts + Pstr.Substring(Ti * 2, 2);
            return Ts;
        }

        public static string g_GetItem(string Pstr, int Pnum, string Pfh)//        '取得一个串中用,分开的某个字符串
        {
            int Ti;
            int Tseat;
            string Tstr;

            Ti = 0; Tstr = Pstr;
            if (Pnum < 1)
                return "";
            else
            {
                while (true)
                {
                    Tseat = Tstr.IndexOf(Pfh);
                    if (Tseat <= 0)
                        return "";
                    else
                    {
                        Ti++;
                        if (Ti == Pnum)
                            return Tstr.Substring(0, Tseat);

                        else
                            Tstr = Tstr.Substring(Tseat + 1, Tstr.Length - Tseat - 1) + " ";
                    }
                }
            }
        }

        public static string getSingle(double Ps, int Pdot, int Pbyte)
        {
            double Tl;
            string Tr;
            Tl = (double)(Math.Abs(Ps) * (int)Interaction.Choose(Pdot + 1, 1, 10, 100, 1000, 10000, 100000));
            Tl = Convert.ToInt32(Tl);
            Tr = "000000000000" + Tl.ToString();
            Tr = Tr.Substring(Tr.Length - Pbyte * 2, Pbyte * 2);
            return revStr(Tr);
        }

        public static string Del33H(string Pstr)
        {
            string Tstr = "";
            int Tlen;
            Tlen = Pstr.Length;
            for (int i = 0; i < Tlen; i = i + 2)
                Tstr = Tstr + string.Format("{0:X2}", (Convert.ToByte(Pstr.Substring(i, 2), 16) - 0x33));
            return Tstr;
        }

        public static string Add33H(string Pstr)
        {
            int Ti;
            int Tlen;
            string Tstr;
            Tlen = Pstr.Length;
            Tstr = "";
            for (Ti = 1; Ti <= Tlen; Ti = Ti + 2)
                Tstr = Tstr + string.Format("{0:X2}", (Convert.ToByte(Pstr.Substring(Ti - 1, 2), 16) + 0x33) % 0x100);
            return Tstr;
        }

        public static string getChk(string Pstr)        //获得校验码
        {
            int Ti;
            int Tlen;
            int Tall;
            Tall = 0;
            Tlen = (int)(Pstr.Length / 2) - 1;
            for (Ti = 0; Ti <= Tlen; Ti++)
                Tall = (Tall + Convert.ToByte(Pstr.Substring(Ti * 2, 2), 16)) % 256;
            return string.Format("{0:X2}", Tall);
        }

        public static bool XiaoYan(string Tvalue)
        {
            byte[] Cl;
            int TCS; int Ti; int Tlen; string Tstr;
            Tlen = (int)(Tvalue.Length / 2);
            Cl = new byte[Tlen];
            TCS = 0;
            for (Ti = 0; Ti < Tlen; Ti++)
            {
                Tstr = Tvalue.Substring(Ti * 2, 2);
                Cl[Ti] = Convert.ToByte(Tstr, 16);
                if (Ti <= Tlen - 3)
                    TCS = (TCS + Cl[Ti]) % 256; ;
            }
            if (TCS % 256 == Cl[Tlen - 2])
                return true;
            else
                return false;
        }
    }
}
