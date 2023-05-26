using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{
    /// <summary>
    /// 对色标 总 
    /// </summary>
    class Geny_RequestWCBSeBiaoZPacket : GenySendPacket
    {

        /// <summary>
        /// 是否关闭
        /// </summary>
        public bool IsOff
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="isOff"></param>
        public Geny_RequestWCBSeBiaoZPacket(byte deviceID, bool isOff)
            : base(deviceID, 0x27)
        {
            this.IsOff = isOff;
        }

        protected override byte[] GetBody()
        {
            return new byte[] { (byte)(this.IsOff ? 0 : 1) };
        }
    }
}
