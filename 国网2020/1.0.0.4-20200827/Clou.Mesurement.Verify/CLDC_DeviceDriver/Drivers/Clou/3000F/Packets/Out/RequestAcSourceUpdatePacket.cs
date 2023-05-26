using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 交流源更新请求包
    /// </summary>
    class RequestAcSourceUpdatePacket : ClouSendPacket_CLT11
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
        /// <summary>
        /// 是否升源
        /// </summary>
        private bool isPowerOn=false ;
        public bool IsPowerOn 
        {   
            get 
            {
                return isPowerOn ;
            }
            set
            {
                isPowerOn = value;
            }
        }
        #endregion

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            buf.Put(0xa3);
            buf.Put(0x05);
            buf.Put(0x44);
            buf.Put(0x80);

            buf.Put(0x07);
            buf.Put(0x03);
            buf.Put(0x3f);

            if (!isPowerOn)
            {
                buf.Put(0xbf);
            }
            else
            {
                buf.Put(0x3f);
            }

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestAcSourceUpdatePacket";
        }
    }
}
