using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;

namespace E_CL311V2.Device
{
    #region 311V2标准表


    #region CL311V2连机脱机
    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL311_RequestLinkPacket : Cl311SendPacket
    {
        public bool IsLink = true;

        public CL311_RequestLinkPacket()
        {
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            if (IsLink)
                buf.Put(0x60);
            else
                buf.Put(0x66);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 标准表，联机返回指令
    /// </summary>
    internal class Cl311_RequestLinkReplyPacket : Cl311RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 2)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[1] == 0x4b)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
    }

    #endregion

    #region CL311V2 单独请求包
    /// <summary>
    /// 一个参数请求包
    /// </summary>
    internal class CL311_RequestReadDataOnlyCmdCodePacket : Cl311SendPacket
    {
        private byte m_CmdCode = 0;
        public CL311_RequestReadDataOnlyCmdCodePacket(byte cmd)
        {
            m_CmdCode = cmd;
            ToID = 0x16;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(m_CmdCode);
            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 单独请求包

    #region CL311V2读谐波数据

    internal class CL311_RequestReadHarmonicDataPacket : Cl311SendPacket
    {
        private byte m_Cmd = 0x00;

        public CL311_RequestReadHarmonicDataPacket(byte cmd)
        {
            m_Cmd = cmd;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x31);
            buf.Put(0x20);
            buf.Put(m_Cmd);
            return buf.ToByteArray();
        }
    }

    internal class CL311_RequestReadHarmonicDataReplayPacket : Cl311RecvPacket
    {
        public float [] Data
        {
            get;
            private set;
        }

        protected override void ParseBody(byte[] data)
        {
            byte[] byteTmp = new byte[4];
            Data = new float[16];
            if (data.Length % 4 == 0)
            {
                for (int i = 0; i < data.Length / 4; i++)
                {
                    Array.Copy(data, i * 4, byteTmp, 0, 4);

                    Array.Reverse(byteTmp);

                    Data[i] = BitConverter.ToSingle(byteTmp, 0);

                }
                ReciveResult = RecvResult.OK;

            }
            else
            {
                ReciveResult = RecvResult.None;
            }
        }
    }
    #endregion CL311V2读谐波数据


    #region CL311V2 读电压、电流谐波数据

    /// <summary>
    /// 读取Harmonious请求包
    /// </summary>
    internal class CL311_RequestReadHarmoniousPacket : Cl311SendPacket
    {

        private byte[] m_readtype = new byte[0];
        public byte ReadType
        {
            set
            {
                if (value == 0)
                    m_readtype = new byte[] { 0x31, 0x80, 0x09 };
                else if (value == 1)
                    m_readtype = new byte[] { 0x31, 0xa0, 0x09 };
                else if (value == 2)
                    m_readtype = new byte[] { 0x31, 0xc0, 0x09 };
                else if (value == 3)
                    m_readtype = new byte[] { 0x31, 0xe0, 0x09 };
                else if (value == 4)
                    m_readtype = new byte[] { 0x31, 0x00, 0x0a };
                else
                    m_readtype = new byte[] { 0x31, 0x20, 0x0a };
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestReadHarmoniousPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x32);
            buf.Put(m_readtype);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取Harmonious回复包
    /// </summary>
    partial class CL311_RequestReadHarmoniousReplayPacket : Cl311RecvPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadHarmoniousReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            throw new Exception();
        }
    }

    #endregion CL311V2 读电压、电流谐波数据

    #region CL311V2 读标准表常数\标准表脉冲数
    /// <summary>
    /// 35指令,0x12读取标准表常数 0x13读取标准表脉冲数
    /// </summary>
    internal class CL311_RequestReadStdMeterConstOrPulsePacket : Cl311SendPacket
    {
        private byte m_data = 0x12;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readStdConst"></param>
        public CL311_RequestReadStdMeterConstOrPulsePacket(bool readStdConst)
        {
            ToID = 0x16;
            if (readStdConst)
                m_data = 0x12;
            else
                m_data = 0x13;
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x35);
            buf.Put(m_data);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class Cl311_RequestReadStdMeterConstOrPulseReplayPacket : Cl311RecvPacket
    {

        public int Data
        {
            get;
            private set;
        }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 6)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else
            {
                byte[] datatemp = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    datatemp[i] = data[5 - i];
                }
                this.Data = DataFormart.HexStrToBin(DataFormart.byteToHexStr(datatemp));
            }
        }
    }

    #endregion CL311V2 读标准表常数\标准表脉冲数

    #region CL311V2 设置接线方式

    internal class CL311_RequestOtherWiringModePacket : Cl311SendPacket
    {
        private byte m_CmdCode = 0x00;
        public CL311_RequestOtherWiringModePacket(byte cmd)
        {
            m_CmdCode = cmd;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x64);
            buf.Put(m_CmdCode);
            return buf.ToByteArray();
        }
    }

    #endregion Cl311V2 设置接线方式

    #region CL311V2 启动标准表

    internal class CL311_RequsetStartStdMeterPacket : Cl311SendPacket
    {
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 启动标准表

    #region CL311V2 设置启动界面

    //0：功率测量		
    //1：伏安测量
    //2：相频测量
    //3：谐波测量
    //4：波形测量
    //5：校电能表
    internal class CL311_RequestSetStdMeterDisplayMode : Cl311SendPacket
    {
        public byte DisplayType
        {
            get;
            set;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x61);
            buf.Put(DisplayType);
            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 设置启动界面

    #region CL311V2 读取8路脉冲和8路误差
    /// <summary>
    /// 读取检定数据请求包
    /// </summary>
    internal class CL311_RequestReadVerifyDataPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVerifyDataPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x02);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    ///  读取检定数据回复包
    /// </summary>
    internal class CL311_RequestReadVerifyDataReplayPacket : Cl311RecvPacket
    {
        public int [] Pulses
        {
            get;
            private set;
        }

        public float[] Errors
        {
            get;
            private set;
        }

        public override string GetPacketName()
        {
            return "CL311_RequestReadVerifyDataReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length > 0)
            {

                ByteBuffer buf = new ByteBuffer(data);

                byte dentify = buf.Get();
                string str = Encoding.UTF8.GetString(data, 0, 1);
                if(data.Length > 64)
                {
                    buf.Get(data.Length -64);
                    Pulses = new int[8];
                    Errors = new float[8];
                    //八路脉冲
                    for (int i = 0; i < 8; i++)
                    {
                        Pulses[i] = buf.GetInt();
                    }
                    //八路误差
                    for (int j = 0; j < 8; j++)
                    {
                        Errors[j] = buf.GetInt();
                    }
                    ReciveResult = RecvResult.OK;
                }
                else
                {
                    ReciveResult = RecvResult.DataError;
                }
            }
        }
    }

    #endregion CL311V2 读取8路脉冲和8路误差

    #region CL311V2 读取设备版本信息
    /// <summary>
    /// 请求读取标准表版本号请求包
    /// </summary>
    internal class CL311_RequestReadVersionPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadVersionPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x20);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取标准表版本号回复包
    /// </summary>
    internal class CL311_RequestReadVersionReplayPacket : Cl311RecvPacket
    {

        public string Version = "UnKnown";

        public CL311_RequestReadVersionReplayPacket() : base() { }

        public override string GetPacketName()
        {
            return "CL311_RequestReadVersionReplayPacket";
        }
        protected override void ParseBody(byte[] data)
        {
            if (data != null && data.Length > 10)
            {
                Version = ASCIIEncoding.UTF8.GetString(data);
            }
        }
    }

    #endregion CL311V2 读取设备版本信息

    #region CL311V2 设置8路常数和校验圈数

    internal class CL311_RequestSet8PassawayConstandCirclePacket : Cl311SendPacket
    {
        //表常数
        public int MeterConst
        {
            get;
            set;
        }
        //校验圈数
        public int Circle
        {
            get;
            set;
        }


        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x03);

            for (int i = 0; i < 8; i++)
            {
                buf.PutInt_S(MeterConst);//8路常数
            }

            for (int j = 0; j < 8; j++)
            {
                buf.PutInt_S(Circle); //8路圈数
            }
            return buf.ToByteArray();
        }

    }

    #endregion CL311V2  设置8路常数和校验圈数

    #region CL311V2 读取电流量限

    internal class CL311_RequestReadCurrentMeasurePacket : Cl311SendPacket
    {
        public byte CurrentMeasure
        {
            get;
            private set;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x65);
            buf.Put(CurrentMeasure);

            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 读取电流量限

    #region CL311V2 设置标准表参数请求包
    /// <summary>
    /// 设置标准表参数请求包
    /// 返回Result包
    /// </summary>
    internal class CL311_RequestSetMeterParaPacket : Cl311SendPacket
    {
        private int m_MeterConst;
        private int m_PulseCount;
        private byte m_Lx;
        private byte m_Clfs;

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="meterconst">被检表常数</param>
        /// <param name="pulsecount">脉冲个数</param>
        /// <param name="lx"></param>
        /// <param name="clfs"></param>
        public void SetPara(int meterconst, int pulsecount, byte lx, byte clfs)
        {
            m_MeterConst = meterconst;
            m_PulseCount = pulsecount;
            m_Lx = lx;
            m_Clfs = clfs;
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetMeterParaPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x62);
            buf.Put(0x00);////62指令的类型 ，0=设置参数，1=启动，2=读数据
            buf.PutInt_S(m_MeterConst);
            buf.PutInt_S(m_PulseCount);
            buf.Put(m_Lx);
            buf.Put(m_Clfs);
            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 设置标准表参数请求包

    #region CL311V2 设置标准表常数
    /// <summary>
    /// 设置标准表常数
    /// </summary>
    internal class CL311_RequestSetStdMeterConstPacket : Cl311SendPacket
    {
        private byte m_auto = 0;
        private int m_meterconst = 0;

        public void SetPara(int meterconst, bool auto)
        {
            m_meterconst = meterconst;
            if (auto)
                m_auto = 0;
            else
                m_auto = 1;
        }
        public override string GetPacketName()
        {
            return "CL311_RequestSetStdMeterConstPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x44);
            buf.Put(m_auto);
            buf.PutInt(m_meterconst);
            return buf.ToByteArray();
        }
    }

    #endregion CL311V2 设置标准表常数

    #region CL311V2 设置电压、电流档位
    /// <summary>
    /// 设置电压/电流档位请求包
    /// </summary>
    internal class CL311_RequestSetUScalePacket : Cl311SendPacket
    {
        private float m_ua;
        private float m_ub;
        private float m_uc;
        /// <summary>
        /// 为TRUE时为设置电压
        /// </summary>
        public bool IsU = false;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="ua">A相电压档位</param>
        /// <param name="ub">B相电压档位</param>
        /// <param name="uc">C相电压档位</param>
        public void SetPara(float a, float b, float c, bool needConvert)
        {
            if (needConvert)
            {
                if (IsU)
                {
                    m_ua = GetUScaleIndex(a);
                    m_ub = GetUScaleIndex(b);
                    m_uc = GetUScaleIndex(c);
                }
                else
                {
                    m_ua = GetIScaleIndex(a);
                    m_ub = GetIScaleIndex(b);
                    m_uc = GetIScaleIndex(c);

                }
            }
            else
            {
                m_ua = a;
                m_ub = b;
                m_uc = c;
            }
        }

        public override string GetPacketName()
        {
            return "CL311_RequestSetUScalePacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x41);
            buf.Put((byte)m_ua);
            buf.Put((byte)m_ub);
            buf.Put((byte)m_uc);
            return buf.ToByteArray();
        }

        private int GetUScaleIndex(Single sng_U)
        {
            //0=1000V  1=600V 2=380V 3=220V 4=100V  5=60V 6=30V  15=自动档

            if (sng_U > 1000)           //超过1000V 则自动档
                return 15;
            else if (1000 >= sng_U && sng_U > 600)          // 1000V 档  1000---600V
                return 0;
            else if (600 >= sng_U && sng_U > 380)           // 600V 档  600---380V
                return 1;
            else if (380 >= sng_U && sng_U > 220 * 1.2)           // 380V 档  380---220V
                return 2;
            else if (220 * 1.2 >= sng_U && sng_U > 100 * 1.2)           // 220V 档  220---100V
                return 3;
            else if (100 * 1.2 >= sng_U && sng_U > 60 * 1.2)            // 100V 档  100---60V
                return 4;
            else if (60 * 1.2 >= sng_U && sng_U > 30 * 1.2)             // 60V 档  100---60V
                return 5;
            else if (30 * 1.2 >= sng_U)                           // 30V 档  100---60V
                return 6;
            else
                return 15;
        }
        private int GetIScaleIndex(Single sng_I)
        {
            //0=100A  1=50A  2=25A  3=10A 4=5A  5=2.5A  6=1A  7=0.5A 8=0.25A  9=0.1A  10=0.05A  11=0.025A  15=自动档
            if (sng_I > 120)                        //超过100A档，为自动档
                return 15;
            else if (120 >= sng_I && sng_I > 60)    //100A档范围内 120%   120---60
                return 0;
            else if (60 >= sng_I && sng_I > 30)     //50A档范围内 120%   60---30
                return 1;
            else if (30 >= sng_I && sng_I > 12)     //25A档范围内 120%   30---12
                return 2;
            else if (12 >= sng_I && sng_I > 6)      //10A档范围内 120%   12---6
                return 3;
            else if (6 >= sng_I && sng_I > 3)       //5A档范围内 120%   6---3
                return 4;
            else if (3 >= sng_I && sng_I > 1.2)       //2.5A档范围内 120%   3---1.2
                return 5;
            else if (1.2 >= sng_I && sng_I > 0.6)       //1A档范围内 120%   1.2---0.6
                return 6;
            else if (0.6 >= sng_I && sng_I > 0.3)       //0.5A档范围内 120%   0.6---0.3
                return 7;
            else if (0.3 >= sng_I && sng_I > 0.12)       //0.25A档范围内 120%   0.3---0.12
                return 8;
            else if (0.12 >= sng_I && sng_I > 0.06)       //0.1A档范围内 120%   0.12---0.06
                return 9;
            else if (0.06 >= sng_I && sng_I > 0.03)       //0.05A档范围内 120%   0.06---0.03
                return 10;
            else if (0.03 >= sng_I)                    //0.025A档范围内 120%   0.03---0
                return 11;
            else
                return 15;
        }
    }

    #endregion 设置电压、电流档位

    #region CL311V2 回复专用包

    /// <summary>
    /// 回复接收指令 公用
    /// </summary>
    internal class CL311_ReplyOkPacket : Cl311RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {

            if (data.Length != 2)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else if (data[1] == 0x4B)
            {
                this.ReciveResult = RecvResult.OK;
            }
            else
            {
                this.ReciveResult = RecvResult.DataError;
            }
        }
    }

    #endregion CL311V2 回复专用包

    #region CL311V2读取标准表信息
    /// <summary>
    /// 读取标准表参数信息
    /// </summary>
    internal class CL311_RequestReadStdParamPacket : Cl311SendPacket
    {
        public override string GetPacketName()
        {
            return "CL311_RequestReadStdParamPacket";
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0x32);
            return buf.ToByteArray();
        }
    }

    /// <summary>
    /// 读取标准表信息返回包
    /// </summary>
    internal class CL311_RequestReadStdInfoReplayPacket : Cl311RecvPacket
    {
        public CL311_RequestReadStdInfoReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL311_RequestReadStdInfoReplayPacket";
        }
        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo PowerInfo { get; private set; }

        /// <summary>
        /// 3字节转换为Float
        /// </summary>
        /// <param name="bytData"></param>
        /// <param name="dotLen"></param>
        /// <returns></returns>
        private float get3ByteValue(byte[] bytData, int dotLen)
        {
            float data = 0F;

            data = bytData[2] << 16;
            data += bytData[1] << 8;
            data += bytData[0];

            // data = bytData[2]<<16 + bytData[1] << 8 + bytData[0];
            data = (float)(data / Math.Pow(10, dotLen));
            return data;
        }


        protected override void ParseBody(byte[] data)
        {

            stStdInfo tagInfo = new stStdInfo();
            ByteBuffer buf = new ByteBuffer(data);
            if (buf.Length != 0x62) return;
            int[] arrDot = new int[9];

            //去掉 命令字
            buf.Get();

            tagInfo.Clfs = (Cus_EmClfs)buf.Get();
            tagInfo.Flip_ABC = buf.Get();
            tagInfo.Freq = buf.GetUShort_S() / 1000F;
            //电压档位
            tagInfo.Scale_Ua = buf.Get();
            tagInfo.Scale_Ub = buf.Get();
            tagInfo.Scale_Uc = buf.Get();
            //电流档位
            tagInfo.Scale_Ia = buf.Get();
            tagInfo.Scale_Ib = buf.Get();
            tagInfo.Scale_Ic = buf.Get();
            //小数点
            for (int i = 0; i < arrDot.Length; i++)
            {
                arrDot[i] = buf.Get();
            }
            //电压电流
            tagInfo.Ua = get3ByteValue(buf.GetByteArray(3), arrDot[0]);
            tagInfo.Ia = get3ByteValue(buf.GetByteArray(3), arrDot[3]);
            tagInfo.Ub = get3ByteValue(buf.GetByteArray(3), arrDot[1]);
            tagInfo.Ib = get3ByteValue(buf.GetByteArray(3), arrDot[4]);
            tagInfo.Uc = get3ByteValue(buf.GetByteArray(3), arrDot[2]);
            tagInfo.Ic = get3ByteValue(buf.GetByteArray(3), arrDot[5]);
            //相位
            tagInfo.Phi_Ua = get3ByteValue(buf.GetByteArray(3), 3);
            tagInfo.Phi_Ia = get3ByteValue(buf.GetByteArray(3), 3);
            tagInfo.Phi_Ub = get3ByteValue(buf.GetByteArray(3), 3);
            tagInfo.Phi_Ib = get3ByteValue(buf.GetByteArray(3), 3);
            tagInfo.Phi_Uc = get3ByteValue(buf.GetByteArray(3), 3);
            tagInfo.Phi_Ic = get3ByteValue(buf.GetByteArray(3), 3);
            //有功功率
            tagInfo.Pa = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.Pb = get3ByteValue(buf.GetByteArray(3), arrDot[7]);
            tagInfo.Pc = get3ByteValue(buf.GetByteArray(3), arrDot[8]);
            //无功功率
            tagInfo.Qa = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.Qb = get3ByteValue(buf.GetByteArray(3), arrDot[7]);
            tagInfo.Qc = get3ByteValue(buf.GetByteArray(3), arrDot[8]);//testtfs
            //视在功率
            tagInfo.Sa = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.Sb = get3ByteValue(buf.GetByteArray(3), arrDot[7]);
            tagInfo.Sc = get3ByteValue(buf.GetByteArray(3), arrDot[8]);
            //总有、总无、总视在、有功功率因数、无功功率因数
            tagInfo.P = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.Q = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.S = get3ByteValue(buf.GetByteArray(3), arrDot[6]);
            tagInfo.COS = get3ByteValue(buf.GetByteArray(3), 5);
            tagInfo.SIN = get3ByteValue(buf.GetByteArray(3), 5);

            PowerInfo = tagInfo;
        }


        /// <summary>
        /// 单个字节由低位向高位取值，
        /// </summary>
        /// <param name="input">单个字节</param>
        /// <param name="index">起始0,1,2..7</param>
        /// <returns></returns>
        protected int GetbitValue(byte input, int index)
        {
            int value;
            value = index > 0 ? input >> index : input;
            return value &= 1;
        }
    }


    #endregion CL311V2读取标准表信息

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
            CL188LinkOk = 0x36,
            /// <summary>
            /// 标准表脱机成功
            /// </summary>
            Cl311UnLinkOk = 0x37
        }
    }
    #endregion
}
