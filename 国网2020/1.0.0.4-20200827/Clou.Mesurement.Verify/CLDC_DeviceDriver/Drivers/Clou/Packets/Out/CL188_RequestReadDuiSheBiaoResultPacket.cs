using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取对标结果0x39
    /// </summary>
    internal class CL188_RequestReadDuiSheBiaoResultPacket : Cl188SendPacket
    {
       private byte m_pos = 0;
       /// <summary>
       /// 设置参数
       /// </summary>
       /// <param name="pos">表位号</param>
       public void SetPara(byte pos)
       {
           m_pos = pos;
       }

       public override string GetPacketName()
       {
           return "CL188_RequestReadDuiSheBiaoResultPacket";
       }

       protected override byte[] GetBody()
       {
           ByteBuffer buf = new ByteBuffer();
           buf.Initialize();
           buf.Put(0x39);
           buf.Put(m_pos);
           return buf.ToByteArray();
       }
    }
}
