﻿using System;
using System.Collections.Generic;
using System.Text;
using SocketModule.Packet;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 初始化2018数据包
    /// </summary>
    internal class RequestInit2018PortPacket : SendPacket
    {
        private string m_strSetting = "";
        public RequestInit2018PortPacket(string strSetting)
        {
            m_strSetting = strSetting;
            //isSettingPacket = true;
            //ResentMaxTimes = 1;
        }

        public override byte[] GetPacketData()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            string str_Data = "init " + m_strSetting.Replace(',', ' ');
            byte[] byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
            //InsertBytes(byt_Data);
            buf.Put(byt_Data);
            return buf.ToByteArray();
        }

    }
}
