using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取源版本 请求包
    /// </summary>
    internal class CL309_RequestReadVerPacket : Cl309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestReadVerPacket()
            : base()
        {}

        /*
         * 81 01 PCID 06 C9 00 CS
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
