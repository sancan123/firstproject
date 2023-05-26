using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;
using CLDC_DeviceDriver.Drivers.Geny.Packets.In;
using CLDC_DeviceDriver.Drivers.Geny.Packets;
using CLDC_Comm.SocketModule;

namespace CLDC_DeviceDriver.Drivers.Geny
{
    class GenyStdMeterControl
    {
        private static short Com_Power_Port = 1;
        private static string SockName;
        static GenyStdMeterControl()
        {
            Geny.Setting.Settings setting = new CLDC_DeviceDriver.Drivers.Geny.Setting.Settings();

            try
            {
                Com_Power_Port = short.Parse(setting["台体通信_串口号"]);
                SockName = "STDMETER" + Com_Power_Port;
                SockPool.Instance.AddComSock(SockName, Com_Power_Port, "19200,n,8,2",1000,100);

            }
            catch
            { }
        }

        public string GetStdMeterType()
        {
            Geny_RequestSelectDoorPacket selp = new Geny_RequestSelectDoorPacket(GenyDoorType.StdMeter);
            GenyRecvPacket recvP = new GenyRecvPacket();

            if (SockPool.Instance.Send(SockName, selp, recvP) == false || recvP.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
            {
                return "";
            }

            Geny_RequestStdMeterReadK6DDataPacket stdRead = new Geny_RequestStdMeterReadK6DDataPacket("", Geny_StandMeterDataType.Type);
            Geny_ReplyStdMeterPacket recvStd = new Geny_ReplyStdMeterPacket();

            if (SockPool.Instance.Send(SockName, stdRead, recvStd) == false || recvStd.ReciveResult != CLDC_Comm.SocketModule.Packet.RecvResult.OK)
            {
                return "";
            }
            return recvStd.ResultData;
        }
    }
}
