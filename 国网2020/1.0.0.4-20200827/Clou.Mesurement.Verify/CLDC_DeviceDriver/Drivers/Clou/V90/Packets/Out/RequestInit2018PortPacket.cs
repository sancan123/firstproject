using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V90.Packets.Out
{
    internal class RequestInit2018PortPacket : Base
    {
        private string m_strSetting = "";
        public RequestInit2018PortPacket(string strSetting):base(false)
        {
            m_strSetting = strSetting;
            isSettingPacket = true;
        }

        protected override void PutBody()
        {
            string str_Data = "init " + m_strSetting.Replace(',', ' ');
            byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
            InsertBytes(byt_Data); 
        }
    }
}
