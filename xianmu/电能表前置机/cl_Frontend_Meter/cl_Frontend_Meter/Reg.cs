using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace cl_Frontend_Meter
{
    public class Reg
    {
        public string sName = "12345678";
        private readonly byte[] Keys = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
        //private static readonly string LogFile = "cldatamanager.txt";


        //private string getCpu()
        //{
        //    string cpu =null;
        //    using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementClass("win32_Processor").GetInstances().GetEnumerator())
        //    {
        //        if (enumerator.MoveNext())
        //            cpu = enumerator.Current.Properties["Processorid"].Value.ToString();
        //    }
        //    return cpu;
        //}

        //private static string GetMbId()
        //{
        //    ManagementObjectCollection instances = new ManagementClass("Win32_BaseBoard").GetInstances();
        //    string mbId = "";
        //    foreach (ManagementBaseObject managementBaseObject in instances)
        //    {
        //        object obj = managementBaseObject.Properties["SerialNumber"].Value;
        //        mbId += obj == null ? "" : obj.ToString();
        //    }
        //    return mbId;
        //}

        private string GetChar(string str)
        {
            string str1 = "";
            for (int i = 0; i < str.Length; ++i)
                str1 += this.Asc(str.Substring(i, 1)).ToString("x2");
            return str1.ToUpper();
        }

        private string GetChar2(string str)
        {
            string str1 = "";
            for (int i = 0; i < str.Length; ++i)
                str1 += this.Asc(str.Substring(i, 1)).ToString("x2").Substring(0, 1);
            return str1.ToUpper();
        }

        private int Asc(string character) => character.Length == 1 ? new ASCIIEncoding().GetBytes(character)[0] : throw new Exception("Character is not valid.");

      //  private string Chr(int asciiCode) => asciiCode >= 0 && asciiCode <= byte.MaxValue ? new ASCIIEncoding().GetString(new byte[1]
      //  {
      //(byte) asciiCode
      //  }) : throw new Exception("ASCII Code is not valid.");

        //private static string GetHdId()
        //{
        //    ManagementObjectCollection instances = new ManagementClass("Win32_PhysicalMedia").GetInstances();
        //    string hdId = "";
        //    foreach (ManagementBaseObject managementBaseObject in instances)
        //    {
        //        object obj = managementBaseObject.Properties["SerialNumber"].Value;
        //        hdId += obj == null ? "" : obj.ToString().Trim();
        //    }
        //    return hdId;
        //}

        //private static void Log(string message) => System.IO.File.AppendAllText(Reg.LogFile, message + Environment.NewLine);

        private string GetDiskVolumeSerialNumber()
        {
            //ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            managementObject.Get();
            return managementObject.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        public string CreateCode()
        {
            string str1 = this.GetChar(this.GetDiskVolumeSerialNumber());
            string str2 = "";
            for (int i = 0; i < str1.Length / 4; ++i)
                str2 = str2 + str1.Substring(i * 4, 4) + "-";
            return str2.Trim('-');
        }

        public string BoolCode(string ln, string sCode) => this.GetCode(ln, "czx") == sCode ? "czx" : "";

        public string GetCode(string ln, string sVer) => sVer != "czx" ? "" : this.GetChar2(this.EncryptDES(ln, this.sName));

        private string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] keys = this.Keys;
                byte[] bytes2 = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(bytes1, keys), CryptoStreamMode.Write);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        //private string DecryptDES(string decryptString, string decryptKey)
        //{
        //    try
        //    {
        //        byte[] bytes = Encoding.UTF8.GetBytes(decryptKey);
        //        byte[] keys = this.Keys;
        //        byte[] buffer = Convert.FromBase64String(decryptString);
        //        DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
        //        MemoryStream memoryStream = new MemoryStream();
        //        CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateDecryptor(bytes, keys), CryptoStreamMode.Write);
        //        cryptoStream.Write(buffer, 0, buffer.Length);
        //        cryptoStream.FlushFinalBlock();
        //        return Encoding.UTF8.GetString(memoryStream.ToArray());
        //    }
        //    catch
        //    {
        //        return decryptString;
        //    }
        //}
    }

}
