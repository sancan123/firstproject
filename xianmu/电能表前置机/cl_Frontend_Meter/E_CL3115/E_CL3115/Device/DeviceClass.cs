using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;

namespace E_CL3115.Device
{
    #region CL3115标准表联机指令
    /// <summary>
    /// 标准表联机/脱机请求包
    /// </summary>
    internal class CL3115_RequestLinkPacket : CL3115SendPacket
    {
        public bool IsLink = true;

        public CL3115_RequestLinkPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }

        public override string GetPacketResolving()
        {
            string strResolve = "联机标准表。";
            return strResolve;
        }
    }
    /// <summary>
    /// 标准表，联机返回指令
    /// </summary>
    internal class CL3115_RequestLinkReplyPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 8)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x50)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115读取标准表常数
    /// <summary>
    /// 读取真实本机常数
    /// </summary>
    internal class CL3115_RequestReadStdMeterConstPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterConstPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x40);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "读取标准表常数。";
            return strResolve;
        }
    }
    /// <summary>
    /// 读取真实本机常数返回包
    /// </summary>
    internal class CL3115_RequestReadStdMeterConstReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterConstReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterConstReplayPacket";
        }
        /// <summary>
        /// 本机常数
        /// </summary>
        /// <returns></returns>
        public int meterConst { get; private set; }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x08) return;
            ByteBuffer buf = new ByteBuffer(data);

            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x02
            buf.Get();
            //去掉0x40
            buf.Get();

            //表常数
            meterConst = buf.GetInt_S();
            ReciveResult = RecvResult.OK;
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + meterConst.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115读取标准表信息
    /// <summary>
    /// 读取标准表信息
    /// </summary>
    internal class CL3115_RequestReadStdInfoPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdInfoPacket()
            : base()
        { }

        /*
         * 81 30 PCID 0e a0 02 3f ff 80 3f ff ff 0f CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0x80);
            buf.Put(0x3F);
            buf.Put(0xFF);
            buf.Put(0xFF);
            buf.Put(0x0F);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "读取标准表信息。";
            return strResolve;
        }
    }

    /// <summary>
    /// 读取标准表信息返回包
    /// </summary>
    internal class CL3115_RequestReadStdInfoReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdInfoReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdInfoReplayPacket";
        }
        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo PowerInfo 
        { 
            get; 
            set; 
        }

        protected override void ParseBody(byte[] data)
        {
            stStdInfo tagInfo = new stStdInfo();
            ByteBuffer buf = new ByteBuffer(data);
            if (buf.Length != 0xA4) return;
            int[] arrDot = new int[9];

            //去掉 命令字
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x3f
            buf.Get();
            //去掉0xff
            buf.Get();

            //tagInfo.Clfs = (Cus_Clfs)buf.Get();
            //tagInfo.Flip_ABC = buf.Get();
            //tagInfo.Freq = buf.GetUShort_S() / 1000F;
            ////电压档位
            //tagInfo.Scale_Ua = buf.Get();
            //tagInfo.Scale_Ub = buf.Get();
            //tagInfo.Scale_Uc = buf.Get();
            ////电流档位
            //tagInfo.Scale_Ia = buf.Get();
            //tagInfo.Scale_Ib = buf.Get();
            //tagInfo.Scale_Ic = buf.Get();
            ////小数点
            //for (int i = 0; i < arrDot.Length; i++)
            //{
            //    arrDot[i] = buf.Get();
            //}
            //电压电流


            tagInfo.Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));


            //tagInfo.Ia = get3ByteValue(buf.GetByteArray(3), arrDot[3]);
            //tagInfo.Ub = get3ByteValue(buf.GetByteArray(3), arrDot[1]);
            //tagInfo.Ib = get3ByteValue(buf.GetByteArray(3), arrDot[4]);
            //tagInfo.Uc = get3ByteValue(buf.GetByteArray(3), arrDot[2]);
            //tagInfo.Ic = get3ByteValue(buf.GetByteArray(3), arrDot[5]);
            //频率
            tagInfo.Freq = BitConverter.ToInt32(buf.GetByteArray(4), 0) / 100000;
            //过载标志
            buf.Get();
            //0x80
            buf.Get();
            //相位
            //
            //buf.GetByteArray(4);
            //0x3f
            //buf.Get();
            tagInfo.SAngle = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.Phi_Uc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ub = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ua = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ic = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ib = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.Phi_Ia = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            if (tagInfo.Phi_Ic > 0)
                tagInfo.Phi_Ic = tagInfo.Phi_Uc - tagInfo.Phi_Ic;
            else
                tagInfo.Phi_Ic = 0;
            if (tagInfo.Phi_Ib > 0)
                tagInfo.Phi_Ib = tagInfo.Phi_Ub - tagInfo.Phi_Ib;
            else
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Phi_Ia > 0)
                tagInfo.Phi_Ia = tagInfo.Phi_Ua - tagInfo.Phi_Ia;
            else
                tagInfo.Phi_Ia = 0;

            if (tagInfo.Phi_Ic < 0)
                tagInfo.Phi_Ic += 360;
            else if (tagInfo.Phi_Ic > 360)
                tagInfo.Phi_Ic -= 360;

            if (tagInfo.Phi_Ib < 0)
                tagInfo.Phi_Ib += 360;
            else if (tagInfo.Phi_Ib > 360)
                tagInfo.Phi_Ib -= 360;

            if (tagInfo.Phi_Ia < 0)
                tagInfo.Phi_Ia += 360;
            else if (tagInfo.Phi_Ia > 360)
                tagInfo.Phi_Ia -= 360;

            if (tagInfo.Ia == 0)
                tagInfo.Phi_Ia = 0;
            if (tagInfo.Ib == 0)
                tagInfo.Phi_Ib = 0;
            if (tagInfo.Ic == 0)
                tagInfo.Phi_Ic = 0;
            //0xff
            buf.Get();
            //C相 B相 A相 相角
            tagInfo.PhiAngle_C = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.PhiAngle_B = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.PhiAngle_A = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            //buf.GetByteArray(4);
            //buf.GetByteArray(4);
            //buf.GetByteArray(4);

            //C相 B相 A相 有功功率因素
            tagInfo.PowerFactor_A = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.PowerFactor_B = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.PowerFactor_C = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            //buf.GetByteArray(4);
            //buf.GetByteArray(4);
            //buf.GetByteArray(4);

            tagInfo.COS = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);
            tagInfo.SIN = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / 10000);

            //0xff
            buf.Get();

            tagInfo.Pc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Pa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.P = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.Qc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Qa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Q = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            //0x0f
            buf.Get();
            tagInfo.Sc = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sb = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.Sa = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));
            tagInfo.S = Convert.ToSingle(BitConverter.ToInt32(buf.GetByteArray(4), 0) / Math.Pow(10, 0 - GetByteFromByteArray(buf.Get())));

            tagInfo.COS = get3ByteValue(buf.GetByteArray(3), 5);
            tagInfo.SIN = get3ByteValue(buf.GetByteArray(3), 5);


            //if (Comm.GlobalUnit.Clfs == Cus_Clfs.三相三线 || Comm.GlobalUnit.Clfs == Cus_Clfs.二元件跨相90 || Comm.GlobalUnit.Clfs == Cus_Clfs.二元件跨相60 || Comm.GlobalUnit.Clfs == Cus_Clfs.三元件跨相90)
            //{
            //    tagInfo.Phi_Ia -= 30;
            //    if (tagInfo.Phi_Ia < 0) tagInfo.Phi_Ia += 360;
            //    tagInfo.Phi_Ic -= 30;
            //    if (tagInfo.Phi_Ic < 0) tagInfo.Phi_Ic += 360;
            //    tagInfo.Phi_Ua -= 30;
            //    if (tagInfo.Phi_Ua < 0) tagInfo.Phi_Ua += 360;
            //    tagInfo.Phi_Uc -= 30;
            //    if (tagInfo.Phi_Uc < 0) tagInfo.Phi_Uc += 360;
            //}
            PowerInfo = tagInfo;

            this.ReciveResult = RecvResult.OK;
        }
        public override string GetPacketResolving()
        {
            string strResolve = string.Format("返回：{0}V,{1}V,{2}V,{3}A,{4}A,{5}A,{6},{7},{8},{9},{10},{11},{12}W,{13}Var,{14}VA", PowerInfo.Ua, PowerInfo.Ub, PowerInfo.Uc, PowerInfo.Ia, PowerInfo.Ib, PowerInfo.Ic, PowerInfo.Phi_Ua, PowerInfo.Phi_Ub, PowerInfo.Phi_Uc, PowerInfo.Phi_Ia, PowerInfo.Phi_Ib, PowerInfo.Phi_Ic, PowerInfo.P, PowerInfo.Q, PowerInfo.S);
            return strResolve;
        }

        private sbyte GetByteFromByteArray(byte pArray)
        {
            string Fmt16 = Convert.ToString(pArray, 16);
            sbyte ReturnValue = (Convert.ToSByte(Fmt16, 16));
            return ReturnValue;
        }
    }
    #endregion

    #region CL3115读取标准表电能
    /// <summary>
    /// 读取标准表电能
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalNumPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 20 10 CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x20);
            buf.Put(0x10);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalNumReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterTotalNumReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterTotalNumReplayPacket";
        }
        /// <summary>
        /// 累计电能 8字节，放大10000倍，低字节先传
        /// </summary>
        /// <returns></returns>
        public float MeterTotalNum 
        { 
            get; 
            private set; 
        }


        /// <summary>
        /// 成功返回数据: 81 PCID 30 11 50 02 20 10 llE1 CS
        /// </summary>
        /// <param name="data"></param>
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x0c)
            {
                ReciveResult = RecvResult.FrameError;
                return;
            }
            ByteBuffer buf = new ByteBuffer(data);

            //去掉 命令字 50
            buf.Get();
            //去掉0x02
            buf.Get();
            //去掉0x20
            buf.Get();
            //去掉0x10
            buf.Get();
            //累计电能,放大10000倍
            float fStdMeter = BitConverter.ToInt64(buf.GetByteArray(8), 0);
            MeterTotalNum = fStdMeter / 10000;
            ReciveResult = RecvResult.OK;
        }
    }
    #endregion

    #region CL3115读取标准表累计脉冲数
    /// <summary>
    /// 读取电能累计脉冲数
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalPulseNumPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterTotalPulseNumPacket()
            : base()
        { }

        /*
         * 81 30 PCID 09 a0 02 40 80 CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x40);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能累计脉冲数
    /// </summary>
    internal class CL3115_RequestReadStdMeterTotalPulseNumReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterTotalPulseNumReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterTotalPulseNumReplayPacket";
        }
        /// <summary>
        /// 电能累计脉冲数8字节，低字节先传 ,CLT协议（UINT8）/变量定义SIN8
        /// </summary>
        /// <returns></returns>
        public long Pulsenum { get; private set; }


        /// <summary>
        /// 成功返回数据:81 PCID 30 11 50 02 40 80 llPulsenum1 CS
        /// </summary>
        /// <param name="data"></param>
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x11)
            {
                ReciveResult = RecvResult.FrameError;
                return;
            }
            ByteBuffer buf = new ByteBuffer(data);
            buf.Initialize();
            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x40
            buf.Get();
            //去掉0x80
            buf.Get();

            //累计电能,放大10000倍
            Pulsenum = buf.GetLong_S();
            ReciveResult = RecvResult.OK;

        }
    }
    #endregion

    #region CL3115读取走字数据
    /// <summary>
    /// 读取电能走字数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterZZDataPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterZZDataPacket()
            : base()
        { }

        /*
         * 81 30 PCID 0a a0 02 60 10 80 CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x60);
            buf.Put(0x10);
            buf.Put(0x80);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取电能走字数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterZZDataReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestReadStdMeterZZDataReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterZZDataReplayPacket";
        }

        /// <summary>
        /// 累计电能 放大10000倍
        /// </summary>
        /// <returns></returns>
        public long meterTotalNum { get; private set; }

        /// <summary>
        /// 电能当前脉冲累计值
        /// </summary>
        public long meterPulseNum { get; private set; }

        /*
         * 成功返回数据
         *  81 PCID 30 1a 50
         * 02 60 
         * 10 
         * 00 00 00 00 00 00 00 00 //累计电能 放大10000倍
         * 80
         * 00 00 00 00 00 00 00 00 //电能当前脉冲累计值
         * CS
         * 失败返回Cmd 33
         * 81 PCID 30 06 33 CS 
         */
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 0x1A)
            {
                ReciveResult = RecvResult.FrameError;
                return;
            }
            ByteBuffer buf = new ByteBuffer(data);
            buf.Initialize();
            //去掉 命令字 50
            buf.Get();

            //去掉0x02
            buf.Get();
            //去掉0x60
            buf.Get();
            //去掉0x10
            buf.Get();

            //累计电能
            meterTotalNum = buf.GetInt_S() / 10000;

            //去掉0x80
            buf.Get();

            meterPulseNum = buf.GetInt_S();
            ReciveResult = RecvResult.OK;

        }
    }
    #endregion

    #region CL3115读取各项电压电流谐波幅值
    /// <summary>
    /// 读取各项电压电流谐波幅值
    /// </summary>
    internal class CL3115_RequestReadStdMeterHarmonicArryPacket : CL3115SendPacket
    {
        private byte ucHPhase;
        private ushort usHAryStart;
        private byte ucHLen;
        public CL3115_RequestReadStdMeterHarmonicArryPacket()
            : base()
        {
        }
        public void SetPara(byte bHp, ushort bStart, byte bLen)
        {
            ucHPhase = bHp;
            usHAryStart = bStart;
            ucHLen = bLen;
        }
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA5);
            buf.Put(0x03);
            buf.Put(ucHPhase);
            buf.PutUShort(usHAryStart);
            buf.Put(ucHLen);
            return buf.ToByteArray();

        }

    }
    /// <summary>
    /// 返回各项电压电流谐波幅值
    /// </summary>
    internal class CL3115_RequestReadStdMeterHarmonicArryReplayPacket : CL3115RecvPacket
    {
        public float[] fHarmonicArryData;
        public CL3115_RequestReadStdMeterHarmonicArryReplayPacket()
            : base()
        {
        }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterHarmonicArryReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length < 5)
            {
                ReciveResult = RecvResult.FrameError;
                return;
            }
            ByteBuffer buf = new ByteBuffer(data);
            int iCount = data.Length - 2 / 4;
            fHarmonicArryData = new float[iCount];
            buf.Initialize();
            //去掉0x55
            buf.Get();

            //去掉0x03
            buf.Get();
            for (int i = 0; i < iCount; i++)
            {
                fHarmonicArryData[i] = buf.GetIntE1();
            }
            ReciveResult = RecvResult.OK;
        }
    }

    #endregion

    #region CL3115读取各项电压电流波形数据
    /// <summary>
    /// 读取各项电压电流波形数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterWaveformArryPacket : CL3115SendPacket
    {
        private byte ucWPhase;

        private ushort usWAryStart;

        private byte ucWLen;

        public CL3115_RequestReadStdMeterWaveformArryPacket()
            : base()
        {
        }
        /// <summary>
        /// 设置合成参数
        /// </summary>
        /// <param name="bWp">相别</param>
        /// <param name="bWaStart">起始参数</param>
        /// <param name="bLen">长度</param>
        public void SetPara(byte bWp, ushort bWaStart, byte bLen)
        {
            ucWPhase = bWp;
            usWAryStart = bWaStart;
            ucWLen = bLen;
        }


        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA5);
            buf.Put(0x03);
            buf.Put(ucWPhase);
            buf.PutUShort(usWAryStart);
            buf.Put(ucWLen);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 返回各项电压电流波形数据
    /// </summary>
    internal class CL3115_RequestReadStdMeterWaveformArryReplayPacket : CL3115RecvPacket
    {
        public float [] fWaveformData;

        public CL3115_RequestReadStdMeterWaveformArryReplayPacket()
            : base()
        {
        }
        public override string GetPacketName()
        {
            return "CL3115_RequestReadStdMeterWaveformArryReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data.Length < 3)
            {
                ReciveResult = RecvResult.FrameError;
                return;
            }
            ByteBuffer buf = new ByteBuffer(data);
            int iCount = data.Length - 2 / 2;
            fWaveformData = new float[iCount];
            buf.Initialize();
            //去掉0x55
            buf.Get();

            //去掉0x03
            buf.Get();
            for (int i = 0; i < iCount; i++)
            {
                fWaveformData[i] = Convert.ToSingle(buf.GetUShort_S());
            }
            ReciveResult = RecvResult.OK;
        }

    }

    #endregion

    #region CL3115设置标准表常数
    /// <summary>
    /// 设置标准表常数
    /// </summary>
    internal class CL3115_RequestSetStdMeterConstPacket : CL3115SendPacket
    {
        /// <summary>
        /// 本机常数，4字节，低字节先传
        /// </summary>
        private int stdMeterConst;

        public CL3115_RequestSetStdMeterConstPacket()
            : base()
        {

        }

        /// <summary>
        /// 设置本机常数
        /// </summary>
        /// <param name="meterconst">本机常数</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterConstPacket(int meterconst, bool needReplay)
            : base(needReplay)
        {
            stdMeterConst = meterconst;
        }

        public void SetPara(int meterconst)
        {
            stdMeterConst = meterconst;
        }
        /*
         * 81 30 PCID 0d a3 00 04 01 uiLocalnum CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x04);
            buf.Put(0x01);
            buf.PutInt_S(stdMeterConst);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "设置标准表常数：" + stdMeterConst.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 设置标准表常数返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterConstReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterConstReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterConstReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115设置标准表参数
    /// <summary>
    /// 设置标准表参数
    /// </summary>
    internal class CL3115_RequestSetParaPacket : CL3115SendPacket
    {
        private byte _YouGongSetData;
        private byte _ClfsSetData;
        private byte _CalcType;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetParaPacket():base()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(Cus_EmClfs _Clfs, Cus_EmPowerFangXiang glfx, int calcType,bool bAuto)
        {

            if (glfx == Cus_EmPowerFangXiang.ZXP || glfx == Cus_EmPowerFangXiang.FXP)
                _YouGongSetData = 0x00;
            else
                _YouGongSetData = 0x40;

            _CalcType = Convert.ToByte(calcType);
            if (bAuto)
            {
                switch (_Clfs)
                {
                    case Cus_EmClfs.PT4:
                        _ClfsSetData = 0x08;
                        break;
                    case Cus_EmClfs.PT3:
                        _ClfsSetData = 0x48;
                        break;
                    case Cus_EmClfs.EK90:
                        _ClfsSetData = 0x44;
                        break;
                    case Cus_EmClfs.EK60:
                        _ClfsSetData = 0x42;
                        break;
                    case Cus_EmClfs.SK90:
                        _ClfsSetData = 0x41;
                        break;
                    default:
                        _ClfsSetData = 0x08;
                        break;
                }
            }
            else
            {
                switch (_Clfs)
                {
                    case Cus_EmClfs.PT4:
                        _ClfsSetData = 0x88;
                        break;
                    case Cus_EmClfs.PT3:
                        _ClfsSetData = 0xC8;
                        break;
                    case Cus_EmClfs.EK90:
                        _ClfsSetData = 0xC4;
                        break;
                    case Cus_EmClfs.EK60:
                        _ClfsSetData = 0xC2;
                        break;
                    case Cus_EmClfs.SK90:
                        _ClfsSetData = 0xC1;
                        break;
                    default:
                        _ClfsSetData = 0x88;
                        break;
                }
            }

        }

        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x09);
            buf.Put(0x20);
            buf.Put(_ClfsSetData);
            buf.Put(0x11);
            buf.Put(_YouGongSetData);
            buf.Put(0x00);
            buf.Put(_CalcType);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "设置标准表参数，测量方式：" + _ClfsSetData.ToString() + "有功无功：" + _YouGongSetData.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 设置标准表参数返回包
    /// </summary>
    internal class CL3115_RequestSetParaReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetParaReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetParaReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115返回指令
    class CL3115_ReplyOkPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data.Length != 1)
            {
                this.ReciveResult = RecvResult.DataError;
            }
            else if (data[0] == 0x30)
            {
                this.ReciveResult = RecvResult.OK;
            }
            else
            {
                this.ReciveResult = RecvResult.DataError;
            }
        }
    }
    #endregion

    #region CL3115设置档位
    /// <summary>
    /// 设置档位
    /// </summary>
    internal class CL3115_RequestSetStdMeterDangWeiPacket : CL3115SendPacket
    {
        /// <summary>
        /// C相电压档位
        /// </summary>
        private Cus_EmStdMeterVDangWei ucUcRange;
        /// <summary>
        /// B相电压档位
        /// </summary>
        private Cus_EmStdMeterVDangWei ucUbRange;
        /// <summary>
        /// A相电压档位
        /// </summary>
        private Cus_EmStdMeterVDangWei ucUaRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_EmStdMeterIDangWei ucIcRange;
        /// <summary>
        /// B相电流档位
        /// </summary>
        private Cus_EmStdMeterIDangWei ucIbRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_EmStdMeterIDangWei ucIaRange;

        /// <summary>
        /// 通一设置档位,默认需要回复
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_EmStdMeterVDangWei uRange, Cus_EmStdMeterIDangWei iRange)
            : base()
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }
        /// <summary>
        /// 通一设置档位
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_EmStdMeterVDangWei uRange, Cus_EmStdMeterIDangWei iRange, bool needReplay)
            : base(needReplay)
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }

        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_EmStdMeterVDangWei uaRange, Cus_EmStdMeterVDangWei ubRange, Cus_EmStdMeterVDangWei ucRange, Cus_EmStdMeterIDangWei iaRange, Cus_EmStdMeterIDangWei ibRange, Cus_EmStdMeterIDangWei icRange)
            : base()
        {
            ucUaRange = uaRange;
            ucUbRange = ubRange;
            ucUcRange = ucRange;
            ucIaRange = iaRange;
            ucIbRange = ibRange;
            ucIcRange = icRange;
        }
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_EmStdMeterVDangWei uaRange, Cus_EmStdMeterVDangWei ubRange, Cus_EmStdMeterVDangWei ucRange, Cus_EmStdMeterIDangWei iaRange, Cus_EmStdMeterIDangWei ibRange, Cus_EmStdMeterIDangWei icRange, bool needReplay)
            : base(needReplay)
        {
            ucUaRange = uaRange;
            ucUbRange = ubRange;
            ucUcRange = ucRange;
            ucIaRange = iaRange;
            ucIbRange = ibRange;
            ucIcRange = icRange;
        }

        /*
         * 81 30 PCID 0F A3 02 02 3F ucUcRange ucUbRange ucUaRange ucIcRange ucIbRange ucIaRange CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put((byte)ucUcRange);
            buf.Put((byte)ucUbRange);
            buf.Put((byte)ucUaRange);
            buf.Put((byte)ucIcRange);
            buf.Put((byte)ucIbRange);
            buf.Put((byte)ucIaRange);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = string.Format("设置标准表档位：{0},{1},{2},{3},{4},{5}", ucUaRange.ToString(), ucUbRange.ToString(), ucUcRange.ToString(), ucIaRange.ToString(), ucIbRange.ToString(), ucIcRange.ToString());
            return strResolve;
        }
    }
    /// <summary>
    /// 设置标准表接线方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterDangWeiReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterDangWeiReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterDangWeiReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115设置接线方式
    /// <summary>
    /// 设置接线方式
    /// </summary>
    internal class CL3115_RequestSetStdMeterLinkTypePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterLinkTypePacket():base()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>
        /// <param name="bAuto">自动，手动</param>
        public void SetPara(Cus_EmClfs _Clfs, bool bAuto)
        {
            if (bAuto)
            {
                switch (_Clfs)
                {
                    case Cus_EmClfs.PT4:
                        _SetData = 0x08;
                        break;
                    case Cus_EmClfs.PT3:
                        _SetData = 0x48;
                        break;
                    case Cus_EmClfs.SK90:
                        _SetData = 0x44;
                        break;
                    case Cus_EmClfs.EK90:
                        _SetData = 0x42;
                        break;
                    case Cus_EmClfs.EK60:
                        _SetData = 0x41;
                        break;
                    default:
                        _SetData = 0x08;
                        break;
                }
            }
            else
            {
                switch (_Clfs)
                {
                    case Cus_EmClfs.PT4:
                        _SetData = 0x88;
                        break;
                    case Cus_EmClfs.PT3:
                        _SetData = 0xC8;
                        break;
                    case Cus_EmClfs.SK90:
                        _SetData = 0xC4;
                        break;
                    case Cus_EmClfs.EK90:
                        _SetData = 0xC2;
                        break;
                    case Cus_EmClfs.EK60:
                        _SetData = 0xC1;
                        break;
                    default:
                        _SetData = 0x88;
                        break;
                }
            }

        }

        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x01);
            buf.Put(0x20);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "设置接线方式：" + _SetData.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 设置标准表接线方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterLinkTypeReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterLinkTypeReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterLinkTypeReplayPacket";
        }


        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115设置标准表显示
    /// <summary>
    /// 置标准表界面
    /// 由于谐波数据和波形数据仅在对应界面下获取，读取谐波数据和波形数据前必须将界面切到对应界面
    /// 界面设置命令在界面切换过程中享有最高优先级，因此为不影响上位机和使用人员的正常操作
    /// 在不需读取谐波数据和波形数据后，将界面设置为清除上位机设置。
    /// </summary>
    internal class CL3115_RequestSetStdMeterScreenPacket : CL3115SendPacket
    {
        /// <summary>
        /// 标准表界面指示
        /// </summary>
        public Cus_EmStdMeterScreen meterScreen;

        /// <summary>
        /// 设置标准表界面
        /// </summary>
        /// <param name="meterscreen">标准表界面指示</param>
        public CL3115_RequestSetStdMeterScreenPacket(Cus_EmStdMeterScreen meterscreen)
            : base()
        {
            meterScreen = meterscreen;
        }


        /*
         * 81 30 PCID 0a a3 00 10 80 ucARM_Menu CS
         */
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put((byte)meterScreen);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "设置标准表显示：" + meterScreen.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 设置标准表显示返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterScreenReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterScreenReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterScreenReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115设置标准表测量方式
    /// <summary>
    /// 设置3115标准表测量方式
    /// </summary>
    internal class CL3115_RequestSetStdMeterUsE1typePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterUsE1typePacket()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(Cus_EmPowerFangXiang glfx)
        {
            if (glfx == Cus_EmPowerFangXiang.ZXP || glfx == Cus_EmPowerFangXiang.FXP)
                _SetData = 0x00;
            else
                _SetData = 0x40;
        }

        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x01);
            buf.Put(0x11);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "设置测量方式：" + _SetData.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 设置3115标准表测量方式返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterUsE1typeReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterUsE1typeReplayPacket() : base() { }
        public override string GetPacketName()
        {
            return "CL3115_RequestSetStdMeterConstReplayPacket";
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }

        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115启动标准表
    /// <summary>
    /// 请求启动标准表指令包
    /// 返回0x4b成功
    /// </summary>
    internal class CL3115_RequestStartTaskPacket : CL3115SendPacket
    {
        /// <summary>
        /// 控制类型 
        /// </summary>
        /// <param name="iType"></param>
        public CL3115_RequestStartTaskPacket()
            : base()
        {

        }
        /// <summary>
        /// 控制类型 0，停止；1，开始计算电能误差；2，开始计算电能走字
        /// </summary>
        private int iControlType;

        public void SetPara(int iType)
        {
            this.iControlType = iType;
        }
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x08);
            buf.Put(0x10);
            buf.Put(Convert.ToByte(iControlType));
            return buf.ToByteArray();
        }
        public override string GetPacketResolving()
        {
            string strResolve = "启动标准表：" + iControlType.ToString();
            return strResolve;
        }
    }
    /// <summary>
    /// 控制标准表启动、停止、开始走字，返回指令
    /// </summary>
    internal class CL3115_RequestStartTaskReplyPacket : CL3115RecvPacket
    {
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }
    #endregion

    #region CL3115设置电能误差检定参数
    /// <summary>
    /// 设置电能误差检定参数
    /// </summary>
    internal class CL3115_RequestSetStdMeterCalcParamsPacket : CL3115SendPacket
    {
        //校验圈数（脉冲数）
        private int pulseNum;
        //被检表常数
        private int testConst;

        public CL3115_RequestSetStdMeterCalcParamsPacket()
            : base()
        {
        }
        public void SetPara(int iPulse,int iConst)
        {
            pulseNum = iPulse;
            testConst = iConst;
        }

        //81 30 PCID 16 a3 00 14 08 lPnum 01 llTestcnt CS
        public override byte[] GetBody()
        {

            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x00);
            buf.Put(0x14);
            buf.Put(0x08);
            buf.PutInt_S(pulseNum);
            buf.Put(0x01);
            buf.PutLong_S(testConst,8,1);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 设置电能误差检定参数返回包
    /// </summary>
    internal class CL3115_RequestSetStdMeterCalcParamsReplayPacket : CL3115RecvPacket
    {
        public CL3115_RequestSetStdMeterCalcParamsReplayPacket()
            : base()
        {
        }

        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 1)
                ReciveResult = RecvResult.DataError;
            else
            {
                if (data[0] == 0x30)
                    ReciveResult = RecvResult.OK;
                else
                    ReciveResult = RecvResult.Unknow;
            }
        }
        public override string GetPacketResolving()
        {
            string strResolve = "返回：" + ReciveResult.ToString();
            return strResolve;
        }
    }

    #endregion

    #region CL3115读取电能误差（仅CL1115主副表版本）

    /// <summary>
    /// 读取电能误差
    /// </summary>
    internal class CL3115_RequestReadStdMeterErrorPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterErrorPacket()
            : base()
        {
        }


        //81 30 PCID 09 a0 02 40 04 CS
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);
            buf.Put(0x40);
            buf.Put(0x04);
            return buf.ToByteArray();
        }


    }
    /// <summary>
    /// 电能误差返回包
    /// </summary>
    internal class CL3115_RequestReadStdMeterErrorReplayPacket : CL3115RecvPacket
    {
        public float fError = -1f;

        //81 PCID 30 0d 50 02 40 04 lErr1 CS
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 8)
                ReciveResult = RecvResult.DataError;
            else
            {
                byte[] bErr = new byte[4];
                Array.Copy(data, 4, bErr, 0, 4);
                Array.Reverse(bErr);
                fError = BitConverter.ToSingle(bErr, 0);
                ReciveResult = RecvResult.OK;
            }
        }
 
    }

    #endregion


    #region CL3115读取最近一次电能误差及误差计算次数
    /// <summary>
    /// 读取最近一次电能误差和次数
    /// </summary>
    internal class CL3115_RequestReadStdMeterLastErrorPacket : CL3115SendPacket
    {
        public CL3115_RequestReadStdMeterLastErrorPacket()
            : base()
        {
        }
        //81 30 PCID 0C a0 00 04 20 02 40 04 CS
        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x00);
            buf.Put(0x04);
            buf.Put(0x20);
            buf.Put(0x02);
            buf.Put(0x40);
            buf.Put(0x04);
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 读取最近一次电能误差和次数返回包
    /// </summary>
    internal class CL3115_RequestReadStdMeterLastErrorReplayPacket : CL3115RecvPacket
    {
        //误差
        public float fError = -1f;
        //次数
        public int iNumber = -1;

        //81 PCID 30 11 50 00 04 20 E_Num 02 40 04 lErr1 CS
        protected override void ParseBody(byte[] data)
        {
            if (data == null || data.Length != 12)
                ReciveResult = RecvResult.DataError;
            else
            {
                iNumber = Convert.ToInt32(data[4]);

                byte[] bErr = new byte[4];
                Array.Copy(data, 8, bErr, 0, 4);
                Array.Reverse(bErr);
                fError = BitConverter.ToSingle(bErr, 0);
                ReciveResult = RecvResult.OK;
            }
        }
    }

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
