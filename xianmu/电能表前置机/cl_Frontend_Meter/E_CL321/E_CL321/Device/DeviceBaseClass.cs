using E_CLSocketModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_CL321.Device
{

    #region CL321误差板数据包基类
    /// <summary>
    /// CL321误差板数据包基类
    /// </summary>
    internal class CL321SendPacket : ClouSendPacket_CLT11
    {
        
        public CL321SendPacket()
            : base()
        {
            ToID = 0x11;
            MyID = 0x20;
        }
        public CL321SendPacket(bool bReplay)
            : base(bReplay)
        {
            ToID = 0x11;
            MyID = 0x20;
        }
        /// <summary>
        /// 返回数据的最大字节数（byte）
        /// </summary>
        /// <returns></returns>
        public override int MaxByte()
        {
            return 18;
        }
        /// <summary>
        /// 地址标识
        /// </summary>
        private string str_AddressFlag;

        public string Str_AddressFlag
        {
            get { return str_AddressFlag; }
            set { str_AddressFlag = value; }
        }
        /// <summary>
        /// 功能标识
        /// </summary>
        private string str_FunFlag;

        public string Str_FunFlag
        {
            get { return str_FunFlag; }
            set { str_FunFlag = value; }
        }

        private string str_Data;

        public string Str_Data
        {
            get { return str_Data; }
            set { str_Data = value; }
        }



        private int posion =1;
        /// <summary>
        /// 表位索引从1起始，表示某一表位；=0代表一条总线上所有要检表
        /// </summary>
        public int Pos
        {
            get
            {
                return posion;
            }
            set
            {
                posion = value;                
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

        public override byte[] GetBody()
        {
            return new byte[0];
        }

        public string BytesToString(byte[] bytesData)
        {
            string strRevalue = string.Empty;
            if (bytesData == null || bytesData.Length < 1)
                return strRevalue;

            strRevalue = BitConverter.ToString(bytesData).Replace("-", "");
            //for (int i = 0; i < bytesData.Length; i++)
            //{
            //    byte byteTmp = bytesData[i];
            //    strRevalue += Convert.ToString(byteTmp, 16);
            //}

            return strRevalue;
        }

        public byte[] StringToBytes(string p_str_Context)
        {
            if (p_str_Context.Length == 0) return new byte[0];
            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];

            for (int i = 0; i < int_ByteCount; i++)
            {
                byt_Return[i] = Convert.ToByte(p_str_Context.Substring(i * 2, 2), 16);
            }

            return byt_Return;
        }

        
    }
    /// <summary>
    /// CL321接收数据包基类
    /// </summary>
    internal class CL321RecvPacket : ClouRecvPacket_CLT11
    {
        public byte Pos { get; set; }

        protected override void ParseBody(byte[] data)
        {
        }
    }
    #endregion

}
