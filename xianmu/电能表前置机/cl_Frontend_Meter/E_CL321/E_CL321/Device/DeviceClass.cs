using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;
using E_CLSocketModule.SocketModule;

namespace E_CL321.Device
{

    #region CL321误差板

    #region CL321 读取数据操作指令0x80以及回复0x80
    /// <summary>
    /// 321读取数据操作请求包
    /// </summary>
    internal class CL321_RequestReadDataPacket : CL321SendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0x80;

        public CL321_RequestReadDataPacket()
            : base(true)
        {
            this.Pos = 0;
        }       

        public override string GetPacketName()
        {
            return "CL321_RequestReadDataPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        public override byte[] GetBody()
        {
            byte[] ArrByt = null;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put((byte)Pos);
            ArrByt = StringToBytes(Str_AddressFlag);
            if (ArrByt.Length > 0)
                buf.Put(ArrByt);
            
            return buf.ToByteArray();
            //return StringToBytes(GetWrite3201(Pos, "1203", "01"));
        }

    }
    /// <summary>
    /// 读取数据指令，返回数据包
    /// </summary>
    internal class CL321_RequestReadDataReplayPacket : ClouRecvPacket_CLT11
    {
        public CL321_RequestReadDataReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL321_RequestReadDataReplayPacket";
        }

        /// <summary>
        /// 读取数据结果
        /// </summary>
        public string strReturnInfo { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.strReturnInfo = "";
            string strInfo = "";
            string strTemp = "";
            if (data[0] != 0x80)
            {
                this.ReciveResult = RecvResult.DataError;                
                return;
            }
            strInfo = BitConverter.ToString(data).Replace("-", "");
            switch(strInfo.Substring(4,4))
            {
                case "1001":    //版本信息
                    strTemp = strInfo.Substring(strInfo.Length - 8, 2);

                    strReturnInfo = strInfo.Substring(strInfo.Length - 6, 6);

                    strReturnInfo += ":" + (float.Parse(strTemp) / 10).ToString("F2");
                    break;
                case "1015":
                case "1016":
                case "1017":
                case "1018":
                    {
                        int iCount = 0;//误差次数
                        double sError = 0;//误差值
                        string str = strInfo.Substring(8, 12);
                        iCount = Convert.ToInt16(str.Substring(8, 4), 16);
                        sError = Convert.ToInt32(str.Substring(2, 6), 16) / Math.Pow(10, (Convert.ToInt16(str.Substring(0, 2), 16) << 4) % 256 / 16);
                        if (Convert.ToInt16(str.Substring(0, 2), 16) >> 7 == 1)
                            sError = 0 - sError;
                        strReturnInfo = iCount.ToString() + ":" + sError;
                    }
                    break;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL321 写数据操作指令0x81以及回复0x81
    /// <summary>
    /// 321写数据操作请求包
    /// </summary>
    internal class CL321_RequestWriteDataPacket : CL321SendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0x81;

        public CL321_RequestWriteDataPacket()
            : base(true)
        {
            this.Pos = 0;
        }

        public override string GetPacketName()
        {
            return "CL321_RequestWriteDataPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        public override byte[] GetBody()
        {
            byte[] ArrByt = null;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put((byte)Pos);
            ArrByt = StringToBytes(Str_AddressFlag);
            if (ArrByt.Length > 0)
                buf.Put(ArrByt);
            ArrByt = StringToBytes(Str_Data);
            if (ArrByt.Length > 0)
                buf.Put(ArrByt);
            return buf.ToByteArray();
            //return StringToBytes(GetWrite3201(Pos, "1203", "01"));
        }

    }
    /// <summary>
    /// 写数据指令，返回数据包
    /// </summary>
    internal class CL321_RequestWriteDataReplayPacket : ClouRecvPacket_CLT11
    {
        public CL321_RequestWriteDataReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL321_RequestWriteDataReplayPacket";
        }

        /// <summary>
        /// 读取数据结果
        /// </summary>
        public string strReturnInfo { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.strReturnInfo = "";
            string strInfo = "";
            string strTemp = "";
            if (data[0] != 0x81)
            {
                this.ReciveResult = RecvResult.DataError;
                return;
            }
            strInfo = BitConverter.ToString(data).Replace("-", "");
            switch (strInfo.Substring(4, 4))
            {
                case "1001":    //版本信息
                    strTemp = strInfo.Substring(strInfo.Length - 8, 2);

                    strReturnInfo = strInfo.Substring(strInfo.Length - 6, 6);

                    strReturnInfo += " " + (float.Parse(strTemp) / 10).ToString("F2");
                    break;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL321 起动操作指令0x82以及回复0x82
    /// <summary>
    /// 321起动数据操作请求包
    /// </summary>
    internal class CL321_RequestStartFunPacket : CL321SendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0x82;

        public CL321_RequestStartFunPacket()
            : base(true)
        {
            this.Pos = 0;
        }

        public override string GetPacketName()
        {
            return "CL321_RequestStartFunPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        public override byte[] GetBody()
        {
            byte[] ArrByt = null;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put((byte)Pos);
            ArrByt = StringToBytes(Str_FunFlag);
            if (ArrByt.Length > 0)
                buf.Put(ArrByt);

            return buf.ToByteArray();
            //return StringToBytes(GetWrite3201(Pos, "1203", "01"));
        }

    }
    /// <summary>
    /// 写数据指令，返回数据包
    /// </summary>
    internal class CL321_RequestStartFunReplayPacket : ClouRecvPacket_CLT11
    {
        public CL321_RequestStartFunReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL321_RequestStartFunReplayPacket";
        }

        /// <summary>
        /// 读取数据结果
        /// </summary>
        public string strReturnInfo { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.strReturnInfo = "";
            string strInfo = "";
            string strTemp = "";
            if (data[0] != 0x82)
            {
                this.ReciveResult = RecvResult.DataError;
                return;
            }
            strInfo = BitConverter.ToString(data).Replace("-", "");
            switch (strInfo.Substring(4, 4))
            {
                case "1001":    //版本信息
                    strTemp = strInfo.Substring(strInfo.Length - 8, 2);

                    strReturnInfo = strInfo.Substring(strInfo.Length - 6, 6);

                    strReturnInfo += " " + (float.Parse(strTemp) / 10).ToString("F2");
                    break;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL321 停止操作指令0x83以及回复0x83
    /// <summary>
    /// 321停止操作请求包
    /// </summary>
    internal class CL321_RequestStopFunPacket : CL321SendPacket
    {

        /// <summary>
        /// 命令代码
        /// </summary>
        private byte m_Cmd = 0x83;

        public CL321_RequestStopFunPacket()
            : base(true)
        {
            this.Pos = 0;
        }

        public override string GetPacketName()
        {
            return "CL321_RequestStartFunPacket";
        }
        /*
         * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
        */
        public override byte[] GetBody()
        {
            byte[] ArrByt = null;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_Cmd);
            buf.Put((byte)Pos);
            ArrByt = StringToBytes(Str_FunFlag);
            if (ArrByt.Length > 0)
                buf.Put(ArrByt);

            return buf.ToByteArray();
            //return StringToBytes(GetWrite3201(Pos, "1203", "01"));
        }

    }
    /// <summary>
    /// 停止操作指令，返回数据包
    /// </summary>
    internal class CL321_RequestStopFunReplayPacket : ClouRecvPacket_CLT11
    {
        public CL321_RequestStopFunReplayPacket()
            : base()
        {
        }

        /// <summary>
        /// 返回路数
        /// </summary>
        public byte Pos
        {
            get;
            private set;
        }
        public override string GetPacketName()
        {
            return "CL321_RequestStopFunReplayPacket";
        }

        /// <summary>
        /// 读取数据结果
        /// </summary>
        public string strReturnInfo { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            this.strReturnInfo = "";
            string strInfo = "";
            string strTemp = "";
            if (data[0] != 0x83)
            {
                this.ReciveResult = RecvResult.DataError;
                return;
            }
            strInfo = BitConverter.ToString(data).Replace("-", "");
            switch (strInfo.Substring(4, 4))
            {
                case "1001":    //版本信息
                    strTemp = strInfo.Substring(strInfo.Length - 8, 2);

                    strReturnInfo = strInfo.Substring(strInfo.Length - 6, 6);

                    strReturnInfo += " " + (float.Parse(strTemp) / 10).ToString("F2");
                    break;
            }
            this.Pos = data[1];
            this.ReciveResult = RecvResult.OK;
        }
    }
    #endregion


    //#region CL321远程升级使能命令 0x00|0x01
    ////命令例子：81 40 01 18 00 FF 0C FF FF FF FF FF FF FF FF FF FF FF FF 11 33 55 77 AA
    //internal class CL321_RequestUpdateLoginPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    private byte m_Cmd = 0x00;

    //    public CL321_RequestUpdateLoginPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateLoginPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(0x11);
    //        buf.Put(0x33);
    //        buf.Put(0x55);
    //        buf.Put(0x77);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestUpdateLoginReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestUpdateLoginReplayPacket() :
    //        base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateLoginReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x01)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321软件重启命令0x02|0x03
    //internal class CL321_RequestReBootPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x02;

    //    public CL321_RequestReBootPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReBootPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestReBootReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestReBootReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReBootPacketReplay";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x03)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }

    //}

    //#endregion

    //#region CL321升级数据命令0x04|0x05

    //internal class CL321_RequestUpdateFirmwarePacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x04;

    //    private UInt16 dataIndex = 0;

    //    private byte[] byteUpdate = null;

    //    public CL321_RequestUpdateFirmwarePacket(UInt16 index, byte[] bytsData)
    //        : base()
    //    {
    //        dataIndex = index;
    //        byteUpdate = bytsData;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateFirmwarePacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(BitConverter.GetBytes(dataIndex));

    //        buf.Put(byteUpdate);

    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestUpdateFirmwareReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestUpdateFirmwareReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateFirmwarePacketReplay";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x05)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 2级设备远程升级0x06|0x07
    //internal class CL321_RequestUpdateLogin2Packet : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x06;

    //    public CL321_RequestUpdateLogin2Packet()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateLogin2Packet";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(0x11);
    //        buf.Put(0x33);
    //        buf.Put(0x55);
    //        buf.Put(0x77);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestUpdateLogin2ReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestUpdateLogin2ReplayPacket() :
    //        base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateLogin2ReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x07)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 2级设备软件重启命令0x08|0x09

    //internal class CL321_RequestReBoot2Packet : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x08;

    //    public CL321_RequestReBoot2Packet()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReBoot2Packet";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestReBoot2ReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestReBoot2ReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReBoot2ReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x09)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }

    //}
    //#endregion

    //#region CL321 2级设备升级0x0A|0X0B

    //internal class CL321_RequestUpdateFirm2warePacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x0A;

    //    private UInt16 dataIndex = 0;

    //    private byte[] byteUpdate = null;

    //    public CL321_RequestUpdateFirm2warePacket(UInt16 index, byte[] bytsData)
    //        : base()
    //    {
    //        dataIndex = index;
    //        byteUpdate = bytsData;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateFirmwarePacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(BitConverter.GetBytes(dataIndex));

    //        buf.Put(byteUpdate);

    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestUpdateFirmware2ReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestUpdateFirmware2ReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestUpdateFirmwarePacketReplay";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x0b)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 读取设备版本2 0x0C|0x0D

    //internal class CL1888M_RequestReadVersion2Packet : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x0C;

    //    public CL1888M_RequestReadVersion2Packet()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL1888M_RequestReadVersion2Packet";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL1888M_RequestReadVersion2ReplayPacket : ClouRecvPacket_CLT11
    //{
    //    //存储解析到的版本号
    //    public string strVersion = string.Empty;

    //    public CL1888M_RequestReadVersion2ReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL1888M_RequestReadVersion2ReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x0D)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 21)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                strVersion = Convert.ToString(data[19], 16) + "."
    //                    + Convert.ToString(data[20], 16) + "."
    //                    + Convert.ToString(data[21], 16);
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 启动硬件测试命令 0x0E|0x0F

    //internal class CL321_RequestStartUpHardWareTestPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x0E;

    //    public CL321_RequestStartUpHardWareTestPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartUpHardWareTestPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(0x00);
    //        buf.Put(0x00);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestStartUpHardWareTestReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStartUpHardWareTestReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartUpHardWareTestReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0x0F)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 查询硬件测试结果命令 0x10|0x11
    //internal class CL321_RequsetQuertHardWareTestResultPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x10;

    //    public CL321_RequsetQuertHardWareTestResultPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequsetQuertHardWareTestResultPacket";
    //    }
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(0x00);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQuertHardWareTestResutlReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public string HardWareTestResult = string.Empty;

    //    public CL321_RequestQuertHardWareTestResutlReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQuertHardWareTestResutlReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0x11)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            HardWareTestResult = Convert.ToString(idata, 16);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}
    //#endregion

    //#region CL321查询功耗参数 0x12以及回复0x13
    ///// <summary>
    ///// 读取功耗参数
    ///// </summary>
    //internal class CL321_RequestReadGHPramPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x12;

    //    public CL321_RequestReadGHPramPacket()
    //        : base(true)
    //    {

    //    }

    //    /// <summary>
    //    /// 设置参数，（自动转换表位list）
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    public void SetPara(bool[] bwstatus)
    //    {

    //        this.BwStatus = bwstatus;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadGHPramPacket";
    //    }
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);

    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 返回功耗参数
    ///// </summary>
    //internal class CL321_RequestReadBwGHPramReplyPacket : CL321RecvPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    public byte bCmd { get; private set; }

    //    /// <summary>
    //    /// bytes List
    //    /// </summary>
    //    public byte[] BwChannelList { get; private set; }

    //    /// <summary>
    //    /// 误差板编号
    //    /// </summary>
    //    public int wcbIndex { get; private set; }
    //    /// <summary>
    //    /// 是否返回错误 
    //    /// </summary>
    //    public bool m_bError { get; private set; }
    //    /// <summary>
    //    /// 功耗数据
    //    /// </summary>
    //    public float AU_Ia_or_I { get; private set; }
    //    public float BU_Ib_or_L1_U { get; private set; }
    //    public float CU_Ic_or_L2_U { get; private set; }
    //    public float AI_Ua { get; private set; }
    //    public float BI_Ub { get; private set; }
    //    public float CI_Uc { get; private set; }
    //    public float AU_Phia_or_Phi { get; private set; }
    //    public float BU_Phib { get; private set; }
    //    public float CU_Phic { get; private set; }
    //    /// <summary>
    //    /// 系统解析帧  
    //    /// </summary>
    //    /// <param name="data"></param>
    //    ///         
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）   
    //     * +误差板编号（1 byte）+  误差板软件版本号（1 byte）。
    //     * 软件版本号的表示采用BCD编码方式编码，小数点不表示。
    //     * 	例：如果版本号为1.1，则版本号早数据帧中被表示为  0x11。
    //     */
    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 36) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        buf.Get();                                  //广播标志(0xFFH)
    //        buf.Get();                                  //1 byte ListLen                  
    //        BwChannelList = buf.GetByteArray(12);       // List
    //        //wcbIndex = buf.Get();                       //误差板编号

    //        if (BitConverter.ToInt32(buf.GetByteArray(4), 0) == 0)
    //            m_bError = false;
    //        else
    //            m_bError = true;
    //        if (!m_bError)
    //        {

    //            AU_Ia_or_I = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            BU_Ib_or_L1_U = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            CU_Ic_or_L2_U = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            AI_Ua = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            BI_Ub = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            CI_Uc = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            AU_Phia_or_Phi = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            BU_Phib = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);
    //            CU_Phic = (float)(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 1000000.0);

    //            ReciveResult = RecvResult.OK;
    //        }
    //        else
    //        {

    //        }
    //    }
    //}
    //#endregion

    //#region CL321启动遥信输出命令0x14以及回复0x15
    ///// <summary>
    ///// 启动遥信输出命令 发送
    ///// </summary>
    //internal class CL321_RequestStartYXOutPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x14;

    //    /// <summary>
    //    /// 遥信路数（1Bytes）
    //    /// </summary>
    //    private int YXTestNo = 0;

    //    /// <summary>
    //    /// 遥信输出方式（1Bytes）0：电平方式1：脉冲方式
    //    /// </summary>
    //    private int YxTestType = 0;

    //    /// <summary>
    //    /// 脉冲个数（4Bytes）脉冲输出个数，若遥信输出方式为电平方式，则该数据为无效数据
    //    /// </summary>
    //    private int YxTestPulseNum = 0;


    //    /// <summary>
    //    ///  脉冲输出频率的1000倍（4Bytes），若遥信输出方式为电平方式，则该数据为无效数据
    //    /// </summary>
    //    private int YxTestPulseOutHz = 0;

    //    /// <summary>
    //    /// 输出占空比的1000000倍（4Bytes），若遥信输出方式为电平方式，则该数据为无效数据
    //    /// </summary>
    //    private int YxTestOutmultiple = 0;


    //    public CL321_RequestStartYXOutPacket()
    //        : base(true)
    //    { }

    //    /// <summary>
    //    /// 设置参数
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="yXTestNo">遥信路数</param>
    //    /// <param name="yxTestType">遥信输出方式（1Bytes）0：电平方式1：脉冲方式</param>
    //    /// <param name="yxTestPulseNum">脉冲个数（4Bytes）脉冲输出个数，若遥信输出方式为电平方式，则该数据为无效数据</param>
    //    /// <param name="yxTestPulseOutHz">脉冲输出频率的1000倍（4Bytes），若遥信输出方式为电平方式，则该数据为无效数据</param>
    //    /// <param name="yxTestOutmultiple">输出占空比的100 0000倍（4Bytes），若遥信输出方式为电平方式，则该数据为无效数据</param>
    //    public void SetPara(bool[] bwstatus, int yXTestNo, int yxTestType, int yxTestPulseNum, float yxTestPulseOutHz, float yxTestOutmultiple)
    //    {
    //        IsNeedReturn = false;
    //        BwStatus = bwstatus;
    //        this.YXTestNo = yXTestNo;
    //        this.YxTestType = yxTestType;
    //        this.YxTestPulseNum = yxTestPulseNum;
    //        this.YxTestPulseOutHz = (int)(yxTestPulseOutHz * 1000);
    //        this.YxTestOutmultiple = (int)(yxTestOutmultiple * 1000000);
    //    }
    //    public void SetPara(int id, int yXTestNo, int yxTestType, int yxTestPulseNum, float yxTestPulseOutHz, float yxTestOutmultiple)
    //    {
    //        IsNeedReturn = false;
    //        Pos = id;
    //        this.YXTestNo = yXTestNo;
    //        this.YxTestType = yxTestType;
    //        this.YxTestPulseNum = yxTestPulseNum;
    //        this.YxTestPulseOutHz = (int)(yxTestPulseOutHz * 1000);
    //        this.YxTestOutmultiple = (int)(yxTestOutmultiple * 1000000);
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartYXOutPacket";
    //    }

    //    /*
    //     *Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte） 
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(YXTestNo));
    //        buf.Put(Convert.ToByte(YxTestType));
    //        buf.PutInt_S(YxTestPulseNum);
    //        buf.PutInt_S(YxTestPulseOutHz);
    //        buf.PutInt_S(YxTestOutmultiple);

    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 启动遥信输出命令，返回数据包
    ///// </summary>
    //internal class CL321_RequestStartYXOutReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStartYXOutReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartYXOutReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0x15)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321启动直流模拟量采集参数命令0x16以及回复0x17

    ///// <summary>
    ///// 启动直流模拟量采集参数命令
    ///// </summary>
    //internal class CL321_RequestStartZLMNTestFunctionPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x16;

    //    /// <summary>
    //    /// 设置的直流模拟量采集实验电流值mA;其他值：无效参数，会导致返回命令返回错误值0x00000001
    //    /// </summary>
    //    private float Current;



    //    public CL321_RequestStartZLMNTestFunctionPacket()
    //        : base(false)
    //    { }

    //    /// <summary>
    //    /// 停止计算功能指令，若表位列表中某一位置1则停止对应表位检定，为0则不改变，若List = 0x30H，
    //    /// 则停止第5和第6表位的检定；检定类型设置同A7指令，自动检表线上，下发07指令停止所有的检定。
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestStartZLMNTestFunctionPacket(bool[] bwstatus, int current)
    //        : base(true)
    //    {
    //        this.isStart = true;
    //        this.Pos = 0;
    //        this.BwStatus = bwstatus;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="current">设置的直流模拟量电流值[4,20]mA;其他值：无效参数，返回错误值0x00000001</param>
    //    public void SetParam(bool[] bwstatus, float current)
    //    {
    //        this.Current = current;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetParam(int id, float current)
    //    {
    //        this.Current = current;
    //        this.Pos = id;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartZLMNTestFunctionPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);

    //        buf.PutUShort_S((ushort)(Current * 1000));

    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 启动直流模拟量采集参数命令，返回数据包
    ///// </summary>
    //internal class CL321_RequestStartZLMNTestFunctionReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStartZLMNTestFunctionReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartZLMNTestFunctionReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xF0)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321停止遥信实验、直流模拟量采集实验命令0x18以及回复0x19
    ///// <summary>
    ///// 停止遥信实验、直流模拟量采集实验
    ///// </summary>
    //internal class CL321_RequestStopPCYXZLTestFunctionPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x18;

    //    /// <summary>
    //    /// 0：遥信实验1：直流模拟量采集实验
    //    /// </summary>
    //    private int checkTestType;
    //    /// <summary>
    //    /// 表示第几路遥信,该字节只有当实验类型是遥信实验时候为有效值,其他值：无效参数
    //    /// </summary>
    //    private int chaneenNo;


    //    public CL321_RequestStopPCYXZLTestFunctionPacket()
    //        : base(false)
    //    { }

    //    /// <summary>
    //    /// 停止计算功能指令，若表位列表中某一位置1则停止对应表位检定，为0则不改变，若List = 0x30H，
    //    /// 则停止第5和第6表位的检定；检定类型设置同A7指令，自动检表线上，下发07指令停止所有的检定。
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestStopPCYXZLTestFunctionPacket(bool[] bwstatus, int checktype)
    //        : base(true)
    //    {
    //        this.isStart = true;
    //        this.Pos = 0;
    //        this.checkTestType = checktype;
    //        this.BwStatus = bwstatus;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checkType">0：遥信实验1：直流模拟量采集实验</param>
    //    /// <param name="chennNo">表示第几路遥信,该字节只有当实验类型是遥信实验时候为有效值,其他值：无效参数</param>
    //    public void SetParam(bool[] bwstatus, int checkType, int chennNo)
    //    {
    //        this.checkTestType = checkType;
    //        this.chaneenNo = chennNo;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetParam(int id, int checkType, int chennNo)
    //    {
    //        this.checkTestType = checkType;
    //        this.chaneenNo = chennNo;
    //        this.Pos = id;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStopPCYXZLTestFunctionPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte((int)checkTestType));
    //        buf.Put(Convert.ToByte((int)chaneenNo));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 停止遥信实验、直流模拟量采集实验，返回数据包
    ///// </summary>
    //internal class CL321_RequestStopPCYXZLTestFunctionReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStopPCYXZLTestFunctionReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStopPCYXZLTestFunctionReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xF0)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321遥控信号读取命令0x1A以及回复0x1B
    ///// <summary>
    ///// 遥控读取0x1A
    ///// </summary>
    //internal class CL321_RequestYaoKongPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x1A;

    //    /// <summary>
    //    /// 遥控路数（1Bytes）
    //    /// </summary>
    //    private int YKTestNo = 0;

    //    public CL321_RequestYaoKongPacket()
    //        : base(true)
    //    {

    //    }
    //    public void SetPara(int YKNo)
    //    {
    //        YKTestNo = YKNo;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestYaoKongPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(YKTestNo));

    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 返回遥控0x1B
    ///// </summary>
    //internal class CL321_RequestYaoKongReplayPacket : CL321RecvPacket
    //{
    //    public CL321_RequestYaoKongReplayPacket()
    //        : base()
    //    {
    //    }
    //    byte bCmd;

    //    byte[] BwChannelList;

    //    /// <summary>
    //    /// 遥控个数
    //    /// </summary>
    //    public int PusleCount { get; private set; }
    //    /// <summary>
    //    /// 是否返回错误 
    //    /// </summary>
    //    public bool m_bError { get; private set; }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestYaoKongReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 36) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        buf.Get();                                  //广播标志(0xFFH)
    //        buf.Get();                                  //1 byte ListLen                  
    //        BwChannelList = buf.GetByteArray(12);       // List
    //        //wcbIndex = buf.Get();                       //误差板编号

    //        if (BitConverter.ToInt32(buf.GetByteArray(4), 0) == 0)
    //            m_bError = false;
    //        else
    //            m_bError = true;
    //        if (!m_bError)
    //        {
    //            PusleCount = BitConverter.ToInt32(buf.GetByteArray(4), 0);
    //        }
    //        else
    //        {

    //        }
    //    }

    //}
    //#endregion

    //#region CL321 跳闸选择命令0x20以及回复0x21

    //internal class CL321_RequestChoiceSwitchPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x20;
    //    //0 停止跳闸|1启动跳闸试验
    //    private byte m_SwitchCommand = 0x00;

    //    public CL321_RequestChoiceSwitchPacket(byte byteCmd)
    //        : base()
    //    {
    //        m_SwitchCommand = byteCmd;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestChoiceSwitchPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(m_SwitchCommand);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestChoiceSwitchReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestChoiceSwitchReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestChoiceSwitchReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x21)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}
    //#endregion

    //#region CL321负控继电器控制命令0x22以及回复0x23
    ///// <summary>
    ///// 启动遥信输出命令 发送
    ///// </summary>
    //internal class CL321_RequestFuKJDQPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x22;

    //    /// <summary>
    //    /// 负控继电器状态
    //    /// 0：复位状态
    //    /// 1：二次开路
    //    /// 2：二次短路
    //    /// </summary>
    //    public int FkTestStatus = 0;
    //    public CL321_RequestFuKJDQPacket()
    //        : base(true)
    //    { }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestFuKJDQPacket";
    //    }

    //    /*
    //     *Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte） 
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);

    //        buf.Put(Convert.ToByte(FkTestStatus));

    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 启动遥信输出命令，返回数据包
    ///// </summary>
    //internal class CL321_RequestFuKJDQReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestFuKJDQReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestFuKJDQReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0x23)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321 直流输出校准命令0x24以及回复0x25
    //internal class CL321_RequestDirectCurrentOutputPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x24;

    //    public CL321_RequestDirectCurrentOutputPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestDirectCurrentOutputPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestDirectCurrentOutputReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestDirectCurrentOutputReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestDirectCurrentOutputReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x25)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321直流输出校准传递实际电流值命令0x26以及回复0x27

    //internal class CL321_RequestDirectCurrentOutputCalibrationPacket : CL321SendPacket
    //{
    //    private  UInt16 MoniCurrent = 0;

    //    private byte m_Cmd = 0x26;

    //    public CL321_RequestDirectCurrentOutputCalibrationPacket(UInt16 data )
    //        : base()
    //    {
    //        MoniCurrent = data;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestDirectCurrentOutputCalibrationPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(BitConverter.GetBytes(MoniCurrent));
    //        return buf.ToByteArray();
    //    }

    //}

    //internal class CL321_RequestDirectCurrentOutputCalibrationReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public short correctParams = 0;
    //    public CL321_RequestDirectCurrentOutputCalibrationReplayPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestDirectCurrentOutputCalibrationReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x27)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 20)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                correctParams = BitConverter.ToInt16(data, 19);
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321电机延时时间设置，翻转电机同协议 0x38|0x39
    ///// <summary>
    ///// 设置电机延时时间，发送
    ///// </summary>
    //internal class CL321_SetElectromotorTimePacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x38;

    //    /// <summary>
    //    /// 0：设置上限位（默认）
    //    /// 1：设置下限位
    //    /// </summary>
    //    private int m_UpOrDown = 0;
    //    /// <summary>
    //    /// 运算标志
    //    /// 0：+
    //    /// 1：-
    //    /// </summary>
    //    private int m_Option = 0;
    //    /// <summary>
    //    /// 需要递增或递减的时间ms
    //    /// </summary>
    //    private int m_CalTime = 0;

    //    /// <summary>
    //    /// 设置电机延时时间，带回复
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="iChannelNum"></param>
    //    /// <param name="UpOrDown">0：设置上限位,1：设置下限位</param>
    //    /// <param name="Option">运算标志0：+ ,1：-</param>
    //    /// <param name="CalTime">需要递增或递减的时间ms</param>
    //    public CL321_SetElectromotorTimePacket(bool[] bwstatus, int iChannelNum, int UpOrDown, int Option, int CalTime)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.BwStatus = bwstatus;

    //        this.m_UpOrDown = UpOrDown;
    //        this.m_Option = Option;
    //        this.m_CalTime = CalTime;
    //    }

    //    public CL321_SetElectromotorTimePacket(int id, int UpOrDown, int Option, int CalTime)
    //        : base(true)
    //    {
    //        this.Pos = id;

    //        this.m_UpOrDown = UpOrDown;
    //        this.m_Option = Option;
    //        this.m_CalTime = CalTime;
    //    }
    //    /// <summary>
    //    /// 包名
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketName()
    //    {
    //        return "CL321_SetElectromotorTimePacket";
    //    }
    //    /// <summary>
    //    /// 解析
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketResolving()
    //    {
    //        string _option = "Err";
    //        if (m_Option == 0)
    //        {
    //            _option = "+";
    //        }
    //        else if (m_Option == 1)
    //        {
    //            _option = "-";
    //        }
    //        string _upDown = "Err";
    //        if (m_UpOrDown == 0)
    //        {
    //            _upDown = "上";
    //        }
    //        else if (m_UpOrDown == 1)
    //        {
    //            _upDown = "下";
    //        }
    //        string rso = "设置电机延时时间: " + _upDown + " " + _option + " " + m_CalTime + "ms";
    //        return rso;
    //    }
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(m_UpOrDown));
    //        buf.Put(Convert.ToByte(m_Option));
    //        buf.PutInt_S(m_CalTime);
    //        return buf.ToByteArray();
    //    }

