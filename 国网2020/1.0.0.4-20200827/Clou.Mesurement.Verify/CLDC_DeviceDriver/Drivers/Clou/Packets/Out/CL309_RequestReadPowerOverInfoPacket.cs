using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取源过载信息请求包
    /// </summary>
    internal class CL309_RequestReadPowerOverInfoPacket : Cl309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestReadPowerOverInfoPacket()
            : base()
        {}

        /*
         * 81 01 PCID 08 a0 02 00 CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();                      
            buf.Put(0xC9);  //命令           
            return buf.ToByteArray();
        }
    }
}
