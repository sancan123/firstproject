using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL485.Device
{

    #region CLElectromotor
    /// <summary>
    /// 翻转电机接收数据包基类
    /// </summary>
    internal class CLElectromotorRecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
    /// <summary>
    /// 翻转电机发送数据包基类
    /// </summary>
    internal class CLElectromotorSendPacket : ClouSendPacket_CLT11
    {
        /// <summary>
        /// 广播标识
        /// </summary>
        public byte AllFlag = 0xFF;

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
        public bool isStart = true;

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
                ChannelByte = ConvertBwStatusToChannelByte(bwStatus, isStart);
            }
        }
        /// <summary>
        /// 通道数
        /// </summary>
        private int iChannelNum;
        /// <summary>
        /// 通道表位数
        /// </summary>
        public int ChannelNum
        {
            get
            {
                return iChannelNum;
            }
            set
            {
                iChannelNum = value;
            }
        }
        /// <summary>
        /// 当前通讯通道
        /// </summary>
        private int iChannelNo;
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
        public CLElectromotorSendPacket()
            : base()
        {
            ToID = 0x40;
            MyID = 0x01;//0x05 青铜鼎嵌入式不受发信ID影响
        }
        public CLElectromotorSendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x40;
            MyID = 0x01;//0x05
        }

        public int Pos { get; set; }

        public override byte[] GetBody()
        {
            return new byte[0];
        }

        /// <summary>
        /// 转化表位list
        /// </summary>
        /// <param name="bwstatus">表位状态</param>
        /// <param name="start">true为启动,false为停止</param>
        /// <returns></returns>
        private byte[] ConvertBwStatusToChannelByte(bool[] bwstatus, bool start)
        {
            byte[] ChannelByte = new byte[12];
            for (int i = 0; i < ChannelByte.Length; i++)
            {
                string hex2 = "";
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (Pos == 0)
                    {
                        int tmp = iChannelNo * (BwNum / iChannelNum);
                        if (j < tmp + BwNum / iChannelNum && j >= tmp)
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
            }

            return ChannelByte;
        }
    }
    #endregion


    /// <summary>
    /// CL188L误差板数据包基类
    /// </summary>
    internal class CL188LSendPacket : ClouSendPacket_CLT11
    {
        /// <summary>
        /// 广播标识
        /// </summary>
        public byte AllFlag = 0xFF;

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
        public bool isStart = true;

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
                ChannelByte = ConvertBwStatusToChannelByte(bwStatus, isStart);
            }
        }
        /// <summary>
        /// 通道数，总线路数
        /// </summary>
        private int iChannelNum = 2;
        /// <summary>
        /// 通道数，总线路数
        /// </summary>
        public int ChannelNum
        {
            get
            {
                return iChannelNum;
            }
            set
            {
                iChannelNum = value;
            }
        }
        /// <summary>
        /// 当前通讯通道
        /// </summary>
        private int iChannelNo;
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
        public CL188LSendPacket()
            : base()
        {
            ToID = 0x40;
            MyID = 0x01;
        }
        public CL188LSendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x40;
            MyID = 0x01;
        }
        /// <summary>
        /// 表位索引从1起始，表示某一表位；=0代表一条总线上所有要检表
        /// </summary>
        public int Pos { get; set; }

        public override byte[] GetBody()
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
            byte[] ChannelByte = new byte[12];//表位list长度，0x0C
            for (int i = 0; i < ChannelByte.Length; i++)
            {
                string hex2 = "";
                for (int j = (i + 1) * 8 - 1; j >= i * 8; j--)
                {
                    if (Pos == 0)
                    {
                        int tmp = iChannelNo * (BwNum / iChannelNum);
                        if (j < tmp + BwNum / iChannelNum && j >= tmp)
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
            }

            return ChannelByte;
        }
    }

    /// <summary>
    /// CL188L接收数据包基类
    /// </summary>
    internal class CL188LRecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }

}