    //}
    ///// <summary>
    ///// 设置电机延时时间，返回
    ///// </summary>
    //internal class CL321_SetElectromotorTimePacketReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_SetElectromotorTimePacketReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_SetElectromotorTimePacketReplayPacket";
    //    }
    //    public override string GetPacketResolving()
    //    {
    //        return "返回：" + Pos + "表位，" + this.ReciveResult.ToString();
    //    }
    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;
    //        if (data == null || data.Length < 20)
    //        {
    //            LinkOk = false;
    //            this.ReciveResult = RecvResult.Unknow;
    //            return;
    //        }
    //        if (data[0] != 0x39)
    //        {
    //            this.ReciveResult = RecvResult.FrameError;
    //            LinkOk = false;
    //            return;
    //        }
    //        if (data[18] == 0 && data[17] == 0 && data[16] == 0 && data[15] == 0)
    //        {
    //            this.Pos = data[19];
    //            this.ReciveResult = RecvResult.OK;
    //        }
    //        else
    //        {
    //            LinkOk = false;
    //            this.Pos = data[19];
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}
    //#endregion

    //#region CL321读电机延时时间 0x40|0x41
    ///// <summary>
    ///// 读电机延时时间，发送
    ///// </summary>
    //internal class CL321_ReadElectromotorTimePacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x40;

    //    public override int WaiteTime()
    //    {
    //        return 200;
    //    }
    //    /// <summary>
    //    /// 读电机延时时间
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="iChannelNum">总线数</param>
    //    public CL321_ReadElectromotorTimePacket(bool[] bwstatus, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.BwStatus = bwstatus;
    //    }

    //    public CL321_ReadElectromotorTimePacket()
    //    {
    //    }
    //    /// <summary>
    //    /// 包名
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketName()
    //    {
    //        return "CL321_ReadElectromotorTimePacket";
    //    }
    //    /// <summary>
    //    /// 解析
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketResolving()
    //    {
    //        return "读电机延时时间";
    //    }
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }

    //}
    ///// <summary>
    ///// 读电机延时时间，返回
    ///// </summary>
    //internal class CL321_ReadElectromotorTimePacketReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_ReadElectromotorTimePacketReplayPacket()
    //        : base()
    //    {
    //    }
    //    /// <summary>
    //    /// 上延时时间
    //    /// </summary>
    //    public int UpDelayTime = 0;
    //    /// <summary>
    //    /// 下延时时间
    //    /// </summary>
    //    public int DownDelayTime = 0;
    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_ReadElectromotorTimePacketReplayPacket";
    //    }

    //    public override string GetPacketResolving()
    //    {
    //        return "返回" + this.ReciveResult.ToString() + "：" + Pos + "表位，上" + this.UpDelayTime.ToString() + ",下" + this.DownDelayTime.ToString();
    //    }
    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;
    //        if (data == null || data.Length < 28)
    //        {
    //            LinkOk = false;
    //            this.ReciveResult = RecvResult.Unknow;
    //            return;
    //        }
    //        ByteBuffer buf = new ByteBuffer(data);

    //        if (buf.Get() != 0x41)
    //        {
    //            this.ReciveResult = RecvResult.FrameError;
    //            LinkOk = false;
    //            return;
    //        }
    //        buf.Get();
    //        buf.Get();
    //        buf.GetByteArray(12);
    //        if (buf.GetInt() == 0)
    //        {
    //            this.Pos = buf.Get();// data[19];
    //            this.UpDelayTime = buf.GetInt_S();
    //            this.DownDelayTime = buf.GetInt_S();
    //            this.ReciveResult = RecvResult.OK;
    //        }
    //        else
    //        {
    //            LinkOk = false;
    //            this.Pos = buf.Get();
    //            this.UpDelayTime = buf.GetInt_S();
    //            this.DownDelayTime = buf.GetInt_S();
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}
    //#endregion

    //#region CL88M读取温度0x82|0x83
    ///// <summary>
    ///// 读取温度
    ///// </summary>
    //internal class CL321_RequestReadBwTemperaturePacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x82;
    //    /// <summary>
    //    /// 读取类型0，A相温度；1，B相温度；2，C相温度
    //    /// </summary>
    //    public int m_intReadType = 0;
    //    public CL321_RequestReadBwTemperaturePacket()
    //        : base(true)
    //    {

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="seperate">读取类型0，A相温度；1，B相温度；2，C相温度</param>
    //    public void SetPara(bool[] bwstatus, int intReadType)
    //    {
    //        this.m_intReadType = intReadType;
    //        this.BwStatus = bwstatus;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadBwTemperaturePacket";
    //    }
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(m_intReadType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 读取温度
    ///// </summary>
    //internal class CL321_RequestReadBwTemperatureReplyPacket : CL321RecvPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    public byte bCmd { get; private set; }

    //    /// <summary>
    //    /// bytes List
    //    /// </summary>
    //    public byte[] BwChannelList { get; private set; }

    //    /// <summary>
    //    /// 误差板编号
    //    /// </summary>
    //    public int wcbIndex { get; private set; }
    //    /// <summary>
    //    /// 是否返回错误 
    //    /// </summary>
    //    public bool m_bError { get; private set; }
    //    /// <summary>
    //    /// 温度
    //    /// </summary>
    //    public string[] m_strTemperature { get; private set; }

    //    /// <summary>
    //    /// 系统解析帧  
    //    /// </summary>
    //    /// <param name="data"></param>
    //    ///         
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）   
    //     * +误差板编号（1 byte）+  误差板软件版本号（1 byte）。
    //     * 软件版本号的表示采用BCD编码方式编码，小数点不表示。
    //     * 	例：如果版本号为1.1，则版本号早数据帧中被表示为  0x11。
    //     */
    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 20) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        buf.Get();                                  //广播标志(0xFFH)
    //        buf.Get();                                  //1 byte ListLen                  
    //        BwChannelList = buf.GetByteArray(12);       // List
    //        //wcbIndex = buf.Get();                       //误差板编号//协议文档不对

    //        if (BitConverter.ToInt32(buf.GetByteArray(4), 0) == 0)
    //        {
    //            m_bError = false;
    //        }
    //        else
    //        {
    //            m_bError = true;
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //        m_strTemperature = new string[4];
    //        if (!m_bError)
    //        {
    //            byte[] byt1 = buf.GetByteArray(4);
    //            byte[] byt2 = buf.GetByteArray(4);
    //            byte[] byt3 = buf.GetByteArray(4);
    //            byte[] byt4 = buf.GetByteArray(4);

    //            if (BitConverter.ToString(byt1, 0) == "FF-FF-FF-FF")
    //            {
    //                m_strTemperature[0] = "FF-FF-FF-FF";
    //            }
    //            else
    //            {
    //                m_strTemperature[0] = (BitConverter.ToInt32(byt1, 0) / 1000F).ToString("F3");//Convert.ToString
    //            }
    //            if (BitConverter.ToString(byt2, 0) == "FF-FF-FF-FF")
    //            {
    //                m_strTemperature[1] = "FF-FF-FF-FF";
    //            }
    //            else
    //            {
    //                m_strTemperature[1] = (BitConverter.ToInt32(byt2, 0) / 1000F).ToString("F3");//Convert.ToString
    //            }
    //            if (BitConverter.ToString(byt3, 0) == "FF-FF-FF-FF")
    //            {
    //                m_strTemperature[2] = "FF-FF-FF-FF";
    //            }
    //            else
    //            {
    //                m_strTemperature[2] = (BitConverter.ToInt32(byt3, 0) / 1000F).ToString("F3");//Convert.ToString
    //            }

    //            if (BitConverter.ToString(byt4, 0) == "FF-FF-FF-FF")
    //            {
    //                m_strTemperature[3] = "FF-FF-FF-FF";
    //            }
    //            else
    //            {
    //                m_strTemperature[3] = (BitConverter.ToInt32(byt4, 0) / 1000F).ToString("F3");
    //            }

    //            wcbIndex = buf.Get();
    //            this.ReciveResult = RecvResult.OK;
    //        }
    //        else
    //        {
    //            //m_strTemperature = "";
    //        }
    //    }
    //}
    //#endregion

    //#region CL321继电器控制指令0X84以及回复0X85

    //internal class CL321_RequestSetSwitchPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 继电器ID
    //    /// </summary>
    //    private byte RelayID = 0;
    //    /// <summary>
    //    /// 控制类型0,断开；1，闭合
    //    /// </summary>
    //    private byte ControlType = 0;

    //    private byte m_Cmd = 0x84;

    //    public CL321_RequestSetSwitchPacket(byte relayID,byte controlType)
    //        : base()
    //    {
    //        RelayID = relayID;
    //        ControlType = controlType;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetSwitchPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(RelayID);
    //        buf.Put(ControlType);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestSetSwitchReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetSwitchReplayPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetSwitchReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x85)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321查询继电器状态命令0x86以及回复0x87

    //internal class CL321_RequestQuerySwitchStatusPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0x86;

    //    private byte switchID = 0;

    //    public CL321_RequestQuerySwitchStatusPacket(byte data)
    //        : base()
    //    {
    //        switchID = data;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQuerySwitchStatusPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(switchID);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQuerySwitchStatusReplayPacket : ClouRecvPacket_CLT11
    //{

    //    public byte SwitchStatus = 0;

    //    public CL321_RequestQuerySwitchStatusReplayPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQuerySwitchStatusReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0x87)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                SwitchStatus = data[19];
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 检定装置信息设置命令0xA0以及回复0xA1

    //internal class CL321_RequestSetEquipInformationPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xA0;

    //    //电能表类型设置如下：
    //    //Bit0：0表示国网电能表，1表示南网电能表
    //    //Bit1：0表示单相电能表，1表示三相电能表
    //    //Bit2：0表示电能表，1表示终端
    //    //其他位保留
    //    private byte MeterType = 0x00;

    //    //0x00代表不带CT
    //    //0x01代表2030-3A
    //    //0x02代表2030-3B
    //    //0x03代表2030-3C
    //    private byte CtType = 0x00;

    //    public CL321_RequestSetEquipInformationPacket(byte meterType,byte ctType)
    //        : base()
    //    {
    //        MeterType = meterType;
    //        CtType = ctType;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetEquipInformationPacket";
    //    }


    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(MeterType);
    //        buf.Put(CtType);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestSetEquipInformationReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetEquipInformationReplayPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetEquipInformationReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xA1)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321 检定装置信息读取命令0xA2以及回复0xA3

    //internal class CL321_RequestReadEquipInformationPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xA2;

    //    public CL321_RequestReadEquipInformationPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadEquipInformationPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestReadEquipInformationReplayPacket : ClouRecvPacket_CLT11
    //{
    //    //解析后的数据表位号
    //    public byte MeterNumber = 0;

    //    public byte MeterType = 0;

    //    public byte CtType = 0;

    //    public CL321_RequestReadEquipInformationReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadEquipInformationReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xA3)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 21)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;

    //                MeterNumber = data[19];//电表的表位号
    //                MeterType = data[20];//电表类型
    //                CtType = data[21];//Ct类型
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }

    //    }
    //}

    //#endregion

    //#region CL321设置被检脉冲通道及检定类型命令0xA7以及回复0xA6
    ///// <summary>
    ///// 选择被检脉冲通道及检定类型
    ///// </summary>
    //internal class CL321_RequestSelectPulseChannelAndCheckTypePacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xA7;

    //    /// <summary>
    //    /// 电能误差通道号,0P+ 、1P-、 2Q+、 3Q-
    //    /// </summary>
    //    private Cus_EmMeterWcChannelNo wcChannelNo;

    //    /// <summary>
    //    /// 光电头选择位,1为感应式脉冲输入，0为电子式脉冲输入
    //    /// </summary>
    //    private Cus_EmPulseType pulseType;

    //    /// <summary>
    //    /// 脉冲极性选择(共阳/共阴),0表示公共端输出低电平（共阴），1表示公共端输出高电平（共阳）
    //    /// </summary>
    //    private Cus_EmGyGyType GyGy;

    //    /// <summary>
    //    /// 多功能误差通道号,1为日计时脉冲、2为需量脉冲。
    //    /// </summary>
    //    private Cus_EmDgnWcChannelNo dgnWcChannelNo;

    //    /// <summary>
    //    /// 检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验
    //    /// </summary>
    //    private Cus_EmCheckType checkType;

    //    public CL321_RequestSelectPulseChannelAndCheckTypePacket()
    //        : base(true)
    //    { }


    //    /// <summary>
    //    /// 设置参数
    //    /// </summary>
    //    /// <param name="bwstatus">电表状态</param>
    //    /// <param name="wcchannelno">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟,0x06：耐压实验 0x07：多功能脉冲计数试验</param>
    //    /// <param name="pulsetype">通道类型,0=脉冲盒,1=光电头</param>
    //    /// <param name="gygy">脉冲类型,0=共阳,1=共阴</param>
    //    /// <param name="dgnwcchannelno">多功能误差通道号,1=日计时，2=需量脉冲</param>
    //    /// <param name="checktype">检定类型</param>
    //    public void SetPara(Cus_EmMeterWcChannelNo wcchannelno, Cus_EmPulseType pulsetype, Cus_EmGyGyType gygy, Cus_EmDgnWcChannelNo dgnwcchannelno, Cus_EmCheckType checktype, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        //BwStatus = bwstatus;
    //        this.wcChannelNo = wcchannelno;
    //        this.pulseType = pulsetype;
    //        this.GyGy = gygy;
    //        this.dgnWcChannelNo = dgnwcchannelno;
    //        this.checkType = checktype;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectPulseChannelAndCheckTypePacket";
    //    }
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 被检脉冲通道号（2Byte）+ 检定类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        //计算第一个字节
    //        string byte1 = Convert.ToString((int)wcChannelNo, 2).PadLeft(3, '0');
    //        byte1 = ((int)GyGy).ToString() + ((int)pulseType).ToString() + byte1;
    //        buf.Put(Str2ToByte(byte1));
    //        //计算第二个字节
    //        buf.Put(Convert.ToByte((int)dgnWcChannelNo));
    //        buf.Put(Convert.ToByte((int)checkType));
    //        return buf.ToByteArray();
    //    }
    //    public override string GetPacketResolving()
    //    {
    //        string strResolve = "设置误差板参数：" + wcChannelNo.ToString() + GyGy.ToString() + pulseType.ToString() + dgnWcChannelNo.ToString() + checkType.ToString();
    //        return strResolve;
    //    }

    //}

    ///// <summary>
    ///// 选择被检脉冲通道及检定类型指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSelectPulseChannelAndCheckTypeReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSelectPulseChannelAndCheckTypeReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectPulseChannelAndCheckTypeReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xA6)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321查询被检脉冲通道及检定类型命令0xA8以及回复0xA9

    //internal class CL321_RequestQueryPusleChannelAndCheckTypePacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xA8;

    //    public CL321_RequestQueryPusleChannelAndCheckTypePacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryPusleChannelAndCheckTypePacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryPusleChannelAndCheckTypeReplayPacket : ClouRecvPacket_CLT11
    //{
    //    /// <summary>
    //    /// 误差通道
    //    /// </summary>
    //    public Cus_EmMeterWcChannelNo wcChannelNo;
    //    /// <summary>
    //    /// 脉冲类型
    //    /// </summary>
    //    public Cus_EmPulseType pulseType;

    //    /// <summary>
    //    /// 脉冲极性选择(共阳/共阴),0表示公共端输出低电平（共阴），1表示公共端输出高电平（共阳）
    //    /// </summary>
    //    public Cus_EmGyGyType GyGy;
    //    /// <summary>
    //    /// 多功能 0误差通道号,1为日计时脉冲、2为需量脉冲。
    //    /// </summary>
    //    public Cus_EmDgnWcChannelNo dgnWcChannelNo;


    //    /// <summary>
    //    /// 检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验
    //    /// </summary>
    //    public Cus_EmCheckType checkType;

    //    //被检脉冲类型 1 标准表高速脉冲类型 0 普通电能表 低速脉冲类型
    //    public byte verificationPusleType = 0;

    //    public CL321_RequestQueryPusleChannelAndCheckTypeReplayPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryPusleChannelAndCheckTypeReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xA9)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 17)
    //        {
    //            //byte[] byteInfor = new byte[4];
    //            //if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                byte enegyError = Convert.ToByte(data[15] & 0x03);
    //                wcChannelNo = (Cus_EmMeterWcChannelNo)enegyError;

    //                pulseType = (data[15] & 0x08) == 0x00 ? Cus_EmPulseType.脉冲盒 : Cus_EmPulseType.光电头;

    //                GyGy = (data[15] & 0x10) == 0x00 ? Cus_EmGyGyType.共阴 : Cus_EmGyGyType.共阳;

    //                verificationPusleType = (data[15] & 0x20) == 0x00 ? Convert.ToByte(0x00) : Convert.ToByte(0x01);
    //                if (data[16] < 3)
    //                {
    //                    dgnWcChannelNo = (Cus_EmDgnWcChannelNo)data[16];
    //                }

    //                if (data[17] < 8)
    //                {
    //                    checkType = (Cus_EmCheckType)data[17];
    //                }

    //            }
    //        }
    //        else
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}

    //#endregion

    //#region CL321查询双回路命令0xAA以及回复0xAB

    //internal class CL321_RequestQueryDoublecontourPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xAA;

    //    public CL321_RequestQueryDoublecontourPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryDoublecontourPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryDoublecontourReplayPacket : ClouRecvPacket_CLT11
    //{
    //    /// <summary>
    //    /// 电流的输出回路
    //    /// </summary>
    //    public Cus_EmBothIRoadType iRoad;

    //    /// <summary>
    //    /// 电压回路选择
    //    /// </summary>
    //    public Cus_EmBothVRoadType vRoad;

    //    public CL321_RequestQueryDoublecontourReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryDoublecontourReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0x87)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;

    //                if (data[19] < 2)
    //                {
    //                    iRoad = (Cus_EmBothIRoadType)data[19];
    //                }
    //                if (data[20] < 3)
    //                {
    //                    vRoad = (Cus_EmBothVRoadType)data[20];
    //                }
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }
    //    }
    //}
    //#endregion

    //#region CL321光点头选择命令0xAC以及回复0xAD
    ///// <summary>
    ///// 光电头状态选择
    ///// 通讯选择： 
    ///// 0x00表示选择一对一模式485通讯（默认模式）；
    ///// 0X01表示选择奇数表位485通讯；
    ///// 0X02表示选择偶数表位485通讯；
    ///// 0x03表示选择一对一模式红外通讯；
    ///// 0X04表示选择奇数表位红外通讯；
    ///// 0X05表示选择偶数表位红外通讯；
    ///// 0X06表示选择切换到485总线（电科院协议用）。
    ///// </summary>
    //internal class CL321_RequestSelectLightStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xAC;

    //    /// <summary>
    //    /// 0x00表示选择一对一模式485通讯（默认模式）；0X01表示选择奇数表位485通讯；0X02表示选择偶数表位485通讯；
    //    /// 0x03表示选择一对一模式红外通讯；0X04表示选择奇数表位红外通讯；0X05表示选择偶数表位红外通讯；0X06表示选择切换到485总线（电科院协议用）。
    //    /// </summary>
    //    private Cus_EmLightSelect selectType;

    //    public CL321_RequestSelectLightStatusPacket(bool[] bwstatus)
    //        : base(true)
    //    {
    //        BwStatus = bwstatus;
    //        this.selectType = Cus_EmLightSelect.一对一模式485通讯;
    //    }
    //    public CL321_RequestSelectLightStatusPacket():base()
    //    {
    //    }

    //    /// <summary>
    //    /// 设置参数
    //    /// </summary>
    //    /// <param name="bwstatus">电表状态</param>
    //    /// <param name="selecttype">通讯选择</param>
    //    public void SetPara(bool[] bwstatus, Cus_EmLightSelect selecttype, int iChannelNo, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.ChannelNo = iChannelNo;
    //        BwStatus = bwstatus;
    //        this.selectType = selecttype;
    //    }
    //    public void SetPara(int id, Cus_EmLightSelect selecttype)
    //    {
    //        this.Pos = id;
    //        this.selectType = selecttype;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectLightStatusPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 通讯选择（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte((int)selectType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 光电头状态选择指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSelectLightStatusReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSelectLightStatusReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectLightStatusReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xAD)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321双回路检定时，选择其中的某一路作为电流的输出回路 0xAF以及回复0xAE
    ///// <summary>
    ///// 用于双回路检定时，选择其中的某一路作为电流的输出回路
    ///// </summary>
    //internal class CL321_RequestSelectCheckRoadPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xAF;

    //    /// <summary>
    //    /// 电流的输出回路
    //    /// </summary>
    //    private Cus_EmBothIRoadType iRoad;

    //    /// <summary>
    //    /// 电压回路选择
    //    /// </summary>
    //    private Cus_EmBothVRoadType vRoad;

    //    public CL321_RequestSelectCheckRoadPacket()
    //        : base(false)
    //    { }

    //    public CL321_RequestSelectCheckRoadPacket(bool[] bwstatus)
    //        : base(false)
    //    {
    //        this.BwStatus = bwstatus;
    //        this.iRoad = Cus_EmBothIRoadType.第一个电流回路;
    //        this.vRoad = Cus_EmBothVRoadType.直接接入式;
    //    }

    //    /// <summary>
    //    /// 用于双回路检定时，选择其中的某一路作为电流的输出回路；0x00表示第一个电流回路，0x01表示第二个电流回路，系统默认为第一个电流回路。
    //    /// 选择电压回路时，0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入，系统默认为直接接入式电表电压回路。
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="iroad"></param>
    //    /// <param name="vroad"></param>
    //    public CL321_RequestSelectCheckRoadPacket(bool[] bwstatus, Cus_EmBothIRoadType iroad, Cus_EmBothVRoadType vroad, int iChannelNum, int iChannelNo)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.ChannelNo = iChannelNo;
    //        this.BwStatus = bwstatus;
    //        this.iRoad = iroad;
    //        this.vRoad = vroad;
    //    }
    //    public CL321_RequestSelectCheckRoadPacket(int id, Cus_EmBothIRoadType iroad, Cus_EmBothVRoadType vroad)
    //        : base(true)
    //    {
    //        this.Pos = id;
    //        this.iRoad = iroad;
    //        this.vRoad = vroad;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectCheckRoadPacket";
    //    }

    //    /// <summary>
    //    /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 电流回路路数（1Byte） + 电压回路路数（1Byte）。
    //    /// </summary>
    //    /// <returns></returns>
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte((int)iRoad));
    //        buf.Put(Convert.ToByte((int)vRoad));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 用于双回路检定时，选择其中的某一路作为电流的输出回路指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSelectCheckRoadReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSelectCheckRoadReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSelectCheckRoadReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xAE)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321启动误差板指令 B1|B0
    ///// <summary>
    ///// 启动计算功能指令
    ///// </summary>
    //internal class CL321_RequestStartPCFunctionPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xB1;

    //    /// <summary>
    //    /// 检定类型
    //    /// </summary>
    //    private Cus_EmCheckType checkType;


    //    /// <summary>
    //    /// 启动计算功能指令，若表位列表中某一位置1则启动对应表位检定，为0则不启动，若List = 0x30H，则只启动第5和第6表位的检定；检定类型设置同A7指令
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestStartPCFunctionPacket()
    //        : base(true)
    //    {

    //    }

    //    public void SetPara(bool[] bwstatus, Cus_EmCheckType checktype, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.checkType = checktype;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetPara(bool[] bwstatus, Cus_EmCheckType checktype)
    //    {
    //        this.Pos = 0;
    //        this.checkType = checktype;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetPara(int id, Cus_EmCheckType checktype)
    //    {
    //        this.Pos = id;
    //        this.checkType = checktype;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartPCFunctionPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        int iType = (int)checkType;
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(iType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 启动误差板指令，返回数据包  
    ///// </summary>
    //internal class CL321_RequestStartPCFunctionReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStartPCFunctionReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStartPCFunctionReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xB0)//到底是B0 指令呢，还是F0 指令呢 2015年7月16日 17:21:21
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321停止误差板B2|B3
    ///// <summary>
    ///// 停止误差板计算功能指令
    ///// </summary>
    //internal class CL321_RequestStopPCFunctionPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xB2;

    //    /// <summary>
    //    /// 检定类型
    //    /// </summary>
    //    private Cus_EmCheckType checkType;


    //    public CL321_RequestStopPCFunctionPacket()
    //        : base(false)
    //    { }

    //    /// <summary>
    //    /// 停止计算功能指令，若表位列表中某一位置1则停止对应表位检定，为0则不改变，若List = 0x30H，
    //    /// 则停止第5和第6表位的检定；检定类型设置同A7指令，自动检表线上，下发07指令停止所有的检定。
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestStopPCFunctionPacket(bool[] bwstatus, Cus_EmCheckType checktype, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.isStart = true;
    //        this.Pos = 0;
    //        this.checkType = checktype;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetParam(int id,Cus_EmCheckType checkType)
    //    {
    //        this.checkType = checkType;
    //        this.Pos = id;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStopPCFunctionPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte((int)checkType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 启动误差板指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestStopPCFunctionReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestStopPCFunctionReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestStopPCFunctionReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xB3)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321隔离表位0xB4|0xB7
    ///// <summary>
    ///// 故障表位电压电流隔离控制、次级开路试验
    ///// </summary>
    //internal class CL321_RequestSeperateBwControlPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xB4;

    //    /// <summary>
    //    /// 隔离/恢复
    //    /// </summary>
    //    private static int Seperate = 0;

    //    private static byte[] mChannelByte = new byte[12];
    //    /// <summary>
    //    /// 隔离/恢复,需要发两次指令，先隔离需要隔离的表位，再恢复掉需要恢复的表位
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="seperate">True为隔离，False为恢复</param>
    //    public CL321_RequestSeperateBwControlPacket()
    //        : base(true)
    //    {

    //    }
    //    public void SetPara( bool[] bwstatus, bool seperate,int iChannelNo, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        BwStatus = bwstatus;
    //        Seperate = seperate ? 1 : 0;
    //        mChannelByte = SeperateBwToChannelByte(iChannelNo, bwstatus, seperate);
    //    }

    //    public void SetPara(int id,  int seperate)
    //    {
    //        this.Pos = id;
    //        Seperate = seperate;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSeperateBwControlPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+隔离控制状态（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        //for (int intInc = 0; intInc < ChannelByte.Length; intInc++)
    //        {
    //            //if (ChannelByte[intInc] == 0x00 && intInc == ChannelByte.Length - 1)
    //            //{
    //            //    return null;
    //            //}
    //            //else
    //            //    continue;
    //        }
    //        buf.Put(mChannelByte);
    //        buf.Put(Convert.ToByte(Seperate));
    //        return buf.ToByteArray();
    //    }

    //    /// <summary>
    //    /// 隔离故障表位,恢复正常表位
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <returns></returns>
    //    private byte[] SeperateBwToChannelByte(int ErrorNo, bool[] bwstatus, bool seperate)
    //    {
    //        int mChnlNum = BwNum / ChannelNum;
    //        string[] str = new string[96];
    //        string Strtmp = "";
    //        for (int z = 0; z < 96; z++)
    //        {
    //            if (z < bwstatus.Length)
    //            {

    //                if (z <= ((ErrorNo + 1) * mChnlNum - 1) && z >= ((ErrorNo) * mChnlNum))//TODO:15 10,已处理每条总线负载，可再优化
    //                {
    //                    if (bwstatus[z])
    //                    {
    //                        str[z] = "1";
    //                    }
    //                    else
    //                    {
    //                        str[z] = "0";
    //                    }
    //                }
    //                else
    //                {
    //                    str[z] = "0";
    //                }
    //            }
    //            else
    //            {
    //                str[z] = "0";
    //            }
    //        }

    //        for (int k = str.Length - 1; k >= 0; k--)
    //        {
    //            Strtmp += str[k];
    //        }
    //        byte[] Arrytmpbyte = new byte[12];


    //        byte tmpbyte = new byte();
    //        for (int i = 0; i < 12; i++)
    //        {
    //            tmpbyte = 0x00;

    //            for (int ii = 0; ii < 8; ii++)
    //            {

    //                tmpbyte += Convert.ToByte(Strtmp.Substring(Strtmp.Length - 1 - 8 * i - ii, 1).Equals("1") ? (Math.Pow(2, ii)) : 0x00);

    //            }
    //            Arrytmpbyte[11 - i] = tmpbyte;
    //        }
    //        return Arrytmpbyte;
    //    }
    //}

    ///// <summary>
    ///// 启动误差板指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSeperateBwControlReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSeperateBwControlReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSeperateBwControlReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xB7)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321 查询表位电压电流隔离命令0xB8以及回复0xB9

    //internal class CL321_RequestQuerySeperateBwControlPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xB8;

    //    public CL321_RequestQuerySeperateBwControlPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQuerySeperateBwControlPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQuerySeperateBwControlReplayPacket : ClouRecvPacket_CLT11
    //{

    //    /// <summary>
    //    /// 隔离/恢复
    //    /// </summary>
    //    public  int Seperate = 0;

    //    public CL321_RequestQuerySeperateBwControlReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQuerySeperateBwControlReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xB9)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 15)
    //        {
    //            //if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                Seperate = data[15];
    //            }
    //        }
    //        else
    //        {
    //                this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}

    //#endregion

    //#region CL321耐压继电器控制设置 0xBA|0xBB
    ///// <summary>
    /////  设置误差板继电器指令控制状态命令
    ///// </summary>
    //internal class CL321_SendCMDToInsulationRelays : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xba;
    //    /// <summary>
    //    /// 控制命令
    //    /// </summary>
    //    private int cmd;

    //    public CL321_SendCMDToInsulationRelays()
    //        : base(true)
    //    {
    //    }
    //    public void SetPara(bool[] bwstatus, int _cmd, int iChannelNum)
    //    {
    //        this.cmd = _cmd;
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetPara(int id, int _cmd)
    //    {
    //        this.cmd = _cmd;
    //        this.Pos = id;
    //    }
    //    /// <summary>
    //    /// 包名
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketName()
    //    {
    //        return " CL321_SendCMDToInsulationRelays";
    //    }
    //    /// <summary>
    //    /// 解析
    //    /// </summary>
    //    /// <returns></returns>
    //    public override string GetPacketResolving()
    //    {
    //        return "发送设置耐压继电器指令控制状态命令";
    //    }
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(Convert.ToByte(AllFlag));
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(this.cmd));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    /////  设置误差板继电器指令控制状态命令结果 返回
    ///// </summary>
    //internal class CL321_SendCMDToInsulationRelaysReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public override string GetPacketName()
    //    {
    //        return "CL321_SendCMDToInsulationRelaysReplayPacket";
    //    }
    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }
    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;
    //        if (data == null || data.Length < 17)
    //        {
    //            LinkOk = false;
    //            this.ReciveResult = RecvResult.Unknow;
    //            return;
    //        }
    //        if (data[0] != 0xbb)
    //        {
    //            this.ReciveResult = RecvResult.FrameError;
    //            LinkOk = false;
    //            return;
    //        }
    //        else
    //        {
    //            if (data.Length > 18)
    //            {
    //                int iData = BitConverter.ToInt32(data, 15);
    //                if (iData == 0)
    //                {
    //                    this.ReciveResult = RecvResult.OK;
    //                }
    //            }
    //        }
    //    }
    //}
    //#endregion

    //#region CL321查询耐压继电器指令控制状态命令0xBC以及回复0xBD

    //internal class CL321_RequestQueryCMDToInsulationRelaysPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xBC;

    //    public CL321_RequestQueryCMDToInsulationRelaysPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryCMDToInsulationRelaysPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(Convert.ToByte(AllFlag));
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryCMDToInsulationRelaysRePlayPacket : ClouRecvPacket_CLT11
    //{
    //    /// <summary>
    //    /// 耐压继电器状态。
    //    /// </summary>
    //    public byte InsulationStatus = 0;

    //    public CL321_RequestQueryCMDToInsulationRelaysRePlayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return base.GetPacketName();
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {

    //        if (data[0] != 0xBD)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 15)
    //        {
    //            this.ReciveResult = RecvResult.OK;
    //            InsulationStatus = data[15];
    //        }
    //        else
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}

    //#endregion

    //#region CL321 CT电流档位选择控制命令0xB5以及回复0xB6
    ///// <summary>
    ///// CT电流档位选择控制
    ///// </summary>
    //internal class CL321_RequestCTPositionChannelControlPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xB5;

    //    /// <summary>
    //    /// 电流档位,01表示100A档位、02表示2A档位。
    //    /// </summary>
    //    private Cus_EmIChannelType iType;

    //    public CL321_RequestCTPositionChannelControlPacket(bool[] bwstatus, Cus_EmIChannelType itype, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.BwStatus = bwstatus;
    //        this.iType = itype;
    //    }

    //    public CL321_RequestCTPositionChannelControlPacket(int id, Cus_EmIChannelType itype)
    //        : base(true)
    //    {
    //        this.Pos = id;
    //        this.iType = itype;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestCTPositionChannelControlPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 档位选择（1Byte）
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte((int)iType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// CT电流档位选择控制指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestCTPositionChannelControlReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestCTPositionChannelControlReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestCTPositionChannelControlReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xB6)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321 联机操作指令0xC0以及回复0x54
    ///// <summary>
    ///// 188联机操作请求包
    ///// </summary>
    //internal class CL321_RequestLinkPacket : CL321SendPacket
    //{

    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x80;

    //    public CL321_RequestLinkPacket()
    //        : base(true)
    //    {
    //        this.Pos = 0;
    //    }

    //    public CL321_RequestLinkPacket(bool[] bwstatus, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.BwStatus = bwstatus;
    //    }

    //    public CL321_RequestLinkPacket(bool[] bwstatus, int iChannelNo, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.ChannelNo = iChannelNo;
    //        this.BwStatus = bwstatus;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestLinkPacket";
    //    }
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
    //    */
    //    public override byte[] GetBody()
    //    {
    //        return StringToBytes(GetWrite3201(Pos, "1203", "01"));
    //    }

    //}
    ///// <summary>
    ///// 联机指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestLinkReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestLinkReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestLinkReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0x50)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321读取各种类型误差及当前各种状态 0xC0以及回复0x50
    ///// <summary>
    ///// 读取表位各类型误差及各种状态
    ///// </summary>
    //internal class CL321_RequestReadBwWcAndStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xC0;

    //    /// <summary>
    //    /// 误差类型
    //    /// </summary>
    //    private int wcBoardQueryType;

    //    /// <summary>
    //    /// 查询误差板当前误差及当前状态指令,C0指令默认查询表位状态。注：此指令只返回当前误差值。
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="wcType">检定误差类型</param>
    //    public CL321_RequestReadBwWcAndStatusPacket(Cus_EmWuchaType wcType)
    //        : base(true)
    //    {
    //        this.wcBoardQueryType = (int)wcType;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadBwWcAndStatusPacket";
    //    }

    //    /// <summary>
    //    /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）。
    //    /// </summary>
    //    /// <returns></returns>
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(wcBoardQueryType));
    //        return buf.ToByteArray();
    //    }

    //}
    //internal class CL321_RequestReadBwWcAndStatusReplyPacket : CL321RecvPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    public byte bCmd { get; private set; }
    //    /// <summary>
    //    /// 检定误差类型
    //    /// </summary>
    //    public Cus_EmCheckType CheckType { get; private set; }
    //    /// <summary>
    //    /// bytes List
    //    /// </summary>
    //    public byte[] BwChannelList { get; private set; }
    //    /// <summary>
    //    /// 当前表位编号
    //    /// </summary>
    //    public int CurBwNum { get; private set; }
    //    /// <summary>
    //    /// 误差次数,即当前误差值是计算得到的第几个误差值
    //    /// </summary>
    //    public int wcNum { get; private set; }

    //    /// <summary>
    //    /// 误差放大倍数（Bit0~Bit6）
    //    /// </summary>
    //    public int openTimes { get; private set; }

    //    /// <summary>
    //    /// 误差值
    //    /// </summary>
    //    public string wcData { get; private set; }

    //    /// <summary>
    //    /// 状态类型
    //    /// </summary>
    //    public byte statusType { get; private set; }

    //    public int MeterConst { get; private set; }
    //    /// <summary>
    //    /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
    //    /// </summary>
    //    public Cus_EmBothIRoadType iType { get; private set; }
    //    /// <summary>
    //    /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
    //    /// </summary>
    //    public Cus_EmBothVRoadType vType { get; private set; }
    //    /// <summary>
    //    /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
    //    /// </summary>
    //    public int ConnType { get; private set; }

    //    /*
    //     * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
    //     * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
    //    */
    //    /// <summary>
    //    /// 接线故障状态,为true则表示该表位有故障,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Jxgz { get; private set; }

    //    /// <summary>
    //    /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Yfftz { get; private set; }

    //    /// <summary>
    //    /// 报警信号状态,为true则表示该表位报警,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Bjxh { get; private set; }

    //    /// <summary>
    //    /// 对标状态,为true则表示该表位对标完成,false为未完成对标
    //    /// </summary>
    //    public bool statusTypeIsOnOver_Db { get; private set; }
    //    /// <summary>
    //    /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Temp { get; private set; }
    //    /// <summary>
    //    /// 光电信号状态（false：未挂表；true：已挂表）
    //    /// </summary>
    //    public bool statusTypeIsOn_HaveMeter { get; private set; }
    //    /// <summary>
    //    /// 表位上限限位状态（false：未就位；true：就位）
    //    /// </summary>
    //    public bool statusTypeIsOn_PressUpLimit { get; private set; }
    //    /// <summary>
    //    /// 表位下限限位状态（false：未就位；true：就位）
    //    /// </summary>
    //    public bool statusTypeIsOn_PressDownLimt { get; private set; }

    //    /*
    //     * 工作状态：电能误差(Bit0)、需量周期误差（Bit1）、日计时误差（Bit2）、脉冲个数（Bit3）、对标（Bit4）、光电脉冲个数（Bit7）
    //     * 1Byte中为1表示对应计算功能已启动；为0表示对应计算功能已停止。 
    //    */
    //    /// <summary>
    //    /// 电能误差(Bit0),True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dn { get; private set; }

    //    /// <summary>
    //    /// 需量周期误差（Bit1）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Xlzq { get; private set; }

    //    /// <summary>
    //    /// 日计时误差（Bit2）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Rjs { get; private set; }

    //    /// <summary>
    //    /// 脉冲个数（Bit3）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dnzz { get; private set; }

    //    /// <summary>
    //    /// 对标（Bit4）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Db { get; private set; }
    //    /// <summary>
    //    /// 预付费（bit5),True表示对应计算功能已启动；为False表示对应计算功能已经停止
    //    /// </summary>
    //    public bool workStatusIsOn_Yff { get; private set; }
    //    /// <summary>
    //    /// 耐压状态（bit6)True表示对应计算功能已启动；为False表示对应计算功能已经停止
    //    /// </summary>
    //    public bool workStatusIsOn_Ny { get; private set; }

    //    /// <summary>
    //    /// 多功能脉冲计数（Bit7）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dgnmcjs { get; private set; }

    //    /// <summary>
    //    /// 系统解析帧  
    //    /// </summary>
    //    /// <param name="data"></param>
    //    ///         
    //    /*
    //     Data的组织方式为：
    //     * 广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte） + 当前表位编号（1Byte）
    //     * +误差次数（1Byte） + 误差值（4Bytes） + 状态类型（1Byte） + 电流回路状态（1Byte） + 电压回路状态（1Byte）
    //     * + 通讯口状态（1Byte） + 工作状态（1Byte）+发送标志1+发送标志2。
    //     */
    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 20) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        buf.Get();                                  //广播标志(0xFFH)
    //        buf.Get();                                  //1 byte ListLen                  
    //        BwChannelList = buf.GetByteArray(12);       // List
    //        CheckType = (Cus_EmCheckType)buf.Get();       //检定误差类型
    //        CurBwNum = buf.Get();                       //当前表位编号
    //        wcNum = buf.Get();
    //        //误差值包括误差数值和脉冲两种格式
    //        byte tmp = buf.Get();
    //        byte[] wcdata = buf.GetByteArray(3);
    //        if ((CheckType == Cus_EmCheckType.脉冲计数 || CheckType == Cus_EmCheckType.多功能脉冲计数试验) && tmp == 0)                                       //脉冲
    //        {
    //            wcData = get3ByteValue(wcdata, 0).ToString();   //脉冲个数
    //        }
    //        else                                                //误差数值
    //        {
    //            if (wcNum > 0 || CheckType == Cus_EmCheckType.耐压实验)
    //            {
    //                openTimes = ReplaceTargetBit(tmp, 7, false);    //误差放大倍数
    //                bool isz = GetbitValue(tmp, 7) == 0;            //误差符号（Bit7）0正误差 1负误差
    //                wcData = get3ByteValue(wcdata, 5).ToString();
    //                if (!isz) wcData = "-" + wcData;
    //            }
    //            else
    //            {
    //                wcData = "-999";
    //            }
    //        }

    //        #region 解析状态类型,为true则表示该表位有故障/跳闸/报警/对标完成，为false则表示正常/正常/正常/未完成对标

    //        statusType = buf.Get();
    //        //接线故障状态,为true则表示该表位有故障,false为正常
    //        statusTypeIsOnErr_Jxgz = GetbitValue(statusType, 0) == 1;
    //        //预付费跳闸状态,为true则表示该表位跳闸,false为正常
    //        statusTypeIsOnErr_Yfftz = GetbitValue(statusType, 1) == 1;
    //        //报警信号状态,为true则表示该表位报警,false为正常
    //        statusTypeIsOnErr_Bjxh = GetbitValue(statusType, 2) == 1;
    //        //对标状态,为true则表示该表位对标完成,false为未完成对标
    //        statusTypeIsOnOver_Db = GetbitValue(statusType, 3) == 1;
    //        // 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器        
    //        statusTypeIsOnErr_Temp = GetbitValue(statusType, 4) == 1;
    //        //光电信号状态（false：未挂表；true：已挂表）        
    //        statusTypeIsOn_HaveMeter = GetbitValue(statusType, 5) == 1;
    //        //表位上限限位状态（false：未就位；true：就位）
    //        statusTypeIsOn_PressUpLimit = GetbitValue(statusType, 6) == 1;
    //        //表位下限限位状态（false：未就位；true：就位）
    //        statusTypeIsOn_PressDownLimt = GetbitValue(statusType, 7) == 1;

    //        #endregion

    //        iType = (Cus_EmBothIRoadType)buf.Get();           //电流回路状态
    //        vType = (Cus_EmBothVRoadType)buf.Get();           //电压回路状态
    //        ConnType = buf.Get();                           //通讯口状态

    //        #region 解析工作状态字节,1表示对应计算功能已启动；为0表示对应计算功能已停止。
    //        //工作状态（1Byte）
    //        byte workStatus = buf.Get();
    //        //电能误差
    //        workStatusIsOn_Dn = GetbitValue(workStatus, 0) == 1;
    //        //需量周期误差
    //        workStatusIsOn_Xlzq = GetbitValue(workStatus, 1) == 1;
    //        //日计时误差
    //        workStatusIsOn_Rjs = GetbitValue(workStatus, 2) == 1;
    //        //电能走字
    //        workStatusIsOn_Dnzz = GetbitValue(workStatus, 3) == 1;
    //        //对标
    //        workStatusIsOn_Db = GetbitValue(workStatus, 4) == 1;
    //        //预付费
    //        workStatusIsOn_Yff = GetbitValue(workStatus, 5) == 1;
    //        //耐压
    //        workStatusIsOn_Ny = GetbitValue(workStatus, 6) == 1;
    //        //光电脉冲个数
    //        workStatusIsOn_Dgnmcjs = GetbitValue(workStatus, 7) == 1;
    //        #endregion

    //        if (true == workStatusIsOn_Dgnmcjs)
    //        {
    //            if (data.Length >= 31)
    //            {
    //                //从启动计算到收到投切脉冲的毫秒数
    //                wcData = buf.GetInt_S().ToString();
    //            }
    //        }
    //    }
    //}
    //#endregion

    //#region CL321清除表位状态命令0xC2以及回复0xC1
    ///// <summary>
    ///// 清除表位状态
    ///// </summary>
    //internal class CL321_RequestClearBwStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xC2;

    //    /// <summary>
    //    ///0x01：清除接线故障状态（上位机通过解析50H或53H指令，发现有接线故障，需要在重新做所有检定前发C2H指令清除。而不能在每个试验点都发命令清除）
    //    ///0x02：清除预付费跳闸状态
    //    ///0x03：清除报警信号状态（暂时未使用）
    //    ///0x04：保留
    //    ///0x05：清除温度过高故障状态
    //    /// </summary>
    //    private int Clear = 0;

    //    /// <summary>
    //    /// 清除表位状态
    //    /// </summary>
    //    public CL321_RequestClearBwStatusPacket(byte status)
    //        : base(true)
    //    {
    //        Clear = status;
    //    }


    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="seperate">True为清除，False为不改变状态</param>
    //    public void SetPara(bool[] bwstatus, bool isclear, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Clear = isclear ? 1 : 0;
    //        this.BwStatus = bwstatus;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestClearBwStatusPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 状态类型（1Byte）+ 状态参数（12Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(Clear));
    //        return buf.ToByteArray();
    //    }

    //}
    ///// <summary>
    ///// 清除故障指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestClearBwStatusReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestClearBwStatusReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestClearBwStatusReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xc1)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321读取10次各类型误差及当前各种状态0xC3以及回复0x53
    ///// <summary>
    ///// 读取表位前10次各类型误差及当前各种状态
    ///// </summary>
    //internal class CL321_RequestReadBwLast10WcAndStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xC3;

    //    /// <summary>
    //    /// 误差类型
    //    /// </summary>
    //    private int wcBoardQueryType;

    //    /// <summary>
    //    /// 此指令与C0H指令的帧格式相同，区别为此指令要求误差板上报前10次误差及当前状态。
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="wcType">检定误差类型</param>
    //    public CL321_RequestReadBwLast10WcAndStatusPacket(bool[] bwstatus, Cus_EmWuchaType wcType)
    //        : base(true)
    //    {
    //        this.BwStatus = bwstatus;
    //        this.wcBoardQueryType = (int)wcType;
    //    }

    //    public CL321_RequestReadBwLast10WcAndStatusPacket(int id, Cus_EmWuchaType wcType)
    //        : base(true)
    //    {
    //        this.Pos = id;
    //        this.wcBoardQueryType = (int)wcType;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadBwLast10WcAndStatusPacket";
    //    }
    //    /// <summary>
    //    /// Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List）+ 检定误差类型（1Byte）。
    //    /// </summary>
    //    /// <returns></returns>
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(wcBoardQueryType));
    //        return buf.ToByteArray();
    //    }

    //}

    //internal class CL321_RequestReadBwLast10WcAndStatusReplyPacket : CL321RecvPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    public byte bCmd { get; private set; }

    //    /// <summary>
    //    /// 检定误差类型
    //    /// </summary>
    //    public Cus_EmCheckType CheckType { get; private set; }

    //    /// <summary>
    //    /// bytes List
    //    /// </summary>
    //    public byte[] BwChannelList { get; private set; }

    //    /// <summary>
    //    /// 当前表位编号
    //    /// </summary>
    //    public int CurBwNum { get; private set; }

    //    /// <summary>
    //    /// 误差次数,即当前误差值是计算得到的第几个误差值
    //    /// </summary>
    //    public int wcNum { get; private set; }

    //    /// <summary>
    //    /// 误差放大倍数（Bit0~Bit6）
    //    /// </summary>
    //    public int openTimes { get; private set; }

    //    /// <summary>
    //    /// 误差值
    //    /// </summary>
    //    public string[] wcData { get; private set; }

    //    /// <summary>
    //    /// 状态类型
    //    /// </summary>
    //    public int MeterConst { get; private set; }

    //    /// <summary>
    //    /// 电流回路状态,0x00表示第一个电流回路，0x01表示第二个电流回路
    //    /// </summary>
    //    public Cus_EmBothIRoadType iType { get; private set; }

    //    /// <summary>
    //    /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
    //    /// </summary>
    //    public Cus_EmBothVRoadType vType { get; private set; }

    //    /// <summary>
    //    /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
    //    /// </summary>
    //    public int ConnType { get; private set; }
    //    /*
    //     * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
    //     * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
    //    */
    //    /// <summary>
    //    /// 接线故障状态,为true则表示该表位有故障,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Jxgz { get; private set; }

    //    /// <summary>
    //    /// 预付费跳闸状态,为true则表示该表位跳闸,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Yfftz { get; private set; }

    //    /// <summary>
    //    /// 报警信号状态,为true则表示该表位报警,false为正常
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Bjxh { get; private set; }

    //    /// <summary>
    //    /// 对标状态,为true则表示该表位对标完成,false为未完成对标
    //    /// </summary>
    //    public bool statusTypeIsOnOver_Db { get; private set; }
    //    /// <summary>
    //    /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
    //    /// </summary>
    //    public bool statusTypeIsOnErr_Temp { get; private set; }
    //    /// <summary>
    //    /// 光电信号状态（false：未挂表；true：已挂表）
    //    /// </summary>
    //    public bool statusTypeIsOn_HaveMeter { get; private set; }
    //    /// <summary>
    //    /// 表位上限限位状态（false：未就位；true：就位）
    //    /// </summary>
    //    public bool statusTypeIsOn_PressUpLimit { get; private set; }
    //    /// <summary>
    //    /// 表位下限限位状态（false：未就位；true：就位）
    //    /// </summary>
    //    public bool statusTypeIsOn_PressDownLimt { get; private set; }

    //    /*
    //     * 工作状态：电能误差(Bit0)、需量周期误差（Bit1）、日计时误差（Bit2）、脉冲个数（Bit3）、对标（Bit4）、光电脉冲个数（Bit7）
    //     * 1Byte中为1表示对应计算功能已启动；为0表示对应计算功能已停止。 
    //    */
    //    /// <summary>
    //    /// 电能误差(Bit0),True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dn { get; private set; }

    //    /// <summary>
    //    /// 需量周期误差（Bit1）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Xlzq { get; private set; }

    //    /// <summary>
    //    /// 日计时误差（Bit2）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Rjs { get; private set; }
    //    /// <summary>
    //    /// 脉冲个数（Bit3）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dnzz { get; private set; }

    //    /// <summary>
    //    /// 对标（Bit4）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Db { get; private set; }
    //    /// <summary>
    //    /// 预付费（bit5),True表示对应计算功能已启动；为False表示对应计算功能已经停止
    //    /// </summary>
    //    public bool workStatusIsOn_Yff { get; private set; }
    //    /// <summary>
    //    /// 耐压状态（bit6)True表示对应计算功能已启动；为False表示对应计算功能已经停止
    //    /// </summary>
    //    public bool workStatusIsOn_Ny { get; private set; }

    //    /// <summary>
    //    /// 多功能脉冲计数（Bit7）,True表示对应计算功能已启动；为Flase表示对应计算功能已停止。
    //    /// </summary>
    //    public bool workStatusIsOn_Dgnmcjs { get; private set; }

    //    /// <summary>
    //    /// 系统解析帧  
    //    /// </summary>
    //    /// <param name="data"></param>
    //    ///         
    //    /*
    //    * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 检定误差类型（1Byte）+ 当前表位编号（1Byte）
    //    * +误差次数（1Byte）+ 误差值（4Bytes * 10） + 状态类型（1Byte）+ 电流回路状态（1Byte） + 电压回路状态（1Byte）
    //    * 通讯口状态（1Byte） + 检定状态（1Byte）+发送标志1+发送标志2。        注：与50H的区别为回复前10次的误差值。
    //    */
    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 20) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        buf.Get();                                  //广播标志(0xFFH)
    //        buf.Get();                                  //1 byte ListLen                  
    //        BwChannelList = buf.GetByteArray(12);       // List
    //        CheckType = (Cus_EmCheckType)buf.Get();       //检定误差类型
    //        CurBwNum = buf.Get();                       //当前表位编号
    //        wcNum = buf.Get();
    //        //10次误差值包括误差数值和脉冲两种格式
    //        wcData = new string[10];
    //        for (int i = 0; i < 10; i++)
    //        {
    //            byte tmp = buf.Get();
    //            byte[] wcdata = buf.GetByteArray(3);
    //            if (tmp == 0)                                       //脉冲
    //            {
    //                wcData[i] = get3ByteValue(wcdata, 0).ToString();   //脉冲个数
    //            }
    //            else                                                //误差数值
    //            {
    //                openTimes = ReplaceTargetBit(tmp, 7, false);    //误差放大倍数
    //                bool isz = GetbitValue(tmp, 7) == 0;            //误差符号（Bit7）0正误差 1负误差
    //                wcData[i] = get3ByteValue(wcdata, 5).ToString();
    //                if (!isz) wcData[i] = "-" + wcData[i];
    //            }
    //        }


    //        #region 解析状态类型,为true则表示该表位有故障/跳闸/报警/对标完成，为false则表示正常/正常/正常/未完成对标
    //        byte statusType = buf.Get();
    //        //接线故障状态,为true则表示该表位有故障,false为正常
    //        statusTypeIsOnErr_Jxgz = GetbitValue(statusType, 0) == 1;
    //        //预付费跳闸状态,为true则表示该表位跳闸,false为正常
    //        statusTypeIsOnErr_Yfftz = GetbitValue(statusType, 1) == 1;
    //        //报警信号状态,为true则表示该表位报警,false为正常
    //        statusTypeIsOnErr_Bjxh = GetbitValue(statusType, 2) == 1;
    //        //对标状态,为true则表示该表位对标完成,false为未完成对标
    //        statusTypeIsOnOver_Db = GetbitValue(statusType, 3) == 1;
    //        // 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器        
    //        statusTypeIsOnErr_Temp = GetbitValue(statusType, 4) == 1;
    //        //光电信号状态（false：未挂表；true：已挂表）        
    //        statusTypeIsOn_HaveMeter = GetbitValue(statusType, 5) == 1;
    //        //表位上限限位状态（false：未就位；true：就位）
    //        statusTypeIsOn_PressUpLimit = GetbitValue(statusType, 6) == 1;
    //        //表位下限限位状态（false：未就位；true：就位）
    //        statusTypeIsOn_PressDownLimt = GetbitValue(statusType, 7) == 1;
    //        #endregion

    //        iType = (Cus_EmBothIRoadType)buf.Get();           //电流回路状态
    //        vType = (Cus_EmBothVRoadType)buf.Get();           //电压回路状态
    //        ConnType = buf.Get();                           //通讯口状态

    //        #region 解析工作状态字节,1表示对应计算功能已启动；为0表示对应计算功能已停止。
    //        //工作状态（1Byte）
    //        byte workStatus = buf.Get();
    //        //电能误差
    //        workStatusIsOn_Dn = GetbitValue(workStatus, 0) == 1;
    //        //需量周期误差
    //        workStatusIsOn_Xlzq = GetbitValue(workStatus, 1) == 1;
    //        //日计时误差
    //        workStatusIsOn_Rjs = GetbitValue(workStatus, 2) == 1;
    //        //电能走字
    //        workStatusIsOn_Dnzz = GetbitValue(workStatus, 3) == 1;
    //        //对标
    //        workStatusIsOn_Db = GetbitValue(workStatus, 4) == 1;
    //        //预付费
    //        workStatusIsOn_Yff = GetbitValue(workStatus, 5) == 1;
    //        //耐压
    //        workStatusIsOn_Ny = GetbitValue(workStatus, 6) == 1;
    //        //光电脉冲个数
    //        workStatusIsOn_Dgnmcjs = GetbitValue(workStatus, 7) == 1;
    //        #endregion
    //    }
    //}
    //#endregion

    //#region CL88M读取相应误差板软件版本号0xC4以及回复0x54
    ///// <summary>
    ///// 读取相应误差板软件版本号
    ///// </summary>
    //internal class CL321_RequestReadWcBoardVerPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xC4;

    //    public CL321_RequestReadWcBoardVerPacket(bool[] bwstatus, int iChannelNum)
    //        : base(true)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.BwStatus = bwstatus;
    //    }

    //    public CL321_RequestReadWcBoardVerPacket(int id):base(true)
    //    {
    //        this.Pos = id;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadWcBoardVerPacket";
    //    }
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 读取误差板软件版本号
    ///// </summary>
    //internal class CL321_RequestReadWcBoardVerReplyPacket : CL321RecvPacket
    //{
    //    /// <summary>
    //    /// 命令码
    //    /// </summary>
    //    public byte bCmd { get; private set; }

    //    /// <summary>
    //    /// bytes List
    //    /// </summary>
    //    public byte[] BwChannelList { get; private set; }

    //    /// <summary>
    //    /// 误差板编号
    //    /// </summary>
    //    public int wcbIndex { get; private set; }

    //    /// <summary>
    //    /// 误差板软件版本号
    //    /// </summary>
    //    public string softVer { get; private set; }

    //    /// <summary>
    //    /// 系统解析帧  
    //    /// </summary>
    //    /// <param name="data"></param>
    //    ///         
    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) + （1 byte ListLen）  + （12 bytes List）   
    //     * +误差板编号（1 byte）+  误差板软件版本号（1 byte）。
    //     * 软件版本号的表示采用BCD编码方式编码，小数点不表示。
    //     * 	例：如果版本号为1.1，则版本号早数据帧中被表示为  0x11。
    //     */
    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data == null || data.Length < 19) return;
    //        ByteBuffer buf = new ByteBuffer(data);
    //        bCmd = buf.Get();                           //命令码
    //        if (bCmd == 0x54)
    //        {
    //            this.ReciveResult = RecvResult.OK;
    //            buf.Get();                                  //广播标志(0xFFH)
    //            buf.Get();                                  //1 byte ListLen                  
    //            BwChannelList = buf.GetByteArray(12);       // List
    //            wcbIndex = buf.Get();                       //误差板编号
    //            softVer += buf.Get().ToString("X2") + ".";
    //            softVer += buf.Get().ToString("X2") + ".";
    //            softVer += buf.Get().ToString("X2");
    //        }
    //        else
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //        }

    //    }
    //}
    //#endregion

    //#region CL321读取打印信息命令0xC6和回复命令0xC7

    //internal class CL321_RequestReadPrintInformationPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xC6;

    //    private byte modNumber = 0;

    //    public CL321_RequestReadPrintInformationPacket(byte Number)
    //        : base()
    //    {
    //        modNumber = Number;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadPrintInformationPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(modNumber);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestReadPrintInformationReplayPacket : CL321RecvPacket
    //{

    //    public byte Serial = 0;

    //    public byte DataNum = 0;

    //    public List<byte> listData = new List<byte>();
    //    public CL321_RequestReadPrintInformationReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestReadPrintInformationReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xC7)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                if (data.Length > 21)
    //                {
    //                    Serial = data[19];
    //                    DataNum = data[20];
    //                    byte[] byteData = new byte[DataNum];
    //                    if (data.Length > 20 + DataNum)
    //                    {
    //                        Array.Copy(data, 21, byteData, 0, DataNum);
    //                        listData.AddRange(byteData);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }
    //    }
    //}

    //#endregion

    //#region CL321扩展查询误差板当前误差命令0xC8和回复命令0xC9

    //internal class CL321_RequestQueryCurrentErrorPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xC8;


    //    /// <summary>
    //    /// 检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验
    //    /// </summary>
    //    private Cus_EmCheckType checkType;

    //    public CL321_RequestQueryCurrentErrorPacket(Cus_EmCheckType Type)
    //        : base()
    //    {
    //        checkType = Type;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryCurrentErrorPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put((byte)checkType);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryCurrentErrorReplayPacket : CL321RecvPacket
    //{
    //}

    //#endregion

    //#region CL321隔离继电器过载动作可靠性检测时间读取命令0xCA和回复命令0xCB

    //internal class CL321_RequestIsolationrelayOverloadReliabilityPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xCA;

    //    public CL321_RequestIsolationrelayOverloadReliabilityPacket()
    //        : base()
    //    { }



    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestIsolationrelayOverloadReliabilityPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestIsolationrelayOverloadReliabilityReplayPacket : CL321RecvPacket
    //{

    //    public byte MeterNumber = 0;

    //    public byte detectionTime = 0;
    //    public CL321_RequestIsolationrelayOverloadReliabilityReplayPacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestIsolationrelayOverloadReliabilityReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xCB)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                if (data.Length > 20)
    //                {
    //                    MeterNumber = data[19];
    //                    detectionTime = data[20];
    //                }
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }
    //    }
    //}

    //#endregion 

    //#region CL321隔离继电器过载动作可靠性检测时间设置命令0xCC和回复命令0xCD

    //internal class CL321_RequestIsolationrelayOverloadReliabilityTimePacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xCC;

    //    private byte m_Time = 0;

    //    public CL321_RequestIsolationrelayOverloadReliabilityTimePacket(byte time)
    //        : base()
    //    {
    //        m_Time = time;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestIsolationrelayOverloadReliabilityTimePacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(m_Time);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestIsolationrelayOverloadReliabilityTimeReplayPacket : CL321RecvPacket
    //{
    //    public CL321_RequestIsolationrelayOverloadReliabilityTimeReplayPacket()
    //        : base()
    //    {

    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestIsolationrelayOverloadReliabilityTimeReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xCD)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            byte[] byteInfor = new byte[4];
    //            Array.Copy(data, 15, byteInfor, 0, 4);
    //            int idata = BitConverter.ToInt32(byteInfor, 0);
    //            if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //            }
    //            else
    //            {
    //                this.ReciveResult = RecvResult.DataError;
    //            }
    //        }
    //    }
    //}

    //#endregion

    //#region CL321设置电能误差检定时脉冲参数0xF1以及回复0xF0
    ///// <summary>
    ///// 设置电能误差检定时脉冲参数
    ///// </summary>
    //internal class CL321_RequestSetPulseParaPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xF1;

    //    /// <summary>
    //    /// 标准脉冲常数
    //    /// </summary>
    //    private int stdMeterConst = 0;

    //    /// <summary>
    //    /// 标准脉冲频率
    //    /// </summary>
    //    private int stdPulseFreq = 0;

    //    /// <summary>
    //    /// 标准脉冲常数缩放倍数
    //    /// </summary>
    //    private int stdMeterConstShortTime = 0;

    //    /// <summary>
    //    /// 被检脉冲常数
    //    /// </summary>
    //    private int meterConst = 0;

    //    /// <summary>
    //    /// 校验圈数
    //    /// </summary>
    //    private int meterQuans = 0;

    //    /// <summary>
    //    /// 被检脉冲常数缩放倍数
    //    /// </summary>
    //    private int meterConstShortTime = 0;

    //    /// <summary>
    //    /// 发送标志
    //    /// </summary>
    //    private byte sendFlag = 0xAA;

    //    public CL321_RequestSetPulseParaPacket()
    //        : base(true)
    //    { }

    //    /// <summary>
    //    /// 设置参数
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="stdmeterconst">标准脉冲常数</param>
    //    /// <param name="stdpulsefreq">标准脉冲频率</param>
    //    /// <param name="stdmeterconstshorttime">标准脉冲常数缩放倍数</param>
    //    /// <param name="meterconst">被检脉冲常数</param>
    //    /// <param name="meterquans">校验圈数</param>
    //    /// <param name="meterconstshorttime">被检脉冲常数缩放倍数</param>
    //    public void SetPara(bool[] bwstatus, int stdmeterconst, int stdpulsefreq, int stdmeterconstshorttime, int meterconst, int meterquans, int meterconstshorttime, int iChannelNum, int iChannelNo)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.ChannelNo = iChannelNo;
    //        stdMeterConst = stdmeterconst;
    //        stdPulseFreq = stdpulsefreq;
    //        stdMeterConstShortTime = stdmeterconstshorttime;
    //        meterConst = meterconst;
    //        meterQuans = meterquans;
    //        meterConstShortTime = meterconstshorttime;
    //        this.Pos = 0;
    //        this.BwStatus = bwstatus;
    //    }

    //    public void SetPara(int id, int stdmeterconst, int stdpulsefreq, int stdmeterconstshorttime, int meterconst, int meterquans, int meterconstshorttime)
    //    {
    //        this.Pos = id;
    //        stdMeterConst = stdmeterconst;
    //        stdPulseFreq = stdpulsefreq;
    //        stdMeterConstShortTime = stdmeterconstshorttime;
    //        meterConst = meterconst;
    //        meterQuans = meterquans;
    //        meterConstShortTime = meterconstshorttime;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetPulseParaPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 标准脉冲常数（4Bytes）+ 
    //     * 标准脉冲频率（4Bytes）+ 标准脉冲常数缩放倍数（1Bytes）+ 被检脉冲常数（4Bytes） + 校验圈数（4Bytes）+ 被检脉冲常数缩放倍数(1Byte)+发送标志1（1Byte） 。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.PutInt_S(stdMeterConst);
    //        buf.PutInt_S(stdPulseFreq);
    //        buf.Put(Convert.ToByte(stdMeterConstShortTime));
    //        buf.PutInt_S(meterConst);
    //        buf.PutInt_S(meterQuans);
    //        buf.Put(Convert.ToByte(meterConstShortTime));
    //        buf.Put(Convert.ToByte(sendFlag));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 设置电能误差检定时脉冲参数指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSetPulseParaReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetPulseParaReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetPulseParaReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xF0)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321查询电能误差检定时脉冲参数命令0xF6以及回复0xF7
    //internal class CL321_RequestQueryPulseParamPacket:CL321SendPacket
    //{
    //    private byte m_Cmd = 0xF6;

    //    public CL321_RequestQueryPulseParamPacket():base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryPulseParamPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryPulseParamReplayPacket : ClouRecvPacket_CLT11
    //{

    //    /// <summary>
    //    /// 标准脉冲常数
    //    /// </summary>
    //    public int stdMeterConst = 0;

    //    /// <summary>
    //    /// 标准脉冲频率
    //    /// </summary>
    //    public int stdPulseFreq = 0;

    //    /// <summary>
    //    /// 标准脉冲常数缩放倍数
    //    /// </summary>
    //    public int stdMeterConstShortTime = 0;

    //    /// <summary>
    //    /// 被检脉冲常数
    //    /// </summary>
    //    public int meterConst = 0;

    //    /// <summary>
    //    /// 校验圈数
    //    /// </summary>
    //    public int meterQuans = 0;

    //    /// <summary>
    //    /// 被检脉冲常数缩放倍数
    //    /// </summary>
    //    public int meterConstShortTime = 0;

    //    public CL321_RequestQueryPulseParamReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryPulseParamReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xF7)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 18)
    //        {
    //            //byte[] byteInfor = new byte[4];
    //            //Array.Copy(data, 15, byteInfor, 0, 4);
    //            //int idata = BitConverter.ToInt32(byteInfor, 0);
    //            //if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                if (data.Length > 32)
    //                {
    //                    //翻倍后的标准表脉冲常数
    //                    byte[] bytesTmp = new byte[4];
    //                    Array.Copy(data,15,bytesTmp,0,4);
    //                    stdMeterConst = BitConverter.ToInt32(bytesTmp, 0);
    //                    //保留数据4个字节调过
    //                     //标准表常数缩放倍数
    //                    stdMeterConstShortTime = data[23];
    //                    //翻倍后的被检脉冲常数
    //                    meterConst = BitConverter.ToInt32(data, 24);
    //                    //校验圈数
    //                    meterQuans = BitConverter.ToInt32(data, 28);
    //                    //被检脉冲常数缩放倍数
    //                    meterConstShortTime = data[32];
    //                }
    //                else
    //                {
    //                    this.ReciveResult = RecvResult.DataError;
    //                }
    //            }

    //        }
    //    }
    //}
    //#endregion

    //#region CL321设置日计时误差检定时钟频率及需量周期误差检定时间0xF3以及回复0xF2
    ///// <summary>
    ///// 设置日计时误差检定时钟频率及需量周期误差检定时间
    ///// </summary>
    //internal class CL321_RequestSetTimePluseNumAndXLTimePacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xF3;

    //    /// <summary>
    //    /// 标准时钟频率100倍（4Bytes）
    //    /// </summary>
    //    private int stdMeterTimeFreq = 0;

    //    /// <summary>
    //    /// 被检时钟频率100倍
    //    /// </summary>
    //    private int meterTimeFreq = 0;

    //    /// <summary>
    //    /// 被检脉冲个数（4Bytes）
    //    /// </summary>
    //    private int meterPulseNum = 0;

    //    /// <summary>
    //    /// 发送标志
    //    /// </summary>
    //    private byte sendFlag = 0x55;

    //    public CL321_RequestSetTimePluseNumAndXLTimePacket()
    //        : base(false)
    //    { }

    //    /// <summary>
    //    /// 设置参数
    //    /// </summary>
    //    /// <param name="stdmetertimefreq">标准时钟频率（4Bytes）</param>
    //    /// <param name="metertimefreq">被检时钟频率</param>
    //    /// <param name="meterpulsenum">被检脉冲个数(4Bytes)</param>
    //    public void SetPara(bool[] bwstatus, int stdmetertimefreq, int metertimefreq, int meterpulsenum, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        IsNeedReturn = false;
    //        BwStatus = bwstatus;
    //        stdMeterTimeFreq = stdmetertimefreq * 100;
    //        meterTimeFreq = metertimefreq * 100;
    //        meterPulseNum = meterpulsenum;
    //    }

    //    public void SetPara(int id, int stdmetertimefreq, int metertimefreq, int meterpulsenum)
    //    {
    //        this.Pos = id;
    //        IsNeedReturn = false;
    //        stdMeterTimeFreq = stdmetertimefreq * 100;
    //        meterTimeFreq = metertimefreq * 100;
    //        meterPulseNum = meterpulsenum;
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetTimePluseNumAndXLTimePacket";
    //    }

    //    /*
    //     *Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List） + 标准时钟频率100倍（4Bytes）+ 被检时钟频率100倍（4Bytes）+ 被检脉冲个数（4Bytes）+发送标志2（1Byte） 
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);

    //        buf.PutInt_S(stdMeterTimeFreq);
    //        buf.PutInt_S(meterTimeFreq);
    //        buf.PutInt_S(meterPulseNum);
    //        buf.Put(Convert.ToByte(sendFlag));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// 设置电能误差检定时脉冲参数指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSetTimePluseNumAndXLTimeReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetTimePluseNumAndXLTimeReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetTimePluseNumAndXLTimeReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xF2)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321查询日计时检定时钟频率及需量周期检定时间0xF4以及回复0xF5
    //internal class CL321_RequestQueryTimePluseNumAndXLTimePacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xF4;

    //    public CL321_RequestQueryTimePluseNumAndXLTimePacket()
    //        : base()
    //    {
    //    }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryTimePluseNumAndXLTimePacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}

    //internal class CL321_RequestQueryTimePusleNumAndXLTimeReplayPacket : ClouRecvPacket_CLT11
    //{
    //    /// <summary>
    //    /// 标准时钟频率100倍（4Bytes）
    //    /// </summary>
    //    public int stdMeterTimeFreq = 0;

    //    /// <summary>
    //    /// 被检时钟频率100倍
    //    /// </summary>
    //    public int meterTimeFreq = 0;

    //    /// <summary>
    //    /// 被检脉冲个数（4Bytes）
    //    /// </summary>
    //    public int meterPulseNum = 0;

    //    public CL321_RequestQueryTimePusleNumAndXLTimeReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryTimePusleNumAndXLTimeReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xF5)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 26)
    //        {
    //            //byte[] byteInfor = new byte[4];
    //            //Array.Copy(data, 15, byteInfor, 0, 4);
    //            //int idata = BitConverter.ToInt32(byteInfor, 0);
    //            //if (idata == 0)
    //            {
    //                this.ReciveResult = RecvResult.OK;
    //                if (data.Length > 27)
    //                {
    //                    //标准时钟频率100倍
    //                    stdMeterTimeFreq = BitConverter.ToInt32(data, 15);
    //                    //需量时间周期的100倍
    //                    meterTimeFreq = BitConverter.ToInt32(data, 19);
    //                    //被检脉冲个数
    //                    meterPulseNum = BitConverter.ToInt32(data, 23);

    //                }
    //            }

    //        }
    //        else
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //        }
    //    }
    //}

    //#endregion

    //#region CL321设置误差板耐压漏电流阀值指令0xF8以及回复0xF9
    ///// <summary>
    ///// 设置误差板耐压漏电流阀值指令
    ///// </summary>
    //internal class CL321_RequestSetWishStandCurrentLimitPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xF8;

    //    /// <summary>
    //    /// 漏电流阀值
    //    /// </summary>
    //    private float CurrentLimit;


    //    /// <summary>
    //    /// 设置误差板耐压漏电流阀值指令
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestSetWishStandCurrentLimitPacket()
    //        : base(true)
    //    {

    //    }

    //    public void SetPara(bool[] bwstatus, float iCurrentLimit, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.CurrentLimit = iCurrentLimit;
    //        this.BwStatus = bwstatus;
    //    }
    //    public void SetPara(int id, float iCurrentLimit)
    //    {
    //        this.Pos = id;
    //        this.CurrentLimit = iCurrentLimit;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetWishStandCurrentLimitPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 漏电流阀值（4Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        //byte[] Blimit = GetBytes(CurrentLimit.ToString(), 4);
    //        byte[] Blimit = BitConverter.GetBytes(Convert.ToInt32(CurrentLimit * 1000));
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Blimit);
    //        return buf.ToByteArray();
    //    }
    //    public byte[] GetBytes(string x, int index)
    //    {
    //        byte[] b = new byte[index];
    //        byte[] bytes = Encoding.ASCII.GetBytes(x);
    //        for (int i = 1; i <= bytes.Length; i++, index--)
    //        {
    //            b[i] = bytes[i - 1];
    //        }
    //        return b;
    //    }
    //}
    ///// <summary>
    ///// 设置误差板耐压漏电流阀值指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSetWishStandCurrentLimitReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetWishStandCurrentLimitReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetWishStandCurrentLimitReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xF9)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL188查询耐压实验的漏电流阈值命令0xFA以及回复0xFB

    //internal class CL321_RequestQueryWishStandCurrentLimitPacket : CL321SendPacket
    //{
    //    private byte m_Cmd = 0xFA;

    //    public CL321_RequestQueryWishStandCurrentLimitPacket()
    //        : base()
    //    {
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryWishStandCurrentLimitPacket";
    //    }

    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        return buf.ToByteArray();
    //    }
    //}


    //internal class CL321_RequestQueryWishStandCurrentLimitReplayPacket : CL321RecvPacket
    //{
    //    //表位号
    //    public byte meterNumber = 0;
    //    //电流阀值
    //    public int CurrentLimit = 0;
        
    //    public CL321_RequestQueryWishStandCurrentLimitReplayPacket()
    //        : base()
    //    { }

    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestQueryWishStandCurrentLimitReplayPacket";
    //    }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        if (data[0] != 0xFB)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            return;
    //        }
    //        //解析信息码
    //        if (data.Length > 19)
    //        {
    //            meterNumber = data[15];

    //            CurrentLimit = BitConverter.ToInt32(data, 16);
    //            this.ReciveResult = RecvResult.OK;
    //        }

    //    }
    //}

    //#endregion

    //#region CL321设置误差板电机控制命令0xFC以及回复0xFD
    ///// <summary>
    ///// 设置误差板电机控制指令
    ///// </summary>
    //internal class CL321_RequestSetElectromotorPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xFC;
    //    /// <summary>
    //    /// 电机控制类型 0，电机伸；1，电机缩；2，电机停；
    //    /// </summary>
    //    private int StatusType;
    //    /// <summary>
    //    /// 设置误差板耐压状态功能指令
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestSetElectromotorPacket()
    //        : base(true)
    //    {

    //    }

    //    public void SetPara(bool[] bwstatus, int iStatusType, int iChannelNo, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.ChannelNo = iChannelNo;
    //        this.Pos = 0;
    //        this.StatusType = iStatusType;
    //        this.BwStatus = bwstatus;
    //    }
    //    public void SetPara(int id, int iStatusType)
    //    {
    //        this.Pos = id;
    //        this.StatusType = iStatusType;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetElectromotorPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 状态类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(StatusType));
    //        return buf.ToByteArray();
    //    }
    //}

    ///// <summary>
    ///// 误差板控制电机指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSetElectromotorReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetElectromotorReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetElectromotorReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xFD)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion

    //#region CL321设置误差板耐压状态指令
    ///// <summary>
    ///// 设置误差板耐压状态功能指令
    ///// </summary>
    //internal class CL321_RequestSetWishStandStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xBA;
    //    /// <summary>
    //    /// 状态类型 0，复位状态；1，控制耐压继电器闭合状态；
    //    /// </summary>
    //    private int StatusType;
    //    /// <summary>
    //    /// 设置误差板耐压状态功能指令
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="checktype"></param>
    //    public CL321_RequestSetWishStandStatusPacket()
    //        : base(true)
    //    {

    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus"></param>
    //    /// <param name="iStatusType">状态类型 0，复位状态；1，控制耐压继电器闭合状态；</param>
    //    /// <param name="iChannelNum"></param>
    //    public void SetPara(bool[] bwstatus, int iStatusType, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Pos = 0;
    //        this.StatusType = iStatusType;
    //        this.BwStatus = bwstatus;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestSetWishStandStatusPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0XFF) +（1 byte ListLen） + （12 bytes List）+ 状态类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(StatusType));
    //        return buf.ToByteArray();
    //    }
    //}
    ///// <summary>
    ///// CL321设置耐压状态指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestSetWishStandStatusReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestSetWishStandStatusReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestLinkPacketReplay";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0xBB)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion





    //#region 不知道哪里的指令
    ///// <summary>
    ///// 序列号显示指令
    ///// </summary>
    //internal class CL321_RequestShowSerialNoPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0xA0;

    //    /// <summary>
    //    /// 序列号
    //    /// </summary>
    //    private int SerialNo = 0;

    //    /// <summary>
    //    /// 序列号显示指令
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="seperate">True为隔离，False为恢复</param>
    //    public CL321_RequestShowSerialNoPacket()
    //        : base(false)
    //    {

    //    }
    //    public void SetPara(bool[] bwstatus, int intSerialNo)
    //    {
    //        this.SerialNo = intSerialNo;
    //        this.BwStatus = bwstatus;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestShowSerialNoPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 要显示的系列号（4Bytes）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0C);
    //        if (ChannelByte == null)
    //            return ChannelByte;
    //        else
    //            buf.Put(ChannelByte);
    //        buf.PutInt_S(SerialNo);
    //        return buf.ToByteArray();
    //    }

    //}
    //#endregion

    //#region 电机互斥控制
    ///// <summary>
    ///// 电机互斥控制
    ///// </summary>
    //internal class CL321_RequestElectromotorStatusPacket : CL321SendPacket
    //{
    //    /// <summary>
    //    /// 命令代码
    //    /// </summary>
    //    private byte m_Cmd = 0x30;

    //    /// <summary>
    //    /// 0：使能互斥判断（默认）
    //    /// 1：禁能互斥判断
    //    /// </summary>
    //    private int Clear = 0;

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public CL321_RequestElectromotorStatusPacket()
    //        : base(true)
    //    {

    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="bwstatus">表位状态</param>
    //    /// <param name="seperate">True为清除，False为不改变状态</param>
    //    public void SetPara(bool[] bwstatus, bool isclear, int iChannelNum)
    //    {
    //        this.ChannelNum = iChannelNum;
    //        this.Clear = isclear ? 1 : 0;
    //        this.BwStatus = bwstatus;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestElectromotorStatusPacket";
    //    }

    //    /*
    //     * Data的组织方式为：广播标志(0xFFH) +（1 byte ListLen） + （12 bytes List） + 状态类型（1Byte）。
    //    */
    //    public override byte[] GetBody()
    //    {
    //        ByteBuffer buf = new ByteBuffer();
    //        buf.Initialize();
    //        buf.Put(m_Cmd);
    //        buf.Put(AllFlag);
    //        buf.Put(0x0c);
    //        buf.Put(ChannelByte);
    //        buf.Put(Convert.ToByte(Clear));
    //        return buf.ToByteArray();
    //    }

    //}
    ///// <summary>
    ///// 清除故障指令，返回数据包
    ///// </summary>
    //internal class CL321_RequestElectromotorStatusReplayPacket : ClouRecvPacket_CLT11
    //{
    //    public CL321_RequestElectromotorStatusReplayPacket()
    //        : base()
    //    {
    //    }

    //    /// <summary>
    //    /// 返回路数
    //    /// </summary>
    //    public byte Pos
    //    {
    //        get;
    //        private set;
    //    }
    //    public override string GetPacketName()
    //    {
    //        return "CL321_RequestElectromotorStatusReplayPacket";
    //    }

    //    /// <summary>
    //    /// 联机结果
    //    /// </summary>
    //    public bool LinkOk { get; private set; }

    //    protected override void ParseBody(byte[] data)
    //    {
    //        this.LinkOk = true;

    //        if (data[0] != 0x31)
    //        {
    //            this.ReciveResult = RecvResult.DataError;
    //            LinkOk = false;
    //            return;
    //        }
    //        //this.Pos = data[1];
    //        this.ReciveResult = RecvResult.OK;
    //    }
    //}
    //#endregion



    #region 多路误差总线控制线程
    /// <summary>
    /// CL2018误差板数据多线程获取误差数据
    /// 调用方法:
    /// ReadWcbManager readWcb=new ReadWcbManager();
    /// readWcb.WcbChannelCount=4;
    /// readWcb.WcbPerChannelBwCount=6;
    /// readWcb.bSelectBw=bSelectBw;
    /// readWcb.portNum=portNum;
    /// readWcb.m_curTaskType=m_curTaskType;
    /// readWcb.Start();
    /// WaitAllThreaWorkDone();
    /// tagError=readWcb.tagError;
    /// </summary>
    public class ReadWcbManager
    {
        #region 
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int WcbChannelCount { get; set; }

        /// <summary>
        /// 每个线程最大任务数
        /// </summary>
        public int WcbPerChannelBwCount { get; set; }

        private bool[] bwStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] BwStatus
        {
            get
            {
                return bwStatus;
            }

            set
            {
                bwStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口系列
        /// </summary>
        public StPortInfo[] portNum { get; set; }

        /// <summary>
        /// 传入参数列表，指令不同参数不同，说明：
        /// 0x14 启动遥信输出：//TODO:
        /// </summary>
        public object[] Params { private get; set; }
        #endregion

        #region 数据结构
        /// <summary>
        /// 所有误差板数据
        /// </summary>
        public stError[] tagError;
        /// <summary>
        /// 功耗数据
        /// </summary>
        public stGHPram[] tagGHPram;
        /// <summary>
        /// 温度A
        /// </summary>
        public string[][] tagTemperatureA;
        /// <summary>
        /// 温度B
        /// </summary>
        public string[][] tagTemperatureB;
        /// <summary>
        /// 温度C
        /// </summary>
        public string[][] tagTemperatureC;
        /// <summary>
        /// 每个表位的返回结果，只返回成功失败的命令用
        /// </summary>
        public RecvResult[] tagResults;
        /// <summary>
        /// 电机上延时时间
        /// </summary>
        public int[] tagUpTime;
        /// <summary>
        /// 电机下延时时间
        /// </summary>
        public int[] tagDownTime;

        #endregion
        /// <summary>
        /// 当前试验类型
        /// </summary>
        public Cus_EmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private ReadWcbThread[] workThreads = new ReadWcbThread[0];

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start()
        {
            //结束上一次的线程
            workThreads = new ReadWcbThread[WcbChannelCount];
            //初始化结构
            int sLen = BwStatus.Length;
            tagError = new stError[sLen];
            tagGHPram = new stGHPram[sLen];
            tagTemperatureA = new string[sLen][];
            tagTemperatureB = new string[sLen][];
            tagTemperatureC = new string[sLen][];
            tagResults = new RecvResult[sLen];
            tagUpTime = new int[sLen];
            tagDownTime = new int[sLen];

            for (int i = 0; i < WcbChannelCount; i++)
            {
                ReadWcbThread newThread = new ReadWcbThread()
                {
                    ThreadID = i + 1,                      //线程编号,用于线程自己推导起始位置
                    ThreadPerCount = WcbPerChannelBwCount,
                    bSelectBw = BwStatus,
                    PortNum = portNum[i],
                    m_curTaskType = m_curTaskType
                };

                newThread.bSelectBw = BwStatus;
                workThreads[i] = newThread;
                newThread.Start();
            }
            return true;
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (ReadWcbThread workthread in workThreads)
            {
                workthread.Stop();
            }
        }
        /// <summary>
        /// 所有线程完毕后，处理数据
        /// </summary>
        public void WaitAllThreaWorkDone()
        {
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }
            for (int i = 0; i < workThreads.Length; i++)
            {
                int startpos = i * WcbPerChannelBwCount;
                if (Cus_EmTaskType.误差板功耗数据 == m_curTaskType)
                {
                    Array.Copy(workThreads[i].tagGHPram, startpos, tagGHPram, startpos, WcbPerChannelBwCount);
                }
                else if (Cus_EmTaskType.读取误差板温度 == m_curTaskType)
                {
                    Array.Copy(workThreads[i].tagTemperatureA, startpos, tagTemperatureA, startpos, WcbPerChannelBwCount);
                    Array.Copy(workThreads[i].tagTemperatureB, startpos, tagTemperatureB, startpos, WcbPerChannelBwCount);
                    Array.Copy(workThreads[i].tagTemperatureC, startpos, tagTemperatureC, startpos, WcbPerChannelBwCount);
                }
                else if (Cus_EmTaskType.压接电机延时时间 == m_curTaskType)
                {
                    Array.Copy(workThreads[i].tagUpTime, startpos, tagUpTime, startpos, WcbPerChannelBwCount);
                    Array.Copy(workThreads[i].tagDownTime, startpos, tagDownTime, startpos, WcbPerChannelBwCount);
                }
                else
                {
                    Array.Copy(workThreads[i].tagError, startpos, tagError, startpos, WcbPerChannelBwCount);
                }
            }
        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (ReadWcbThread workthread in workThreads)
            {
                isAllThreaWorkDone = workthread.IsWorkFinish();
                if (!isAllThreaWorkDone) break;
            }
            return isAllThreaWorkDone;
        }

    }

    public class ReadWcbThread
    {
        #region 
        /// <summary>
        /// 当前线程
        /// </summary>
        Thread workThread = null;
        /// <summary>
        /// 当前线程执行的方法
        /// </summary>
        ThreadStart StartMethod = null;
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runFlag = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool workOverFlag = false;

        /// <summary>
        /// 每个线程个数,一路误差总线上的表位数
        /// </summary>
        public int ThreadPerCount { get; set; }

        private bool[] bSelectStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] bSelectBw
        {
            get
            {
                return bSelectStatus;
            }
            set
            {
                bSelectStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口号
        /// </summary>
        public StPortInfo PortNum { get; set; }

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public Cus_EmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID { get; set; }
        #endregion

        #region 数据结构，与ReadWcbManager一致
        /// <summary>
        /// 该端口下的误差板数据
        /// </summary>
        public stError[] tagError;
        /// <summary>
        /// 误差板功耗数据
        /// </summary>
        public stGHPram[] tagGHPram;
        /// <summary>
        /// 温度A
        /// </summary>
        public string[][] tagTemperatureA;
        /// <summary>
        /// 温度B
        /// </summary>
        public string[][] tagTemperatureB;
        /// <summary>
        /// 温度C
        /// </summary>
        public string[][] tagTemperatureC;
        /// <summary>
        /// 每个表位的返回结果，只返回成功失败的命令用
        /// </summary>
        public RecvResult[] tagResults;
        /// <summary>
        /// 电机上延时时间
        /// </summary>
        public int[] tagUpTime;
        /// <summary>
        /// 电机下延时时间
        /// </summary>
        public int[] tagDownTime;

        #endregion

        #region 线程处理
        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            runFlag = true;
            workThread.Abort();
        }

        /// <summary>
        /// 工作线程是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsWorkFinish()
        {
            return workOverFlag;
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <param name="paras"></param>
        public void Start()
        {
            if (Cus_EmTaskType.误差板功耗数据 == m_curTaskType)
            {
                workThread = new Thread(StartWorkGH);
            }
            else if (Cus_EmTaskType.读取误差板温度 == m_curTaskType)
            {
                workThread = new Thread(StartWorkTemperature);
            }
            else if (Cus_EmTaskType.压接电机延时时间 == m_curTaskType)
            {
                workThread = new Thread(StartWorkReadDelayTime);
            }
            else if (Cus_EmTaskType.AutoThreadMethod == m_curTaskType)
            {
                workThread = new Thread(StartMethod);
            }
            else
            {
                workThread = new Thread(StartWork);
            }
            workThread.Start();
        }
        #endregion

        #region 各种试验类型
        /// <summary>
        /// 读取误差，各种状态，报警等
        /// </summary>
        private void StartWork()
        {
            ////初始化标志
            //runFlag = false;
            //workOverFlag = false;
            ////调用方法
            //try
            //{
            //    //计算负载
            //    int startpos = (ThreadID - 1) * ThreadPerCount;
            //    int endpos = startpos + ThreadPerCount;
            //    CL321_RequestReadBwWcAndStatusPacket rc = new CL321_RequestReadBwWcAndStatusPacket(Const.GlobalUnit.g_CurTestType);
            //    CL321_RequestReadBwWcAndStatusReplyPacket rcback = new CL321_RequestReadBwWcAndStatusReplyPacket();
            //    tagError = new stError[bSelectBw.Length];
            //    bool[] newSelectBw = new bool[bSelectBw.Length];
            //    for (int i = startpos; i < endpos; i++)
            //    {
            //        ///假如停止试验,则跳出
            //        if (runFlag) return;

            //        //重新获取表位状态
            //        rc.Pos = i + 1;
            //        rc.ChannelNo = ThreadID - 1;
            //        rc.ChannelNum = bSelectBw.Length / ThreadPerCount;// PortNum;//TODO:
            //        rc.BwStatus = SelectOneBwChannel(newSelectBw, i); //bSelectBw
            //        //rc.BwStatus = bSelectBw;
            //        ///假如不需要检表,则跳出
            //        if (!bSelectBw[i]) continue;
            //        tagError[i].szError = "";
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagError[i].szError = rcback.wcData;
            //                tagError[i].Index = rcback.wcNum;
            //                tagError[i].MeterConst = rcback.MeterConst;
            //                tagError[i].iType = rcback.iType;
            //                tagError[i].statusTypeIsOn_HaveMeter = rcback.statusTypeIsOn_HaveMeter;
            //                tagError[i].statusTypeIsOn_PressDownLimt = rcback.statusTypeIsOn_PressDownLimt;
            //                tagError[i].statusTypeIsOn_PressUpLimit = rcback.statusTypeIsOn_PressUpLimit;
            //                tagError[i].statusTypeIsOnErr_Bjxh = rcback.statusTypeIsOnErr_Bjxh;
            //                tagError[i].statusTypeIsOnErr_Jxgz = rcback.statusTypeIsOnErr_Jxgz;
            //                tagError[i].statusTypeIsOnErr_Temp = rcback.statusTypeIsOnErr_Temp;
            //                tagError[i].statusTypeIsOnErr_Yfftz = rcback.statusTypeIsOnErr_Yfftz;
            //                tagError[i].statusTypeIsOnOver_Db = rcback.statusTypeIsOnOver_Db;
            //                tagError[i].vType = rcback.vType;
            //                tagError[i].ConnType = rcback.ConnType;
            //                tagError[i].statusReadFlog = true;

            //                tagError[i].MeterIndex = i;
            //                if (m_curTaskType == Cus_EmTaskType.需量周期)
            //                {
            //                    tagError[i].Index = (tagError[i].Index + 1) / 10;
            //                }
            //                break;
            //            }
            //            else
            //            {
            //                tagError[i].statusReadFlog = false;
            //            }
            //            Thread.Sleep(200);
            //        }
            //    }
            //}
            //catch { }
            //finally
            //{
            //    //恢复标志
            //    runFlag = true;
            //    workOverFlag = true;
            //}
        }
        /// <summary>
        /// 读取功耗参数
        /// </summary>
        private void StartWorkGH()
        {
            ////初始化标志
            //runFlag = false;
            //workOverFlag = false;
            ////调用方法
            //try
            //{
            //    //计算负载
            //    int startpos = (ThreadID - 1) * ThreadPerCount;
            //    int endpos = startpos + ThreadPerCount;
            //    CL321_RequestReadGHPramPacket rc = new CL321_RequestReadGHPramPacket();
            //    CL321_RequestReadBwGHPramReplyPacket rcback = new CL321_RequestReadBwGHPramReplyPacket();
            //    tagGHPram = new stGHPram[bSelectBw.Length];
            //    bool[] newSelectBw = new bool[bSelectBw.Length];
            //    for (int i = startpos; i < endpos; i++)
            //    {
            //        ///假如停止试验,则跳出
            //        if (runFlag) return;

            //        //重新获取表位状态
            //        rc.Pos = i + 1;
            //        rc.ChannelNo = ThreadID - 1;
            //        rc.ChannelNum = bSelectBw.Length / ThreadPerCount;// PortNum;//TODO:
            //        rc.BwStatus = SelectOneBwChannel(bSelectBw, i);

            //        ///假如不需要检表,则跳出
            //        if (!bSelectBw[i]) continue;

            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagGHPram[i].AU_Ia_or_I = rcback.AU_Ia_or_I;
            //                tagGHPram[i].BU_Ib_or_L1_U = rcback.BU_Ib_or_L1_U;
            //                tagGHPram[i].CU_Ic_or_L2_U = rcback.CU_Ic_or_L2_U;
            //                tagGHPram[i].AI_Ua = rcback.AI_Ua;
            //                tagGHPram[i].BI_Ub = rcback.BI_Ub;
            //                tagGHPram[i].CI_Uc = rcback.CI_Uc;
            //                tagGHPram[i].AU_Phia_or_Phi = rcback.AU_Phia_or_Phi;
            //                tagGHPram[i].BU_Ib_or_L1_U = rcback.BU_Ib_or_L1_U;
            //                tagGHPram[i].CU_Ic_or_L2_U = rcback.CU_Ic_or_L2_U;

            //                tagGHPram[i].MeterIndex = i;

            //                break;
            //            }
            //            Thread.Sleep(200);
            //        }
            //    }
            //}
            //catch { }
            //finally
            //{
            //    //恢复标志
            //    runFlag = true;
            //    workOverFlag = true;
            //}
        }
        /// <summary>
        /// 读取温度
        /// </summary>
        private void StartWorkTemperature()
        {
            ////初始化标志
            //runFlag = false;
            //workOverFlag = false;
            ////调用方法
            //try
            //{
            //    //计算负载
            //    int startpos = (ThreadID - 1) * ThreadPerCount;
            //    int endpos = startpos + ThreadPerCount;
            //    CL321_RequestReadBwTemperaturePacket rc = new CL321_RequestReadBwTemperaturePacket();
            //    CL321_RequestReadBwTemperatureReplyPacket rcback = new CL321_RequestReadBwTemperatureReplyPacket();
            //    tagTemperatureA = new string[bSelectBw.Length][];
            //    tagTemperatureB = new string[bSelectBw.Length][];
            //    tagTemperatureC = new string[bSelectBw.Length][];
            //    bool[] newSelectBw = new bool[bSelectBw.Length];
            //    int chinal = bSelectBw.Length / ThreadPerCount;
            //    for (int i = startpos; i < endpos; i++)
            //    {
            //        ///假如停止试验,则跳出
            //        if (runFlag) return;

            //        //重新获取表位状态
            //        rc.Pos = i + 1;
            //        rc.ChannelNo = ThreadID - 1;
            //        rc.ChannelNum = chinal;// PortNum;//TODO:
            //        rc.BwStatus = SelectOneBwChannel(bSelectBw, i);

            //        ///假如不需要检表,则跳出
            //        if (!bSelectBw[i])
            //        {

            //            continue;
            //        }
            //        //A

            //        rc.m_intReadType = 0;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagTemperatureA[i] = rcback.m_strTemperature;

            //                break;
            //            }
            //            Thread.Sleep(200);
            //        }

            //        //B
            //        rc.m_intReadType = 1;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagTemperatureB[i] = rcback.m_strTemperature;

            //                break;
            //            }
            //            Thread.Sleep(200);
            //        }

            //        //C
            //        rc.m_intReadType = 2;
            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagTemperatureC[i] = rcback.m_strTemperature;

            //                break;
            //            }
            //            Thread.Sleep(200);
            //        }

            //    }
            //}
            //catch { }
            //finally
            //{
            //    //恢复标志
            //    runFlag = true;
            //    workOverFlag = true;
            //}
        }
        /// <summary>
        /// 读取电机延时时间
        /// </summary>
        private void StartWorkReadDelayTime()
        {
            ////初始化标志
            //runFlag = false;
            //workOverFlag = false;
            ////调用方法
            //try
            //{
            //    //计算负载
            //    int startpos = (ThreadID - 1) * ThreadPerCount;
            //    int endpos = startpos + ThreadPerCount;
            //    int ChnNo = bSelectBw.Length / ThreadPerCount;
            //    CL321_ReadElectromotorTimePacket rc = new CL321_ReadElectromotorTimePacket(bSelectBw, ChnNo);
            //    CL321_ReadElectromotorTimePacketReplayPacket rcback = new CL321_ReadElectromotorTimePacketReplayPacket();
            //    tagUpTime = new int[bSelectBw.Length];
            //    tagDownTime = new int[bSelectBw.Length];
            //    bool[] newSelectBw = new bool[bSelectBw.Length];
            //    for (int i = startpos; i < endpos; i++)
            //    {
            //        ///假如停止试验,则跳出
            //        if (runFlag) return;

            //        //重新获取表位状态
            //        rc.Pos = i + 1;
            //        rc.ChannelNo = ThreadID - 1;
            //        rc.ChannelNum = ChnNo;
            //        rc.BwStatus = SelectOneBwChannel(bSelectBw, i);

            //        ///假如不需要检表,则跳出
            //        if (!bSelectBw[i]) continue;

            //        for (int j = 0; j < 3; j++)
            //        {
            //            if (SendData(PortNum, rc, rcback))
            //            {
            //                tagUpTime[i] = rcback.UpDelayTime;
            //                tagDownTime[i] = rcback.DownDelayTime;

            //                break;
            //            }
            //            Thread.Sleep(200);
            //        }

            //    }
            //}
            //catch { }
            //finally
            //{
            //    //恢复标志
            //    runFlag = true;
            //    workOverFlag = true;
            //}
        }

        #endregion

        #region 内部
        /// <summary>
        /// 发送误差板端口数据
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        private bool SendData(StPortInfo port, SendPacket sendPacket, RecvPacket recvPacket)
        {
            string portName = GetPortNameByPortNumber(port);

            return SockPool.Instance.Send(portName, sendPacket, recvPacket);
        }

        /// <summary>
        /// 根据端口号获取端口名
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="UDPorCOM">true：UDP false：COM</param>
        /// <returns>端口名</returns>
        private string GetPortNameByPortNumber(StPortInfo port)
        {
            switch (port.m_Port_Type)
            {
                case Cus_EmComType.COM:
                    return string.Format("Port_COM_{0}", port);
                case Cus_EmComType.UDP:
                    return string.Format("Port_UDP_{0}_{1}", port.m_IP, port);
                case Cus_EmComType.TCP:
                    return string.Format("Port_TCP_{0}_{1}", port.m_IP, port);
                default:
                    return string.Format("Port_UDP_{0}_{1}", port.m_IP, port);
            }

        }

        /// <summary>
        /// 切换到指定表位通道
        /// </summary>
        /// <param name="bwdata"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool[] SelectOneBwChannel(bool[] _bwdata, int index)
        {
            bool[] bwdata = new bool[_bwdata.Length];

            bwdata[index] = true;

            return bwdata;
        }
        #endregion
    }
    #endregion

    #endregion

    #region 其它
    /// <summary>
    /// 结论返回
    /// 0x4b:成功
    /// </summary>
    internal class CLNormalRequestResultReplayPacket : ClouRecvPacket_NotCltTwo
    {
        public CLNormalRequestResultReplayPacket()
            : base()
        {
        }
        /// <summary>
        /// 结论
        /// </summary>
        public virtual ReplayCode ReplayResult
        {
            get;
            private set;
        }

        public override string GetPacketName()
        {
            return "CLNormalRequestResultReplayPacket";
        }
        protected override void ParseBody(byte[] data)
        {
            if (data.Length == 2)
                ReplayResult = (ReplayCode)data[1];
            else
                ReplayResult = (ReplayCode)data[0];
        }

        public enum ReplayCode
        {
            /// <summary>
            /// CLT11返回
            /// </summary>
            CLT11OK = 0x30,
            /// <summary>
            /// 响应命令，表示“OK”
            /// </summary>
            Ok = 0x4b,
            /// <summary>
            /// 响应命令，表示出错
            /// </summary>
            Error = 0x33,
            /// <summary>
            /// 响应命令，表示系统估计还要忙若干mS
            /// </summary>
            Busy = 0x35,
            /// <summary>
            /// 误差板联机成功
            /// </summary>
            CL321inkOk = 0x36,
            /// <summary>
            /// 标准表脱机成功
            /// </summary>
            Cl311UnLinkOk = 0x37
        }
    }
    #endregion
}
