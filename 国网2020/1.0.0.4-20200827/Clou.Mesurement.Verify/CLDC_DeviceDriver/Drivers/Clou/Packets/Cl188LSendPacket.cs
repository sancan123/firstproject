using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets
{
    /// <summary>
    /// CL188L误差板数据包基类
    /// </summary>
    internal class Cl188LSendPacket : ClouSendPacket_CLT11 
    {
        /// <summary>
        /// Data数据域首字节
        /// </summary>
        public byte Data1 = 0xFF;

        /// <summary>
        /// 表位数
        /// </summary>
        public int BwNum = 0;

        /// <summary>
        /// 误差版通道
        /// </summary>
        public byte[] ChannelByte;

        /// <summary>
        /// 启动/停止当前功能
        /// </summary>
        public bool isStart=true;

        /// <summary>
        /// 表位状态
        /// </summary>
        private bool[] bwStatus;
        public bool[] BwStatus
        {
            get
            {
                return bwStatus;
            }
            set
            {
                bwStatus = value;
                BwNum = bwStatus.Length;
                ChannelByte = ConvertBwStatusToChannelByte(bwStatus,isStart);
            }
        }
        /// <summary>
        /// 当前通讯通道
        /// </summary>
        public int iChannelNo;
        /// <summary>
        /// 通讯通道
        /// </summary>
        public int ChannelNo
        {
            get
            {
                return iChannelNo;
            }
            set
            {
                iChannelNo = value;
            }
        }
        public Cl188LSendPacket()
            : base()
        {
            ToID = 0x40;
            MyID = 0x05;
        }
        public Cl188LSendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x40;
            MyID = 0x05;
        }

        public int Pos { get; set; }

        protected override byte[] GetBody()
        {
            return new byte[0]; 
        }

        /// <summary>
        /// 启动停止功能
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="start">true为启动,false为停止</param>
        /// <returns></returns>
        private byte[] ConvertBwStatusToChannelByte(bool[] bwstatus, bool start)
        {
            byte[] ChannelByte = new byte[12];
            bool bSendByte = false;

            for (int i = 0; i < ChannelByte.Length; i++)
            {                
                string hex2 = "";
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (Pos == 0)
                    {
                        int tmp = iChannelNo * (BwNum / 5);
                        if (j < tmp + BwNum / 5 && j >= tmp)
                            hex2 += bwstatus[j] ? (start ? "1" : "0") : "0";
                        else
                            hex2 += "0";
                    }
                    else
                    {
                        if (j == Pos - 1)
                            hex2 += "1";
                        else
                            hex2 += "0";
                    }
                }
                ChannelByte[ChannelByte.Length - 1 - i] = Str2ToByte(hex2);
                if (ChannelByte[ChannelByte.Length - 1 - i] != 0)
                    bSendByte = true;
            }
            if (bSendByte)
                return ChannelByte;
            else
                return null;
        }
    }
}
