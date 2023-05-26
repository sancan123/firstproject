using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 启动最大需数量测试据包
    /// 返回 指令
    /// </summary>
    internal class Geny_RequestMaxXLPacket : GenySendPacket
    {

        /// <summary>
        /// 电表位号
        /// </summary>
        public byte MeterIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 标准表常数
        /// </summary>
        public string StdConst
        {
            get;
            set;
        }

        /// <summary>
        /// 需量周期时间
        /// 以 分为 单位
        /// </summary>
        public byte XuLiangTime
        {
            get;
            set;
        }

        /// <summary>
        /// 滑差次数
        /// 每个需量周期 内统计的计数
        /// </summary>
        public byte HuaChaTimes
        {
            get;
            set;
        }

        public Geny_RequestMaxXLPacket()
        { }

        public Geny_RequestMaxXLPacket(byte meterIndex, string stdConst, byte xuLiangTime, byte huaChaTimes)
            : base(meterIndex, 0x19)
        {
            this.MeterIndex = meterIndex;
            this.StdConst = stdConst;
            this.XuLiangTime = xuLiangTime;
            this.HuaChaTimes = huaChaTimes;
        }

        /// <summary>
        /// 已重写
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            byte[] buf = new byte[16];

            SetArrayValue(buf);

            byte[] stdconstTmp = Encoding.ASCII.GetBytes(this.StdConst);
            Array.Copy(stdconstTmp, 0, buf, 0, stdconstTmp.Length);

            buf[14] = (byte)(this.XuLiangTime);
            buf[15] = (byte)(this.HuaChaTimes);

            return buf;
        }
    }
}
