using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 源联机/脱机请求包
    /// </summary>
    internal class CL309_RequestLinkPacket : Cl309SendPacket
    {
        public bool IsLink = true;

        public CL309_RequestLinkPacket()
            : base()
        {}

        public override string GetPacketResolving()
        {
            return "源联机/脱机请求包，询问设备型号、版本号、产品出厂串号";
        }
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
