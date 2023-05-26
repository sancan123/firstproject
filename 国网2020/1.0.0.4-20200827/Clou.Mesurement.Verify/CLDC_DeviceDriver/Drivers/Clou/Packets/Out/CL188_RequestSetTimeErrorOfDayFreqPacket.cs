using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置日计时误差频率
    /// </summary>
    internal class CL188_RequestSetTimeErrorOfDayFreqPacket : Cl188SendPacket
    {
        /// <summary>
        /// 表位号
        /// </summary>
        private byte Pos = 0;
        private int Freq = 0;
        private int PulseCount = 0;


        public CL188_RequestSetTimeErrorOfDayFreqPacket():base(false)
        {
            //不需要回复
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="pos">表位号FF为广播</param>
        /// <param name="freq">被检表时钟频率</param>
        /// <param name="pcount">被检脉冲个数</param>
        public void SetPara(byte pos, int freq, int pcount)
        {
            Pos = pos;
            Freq = freq;
            PulseCount = pcount;
        }
        public override string GetPacketName()
        {
            return "CL188_RequestSetTimeErrorOfDayFreqPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            Freq = Freq * 100;
            PulseCount *= 10;
            buf.Put(0x44);
            buf.Put(Pos);
            buf.PutInt(Freq <<8);
            buf.Position--;
            buf.PutInt(PulseCount<<8);
            buf.Position--;
            //return buf.ToByteArray();

            byte[] bytReturn = new byte[buf.ToByteArray().Length - 1];
            Array.Copy(buf.ToByteArray(), 0, bytReturn, 0, bytReturn.Length);
            return bytReturn;
        }
    }
}
