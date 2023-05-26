using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置工作点请求包
    /// </summary>
    class RequestSetWorkingPointPacket : ClouSendPacket_CLT11
    {

        #region 变量声明
        /// <summary>
        /// 额定电压
        /// </summary>
        public float Un{get;set;}
        /// <summary>
        /// A相电压
        /// </summary>
        public float Ua { get; set; }
        /// <summary>
        /// B相电压
        /// </summary>
        public float Ub { get; set; }
        /// <summary>
        /// C相电压
        /// </summary>
        public float Uc { get; set; }
        /// <summary>
        /// 接线方式
        /// </summary>
        public byte LineType { get; set; }
        /// <summary>
        /// 功率因数
        /// </summary>
        public string PowerFactor { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        public float Freq { get; set; }
        /// <summary>
        /// 元件
        /// </summary>
        public byte Element { get; set; }
        /// <summary>
        /// 电流
        /// </summary>
        public float Inb { get; set; }
        /// <summary>
        /// 最大电流
        /// </summary>
        public float Inmax { get; set; }
        /// <summary>
        /// A相电流
        /// </summary>
        public float Ia { get; set; }
        /// <summary>
        /// B相电流
        /// </summary>
        public float Ib { get; set; }
        /// <summary>
        /// C相电流
        /// </summary>
        public float Ic { get; set; }
        /// <summary>
        /// 输出类型
        /// </summary>
        public byte OutType { get; set; }
        /// <summary>
        /// 感性容性
        /// </summary>
        public byte Inductive { get; set; }

        #endregion


        protected override byte[] GetBody()
        {

            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xa3);
            buf.Put(0);
            buf.Put(0xc0);
            buf.Put(0xff);

            int un = (int)Un * 10000;
            buf.PutInt_S(un);
            buf.Put(0xfc);

            int uc =(int) (Uc / Un * 10000);
            buf.PutInt_S(uc);

            int ub = (int)(Uc / Un * 10000);
            buf.PutInt_S(ub);

            int ua = (int)(Ua / Un * 10000);
            buf.PutInt_S(ua);

            buf.Put(LineType);

            buf.PutInt_S((int)(GetPowerFactor()*10000));

            buf.PutInt_S((int)Freq);
            buf.Put(Element);

            int inb = (int)Inb * 10000;
            buf.PutInt_S(inb);
            buf.Put(0xfc);

            int imax = (int)Inmax * 10000;
            buf.PutInt_S(imax);
            buf.Put(0xfc);

            int ic = (int)(Ic * Inb / Inmax * 10000);
            buf.PutInt_S(ic);

            int ib = (int)(Ib * Inb / Inmax * 10000);
            buf.PutInt_S(ib);

            int ia = (int)(Ia * Inb / Inmax * 10000);
            buf.PutInt_S(ia);

            buf.Put(1);
            buf.Put(GetInductive());

            return buf.ToByteArray();
        }

        /// <summary>
        /// 取功率因数
        /// </summary>
        /// <returns></returns>
        private float GetPowerFactor()
        { 
            return float.Parse(PowerFactor.Substring(0,PowerFactor.Length-1));
        }

        /// <summary>
        /// 取感性容性
        /// </summary>
        /// <returns></returns>
        private byte GetInductive()
        {
            if (PowerFactor.Substring(PowerFactor.Length, 1) == "C")
            {
                return (byte)1;
            }
            else
            {
                return (byte)0;
            }
        }

        public override string GetPacketName()
        {
            return "RequestSetWorkingPointPacket";
        }
    }
}
