using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 交流源角度更新请求包
    /// </summary>
    class RequestAcSourceAngleUpdatePacket:ClouSendPacket_CLT11 
    {
        #region 变量声明
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
        #endregion

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x05);
            buf.Put(0x02);
            buf.Put(0x3f);

            int uc = (int)(Uc * 10000);
            buf.PutInt_S(uc);

            int ub = (int)(Ub * 10000);
            buf.PutInt_S(ub);

            int ua = (int)(Ua * 10000);
            buf.PutInt_S(ua);

            int ic = (int)(Ic * 10000);
            buf.PutInt_S(ic);

            int ib = (int)(Ib * 10000);
            buf.PutInt_S(ib);

            int ia = (int)(Ia * 10000);
            buf.PutInt_S(ia);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestAcSourceAngleUpdatePacket";
        }
    }
}
