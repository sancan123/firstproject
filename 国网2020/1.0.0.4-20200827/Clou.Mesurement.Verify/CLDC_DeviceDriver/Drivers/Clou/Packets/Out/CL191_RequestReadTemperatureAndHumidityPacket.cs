using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 读取GPS温度湿度请求包
    /// </summary>
    internal class CL191_RequestReadTemperatureAndHumidityPacket : Cl191SendPacket
    {

        public CL191_RequestReadTemperatureAndHumidityPacket()
            : base()
        {
            ToID = 0xBF;
            MyID = 0x20;

        }
        public override string GetPacketName()
        {
            return "CL191_RequestReadTemperatureAndHumidityPacket";
        }
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            byte[] data = new byte[4] { 0xA0, 00, 03, 00 };
            buf.Put(data);
            return buf.ToByteArray();
        }
    }
}
