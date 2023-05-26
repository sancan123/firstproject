using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V90.Packets.Out
{
    /// <summary>
    /// 联机操作请求包
    /// </summary>
    class RequestLinkPacket : V90.Packets.Base
    {
        /// <summary>
        /// 是否是联机操作 T联机F脱机
        /// </summary>
        public bool IsLink = true;
        protected override void PutBody()
        {

            byte[] bytData = new byte[5]{
                0xa3,
                0x00,
                0x04,
                0x01,
                IsLink?(byte)0x01:(byte)0x00
            };
            InsertBytes(bytData); 

            //InsertInt4(false, 4); 
        }
    }
}
