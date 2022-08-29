using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;

namespace E_CL3112.Device
{
    #region CL3112标准表
    /// <summary>
    /// 读取标准表信息
    /// </summary>
    internal class CL3112RequestReadStdInfoPacket : CL3112SendPacket
    {
        public CL3112RequestReadStdInfoPacket()
            : base()
        {
        }

        public override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA0);          //命令 
            buf.Put(0x02);          //页
            buf.Put(0x3F);          //组
            buf.Put(0xFF);          //0
            buf.Put(0x80);          //1
            buf.Put(0x3F);          //2
            buf.Put(0xFF);          //3
            buf.Put(0xFF);          //4
            buf.Put(0x0F);          //5
            return buf.ToByteArray();
        }
    }
    /// <summary>
    /// 返回标准表信息
    /// </summary>
    internal class CL3112RequestReadStdInfoReplayPacket : CL3112RecvPacket
    {
        /// <summary>
        /// 获取源信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo PowerInfo { get; private set; }

        public CL3112RequestReadStdInfoReplayPacket()
            : base()
        { }
        public override string ToString()
        {
            return "CL3112RequestReadStdInfoReplayPacket";
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
            buf.GetByteArray(4);
            //0x3f
            buf.Get();
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
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

            //C相 B相 A相 有功功率因素
            buf.GetByteArray(4);
            buf.GetByteArray(4);
            buf.GetByteArray(4);

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

            ReciveResult = RecvResult.OK;
        }

        private sbyte GetByteFromByteArray(byte pArray)
        {
            string Fmt16 = Convert.ToString(pArray, 16);
            sbyte ReturnValue = (Convert.ToSByte(Fmt16, 16));
            return ReturnValue;
        }
    }

    #endregion CL3112标准表


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
